namespace ManageEmployee.Helpers;

public class ErrorMessages
{
    public const string AccountCodeMustBetween3And6 = "Mã tài khoản phải nằm trong khoản từ 3 - 6 số.";
    public const string AccountDetailCodeMustBeLessThan60 = "Mã chi tiết phải nằm trong khoản từ 0 - 60 ký tự";
    public const string AccountCodeAlreadyExist = "Mã tài khoản bị trùng. Vui lòng nhập mã khác.";
    public const string AccountDetailCodeAlreadyExist = "Mã chi tiết bị trùng. Vui lòng nhập mã khác.";
    public const string AccountDetailCodeDuplicateWithParentCode = "Mã chi tiết không được trùng với chi tiết cha. Vui lòng nhập mã khác.";
    public const string AccountDetailContainsChild = "Mã chi tiết này có tồn tại chi tiết con. Vui lòng nhập mã khác.";
    public const string MissingParentAccount = "Mã tài khoản này chưa có tài khoản cha.";
    public const string FailedToDelete = "Không dược xóa.";
    public const string DataNotFound = "Không tìm thấy dữ liệu.";
    public const string DataNotFoundWithData = "Không tìm thấy dữ liệu {0}.";
    public const string ParentAccountIsInSyncGroupAndCanNotCreate = "Tài khoản cha đã có trong nhóm đồng bộ. Không được thêm tài khoản con.";
    public const string TaxCodeAlreadyExist = "Mã đơn đã tồn tại.";
    public const string DescriptionNameAlreadyExist = "Diễn giải cùng tên đã tồn tại.";
    public const string AccountGroupCodeAlreadyExist = "Mã nhóm bị trùng. Vui lòng nhập mã khác.";
    public const string AccGroupDoesNotMatchForMerging = "Thông báo mã này không cùng loại với mã bạn định gộp.";
    public const string AccountContainsDetailsAndDoesNotAllowToBeMerged = "Mã này có chi tiết, không được gộp.";
    public const string CustomerCodeAlreadyExist = "Mã khách hàng đã tồn tại, vui lòng nhập mã khác";
    public const string CustomerAlreadyExist = "Số điện thoại khách hàng đã tồn tại, vui lòng nhập kiểm tra lại";
    public const string CustomerCodeNotExist = "Mã khách hàng không tồn tại, không thể cập nhật";
    public const string CustomerTaxCodeNotExist = "Thông tin thuế không tồn tại. Không thể cập nhật";
    public const string DeskFloorNameAlreadyExist = "Tên bàn/lầu đã tồn tại, vui lòng nhập bàn/lầu khác";
    public const string CategoryNameAlreadyExist = "Danh mục đã tồn tại, vui lòng nhập danh mục khác";
    public const string GoodsCodeAlreadyExist = "Hàng hóa đã tồn tại, vui lòng nhập hàng hóa khác";
    public const string JobCodeNotExist = "Công việc không tồn tại trong hệ thống!";
    public const string StatusCodeNotExist = "Trạng thái không tồn tại trong hệ thống!";
    public const string AccountIsInGroupSyncAndDoesNotAllowToEdit =
        "Thông báo tài khoản này nằm trong đồng bộ. Bạn không được sửa. Nếu muốn sửa hoặc gộp thì phải vào đồng bộ và xóa tài khoản ra khỏi nhóm.";
    public const string NameAlreadyExist = "Tên đã tồn tại, vui lòng nhập tên khác";
    public const string UpdateFixedAssetError = "Lỗi cập nhật tài sản cố định";
    public const string AccessDenined = "Bạn không có quyền truy cập";
    public const string NotDeletedAll = "Không thể xóa hết dữ liệu đã chọn";
    public const string ImportFileError = "Không thể nhập thông tin từ file";
    public const string CreateCostOfGoodsError = "Không thể ghi nhận bút toán xuất giá vốn";
    public const string DataExist = "Dữ liệu đang được sử dụng";
    public const string BranchNameAlreadyExist = "Tên chi nhánh đã tồn tại";
    public const string BranchCodeAlreadyExist = "Mã chi nhánh đã tồn tại";
    public const string MenuCodeAlreadyExist = "Mã menu đã tồn tại";
    public const string MenuKpiCodeAlreadyExist = "Mã menu đã tồn tại"; 
    public const string EmailAlreadyExist = "Email đã tồn tại";
    public const string UserNotFound = "Không tìm thấy user";
    public const string ProcedureNotFound = "Không tìm thấy quy trình";
    public const string ProcedureStepNotFound = "Chưa thiết lập bước quy trình";
    public const string YearShouldUnique = "Năm thiết lập là duy nhất";
    public const string CannotChangeStatus = "Bạn không thể chọn status này";
    public const string ProcedureFinished = "Quy trình đã hoàn thành";
    public const string ProcedureCannotDelete = "Quy trình đã tồn tại bước tiếp theo không thể xóa";
    public const string ItemsIsNull = "Bạn chưa chọn hàng hóa";
    public const string ItemsCarIsNull = "Bạn chưa chọn xe";
    public const string ProcedureNotFinished = "Quy trình chưa hoàn thành";
    public const string ProcedureNotNotSameDepartment = "Người duyệt không cùng phòng ban";
    public const string CannotRemoveDetailApplied = "Không thể xóa những phiếu đã được duyệt";
    public const string CannotAccept = "Bạn đã duyệt phiếu này";
    public const string IncorrectFormat = "Sai định dạng dữ liệu";
    public const string LOGIN_AGAIN = "LOGIN_AGAIN";//"Bạn cần login lại tài khoản thuế"
    public const string ChatboxAITopicCodeAlreadyExist = "Mã chủ đề đã tồn tại";


}