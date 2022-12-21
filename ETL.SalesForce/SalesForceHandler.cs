using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ETL.SalesForce
{
    public class SalesForceHandler : ISalesForceHandler
    {
        private readonly SalesforceClinetConfiguration _settings;
        public SalesForceHandler(IOptions<SalesforceClinetConfiguration> settings)
        {
            _settings = settings.Value;
        }

        public void CreateAccount(string token, string serviceUrl)
        {
            string companyName = "Bob's Builders";
            string phone = "123-456-7890";

            string createMessage = $"<root>" +
                $"<Name>{companyName}</Name>" +
                $"<Phone>{phone}</Phone>" +
                $"</root>";

            string result = CreateRecord(token,serviceUrl,createMessage, "Account");

          

        }
        private string CreateRecord(string token, string serviceUrl,string createMessage, string recordType)
        {
            HttpContent contentCreate = new StringContent(createMessage, Encoding.UTF8, "application/xml");
            string uri = $"{serviceUrl}{_settings.APIEndPoint}sobjects/{recordType}";

            HttpRequestMessage requestCreate = new HttpRequestMessage(HttpMethod.Post, uri);
            requestCreate.Headers.Add("Authorization", "Bearer " + token);
            requestCreate.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            requestCreate.Content = contentCreate;
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.SendAsync(requestCreate).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
        public void GetData(string token, string serviceUrl)
        {
            string companyName = "ETLRecord";
            string queryMessage = $"SELECT Id, Name, Phone, Type FROM Account WHERE Name = '{companyName}'";

            JObject obj = JObject.Parse(QueryRecord(token, serviceUrl, queryMessage));

            //if ((string)obj["totalSize"] == "1")
            //{
            //    // Only one record, use it
            //    string accountId = (string)obj["records"][0]["Id"];
            //    string accountPhone = (string)obj["records"][0]["Phone"];
            //}
            //if ((string)obj["totalSize"] == "0")
            //{
            //    // No record, create an Account
            //}
            //else
            //{
            //    // Multiple records, either filter further to determine correct Account or choose the first result
            //}
            
        }
        public string QueryRecord(string token, string serviceUrl, string queryMessage)
        {
            string restQuery = $"{serviceUrl}{_settings.APIEndPoint}query?q={queryMessage}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restQuery);
            request.Headers.Add("Authorization", "Bearer " + token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.SendAsync(request).Result;
            var i= response.Content.ReadAsStringAsync().Result;
            return response.Content.ReadAsStringAsync().Result;
        }

       

        public JObject Login()
        {
            String jsonResponse;
            JObject obj;
            using (var client = new HttpClient())
            {
                var request = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"client_id", _settings.ClientId.ToString()},
                {"client_secret", _settings.ClientSecret.ToString()},
                {"username", _settings.UserName.ToString()},
                {"password", _settings.Password.ToString()}
            }
                );
                request.Headers.Add("X-PrettyPrint", "1");
                var response = client.PostAsync(_settings.LoginEndPoint.ToString(), request).Result;
                jsonResponse = response.Content.ReadAsStringAsync().Result;
                obj = JObject.Parse(jsonResponse);
            }
            return obj;
            //var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
            

        }

        public void PostData()
        {
            throw new NotImplementedException();
        }

        public void PushData()
        {
            throw new NotImplementedException();
        }
    }
}
