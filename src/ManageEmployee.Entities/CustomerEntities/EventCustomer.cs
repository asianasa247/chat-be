// File: ManageEmployee/Entities/CustomerEntities/EventCustomer.cs
using System;

namespace ManageEmployee.Entities.CustomerEntities
{
    public class EventCustomer
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? EventCode { get; set; }
        public string EventName { get; set; } = string.Empty;

        public string? Supervisor { get; set; }
        public string? Note { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
