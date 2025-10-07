using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProcedureEntities;

namespace ManageEmployee.Entities.DataMaster;

public class ProcedureData
{
    public static List<P_Procedure> datas = new()
    {
        new P_Procedure
            {
                Id = 1,
                Code = nameof(ProcedureEnum.KPI),
                Name = "KPI",
            },
        new P_Procedure
            {
                Id = 2,
                Code = nameof(ProcedureEnum.LEAVE),
                Name = "Quy trình xin nghỉ phép",
            },
        new P_Procedure
            {
                Id = 3,
                Code = nameof(ProcedureEnum.OVERTIME),
                Name = "Quy trình tăng ca",
            },
        new P_Procedure
            {
                Id = 4,
                Code = nameof(ProcedureEnum.SALARY_ADVANCE),
                Name = "Quy trình xin tăng lương",
            },
         new P_Procedure
            {
                Id = 5,
                Code = nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT),
                Name = "Quy trình lệnh sản xuất",
            },
    };
}