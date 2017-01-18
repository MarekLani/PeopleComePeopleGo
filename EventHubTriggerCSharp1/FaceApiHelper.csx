using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

    public class FaceData
    {
        public string deviceId { get; set; }
        public string blobName { get; set; }
        public string cameraId { get; set; }
    }

    public class FaceAPIHelper
    {
        public static int RetryCountOnQuotaLimitError = 6;
        public static int RetryDelayOnQuotaLimitError = 500;

        private static FaceServiceClient faceClient { get; set; }

        public static Action Throttled;

        private static string apiKey;
        public static string ApiKey
        {
            get { return apiKey; }
            set
            {
                var changed = apiKey != value;
                apiKey = value;
                if (changed)
                {
                    InitializeFaceServiceClient();
                }
            }
        }

        static FaceAPIHelper()
        {
            InitializeFaceServiceClient();
        }

        private static void InitializeFaceServiceClient()
        {
            faceClient = new FaceServiceClient(apiKey);
        }


        private static async Task<TResponse> RunTaskWithAutoRetryOnQuotaLimitExceededError<TResponse>(Func<Task<TResponse>> action)
        {
            int retriesLeft = FaceAPIHelper.RetryCountOnQuotaLimitError;
            int delay = FaceAPIHelper.RetryDelayOnQuotaLimitError;

            TResponse response = default(TResponse);

            while (true)
            {
                try
                {
                    response = await action();
                    break;
                }
                catch (FaceAPIException exception) when (exception.HttpStatus == (System.Net.HttpStatusCode)429 && retriesLeft > 0)
                {
                    if (retriesLeft == 1 && Throttled != null)
                    {
                        Throttled();
                    }

                    await Task.Delay(delay);
                    retriesLeft--;
                    delay *= 2;
                    continue;
                }
            }

            return response;
        }

        private static async Task RunTaskWithAutoRetryOnQuotaLimitExceededError(Func<Task> action)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError<object>(async () => { await action(); return null; });
        }

        public static async Task<PersonGroup> CreatePersonGroupIfNoGroupExists(string name)
        {

            //await faceClient.DeletePersonGroupAsync("111");
            var groups = await faceClient.ListPersonGroupsAsync();
            if (groups != null && !groups.Where(g => g.Name == name).Any())
            {
                var newGroupId = Guid.NewGuid();
                await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.CreatePersonGroupAsync(newGroupId.ToString(), name));
                groups = await faceClient.ListPersonGroupsAsync();
                return groups.Where(g => g.PersonGroupId == newGroupId.ToString()).FirstOrDefault();
            }
            else
                return groups.Where(g => g.Name == name).FirstOrDefault();


        }

        public static async Task CreatePersonGroupAsync(string personGroupId, string name, string userData)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.CreatePersonGroupAsync(personGroupId, name, userData));
        }

        public static async Task<Person[]> GetPersonsAsync(string personGroupId)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<Person[]>(() => faceClient.GetPersonsAsync(personGroupId));
        }

        public static async Task<Face[]> DetectAsync(Stream imageStream, bool returnFaceId = true, bool returnFaceLandmarks = false, IEnumerable<FaceAttributeType> returnFaceAttributes = null)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<Face[]>(() => faceClient.DetectAsync(imageStream, returnFaceId, returnFaceLandmarks, returnFaceAttributes));
        }

        public static async Task<Face[]> DetectAsync(string url, bool returnFaceId = true, bool returnFaceLandmarks = false, IEnumerable<FaceAttributeType> returnFaceAttributes = null)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<Face[]>(() => faceClient.DetectAsync(url, returnFaceId, returnFaceLandmarks, returnFaceAttributes));
        }

        public static async Task<PersonFace> GetPersonFaceAsync(string personGroupId, Guid personId, Guid face)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<PersonFace>(() => faceClient.GetPersonFaceAsync(personGroupId, personId, face));
        }

        public static async Task<IEnumerable<PersonGroup>> GetPersonGroupsAsync(string userDataFilter = null)
        {
            return (await RunTaskWithAutoRetryOnQuotaLimitExceededError<PersonGroup[]>(() => faceClient.GetPersonGroupsAsync())).Where(group => string.IsNullOrEmpty(userDataFilter) || string.Equals(group.UserData, userDataFilter));
        }

        public static async Task<IEnumerable<FaceListMetadata>> GetFaceListsAsync(string userDataFilter = null)
        {
            return (await RunTaskWithAutoRetryOnQuotaLimitExceededError<FaceListMetadata[]>(() => faceClient.ListFaceListsAsync())).Where(list => string.IsNullOrEmpty(userDataFilter) || string.Equals(list.UserData, userDataFilter));
        }

        public static async Task<SimilarPersistedFace[]> FindSimilarAsync(Guid faceId, string faceListId, int maxNumOfCandidatesReturned = 1)
        {
            return (await RunTaskWithAutoRetryOnQuotaLimitExceededError<SimilarPersistedFace[]>(() => faceClient.FindSimilarAsync(faceId, faceListId, maxNumOfCandidatesReturned)));
        }

        public static async Task<AddPersistedFaceResult> AddFaceToFaceListAsync(string faceListId, Stream imageStream, FaceRectangle targetFace)
        {
            return (await RunTaskWithAutoRetryOnQuotaLimitExceededError<AddPersistedFaceResult>(() => faceClient.AddFaceToFaceListAsync(faceListId, imageStream, null, targetFace)));
        }

        public static async Task CreateFaceListAsync(string faceListId, string name, string userData)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.CreateFaceListAsync(faceListId, name, userData));
        }
        public static async Task CreateFaceListAsync(string faceListId, string name)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.CreateFaceListAsync(faceListId, name, ""));
        }

        public static async Task DeleteFaceListAsync(string faceListId)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.DeleteFaceListAsync(faceListId));
        }

        public static async Task DeleteFaceFromFaceListAsync(string faceListId, Guid peristedFaceId)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.DeleteFaceFromFaceListAsync(faceListId, peristedFaceId));
        }

        public static async Task UpdatePersonGroupsAsync(string personGroupId, string name, string userData)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.UpdatePersonGroupAsync(personGroupId, name, userData));
        }

        public static async Task AddPersonFaceAsync(string personGroupId, Guid personId, string imageUrl, string userData, FaceRectangle targetFace)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.AddPersonFaceAsync(personGroupId, personId, imageUrl, userData, targetFace));
        }

        public static async Task<AddPersistedFaceResult> AddPersonFaceAsync(string personGroupId, Guid personId, Stream imageStream, string userData, FaceRectangle targetFace)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.AddPersonFaceAsync(personGroupId, personId, imageStream, userData, targetFace));
        }

        public static async Task<IdentifyResult[]> IdentifyAsync(string personGroupId, Guid[] detectedFaceIds)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<IdentifyResult[]>(() => faceClient.IdentifyAsync(personGroupId, detectedFaceIds, 5));
        }

        public static async Task DeletePersonAsync(string personGroupId, Guid personId)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.DeletePersonAsync(personGroupId, personId));
        }

        public static async Task CreatePersonAsync(string personGroupId, string name)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.CreatePersonAsync(personGroupId, name));
        }

        public static async Task<CreatePersonResult> CreatePersonWithResultAsync(string personGroupId, string name)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.CreatePersonAsync(personGroupId, name));
        }

        public static async Task<Person> GetPersonAsync(string personGroupId, Guid personId)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<Person>(() => faceClient.GetPersonAsync(personGroupId, personId));
        }

        public static async Task TrainPersonGroupAsync(string personGroupId)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.TrainPersonGroupAsync(personGroupId));
        }

        public static async Task DeletePersonGroupAsync(string personGroupId)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.DeletePersonGroupAsync(personGroupId));
        }

        public static async Task DeletePersonFaceAsync(string personGroupId, Guid personId, Guid faceId)
        {
            await RunTaskWithAutoRetryOnQuotaLimitExceededError(() => faceClient.DeletePersonFaceAsync(personGroupId, personId, faceId));
        }

        public static async Task<TrainingStatus> GetPersonGroupTrainingStatusAsync(string personGroupId)
        {
            return await RunTaskWithAutoRetryOnQuotaLimitExceededError<TrainingStatus>(() => faceClient.GetPersonGroupTrainingStatusAsync(personGroupId));
        }
    }

