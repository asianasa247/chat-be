using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatappLC.Application.DTOs.User
{
    public class UserDTO
    {
        public string Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
