using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            string result = CreateRecord(token, serviceUrl, createMessage, "Account");



        }
        private string CreateRecord(string token, string serviceUrl, string createMessage, string recordType)
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
            var i = response.Content.ReadAsStringAsync().Result;
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

        public void BulkUpsert(string token)
        {
            //create job
            string restQuery = $"{_settings.ServiceUrl}{_settings.APIEndPointBulkJob}";
            //CreateJob(token, restQuery);
            JObject data = JObject.Parse(File.ReadAllText(@"C:\Code\Salesforce\newinsertjob.json"));
            string json = @"{'object' : 'Account','externalIdFieldName' :'customExtIdField__c','contentType' :'CSV','operation' : 'upsert', 'lineEnding' :'LF'}";

            var contentData = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, restQuery);
            request.Headers.Add("X-PrettyPrint", "1");
            request.Headers.Add("Authorization", "Bearer " + token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = contentData;
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.SendAsync(request).Result;
            var jsonResponse = response.Content.ReadAsStringAsync().Result;
            var obj = JObject.Parse(jsonResponse);

            var jobId = (string)obj["id"];

            //pushdata

            string inserUri = $"{_settings.ServiceUrl}{_settings.APIEndPointBulkJob + jobId + "/batches/"}";

           // FileStream stream = File.OpenRead(@"C:\Code\Salesforce\csvData.csv");
            HttpContent _form;
            _form= new  StreamContent(File.OpenRead(@"C:\Code\Salesforce\csvData.csv"));
            _form.Headers.ContentType = new MediaTypeHeaderValue("text/csv");



            // var contentDataInsert = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "test/csv");
            HttpRequestMessage requestInsert = new HttpRequestMessage(HttpMethod.Put, inserUri);
            requestInsert.Headers.Add("X-PrettyPrint", "1");
            requestInsert.Headers.Add("Authorization", "Bearer " + token);
            requestInsert.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestInsert.Content = _form;
            HttpClient clientInsert = new HttpClient();
            HttpResponseMessage responseInsert = clientInsert.SendAsync(requestInsert).Result;
            var jsonResponseInsert = responseInsert.Content.ReadAsStringAsync().Result;

            var objInsert = JObject.Parse(jsonResponseInsert);

            //job closing

            string closeUri = $"{_settings.ServiceUrl}{_settings.APIEndPointBulkJob + jobId+"/"}";
            HttpRequestMessage requestClose = new HttpRequestMessage(HttpMethod.Patch, closeUri);
            requestClose.Headers.Add("X-PrettyPrint", "1");
            requestClose.Headers.Add("Authorization", "Bearer " + token);
            requestClose.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestClose.Content = new StringContent("{\"state\":\"UploadComplete\"}",
                                Encoding.UTF8,
                                "application/json");
           
            HttpClient clientClose = new HttpClient();
            HttpResponseMessage responseClose = clientClose.SendAsync(requestClose).Result;
            var jsonResponseClose = responseClose.Content.ReadAsStringAsync().Result;
            var objClose = JObject.Parse(jsonResponseClose);

            //Get JobStatus

            string jobUri = $"{_settings.ServiceUrl}{_settings.APIEndPointBulkJob + jobId + "/"}";
            HttpRequestMessage requestJobStatus = new HttpRequestMessage(HttpMethod.Get, jobUri);
            requestJobStatus.Headers.Add("X-PrettyPrint", "1");
            requestJobStatus.Headers.Add("Authorization", "Bearer " + token);
            requestJobStatus.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //requestJobStatus.Content = new StringContent("{\"state\":\"UploadComplete\"}",
                                //Encoding.UTF8,
                                //"application/json");

            HttpClient clientJobStatus = new HttpClient();
            HttpResponseMessage responseJobStatus = clientJobStatus.SendAsync(requestJobStatus).Result;
            var jsonResponseJobStatus = responseJobStatus.Content.ReadAsStringAsync().Result;
            var objJobStatus = JObject.Parse(jsonResponseJobStatus);

            //Get SucessfulRecords



        }
        public void CreateJob(string token, string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string jsonResponse;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);



                string json = @"{'object' : 'Account','externalIdFieldName' :'customExtIdField__c','contentType' :'CSV','operation' : 'upsert', 'lineEnding' :'LF'}";

                var jobRequest = JsonConvert.SerializeObject(json);
                //string url = $"{config["SalesforceInstanceBaseUrl"]}/services/data/{config["SalesforceVersion"]}/jobs/ingest";
                //string jobRequest = JsonConvert.SerializeObject(new JobRequestBody(sObjectType));
                StringContent content = new StringContent(jobRequest, Encoding.UTF8, "application/json");
                jsonResponse = client.PostAsync(url, content)
                    .Result.Content.ReadAsStringAsync()
                    .Result;
            }

            //dynamic jobResponse = JObject.Parse(jsonResponse);
            // return jobResponse.id;
        }
        // throw new NotImplementedException();
    }
}

