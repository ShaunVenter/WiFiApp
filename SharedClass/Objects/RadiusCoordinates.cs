using System;

namespace AlwaysOn
{
	public class RadiusCoordinates
	{
		//properties

		public double CurrentLat { get; set; }
		public double CurrentLong { get; set; }

		public double pointALat{ get; private set; }
		public double pointALong { get; private set; }
		public double pointBLat { get; private set; }
		public double pointBLong { get; private set; }


		//methods

		public RadiusCoordinates () { }

        public RadiusCoordinates(double Lat, double Long)
        {
            this.CurrentLat = Lat;
            this.CurrentLong = Long;
            SetRadiusPoints(0.02);
        }

        public void SetRadiusPoints(double baseOffset)
		{
            pointALat = CurrentLat - baseOffset;
            pointBLat = CurrentLat + baseOffset;
            pointALong = CurrentLong - baseOffset;
            pointBLong = CurrentLong + baseOffset;

            var latA = pointALat > pointBLat ? pointBLat : pointALat;
            var latB = pointALat > pointBLat ? pointALat : pointBLat;
            var lngA = pointALong > pointBLong ? pointBLong : pointALong;
            var lngB = pointALong > pointBLong ? pointALong : pointBLong;

            pointALat = latA;
            pointBLat = latB;
            pointALong = lngA;
            pointBLong = lngB;
        }
	}
}

