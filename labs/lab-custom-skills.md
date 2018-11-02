# Lab 3: Create a Cognitive Search Skillset with **Custom** Skills

In this lab, you will learn [how to create a Custom Skill](https://docs.microsoft.com/en-us/azure/search/cognitive-search-custom-skill-interface)
and integrate it to the enrichment pipeline. Custom skills allow you to add any REST API transformation to the dataset, also feeding other transformations within the pipeline, if required.

You will use an [Azure Function](https://azure.microsoft.com/services/functions/) to wrap the [Content Moderator API](https://azure.microsoft.com/en-us/services/cognitive-services/content-moderator/) and detect uncomlient documents in the dataset.

The Azure Content Moderator API is a cognitive service that checks text, image, and video content for material that is potentially offensive, risky, or otherwise undesirable. When such material is found, the service applies appropriate labels (flags) to the content. Your app can then handle flagged content in order to comply with regulations or maintain the intended environment for users. See the [Content Moderator APIs](https://docs.microsoft.com/en-us/azure/cognitive-services/content-moderator/overview#content-moderator-apis) section to learn more about what the different content flags indicate.

The text moderation responses include:

+ Profanity: term-based matching with built-in list of profane terms in various languages
+ Classification: machine-assisted classification into three categories
+ Personally Identifiable Information (PII)
+ Auto-corrected text
+ Original text
+ Language

>Note! This Azure Function output is **Edm.String**, so we need to use the same type in the index definition.  

## Step 1 - Content Moderator API

Use the [Azure Portal](https://ms.portal.azure.com) to create a Content Moderator API, using the name you want, the location of the Azure Search service and the F0 pricing tier. You should also save the keys and the endpoint, for later use in this lab.

To see how the API works, and also to learn how to demo this technology in minutes, navigate to the [Content Moderator Text API console](https://docs.microsoft.com/en-us/azure/cognitive-services/content-moderator/try-text-api). Read the all page, you will need 4 minutes to do it. When you are done, scroll all the way up and click the first like of the page, [Text Moderation API](https://westus.dev.cognitive.microsoft.com/docs/services/57cf753a3f9b070c105bd2c1/operations/57cf753a3f9b070868a1f66f). It will open a control panel for Cognitive Services, as you can see in the image below.

![Cognitive Services Panel](../resources/images/lab-custom-skills/panel.png)

Now, clicking the blue buttons, choose the region where you created your Content Moderator API, that should be the same region you are using for all services in this training. Now set the values as listed below:

+ autocorrect = true
+ PII = true
+ listId = remove parameter (we don't have a list of prohibited terms to work with at this point)
+ classify = true
+ language = eng (The default example is in english)
+ keep Conten-Type as "text/plain"
+ paste your Content Moderator API key

Now you are ready to test the API you created on Azure Portal. Scroll down and check the suggested text in "Request body" section. It has PIIs like email, phone number, physical and IP addresses. It also has a profanity word. Scroll until the end of the page and click the blue "Send" button. The expected results are:

+ Response Status = 200 OK
+ Response Latency <= 1000 ms
+ Response content with a json file where you can read all of the detected problems. The "Index" field indicates the position of the term within the submitted text.

## Step 2 - Create an Azure Function

Although this example uses an Azure Function to host a web API, it is not required.  As long as you meet the [interface requirements for a cognitive skill](https://docs.microsoft.com/en-us/azure/search/cognitive-search-custom-skill-interface), the approach you take is immaterial. Azure Functions, however, make it easy to create a custom skill.

### Step 2.1 - Create a function app

1. In Visual Studio, select **New** > **Project** from the File menu.

1. In the New Project dialog, select **Installed**, expand **Visual C#** > **Cloud**, select **Azure Functions**, type a Name for your project, and select **OK**. The function app name must be valid as a C# namespace, so don't use underscores, hyphens, or any other nonalphanumeric characters.

1. Select **Azure Functions v2 (.Net Core)**. You could also do it with version 1, but the code written below is based on the v2 template.

1. Select the type to be **HTTP Trigger**.

1. For Storage Account, you may select **None**, as you won't need any storage for this function.

1. Select **OK** to create the function project and HTTP triggered function.

#### Modify the code to call the Translate Cognitive Service

Visual Studio creates a project and in it a class that contains boilerplate code for the chosen function type. The *FunctionName* attribute on the method sets the name of the function. The *HttpTrigger* attribute specifies that the function is triggered by an HTTP request.

Now, replace all of the content of the file *Function1.cs* with the following code:

```csharp
using System.Collections.Generic;
using System.IO;
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

            respMessage outputObj = new respMessage();
            outputObj.values = new List<OutputItem>();

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

            //URL of the Moderator API. Fix the Prefix with your URL, what can be found in the Azure Portal.
            var uriPrefix = "https://southcentralus.api.cognitive.microsoft.com/contentmoderator";
            var uriSuffix = "/moderate/v1.0/ProcessText/Screen?autocorrect=false&PII=true&classify=false&language=eng";


            // Request headers - Add your API key to the placeholder below.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "<your key here>");

            // Add the correct URL of your host, same prefix of uriPrefix but without https:// and finishing on ".com".
            // If your are using south central us, don't need to change the Host.
            client.DefaultRequestHeaders.Add("Host", "southcentralus.api.cognitive.microsoft.com");


            var uri = uriPrefix + uriSuffix;

            HttpResponseMessage response;
            byte[] byteData = Encoding.UTF8.GetBytes(input);
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
```

Make sure to enter your own *key* value in the *Moderate* method based on the key you got when signing up for the Content Moderator API.

This example is a simple enricher that only works on one record at a time. This fact will become important later, when you're setting the batch size for the skillset.

## Step 3 - Test the function from Visual Studio

Press **F5** to run the program and test function behaviors. Use Postman to issue a call like the one shown below:

```http
POST https://localhost:7071/api/Moderate
```

### Request body

```json
{
   "values": [
        {
            "recordId": "a1",
            "data":
            {
               "text":  "Este es un contrato en Inglés",
               "language": "es"
            }
        }
   ]
}
```

### Response

You should see a response similar to the following example:

```json
{
    "values": [
        {
            "recordId": "a1",
            "data": {
                "text": "This is a contract in English"
            },
            "errors": null,
            "warnings": null
        }
    ]
}
```

## Step 4 - Publish the function to Azure

When you are satisfied with the function behavior, you can publish it.

1. In **Solution Explorer**, right-click the project and select **Publish**. Choose **Create New** > **Publish**.

2. If you haven't already connected Visual Studio to your Azure account, select **Add an account....**

3. Follow the on-screen prompts. You are asked to specify the Azure account, the resource group, the hosting plan, and the storage account you want to use. You can create a new resource group, a new hosting plan, and a storage account if you don't already have these. When finished, select **Create**

4. After the deployment is complete, note the Site URL. It is the address of your function app in Azure.

5. In the [Azure portal](https://portal.azure.com), navigate to the Resource Group, and look for the Translate Function you published. Under the **Manage** section, you should see Host Keys. Select the **Copy** icon for the *default* host key.  

## Step 5 - Test the function in Azure

Now that you have the default host key, use Postman to test your function as follows:

```http
POST https://[enter you Function name here].azurewebsites.net/api/Translate?code=[enter default host key here]
```

### Request Body

```json
{
   "values": [
        {
            "recordId": "a1",
            "data":
            {
               "text":  "Este es un contrato en Inglés",
               "language": "es"
            }
        }
   ]
}
```

This should produce a similar result to the one you saw previously when running the function in the local environment.

### Step 5.1 - Update SSL Settings

All Azure Functions created after June 30th, 2018 have disabled TLS 1.0, which is not currently compatible with custom skills. Today, August 2018, Azure Functions default TLS is 1.2, which is causing issues. This is a **required workaround**:

1. In the Azure portal, navigate to the Resource Group, and look for the Translate Function you published. Under the Platform features section, you should see SSL.
2. After selecting SSL, you should change the **Minimum TLS version** to 1.0. TLS 1.2 functions are not yet supported as custom skills.

For more information, click [here](https://docs.microsoft.com/en-us/azure/search/cognitive-search-create-custom-skill-example#update-ssl-settings ) .

## Step 6 - Cleaning the environment again

Let's do the same cleaning process of lab 2. Save all scripts (API calls) you did until here, including the definition json files you used in the "body" field.

### Step 6.1

Let's start deleting the index and the indexer. You can use Azure Portal or API calls:

1. [Deleting the indexer](https://docs.microsoft.com/en-us/rest/api/searchservice/delete-indexer) - Just use your service, key and indexer name
2. [Deleting the index](https://docs.microsoft.com/en-us/rest/api/searchservice/delete-index) - Just use your service, key and indexer name

### Step 6.2

Skillsets can only be deleted through an HTTP command, let's use another API call request to delete it. Don't forget to add your skillset name in the URL.

```http
DELETE https://[servicename].search.windows.net/skillsets/demoskillset?api-version=2017-11-11-Preview
api-key: [api-key]
Content-Type: application/json
```

Status code 204 is returned on successful deletion.

## Step 7 - Connect to your Pipeline, recreating the environment

Now let's use the [official documentation](https://docs.microsoft.com/en-us/azure/search/cognitive-search-custom-skill-interface) to learn the syntax we need to add the custom skill to our enrichment pipeline.

As you can see, the custom skill works like all other predefined skills, but the type is **WebApiSkill** and you need to specify the URL you created above. The example below shows you how to call the skill. Because the skill doesn't handle batches, you have to add an instruction for maximum batch size to be just ```1``` to send documents one at a time.
Like we did in Lab 2, we suggest you add this new skill at the end of the body definition of the skillset.

```json
      {
        "@odata.type": "#Microsoft.Skills.Custom.WebApiSkill",
        "description": "Our new translator custom skill",
        "uri": "https://[enter function name here].azurewebsites.net/api/Translate?code=[enter default host key here]",
        "batchSize":1,
        "context": "/document",
        "inputs": [
          {
            "name": "text",
            "source": "/document/content"
          },
          {
            "name": "language",
            "source": "/document/languageCode"
          }
        ],
        "outputs": [
          {
            "name": "text",
            "targetName": "translatedText"
          }
        ]
      }

```

### Step 7.1 - Challenge

As you can see, again we are not giving you the body request. One more time you need to use Lab 1 as a reference. We can't use lab 2 definition because we've hit the maximum number of skills allowed within a skillset of a basic account (five). So, let's use Lab 1 json requests again.
Skipping the services and the data source creation, repeat the other steps of the Lab 1, in the same order.

1. ~~Create the services at the portal~~ **Not required, we did not delete it**.
2. ~~Create the Data Source~~ **Not required, we did not delete it**.
3. Recreate the Skillset
4. Recreate the Index
5. Recreate the Indexer
6. Check Indexer Status - Check the translation results.  
7. Check the Index Fields - Check the translated text new field.
8. Check the data - If you don't see the translated data, something went wrong.

## Step 8

Now we have our data enriched with pre-defined and custom skills. Use the Search Explorer on the Azure Portal to query the data. Create a query to identity documents with compliance issues, the moderated documents.

## Finished Solution

If you could not make it, [here](../resources/finished-solutions/finished-solution-Lab-custom-skills.md) is the challenge solution. You just need to follow the steps.

## Next Step

[Final Case Lab](../labs/lab-final-case.md) or
[Back to Read Me](../README.md)
