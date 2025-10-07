using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.P_ProcedureServices;
public class ProcedureConditionService: IProcedureConditionService
{
    private readonly ApplicationDbContext _context;

    public ProcedureConditionService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<ProcedureCondition>> GetList(string procedureCode)
    {
        return await _context.ProcedureConditions.Where(x => x.ProcedureCodes == procedureCode).ToListAsync();
    }
}
