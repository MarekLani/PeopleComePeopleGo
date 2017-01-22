#r "Newtonsoft.Json"
#r "System.IO"
#r "System.Net.Primitives"
#load "FaceApiHelper.csx"
#load "StorageHelper.csx"
#load "redisManager.csx"


using System;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Newtonsoft.Json;
using System.Configuration;
using System.Drawing;

public static async void Run(string myEventHubMessage, TraceWriter log)
{
    log.Info($"C# Event Hub trigger function processing a message: {myEventHubMessage}");

    FaceServiceHelper.log = log;
    EmotionServiceHelper.log = log;

    var fd = JsonConvert.DeserializeObject<FaceData>(myEventHubMessage);
    FaceServiceHelper.ApiKey = ConfigurationManager.AppSettings["FaceApiKey"].ToString();

    //TODO create emotion api and add setting
    EmotionServiceHelper.ApiKey = ConfigurationManager.AppSettings["EmotionApiKey"].ToString();
    string imageUrl = ConfigurationManager.AppSettings["StorageURL"].ToString() +"/"+ ConfigurationManager.AppSettings["StorageContainer"].ToString() + "/" + fd.deviceId + "/" + fd.blobName;
    log.Info(imageUrl);

    Face f = (await FaceServiceHelper.DetectAsync(imageUrl, true, true, new FaceAttributeType[] { FaceAttributeType.Age, FaceAttributeType.FacialHair, FaceAttributeType.Glasses, FaceAttributeType.Smile, FaceAttributeType.Gender, FaceAttributeType.HeadPose })).ToList()?.FirstOrDefault();
    if (f == null)
        return;

    //person is entering premises
    if (fd.entryCamera)
    {
        var similarFace = await FaceServiceHelper.FindBestMatch(f.FaceId);
        if (similarFace == null)
        {
            // Detect emotion if not already in face list and send to event hub
            var emotion = EmotionServiceHelper.RecognizeWithFaceRectanglesAsync(imageUrl, new System.Drawing.Rectangle[] { new System.Drawing.Rectangle() { Top = f.FaceRectangle.Top, Height = f.FaceRectangle.Height, Left = f.FaceRectangle.Left, Width = f.FaceRectangle.Width } });
            //Solve creation of face lists
            var persistedFace = await FaceServiceHelper.AddPersonToListAndCreateListIfNeeded("faceListId", imageUrl, Face.FaceRectangle);
            //We need to send just similarPersistedFaceID (it is GUID)
            //Todo send to EventHub event: entering, persistedFace.SimilarPersistedFaceId as FaceID, emotion, face landmakrs (age, mustache, glasses)...
        }
    }
    //Person is leaving premises
    else
    {
        //We will need to loop thru all facelists
        var bestMatch = await FaceServiceHelper.FindBestMatch(f.FaceId);
        if (bestMatch != null) {

            var emotion = EmotionServiceHelper.RecognizeWithFaceRectanglesAsync(imageUrl, new Rectangle[] { new Rectangle() { Top = f.FaceRectangle.Top, Height = f.FaceRectangle.Height, Left = f.FaceRectangle.Left, Width = f.FaceRectangle.Width } });
            
            //Discuss what should happen, when we want to send info also about person that was not caught during entering
            //Possible solution Add to be deleted queue and function which will go thru this queue every minute or so, and will be deleting these faces

            await FaceServiceHelper.DeleteFaceFromFaceListAsync(bestMatch.Item2, bestMatch.Item1.PersistedFaceId);
            //Todo send to EventHub event: leaving, persistedFace.SimilarPersistedFaceId as FaceID, emotion, face landmakrs (age, mustache, glasses)...
        }
    }
    //var r = insertKeyValuePair();

    StorageHelper.DeleteFile(fd.deviceId + "/" + fd.blobName, ConfigurationManager.AppSettings["ContainerName"].ToString());

}


