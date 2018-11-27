# Lab 2: Create a Cognitive Search Skillset with **Image** Skills

In this lab, you will verify the lack of image processing results we got from the previous lab and fix it by adding image analysis skill set to our pipeline.

## Part I: The Problem Statement

There are png and jpg images within the provided dataset. If you decided to bring your own data, it was suggested to also include images. But we did not add any predefined skillsets for image analysis. This is exactly what you will do now, but first, let's check out the kind of problems we could expect to see if we used the Language Detection, Text Split, Named Entity Recognition and Key Phrase Extraction Skills on images with steps 1 and 2.

### Step 1 - Checking warning message from the API

Let's check the indexer status again, it has valuable information about our "images problem". You can use the same command we used in the previous lab (pasted below for convenience). If you used another indexer name, just change it in the URL.

```http
GET https://[your-service-name].search.windows.net/indexers/demoindexer/status?api-version=2017-11-11-Preview
Content-Type: application/json
api-key: [api-key]
```

If you check the response messages for any of the png or jpg files in the results, there will be warnings about missing text for the images.

### Step 2 - Existing skills will show no results

Let's again repeat a previous lab request, but with another analysis. you will re-execute the step to verify content.  

```http
GET https://[your-service-name].search.windows.net/indexes/demoindex/docs?search=*&$select=blob_uri,organizations,languageCode,keyPhrases&api-version=2017-11-11-Preview
api-key: [api-key]
```

Send the request and go to any result set about an image file like jpg or png, like these images listed in the image below. Note that no organizations, languageCode and keyPhrases are returned for the images, jpg or png files. That's because they were not created.

![No metada created for images](../resources/images/lab-image-skills/postman-no-data.png)

>Tip: if any image is in the top results: There is a magnifying glass button in the top right of the results screen. Click on this button to open the Search box, in the text box type **jpg** or **png** and click on the find next button to find this result.

## PART II: How to fix it

The next steps will guide you through a challenge and don't worry if you get stuck (that's why it's a challenge!), you will share the solution, too.

### Step 3 - Learning the OCR image skill

Two of the nine [predefined skills](https://docs.microsoft.com/en-us/azure/search/cognitive-search-predefined-skills) are related to image analysis. Your first assignment is to read about how to use them using this [link](https://docs.microsoft.com/en-us/azure/search/cognitive-search-concept-image-scenarios).

You will add OCR to the cognitive search pipeline, this skill set will read text from the images within our dataset. Here is a [link](https://docs.microsoft.com/en-us/azure/search/cognitive-search-skill-ocr) where you can read more details.

### Step 4 - Cleaning the environment

You need to prepare the environment to add the image analysis you will create. The most practical approach is to delete the objects from Azure Search and rebuild them. This also avoids redundancy of similar information. This cleaning also reduces cost, two replicated/similar indexes will use space os the service. Last, but not least: to teach about DELETES is also an objective of this training. With the exception of the data source, you will delete everything else. Resource names are unique, so by deleting an object, you can recreate it using the same name.

 Save all the scripts (API calls) you've done up until this point, including the definition json files you used in the "body" field. Let's start deleting the index and the indexer. You can use Azure Portal or API calls:

1. [Deleting the indexer - API call](https://docs.microsoft.com/en-us/rest/api/searchservice/delete-indexer) - Just use your service, key and indexer name
1. [Deleting the index](https://docs.microsoft.com/en-us/rest/api/searchservice/delete-index) - Just use your service, key and indexer name
1. [Deleting the Skillset](https://docs.microsoft.com/en-us/rest/api/searchservice/delete-skillset) - Just use your service, key and skillset name

Status code 204 is returned on a successful deletion.

### Step 5 - Recreating the environment - Challenge

In this challenge, you will perform the following steps:

1. ~~Create the services at the portal~~ **Not required, we did not delete it**.
1. ~~Create the Data Source~~ **Not required, we did not delete it**.
1. Recreate the Skillset
1. Recreate the Index
1. Recreate the Indexer
1. Check Indexer Status - If you don't have a different result, something went wrong.
1. Check the Index Fields - Check the image fields you just created.
1. Check the data - If you don't have a different result, something went wrong.

#### Step 5.1 Creating the skillset with the OCR image skillset

Use the same skillset definition from Lab 1,  add in the [OCR image analysis skill](https://docs.microsoft.com/en-us/azure/search/cognitive-search-skill-ocr) to your skillset. The objectives are:

1. Save the text extracted from OCR into the index
1. Submit the text extracted from OCR to language detection, key phrases and entity detection.

#### Step 5.2 - Recreating the index and indexer

Skipping the services and the data source creation, repeat the other steps of the Lab 1, in the same order. Use the previous lab as a reference.

**TIP 1:** What you need to do:

1. Create a new index exactly like the one we did in Lab 1 but with an extra field for the OCR text from the images. Name suggestion: myOcrText. You can use the same json body field and add the new OCR field in the end
1. Create a new indexer exactly like the one we did in Lab 1, but with and extra mapping for the new skill and the new field listed above. You can use the same json body field and add the new OCR mapping in the end
1. Check the indexer execution status as you did in the previous lab

**TIP 2:** Your new field in the Index must have the [Collection Data Type](https://docs.microsoft.com/en-us/rest/api/searchservice/Supported-data-types?redirectedfrom=MSDN).

**TIP 3:** Your indexer sourceFieldName for the OCR text field has to be /document/normalized_images/*/myOcrText if your field is named myOcrText.  

#### Step 5.3 - Validation

Run the same query of the Step 2, the URL is pasted below. Now you should see organizations, languageCode and keyPhrases for most of the images.

```http
GET https://[your-service-name].search.windows.net/indexes/demoindex/docs?search=*&$select=blob_uri,organizations,languageCode,keyPhrases&api-version=2017-11-11-Preview
Content-Type: application/json
api-key: [api-key]
```

Now run the query below to check the OCR text extracted from the images. You Should see text for most of the images.

```http
GET https://[your-service-name].search.windows.net/indexes/demoindex/docs?search=*&$select=blob_uri,myOcrText&api-version=2017-11-11-Preview
Content-Type: application/json
api-key: [api-key]
```

#### Step 5.4 - Portal

Log into the Azure portal and verify the creation of the skillset, index and indexers in the Azure Search dashboard. If nothing is missed, use the Search Explorer to do the searches below. Click on the files URLs (crtrl+click) to check if the AI services created the metadada as expected.

+ Search for "linux"

```http
search=myOcrText:linux&querytype=full
```

+ Search for "microsoft"

```http
search=myOcrText:microsoft&querytype=full
```

+ Search for "Learning", what will show you an image of the LearnAI Team portal, who created this training.

```http
search=myOcrText:Learning&querytype=full
```

## Finished Solution

If you could not make it, [here](../resources/finished-solutions/finished-solution-lab-image-skills.md) is the challenge solution. You just need to follow the steps.

## Next Step

+ [Custom Skills Lab](../labs/lab-custom-skills.md) or [Back to Read Me](../README.md)