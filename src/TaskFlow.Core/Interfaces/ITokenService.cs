using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Core.Entities;

namespace TaskFlow.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(AppUser user);
    }
}
