using ETL.Data.Models;
using ETL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ETL.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private IAccountService _accountRepository;


        public CustomerController(IAccountService accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [HttpPost(Name = "Update Customer")]
        public async Task<IActionResult> PostAsync(AccountType account)
        {
            var result = await _accountRepository.GetAccountDetails();
            return Ok();
        }
    }
}
