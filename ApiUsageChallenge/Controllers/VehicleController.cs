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
	class VehicleController
	{
		private static HttpClient apiClient = Program.apiClient;
		private static List<VehicleModel> vehicleCache = new List<VehicleModel>();

		public static async Task<List<VehicleModel>> DisplayAllVehicles(List<VehicleModel> vehicles)
		{
			string lookUpAnother = "";

			do
			{
				try
				{
					foreach (VehicleModel v in vehicles)
					{
						string id = v.Url.Split('/')[5];
						v.Id = id;
						Console.WriteLine($"{ v.Id } { v.Name }");
					}

					Console.Write("\nPlease enter and ID number: ");
					string idText = Console.ReadLine();

					VehicleModel vehicle = await GetSingleVehicle(idText);
					List<PersonModel> person = await CharacterController.GetStarWarsCharacters(vehicle.Pilots);
					List<FilModel> film = await FilmController.GetStarWarsFilms(vehicle.Films);

					Console.WriteLine("============================================================");
					Console.WriteLine($"Name: {vehicle.Name} | Model: {vehicle.Model}");
					Console.WriteLine($"Manufacturer: {vehicle.Manufacturer} | Class: {vehicle.VehicleClass}");
					Console.WriteLine($"Cost: {vehicle.CostInCredits} | Cargo Capacity: {vehicle.CargoCapacity}");
					Console.WriteLine($"Consumables: {vehicle.Consumables} | Max Speed: {vehicle.MaxAtmospheringSpeed} | Length: {vehicle.Length}m");
					Console.WriteLine($"Crew: {vehicle.Crew} | Passengers: {vehicle.Passengers}");
					Console.WriteLine($"Pilots: {CharacterController.FormatPersonLine(person)}");
					Console.WriteLine($"Films: {FilmController.FormatFilmLine(film)}");
					Console.WriteLine(String.Format("{0:N}", Convert.ToInt32(vehicle.CargoCapacity)));
					Console.WriteLine(String.Format("{0:#,##0.##}", Convert.ToInt32(vehicle.CargoCapacity)));
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

		public static string FormatVehicleLine(List<VehicleModel> vehicle)
		{
			string vehicleString = "";

			if (vehicle.Count > 0)
			{
				foreach (VehicleModel model in vehicle)
				{
					vehicleString += model.Name + "\n\t";
				}
				vehicleString = vehicleString.Substring(0, vehicleString.Length - 2);
			}
			else
			{
				vehicleString += "none";
			}

			return vehicleString;
		}

		public static async Task<List<VehicleModel>> GetAllVehicles()
		{
			string url = "https://swapi.co/api/vehicles";
			List<VehicleModel> output = new List<VehicleModel>();

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

						if (count == vehicleCache.Count)
						{
							return vehicleCache;
						}
						else
						{
							vehicleCache = new List<VehicleModel>();
						}
						url = responseJson["next"].ToString();

						output.AddRange(JsonConvert.DeserializeObject<IEnumerable<VehicleModel>>(results).ToList());
					}
					else
					{
						throw new Exception(response.ReasonPhrase);
					}
				}
			} while (url != "");

			vehicleCache = output;
			return output;
		}

		public static async Task<List<VehicleModel>> GetStarWarsVehicles(string[] id)
		{
			string url = "";
			List<VehicleModel> model = new List<VehicleModel>();

			foreach (string _id in id)
			{
				_id.Split(',');

				url = $"{ _id }";

				VehicleModel cached = vehicleCache.Where(v => v.Id == _id).FirstOrDefault();

				if (cached != null)
				{
					model.Add(cached);
				}
				else
				{
					using (HttpResponseMessage response = await apiClient.GetAsync(url))
					{
						if (response.IsSuccessStatusCode)
						{
							VehicleModel output = await response.Content.ReadAsAsync<VehicleModel>();
							output.Id = _id;

							model.Add(output);
							vehicleCache.Add(output);
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

		public static async Task<VehicleModel> GetSingleVehicle(string id)
		{
			string url = $"https://swapi.co/api/vehicles/{ id }/";

			VehicleModel cached = vehicleCache.Where(p => p.Id == id).FirstOrDefault();

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
					VehicleModel output = await response.Content.ReadAsAsync<VehicleModel>();
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
