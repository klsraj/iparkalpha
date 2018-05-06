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
    public class ParkingLotController : TableController<ParkingLot>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<ParkingLot>(context, Request);
        }

        // GET tables/ParkingLot
        public IQueryable<ParkingLot> GetAllParkingLot()
        {
            return Query(); 
        }

        // GET tables/ParkingLot/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<ParkingLot> GetParkingLot(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/ParkingLot/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<ParkingLot> PatchParkingLot(string id, Delta<ParkingLot> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/ParkingLot
        public async Task<IHttpActionResult> PostParkingLot(ParkingLot item)
        {
            ParkingLot current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/ParkingLot/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteParkingLot(string id)
        {
             return DeleteAsync(id);
        }
    }
}
