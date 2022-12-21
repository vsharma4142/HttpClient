using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.JWT
{
    public interface IJwtHandler
    {
        JwtResponse CreateToken(JwtCustomClaims claims);
        bool ValidateToken(string token);
    }
}
