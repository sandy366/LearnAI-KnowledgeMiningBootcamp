# Finished Solution - Custom Skills Lab

Hello!

Here are the body requests for the custom skills lab. Don't forget to adjust the URLs to use your Azure Search service name.

## Delete Skillset

```http
https://[your-service-name].search.windows.net/skillsets/demoskillset?api-version=2017-11-11-Preview
```

## Delete Index

```http
https://[your-service-name].search.windows.net/indexes/demoindex?api-version=2017-11-11-Preview
```

## Delete Indexer

```http
https://[your-service-name].search.windows.net/indexers/demoindexer?api-version=2017-11-11-Preview
```

## Skillset

```json
{
  "description":
  "Extract entities, detect language and extract key-phrases. Also does OCR and submit everything to Content Moderator",
  "skills":
  [
     {
        "description": "Extract text (plain and structured) from image.",
        "@odata.type": "#Microsoft.Skills.Vision.OcrSkill",
        "context": "/document/normalized_images/*",
        "defaultLanguageCode": "en",
        "detectOrientation": true,
        "inputs": [
          {
            "name": "image",
            "source": "/document/normalized_images/*"
          }
        ],
        "outputs": [
          {
            "name": "text", "targetName": "myOcrText"
          }
        ]
    },
    {
      "@odata.type": "#Microsoft.Skills.Text.MergeSkill",
      "description": "Create mergedText, which includes all the textual representation of each image inserted at the right location in the content field.",
      "context": "/document",
      "insertPreTag": " ",
      "insertPostTag": " ",
      "inputs": [
        {
          "name":"text", "source": "/document/content"
        },
        {
          "name": "itemsToInsert", "source": "/document/normalized_images/*/myOcrText"
        },
        {
          "name":"offsets", "source": "/document/normalized_images/*/contentOffset"
        }
      ],
      "outputs": [
        {
          "name": "mergedText", "targetName" : "mergedText"
        }
      ]
    },
    {
      "@odata.type": "#Microsoft.Skills.Text.NamedEntityRecognitionSkill",
      "categories": [ "Organization" ],
      "defaultLanguageCode": "en",
      "inputs": [
        {
          "name": "text", "source": "/document/mergedText"
        }
      ],
      "outputs": [
        {
          "name": "organizations", "targetName": "organizations"
        }
      ]
    },
    {
      "@odata.type": "#Microsoft.Skills.Text.LanguageDetectionSkill",
      "inputs": [
        {
          "name": "text", "source": "/document/mergedText"
        }
      ],
      "outputs": [
        {
          "name": "languageCode",
          "targetName": "languageCode"
        }
      ]
    },
    {
      "@odata.type": "#Microsoft.Skills.Text.SplitSkill",
      "textSplitMode" : "pages",
      "maximumPageLength": 50000,
      "inputs": [
      {
        "name": "text",
        "source": "/document/mergedText"
      },
      {
        "name": "languageCode",
        "source": "/document/languageCode"
      }
    ],
    "outputs": [
      {
            "name": "textItems",
            "targetName": "pages"
      }
    ]
  },
  {
      "@odata.type": "#Microsoft.Skills.Text.KeyPhraseExtractionSkill",
      "context": "/document/pages/*",
      "inputs": [
        {
          "name": "text", "source": "/document/pages/*"
        },
        {
          "name":"languageCode", "source": "/document/languageCode"
        }
      ],
      "outputs": [
        {
          "name": "keyPhrases",
          "targetName": "keyPhrases"
        }
      ]
    },
        {
        "@odata.type": "#Microsoft.Skills.Custom.WebApiSkill",
        "description": "Our new moderator custom skill",
        "uri": "https://cognitiveskill20181107032017.azurewebsites.net/api/ContentModerator?code=bA/CVOmqtLEpEEGRiedDiMR5aPybUcU1Pa3d1cB4POnrOYEOf/4Zyw==",
        "batchSize":1,
        "context": "/document",
        "inputs": [
          {
            "name": "text",
            "source": "/document/mergedText"
          }
        ],
        "outputs": [
          {
            "name": "text",
            "targetName": "moderatedText"
          }
        ]
    }
  ]
}
```

## Index

```json
{
  "fields": [
    {
      "name": "id",
      "type": "Edm.String",
      "key": true,
      "searchable": true,
      "filterable": false,
      "facetable": false,
      "sortable": true
    },
     {
      "name": "blob_uri",
      "type": "Edm.String",
      "searchable": true,
      "filterable": false,
      "facetable": false,
      "sortable": true
    },
    {
      "name": "content",
      "type": "Edm.String",
      "sortable": false,
      "searchable": true,
      "filterable": false,
      "facetable": false
    },
    {
      "name": "languageCode",
      "type": "Edm.String",
      "searchable": true,
      "filterable": false,
      "facetable": false
    },
    {
      "name": "keyPhrases",
      "type": "Collection(Edm.String)",
      "searchable": true,
      "filterable": false,
      "facetable": false
    },
    {
      "name": "organizations",
      "type": "Collection(Edm.String)",
      "searchable": true,
      "sortable": false,
      "filterable": false,
      "facetable": false
    },
    {
      "name": "myOcrText",
      "type": "Collection(Edm.String)",
      "searchable": true,
      "filterable": false,
      "facetable": false
    } ,
   {
      "name": "moderatedText",
      "type": "Edm.Boolean",
      "searchable": false,
      "sortable": false,
      "filterable": true,
      "facetable": false
    }
  ]
}
```

## Indexer

```json
{
  "dataSourceName" : "demodata",
  "targetIndexName" : "demoindex",
  "skillsetName" : "demoskillset",
  "fieldMappings" : [
        {
          "sourceFieldName" : "metadata_storage_path",
          "targetFieldName" : "id",
          "mappingFunction" :
            { "name" : "base64Encode" }
        },
        {
          "sourceFieldName" : "content",
          "targetFieldName" : "content"
        },
         {
          "sourceFieldName" : "metadata_storage_path",
          "targetFieldName" : "blob_uri"
        }
   ],
  "outputFieldMappings" :
  [
        {
          "sourceFieldName" : "/document/organizations",
          "targetFieldName" : "organizations"
        },
        {
          "sourceFieldName" : "/document/pages/*/keyPhrases/*",
          "targetFieldName" : "keyPhrases"
        },
        {
            "sourceFieldName": "/document/languageCode",
            "targetFieldName": "languageCode"
        },
         {
            "sourceFieldName": "/document/normalized_images/*/myOcrText",
            "targetFieldName": "myOcrText"
        },
        {
            "sourceFieldName": "/document/moderatedText",
            "targetFieldName": "moderatedText"
        }
  ],
  "parameters":
  {
      "maxFailedItems":-1,
      "maxFailedItemsPerBatch":-1,
      "configuration":
    {
        "dataToExtract": "contentAndMetadata",
         "imageAction": "generateNormalizedImages"
        }
  }
}

```

## Check Status

```http
GET https://[your-service-name].search.windows.net/indexers/demoindexer/status?api-version=2017-11-11-Preview
Content-Type: application/json
api-key: [api-key]
```

## Check files and the moderated text indicator

```http
GET https://[your-service-name].search.windows.net/indexes/demoindex/docs?search=*&$select=blob_uri,moderatedText,organizations&api-version=2017-11-11-Preview
Content-Type: application/json
api-key: [api-key]
```

## Filter moderated content using Azure Search Explorer

```http
$select=blob_uri,moderatedText,content&$filter=moderatedText eq true
```

## Next Step

[Bots Lab](../../labs/lab-bot-business-documents.md) or
[Back to Read Me](../../README.md)