# Azure Search Workshop - Building an Enterprise Cognitive Search Solution

## Welcome 
Cognitive Search adds data extraction, natural language processing (NLP), and image processing skills to an Azure Search indexing pipeline, making previously unsearchable or unstructured content more searchable. Information created by Cognitive Search Skills, such as entity recognition or image analysis, gets added to an index in Azure Search.

This one day training will focus on hands-on activities that develop proficiency with Cognitive Search, an Azure Search AI-oriented capability announced at [Microsoft Build 2018](https://www.microsoft.com/en-us/build). These labs assume an introductory to intermediate knowledge of [Visual Studio](https://www.visualstudio.com/vs/community/), the [Azure Portal](https://portal.azure.com), [Azure Functions](https://azure.microsoft.com/en-us/services/functions/) and [Azure Search](https://azure.microsoft.com/en-us/services/search/). If you are not at that skill level, we have prerequisite materials below that you **need** to complete prior to beginning this training.

## Goals
We will focus on hands-on activities to learn how to create a Cognitive Search solution for all types of business documents. The documents include pdfs, docs, ppts and images, as well as documents with multiple languages. 
In this training, you will create a data flow that uses cognitive skills to enrich your business documents. These enrichments will become part of an Azure Search index. 

At the end of this workshop, you should have learned:

+ **What** Cognitive Search is 
+ **How** to implement this Cognitive Search Solution
+ **Why** to use this solution with demos, POCs and other business scenarios


## Prerequisites
Since this is an AI training on top of Microsoft Azure Services, before we start you need:

+ **If if you don't have prior experience:**
1. **To Read:** [Visual Studio](https://docs.microsoft.com/en-us/visualstudio/ide/visual-studio-ide) tutorial
1. **To Read:** [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview) quick introduction 
1. **To Read:** [Azure Search](https://docs.microsoft.com/en-us/azure/search/search-what-is-azure-search) overview
1. **To Read:** [Using Postman](https://docs.microsoft.com/en-us/azure/search/search-fiddler) tutorial
1. **To Watch:** [MVA: C# Fundamentals](https://mva.microsoft.com/en-us/training-courses/c-fundamentals-for-absolute-beginners-16169?l=Lvld4EQIC_2706218949) short videos

+ **Mandatory**
1. **To Create:** You need a Microsoft Azure account to create the services we use in our solution. You can create a [free account](https://azure.microsoft.com/en-us/free/), use your [MSDN account](https://azure.microsoft.com/en-us/pricing/member-offers/credit-for-visual-studio-subscribers/) or use any other subscription where you have permission to create services.
1. **To Install:** [Visual Studio 2017](https://www.visualstudio.com/vs/) version version 15.5 or later, including the Azure development workload.
1. **To Install:** [Postman](https://www.getpostman.com/). To call the labs APIs.

## Agenda
Since you have finished the prerequisites, let's start the training. You just need to follow the workshop structure presented below.

+ [Introduction](01-Introduction.md) - 1 hour - Motivation, context, key concepts
+ [Solution Architecture](02-Solution-Architecture.md) - 1 hour - Diagram, use cases, deployment options and costs
+ [Environment Creation](03-Environment-Creation.md) - 1 hour - Using the Azure Portal, we will create the services we need fo the workshop
+ [Lab 1](04-Lab-1-Text-Skills.md) - 2 hours - Create a Cognitive Search Enrichment Process: **Text** Skills
+ [Lab 2](05-Lab-2-Image-Skills.md) - 1 hour - Create a Cognitive Search Skillset: **Image** Skills
+ [Lab 3](06-Lab-3-Custom-Skills.md) - 2 hours - Create a Cognitive Search Skillset with **Custom** Skills
+ [Final Case](07-Final-Case.md) - 0.5 hour - Brainstorm - Create a Cognitive Search Solution
+ [Q&A, Feedback and Survey](08-QA-Feedback-Survey.md) - 0.5 hour


## Workshop clean up
If you don't want to keep the solution up and running for future use, you should get rid of the environment after the course. Assuming that you created all services in the same resource group, the fastest way to clean up is by deleting it. This will permanently remove the Azure Search service, the Azure Function app and Azure Blob service (including the services and any stored content that you created for this workshop). In the portal, the resource group name is on the Overview page of each service.


## Useful Links
+ [Microsoft AI School](https://aischool.microsoft.com/learning-paths)
+ [Microsoft AI Platform](https://www.microsoft.com/en-us/ai)
+ [Microsoft AI Principles](https://www.microsoft.com/en-us/AI/our-approach-to-ai)
+ [Microsoft AI Ethics](https://aka.ms/ai-ethics)
+ [Microsoft AI Customer Cases](https://www.microsoft.com/en-us/ai/customer-stories)
+ [Microsoft AI Lab](https://www.ailab.microsoft.com/) **-> Released May, 2018**
+ [Microsoft AI TV](https://aka.ms/AzureTV) **-> Released May, 2018**
+ [Microsoft AI Analytics School](https://learnanalytics.microsoft.com/) 
+ [Microsoft Research Open Data](https://msropendata.com/)
+ [Cognitive Search Official Demo - JFK Files](https://jfk-demo.azurewebsites.net/)
+ [Cognitive Search Official Code - JFK Files](https://github.com/Microsoft/AzureSearch_JFK_Files)
+ [Cognitive Search Business Documents Demo - Wolters Kluwer](https://wolterskluwereap.azurewebsites.net/)
+ [Cognitive Search Business Oil & Gas Demo - Exxon Mobile](http://seismicsearch.azurewebsites.net/)
+ [Cognitive Search Business Healthcare Demo - CTakes](http://webmedsearch.azurewebsites.net/)
+ [Cognitive Services Enrichment Pipeline Demo](https://text-analytics-demo-dev.azurewebsites.net/)




## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


