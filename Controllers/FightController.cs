using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Services.FightService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

//[Authorize]
[ApiController]
[Route("[controller]")]
public class FightController : Controller
{

    private readonly IFightService _fightService;

    public FightController(IFightService fightService)
    {
        _fightService = fightService;
    }
    
    [HttpPost("Weapon")]
    public async Task<IActionResult> WeaponAttack(WeaponAttackDto request)
    {
        return Ok(await _fightService.WeaponAttack(request));
    }

    [HttpPost("Skill")]
    public async Task<IActionResult> SkillAttack(SkillAttackDto request)
    {
        return Ok(await _fightService.SkillAttack(request));
    }

    [HttpPost]
    public async Task<IActionResult> Fight(FightRequestDto request)
    {
        return Ok(await _fightService.Fight(request));
    }

    [HttpGet]
    public async Task<IActionResult> GetHighScore()
    {
        return Ok(await _fightService.GetHighScore());
    }

}