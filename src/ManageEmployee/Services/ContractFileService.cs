using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.ContractTypeModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ContractEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class ContractFileService : IContractFileService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ContractFileService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ContractFileListModel>> GetAll(int contractTypeId)
    {
        var contracts = await _context.ContractFiles.Where(x => x.ContractTypeId == contractTypeId).OrderBy(x => x.Name).ToListAsync();
        var contractOuts = new List<ContractFileListModel>();
        foreach (var contract in contracts)
        {
            var contractOut = new ContractFileListModel()
            {
                Id = contract.Id,
                Code = contract.Code,
                Name = contract.Name,
                LinkFile = contract.LinkFile,
            };
            contractOuts.Add(contractOut);
        }
        return contractOuts;
    }

    public async Task<PagingResult<ContractFile>> GetAll(PagingRequestModel param)
    {
        if (param.PageSize <= 0)
            param.PageSize = 20;

        if (param.Page < 0)
            param.Page = 1;

        
        var result = new PagingResult<ContractFile>()
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
        };

        var query = _context.ContractFiles
            .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Code.Contains(param.SearchText)
            || x.Name.Contains(param.SearchText));
        result.TotalItems = await query.CountAsync();
        result.Data = await query.OrderBy(x => x.Id).Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync();
        return result;
    }

    public async Task Create(ContractFileModel param)
    {
        var item = _mapper.Map<ContractFile>(param);
        await _context.ContractFiles.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task Update(ContractFileModel param)
    {
        var data = _mapper.Map<ContractFile>(param);
        _context.ContractFiles.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var data = await _context.ContractFiles.FindAsync(id);
        if (data == null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        _context.ContractFiles.Remove(data);
        await _context.SaveChangesAsync();
    }

    public async Task<ContractFileModel> GetById(int id)
    {
        var contractFile = await _context.ContractFiles.FirstOrDefaultAsync(x => x.Id == id);
        if (contractFile == null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        return _mapper.Map<ContractFileModel>(contractFile);
    }
}