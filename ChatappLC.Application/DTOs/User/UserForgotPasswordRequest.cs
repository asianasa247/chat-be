using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatappLC.Application.DTOs.User
{
    public class UserForgotPasswordRequest
    {
        public required string Email { get; set; }
    }
}
