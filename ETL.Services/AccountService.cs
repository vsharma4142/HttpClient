using ETL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Services
{
    public class AccountService : IAccountService
    {
        private new ETLDbContext _context;

        public AccountService(ETLDbContext context) : base()
        {
            _context = context;
        }
        public async Task<IEnumerable<AccountDetail>> GetAccountDetails()
        {

            return await _context.AccountDetails.ToListAsync();

        }
        public async Task<int> UpdateAccounts(AccountType accountDetail)
        {
            _context.AccountTypes.Add(accountDetail);
            var result=await _context.SaveChangesAsync();
            return result;
        }
    }
}
