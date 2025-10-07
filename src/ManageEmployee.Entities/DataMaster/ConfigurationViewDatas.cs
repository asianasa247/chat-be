using ManageEmployee.Entities.ConfigurationEntities;

namespace ManageEmployee.Entities.DataMaster;

public class ConfigurationViewDatas
{
    public static List<ConfigurationView> datas = new()
    {
        new ConfigurationView
        {
            Id = 1,
            ViewName = "cashier",
            FieldName = "TypePay",
            Value = @"[
                        { label: 'Tiền mặt', value: 'TM' },
                                { label: 'Công nợ', value: 'CN' },
                                { label: 'Ngân hàng', value: 'NH' },
                    ]",
        },
        new ConfigurationView
        {
            Id = 2,
            ViewName = "cashier",
            FieldName = "Layout",
            Value = "list",// list/ grid
        },
        new ConfigurationView
        {
            Id = 3,
            ViewName = "cashier",
            FieldName = "PrintBill",
            Value = "ExporttBill,DeliveryBill,BillPrint",
        },
        new ConfigurationView
        {
            Id = 4,
            ViewName = "cashier",
            FieldName = "QuantityBoxNec",
            Value = "1",// 1: show, 0:hide
        }
    };
}
