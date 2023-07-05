using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;

    public CharacterService(IMapper mapper, DataContext context)
    {
        _context = context;
        _mapper = mapper;
    }

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

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters(int id)
    {
        ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
        var dbCharacters = await _context.Characters.Where(c => c.Id == id).ToListAsync();
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
            var dbCharacter = await _context.Characters.Where(c => c.Id == id).FirstAsync();
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

        await _context.Characters.AddAsync(character);
        await _context.SaveChangesAsync();

        serviceResponse.Data = _mapper.Map<List<GetCharacterDto>>(characters);
        return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
        ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();

        try
        {
            //var character = characters.Where(c => c.Id == updatedCharacter.Id).FirstOrDefault();
            var character = await _context.Characters.Where(c => c.Id == updatedCharacter.Id).FirstOrDefaultAsync();
            character.Name = updatedCharacter.Name;
            character.Class = updatedCharacter.Class;
            character.Defense = updatedCharacter.Defense;
            character.HitPoints = updatedCharacter.HitPoints;
            character.Intelligence = updatedCharacter.Intelligence;
            character.Strength = updatedCharacter.Strength;
            
           // _mapper.Map(updatedCharacter, character);

            _context.Characters.Update(character);
            await _context.SaveChangesAsync();

          
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
            var character = await _context.Characters.Where(c => c.Id == id).FirstAsync();

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();

            serviceResponse.Data = _mapper.Map<List<GetCharacterDto>>(_context.Characters.ToList());
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
        }

        return serviceResponse;
    }
}