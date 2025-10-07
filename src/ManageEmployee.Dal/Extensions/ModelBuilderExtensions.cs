using ManageEmployee.Dal.Configurations;
using ManageEmployee.Entities.ConfigurationEntities;
using ManageEmployee.Entities.PrintEntities;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Entities.UserEntites;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Entities.DataMaster;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SalarySocial>().HasData(SalarySocialDatas.datas);
        modelBuilder.Entity<ConfigurationView>().HasData(ConfigurationViewDatas.datas);
        modelBuilder.Entity<AccountPay>().HasData(AccountPayData.datas);
        modelBuilder.Entity<LookupValue>().HasData(LookupValuesDatas.datas);
        modelBuilder.Entity<PagePrint>().HasData(PagePrintDatas.datas);
        modelBuilder.Entity<Print>().HasData(PrintDatas.datas);
        modelBuilder.Entity<ProcedureCondition>().HasData(ProcedureConditionDatas.datas);
    }

    public static void ApplyContractorConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserToContractorConfiguration());
        modelBuilder.ApplyConfiguration(new ContractorToCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ContractorToCategoryToProductConfiguration());
    }
}