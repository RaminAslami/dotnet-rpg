using System.Security.Claims;
using System.Xml.Xsl;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }
    
    //get user ID
    private int GetUserId() =>
        int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

    private string GetUserRole() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
    
    private static List<Character> characters = new List<Character>
    {
        new Character(),
        new Character { Id = 1, Name = "Sam" },
        new Character
        {
            Id = 2,
            Name = "Ramin",
            HitPoints = 100,
            Defense = 100,
            Intelligence = 10,
            Class = RpgClass.Thief
        }
    };

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
        ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
    
        var dbCharacters = await _context.Characters.Include(c => c.Weapon)
            .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
            .Where(c => c.User.Id == GetUserId()).ToListAsync();


        var dbCharacters2 = GetUserRole().Equals("Admin")
            ? await _context.Characters.Include(c => c.Weapon)
                .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill).ToListAsync()
            : await _context.Characters.Include(c => c.Weapon)
                .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill).Where(c => c.User.Id == GetUserId()).ToListAsync();
        
        // var character = await _dataContext.Characters.Include(c => c.Weapon)
        //     .Include(c => c.CharacterSkills)
        //     .ThenInclude(cs => cs.Skill)
        //     .Where(c => c.User.Id
        //                 == GetUserId() &&
        //                 characterSkill.CharacterId == c.Id)
        //     .FirstOrDefaultAsync();
        
        
        serviceResponse.Data = _mapper.Map<List<GetCharacterDto>>(dbCharacters);
        return serviceResponse;

        /*
         *
         * ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
        List<Character> dbCharacters = await _context.Characters.ToListAsync();
        serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
        return serviceResponse;
         */
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacter(int id)
    {
        ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();

        try
        {
            var dbCharacter = await _context.Characters.Where(c => c.Id == id && c.User.Id == GetUserId()).FirstAsync();
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
        ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

        var character = _mapper.Map<Character>(newCharacter);
        character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

        await _context.Characters.AddAsync(character);
        await _context.SaveChangesAsync();

        // serviceResponse.Data = _mapper
        //     .Map<List<GetCharacterDto>>(_context.Characters.Where(c => c.User.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
        //
        //serviceResponse.Data = _mapper.Map<List<GetCharacterDto>>(characters);
        //serviceResponse.Data = _context.Characters.Where(c => c.User.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();

        return await GetAllCharacters();
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
        ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();

        try
        {
            //var character = characters.Where(c => c.Id == updatedCharacter.Id).FirstOrDefault();
            
            //we must include the user since EF do not know anything about it 
            
           // var character = await _context.Characters.Include(c => c.User).Where(c => c.Id == updatedCharacter.Id && c.User.Id == GetUserId()).FirstOrDefaultAsync();

            Character character = await _context.Characters.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

            
            if (character.User.Id == GetUserId())
            {

                character.Name = updatedCharacter.Name;
                character.Class = updatedCharacter.Class;
                character.Defense = updatedCharacter.Defense;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Strength = updatedCharacter.Strength;

                // _mapper.Map(updatedCharacter, character);

                _context.Characters.Update(character);
                await _context.SaveChangesAsync();

            }

            else
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Character has not been found!";
            }
            
            //better practice since we don't have to map every single property!

            serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
        ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

        try
        {
            //var character = characters.Where(c => c.Id == id).First();
            var character = await _context.Characters.Where(c => c.Id == id && c.User.Id == GetUserId()).FirstOrDefaultAsync();

            if (character != null)
            {
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<List<GetCharacterDto>>(_context.Characters.ToList());
            }
            else
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Character has not been found!";
            }
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }
}