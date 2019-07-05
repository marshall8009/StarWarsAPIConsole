using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//...
using ApiUsageChallenge.Controllers;
using System.Windows.Forms;
using System.Collections;

namespace ApiUsageChallenge
{
	partial class Program
	{
		public static HttpClient apiClient;

		static async Task Main(string[] args)
		{
			string returnToStart = "";
			do
			{
				await ProgramStart();
				Console.Write("\nPress r to return to start menu or x to exit program: ");
				returnToStart = Console.ReadLine();
				Console.Clear();
				if (returnToStart == "x")
				{
					Application.Exit();
				}
		
			} while (returnToStart.ToLower() == "r");
			
		}

		static async Task ProgramStart()
		{
			InitializeClient();
			string input = "";
			List<PersonModel> people = new List<PersonModel>();
			List<PlanetModel> planets = new List<PlanetModel>();
			List<VehicleModel> vehicles = new List<VehicleModel>();
			List<StarshipModel> starships = new List<StarshipModel>();
			List<SpeciesModel> species = new List<SpeciesModel>();
			List<FilModel> films = new List<FilModel>();

			input = StartMenu();

			try
			{
				switch (input.ToLower())
				{
					case "a":
						people = await CharacterController.GetAllPeople();
						await CharacterController.DisplayAllCharacters(people);
						break;
					case "b":
						planets = await PlanetController.GetAllPlanets();
						await PlanetController.DisplayAllPlanets(planets);
						break;
					case "c":
						vehicles = await VehicleController.GetAllVehicles();
						await VehicleController.DisplayAllVehicles(vehicles);
						break;
					case "d":
						starships = await StarshipController.GetAllStarships();
						await StarshipController.DisplayAllStarships(starships);
						break;
					case "e":
						species = await SpeciesController.GetAllSpecies();
						await SpeciesController.DisplayAllSpecies(species);
						break;
					case "f":
						films = await FilmController.GetAllFilms();
						await FilmController.DisplayAllFilms(films);
						break;
					default:
						Console.WriteLine("Invalid input");
						break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: { ex.Message }");
			}
		}

		private static string StartMenu()
		{
			string input = "";
			Console.WriteLine("A: Characters");
			Console.WriteLine("B: Planets");
			Console.WriteLine("C: Vehicles");
			Console.WriteLine("D: Starships");
			Console.WriteLine("E: Species");
			Console.WriteLine("F: Films");
			Console.Write("Enter a letter to display information: ");
			input = Console.ReadLine();
			return input;
		}

		private static void InitializeClient()
		{
			apiClient = new HttpClient();
			apiClient.DefaultRequestHeaders.Accept.Clear();

			// we want to accept application/json coming back
			apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			
		}

	}

}
