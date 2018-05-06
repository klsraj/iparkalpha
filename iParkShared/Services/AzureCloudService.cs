using Microsoft.WindowsAzure.MobileServices;

using iPark.Models;
using iParkShared;

namespace iPark.Services
{
    public class AzureCloudService : ICloudService
    {
        MobileServiceClient client;

        public AzureCloudService()
        {
            client = new MobileServiceClient(Constants.ApplicationURL, new LoggingHandler(true));
        }

        public IDataStore<T> GetTable<T>() where T : BaseModel
        {
            return new AzureDataStore<T>(client);
        }

        public MobileServiceClient GetClient()
        {
            return client;
        }
    }
}
