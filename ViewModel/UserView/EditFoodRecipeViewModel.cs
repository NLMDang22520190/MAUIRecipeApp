using CommunityToolkit.Mvvm.ComponentModel;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Google.Protobuf.WellKnownTypes;

namespace MAUIRecipeApp.ViewModel.UserView
{
	public partial class EditFoodRecipeViewModel : ObservableObject
	{
		[ObservableProperty]
		private ObservableCollection<FoodRecipe> foodRecipes = new();

		[ObservableProperty] private ObservableCollection<FoodRecipe> allFoodRecipes = new();
		[ObservableProperty] private FoodRecipe selectedFoodRecipe;

		[ObservableProperty] private ObservableCollection<FoodRecipeType> foodRecipeTypes = new();
		[ObservableProperty] private FoodRecipeType selectedFoodType;

		[ObservableProperty] private ObservableCollection<Ingredient> ingredients = new();
		[ObservableProperty] private ObservableCollection<IngredientInfos> ingredientInfosList = new();
		[ObservableProperty] private Ingredient selectedIngredient;
		[ObservableProperty] private double quantity;
		[ObservableProperty] private ObservableCollection<RecipeIngredient> recipeIngredients = new();

		[ObservableProperty] private string recipeName;
		[ObservableProperty] private string calories;
		[ObservableProperty] private string cookingTime;
		[ObservableProperty] private string healthBenefits;
		[ObservableProperty] private int difficulty;
		[ObservableProperty] private int portion;
		[ObservableProperty] private string imageUrl;
		[ObservableProperty] private string videoUrl;

		private FirestoreDb _db = FirestoreService.Instance.Db;

		public EditFoodRecipeViewModel()
		{
			LoadItem();
		}

        [RelayCommand]
        public async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        public void OnAppearing()
		{
			_db = FirestoreService.Instance.Db;
			if (_db == null)
			{
				Debug.WriteLine("Firestore DB is null");
				return;
			}
			LoadItem();
		}

		[RelayCommand]
		private async void Test()
		{
			IngredientInfosList.Clear();
			SelectedFoodType = null;

			// Check if the recipe is found
			if (SelectedFoodRecipe == null)
			{
				Debug.WriteLine("Selected food recipe is null");
				await Application.Current.MainPage.DisplayAlert("Error", "Recipe not found.", "OK");
				return;
			}

			// Map Firestore values to the UI properties
			Difficulty = SelectedFoodRecipe.DifficultyLevel switch
			{
				"Easy" => 1,
				"Medium" => 2,
				"Hard" => 3,
				_ => 0
			};

			RecipeName = SelectedFoodRecipe.RecipeName;
			Calories = SelectedFoodRecipe.Calories.ToString();
			CookingTime = SelectedFoodRecipe.CookingTime.ToString();
			HealthBenefits = SelectedFoodRecipe.HealthBenefits;
			Portion = SelectedFoodRecipe.Portion ?? 0;
			ImageUrl = SelectedFoodRecipe.ImgUrl;
			VideoUrl = SelectedFoodRecipe.VideoUrl;

			// After setting basic properties, load the related data
			LoadCurrentRecipeType();
			LoadRecipeIngredients();
		}

		#region Base Loading
		private void LoadItem()
		{
			LoadFoodRecipesByUserID();
			//LoadAllRecipes();
			LoadFoodRecipeTypes();
			LoadIngredients();
		}

		private async void LoadAllRecipes()
		{
			// Clear the existing recipes to prevent duplicates or old data
			Debug.WriteLine("Start Loading...");
			AllFoodRecipes.Clear();

			try
			{
				// Reference the "FoodRecipes" collection
				CollectionReference recipesRef = _db.Collection("FoodRecipes");

				// Query to get all non-deleted recipes
				Query query = recipesRef.WhereEqualTo("IsDeleted", false);

				// Fetch the snapshot asynchronously
				QuerySnapshot snapshot = await query.GetSnapshotAsync();

				// Loop through the documents returned by the query
				foreach (DocumentSnapshot document in snapshot.Documents)
				{
					if (document.Exists)
					{
						// Convert the document to a FoodRecipe object
						FoodRecipe recipe = document.ConvertTo<FoodRecipe>();
						recipe.Frid = document.Id; // Store the document ID in the FoodRecipe

						// Log the Recipe ID for debugging purposes
						Debug.WriteLine($"Recipe ID: {recipe.Frid}");

						// Add the recipe to the ObservableCollection to update the UI
						AllFoodRecipes.Add(recipe);
					}
				}

				// You can display a success message or update the UI after the data is loaded
				await Application.Current.MainPage.DisplayAlert("Success", $"Recipes loaded successfully. Count: {AllFoodRecipes.Count}", "OK");
			}
			catch (Exception ex)
			{
				// Handle any errors that may occur during the fetch process
				Debug.WriteLine($"Error loading all recipes: {ex.Message}");
				await Application.Current.MainPage.DisplayAlert("Error", "Failed to load recipes.", "OK");
			}
		}

