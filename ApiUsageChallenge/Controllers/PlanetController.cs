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
	class PlanetController
	{
		private static HttpClient apiClient = Program.apiClient;
		private static List<PlanetModel> planetCache = new List<PlanetModel>();

		public static string FormatPlanetLine(List<PlanetModel> planet)
		{
			string planetString = "";


			if (planet.Count > 0)
			{
				foreach (PlanetModel model in planet)
				{
					planetString += model.Name + "\n\t";
				}
				planetString = planetString.Substring(0, planetString.Length - 2);
			}
			else
			{
				planetString += "none";
			}

			return planetString;
		}

		public static async Task<List<PlanetModel>> DisplayAllPlanets(List<PlanetModel> planets)
		{
			string lookUpAnother = "";
			do
			{
				try
				{
					foreach (PlanetModel p in planets)
					{
						string id = p.Url.Split('/')[5];
						p.Id = id;
						Console.WriteLine($"{ p.Id } { p.Name }");
					}

					Console.Write("\nPlease enter an ID number: ");
					string idText = Console.ReadLine();

					PlanetModel planet = await GetStarWarsPlanets(idText);
					List<PersonModel> person = await CharacterController.GetStarWarsCharacters(planet.Residents);
					List<FilModel> film = await FilmController.GetStarWarsFilms(planet.Films);

					Console.WriteLine("============================================================");
					Console.WriteLine($"Name: {planet.Name} | Diameter: {planet.Diameter} | Population: {planet.Population}");
					Console.WriteLine($"It's terrain is {planet.Terrain}, surface water of {planet.SurfaceWater} with {planet.Gravity} gravity  \nand the climate is {planet.Climate}");
					Console.WriteLine($"{planet.Name} orbits every {planet.OrbitalPeriod} days and rotates every {planet.RotationPeriod} hours");
					Console.WriteLine($"Residents: {CharacterController.FormatPersonLine(person)}");
					Console.WriteLine($"Films: {FilmController.FormatFilmLine(film)}");
					Console.WriteLine("============================================================");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error: { ex.Message }");
				}

				Console.Write("Do you want to look up another planet (y/n)? ");
				lookUpAnother = Console.ReadLine();

			} while (lookUpAnother.ToLower() == "y");

			return null;
		}

		public static async Task<List<PlanetModel>> GetAllPlanets()
		{
			string url = "https://swapi.co/api/planets";
			List<PlanetModel> output = new List<PlanetModel>();

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

						if (count == planetCache.Count)
						{
							return planetCache;
						}
						else
						{
							planetCache = new List<PlanetModel>();
						}
						url = responseJson["next"].ToString();

						output.AddRange(JsonConvert.DeserializeObject<IEnumerable<PlanetModel>>(results).ToList());
					}
					else
					{
						throw new Exception(response.ReasonPhrase);
					}
				}
			} while (url != "");

			planetCache = output;

			return output;
		}

		public static async Task<PlanetModel> GetStarWarsPlanets(string id)
		{
			string url = $"{ id }";

			PlanetModel cached = planetCache.Where(x => x.Id == id).FirstOrDefault();

			if (cached != null)
			{
				return cached;
			}

			using (HttpResponseMessage response = await apiClient.GetAsync(url))
			{
				if (response.IsSuccessStatusCode)
				{
					PlanetModel output = await response.Content.ReadAsAsync<PlanetModel>();
					output.Id = id;

					return output;
				}
				else
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}

		public static async Task<PlanetModel> GetSpeciesHomeworld(string id)
		{
			string url = id;

			PlanetModel cached = planetCache.Where(x => x.Id == id).FirstOrDefault();

			if (cached != null)
			{
				return cached;
			}

			using (HttpResponseMessage response = await apiClient.GetAsync(url))
			{
				if (response.IsSuccessStatusCode)
				{
					PlanetModel output = await response.Content.ReadAsAsync<PlanetModel>();
					output.Id = id;

					return output;
				}
				else
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}

		public static async Task<List<PlanetModel>> GetStarWarsPlanetArray(string[] id)
		{
			string url = "";
			List<PlanetModel> model = new List<PlanetModel>();

			foreach (string apiLink in id)
			{
				apiLink.Split(',');

				url = $"{ apiLink }";

				PlanetModel cahced = planetCache.Where(s => s.Id == apiLink).FirstOrDefault();

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
							PlanetModel output = await response.Content.ReadAsAsync<PlanetModel>();
							output.Id = apiLink;

							model.Add(output);
							planetCache.Add(output);
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
	}
}
