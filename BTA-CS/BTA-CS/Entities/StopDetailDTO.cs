using System;
namespace BTA_CS.Entities
{
    public class StopDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BusID { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
