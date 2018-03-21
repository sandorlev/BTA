using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTA_CS.Entities
{
    public class Stop : IEntity
    {
        public int ID { get; set; }

        public String Name { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public int BusID { get; set; }
        public Bus Bus { get; set; }
    }
}