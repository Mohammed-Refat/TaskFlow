using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Core.DTOs.Auth
{
    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty; // string.Empty is used to avoid null reference issues
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }
}
