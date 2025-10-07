using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject
{
    public class MessengerButton
    {
        public string Title { get; set; }
        public string Payload { get; set; }

        public MessengerButton(string title, string payload)
        {
            Title = title;
            Payload = payload;
        }
    }
}
