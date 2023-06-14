using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//...
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace ApiUsageChallenge.Controllers
{
	public class CharacterController
	{
		private static HttpClient apiClient = Program.apiClient;
		private static List<PersonModel> peopleCache = new List<PersonModel>();

		public static async Task<List<PersonModel>> DisplayAllCharacters(List<PersonModel> people)
		{
			string lookUpAnother = "";
			do
			{
				try
				{
					foreach (PersonModel p in people)
					{
						string id = p.Url.Split('/')[5];
						p.Id = id;
						Console.WriteLine($"{p.Id} {p.FullName}");
					}

					Console.Write("\nPlease enter an ID number (1-88): ");
					string idText = Console.ReadLine();

					PersonModel person = await GetStarWarsCharacter(idText);
					PlanetModel planet = await PlanetController.GetStarWarsPlanets(person.Homeworld);
					List<VehicleModel> vehicle = await VehicleController.GetStarWarsVehicles(person.Vehicles);
					List<StarshipModel> starship = await StarshipController.GetStarWarsStarShips(person.Starships);
					List<FilModel> film = await FilmController.GetStarWarsFilms(person.Films);
					List<SpeciesModel> species = await SpeciesController.GetStarWarsSpecies(person.Species);
					Console.WriteLine("============================================================");
					Console.WriteLine($"Name: { person.FullName } | Gender: { person.Gender }");
					Console.WriteLine($"Species: { SpeciesController.FormatSpeciesLine(species) } race");
					Console.WriteLine($"Born: { person.BirthYear }");
					Console.WriteLine($"Homeworld: { planet.Name }");
					Console.WriteLine($"{ FormatMeasurementLine(person) }");
					Console.WriteLine($"Hair: { person.HairColor } | Skin: { person.SkinColor } | " +
						$"Eyes: { person.Eye_Color}");

					Console.WriteLine($"Vehicles: { VehicleController.FormatVehicleLine(vehicle) }");
					Console.WriteLine($"Starships: { StarshipController.FormatStarshipLine(starship) }");
					Console.WriteLine($"Films: { FilmController.FormatFilmLine(film) }");
					Console.WriteLine("============================================================");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error: { ex.Message }");
				}

				Console.Write("Do you want to look up another character (y/n)?");
				lookUpAnother = Console.ReadLine();
				
			} while (lookUpAnother.ToLower() == "y");

			return null;
		}

		public static string FormatMeasurementLine(PersonModel person)
		{
			string heightString = "";
			string weightString = "";
			string fullMeasurements = "";

			if (person.Height.ToLower() != "unknown")
			{
				heightString = $"{ person.Height}cm";

			}
			else
			{
				heightString = person.Height;
			}

			if (person.Mass.ToLower() != "unknown")
			{
				weightString = $"{ person.Mass}kg";
			}
			else
			{
				weightString = person.Mass;
			}

			fullMeasurements = $"Height: { heightString } | Weight: { weightString }";

			return fullMeasurements;
		}

		public static string FormatPersonLine(List<PersonModel> person)
		{
			string personString = "";
			

			if (person.Count > 0)
			{
				foreach (PersonModel model in person)
				{
					personString += model.FullName + "\n\t";
				}
				personString = personString.Substring(0, personString.Length - 2);
			}
			else
			{
				personString += "none";
			}

			return personString;
		}

		public static async Task<List<PersonModel>> GetAllPeople()
		{
			string url = "https://swapi.dev/api/people/";
			List<PersonModel> output = new List<PersonModel>();

			do
			{
				using (HttpResponseMessage response = await apiClient.GetAsync(url))
				{
					if (response.IsSuccessStatusCode)
					{
						// if response was successful 
						// read the content of the response asynchronously
						// map it over to the person model
						//List<PersonModel> output = await response.Content.ReadAsAsync<List<PersonModel>>();
						var responseBody = await response.Content.ReadAsStringAsync();
						var responseJson = JObject.Parse(responseBody);
						var results = responseJson["results"].ToString();
						var count = int.Parse(responseJson["count"].ToString());

						if (count == peopleCache.Count)
						{
							return peopleCache;
						}
						else
						{
							peopleCache = new List<PersonModel>();
						}
						url = responseJson["next"].ToString();

						output.AddRange(JsonConvert.DeserializeObject<IEnumerable<PersonModel>>(results).ToList());

					}
					else
					{
						throw new Exception(response.ReasonPhrase);
					}
				}
			} while (url != "");

			peopleCache = output;

			return output;

		}

		public static async Task<List<PersonModel>> GetStarWarsCharacters(string[] id)
		{
			string url = "";
			List<PersonModel> model = new List<PersonModel>();

			foreach (string apiLink in id)
			{
				apiLink.Split(',');

				url = $"{ apiLink }";

				PersonModel cahced = peopleCache.Where(s => s.Id == apiLink).FirstOrDefault();

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
							PersonModel output = await response.Content.ReadAsAsync<PersonModel>();
							output.Id = apiLink;

							model.Add(output);
							peopleCache.Add(output);
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

		public static async Task<PersonModel> GetStarWarsCharacter(string id)
		{
			string url = $"https://swapi.dev/api/people/{ id }/";

			PersonModel cached = peopleCache.Where(p => p.Id == id).FirstOrDefault();

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
					PersonModel output = await response.Content.ReadAsAsync<PersonModel>();
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
