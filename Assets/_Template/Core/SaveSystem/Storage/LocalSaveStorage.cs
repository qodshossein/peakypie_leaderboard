using _Template.Core.SaveSystem.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine;

namespace _Template.Core.SaveSystem.Storage
{
    public class LocalSaveStorage : ISaveStorage
    {
        public Task SaveAsync<T>(string key, T data)
            where T : ISaveData
        {
            var json = JsonConvert.SerializeObject(data);

            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();

            return Task.CompletedTask;
        }

        public Task<T> LoadAsync<T>(string key)
            where T : ISaveData
        {
            if (!PlayerPrefs.HasKey(key))
                return Task.FromResult<T>(default);

            var json = PlayerPrefs.GetString(key);

            if (string.IsNullOrEmpty(json))
                return Task.FromResult<T>(default);

            var data = JsonConvert.DeserializeObject<T>(json);

            return Task.FromResult(data);
        }

        public Task DeleteAsync(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();

            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(PlayerPrefs.HasKey(key));
        }
    }
}