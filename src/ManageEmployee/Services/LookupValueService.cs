using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces.LookupValues;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class LookupValueService: ILookupValueService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public LookupValueService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<List<LookupValue>> GetLookupValues(string scope)
    {
        var values =  await _context.LookupValues.Where(x => x.Scope == scope).ToListAsync();
        return values;
    }
}