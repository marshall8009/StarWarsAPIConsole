using System;
using System.Runtime.Serialization;

namespace ApiUsageChallenge
{
	[DataContract]
	public class PlanetModel
	{
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "rotation_period")]
		public string RotationPeriod { get; set; }

		[DataMember(Name = "orbital_period")]
		public string OrbitalPeriod { get; set; }

		[DataMember(Name = "diameter")]
		public string Diameter { get; set; }

		[DataMember(Name = "climate")]
		public string Climate { get; set; }

		[DataMember(Name = "gravity")]
		public string Gravity { get; set; }

		[DataMember(Name = "terrain")]
		public string Terrain { get; set; }

		[DataMember(Name = "surface_water")]
		public string SurfaceWater { get; set; }

		[DataMember(Name = "population")]
		public string Population { get; set; }

		[DataMember(Name = "residents")]
		public string[] Residents { get; set; }

		[DataMember(Name = "films")]
		public string[] Films { get; set; }

		[DataMember(Name = "created")]
		public DateTime Created { get; set; }

		[DataMember(Name = "edited")]
		public DateTime Edited { get; set; }

		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

}
