using System;
using System.Collections.Generic;

namespace AlwaysOn.Objects
{
	public class Hotspot
	{
		public Hotspot ()
		{
		}

		public string data {get; set;}
		public string lat {get; set;}
		public string lng {get; set;}
		public string address { get; set; }
		public Boolean superwifi {get; set;}
        public Boolean international { get; set; }
        public double distance { get; set;}
		public Boolean inrange { get; set; }
		public int signalstrength { get; set; }
    }
}