using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Skill;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.SkillsService;

public class SkillService : ISkillService
{
    private readonly IMapper _mapper;
    private readonly DataContext _dataContext;

    public SkillService(IMapper mapper, DataContext dataContext)
    {
        _mapper = mapper;
        _dataContext = dataContext;
    }
    
    public async Task<ServiceResponse<GetSkillDto>> GetSkills(int id)
    {
        var service = new ServiceResponse<GetSkillDto>();

        try
        {
            var skill = await _dataContext.Skills.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (skill == null)
            {
                service.Message = "Skill not found";
                service.Success = false;
            }

            service.Data = _mapper.Map<GetSkillDto>(skill);


        }
        catch (Exception e)
        {
            service.Message = e.Message;
            service.Success = false;
        }

        return service;

    }

    public async Task<ServiceResponse<GetSkillDto>> AddSkill(AddSkillDto newSkill)
    {
        var response = new ServiceResponse<GetSkillDto>();

        try
        {
            var skills = _mapper.Map<Skill>(newSkill);
            
            // var skill = new Skill
            // {
            //     Name = newSkill.Name,
            //     Damage = newSkill.Damage
            // };
            
              await _dataContext.Skills.AddAsync(skills);
              await _dataContext.SaveChangesAsync();

              response.Data = _mapper.Map<GetSkillDto>(_dataContext.Skills.ToList());


        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.Success = false;
        }

        return response;
    }

    public async Task<ServiceResponse<List<GetSkillDto>>> GetAllSkills()
    {
        var response = new ServiceResponse<List<GetSkillDto>>();

        try
        {
            var skills = await _dataContext.Skills.OrderBy(s => s.Id).ToListAsync();

            var allSkills = _mapper.Map<List<GetSkillDto>>(skills);

            response.Data = allSkills;

        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.Success = false;
        }

        return response;

    }
}