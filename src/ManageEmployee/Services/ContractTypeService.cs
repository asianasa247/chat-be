using AutoMapper;
using Common.Extensions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.ContractTypeModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.ContractEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class ContractTypeService : IContractTypeService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public ContractTypeService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ContractTypeListModel>> GetAll(TypeContractEnum type)
    {
        var contracts = await _context.ContractTypes.Where(x => x.TypeContract == type).OrderBy(x => x.Name).ToListAsync();
        var contractOuts = new List<ContractTypeListModel>();
        var departments = await _context.Departments.Where(x => !x.isDelete).ToListAsync();
        foreach(var contract in contracts)
        {
            var contractOut = new ContractTypeListModel()
            {
                Id = contract.Id,
                Code = contract.Code,
                Name = contract.Name,
            };
            contractOuts.Add(contractOut);
        }
        return contractOuts;
    }

    public async Task<PagingResult<ContractType>> GetAll(ContractTypePagingRequestModel param)
    {
        if (param.PageSize <= 0)
            param.PageSize = 20;

        if (param.Page < 0)
            param.Page = 1;

        var result = new PagingResult<ContractType>()
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
        };

        var query = _context.ContractTypes
            .Where(x => param.TypeContract == null || x.TypeContract == param.TypeContract)
            .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Code.Contains(param.SearchText)
            || x.Name.Contains(param.SearchText));
        result.TotalItems = await query.CountAsync();
        result.Data = await query.OrderBy(x => x.Id).Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync();
        return result;
    }

    public async Task Create(ContractTypeModel param)
    {
        if (param.Name.IsNullOrEmpty() && param.Code.IsNullOrEmpty())
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        if (await _context.ContractTypes.AnyAsync(u => u.Code == param.Code))
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }
        var item = _mapper.Map<ContractType>(param);
        await _context.ContractTypes.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task Update(ContractTypeModel param)
    {
        if (param.Name.IsNullOrEmpty() && param.Code.IsNullOrEmpty())
            throw new ErrorException( ResultErrorConstants.MODEL_MISS);

        var data = await _context.ContractTypes.FindAsync(param.Id);
        if (data == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        var checkMemberHaveWarehoue = await _context.Users.AnyAsync(x => !x.IsDelete && x.ContractTypeId == param.Id);
        if (checkMemberHaveWarehoue)
            throw new ErrorException(ResultErrorConstants.WAREHOUSE_USER_CONTAINT);

        var isExist = await _context.ContractTypes.AnyAsync(x => x.Id != data.Id && x.Code == param.Code);
        if (isExist)
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);

        data = _mapper.Map<ContractType>(param);


        _context.ContractTypes.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var data = await _context.ContractTypes.FindAsync(id);
        if (data == null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        var checkMemberHaveWarehoue = await _context.Users.Where(x => !x.IsDelete && x.ContractTypeId == id).ToListAsync();
        if (checkMemberHaveWarehoue.Any())
            throw new ErrorException(ResultErrorConstants.CONTRACT_TYPE_USED);

        _context.ContractTypes.Remove(data);
        await _context.SaveChangesAsync();
    }

    public async Task<ContractType> GetById(int id)
    {
        var contractType = await _context.ContractTypes.FirstOrDefaultAsync(x => x.Id == id);
        if (contractType == null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        return contractType;
    }
}