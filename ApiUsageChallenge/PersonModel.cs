using System;
using System.Runtime.Serialization;

namespace ApiUsageChallenge
{
	// allows you to map json values to property names you want
	[DataContract]
	public class PersonModel
	{
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string FullName { get; set; }

		[DataMember(Name = "height")]
		public string Height { get; set; }

		[DataMember(Name = "mass")]
		public string Mass { get; set; }

		[DataMember(Name = "hair_color")]
		public string HairColor { get; set; }

		[DataMember(Name = "skin_color")]
		public string SkinColor { get; set; }

		[DataMember(Name = "eye_color")]
		public string Eye_Color { get; set; }

		[DataMember(Name = "birth_year")]
		public string BirthYear { get; set; }

		[DataMember(Name = "gender")]
		public string Gender { get; set; }

		[DataMember(Name = "homeworld")]
		public string Homeworld { get; set; }

		[DataMember(Name = "films")]
		public string[] Films { get; set; }

		[DataMember(Name = "species")]
		public string[] Species { get; set; }

		[DataMember(Name = "vehicles")]
		public string[] Vehicles { get; set; }

		[DataMember(Name = "starships")]
		public string[] Starships { get; set; }

		[DataMember(Name = "created")]
		public DateTime Created { get; set; }

		[DataMember(Name = "edited")]
		public DateTime Edited { get; set; }

		[DataMember(Name = "url")]
		public string Url { get; set; }
	}
}
