using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.P_ProcedureServices;

public class ProcedureStatusService : IProcedureStatusService
{
    private readonly ApplicationDbContext _context;

    public ProcedureStatusService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<P_ProcedureStatus>> GetStatus(int procedureId)
    {
        return await _context.P_ProcedureStatus.Where(x => x.P_ProcedureId == procedureId).ToListAsync();
    }

    public async Task<IEnumerable<P_ProcedureStatus>> GetStatusForFilter(string pocedureCode)
    {
        var procedure = await _context.P_Procedure.FirstOrDefaultAsync(x => x.Code == pocedureCode);
        if (procedure is null)
        {
            throw new Exception("Not find procedure");
        }
        return await _context.P_ProcedureStatus.Where(x => x.P_ProcedureId == procedure.Id).ToListAsync();
    }
}