using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities
{
    public class AdditionWebGoods : BaseEntity
    {
        public int Id { get; set; }
        public int AdditionWebId { get; set; }
        public int GoodId { get; set; }

        [ForeignKey(nameof(AdditionWebId))]
        public AdditionWeb? AdditionWeb { get; set; }
    }
}
