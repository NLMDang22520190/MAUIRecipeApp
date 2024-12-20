using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;

namespace MAUIRecipeApp.ViewModel.UserView
{
	public partial class SubmitNewRecipeViewModel : ObservableObject
	{
		private readonly FirestoreDb _db;

		public SubmitNewRecipeViewModel()
		{
			_db = FirestoreDb.Create("your-project-id"); // Replace with your Firestore project ID.
			DifficultyOptions = new ObservableCollection<string> { "Easy", "Medium", "Hard" };
			FoodRecipeTypes = new ObservableCollection<FoodRecipeType>();
			Ingredients = new ObservableCollection<Ingredient>();

			LoadFoodRecipeTypes();
			LoadIngredients();
		}

		// Properties
		[ObservableProperty]
		private string recipeName;

		[ObservableProperty]
		private int? calories;

		[ObservableProperty]
		private int? cookingTime;

		[ObservableProperty]
		private string difficultyLevel;

		[ObservableProperty]
		private string healthBenefits;

		[ObservableProperty]
		private string imgUrl;

		[ObservableProperty]
		private string videoUrl;

		[ObservableProperty]
		private string frid;

		public ObservableCollection<string> DifficultyOptions { get; }

		public ObservableCollection<FoodRecipeType> FoodRecipeTypes { get; }

		public ObservableCollection<Ingredient> Ingredients { get; }

		// Commands
		[RelayCommand]
		public async Task SubmitRecipeAsync()
		{
			// Debug all the food types retrieved from the database
			Debug.WriteLine("All Food Types from Database:");
			if (FoodRecipeTypes.Count > 0)
			{
				foreach (var foodType in FoodRecipeTypes)
				{
					Debug.WriteLine($"- {foodType.FoodTypeName}");
				}
			}
			else
			{
				Debug.WriteLine("No Food Types found in the database.");
			}

			// Debug all the ingredients retrieved from the database
			Debug.WriteLine("All Ingredients from Database:");
			if (Ingredients.Count > 0)
			{
				foreach (var ingredient in Ingredients)
				{
					Debug.WriteLine($"- {ingredient.IngredientName}");
				}
			}
			else
			{
				Debug.WriteLine("No Ingredients found in the database.");
			}

			// Debugging the recipe details
			Debug.WriteLine("Submitting Recipe Details:");
			Debug.WriteLine($"Recipe Name: {RecipeName}");
			Debug.WriteLine($"Calories: {Calories}");
			Debug.WriteLine($"Cooking Time: {CookingTime} mins");
			Debug.WriteLine($"Difficulty Level: {DifficultyLevel}");
			Debug.WriteLine($"Health Benefits: {HealthBenefits}");
			Debug.WriteLine($"Image URL: {ImgUrl}");
			Debug.WriteLine($"Video URL: {VideoUrl}");

			// Debug selected food types
			Debug.WriteLine("Selected Food Types:");
			if (FoodRecipeTypes.Count > 0)
			{
				foreach (var foodType in FoodRecipeTypes)
				{
					Debug.WriteLine($"- {foodType.FoodTypeName}");
				}
			}
			else
			{
				Debug.WriteLine("No food types selected.");
			}

			// Debug selected ingredients
			Debug.WriteLine("Selected Ingredients:");
			if (Ingredients.Count > 0)
			{
				foreach (var ingredient in Ingredients)
				{
					Debug.WriteLine($"- {ingredient.IngredientName}");
				}
			}
			else
			{
				Debug.WriteLine("No ingredients selected.");
			}

			Debug.WriteLine("Recipe submitted successfully!");
		}
		// Methods
		private async void LoadFoodRecipeTypes()
		{
			try
			{
				var querySnapshot = await _db.Collection("FoodRecipeTypes").GetSnapshotAsync();
				foreach (var document in querySnapshot.Documents)
				{
					var foodType = document.ConvertTo<FoodRecipeType>();
					FoodRecipeTypes.Add(foodType);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading food recipe types: {ex.Message}");
			}
		}

		private async void LoadIngredients()
		{
			try
			{
				var querySnapshot = await _db.Collection("Ingredients").GetSnapshotAsync();
				foreach (var document in querySnapshot.Documents)
				{
					var ingredient = document.ConvertTo<Ingredient>();
					Ingredients.Add(ingredient);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading ingredients: {ex.Message}");
			}
		}
	}
}
