# Instructor Notes

This folder has resources for the instructor listed below.

## Opening Presentation

Use the KMB-Opening.pptx presentation to:

1. Presenters name
1. Start your training
1. Polls
1. Customized Agenda
1. Announcements
1. Known bugs not fixed yet
1. CSW x KMB difference (slide 4)
1. Breaking News
1. Spektra Instructions
1. Github link - <http://aka.ms/kmb>
1. Survey URL
1. Talk about environment cleaning, to avoid costs.

While the introduction has a pdf version, this one won't have one. The idea is to keep on editing this presentation for each delivery.

## Image Skills Lab

OCR: Expected performance is 3 seconds per image.  For documents with multiple images, you can apply those 3 seconds for every one of them. The product performance is under constant optimizations and improvements are expexted soon.

## Custom Skills LAB

Important to comment in the Azure Functions Code:

1. language: eng (Content Moderator) x en (Azure Search)
1. For now, language is not dynamic (not using the document extracted language). So, we are not moderating spanish docs very well.
1. 1024 limit
1. host x region. Without and with https
1. Complex types explanation: why we are transforming in boolean
1. You can use [this](https://github.com/Rodrigossz/AzureCognitiveSkill) Azure Functions solution to demo the complex type that is returned
1. Explanation: we are only using PII

## Bot LAB

+ You can demo searches for linux e LearnAI. Ctrl+click the URL to open the IMAGES, not the other files.
+ Search for moderated documents
+ Explain details of Framework 4 code

## Final Case - Finished Solution

Expected:

+ Complex Scenarios
  + Multiple datasets:
    + More than one datasource: product catalog, reviews, ERP, CRM
    + Data source is too big, data needs to be partitioned 
  + Multiple Indexes
    + One for each region, in each languages
    + Each region demands different fields or ranking or Analyzer or suggester
  + Multiple Indexers
    + Datasources updated in different times
    + Each product or region requires different schdules
  + Multiple Skillsets
    + VIVINO/CREDIT/Logistics/Other custom skillset with a different schedule update - "runs once a day"

+ Sizing discussion: [Tier](https://azure.microsoft.com/en-us/pricing/details/search/) x [SLA](https://azure.microsoft.com/en-us/support/legal/sla/search/v1_0/), Units. For SLA is required 2 or more copies. Basic tear limit is 2 GB, Standard has 300 GB total.

+ Skills
  + Text Skills
  + OCR (text in images)
  + Image Analysis too (images scenes descriptions)
  + Language detection
  + Celebrity Detection (famous somelier)
  + Entity Extraction (location, organizations)
  + Important to discuss the order of the transformations

+ Other AI envolved
  + Translation API
  + Recommendation API created with AML and running on AKS

+ South America means 2 more languages after the orginal docs in English: Portuguese and Spanish. It is expected discussions like:
  + Save all fields in 3 languages? Storage costs. Write once and read many
  + Translate on the fly? Data out and API usage costs, latency

+ Other Technologies: CosmosDB for the product catalog? Blob Storage? Bot Services? Batch AI to train the recomendation API?