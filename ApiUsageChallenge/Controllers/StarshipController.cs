using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiUsageChallenge.Controllers
{
	class StarshipController
	{
		private static HttpClient apiClient = Program.apiClient;
		private static List<StarshipModel> starshipCache = new List<StarshipModel>();

		public static async Task<List<StarshipModel>> DisplayAllStarships(List<StarshipModel> starships)
		{
			string lookUpAnother = "";

			do
			{
				try
				{
					foreach (StarshipModel s in starships)
					{
						string id = s.Url.Split('/')[5];
						s.Id = id;
						Console.WriteLine($"{s.Id} {s.Name}");
					}
					Console.Write("\nPlease enter an ID number: ");
					string idText = Console.ReadLine();

					StarshipModel starship = await GetSingleStarship(idText);
					List<PersonModel> person = await CharacterController.GetStarWarsCharacters(starship.Pilots);
					List<FilModel> film = await FilmController.GetStarWarsFilms(starship.Films);

					Console.WriteLine("============================================================");
					Console.WriteLine($"Name: {starship.Name} | Model: {starship.Model}");
					Console.WriteLine($"Manufacturer: {starship.Manufacturer} | Class: {starship.StarshipClass}");
					Console.WriteLine($"Cost: {starship.CostInCredits} | Cargo Capacity: {starship.CargoCapacity}");
					Console.WriteLine($"Consumables: {starship.Consumables} | Max Speed: {starship.MaxAtmospheringSpeed}");
					Console.WriteLine($"Hyperdrive: {starship.HyperdriveRating} | MGLT: {starship.MGLT} | Length: {starship.Length}");
					Console.WriteLine($"Crew: {starship.Crew} | Passengers: {starship.Passengers}");
					Console.WriteLine($"Pilots: {CharacterController.FormatPersonLine(person)}");
					Console.WriteLine($"Films: {FilmController.FormatFilmLine(film)}");
					Console.WriteLine("============================================================");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error: {ex.Message}");
				}

				Console.Write("Do you want to look up another character (y/n)?");
				lookUpAnother = Console.ReadLine();

			} while (lookUpAnother == "y");

			return null;
		}

		public static string FormatStarshipLine(List<StarshipModel> starship)
		{
			string starshipString = "";

			if (starship.Count > 0)
			{
				foreach (StarshipModel model in starship)
				{
					starshipString += model.Name + "\n\t";
				}
				starshipString = starshipString.Substring(0, starshipString.Length - 2);
			}
			else
			{
				starshipString += "none";
			}

			return starshipString;
		}

		public static async Task<List<StarshipModel>> GetAllStarships()
		{
			string url = "https://swapi.co/api/starships";
			List<StarshipModel> output = new List<StarshipModel>();

			do
			{
				using(HttpResponseMessage response = await apiClient.GetAsync(url))
				{
					if (response.IsSuccessStatusCode)
					{
						var responseBody = await response.Content.ReadAsStringAsync();
						var responseJson = JObject.Parse(responseBody);
						var results = responseJson["results"].ToString();
						var count = int.Parse(responseJson["count"].ToString());

						if (count == starshipCache.Count)
						{
							return starshipCache;
						}
						else
						{
							starshipCache = new List<StarshipModel>();
						}
						url = responseJson["next"].ToString();

						output.AddRange(JsonConvert.DeserializeObject<IEnumerable<StarshipModel>>(results).ToList());
					}
					else
					{
						throw new Exception(response.ReasonPhrase);
					}
				}
			} while (url != "");

			starshipCache = output;
			return output;
		}

		public static async Task<List<StarshipModel>> GetStarWarsStarShips(string[] id)
		{
			string url = "";
			List<StarshipModel> model = new List<StarshipModel>();

			foreach (string apiLink in id)
			{
				apiLink.Split(',');

				url = $"{ apiLink }";

				StarshipModel cahced = starshipCache.Where(s => s.Id == apiLink).FirstOrDefault();

				if (cahced != null)
				{
					model.Add(cahced);
				}
				else
				{
					using (HttpResponseMessage response = await apiClient.GetAsync(url))
					{
						if (response.IsSuccessStatusCode)
						{
							StarshipModel output = await response.Content.ReadAsAsync<StarshipModel>();
							output.Id = apiLink;

							model.Add(output);
							starshipCache.Add(output);
						}
						else
						{
							throw new Exception(response.ReasonPhrase);
						}
					}
				}
			}

			return model;
		}

		public static async Task<StarshipModel> GetSingleStarship(string id)
		{
			string url = $"https://swapi.co/api/starships/{ id }/";

			StarshipModel cached = starshipCache.Where(s => s.Id == id).FirstOrDefault();

			if (cached != null)
			{
				return cached;
			}

			using (HttpResponseMessage response = await apiClient.GetAsync(url))
			{
				if (response.IsSuccessStatusCode)
				{
					// if response was successful 
					// read the content of the response asynchronously
					// map it over to the person model
					StarshipModel output = await response.Content.ReadAsAsync<StarshipModel>();
					output.Id = id;

					return output;
				}
				else
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}
	}
}
