#r "Newtonsoft.Json"
#r "System.IO"
#r "System.Net.Primitives"

#load "../Shared/FaceApiHelper.cs"
#load "../Shared/StorageHelper.cs"
#load "redisManager.csx"
#load "FaceDataObjects.cs"


using System;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.ProjectOxford.Common;

public static async Task<string> Run(string myEventHubMessage, TraceWriter log)
{
    log.Info($"C# Event Hub trigger function processing a message: {myEventHubMessage}");

    FaceServiceHelper.log = log;
    FaceServiceHelper.ApiKey = ConfigurationManager.AppSettings["FaceApiKey"].ToString();

    ////TODO create emotion api and add setting
    EmotionServiceHelper.ApiKey = ConfigurationManager.AppSettings["EmotionApiKey"].ToString();
    FaceServiceHelper fsh = new FaceServiceHelper();
    EmotionServiceHelper.log = log;


    var fd = JsonConvert.DeserializeObject<FaceData>(myEventHubMessage);
    log.Info(fd.ToString());
   
    string imageUrl = ConfigurationManager.AppSettings["StorageURL"].ToString() + "/" + ConfigurationManager.AppSettings["StorageContainer"].ToString() + "/" + fd.deviceId + "/" + fd.blobName;
    log.Info(imageUrl);


    try
    {
        var f = (await fsh.DetectAsync(imageUrl, true, true, new FaceAttributeType[] { FaceAttributeType.Age, FaceAttributeType.FacialHair, FaceAttributeType.Glasses, FaceAttributeType.Smile, FaceAttributeType.Gender, FaceAttributeType.HeadPose }))?.ToList().FirstOrDefault();

        if (f != null)
        {

            log.Info("Face detected");
            //person is entering premises

            string personId = "";

            var similarFace = await fsh.FindBestMatch(f.FaceId);
            if (similarFace == null)
            {
                log.Info("No Similar Face found");
                //Solve creation of face lists
                var persistedFace = await fsh.AddPersonToListAndCreateListIfNeeded(imageUrl, f.FaceRectangle);
                //We need to send just similarPersistedFaceID (it is GUID)
                personId = persistedFace.PersistedFaceId.ToString();
            }
            else
            {
                personId = similarFace.Item1.PersistedFaceId.ToString();
            }

            // Detect emotion
            var emotion = await EmotionServiceHelper.RecognizeWithFaceRectanglesAsync(imageUrl, new Rectangle[] { new Rectangle() { Top = f.FaceRectangle.Top, Height = f.FaceRectangle.Height, Left = f.FaceRectangle.Left, Width = f.FaceRectangle.Width } });



            //            //Discuss what should happen, when we want to send info also about person that was not caught during entering
            //            //Possible solution Add to be deleted queue and function which will go thru this queue every minute or so, and will be deleting these faces
            var returnMessage = JsonConvert.SerializeObject(CreateReturnMessage(f,e,personId,fd.timeStamp));
            return returnMessage;
        }

    }
    catch (Exception e)
    {
        if (e.GetType() == typeof(FaceAPIException))
            log.Info(((FaceAPIException)e).ErrorMessage);
        else
        {
            log.Info(e.Message + "stack" + e.StackTrace);
        }
    }
    finally
    {
        StorageHelper.DeleteFile(fd.deviceId + "/" + fd.blobName, ConfigurationManager.ConnectionStrings["StorageConnectionString"].ToString(),ConfigurationManager.AppSettings["StorageContainer"].ToString());
    }

    return "Error";
}

public FaceSendInfo CreateReturnMessage(Face f, Emotion e, string faceId, DateTime timeStamp)
{
    var fsi = new FaceSendInfo();

    fsi.faceId = faceId;

    fsi.age = f.FaceAttributes.Age;
    fsi.gender = f.FaceAttributes.Gender;
    fsi.smile = f.FaceAttributes.Smile;

    fsi.beard = f.FaceAttributes.FacialHair.Beard;
    fsi.moustache = f.FaceAttributes.FacialHair.Moustache;
    fsi.sideburns = f.FaceAttributes.FacialHair.Sideburns;

    fsi.glasses = f.FaceAttributes.Glasses.ToString();

    fsi.headYaw = f.FaceAttributes.HeadPose.Yaw;
    fsi.headRoll = f.FaceAttributes.HeadPose.Roll;
    fsi.headPitch = f.FaceAttributes.HeadPose.Pitch;

    fsi.anger = e.Scores.Anger;
    fsi.contempt = e.Scores.Contempt;
    fsi.disgust = e.Scores.Disgust;
    fsi.fear = e.Scores.Fear;
    fsi.happiness = e.Scores.Happiness;
    fsi.neutral = e.Scores.Neutral;
    fsi.sadness = e.Scores.Sadness;
    fsi.surprise = e.Scores.Surprise;

    fsi.timeStamp = timeStamp;

    return fsi;
}


