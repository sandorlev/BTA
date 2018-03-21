using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BTA_CS.Entities;
using System.Web.Http;
using System.Web.Http.Description;

namespace BTA_CS.Controllers
{
    public class BusController : ApiController
    {
        private BTAContext db = new BTAContext();

        // GET: api/Bus
        public IQueryable<Bus> GetBuses()
        {
            return db.Buses;
        }

        // GET: api/Buses/5
        [ResponseType(typeof(Bus))]
        public async Task<IHttpActionResult> GetBus(int id)
        {
            Bus bus = await db.Buses.FindAsync(id);
            if (bus == null)
            {
                return NotFound();
            }

            return Ok(bus);
        }

        // PUT: api/Buses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBus(int id, Bus bus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bus.ID)
            {
                return BadRequest();
            }

            db.Entry(bus).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Buses
        [ResponseType(typeof(Bus))]
        public async Task<IHttpActionResult> PostBus(Bus bus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Buses.Add(bus);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = bus.Id }, bus);
        }

        // DELETE: api/Buses/5
        [ResponseType(typeof(Bus))]
        public async Task<IHttpActionResult> DeleteBus(int id)
        {
            Bus bus = await db.Buses.FindAsync(id);
            if (bus == null)
            {
                return NotFound();
            }

            db.Buses.Remove(bus);
            await db.SaveChangesAsync();

            return Ok(bus);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BusExists(int id)
        {
            return db.Buses.Count(e => e.Id == id) > 0;
        }
    }
}