using System;
using System.Runtime.Serialization;

namespace ApiUsageChallenge
{
	[DataContract]
	public class SpeciesModel
	{
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "classification")]
		public string Classification { get; set; }

		[DataMember(Name = "designation")]
		public string Designation { get; set; }

		[DataMember(Name = "average_height")]
		public string AverageHeight { get; set; }

		[DataMember(Name = "skin_colors")]
		public string SkinColors { get; set; }

		[DataMember(Name = "hair_colors")]
		public string HairColors { get; set; }

		[DataMember(Name = "eye_colors")]
		public string EyeColors { get; set; }

		[DataMember(Name = "average_lifespan")]
		public string AverageLifespan { get; set; }

		[DataMember(Name = "homeworld")]
		public string Homeworld { get; set; }

		[DataMember(Name = "language")]
		public string Language { get; set; }

		[DataMember(Name = "people")]
		public string[] People { get; set; }

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
