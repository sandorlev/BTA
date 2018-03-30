using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTA_CS.Entities
{
    public class Bus : IEntity
    {
        public int ID { get; set; }

        public string Name { get; set; }
    }
}