using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BTA_CS.Entities;

namespace BTA_CS.Controllers
{
    public class StopController : Controller
    {
        private BTAContext db = new BTAContext();

        // GET: api/Stops
        public IQueryable<StopDTO> GetStops()
        {
            var stop = from b in db.Stops
                       select new StopDTO()
                       {
                            Id = b.Id,
                            Name = b.Name,
                        };

            return stops;
        }

        // GET: api/Stops/5
        [ResponseType(typeof(StopDetailDTO))]
        public async Task<IHttpActionResult> GetStop(int id)
        {
            var stop = await db.Stops.Include(b => b.Name).Select(b =>
                new StopDetailDTO()
                {
                    Id = b.Id,
                    Name = b.Name,
                    Latitude = b.Latitude,
                    Longitude = b.Longitude
                }).SingleOrDefaultAsync(b => b.Id == id);
            if (stop == null)
            {
                return NotFound();
            }

            return Ok(stop);
        }

        // PUT: api/Stops/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutStop(int id, Stop stop)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != stop.Id)
            {
                return BadRequest();
            }

            db.Entry(stop).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StopExists(id))
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

        // POST: api/Stops
        [ResponseType(typeof(Stop))]
        public async Task<IHttpActionResult> PostStop(Stop stop)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Stops.Add(stop);
            await db.SaveChangesAsync();

            db.Entry(stop).Reference(x => x.Bus).Load();

            var dto = new StopDTO()
            {
                Id = stop.Id,
                Name = stop.Name,
            };

            return CreatedAtRoute("DefaultApi", new { id = stop.Id }, dto);
        }

        // DELETE: api/Stops/5
        [ResponseType(typeof(Stop))]
        public async Task<IHttpActionResult> DeleteStop(int id)
        {
           Stop stop = await db.Stops.FindAsync(id);
            if (stop == null)
            {
                return NotFound();
            }

            db.Stops.Remove(stop);
            await db.SaveChangesAsync();

            return Ok(stop);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StopExists(int id)
        {
            return db.Stops.Count(e => e.Id == id) > 0;
        }
    }
}