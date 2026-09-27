using _Template.Core.SaveSystem.Data;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace _Template.Core.SaveSystem.Storage
{
    public class ServerSaveStorage : ISaveStorage
    {
        private readonly string _baseUrl = "2x2games.ir";

        public async Task SaveAsync<T>(string key, T data)
            where T : ISaveData
        {
            var json = JsonConvert.SerializeObject(data);

            using var request = new UnityWebRequest(
                $"{_baseUrl}/api/save/{UnityWebRequest.EscapeURL(key)}",
                UnityWebRequest.kHttpVerbPOST);

            var body = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader(
                "Content-Type",
                "application/json");

            await SendAsync(request);
        }

        public async Task<T> LoadAsync<T>(string key)
            where T : ISaveData
        {
            using var request = UnityWebRequest.Get(
                $"{_baseUrl}/api/save/{UnityWebRequest.EscapeURL(key)}");

            var response = await SendAsync(request);

            if (string.IsNullOrEmpty(response))
                return default;

            return JsonConvert.DeserializeObject<T>(response);
        }

        public async Task DeleteAsync(string key)
        {
            using var request = UnityWebRequest.Delete(
                $"{_baseUrl}/api/save/{UnityWebRequest.EscapeURL(key)}");

            await SendAsync(request);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            using var request = UnityWebRequest.Head(
                $"{_baseUrl}/api/save/{UnityWebRequest.EscapeURL(key)}");

            try
            {
                await SendAsync(request);
                return true;
            }
            catch (ServerSaveException exception)
            {
                if (exception.StatusCode == 404)
                    return false;

                throw;
            }
        }

        private async Task<string> SendAsync(UnityWebRequest request)
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new ServerSaveException(
                    request.responseCode,
                    request.error);
            }

            return request.downloadHandler?.text;
        }
    }

    public class ServerSaveException : Exception
    {
        public long StatusCode { get; }

        public ServerSaveException(long statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}