using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Skill;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.SkillsService;

public interface ISkillService
{
    Task<ServiceResponse<GetSkillDto>> GetSkills(int id);
    Task<ServiceResponse<GetSkillDto>> AddSkill(AddSkillDto newSkill);
    Task<ServiceResponse<List<GetSkillDto>>> GetAllSkills();
}