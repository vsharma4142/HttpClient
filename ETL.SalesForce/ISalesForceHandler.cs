using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.SalesForce
{
    public interface ISalesForceHandler
    {
        JObject Login();

        void CreateAccount(string token, string serviceUrl);

        void GetData(string token, string serviceUrl);
        void PostData();
        void PushData();

    }
}
