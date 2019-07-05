using System;
using System.Runtime.Serialization;

namespace ApiUsageChallenge
{
	[DataContract]
	public class FilModel
	{
		public string Id { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "episode_id")]
		public int  EpisodeId { get; set; }

		[DataMember(Name = "opening_crawl")]
		public string OpeningCrawl { get; set; }

		[DataMember(Name = "director")]
		public string Director { get; set; }

		[DataMember(Name = "producer")]
		public string Producer { get; set; }

		[DataMember(Name = "release_date")]
		public string ReleaseDate { get; set; }

		[DataMember(Name = "characters")]
		public string[] Characters { get; set; }

		[DataMember(Name = "planets")]
		public string[] Planets { get; set; }

		[DataMember(Name = "starships")]
		public string[] Starships { get; set; }

		[DataMember(Name = "vehicles")]
		public string[] Vehicles { get; set; }

		[DataMember(Name = "species")]
		public string[] Species { get; set; }

		[DataMember(Name = "created")]
		public DateTime Created { get; set; }

		[DataMember(Name = "edited")]
		public DateTime Edited { get; set; }

		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

}
