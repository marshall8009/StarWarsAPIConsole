using System;
using System.Runtime.Serialization;

namespace ApiUsageChallenge
{
	[DataContract]
	public class VehicleModel
	{
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "model")]
		public string Model { get; set; }

		[DataMember(Name = "manufacturer")]
		public string Manufacturer { get; set; }

		[DataMember(Name = "cost_in_credits")]
		public string CostInCredits { get; set; }

		[DataMember(Name = "length")]
		public string Length { get; set; }

		[DataMember(Name = "max_atmosphering_speed")]
		public string MaxAtmospheringSpeed { get; set; }

		[DataMember(Name = "crew")]
		public string Crew { get; set; }

		[DataMember(Name = "passengers")]
		public string Passengers { get; set; }

		[DataMember(Name = "cargo_capacity")]
		public string CargoCapacity { get; set; }

		[DataMember(Name = "consumables")]
		public string Consumables { get; set; }

		[DataMember(Name = "vehicle_class")]
		public string VehicleClass { get; set; }

		[DataMember(Name = "pilots")]
		public string[] Pilots { get; set; }

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
