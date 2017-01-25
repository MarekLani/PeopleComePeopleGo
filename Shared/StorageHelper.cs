using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

/// <summary>
/// Summary description for StorageHelper
/// </summary>
public class StorageHelper
{
	public StorageHelper()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static void DeleteContainerContent(string connectioString, string containerName)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectioString);

        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference(containerName);


        foreach(var blob in container.ListBlobs())
        {
            if(blob.GetType() == typeof(CloudBlockBlob))
                ((CloudBlockBlob)blob).DeleteIfExists();

            if (blob.GetType() == typeof(CloudBlockDirectory))
                ((CloudBlockDirectory)blob).DeleteIfExists();
        }
    }

    public static void DeleteFile(string filename, string connectioString, string containerName)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectioString);

        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference(containerName);

        CloudBlockBlob blob = container.GetBlockBlobReference(filename);
        blob.DeleteIfExists();
    }

    

       
}