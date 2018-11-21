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

While the introduction has a pdf version, this one won't have one. The idea is to keep on editing this presentation for each delivery.

## Azure Functions Code

Important to comment:

1. language: eng (Content Moderator) x en (Azure Search)
1. 1024 limit
1. host x region. Without and with https
1. Complex types explanation: why we are transforming in boolean
1. You can use [this](https://github.com/Rodrigossz/AzureCognitiveSkill) Azure Functions solution to demo the complex type that is returned
1. Explanation: we are only using PII

## Final Case - Finished Solution

Expected:

1. Sizing discussion: [Tier](https://azure.microsoft.com/en-us/pricing/details/search/) x [SLA](https://azure.microsoft.com/en-us/support/legal/sla/search/v1_0/), Units. For SLA is required 2 or more copies. Basic tear limit is 2 GB, Standard has 300 GB total.

1. Skills
    - Text Skills
    - OCR (text in images)
    - Image Analysis too (images scenes descriptions)
    - Language detection
    - Celebrity Detection (famous somelier)
    - Entity Extraction (location, organizations)
    - Important to discuss the order of the transformations

1. Other AI envolved
    - Translation API
    - Recommendation API created with AML and running on AKS

1. South America means 2 more languages after the orginal docs in English: Portuguese and Spanish. It is expected discussions like:
    - Save all fields in 3 languages? Storage costs. Write once and read many
    - Translate on the fly? Data out and API usage costs, latency

1. Other Technologies: CosmosDB for the product catalog? Blob Storage? Bot Services? Batch AI to train the recomendation API?