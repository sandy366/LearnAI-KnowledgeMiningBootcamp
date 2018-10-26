# Azure Search Fundamentals

In this lab you will learn the basics of the Azure search service and how to ingest and index the provided dataset with the built-in tools.

## What is Azure Search

[Azure Search](https://docs.microsoft.com/en-us/azure/search/search-what-is-azure-search) is a search-as-a-service solution allowing developers to incorporate great search experiences into applications without managing infrastructure or needing to become search experts.

Developers look for PaaS services in Azure to achieve better and faster results in their apps. While search is a key to many types of applications, web search engines have set the bar high for search. Users expect: instant results, auto-complete as they type, highlighting hits within the results, great ranking, and the ability to understand what they are looking for, even if they spell it incorrectly or include extra words.

Search is a hard and rarely a core expertise area. From an infrastructure standpoint, it needs to have high availability, durability, scale, and operations. From a functionality standpoint, it needs to have ranking, language support, and geospatial capabilities.

![Example of Search Requirements](./resources/images/azure-search-images/AzureSearch-Example.png)

The example above illustrates some of the components users are expecting in their search experience. [Azure Search](https://docs.microsoft.com/en-us/azure/search/search-what-is-azure-search) can accomplish these user experience features, along with giving you [monitoring and reporting](https://docs.microsoft.com/en-us/azure/search/search-traffic-analytics), [simple scoring](https://docs.microsoft.com/en-us/rest/api/searchservice/add-scoring-profiles-to-a-search-index), and tools for [prototyping](https://docs.microsoft.com/en-us/azure/search/search-import-data-portal) and [inspection](https://docs.microsoft.com/en-us/azure/search/search-explorer).

## Typical Workflow

1. Provision service
    - You can create or provision an Azure Search service from the [portal](https://docs.microsoft.com/en-us/azure/search/search-create-service-portal) or with [PowerShell](https://docs.microsoft.com/en-us/azure/search/search-manage-powershell).
1. Create an index
    - An [index](https://docs.microsoft.com/en-us/azure/search/search-what-is-an-index) is a container for data, think "table". It has schema, [CORS options](https://docs.microsoft.com/en-us/aspnet/core/security/cors), search options. You can create it in the [portal](https://docs.microsoft.com/en-us/azure/search/search-create-index-portal) or during [app initialization](https://docs.microsoft.com/en-us/azure/search/search-create-index-dotnet).
1. Index your data
    - There are two ways to [populate an index with your data](https://docs.microsoft.com/en-us/azure/search/search-what-is-data-import). The first option is to manually push your data into the index using the Azure Search [REST API](https://docs.microsoft.com/en-us/azure/search/search-import-data-rest-api) or [.NET SDK](https://docs.microsoft.com/en-us/azure/search/search-import-data-dotnet). The second option is to point a [supported data source](https://docs.microsoft.com/en-us/azure/search/search-import-data-portal) to your index and let Azure Search automatically pull in the data on a schedule.
1. Search an index
    - When submitting search requests to Azure Search, you can use simple search options, you can [filter](https://docs.microsoft.com/en-us/azure/search/search-filters), [sort](https://docs.microsoft.com/en-us/rest/api/searchservice/add-scoring-profiles-to-a-search-index), [project](https://docs.microsoft.com/en-us/azure/search/search-faceted-navigation), and [page over results](https://docs.microsoft.com/en-us/azure/search/search-pagination-page-layout). You have the ability to address spelling mistakes, phonetics, and Regex, and there are options for working with search and [suggest](https://docs.microsoft.com/en-us/rest/api/searchservice/suggesters). These query parameters allow you to achieve deeper control of the [full-text search experience](https://docs.microsoft.com/en-us/azure/search/search-query-overview).

## Step 1 - Importing Data

The Azure Search service was created in the previous lab, so the next step is to create a data source. This will connect the service with the dataset, uploaded to the storage account.

- From the Overview tab, click on the**Import Data** option and right after, click on **Connect to Data Source**

![Example of Search Requirements](./resources/images/azure-search-images/import-data.png)

- Choose the **Azure Blob Storage** Data Source and name it as `lab1data`. Choose the **Content and Metadata** option, we want to index not only the files propreties but also their content. Choose the **Default** parsing mode, since the dataset also had images, and connect to the storage container created in the previous lab. You skip Blob Folder and Description.

![Example of Search Requirements](./resources/images/azure-search-images/data-source.png)

> Note: You may notice that you are offered the option to "add congitive skills". You can ignore this for now.

## Step 2 - Create the Index

An index is a collection of fields from your data source that can be searched. The index is how your search service knows in what ways your data should be searched.

- In Import data select Customize target index

- Enter a name for your index in the Index name field

- Select the Retrievable attribute's checkbox under metadata_storage_name.










## Step 2 - Create an Azure Search Index

An index is a persistent store of documents and other constructs used by an Azure Search service. An index is like a database that holds your data and can accept search queries. You define the index schema to map to the structure of the documents you wish to search, similar to fields in a database. These fields can have properties that tell things such as if it is full text searchable, or if it is filterable.  You can populate content into Azure Search by programmatically [pushing content](https://docs.microsoft.com/en-us/rest/api/searchservice/addupdate-or-delete-documents) or by using the [Azure Search Indexer](https://docs.microsoft.com/en-us/azure/search/search-indexer-overview) (which can crawl common datastores for data).

For this lab, we will use the [Azure Search Indexer for Cosmos DB](https://docs.microsoft.com/en-us/azure/search/search-howto-index-documentdb) to crawl the data in the Cosmos DB collection.

![Import Wizard](./resources/assets/AzureSearch-ImportData.png)

Within the Azure Search blade you just created, click **Import Data->Data Source->Cosmos DB**.

![Import Wizard for DocDB](./resources/assets/AzureSearch-DataSource.png)

Once you click this, choose a name for the Cosmos DB data source. If you completed the previous lab, `lab01.1-computer_vision`, choose the Cosmos DB account where your data resides as well as the corresponding Container and Collections. If you did not complete the previous lab, select "Or input a connection string" and find a friendly neighbor that is willing to provide their connection string. For both, the Database should be "images" and the Collection should be "metadata".

Click **OK**.

At this point Azure Search will connect to your Cosmos DB container and analyze a few documents to identify a default schema for your Azure Search Index. After this is complete, you can set the properties for the fields as needed by your application (e.g. you can "Customize target index").

>Note: You may see a warning that "_ts" fields are not valid field names. You can ignore this for our labs, but you can read more about it [here](https://docs.microsoft.com/azure/search/search-indexer-field-mappings).
>
>Note: You may notice that you are offered the option to "add congitive skills". You can ignore this for now.

Update the Index name to: **images**

Update the Key to: **id** (which uniquely identifies each document)

Set all fields to be **Retrievable** (to allow the client to retrieve these fields when searched)

Set the fields **Tags** to be **Filterable** (to allow the client to filter results based on these values)

Set the fields **Tags** to be **Facetable** (to allow the client to group the results by count, for example for your search result, there were "5 pictures that had a Tag of "beach")

Set the fields **Caption and Tags** to be **Searchable** (to allow the client to do full text search over the text in these fields)

![Configure Azure Search Index](./resources/assets/AzureSearch-ConfigureIndex.png)

At this point we will configure the Azure Search Analyzers.  At a high level, you can think of an analyzer as the thing that takes the terms a user enters and works to find the best matching terms in the Index.  Azure Search includes analyzers that are used in technologies like Bing and Office that have deep understanding of 56 languages.  

Click the **Analyzer** checkbox and set the fields **Caption and Tags** to use the **English-Microsoft** [analyzer](https://docs.microsoft.com/en-us/azure/search/search-analyzers).

![Language Analyzers](./resources/assets/AzureSearch-Analyzer.png)

For the final Index configuration step, we will create a [**Suggester**](https://docs.microsoft.com/en-us/rest/api/searchservice/suggesters) to set the fields that will be used for type ahead, allowing the user to type parts of a word where Azure Search will look for best matches in these fields. To learn more about suggestors and how to extend your searches to support fuzzy matching, which allows you to get results based on close matches even if the user misspells a word, check out [this example](https://docs.microsoft.com/en-us/azure/search/search-query-lucene-examples#fuzzy-search-example).

Click the **Suggester** checkbox and enter a Suggester Name: **sg** and choose **Tags** to be the fields to look for term suggestions

![Search Suggestions](./resources/assets/AzureSearch-Suggester.png)

Click **OK** to complete the configuration of the Indexer.  

Next, we configure how to "Import your data". You could set at schedule for how often the Indexer should check for changes, however, for this lab we will just run it once.  

Click **Advanced Options** and choose to **Base 64 Encode Keys** to ensure that the ID field only uses characters supported in the Azure Search key field.

Click **OK, three times** to start the Indexer job that will start the importing of the data from the Cosmos DB database.

![Configure Indexer](./resources/assets/AzureSearch-ConfigureIndexer.png)

***Query the Search Index***

You should see a message pop up indicating that Indexing has started.  If you wish to check the status of the Index, you can choose the "Indexers" option in the main Azure Search blade.

At this point we can try searching the index.  

Click **Search Explorer** and in the resulting blade choose your Index if it is not already selected.

Click **Search** to search for all documents. Try searching for "water", or something else, and use **ctrl+click** to select and view the URLs. Were your results what you expected?

![Search Explorer](./resources/assets/AzureSearch-SearchExplorer.png)

In the resulting json, you'll see a number after `@search.score`. Scoring refers to the computation of a search score for every item returned in search results. The score is an indicator of an item's relevance in the context of the current search operation. The higher the score, the more relevant the item. In search results, items are rank ordered from high to low, based on the search scores calculated for each item.

Azure Search uses default scoring to compute an initial score, but you can customize the calculation through a [scoring profile](https://docs.microsoft.com/en-us/rest/api/searchservice/add-scoring-profiles-to-a-search-index). There is an extra lab at the end of this workshop if you want to get some hands on experience with using [term boosting](https://docs.microsoft.com/en-us/rest/api/searchservice/Lucene-query-syntax-in-Azure-Search#bkmk_termboost) for scoring.

## Next Step

[Text Skills Lab](./Lab-Text-Skills.md) or
[Back to Main Menu](./readme.md)