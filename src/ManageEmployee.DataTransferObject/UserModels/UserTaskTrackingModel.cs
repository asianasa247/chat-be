namespace ManageEmployee.DataTransferObject.UserModels;

public class UserTaskTrackingModel
{
    /// <summary>
    /// id task
    /// </summary>
    public int UserTaskId { get; set; }

    /// <summary>
    /// 0= Hoãn lại
    /// 1 = Bắt đầu
    /// </summary>
    public int IsStart { get; set; }
}