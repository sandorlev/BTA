﻿using System;
using System.Net.Sockets;
using System.Text;

namespace BTAServer
{
    public class StateObject
    {
        public Socket socket = null;
        public const int bufferSize = 256;
        public Byte[] buffer = new Byte[bufferSize];
    }

    public class Location
    {
        public Location(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Location() : this(0, 0) { }

        public override String ToString()
        {
            return String.Format("{0:N6}:{1:N6}", Latitude, Longitude);
        }

        public void FromString(String str)
        {
            var tokens = str.Split(':');
            Latitude = float.Parse(tokens[0]);
            Longitude = float.Parse(tokens[1]);
        }

        public Byte[] ToBytes()
        {
            return Encoding.ASCII.GetBytes(ToString());
        }

        public void FromBytes(Byte[] bytes)
        {
            var tokens = Encoding.ASCII.GetString(bytes).Split(':');
            Latitude = float.Parse(tokens[0]);
            Longitude = float.Parse(tokens[1]);
        }

        public Location Duplicate()
        {
            return new Location(Latitude, Longitude);
        }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
