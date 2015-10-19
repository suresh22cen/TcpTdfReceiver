namespace RaceBusinessLogic
{
    using System;

    [Serializable]
	public class DataRecord
	{
		public int rider;
		public float accuracy;
		public double altitude;
		public float bearing;
		public float speed;

		public int battery_remaining;
		
		public double latitude;
		public double longitude;

		public long timeRecorded;
	}
}
