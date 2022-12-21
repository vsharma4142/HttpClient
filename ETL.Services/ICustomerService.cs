using ETL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Services
{
    public interface ICustomerService
    {
        public Task<IEnumerable<Customer>> GetCustomer();
        public Task<int> UpdateCustomer(Customer accountDetail);
    }
}
