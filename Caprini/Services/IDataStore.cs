using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caprini.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);

        Task<bool> UpdateItemAsync(T item);

        Task<bool> DeleteItemAsync(long id);

        Task<T> GetItemAsync(long id);

        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}