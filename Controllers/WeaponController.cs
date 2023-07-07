using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;
using dotnet_rpg.Services.WeaponService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class WeaponController : Controller
{

    private readonly IWeaponService _weaponService;
        
    public WeaponController(IWeaponService weaponService)
    {
        _weaponService = weaponService;
    }

    [HttpPost]
    public async Task<IActionResult> AddWeapon(AddWeaponDto addWeapon)
    {
   
        return Ok(await _weaponService.AddWeapon(addWeapon));


    }

}