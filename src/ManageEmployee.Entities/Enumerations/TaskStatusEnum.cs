namespace ManageEmployee.Entities.Enumerations;

public enum TaskStatusEnum
{
    //0: đang mở, 1: đang tiến hành, 2: tạm hoãn, 3 hoàn thành
    Open = 0,
    Processing = 1,
    Pending = 2,
    Done = 3,
    Reviewing = 4,
}