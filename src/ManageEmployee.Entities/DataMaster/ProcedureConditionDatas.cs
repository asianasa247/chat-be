using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProcedureEntities;

namespace ManageEmployee.Entities.DataMaster;

public static class ProcedureConditionDatas
{
    public static List<ProcedureCondition> datas = new()
    {
        new ProcedureCondition
        {
            Id = 1,
            Code = nameof(ProcedureOrderProduceProductConditionEnum.PriceLower),
            Name = "Kiểm tra đơn giá thấp hơn hiện tại",
            ProcedureCodes = nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT)
        },
        new ProcedureCondition
        {
            Id = 2,
            Code = nameof(ProcedureOrderProduceProductConditionEnum.SendToWarehouse),
            Name = "Gửi xuống kho",
            ProcedureCodes = nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT)
        },
        new ProcedureCondition
        {
            Id = 3,
            Code = nameof(ProcedureOrderProduceProductConditionEnum.SendToCashier),
            Name = "Gửi cho bộ phận bán hàng",
            ProcedureCodes = nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT)
        },
        new ProcedureCondition
        {
            Id = 4,
            Code = nameof(ProcedureOrderProduceProductConditionEnum.Special),
            Name = "Đơn hàng đặc biệt",
            ProcedureCodes = nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT)
        },
        new ProcedureCondition
        {
            Id = 5,
            Code = nameof(ProcedureOrderProduceProductConditionEnum.LedgerDebit),
            Name = "Thêm công nợ kế toán",
            ProcedureCodes = nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT)
        },
        new ProcedureCondition
        {
            Id = 6,
            Code = nameof(ProcedureOrderProduceProductConditionEnum.PlanningWarehouse),
            Name = "Gửi kho",
            ProcedureCodes = nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT)
        },
        new ProcedureCondition
        {
            Id = 7,
            Code = nameof(ProcedureOrderProduceProductConditionEnum.ProduceProduct),
            Name = "Gửi kho",
            ProcedureCodes = nameof(ProcedureEnum.MANUFACTURE_ORDER)
        },
        new ProcedureCondition
        {
            Id = 8,
            Code = nameof(ProcedureOrderProduceProductConditionEnum.Special),
            Name = "Đặc biệt",
            ProcedureCodes = nameof(ProcedureEnum.GATE_PASS)
        },
        new ProcedureCondition
        {
            Id = 9,
            Code = nameof(ProcedureOrderProduceProductConditionEnum.SameDepartment),
            Name = "Cùng phòng ban",
            ProcedureCodes = nameof(ProcedureEnum.REQUEST_EQUIPMENT)
        },
        new ProcedureCondition
        {
            Id = 11,
            Code = nameof(ProcedureOrderProduceProductConditionEnum.SameDepartment),
            Name = "Cùng phòng ban",
            ProcedureCodes = nameof(ProcedureEnum.ADVANCE_PAYMENT)
        },
        new ProcedureCondition
        {
            Id = 12,
            Code = nameof(ProcedureOrderProduceProductConditionEnum.SendToCashier),
            Name = "Gửi cho bộ phận bán hàng",
            ProcedureCodes = nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT)
        },
    };
}