using _Template.Core.SaveSystem.Data;
using _Template.Core.SaveSystem.Storage;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace _Template.Core.SaveSystem
{
    public class SaveService : ISaveService
    {
        private readonly Dictionary<SaveTarget, ISaveStorage> _storages;

        public SaveService(LocalSaveStorage localStorage, ServerSaveStorage serverStorage)
        {
            _storages = new Dictionary<SaveTarget, ISaveStorage>
            {
                { SaveTarget.Local, localStorage },
                { SaveTarget.Server, serverStorage }
            };
        }
        public async Task<T> LoadAsync<T>()
            where T : ISaveData
        {
            var attribute = typeof(T).GetCustomAttribute<SaveDataAttribute>();
            var storage = _storages[attribute.Target];

            var data = await storage.LoadAsync<T>(attribute.Key);

            return data;
        }

        public async Task SaveAsync<T>(T data)
            where T : ISaveData
        {
            var attribute = typeof(T).GetCustomAttribute<SaveDataAttribute>();
            var storage = _storages[attribute.Target];

            await storage.SaveAsync(attribute.Key, data);
        }

        public async Task DeleteAsync<T>() where T : ISaveData
        {
            var attribute = typeof(T).GetCustomAttribute<SaveDataAttribute>();
            var storage = _storages[attribute.Target];

            await storage.DeleteAsync(attribute.Key);
        }

        public async Task<bool> ExistsAsync<T>() where T : ISaveData
        {
            var attribute = typeof(T).GetCustomAttribute<SaveDataAttribute>();
            var storage = _storages[attribute.Target];

            var data = await storage.ExistsAsync(attribute.Key);

            return data;
        }
    }
}