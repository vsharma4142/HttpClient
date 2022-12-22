using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.SalesForce
{
    public  class JobConfiguration
    {
        public string objects { get; set; }
        public string externalIdFieldName { get; set; }
        public string contentType { get; set; }
        public string operation { get; set; }
        public string lineEnding { get; set; }

    }
}


