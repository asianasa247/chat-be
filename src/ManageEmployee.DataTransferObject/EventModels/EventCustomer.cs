
using System;

namespace ManageEmployee.DataTransferObject.EventModels
{
    /// <summary>
    /// DTO tạo mới
    /// </summary>
    public class EventCustomerCreateDto
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? EventCode { get; set; }
        public string EventName { get; set; } = string.Empty;

        public string? Supervisor { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// DTO cập nhật
    /// </summary>
    public class EventCustomerUpdateDto
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? EventCode { get; set; }
        public string EventName { get; set; } = string.Empty;

        public string? Supervisor { get; set; }
        public string? Note { get; set; }
    }
}
