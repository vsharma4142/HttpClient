using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ETL.SalesForce
{
    public  class SalesforceClinetConfiguration
    {
        //public SalesforceClinetConfiguration()
        //{
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        //}


        public string APIEndPoint { get; set; }//version
        public string APIEndPointBulkJob { get; set; }//version
        public string APIEndPointBulkInsert { get; set; }//version
        public string LoginEndPoint { get; set; } //url
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthToken { get; set; }
        public string ServiceUrl { get; set; }
    }
}
