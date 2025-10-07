namespace ManageEmployee.DataTransferObject.XML;

public class TKhaiThue
{
    public string? maTKhai { get; set; }
    public string? tenTKhai { get; set; }
    public string? moTaBMau { get; set; }
    public string? pbanTKhaiXML { get; set; }
    public string? loaiTKhai { get; set; }
    public int soLan { get; set; }
    public KyKKhaiThue? KyKKhaiThue { get; set; }
    public int maCQTNoiNop { get; set; }
    public string? tenCQTNoiNop { get; set; }
    public string? ngayLapTKhai { get; set; }
    public GiaHan? GiaHan { get; set; }
    public string? nguoiKy { get; set; }
    public string? ngayKy { get; set; }
    public string? nganhNgheKD { get; set; }

}
