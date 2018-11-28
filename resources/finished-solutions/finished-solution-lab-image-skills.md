# Finished Solution - Image Skills Lab

Hello!

Here are the body requests for the Image Skills lab. Don't forget to adjust the URLs to use your Azure Search service name.

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
  "Extract entities, detect language and extract key-phrases",
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
          "targetFieldName" : "metadata_storage_path",
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
api-key: [api-key]
Content-Type: application/json
```

## Check organizations, languageCode and keyPhrases

```http
https://[your-service-name].search.windows.net/indexes/demoindex/docs?search=*&$select=blob_uri,organizations,languageCode,keyPhrases&api-version=2017-11-11-Preview
api-key: [api-key]
Content-Type: application/json
```

## Check the OCR Content extracted

```http
GET https://[your-service-name].search.windows.net/indexes/demoindex/docs?search=*&$select=blob_uri,myOcrText&api-version=2017-11-11-Preview
api-key: [api-key]
Content-Type: application/json
```

## Next Step

[Custom Skills Lab](../../labs/lab-custom-skills.md) or
[Back to Read Me](../../README.md)