using System.Security.Claims;
using AutoMapper;
using Azure;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.WeaponService;

public class WeaponService : IWeaponService
{

    private readonly DataContext _dataContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public WeaponService(DataContext dataContext, IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _dataContext = dataContext;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User
        .FindFirstValue(ClaimTypes.NameIdentifier));
    
    public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto addWeapon)
    {
        var serviceResponse = new ServiceResponse<GetCharacterDto>();

        try
        {
           /* var currentCharacter = await _dataContext.Characters.FirstOrDefaultAsync(c => c.Id
                                                                      == addWeapon.CharacterId
                                                                      && c.User.Id == GetUserId());
                                                                      */
           Character character = await _dataContext.Characters
               .FirstOrDefaultAsync(c => c.Id == addWeapon.CharacterId &&
                                         c.User.Id == int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
           
            if (character != null)
            {
                var weapon = new Weapon
                {
                    Name = addWeapon.Name,
                    Damage = addWeapon.Damage,
                    CharacterId = addWeapon.CharacterId
                };
                
                await _dataContext.Weapons.AddAsync(weapon);
                await _dataContext.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);

            }
            else
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Character not found.";
                return serviceResponse;
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