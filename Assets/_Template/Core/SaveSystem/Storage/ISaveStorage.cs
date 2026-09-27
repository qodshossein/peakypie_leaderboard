using _Template.Core.SaveSystem.Data;
using System.Threading.Tasks;

namespace _Template.Core.SaveSystem.Storage
{
    public interface ISaveStorage
    {
        public Task<T> LoadAsync<T>(string key) where T : ISaveData;

        public Task SaveAsync<T>(string key, T data) where T : ISaveData;

        public Task DeleteAsync(string key);

        public Task<bool> ExistsAsync(string key);
    }
}