		private async void LoadFoodRecipesByUserID()
		{
			if (_db == null)
			{
				Debug.WriteLine("Firestore DB is not initialized.");
				return;
			}

			try
			{
				// Reference the "FoodRecipes" collection
				var recipesRef = _db.Collection("FoodRecipes");

				// Query to filter recipes by UploaderUid and IsDeleted
				var query = recipesRef
					.WhereEqualTo("UploaderUid", _db.Collection("User").Document(UserService.Instance.CurrentUser.Uid))
					.WhereEqualTo("IsDeleted", false);

				// Fetch the snapshot asynchronously
				var snapshot = await query.GetSnapshotAsync();

				// Clear the current recipe collection to avoid duplicates
				AllFoodRecipes.Clear();

				// Process the documents in the snapshot
				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						// Convert Firestore document to FoodRecipe model
						var recipe = document.ConvertTo<FoodRecipe>();
						recipe.Frid = document.Id; // Assign the document ID to the recipe object
						AllFoodRecipes.Add(recipe);
					}
				}

				// Update the observable collection
				FoodRecipes = new ObservableCollection<FoodRecipe>(AllFoodRecipes);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Exception: Error loading recipes - {ex.Message}");
			}
		}

		private async void LoadFoodRecipeTypes()
		{
			try
			{
				var recipeTypesRef = _db.Collection("FoodRecipeTypes");
				var snapshot = await recipeTypesRef.WhereEqualTo("IsDeleted", false).GetSnapshotAsync();

				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						var recipeType = document.ConvertTo<FoodRecipeType>();
						recipeType.Tofid = document.Id;
						FoodRecipeTypes.Add(recipeType);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading recipe types: {ex.Message}");
			}
		}

		private async void LoadIngredients()
		{
			try
			{
				var ingredientsRef = _db.Collection("Ingredients");
				var snapshot = await ingredientsRef.WhereEqualTo("IsDeleted", false).GetSnapshotAsync();

				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						var ingredient = document.ConvertTo<Ingredient>();
						ingredient.Iid = document.Id;
						Ingredients.Add(ingredient);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading ingredients: {ex.Message}");
			}
		}

		#endregion

		#region Recipe Info Loading
		private async void LoadCurrentRecipeType()
		{
			try
			{
				// Ensure SelectedFoodRecipe is not null
				if (SelectedFoodRecipe == null)
				{
					return;
				}

				// Create a dictionary for fast lookup of FoodRecipeTypes
				var foodRecipeTypesDictionary = FoodRecipeTypes.ToDictionary(rt => rt.Tofid);

				var recipeTypeMappingRef = _db.Collection("FoodRecipeTypeMappings");
				var snapshot = await recipeTypeMappingRef
					.WhereEqualTo("Frid", SelectedFoodRecipe.Frid)
					.WhereEqualTo("IsDeleted", false)
					.GetSnapshotAsync();

				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						// Convert the document to a FoodRecipeTypeMapping object
						var recipeTypeMapping = document.ConvertTo<FoodRecipeTypeMapping>();

						// Look up the corresponding FoodRecipeType
						if (recipeTypeMapping.Tofid != null &&
							foodRecipeTypesDictionary.TryGetValue(recipeTypeMapping.Tofid, out var recipeType))
						{
							SelectedFoodType = recipeType; // Set the selected type
							return; // Exit after finding the match
						}
					}
				}

				// If no type is found, clear the selection and log a message
				SelectedFoodType = null;
				Debug.WriteLine("No matching recipe type found.");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading recipe type: {ex.Message}");
			}
		}

		private async void LoadRecipeIngredients()
		{
			try
			{
				// Ensure SelectedFoodRecipe is not null before proceeding
				if (SelectedFoodRecipe == null)
				{
					Debug.WriteLine("SelectedFoodRecipe is null");
					return;
				}

				// Load ingredients into a dictionary for faster lookup
				var ingredientsDictionary = Ingredients.ToDictionary(i => i.Iid);

				var recipeIngredientsRef = _db.Collection("RecipeIngredients");
				var snapshot = await recipeIngredientsRef
					.WhereEqualTo("Frid", SelectedFoodRecipe?.Frid)
					.WhereEqualTo("IsDeleted", false)
					.GetSnapshotAsync();

				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						var recipeIngredient = document.ConvertTo<RecipeIngredient>();
						recipeIngredient.Frid = document.Id;

						// Retrieve the matching ingredient using the dictionary
						var ingredient = ingredientsDictionary.GetValueOrDefault(recipeIngredient.Iid);

						if (ingredient != null)
						{
							// Create IngredientInfos based on the recipeIngredient and the ingredient data
							IngredientInfos ingredientInfo = new()
							{
								Iid = recipeIngredient.Iid,
								Name = ingredient.IngredientName,
								Quantity = recipeIngredient.Quantity ?? 0,
								Unit = ingredient.MeasurementUnit,
								IsDeleted = false
							};

							IngredientInfosList.Add(ingredientInfo);
						}
						else
						{
							Debug.WriteLine($"Ingredient with ID {recipeIngredient.Iid} not found.");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading recipe ingredients: {ex.Message}");
			}
		}

		[RelayCommand]
		private void AddIngredient()
		{
			if (SelectedIngredient == null || string.IsNullOrWhiteSpace(SelectedIngredient.IngredientName))
			{
				Application.Current.MainPage.DisplayAlert("Invalid Ingredient", "Please select a valid ingredient.", "OK");
				return;
			}

			if (Quantity <= 0)
			{
				Application.Current.MainPage.DisplayAlert("Invalid Quantity", "Please enter a valid quantity.", "OK");
				return;
			}

			if (string.IsNullOrEmpty(SelectedIngredient.Iid))
			{
				Application.Current.MainPage.DisplayAlert("Invalid Ingredient", "The selected ingredient does not have a valid ID.", "OK");
				return;
			}

			var ingredientInfo = new IngredientInfos
			{
				Iid = SelectedIngredient.Iid,
				Name = SelectedIngredient.IngredientName,
				Quantity = Quantity,
				Unit = SelectedIngredient.MeasurementUnit,
				IsDeleted = false
			};

			IngredientInfosList.Add(ingredientInfo);

			Quantity = 0;
			SelectedIngredient = null;
		}

		[RelayCommand]
		private void RemoveIngredient(IngredientInfos ingredient)
		{
			IngredientInfosList.Remove(ingredient);
		}
		#endregion

		#region Recipe Editing

		[RelayCommand]
		private void UpdateRecipe()
		{
			UpdateRecipeInfo();
			UpdateRecipeType();
			UpdateRecipeIngredients();
		}

		private async void UpdateRecipeInfo()
		{
			if (!ValidateRecipeDetails())
			{
				// Show error if validation fails
				return;
			}

			try
			{
				// Create a dictionary to store the updated recipe data
				var updatedData = new Dictionary<string, object>
				{

					{ "RecipeName", RecipeName },
					{ "Calories", double.Parse(Calories) },
					{ "CookingTime", int.Parse(CookingTime) },
					{ "HealthBenefits", HealthBenefits },
					{ "DifficultyLevel", GetDifficultyLevel() },
					{ "Portion", Portion },
					{ "ImgUrl", ImageUrl },
					{ "VideoUrl", VideoUrl }
				};

				// Get document id of the selected recipe
				var recipeId = SelectedFoodRecipe.Frid;

				// Update the recipe document with the new data
				await FirestoreService.Instance.UpdateDocumentAsync("FoodRecipes", recipeId, updatedData);

				await Application.Current.MainPage.DisplayAlert("Success", "Recipe information updated.", "OK");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error updating recipe info: {ex.Message}");
				await Application.Current.MainPage.DisplayAlert("Error", "Failed to update recipe info.", "OK");
			}
		}

		private string GetDifficultyLevel()
		{
			return Difficulty switch
			{
				0 => "Easy",
				1 => "Medium",
				2 => "Hard",
				_ => "Unknown"
			};
		}

		private async void UpdateRecipeType()
		{
			if (SelectedFoodType == null)
			{
				await Application.Current.MainPage.DisplayAlert("Invalid Type", "Please select a valid food recipe type.", "OK");
				return;
			}

			try
			{
				var recipeTypeMappingRef = _db.Collection("FoodRecipeTypeMappings")
					.WhereEqualTo("Frid", SelectedFoodRecipe.Frid);

				var snapshot = await recipeTypeMappingRef.GetSnapshotAsync();

				if (snapshot.Documents.Count > 0)
				{
					// Assuming the first match is the correct one to update
					var recipeTypeMappingDoc = snapshot.Documents.First();
					var recipeTypeMapping = recipeTypeMappingDoc.ConvertTo<FoodRecipeTypeMapping>();

					// Update the FoodRecipeType
					await recipeTypeMappingDoc.Reference.UpdateAsync(new Dictionary<string, object>
			{
				{ "Tofid", SelectedFoodType.Tofid }
			});

					await Application.Current.MainPage.DisplayAlert("Success", "Recipe type updated.", "OK");
				}
				else
				{
					await Application.Current.MainPage.DisplayAlert("Error", "Recipe type mapping not found.", "OK");
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error updating recipe type: {ex.Message}");
				await Application.Current.MainPage.DisplayAlert("Error", "Failed to update recipe type.", "OK");
			}
		}

		private async void UpdateRecipeIngredients()
		{
			try
			{
				// Remove old ingredients
				var recipeIngredientsRef = _db.Collection("RecipeIngredients")
					.WhereEqualTo("Frid", SelectedFoodRecipe.Frid);

				var snapshot = await recipeIngredientsRef.GetSnapshotAsync();
				foreach (var document in snapshot.Documents)
				{
					await document.Reference.DeleteAsync();
				}

				// Add new ingredients
				foreach (var ingredientInfo in IngredientInfosList)
				{
					await _db.Collection("RecipeIngredients").AddAsync(new Dictionary<string, object>
			{
				{ "Frid", SelectedFoodRecipe.Frid },
				{ "Iid", ingredientInfo.Iid },
				{ "Quantity", ingredientInfo.Quantity },
				{ "IsDeleted", ingredientInfo.IsDeleted }
			});
				}

				await Application.Current.MainPage.DisplayAlert("Success", "Recipe ingredients updated.", "OK");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error updating recipe ingredients: {ex.Message}");
				await Application.Current.MainPage.DisplayAlert("Error", "Failed to update recipe ingredients.", "OK");
			}
		}
		#endregion

		#region Validation
		private bool ValidateRecipeDetails()
		{
			// Validate Base Info
			if (string.IsNullOrWhiteSpace(RecipeName))
			{
				Application.Current.MainPage.DisplayAlert("Validation Error", "Recipe Name is required.", "OK");
				return false;
			}

			if (string.IsNullOrWhiteSpace(Calories) || !double.TryParse(Calories, out _))
			{
				Application.Current.MainPage.DisplayAlert("Validation Error", "Valid Calories value is required.", "OK");
				return false;
			}

			if (string.IsNullOrWhiteSpace(CookingTime) || !int.TryParse(CookingTime, out _))
			{
				Application.Current.MainPage.DisplayAlert("Validation Error", "Valid Cooking Time is required.", "OK");
				return false;
			}

			if (string.IsNullOrWhiteSpace(HealthBenefits))
			{
				Application.Current.MainPage.DisplayAlert("Validation Error", "Health Benefit is required.", "OK");
				return false;
			}

			if (Difficulty < 0 || Difficulty > 3)
			{
				Application.Current.MainPage.DisplayAlert("Validation Error", "Please select a valid difficulty level (1-3).", "OK");
				return false;
			}

			if (Portion <= 0)
			{
				Application.Current.MainPage.DisplayAlert("Validation Error", "Portion size must be greater than 0.", "OK");
				return false;
			}

			if (string.IsNullOrWhiteSpace(ImageUrl))
			{
				Application.Current.MainPage.DisplayAlert("Validation Error", "Image URL is required.", "OK");
				return false;
			}

			if (string.IsNullOrWhiteSpace(VideoUrl))
			{
				Application.Current.MainPage.DisplayAlert("Validation Error", "Video URL is required.", "OK");
				return false;
			}

			// Validate Food Type
			if (SelectedFoodType == null)
			{
				Application.Current.MainPage.DisplayAlert("Validation Error", "Please select a valid food type.", "OK");
				return false;
			}

			// Validate Ingredients
			if (IngredientInfosList == null || IngredientInfosList.Count == 0)
			{
				Application.Current.MainPage.DisplayAlert("Validation Error", "At least one ingredient is required.", "OK");
				return false;
			}

			foreach (var ingredientInfo in IngredientInfosList)
			{
				if (ingredientInfo.Quantity <= 0)
				{
					Application.Current.MainPage.DisplayAlert("Validation Error", $"Invalid quantity for ingredient {ingredientInfo.Name}.", "OK");
					return false;
				}
			}

			// If all validations pass
			return true;
		}
		#endregion

		[RelayCommand]
		public void Cancel()
		{
			Shell.Current.GoToAsync("///home");
		}
	}
}

