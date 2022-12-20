using ETL.Data.Models;
using ETL.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            List<AccountType> lstbatchData = new List<AccountType>();
            foreach (var data in result)
            {
                if (batchIncrement > bachCount)
                {
                    batchIncrement = 1;
                    lstbatchData.Clear();
                }
                AccountType accountType = new AccountType();
                accountType.AccontCatageory = data.AccontCatageory;
                accountType.AccountRegion = data.AccountRegion;
                accountType.AccountType1 = data.AccountType;

                lstbatchData.Add(accountType);
                string responseString = string.Empty;
                if (batchIncrement == bachCount)
                {

                    string jsonString = JsonConvert.SerializeObject(accountType);
                    var payload = new StringContent(jsonString, Encoding.UTF8, "application/json");
                    var client = _clientFactory.CreateClient();
                    HttpResponseMessage response = await client.PostAsync("http://localhost:7140/Customer", payload);
                    string responseJson = await response.Content.ReadAsStringAsync();
                    //using (var client = new HttpClient())
                    //{
                    //    try
                    //    {

                    //        // return URI of the created resource.
                    //        //var i = response.Headers.Location;
                    //        client.BaseAddress = new Uri("http://localhost:7140/");//WebApi 1 project URL
                    //        client.DefaultRequestHeaders.Accept.Clear();
                    //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //        StringContent content = new StringContent(JsonConvert.SerializeObject(lstbatchData), Encoding.UTF8, "application/json");
                    //        var response = await client.PostAsJsonAsync("http://localhost:7140/Customer", accountType);
                    //        if (response.IsSuccessStatusCode)
                    //        {
                    //            responseString = response.Content.ReadAsStringAsync().Result;
                    //            var accountTypeResult = response.Content.ReadFromJsonAsync<AccountType>().Result;
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        throw ex;
                    //    }


                    //}
                }
                batchIncrement++;

            }

            return Ok(JsonConvert.SerializeObject(result));
        }


    }
}
