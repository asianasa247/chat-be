using Microsoft.AspNetCore.Http;
namespace ManageEmployee.DataTransferObject
{
    public class SchedulePostRequest
    {
        public string Message { get; set; }
        public IFormFile? Photo { get; set; }        
        public string? ImageUrl { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string RepeatType { get; set; }
        public Guid AccessId { get; set; }
    }
}
