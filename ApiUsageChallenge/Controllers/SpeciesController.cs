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
	class SpeciesController
	{
		private static HttpClient apiClient = Program.apiClient;
		private static List<SpeciesModel> speciesCache = new List<SpeciesModel>();

		public static async Task<List<SpeciesModel>> DisplayAllSpecies(List<SpeciesModel> species)
		{
			string lookUpAnother = "";

			do
			{
				try
				{
					foreach (SpeciesModel s in species)
					{
						string id = s.Url.Split('/')[5];
						s.Id = id;
						Console.WriteLine($"{s.Id} {s.Name}");
					}
					Console.Write("\nPlease enter an ID number: ");
					string idText = Console.ReadLine();

					SpeciesModel sp = await GetSingleSpecies(idText);
					List<PersonModel> person = await CharacterController.GetStarWarsCharacters(sp.People);
					List<FilModel> film = await FilmController.GetStarWarsFilms(sp.Films);
					PlanetModel planet = await PlanetController.GetSpeciesHomeworld(sp.Homeworld);

					Console.WriteLine("============================================================");
					Console.WriteLine($"Name: {sp.Name} | Classification: {sp.Classification}");
					Console.WriteLine($"Designation: {sp.Designation} | Average Height: {sp.AverageHeight}cm");
					Console.WriteLine($"Skin Colors: {sp.SkinColors}");
					Console.WriteLine($"Hair Colors: {sp.HairColors}");
					Console.WriteLine($"Eye Colors: {sp.EyeColors}");
					Console.WriteLine($"Average Lifespan: {sp.AverageLifespan} years");
					Console.WriteLine($"Homeworld: {planet.Name} | Language: {sp.Language}");
					Console.WriteLine($"People: {CharacterController.FormatPersonLine(person)}");
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

		public static async Task<List<SpeciesModel>> GetAllSpecies()
		{
			string url = "https://swapi.co/api/species";
			List<SpeciesModel> output = new List<SpeciesModel>();

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

						if (count == speciesCache.Count)
						{
							return speciesCache;
						}
						else
						{
							speciesCache = new List<SpeciesModel>();
						}
						url = responseJson["next"].ToString();

						output.AddRange(JsonConvert.DeserializeObject<IEnumerable<SpeciesModel>>(results).ToList());
					}
					else
					{
						throw new Exception(response.ReasonPhrase);
					}
				}
			} while (url != "");

			speciesCache = output;
			return output;
		}

		public static async Task<List<SpeciesModel>> GetStarWarsSpecies(string[] id)
		{
			string url = "";
			List<SpeciesModel> model = new List<SpeciesModel>();

			foreach (string apiLink in id)
			{
				apiLink.Split(',');

				url = $"{ apiLink }";

				SpeciesModel cahced = speciesCache.Where(s => s.Id == apiLink).FirstOrDefault();

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
							SpeciesModel output = await response.Content.ReadAsAsync<SpeciesModel>();
							output.Id = apiLink;

							model.Add(output);
							speciesCache.Add(output);
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

		public static async Task<SpeciesModel> GetSingleSpecies(string id)
		{
			string url = $"https://swapi.co/api/species/{ id }/";

			SpeciesModel cached = speciesCache.Where(p => p.Id == id).FirstOrDefault();

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
					SpeciesModel output = await response.Content.ReadAsAsync<SpeciesModel>();
					output.Id = id;

					return output;
				}
				else
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}

		public static string FormatSpeciesLine(List<SpeciesModel> species)
		{
			string speciesString = "";

			if (species.Count > 0)
			{
				foreach (SpeciesModel model in species)
				{
					speciesString += model.Name + "\n\t";
				}
				speciesString = speciesString.Substring(0, speciesString.Length - 2);
			}
			else
			{
				speciesString += "Unknown";
			}

			return speciesString;
		}
	}
}
