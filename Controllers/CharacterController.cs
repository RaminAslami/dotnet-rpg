using System.Security.Claims;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using dotnet_rpg.Services.CharacterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : Controller
    {
        private readonly ICharacterService _characterService;
        //public ClaimsPrincipal User { get; }

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        //[AllowAnonymous] //Call method without authentication!
        [HttpGet]
        public async Task<IActionResult> Get()
        {
           // var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

           // var nameIdentifierClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
           // if (nameIdentifierClaim == null)
           // {
           //     // Handle the case where the claim is not found
           //     return BadRequest("NameIdentifier claim not found");
           // }
           //
           // if (!int.TryParse(nameIdentifierClaim.Value, out var userId))
           // {
           //     // Handle the case where parsing the user ID fails
           //     return BadRequest("Invalid user ID");
           // }

           return Ok(await _characterService.GetAllCharacters());
        }

        [HttpGet("{id}")]
        public async Task <IActionResult> GetSingleCharacter(int id)
        {

            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var getCharacter = await _characterService.GetCharacter(id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(getCharacter);
        }

        [HttpPost]
        public async Task <IActionResult> AddCharacter([FromBody] AddCharacterDto newCharacter)
        {

            await _characterService.AddCharacter(newCharacter);
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            ServiceResponse<GetCharacterDto> response = await _characterService.UpdateCharacter(updatedCharacter);

            if (response.Data == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            var response = await _characterService.DeleteCharacter(id);
          
            if (response.Data == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}