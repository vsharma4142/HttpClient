using ETL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Services
{
    public class CustomerService : ICustomerService
    {
       
        private new ETLDbContext _context;

        public CustomerService(ETLDbContext context) : base()
        {
            _context = context;
        }
        public async Task<IEnumerable<Customer>> GetCustomer()
        {

            return await _context.Customers.ToListAsync();

        }
        public async Task<int> UpdateCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            var result = await _context.SaveChangesAsync();
            return result;
        }
    }
}
