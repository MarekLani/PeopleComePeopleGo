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

    public static void DeleteFile(string filename, string containerName) {

        string storageConnectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ToString();

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference(containerName);

        CloudBlockBlob blob = container.GetBlockBlobReference(filename);
        blob.DeleteIfExists();
    }

    

       
}