using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using iParkBackend.DataObjects;
using iParkBackend.Models;

namespace iParkBackend.Controllers
{
    public class PriceModelController : TableController<PriceModel>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<PriceModel>(context, Request);
        }

        // GET tables/PriceModel
        public IQueryable<PriceModel> GetAllPriceModel()
        {
            return Query(); 
        }

        // GET tables/PriceModel/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<PriceModel> GetPriceModel(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/PriceModel/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<PriceModel> PatchPriceModel(string id, Delta<PriceModel> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/PriceModel
        public async Task<IHttpActionResult> PostPriceModel(PriceModel item)
        {
            PriceModel current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/PriceModel/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePriceModel(string id)
        {
             return DeleteAsync(id);
        }
    }
}
