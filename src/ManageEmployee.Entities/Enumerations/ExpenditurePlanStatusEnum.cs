using System.Text.Json.Serialization;

namespace ManageEmployee.Entities.Enumerations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExpenditurePlanStatusEnum
{
    Unspent,// chua chi
    PartiallyPaid,// chi 1 phan
    FullyPaid,// hoan thanh
}