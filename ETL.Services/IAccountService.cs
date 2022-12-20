using ETL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Services
{
    public interface  IAccountService
    {
        public Task<IEnumerable<AccountDetail>> GetAccountDetails();
        public Task<int> UpdateAccounts(AccountType accountDetail);
    }
}
