using Microsoft.WindowsAzure.MobileServices;

using iPark.Models;

namespace iPark.Services
{
    public interface ICloudService
    {
        IDataStore<T> GetTable<T>() where T : BaseModel;

        MobileServiceClient GetClient();
    }
}
