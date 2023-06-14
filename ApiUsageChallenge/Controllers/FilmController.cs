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
	class FilmController
	{
		private static HttpClient apiClient = Program.apiClient;
		private static List<FilModel> filmCache = new List<FilModel>();

		public static async Task<List<FilModel>> DisplayAllFilms(List<FilModel> films)
		{
			string lookUpAnother = "";

			do
			{
				try
				{
					foreach (FilModel f in films)
					{
						string id = f.Url.Split('/')[5];
						f.Id = id;
						Console.WriteLine($"{f.Id} {f.Title}");
					}
					Console.Write("\nPlease enter an ID number: ");
					string idText = Console.ReadLine();

					FilModel film = await GetSingleFilm(idText);
					List<PersonModel> person = await CharacterController.GetStarWarsCharacters(film.Characters);
					List<PlanetModel> planet = await PlanetController.GetStarWarsPlanetArray(film.Planets);
					List<SpeciesModel> species = await SpeciesController.GetStarWarsSpecies(film.Species);
					List<StarshipModel> starship = await StarshipController.GetStarWarsStarShips(film.Starships);
					List<VehicleModel> vehicle = await VehicleController.GetStarWarsVehicles(film.Vehicles);

					Console.WriteLine("============================================================");
					Console.WriteLine($"Title: {film.Title} | Release Date: {film.ReleaseDate}");
					Console.WriteLine($"Producer: {film.Producer} | Director: {film.Director}");
					Console.WriteLine($"    Opening Crawl: \n{film.OpeningCrawl}");
					Console.WriteLine($"Characters: {CharacterController.FormatPersonLine(person)}");
					Console.WriteLine($"Planets: {PlanetController.FormatPlanetLine(planet)}");
					Console.WriteLine($"Starships: {StarshipController.FormatStarshipLine(starship)}");
					Console.WriteLine($"Vehicles: {VehicleController.FormatVehicleLine(vehicle)}");
					Console.WriteLine($"Species: {SpeciesController.FormatSpeciesLine(species)}");
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

		public static async Task<List<FilModel>> GetStarWarsFilms(string[] id)
		{
			string url = "";
			List<FilModel> model = new List<FilModel>();

			foreach (string apiLink in id)
			{
				apiLink.Split(',');

				url = $"{ apiLink }";

				FilModel cahced = filmCache.Where(f => f.Id == apiLink).FirstOrDefault();

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
							FilModel output = await response.Content.ReadAsAsync<FilModel>();
							output.Id = apiLink;

							model.Add(output);
							filmCache.Add(output);
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

		public static async Task<List<FilModel>> GetAllFilms()
		{
			string url = "https://swapi.dev/api/films";
			List<FilModel> output = new List<FilModel>();

			do
			{
				using (HttpResponseMessage response = await apiClient.GetAsync(url))
				{
					if (response.IsSuccessStatusCode)
					{
						var responseBody = await response.Content.ReadAsStringAsync();
						var responseJson = JObject.Parse(responseBody);
						var results = responseJson["results"].ToString();
						var count = int.Parse(responseJson["count"].ToString());

						if (count == filmCache.Count)
						{
							return filmCache;
						}
						else
						{
							filmCache = new List<FilModel>();
						}
						url = responseJson["next"].ToString();

						output.AddRange(JsonConvert.DeserializeObject<IEnumerable<FilModel>>(results).ToList());
					}
					else
					{
						throw new Exception(response.ReasonPhrase);
					}
				}
			} while (url != "");

			filmCache
 = output;
			return output;
		}

		public static async Task<FilModel> GetSingleFilm(string id)
		{
			string url = $"https://swapi.dev/api/starships/{ id }/";

			FilModel cached = filmCache.Where(s => s.Id == id).FirstOrDefault();

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
					FilModel output = await response.Content.ReadAsAsync<FilModel>();
					output.Id = id;

					return output;
				}
				else
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}

		public static string FormatFilmLine(List<FilModel> film)
		{
			string filmString = "";

			if (film.Count > 0)
			{
				foreach (FilModel model in film)
				{
					filmString += model.Title + "\n       ";
				}
				filmString = filmString.Substring(0, filmString.Length - 8);
			}
			else
			{
				filmString += "Did not appear in any films";
			}

			return filmString;
		}
	}
}
