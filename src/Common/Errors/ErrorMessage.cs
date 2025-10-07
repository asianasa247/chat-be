namespace Common.Errors;

public static class ErrorMessage
{
    public const string DATA_IS_EMPTY = "Dữ liệu trống";
    public const string DATA_IS_NOT_EXIST = "Không tồn tại dữ liệu";
    public const string NOT_DECLARE_INVOICE = "Bạn chưa khai báo hoá đơn";
    public const string CUSTOMER_UPDATE_PERMISSION = "Bạn không có quyền update khách hàng này !!";
    public const string QUANTITY_ROOM_TYPE_FAIL = "Số lượng phòng bạn update đang khác so với số phòng hiện tại.";

    #region bill
    public const string BILL_IS_PAYED = "Hóa đơn đã thanh toán";
    public const string BILL_IS_NOT_FOUND = "Hóa đơn không được tìm thấy";
    public const string BILL_IS_CREATED = "Số hóa đơn đã tồn tại trong hệ thống";
    #endregion

    #region customer
    public const string USERID_IS_EMPTY = "Bạn chưa chọn nhân viên chuyển giao!";

    #endregion
    #region web
    public const string PHONE_NUMBER_IS_NOT_FORMAT = "Số điện thoại không đúng!";
    public const string PHONE_NUMBER_IS_EXIST = "Số điện thoại đã tồn tại!";

    #endregion
}
