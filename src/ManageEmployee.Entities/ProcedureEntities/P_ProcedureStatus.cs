using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ProcedureEntities;

public class P_ProcedureStatus
{
    public int Id { get; set; }
    public int P_ProcedureId { get; set; }
    public int Type { get; set; }//1: khoi tao, 2:ket thuc
    [StringLength(255)]
    public string? Name { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }
}
