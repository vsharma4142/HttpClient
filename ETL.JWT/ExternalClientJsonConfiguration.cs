using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.JWT
{
    public class ExternalClientJsonConfiguration
    {
        public string? ReferralUrl { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? ReferralId { get; set; }
        public string? RsaPrivateKey { get; set; }
        public string? RsaPublicKey { get; set; }
        public string Key { get; set; }
    }
}
