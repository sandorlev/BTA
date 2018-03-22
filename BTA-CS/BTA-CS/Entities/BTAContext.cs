using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace BTA_CS.Entities
{
    //public class BTAContext : WebContext
    //{
    //    // You can add custom code to this file. Changes will not be overwritten.
    //    // 
    //    // If you want Entity Framework to drop and regenerate your database
    //    // automatically whenever you change your model schema, please use data migrations.
    //    // For more information refer to the documentation:
    //    // http://msdn.microsoft.com/en-us/data/jj591621.aspx


    //    public BTAContext() : base("name=BTAContext")
    //    {
    //        this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
    //    }


    //    public System.Data.Entity.DbSet<BTA_CS.Entities.Bus> Buses { get; set; }

    //    public System.Data.Entity.DbSet<BTA_CS.Entities.Stop> Stops { get; set; }

    //}
    public class BTAContext : DbContext
    {
        public BTAContext(string connectionString) : base(connectionString)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<BTAContext>(null);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Bus> Bus { get; set; }
        public DbSet<Route> Route { get; set; }
        public DbSet<RouteElement> RouteElement { get; set; }
        public DbSet<Stop> Stop { get; set; }
        public DbSet<StopDetailDTO> StopDetailDTO { get; set; }
        public DbSet<StopDTO> StopDTO { get; set; }
    }
}
