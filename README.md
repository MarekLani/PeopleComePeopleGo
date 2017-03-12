# PeopleComePeopleGo

This repository contains code for Azure Functions, which are part of bigger solution designed to track ariving and leaving people within certain location (e.g. building/store/office) using Face and Emotion Api - Azure Cognitive Services. Firs part of solution is client UWP application which takes pictures of arriving and leaving people. These pictures are than stored in Azure Storage Container and there is message with metadata send to event hub. This message triggers **ProcessFacesLoggedInEventHub function** 

The function reads out metadata (incl. uri of uploaded photo with face) and than makes use of FaceApiHelper and EmotionApiHelper methods, which are build on top of Project Oxford FaceApi and EmotionApi nuget packages. We have used Similar Face API  able to recognize person when leaving the premises. This function outputs data obtained from Face API and Emotion API to event hub, to which there is connected third part of solution in form of Azure Stream Analytics and Power BI, which enables visualization of data.

We also created **CleanFacesListsTimeTrigger** function which is scheduled to be run in the evening and cleans created Face Lists and also blob container, as people are not expected to stay at the
monitored location thru the night and the scenario, for which this solution was implemented does not require retention of faces for longer period of time.





