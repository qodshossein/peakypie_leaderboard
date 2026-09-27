using _Template.Core.SaveSystem.Data;
using System.Threading.Tasks;

namespace _Template.Core.SaveSystem
{
    public interface ISaveService
    {
        Task<T> LoadAsync<T>() where T : ISaveData;

        Task SaveAsync<T>(T data) where T : ISaveData;

        Task DeleteAsync<T>() where T : ISaveData;

        Task<bool> ExistsAsync<T>() where T : ISaveData;
    }
}