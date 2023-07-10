using AutoMapper;
using Azure;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace dotnet_rpg.Services.FightService;

public class FightService : IFightService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public FightService(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
    {
        var response = new ServiceResponse<AttackResultDto>();

        try
        {
            var attacker = await _dataContext.Characters.Include(w => w.Weapon)
                .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

            var opponent = await _dataContext.Characters.Include(w => w.Weapon)
                .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

            var damage = Damage(attacker, opponent);

            if (opponent.HitPoints <= 0)
            {
                response.Message = $"{opponent.Name} has been defeated!";
            }

            _dataContext.Characters.Update(opponent);
            await _dataContext.SaveChangesAsync();

            response.Data = new AttackResultDto
            {
                Attacker = attacker.Name,
                AttackerHP = attacker.HitPoints,
                Opponent = opponent.Name,
                OpponentHP = opponent.HitPoints,
                Damage = damage
            };
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.Message;
        }

        return response;
    }

    private static int Damage(Character? attacker, Character? opponent)
    {
        int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
        //damage of weapon + random strength of the character

        damage -= new Random().Next(opponent.Defense);

        if (damage > 0)
        {
            opponent.HitPoints -= damage;
        }

        return damage;
    }

    public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
    {
        var response = new ServiceResponse<AttackResultDto>();

        try
        {
            var attacker = await _dataContext.Characters.Include(s => s.CharacterSkills)
                .ThenInclude(s => s.Skill).FirstOrDefaultAsync(a => a.Id == request.AttackerId);

            var defender = await _dataContext.Characters.Include(s => s.CharacterSkills)
                .ThenInclude(s => s.Skill).FirstOrDefaultAsync(o => o.Id == request.OpponentId);

            // var damage = attacker.CharacterSkills.Where(a => a.Skill.Id == attacker.Id)
            //     .Select(s => s.Skill.Damage);

            var characterSkill = attacker.CharacterSkills.FirstOrDefault(cs => cs.Skill.Id == request.SkillId);

            if (characterSkill == null)
            {
                response.Success = false;
                response.Message = $"{attacker.Name} doesn't know that skill!";
                return response;
            }

            var damage = SkillAttackCalculation(characterSkill, attacker, defender);

            if (defender.HitPoints <= 0)
            {
                response.Message = $"{defender.Name} has been defeated!";
            }

            response.Data = new AttackResultDto
            {
                Attacker = attacker.Name,
                AttackerHP = attacker.HitPoints,
                Opponent = defender.Name,
                OpponentHP = defender.HitPoints,
                Damage = damage
            };
        }

        catch (Exception e)
        {
            response.Message = e.Message;
            response.Success = false;
        }

        return response;
    }

    private static int SkillAttackCalculation(CharacterSkill characterSkill, Character attacker, Character? defender)
    {
        int damage = characterSkill.Skill.Damage + (new Random().Next(attacker.Intelligence));

        damage -= new Random().Next(defender.Defense);

        if (damage > 0)
        {
            defender.HitPoints -= damage;
        }

        return damage;
    }

    public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
    {
        ServiceResponse<FightResultDto> response = new ServiceResponse<FightResultDto>
        {
            Data = new FightResultDto()
        };

        try
        {
            var characters = await _dataContext.Characters.Include(w => w.Weapon)
                .Include(cs => cs.CharacterSkills).ThenInclude(s => s.Skill)
                .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();

            bool defeated = false;

            while (!defeated)
            {
                foreach (Character attacker in characters)
                {
                    var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                    var opponent = opponents[new Random().Next(opponents.Count)];
                    
                    int damage = 0;
                    string attackUsed = string.Empty;

                    bool useWeapon = new Random().Next(2) == 0;

                    if (useWeapon)
                    {
                        //useWeapon == 0 
                        attackUsed = attacker.Weapon.Name;
                        damage = Damage(attacker, opponent);

                    }
                    else
                    {
                        //use skill
                        var randomCharacterSkill = new Random().Next(attacker.CharacterSkills.Count);
                        attackUsed = attacker.CharacterSkills[randomCharacterSkill].Skill.Name;
                        damage = SkillAttackCalculation(attacker.CharacterSkills[randomCharacterSkill], opponent, attacker);
                    }
                    
                    response.Data.Log.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} " +
                                          $"with {(damage >= 0 ? damage : 0)} damage");

                    if (opponent.HitPoints <= 0)
                    {
                        defeated = true;
                        attacker.Victories++;
                        opponent.Defeats++;
                        response.Data.Log.Add($"{opponent.Name} has been defeated!");
                        response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left");
                        break;

                    }
                    
                }
                
                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;
                });
                
                _dataContext.Characters.UpdateRange(characters);
                await _dataContext.SaveChangesAsync();


            }

        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.Success = false;
        }

        return response;

    }

    public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
    {
        var characters = await _dataContext.Characters.Where(c => c.Fights > 0)
            .OrderByDescending(c => c.Victories).ThenBy(c => c.Defeats).ToListAsync();

        var response = new ServiceResponse<List<HighScoreDto>>
        {
            Data = _mapper.Map<List<HighScoreDto>>(characters)
        };


        return response;
    }
}