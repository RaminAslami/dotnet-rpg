using System.Security.Claims;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.CharacterSkill;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterSkillService;

public class CharacterSkillService : ICharacterSkillService
{

    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CharacterSkillService(DataContext dataContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _dataContext = dataContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }
    
    private int GetUserId() =>
        int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));


    public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto characterSkill)
    {
        var response = new ServiceResponse<GetCharacterDto>();

        try
        {
            var character = await _dataContext.Characters.Include(c => c.Weapon)
                .Include(c => c.CharacterSkills)
                .ThenInclude(cs => cs.Skill)
                .Where(c => c.User.Id
                                                               == GetUserId() &&
                                                               characterSkill.CharacterId == c.Id)
                .FirstOrDefaultAsync();


            if (character == null)
            {
                response.Success = false;
                response.Message = "Character not found.";
                return response;
            }

            var skill = await _dataContext.Skills.FirstOrDefaultAsync(s => s.Id == characterSkill.SkillId);

            if (skill == null)
            {
                response.Success = false;
                response.Message = "Skill not found";
                return response;
            }

            CharacterSkill characterSkills = new CharacterSkill
            {
                Character = character,
                Skill = skill
            };
            
            await _dataContext.AddAsync(characterSkills);
            await _dataContext.SaveChangesAsync();

            response.Data = _mapper.Map<GetCharacterDto>(character);



        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.Success = false;

        }

        return response;
    }

 
}