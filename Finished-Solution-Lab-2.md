# Finished Solution for Lab 2
Hello!

Here are the body requests for the solution to Lab 2. Don't forget to adjust the URLs to use your Azure Search service name.

## Skillset
```json
{
    "@odata.context": "https://[servicename].search.windows.net/$metadata#skillsets/$entity",
    "@odata.etag": "\"0x8D5B9CB6F0A77E2\"",
    "name": "rodskillset2",
    "description": "Extract entities, detect language and extract key-phrases",
    "skills": [
        {
            "@odata.type": "#Microsoft.Skills.Text.NamedEntityRecognitionSkill",
            "description": null,
            "context": null,
            "inputs": [
                {
                    "name": "text",
                    "source": "/document/content"
                }
            ],
            "outputs": [
                {
                    "name": "organizations",
                    "targetName": "organizations"
                }
            ],
            "categories": [
                "Organization"
            ],
            "defaultLanguageCode": "en",
            "minimumPrecision": null
        },
        {
            "@odata.type": "#Microsoft.Skills.Text.LanguageDetectionSkill",
            "description": null,
            "context": null,
            "inputs": [
                {
                    "name": "text",
                    "source": "/document/content"
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
            "description": null,
            "context": null,
            "inputs": [
                {
                    "name": "text",
                    "source": "/document/content"
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
            ],
            "defaultLanguageCode": null,
            "textSplitMode": "pages",
            "maximumPageLength": 4000
        },
        {
            "@odata.type": "#Microsoft.Skills.Text.KeyPhraseExtractionSkill",
            "description": null,
            "context": "/document/pages/*",
            "inputs": [
                {
                    "name": "text",
                    "source": "/document/pages/*"
                },
                {
                    "name": "languageCode",
                    "source": "/document/languageCode"
                }
            ],
            "outputs": [
                {
                    "name": "keyPhrases",
                    "targetName": "keyPhrases"
                }
            ],
            "defaultLanguageCode": null,
            "maxKeyPhraseCount": null
        },
        {
            "@odata.type": "#Microsoft.Skills.Vision.OcrSkill",
            "description": "Extracts text (plain and structured) from image.",
            "context": "/document/normalized_images/*",
            "inputs": [
                {
                    "name": "image",
                    "source": "/document/normalized_images/*"
                }
            ],
            "outputs": [
                {
                    "name": "text",
                    "targetName": "myOcrText"
                }
            ],
            "textExtractionAlgorithm": null,
            "defaultLanguageCode": null,
            "detectOrientation": true
        }
    ]
}
```

## Index
```json
{
    "@odata.context": "https://[servicename].search.windows.net/$metadata#indexes/$entity",
    "@odata.etag": "\"0x8D5B9CB96002CA5\"",
    "name": "rodindex2",
    "fields": [
        {
            "name": "id",
            "type": "Edm.String",
            "searchable": true,
            "filterable": false,
            "retrievable": true,
            "sortable": true,
            "facetable": false,
            "key": true,
            "indexAnalyzer": null,
            "searchAnalyzer": null,
            "analyzer": null,
            "synonymMaps": []
        },
        {
            "name": "content",
            "type": "Edm.String",
            "searchable": true,
            "filterable": false,
            "retrievable": true,
            "sortable": false,
            "facetable": false,
            "key": false,
            "indexAnalyzer": null,
            "searchAnalyzer": null,
            "analyzer": null,
            "synonymMaps": []
        },
        {
            "name": "languageCode",
            "type": "Edm.String",
            "searchable": true,
            "filterable": false,
            "retrievable": true,
            "sortable": true,
            "facetable": false,
            "key": false,
            "indexAnalyzer": null,
            "searchAnalyzer": null,
            "analyzer": null,
            "synonymMaps": []
        },
        {
            "name": "keyPhrases",
            "type": "Collection(Edm.String)",
            "searchable": true,
            "filterable": false,
            "retrievable": true,
            "sortable": false,
            "facetable": false,
            "key": false,
            "indexAnalyzer": null,
            "searchAnalyzer": null,
            "analyzer": null,
            "synonymMaps": []
        },
        {
            "name": "organizations",
            "type": "Collection(Edm.String)",
            "searchable": true,
            "filterable": false,
            "retrievable": true,
            "sortable": false,
            "facetable": false,
            "key": false,
            "indexAnalyzer": null,
            "searchAnalyzer": null,
            "analyzer": null,
            "synonymMaps": []
        },
        {
            "name": "myOcrText",
            "type": "Collection(Edm.String)",
            "searchable": true,
            "filterable": false,
            "retrievable": true,
            "sortable": false,
            "facetable": false,
            "key": false,
            "indexAnalyzer": null,
            "searchAnalyzer": null,
            "analyzer": null,
            "synonymMaps": []
        }
    ],
    "scoringProfiles": [],
    "defaultScoringProfile": null,
    "corsOptions": null,
    "suggesters": [],
    "analyzers": [],
    "tokenizers": [],
    "tokenFilters": [],
    "charFilters": []
}
```
## Indexer
```json
{
  "name":"rodindexer2",	
  "dataSourceName" : "demodata",
  "targetIndexName" : "rodindex2",
  "skillsetName" : "rodskillset2",
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
GET https://[servicename].search.windows.net/indexers/demoindexer/status?api-version=2017-11-11-Preview
api-key: [api-key]
Content-Type: application/json
```

## Check the OCR Content extracted
```http
GET https://[servicename].search.windows.net/indexes/demoindex/docs?search=*&$select=myOcrText&api-version=2017-11-11-Preview
api-key: [api-key]
Content-Type: application/json
```
## Next Step
[Back to Main Menu](./readme.md)



