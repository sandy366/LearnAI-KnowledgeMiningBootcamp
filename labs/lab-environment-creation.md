# Environment Creation

In this lab, you will create an Azure Search service and a storage account. We recommend keeping both in a new and unique resource group, to make it easier to delete at the end of the workshop (if you want to). We will also upload the data to a blob storage within the storage account.

## Step 1 - Create the Azure Search service

1. Go to the [Azure portal](https://portal.azure.com) and sign in with your Azure account.

1. Click **Create a resource**, search for Azure Search, and click **Create**. See [Create an Azure Search service in the portal](https://docs.microsoft.com/en-us/azure/search/search-create-service-portal) if you are setting up a search service for the first time, and use the bullet point list below for the details you will use to fill out the details for the Azure Search service.

  ![Dashboard portal](../resources/images/lab-environment-creation/create-service-full-portal.png)

1. For the **URL**, that is the service name, choose a name that you can easily remember. We will use it dozens of times in the next labs. The name of the service in the screenshots of this lab won't be available, you must create your own service name.

1. For **Resource group**, create a resource group to contain all the resources you create in this tutorial. This makes it easier to clean up the resources after you have finished the tutorial.

1. For **Location**, choose one of the regions below, Cognitive Search is not available in all Azure regions.
- West Central US
- South Central US
- East US
- East US 2
- West US 2
- Canada Central
- West Europe
- UK South
- North Europe
- Brazil South
- Southeast Asia
- Central India
- Australia East

1. For **Pricing tier**, create a **Basic** service to complete tutorials and quickstarts. For deeper information on Azure Search pricing and limits, click [here](https://azure.microsoft.com/pricing/details/search/) and [here](https://docs.microsoft.com/en-us/azure/search/search-limits-quotas-capacity).

  > Tip! While Cognitive Search is in public preview, you just need to pay for the Azure Search service.  At a later time, the pricing for this capability will be announced. Skillset execution is currently available in all tiers, including free. But it is not enough for this training.

1. Pin the service to the dashboard for fast access to service information.

  ![Service definition page in the portal](../resources/images/lab-environment-creation/create-search-service.png)

1. After the service is created, collect the following information: **URL** from the Overview page, and **api-key** (either primary or secondary) from the Keys page. You will need them in the following labs.

  ![Endpoint and key information in the portal](../resources/images/lab-environment-creation/create-search-collect-info.png "Endpoint and key information in the portal")

>Note!
> Azure Search must have 2 replicas for read-only SLA and 3 replicas for read/write SLA. This is not addressed in this training. For more information, click [here](https://azure.microsoft.com/en-us/support/legal/sla/search/v1_0/)

## Step 2 - Clone the Repo

Cloning the repo will download all the training materials to your computer, including the dataset, the slides and the code for the Bot project. **The cloning of the repository will use close to 110 MB in total**.

1. In the Cortana search bar, type "git bash" and select "Git Bash Desktop App", or type "cmd" and select "Command Prompt".

1. Next, type `cd c:` then enter. Now, you will be in the root directory of your computer. If you don't have permission to create folders and files here, navigate to a folder where you can download the Bootcamp materials.

1. Type and enter `git clone https://github.com/Azure/LearnAI-KnowledgeMiningBootcamp.git`

1. *Validation step*: Go to **C:\LearnAI-KnowledgeMiningBootcamp** and confirm it exists, including the dataset in the resources folder

![Git process](../resources/images/lab-environment-creation/git.png)

>Note! The image above has a smaller number of files downloaded than expected, the training is under constant development.

## Step 3 - Create the Azure Blob service and upload the dataset

The enrichment pipeline pulls from Azure data sources. Source data must originate from a supported data source type of an [Azure Search indexer](https://docs.microsoft.com/en-us/azure/search/search-indexer-overview). For this exercise, we use blob storage to showcase multiple content types.

 1. Create a **storage account** in the **same region of your Azure Search service** created in the step above, to avoid latency between the search service and the files.  Use a **general purpose** account and **LRS replication**. For production environments, you may need to use another replication type.

 1. From the storage account **Overview** tab, click the link to **Blobs** and create a **container** named **`basicdemo`**:
    - Enter a name for the container.
    - Select **Private** for Access Type. When you set the access to private, the container and blob data can be read by the Windows Azure account owner only.

 1. You can upload the sample **all files** from the **\resources\dataset** folder to the blob storage using [Azure Portal](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-portal) or [Azure Storage Explorer](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-storage-explorer). There are other methods to upload data to Azure, but we don't  suggest them for this training.

>Tip! If you are using Azure Storage Explorer, in the `basicdemo` container you created, click **Upload** to upload the sample files.
>
>Be careful! don't create another folder level. This training is created with the assumption that all of the data is located in the root folder of the container.

Use the portal to confirm that all of the 21 files are uploaded to the root directory of the **basicdemo** container.

Also in the portal, find the storage account tab called **Access keys**, it has information you will use in the next lab. Get familiar with the blue icons to copy the information for storage account name, key and connection string. They prevent partial copy of the codes. Extra care with the connection string, it is long and finished with **core.windows.net**.

## Next Step

[Azure Search Lab](../labs/lab-azure-search.md) or
[Back to Read Me](../README.md)