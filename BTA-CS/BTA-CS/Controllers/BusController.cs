using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BTA_CS.Entities;
using System.Web.Http;
using System.Web.Http.Description;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BTA_CS.Controllers
{
    public class BusController : ApiController 
    {
        static string myConnectionString = "server=127.0.0.1;uid=root;" +
    "pwd=123456789;database=test";

        private BTAContext db = new BTAContext(myConnectionString);

        public static void getBusLocation()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "busLocationMQ",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    //Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: "busLocationMQ",
                                     autoAck: true,
                                     consumer: consumer);

                //Console.WriteLine(" Press [enter] to exit.");
                //Console.ReadLine();
            }
        }


        // GET: api/Bus
        public IQueryable<Bus> GetBuses()
        {
            return db.Bus;
        }

        // GET: api/Buses/5
        [ResponseType(typeof(Bus))]
        public async Task<IHttpActionResult> GetBus(int id)
        {
            Bus bus = await db.Bus.FindAsync(id);
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

            db.Bus.Add(bus);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = bus.ID }, bus);
        }

        // DELETE: api/Buses/5
        [ResponseType(typeof(Bus))]
        public async Task<IHttpActionResult> DeleteBus(int id)
        {
            Bus bus = await db.Bus.FindAsync(id);
            if (bus == null)
            {
                return NotFound();
            }

            db.Bus.Remove(bus);
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
            return db.Bus.Count(e => e.ID == id) > 0;
        }

        public static void Main()
        {
            getBusLocation();
        }
    }
}