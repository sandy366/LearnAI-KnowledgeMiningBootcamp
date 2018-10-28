# Lab 3: Create a Cognitive Search Skillset with **Custom** Skills

In this lab, you will learn how to create a web API custom skill that accepts text in any language and translates it to English. It is required because there are documents in languages besides English in our dataset. The expected behavior of this code is to do nothing if the document language is English and translate to English if the document is in another language. **The provided dataset has documents in Spanish**.

We will use an [Azure Function](https://azure.microsoft.com/services/functions/) to wrap the [Translate Text API](https://azure.microsoft.com/services/cognitive-services/translator-text-api/) so that it implements the custom skill interface.

For documents in english we will replicate the original text in the output, so we can search only one field of our index. Another important detail, this function output is **Edm.String**, so we need to use the same type in the index definition.  

## Step 1 - Translator Text API

Use this [link](https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-text-how-to-signup) to sign up for the Translator Text API. Keep the API key, you will use it later in this lab. **Again, use the same region of your Azure Search Service.**

## Step 2 - Create an Azure Function

Although this example uses an Azure Function to host a web API, it is not required.  As long as you meet the [interface requirements for a cognitive skill](https://docs.microsoft.com/en-us/azure/search/cognitive-search-custom-skill-interface), the approach you take is immaterial. Azure Functions, however, make it easy to create a custom skill.

### Step 2.1 - Create a function app

1. In Visual Studio, select **New** > **Project** from the File menu.

2. In the New Project dialog, select **Installed**, expand **Visual C#** > **Cloud**, select **Azure Functions**, type a Name for your project, and select **OK**. The function app name must be valid as a C# namespace, so don't use underscores, hyphens, or any other nonalphanumeric characters.

3. Select the type to be **HTTP Trigger**.

4. For Storage Account, you may select **None**, as you won't need any storage for this function.

5. Select **OK** to create the function project and HTTP triggered function.

#### Modify the code to call the Translate Cognitive Service

Visual Studio creates a project and in it a class that contains boilerplate code for the chosen function type. The *FunctionName* attribute on the method sets the name of the function. The *HttpTrigger* attribute specifies that the function is triggered by an HTTP request.

Now, replace all of the content of the file *Function1.cs* with the following code:

```csharp
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace TranslateFunction
{
    // This function will simply translate messages sent to it.
    public static class Function1
    {
        #region classes used to serialize the response
        private class WebApiResponseError
        {
            public string message { get; set; }
        }

        private class WebApiResponseWarning
        {
            public string message { get; set; }
        }

        private class WebApiResponseRecord
        {
            public string recordId { get; set; }
            public Dictionary<string, object> data { get; set; }
            public List<WebApiResponseError> errors { get; set; }
            public List<WebApiResponseWarning> warnings { get; set; }
        }

        private class WebApiEnricherResponse
        {
            public List<WebApiResponseRecord> values { get; set; }
        }
        #endregion


        /// <summary>
        /// Note that this function can translate up to 1000 characters. If you expect to need to translate more characters, use
        /// the paginator skill before calling this custom enricher
        /// </summary>
        [FunctionName("Translate")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string recordId = null;
            string originalText = null;
            string originalLanguage = null;
            string translatedText = null;

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // Validation
            if (data?.values == null)
            {
                return new BadRequestObjectResult(" Could not find values array");
            }
            if (data?.values.HasValues == false || data?.values.First.HasValues == false)
            {
                // It could not find a record, then return empty values array.
                return new BadRequestObjectResult(" Could not find valid records in values array");
            }

            recordId = data?.values?.First?.recordId?.Value as string;
            originalText = data?.values?.First?.data?.text?.Value as string;
            originalLanguage = data?.values?.First?.data?.language?.Value as string;

            if (recordId == null)
            {
                return new BadRequestObjectResult("recordId cannot be null");
            }

            // Only translate records that actually need to be translated.
            if (!originalLanguage.Contains("en"))
            {
                translatedText = TranslateText(originalText, "en-us").Result;
            }
            else
            {
                // text is already in English.
                translatedText = originalText;
            }

            // Put together response.
            WebApiResponseRecord responseRecord = new WebApiResponseRecord();
            responseRecord.data = new Dictionary<string, object>();
            responseRecord.recordId = recordId;
            responseRecord.data.Add("text", translatedText);

            WebApiEnricherResponse response = new WebApiEnricherResponse();
            response.values = new List<WebApiResponseRecord>();
            response.values.Add(responseRecord);

            return (ActionResult)new OkObjectResult(response);
        }

        /// <summary>
        /// Use Cognitive Service to translate text from one language to antoher.
        /// </summary>
        /// <param name="myText">The text to translate</param>
        /// <param name="destinationLanguage">The language you want to translate to.</param>
        /// <returns>Asynchronous task that returns the translated text. </returns>
        async static Task<string> TranslateText(string myText, string destinationLanguage)
        {
            string host = "https://api.microsofttranslator.com";
            string path = "/V2/Http.svc/Translate";

            // NOTE: Replace this example key with a valid subscription key.
            string key = "064d8095730d4a99b49f4bcf16ac67f8";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>(myText, "en-us")
            };

            StringBuilder totalResult = new StringBuilder();

            foreach (KeyValuePair<string, string> i in list)
            {
                string uri = host + path + "?to=" + i.Value + "&text=" + System.Net.WebUtility.UrlEncode(i.Key);

                HttpResponseMessage response = await client.GetAsync(uri);

                string result = await response.Content.ReadAsStringAsync();

                // Parse the response XML
                System.Xml.XmlDocument xmlResponse = new System.Xml.XmlDocument();
                xmlResponse.LoadXml(result);
                totalResult.Append(xmlResponse.InnerText);
            }

            return totalResult.ToString();
        }
    }
}
```

Make sure to enter your own *key* value in the *TranslateText* method based on the key you got when signing up for the Translate Text API.

This example is a simple enricher that only works on one record at a time. This fact will become important later, when you're setting the batch size for the skillset.

## Step 3 - Test the function from Visual Studio

Press **F5** to run the program and test function behaviors. Use Postman or Fiddler to issue a call like the one shown below:

```http
POST https://localhost:7071/api/Translate
```

#### Request body

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

#### Response

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

Now that you have the default host key, test your function as follows:

```http
POST https://translatecogsrch.azurewebsites.net/api/Translate?code=[enter default host key here]
```

#### Request Body

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

### Step 7.1 - Challenge!!

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

Now we have our data enriched with pre-defined and custom skills. Now we just need to learn how to [query the data using Azure Portal](https://docs.microsoft.com/en-us/azure/search/search-explorer). Since you know the entities and the key phrases of the documents, try to search for them. 

Check the image below to see how Azure Search returns the metadata about your data. This image also helps to understand how to use the Search Explorer at the Azure Portal.

![](./resources/images/Portal-Search.png)

## Finished Solution

If you could not make it, [here](../resources/finished-solutions/finished-solution-Lab-custom-skills.md) is the challenge solution. You just need to follow the steps.

## Next Step

[Final Case Lab](../labs/lab-final-case.md) or 
[Back to Read Me](../README.md)
