using dotnet_rpg.Dtos.CharacterSkill;
using dotnet_rpg.Services.CharacterSkillService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CharacterSkillController : Controller
{
    private readonly ICharacterSkillService _characterSkillService;

    public CharacterSkillController(ICharacterSkillService characterSkillService)
    {
        _characterSkillService = characterSkillService;
    }

    [HttpPost]
    public async Task<IActionResult> AddCharacterSkill(AddCharacterSkillDto characterSkill)
    {
      var request = await _characterSkillService.AddCharacterSkill(characterSkill);

      if (!ModelState.IsValid)
      {
          return BadRequest(ModelState);
      }

      return Ok(request);
    }


}