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
using System.Web.Mvc;
using BTA_CS.Entities;

namespace BTA_CS.Controllers
{
    public class StopController : /*Controller,*/ ApiController
    {
        static string myConnectionString = "server=127.0.0.1;uid=root;" +
    "pwd=123456789;database=test";

        private BTAContext db = new BTAContext(myConnectionString);

        // GET: api/Stops
        public IQueryable<StopDTO> GetStops()
        {
            var stop = from b in db.Stop
                       select new StopDTO()
                       {
                            Id = b.ID,
                            Name = b.Name,
                        };

            return stop;
        }

        // GET: api/Stops/5
        [ResponseType(typeof(StopDetailDTO))]
        public async Task<IHttpActionResult> GetStop(int id)
        {
            var stop = await db.Stop.Include(b => b.Name).Select(b =>
                new StopDetailDTO()
                {
                    Id = b.ID,
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

            if (id != stop.ID)
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

            db.Stop.Add(stop);
            await db.SaveChangesAsync();

            db.Entry(stop).Reference(x => x.Bus).Load();

            var dto = new StopDTO()
            {
                Id = stop.ID,
                Name = stop.Name,
            };

            return CreatedAtRoute("DefaultApi", new { id = stop.ID }, dto);
        }

        // DELETE: api/Stops/5
        [ResponseType(typeof(Stop))]
        public async Task<IHttpActionResult> DeleteStop(int id)
        {
           Stop stop = await db.Stop.FindAsync(id);
            if (stop == null)
            {
                return NotFound();
            }

            db.Stop.Remove(stop);
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
            return db.Stop.Count(e => e.ID == id) > 0;
        }
    }
}