using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.P_ProcedureServices;
public class ProcedureStatusStepService: IProcedureStatusStepService
{
    private readonly ApplicationDbContext _context;

    public ProcedureStatusStepService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<P_ProcedureStatusStepModel>> Getter(int procedureId)
    {
        return await _context.P_ProcedureStatusSteps.Where(x => x.P_ProcedureId == procedureId)
            .Select(x => new P_ProcedureStatusStepModel
            {
                ProcedureStatusIdFrom = x.ProcedureStatusIdFrom,
                ProcedureStatusIdTo = x.ProcedureStatusIdTo,
                Order = x.Order,
                Note = x.Note,
                ProcedureConditionId = x.ProcedureConditionId,
            }).ToListAsync();
    }
    public async Task Setter(int procedureId, List<P_ProcedureStatusStepModel> form)
    {
        var stepDels = await _context.P_ProcedureStatusSteps.Where(x => x.P_ProcedureId == procedureId).ToListAsync();
        _context.P_ProcedureStatusSteps.RemoveRange(stepDels);
        await _context.SaveChangesAsync();

        var steps = new List<P_ProcedureStatusStep>();
        int i = 0;
        foreach (var item in form)
        {
            var step = new P_ProcedureStatusStep
            {
                ProcedureStatusIdFrom = item.ProcedureStatusIdFrom,
                ProcedureStatusIdTo = item.ProcedureStatusIdTo,
                Order = item.Order,
                Note = item.Note,
                IsInit = false,
                IsFinish = false,
                P_ProcedureId = procedureId,
                ProcedureConditionId = item.ProcedureConditionId,
            };

            if (item.ProcedureConditionId != null)
            {
                step.ProcedureConditionCode = (await _context.ProcedureConditions.FirstOrDefaultAsync(x => x.Id == item.ProcedureConditionId))?.Code;
            }

            //if (form.Count(x => x.ProcedureStatusIdFrom == item.ProcedureStatusIdFrom) == 1 && !form.Any(x => x.ProcedureStatusIdTo == item.ProcedureStatusIdFrom && (x.ProcedureConditionId == null || x.ProcedureConditionId == 0)))
            if (i == 0)
            {
                step.IsInit = true;
            }
            if (form.All(x => x.ProcedureStatusIdFrom != item.ProcedureStatusIdTo))
            {
                step.IsFinish = true;
            }

            steps.Add(step);
            i++;
        }
        if (steps.Count(x => x.IsInit) != 1 || steps.Count(x => x.IsFinish) < 1)
        {
            throw new ErrorException("Bạn đang thiết lập sai");
        }

        await _context.P_ProcedureStatusSteps.AddRangeAsync(steps);
        await _context.SaveChangesAsync();
    }
}
