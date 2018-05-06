using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

using iPark.Models;

namespace iPark.Services
{
    public class AzureDataStore<T> : IDataStore<T> where T : BaseModel
    {
        MobileServiceClient client;
        IMobileServiceTable<T> table;

        public AzureDataStore(MobileServiceClient client)
        {
            this.client = client;
            this.table = client.GetTable<T>();
        }

        #region ICloudTable implementation
        public async Task<T> CreateItemAsync(T item)
        {
            await table.InsertAsync(item);
            return item;
        }

        public async Task DeleteItemAsync(T item) => await table.DeleteAsync(item);

        public async Task<ICollection<T>> ReadAllItemsAsync() => await table.ToListAsync();

        public async Task<T> ReadItemAsync(string id)
        {
            return await table.LookupAsync(id);
        }

        public async Task<T> UpdateItemAsync(T item)
        {
            await table.UpdateAsync(item);
            return item;
        }

        public IMobileServiceTableQuery<T> GetQuery()
        {
            return table.CreateQuery();
        }
        #endregion
    }
}

