# Lab 3: Create a Cognitive Search Skillset with **Custom** Skills

In this lab, you will learn [how to create a Custom Skill](https://docs.microsoft.com/en-us/azure/search/cognitive-search-custom-skill-interface)
and integrate it into the enrichment pipeline. Custom skills allow you to add any REST API transformation to the dataset, also feeding other transformations within the pipeline, if required.

You will use an [Azure Function](https://azure.microsoft.com/services/functions/) to wrap the [Content Moderator API](https://azure.microsoft.com/en-us/services/cognitive-services/content-moderator/) and detect incompliant documents in the dataset.

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

To see how the API works, and also to learn how to demo this technology in minutes, navigate to the [Content Moderator Text API console](https://docs.microsoft.com/en-us/azure/cognitive-services/content-moderator/try-text-api). Read the whole page, it should take you about four minutes. When you are done, scroll all the way up and click the first link on the page, [Text Moderation API](https://westus.dev.cognitive.microsoft.com/docs/services/57cf753a3f9b070c105bd2c1/operations/57cf753a3f9b070868a1f66f). It will open a control panel for Cognitive Services, as you can see in the image below.

![Cognitive Services Panel](../resources/images/lab-custom-skills/panel.png)

Now, clicking the blue buttons, choose the region where you created your Content Moderator API, that should be the same region you are using for all services in this training. Now set the values as listed below:

+ autocorrect = true
+ PII = true
+ listId = remove parameter (we don't have a list of prohibited terms to work with at this point)
+ classify = true
+ language = eng (The default example is in english)
+ Keep Content-Type as "text/plain"
+ Paste in your Content Moderator API key

Now you are ready to test the API you created on Azure Portal. Scroll down and check the suggested text in "Request body" section. It has PIIs like email, phone number, physical and IP addresses. It also has a profanity word. Scroll until the end of the page and click the blue "Send" button. The expected results are:

+ Response Status = 200 OK
+ Response Latency <= 1000 ms
+ Response content with a json file where you can read all of the detected problems. The "Index" field indicates the position of the term within the submitted text.

## Step 2 - Visual Studio

Visual Studio has [Tools for AI](https://visualstudio.microsoft.com/downloads/ai-tools-vs/) but you don't need it for this training since they are used to **create** custom AI projects, while in this training you are **using** AI through Azure Cognitive Search and Services.

### Step 2.1 - Checking Versions

Open your Visual Studio and click "Help / About Microsoft Visual Studio" on the main menu. You can try to use different versions, especially newer versions. But this training was created using the envivonment below and It is strongly recommended that you use the same versions. Please check if your system match this versions:

+ Visual Studio version 15.8.9
+ Microsoft .Net Framework version 4.7.03190
+ Azure Functions and Web Jobs Tools - 15.9.02046.0

![Versions](../resources/images/lab-custom-skills/versions.png)

If your Azure Functions Tools don't match, follow [this](https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-update-a-visual-studio-extension?view=vs-2017) procedure to update it.

If your Visual Studio version doesn't match, click "Help / Check for Updates" and follow the instructions.

If your .Net Framework version doesn't match, [download](https://www.microsoft.com/net/download) and install the last version.

### Step 2.2 - Preparing the solution

Open the Windows Explorer and find the folder "resources/azure-function-code/ where you cloned this repository. You need to locate the file ContentModerator.sln. There should be a ContentModerator folder in the same folder where you found the .sln file.

Double click, or hit enter, on the .sln file to open it in Visual Studio. Check your Solution Explorer on the right and confirm that you can see the same structure of the image below.

![Solution Structure](../resources/images/lab-custom-skills/structure.png)

>Note! This is not a C# training and this Azure Function application is a way to add the custom skill to the enrichment pipeline. Please note that good practices are not 100% used in the code (e.g the key wide open and fixed in the code). For enterprise grade solutions, this code should be adapted to all good practices, business and security requirements.

Click on the Moderator.cs file on the Solution Explorer, it should open in the main window of your Visual Studio. Get familiar with the "using session", to learn which packages are used. Scroll down until the three "TO DO - Action Required" sessions of the code.

Follow the instructions of the three "TO DO" sections:

+ If necessary change "southcentralus" in the uriPrefix URL. This is the same endpoint of the first step of this lab
+ Add your key. This is the same key of the first step of this lab
+ If necessary, change the host url with the same endpoint of the first step of this lab, but without `https://` and without `/contentmoderator`

>Note! Our dataset has a text file with PII, in english. That's why we are only testing this capability, to enforce this regulation within the business documents. This simple usage was defined to keep the training simple. For advanced scenarios, you can use all other Content Moderator capabilities.

## Step 3 - Test the function from Visual Studio

Press **F5** or click the green arrow on the "ContentModerator" button to run the solution. you should expect a "cmd" window with some information and the local URL of the endpoint in the end of the log, like you can see in the image below.

![Cmd window](../resources/images/lab-custom-skills/cmd.png)

Now use Postman to issue a call like the one shown below:

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
               "text":  "Email: abcdef@abcd.com, phone: 6657789887, IP: 255.255.255.255, 1 Microsoft Way, Redmond, WA 98052"
            }
        }
   ]
}
```

### Response

You should see a response similar to the following example:

```json
{
    "RecordId": "a1",
    "Data": {
        "PII": {
            "Email": [
                {
                    "Detected": "abcdef@abcd.com",
                    "SubType": "Regular",
                    "Text": "abcdef@abcd.com",
                    "Index": 7
                }
            ],
            "IPA": [
                {
                    "SubType": "IPV4",
                    "Text": "255.255.255.255",
                    "Index": 47
                }
            ],
            "Phone": [
                {
                    "CountryCode": "US",
                    "Text": "6657789887",
                    "Index": 31
                }
            ],
            "Address": [
                {
                    "Text": "1 Microsoft Way, Redmond, WA 98052",
                    "Index": 64
                }
            ]
        }
    },
    "Errors": [],
    "Warnings": []
}   ]
}
```

## Step 4 - Publish the function to Azure

When you are satisfied with the function behavior, you can publish it.

1. In **Solution Explorer**, right-click the project and select **Publish**. Choose **Create New** > **Publish**.

2. If you haven't already connected Visual Studio to your Azure account, select **Add an account....**

3. Follow the on-screen prompts. You are asked to specify the Azure account, the resource group, the hosting plan, and the storage account you want to use. You will have the option to create a new resource group (or use the one you've already created for these labs), a new hosting plan, and a storage account if you don't already have one in your Azure Account. When finished, select **Create**.

4. After the deployment is complete, note the Site URL. It is the address of your function app in Azure.

5. In the [Azure portal](https://portal.azure.com), navigate to the Resource Group, and look for the Content Moderator Function you published. Under the **Manage** section, you should see Host Keys. Select the **Copy** icon for the *default* host key.  

## Step 5 - Test the function in Azure

Now that you have the default host key, use Postman to test your function as follows:

```http
POST https://[enter you Function name here].azurewebsites.net/api/Moderate?code=[enter default host key here]
```

### Request Body

```json
{
   "values": [
        {
            "recordId": "a1",
            "data":
            {
               "text":  "Email: abcdef@abcd.com, phone: 6657789887, IP: 255.255.255.255, 1 Microsoft Way, Redmond, WA 98052"
            }
        }
   ]
}
```

This should produce a similar result to the one you saw previously when running the function in the local environment.

### Step 5.1 - Update SSL Settings

All Azure Functions created after June 30th, 2018 have disabled TLS 1.0, which is not currently compatible with custom skills. Today, August 2018, Azure Functions default TLS is 1.2, which is causing issues. This is a **required workaround**:

1. In the Azure portal, navigate to the Resource Group, and look for the Content Moderator Function you published. Under the Platform features section, you should see SSL.
2. After selecting SSL, you should change the **Minimum TLS version** to 1.0 (TLS 1.2 functions are not yet supported as custom skills).

For more information, click [here](https://docs.microsoft.com/en-us/azure/search/cognitive-search-create-custom-skill-example#update-ssl-settings ) .

## Step 6 - Cleaning the environment again

Let's do the same cleaning process of lab 2. Save all scripts (API calls) you did until here, including the definition json files you used in the "body" field.

### Step 6.1

Let's start deleting the index and the indexer. You can use Azure Portal or API calls:

1. [Deleting the indexer](https://docs.microsoft.com/en-us/rest/api/searchservice/delete-indexer) - Just use your service, key and indexer name
2. [Deleting the index](https://docs.microsoft.com/en-us/rest/api/searchservice/delete-index) - Just use your service, key and indexer name

### Step 6.2

If you didn't use the portal to delete the indexer, skillsets can only be deleted through an HTTP command, let's use another API call request to delete it. Don't forget to add your skillset name in the URL.

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
        "description": "Our new content moderator custom skill",
        "uri": "https://[enter function name here].azurewebsites.net/api/Moderate?code=[enter default host key here]",
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
            "targetName": "moderatedText"
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

Now we have our data enriched with pre-defined and custom skills. Use the Search Explorer within your Azure Search service in the Azure Portal to query the data. Create a query to identify documents with compliance issues, the moderated documents.

## Finished Solution

If you could not make it, [here](../resources/finished-solutions/finished-solution-Lab-custom-skills.md) is the challenge solution. You just need to follow the steps.

## Next Step

[Final Case Lab](../labs/lab-final-case.md) or
[Back to Read Me](../README.md)
