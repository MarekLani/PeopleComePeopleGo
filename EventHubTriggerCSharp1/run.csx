#r "Newtonsoft.Json"
#r "System.IO"
#r "System.Net.Primitives"
#load "FaceApiHelper.csx"


using System;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Newtonsoft.Json;

public static void Run(string myEventHubMessage, TraceWriter log)
{
    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
    log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
    var e = JsonConvert.DeserializeObject<FaceData>(myEventHubMessage);
    
    
        
    
}
