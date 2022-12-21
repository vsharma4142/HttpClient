using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.JWT
{
    public class JwtResponse
    {
        public string Token { get; set; }
        public long ExpiresAt { get; set; }
    }
}
