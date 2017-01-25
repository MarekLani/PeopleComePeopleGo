#load "../Shared/FaceApiHelper.cs"
#load "../Shared/StorageHelper.cs"

using System;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Configuration;
using Microsoft.ProjectOxford.Common;


public static void Run(TraceWriter log)
{
    FaceServiceHelper.ApiKey = ConfigurationManager.AppSettings["FaceApiKey"].ToString();
    var fsh = new FaceServiceHelper();
    fsh.DeleteAllFaceLists();

    StorageHelper.DeleteContainerContent(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ToString(), ConfigurationManager.AppSettings["StorageContainer"].ToString());
}