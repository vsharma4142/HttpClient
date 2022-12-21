using ETL.Data.Models;
using ETL.JWT;
using ETL.SalesForce;
using ETL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETL.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private ICustomerService _customerRepository;
        private readonly IJwtHandler _jwtHandler;
        private readonly ISalesForceHandler _salesForceHandler;

        public CustomerController(ICustomerService customerRepository, IJwtHandler jwtHandler, ISalesForceHandler salesForceHandler)
        {
            _customerRepository = customerRepository;
            _jwtHandler = jwtHandler;
            _salesForceHandler = salesForceHandler;
        }
        [HttpPost(Name = "Update Customer")]
        public async Task<IActionResult> PostAsync(IEnumerable<Customer> customers)
        {
            foreach (var customer in customers)
            {
                var result = await _customerRepository.UpdateCustomer(customer);
            }
            return Ok();
        }
        // [Authorize]
        [HttpGet(Name = "Get Customers")]
        public async Task<IActionResult> GetCustomers(int id)
        {
            var resultObject = _salesForceHandler.Login();
            string AuthToken = (string)resultObject["access_token"];
            string ServiceUrl = (string)resultObject["instance_url"];
           _salesForceHandler.CreateAccount(AuthToken,ServiceUrl);
            //_salesForceHandler.GetData(AuthToken, ServiceUrl);
            var result = await _customerRepository.GetCustomer();

            return Ok();
        }
    }
}
