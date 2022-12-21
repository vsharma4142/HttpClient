using ETL.Data.Models;
using ETL.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ETL.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountRepository;
        private readonly IHttpClientFactory _clientFactory;


        public AccountController(IAccountService accountRepository, IHttpClientFactory clientFactory)
        {
            _accountRepository = accountRepository;
            _clientFactory = clientFactory;
        }

        [HttpGet(Name = "GetAccounts")]
        public async Task<IActionResult> GetAccounts(int id)
        {
            var result = await _accountRepository.GetAccountDetails();

            var totalRecords = result.Count();
            //max recound one request=2
            //max req per day =5


            var bachCount = totalRecords / 6;

            int batchIncrement = 1;
            int pk = 1;
            List<Customer> lstbatchData = new List<Customer>();
            foreach (var data in result)
            {
                if (batchIncrement > bachCount)
                {
                    batchIncrement = 1;
                    lstbatchData.Clear();
                }
                Customer customer = new()
                {
                    CustomerId = pk,
                    CustomerRegion = data.AccountRegion,
                    CustomerName = "jacob"+ pk.ToString(),
                    Ssn = "999 0000"+ pk.ToString(),
                    Phone = "1 9999 0000" + pk.ToString(),
                    AccountId = data.AccountId
                };

                lstbatchData.Add(customer);
                string responseString = string.Empty;
                if (batchIncrement == bachCount)
                {
                    try
                    {
                        string jsonString = JsonConvert.SerializeObject(lstbatchData);
                        var payload = new StringContent(jsonString, Encoding.UTF8, "application/json");
                        var client = _clientFactory.CreateClient();
                        HttpResponseMessage response = await client.PostAsync("https://localhost:7140/Customer", payload);
                        string responseJson = await response.Content.ReadAsStringAsync();
                    }
                    catch(Exception ex)
                    {

                    }

                }
                batchIncrement++;
                pk++;

            }

            return Ok(JsonConvert.SerializeObject(result));
        }


    }
}
