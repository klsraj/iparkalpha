using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

using iPark.Models;

namespace iPark.Services
{
    public interface IDataStore<T> where T : BaseModel
    {
        Task<T> CreateItemAsync(T item);
        Task<T> ReadItemAsync(string id);
        Task<T> UpdateItemAsync(T item);
        Task DeleteItemAsync(T item);

        Task<ICollection<T>> ReadAllItemsAsync();

        IMobileServiceTableQuery<T> GetQuery();
    }
}
