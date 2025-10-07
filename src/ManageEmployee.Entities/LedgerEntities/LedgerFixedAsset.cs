using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities.LedgerEntities;

public class LedgerFixedAsset
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long LedgerId { get; set; }
    public int FixedAssetId { get; set; }
    public int FixedAsset242Id { get; set; }
    public int IsInternal { get; set; } = 0;
}