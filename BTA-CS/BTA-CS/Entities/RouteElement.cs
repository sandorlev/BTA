using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTA_CS.Entities
{
    public class RouteElement : IEntity
    {
        public int ID { get; set; }

        public Route Route { get; set; }
        public Stop Stop { get; set; }

        public int position { get; set; }
    }
}