namespace ManageEmployee.Entities.Constants;

public class ChartOfAccountResultErrorConstants
{
    public const string NOT_ACCOUNT_TEST = "Tài khoản này không có trong hệ thống tài khoản bạn";
    public const string ACCOUNT_GROUP_LINK = "Tài khoản này nằm trong Hệ thống tài khoản đồng bộ nên không được thêm tài khoản con";
    public const string ACCOUNT_EXIST = "Mã này đã có vui lòng nhập 1 Mã khác nha!!!";
    public const string ACCOUNT_LEDGER = "Tài khoản cha của mã này đã có Phát sinh vui lòng nhập 1 Mã khác";
    public const string ACCOUNT_LEDGER_DETAIL = "Đã có Phát sinh rồi không thể thêm chi tiết  được!!!";
    public const string ACCOUNT_PARENT_NOT_EXIST = "Tài khoản này chưa có Tài khoản Cha";
    public const string ACCOUNT_USING = "Tài khoản đã có chứng từ phát sinh không xóa được";
    public const string ACCOUNT_USED = "Tài khoản đã được dùng trong kế toán hoặc hàng hoá không thể update mã";
    public const string ACCOUNT_USING_OTHER_TABLE = "Tài khoản đang được sử dụng ở {0} không xóa được";

}
