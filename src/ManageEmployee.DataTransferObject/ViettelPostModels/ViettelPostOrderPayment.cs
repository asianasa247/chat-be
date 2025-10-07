namespace ManageEmployee.DataTransferObject.ViettelPostModels;

public enum ViettelPostOrderPayment
{
    NoCOD = 1,              // Không thu hộ
    CODWithShipping = 2,    // Thu hộ + phí vận chuyển
    CODOnly = 3,            // Chỉ thu hộ
    ShippingOnly = 4        // Chỉ phí vận chuyển
}

