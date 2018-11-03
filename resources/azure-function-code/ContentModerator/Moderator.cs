using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.CognitiveSearch.WebApiSkills;
using Newtonsoft.Json;

namespace CustomWebSkill
{
    public static class ModorationFunction
    {
               
        [FunctionName("Moderate")]
        public static async Task<HttpResponseMessage> RunContentMod([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log, ExecutionContext executionContext)
        {
            log.Info("C# HTTP trigger function processed a request.");
            string skillName = executionContext.FunctionName;

            IEnumerable<WebApiRequestRecord> requestRecords = WebApiSkillHelpers.GetRequestRecords(req);
            if (requestRecords == null)
            {

                return req.CreateErrorResponse(HttpStatusCode.BadRequest, $"{skillName} - Invalid request record array.");
            }
            dynamic obj = requestRecords.First().Data.First().Value;

            string val = await MakeRequest(obj);
            ContentModerator mod = JsonConvert.DeserializeObject<ContentModerator>(val);
            WebApiResponseRecord output = new WebApiResponseRecord();
            output.RecordId = requestRecords.First().RecordId;
            output.Data["PII"] = mod.PII;
                                                              
            return req.CreateResponse(HttpStatusCode.OK, output);

        }

        static async Task<string> MakeRequest(string input)
        {
            var client = new HttpClient();

            // TO DO - Action required!!
            // URL of the Moderator API. Fix the Prefix below, using your service URL. It can be found in the Azure Portal.
            // If you are using South Central US, you don't need to change it.
            var uriPrefix = "https://southcentralus.api.cognitive.microsoft.com/contentmoderator";

            // Suffix, don't need to change anything.
            // Please realize the code will do PII only, for english only. This restriction was implemented by design, to keep the training as simple as possible.
            var uriSuffix = "/moderate/v1.0/ProcessText/Screen?autocorrect=false&PII=true&classify=false&language=eng";

            // TO DO - Action required!!
            // Request headers - Add your API key to the placeholder below.        
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "your key here");

            //TO DO - Action required!!
            // Add the correct URL of your host, same prefix of uriPrefix but without https:// and finishing on ".com". 
            // If your are using south central us, don't need to change the Host.
            // If you are using South Central US, you don't need to change it.
            client.DefaultRequestHeaders.Add("Host", "southcentralus.api.cognitive.microsoft.com");


            var uri = uriPrefix + uriSuffix;

            HttpResponseMessage response;
            byte[] byteData = Encoding.UTF8.GetBytes(input);
            // Request body

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                response = await client.PostAsync(uri, content);
            }
            return await response.Content.ReadAsStringAsync();

        }
    }

    public class ContentModerator
    {
        public string OriginalText { get; set; }
        public string NormalizedText { get; set; }
        public string AutoCorrectedText { get; set; }
        public object Misrepresentation { get; set; }
        public Classification Classification { get; set; }
        public Status Status { get; set; }
        public PII PII { get; set; }
        public string Language { get; set; }
        public Terms[] Terms { get; set; }
        public string TrackingId { get; set; }
    }

    public class Classification
    {
        public Category1 Category1 { get; set; }
        public Category2 Category2 { get; set; }
        public Category3 Category3 { get; set; }
        public bool ReviewRecommended { get; set; }
    }

    public class Category1
    {
        public float Score { get; set; }
    }

    public class Category2
    {
        public float Score { get; set; }
    }

    public class Category3
    {
        public float Score { get; set; }
    }

    public class Status
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public object Exception { get; set; }
    }

    public class PII
    {
        public Email[] Email { get; set; }
        public IPA[] IPA { get; set; }
        public Phone[] Phone { get; set; }
        public Address[] Address { get; set; }
    }

    public class Email
    {
        public string Detected { get; set; }
        public string SubType { get; set; }
        public string Text { get; set; }
        public int Index { get; set; }
    }

    public class IPA
    {
        public string SubType { get; set; }
        public string Text { get; set; }
        public int Index { get; set; }
    }

    public class Phone
    {
        public string CountryCode { get; set; }
        public string Text { get; set; }
        public int Index { get; set; }
    }

    public class Address
    {
        public string Text { get; set; }
        public int Index { get; set; }
    }

    public class Terms
    {
        public int Index { get; set; }
        public int OriginalIndex { get; set; }
        public int ListId { get; set; }
        public string Term { get; set; }
    }

}

