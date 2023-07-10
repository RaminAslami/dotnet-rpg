using dotnet_rpg.Dtos.Skill;
using dotnet_rpg.Services.SkillsService;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;


[ApiController]
[Route("[controller]")]
public class SkillController : Controller
{

    private readonly ISkillService _skillService;
    
    public SkillController(ISkillService skillService)
    {
        _skillService = skillService;
    }

   // [HttpGet("id")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSkill(int id)
    {
        return Ok(await _skillService.GetSkills(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSkills()
    {
        return Ok(_skillService.GetAllSkills());
    }

    [HttpPost]
    public async Task<IActionResult> AddNewSkill(AddSkillDto newSkill)
    {
        await _skillService.AddSkill(newSkill);

        return Ok(GetAllSkills());

    }
    


}