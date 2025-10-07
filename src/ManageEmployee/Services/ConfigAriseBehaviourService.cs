using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Helpers;
using Newtonsoft.Json;
using ManageEmployee.Services.Interfaces.Configs;
using ManageEmployee.Entities.ConfigurationEntities;
using ManageEmployee.DataTransferObject.AriseModels;

namespace ManageEmployee.Services;

public class ConfigAriseBehaviourService : IConfigAriseBehaviourService
{
    private readonly ApplicationDbContext _context;

    public ConfigAriseBehaviourService(
        ApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<List<ConfigAriseDocumentBehaviourDto>> GetAllByDocumentAsync(int documentId)
    {
        var query = await (from cadb in _context.ConfigAriseDocumentBehaviour
                           join cab in _context.ConfigAriseBehaviour on cadb.AriseBehaviourId equals cab.Id
                           where cadb.DocumentId == documentId
                           select new ConfigAriseDocumentBehaviourDto()
                           {
                               Id = cadb.Id,
                               AriseBehaviourId = cadb.AriseBehaviourId,
                               DocumentId = cadb.DocumentId,
                               NokeepDataChartOfAccount = cadb.NokeepDataChartOfAccount,
                               NokeepDataBill = cadb.NokeepDataBill,
                               NokeepDataTax = cadb.NokeepDataTax,
                               FocusLedger = cadb.FocusLedger,
                               FocusFunctions = JsonConvert.DeserializeObject<List<string>>(cadb.FocusFunctions ?? string.Empty) ?? new List<string>(),
                               ConfigAriseBehaviour = new ConfigAriseBehaviourDto()
                               {
                                   Id = cab.Id,
                                   Name = cab.Name,
                                   CodeData = cab.CodeData,
                                   Code = cab.Code,
                                   Index = cab.Index,
                                   Order = cab.Order,
                               }
                           }).ToListAsync();
        return query;
    }

    public async Task UpdateNoKeepValueAsync(int ariseBehaviourId, ConfigAriseDocumentBehaviourInputDto input)
    {
        var code = await _context.ConfigAriseBehaviour
            .Where(w => w.Id == ariseBehaviourId)
            .Select(s => s.Code).FirstOrDefaultAsync();
        if (code == null)
        {
            throw new ErrorException($"Not find configId {input.AriseBehaviourId}");
        }

        var query = await _context.ConfigAriseDocumentBehaviour.AsTracking()
                .Include(i => i.ConfigAriseBehaviour)
                .Where(w => w.ConfigAriseBehaviour.Code.StartsWith(code) && w.DocumentId == input.DocumentId)
                .ToListAsync();

        if (!query.Any())
        {
            throw new ErrorException($"Not find ConfigAriseDocumentBehaviours with configId {input.AriseBehaviourId}");
        }

        foreach (var item in query)
        {
            switch (input.Key)
            {
                case "nokeepDataChartOfAccount":
                    item.NokeepDataChartOfAccount = input.Value;
                    break;
                case "nokeepDataBill":
                    item.NokeepDataBill = input.Value;
                    break;
                case "nokeepDataTax":
                    item.NokeepDataTax = input.Value;
                    break;
                default:
                    break;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task UpdateFocusValueAsync(int ariseBehaviourId, ConfigAriseDocumentBehaviourInputDto input)
    {
        var ariseDocumentBehaviours = await _context.ConfigAriseDocumentBehaviour.AsTracking()
            .Where(w => w.DocumentId == input.DocumentId)
            .ToListAsync();
        foreach (var item in ariseDocumentBehaviours)
        {
            var functions = JsonConvert.DeserializeObject<List<string>>(item.FocusFunctions ?? string.Empty)
                            ?? new List<string>();
            if(item.AriseBehaviourId == input.AriseBehaviourId)
            {
                item.FocusLedger = true;
                functions.Add(input.Function);
            }
            else
            {
                item.FocusLedger = false;
                if (functions.Contains(input.Function))
                {
                    functions.Remove(input.Function);
                }
            }
            item.FocusFunctions = JsonConvert.SerializeObject(functions);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<ConfigAriseDocumentBehaviourDto>> PreparationAriseDocumentBehaviourAsync(int documentId)
    {
        if(!(await _context.Documents.AnyAsync(a => a.Id == documentId)))
        {
            throw new ErrorException($"Not find documentId {documentId}");
        }

        if (await _context.ConfigAriseDocumentBehaviour.AnyAsync(a => a.DocumentId == documentId))
        {
            return await GetAllByDocumentAsync(documentId);
        }
        var configAriseBehaviours = await _context.ConfigAriseBehaviour.AsNoTracking().ToListAsync();

        var configAriseDocumentBehaviours = new List<ConfigAriseDocumentBehaviour>();
        var nokeepDataChartOfAccounts = new string[] {
                "orginalVoucherNumber", "orginalBookDate",
                "referenceVoucherNumber",
                "referenceBookDate",
                "referenceFullName",
                "referenceAddress",
                "orginalCompanyName",
                "orginalAddress",
                "orginalDescription",
                "attachVoucher",
                "invoiceCode",
                "invoiceNumber",
                "invoiceSerial",
                "invoiceDate",
                "invoiceTaxCode",
                "invoiceAddress",
                "invoiceName",
                "invoiceProductItem"
        };
        var nokeepDataBills = new string[] {
                "orginalVoucherNumber", 
                "orginalBookDate",
                "referenceVoucherNumber",
                "referenceBookDate",
                "referenceFullName",
                "referenceAddress",
                "orginalCompanyName",
                "orginalAddress",
                "orginalDescription",
                "attachVoucher"
        };
        var nokeepDataTaxs = new string[] {
                "orginalBookDate",
                "referenceVoucherNumber",
                "referenceBookDate",
                "referenceFullName",
                "referenceAddress"
        };
        foreach (var item in configAriseBehaviours)
        {
            var data = new ConfigAriseDocumentBehaviour
            {
                DocumentId = documentId,
                AriseBehaviourId = item.Id,
                NokeepDataChartOfAccount = true,
                FocusLedger = false,
                NokeepDataBill = true,
                NokeepDataTax = true,
            };
       
            if(nokeepDataChartOfAccounts.Contains(item.CodeData))
            {
                if(item.Code == "orginalVoucherNumber")
                {
                    data.FocusLedger = true;
                }
                data.NokeepDataChartOfAccount = false;
            }
            if (nokeepDataBills.Contains(item.CodeData))
            {
                data.NokeepDataBill = false;
            }
            if (nokeepDataTaxs.Contains(item.CodeData))
            {
                data.NokeepDataTax = false;
            }
            configAriseDocumentBehaviours.Add(data);
        }
        await _context.ConfigAriseDocumentBehaviour.AddRangeAsync(configAriseDocumentBehaviours);
        await _context.SaveChangesAsync();
        return await GetAllByDocumentAsync(documentId);
    }
}
