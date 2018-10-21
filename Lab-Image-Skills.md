# Lab 2: Re-create a Cognitive Search Skillset with **Image** Skills

In this lab, we will verify the lack of image processing results we got from the previous lab and fix it by adding image analysis skill sets to our pipeline. 

## The Problem
There were png and jpg images within the provided dataset. If you decided to bring your own data, it was suggested to also include images. But we did not add any predefined skillsets for image analysis. This is exactly what we will do now, but first, let's check out the problem with steps 1 and 2.


### Step 1 - Checking part of the problem
Let's check the indexer status again, it has valuable information about our "images problem". You can use the same command we used in the previous lab (pasted below for convenience). If you used another indexer name, just change it in the URL.

```http
GET https://[servicename].search.windows.net/indexers/demoindexer/status?api-version=2017-11-11-Preview
api-key: [api-key]
Content-Type: application/json
```
If you check the response messages for any of the png or jpg files, there will be warnings and not data.  

### Step 2 - Checking the other part of the problem
Now let's again repeat a previous lab request, but with another analysis. We will re-execute the step to verify content, but this time querying all fields.  

```http
GET https://[servicename].search.windows.net/indexes/demoindex/docs?search=*&$select=*&api-version=2017-11-11-Preview
api-key: [api-key]
Content-Type: application/json
```
You will probably see something similar to the image below - no information for the images we have.

![](./media/no-images-info.png)


## How can we fix it?

We will fix it, but there is a challenge for you increase your learning about Predefined Skills. The next steps will guide you through the challenge and don't worry if you get stuck (that's why it's a challenge!), we will share the solution, too. 

### Step 3 - Learning
Two of the nine [predefined skills](https://docs.microsoft.com/en-us/azure/search/cognitive-search-predefined-skills) are related to image analysis. Your first assignment is to read about how to use them using this [link](https://docs.microsoft.com/en-us/azure/search/cognitive-search-concept-image-scenarios). 

We will add OCR to our cognitive search pipeline, this skill set will read text from the images within our dataset. Here is a [link](https://docs.microsoft.com/en-us/azure/search/cognitive-search-skill-ocr) where you can read more details. 

### Step 4 - Cleaning the environment
We need to prepare the environment to add the image analysis we will create. The most practical approach is to delete the objects from Azure Search and rebuild them. With the exception of the data source, we will delete everything else. Resource names are unique, so by deleting an object, you can recreate it using the same name. 

#### Step 4.1
 Save all scripts (API calls) you did until here, including the definition json files you used in the "body" field. Let's start deleting the index and the indexer. You can use Azure Portal or API calls:
1. [Deleting the indexer](https://docs.microsoft.com/en-us/rest/api/searchservice/delete-indexer) - Just use your service, key and indexer name
2. [Deleting the index](https://docs.microsoft.com/en-us/rest/api/searchservice/delete-index) - Just use your service, key and indexer name

#### Step 4.2
Skillsets can only be deleted through an HTTP command, let's use another API call request to delete it. If you used another skillset name, just change it in the URL.

```http
DELETE https://[servicename].search.windows.net/skillsets/demoskillset?api-version=2017-11-11-Preview
api-key: [api-key]
Content-Type: application/json
```
Status code 204 is returned on successful deletion.

### Step 5 - Recreating the environment - Challenge!!
Now it is your time to guide the work. We are using a basic Azure Search service, so we can create skillsets with up to 5 skills. Since we currently are using 4, from the previous lab, we can add one more for image processing.

#### Step 5.1
Use the same skillset definition from Lab 1, but add in the [OCR image analysis skill](https://docs.microsoft.com/en-us/azure/search/cognitive-search-skill-ocr) you read about in Step 3. We suggest you add them at the end of the JSON of the body syntax definition. 

#### Step 5.2
Skipping the services and the data source creation, repeat the other steps of the Lab 1, in the same order. Use the previous lab as a reference.

1. ~~Create the services at the portal~~ **Not required, we did not delete it**.
2. ~~Create the Data Source~~ **Not required, we did not delete it**.
3. Recreate the Skillset
4. Recreate the Index
5. Recreate the Indexer
6. Check Indexer Status - Here you can repeat the same verification of Lab 2, Step 1. If you don't have a different result, something went wrong.  
7. Check the Index Fields - Check the image fields you just created.
8. Check the data - Here you can repeat the same verification of Lab 2, Step 2. If you don't have a different result, something went wrong.

**TIP 1:** What you need to do:
1. Create a new skillset exactly like the one we did in Lab 1, but with an extra skill, the OCR skillset. You can use the same json body field and add the new OCR skill in the end.
2. Create a new index exactly like the one we did in Lab 1 but with an extra field for the OCR text from the images. Name suggestion: myOCRtext. You can use the same json body field and add the new OCR field in the end.
3. Create a new indexer exactly like the one we did in Lab 1, but with and extra mapping for the new skill and the new field listed above. You can use the same json body field and add the new OCR mapping in the end.

**TIP 2:** Your new field in the Index must have the [Collection Data Type](https://docs.microsoft.com/en-us/rest/api/searchservice/Supported-data-types?redirectedfrom=MSDN).

**TIP 3:** You can query only the OCR field, to better visualize the results. Suppose that your new index field name is myOcrTex. You can query it using:
```http
GET https://[servicename].search.search.windows.net/indexes/rodindex2/docs?search=*&$select=myOcrText&api-version=2017-11-11-Preview
api-key: [api-key]
Content-Type: application/json
```
**TIP 4:** Your indexer sourceFieldName for the OCR text field has to be /document/normalized_images/*/myOcrText if your field is named myOcrText.  

## Finished Solution
If you could not make it, [here](./resources/Finished-Solutions/Finished-Solution-Lab-2.md) is the challenge solution. You just need to follow the steps.

## Next Step
+ [Custom Skills Lab](./Lab-Custom-Skills.md) or [Back to Main Menu](./readme.md)


