using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.DTO;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.UserView
{
	[QueryProperty(nameof(SelectedFoodRecipeID), "FRID")]
	public partial class FoodRecipePageViewModel : ObservableObject
	{
		[ObservableProperty]
		private string selectedFoodRecipeID;

		[ObservableProperty]
		private FoodRecipe selectedFoodRecipe;

		[ObservableProperty]
		private string uploaderName;

		[ObservableProperty]
		private string comment;

		[ObservableProperty]
		private int selectedRating;

		[ObservableProperty]
		private ObservableCollection<IngredientDetailDto> ingredientDetails;

		private FirestoreDb _db;
		public FoodRecipePageViewModel()
		{

		}

		public void OnAppearing()
		{
			_db = FirestoreService.Instance.Db;
			LoadFoodRecipe();
		}

		[RelayCommand]
		public async Task Back()
		{
			await Shell.Current.Navigation.PopAsync();
		}

		private void LoadFoodRecipe()
		{
			LoadSelectedFoodRecipe();
			LoadIngredientDetails();
		}

		private async void LoadSelectedFoodRecipe()
		{
			DocumentReference docRef = _db.Collection("FoodRecipes").Document(selectedFoodRecipeID);
			DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

			if (snapshot.Exists)
			{
				// Convert DocumentSnapshot to FoodRecipe object
				SelectedFoodRecipe = snapshot.ConvertTo<FoodRecipe>();
			}
			else
			{
				Debug.WriteLine("Document does not exist!");
			}
		}

		private async void LoadIngredientDetails()
		{
			// Collection "RecipeIngredients"
			CollectionReference recipeIngredientsCollection = _db.Collection("RecipeIngredients");

			// Step 1: Get all RecipeIngredients for selectedFoodRecipeID
			Query recipeIngredientsQuery = recipeIngredientsCollection.WhereEqualTo("Frid", selectedFoodRecipeID);
			QuerySnapshot recipeIngredientsSnapshot = await recipeIngredientsQuery.GetSnapshotAsync();
			List<IngredientDetailDto> ingredientDetailsList = new List<IngredientDetailDto>();

			foreach (DocumentSnapshot recipeIngredientDoc in recipeIngredientsSnapshot.Documents)
			{
				var recipeIngredient = recipeIngredientDoc.ConvertTo<RecipeIngredient>();

				// Step 2: Get ingredient details from "Ingredients" collection based on Iid
				DocumentReference ingredientDocRef = _db.Collection("Ingredients").Document(recipeIngredient.Iid.ToString());
				DocumentSnapshot ingredientSnapshot = await ingredientDocRef.GetSnapshotAsync();

				if (ingredientSnapshot.Exists)
				{
					var ingredient = ingredientSnapshot.ConvertTo<Ingredient>();

					// Add ingredient details to the list
					ingredientDetailsList.Add(new IngredientDetailDto
					{
						IngredientName = ingredient.IngredientName,
						Quantity = recipeIngredient.Quantity ?? 0, // Convert Quantity from double to decimal
						MeasurementUnit = ingredient.MeasurementUnit
					});
				}
			}

			IngredientDetails = new ObservableCollection<IngredientDetailDto>(ingredientDetailsList);
		}

		[RelayCommand]
		private async Task AddToSavedRecipes()
		{
			try
			{
				if (SelectedFoodRecipe != null && UserService.Instance.CurrentUser != null)
				{
					// Create a UserSavedRecipe object
					var userSavedRecipe = new UserSavedRecipe
					{
						FRID = _db.Collection("FoodRecipes").Document(SelectedFoodRecipeID), // Reference to FoodRecipe
						IsDeleted = false,
						UUID = _db.Collection("User").Document(UserService.Instance.CurrentUser.Uid.ToString()), // Reference to User
					};

					// Call the service to add the saved recipe
					bool isSaved = await UserSavedRecipesService.Instance.AddUserSavedRecipe(userSavedRecipe);

					if (isSaved)
					{
						await DisplayAlert("Success", "Recipe saved successfully.");
					}
					else
					{
						await DisplayAlert("Error", "Failed to save the recipe.");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding saved recipe: {ex.Message}");
				await DisplayAlert("Error", $"An error occurred while saving the recipe:\n {UserService.Instance.CurrentUser.Uid}\n {SelectedFoodRecipe.Frid}");
			}
		}

		[RelayCommand]
		private void Rate(string rating)
		{
			// Convert string to int
			SelectedRating = int.Parse(rating);
			Debug.WriteLine($"Selected Rating: {rating}");
		}

		[RelayCommand]
		private async Task SubmitRating()
		{
			try
			{
				if (SelectedFoodRecipe != null && UserService.Instance.CurrentUser != null)
				{
					// Create a FoodRating object
					var foodRating = new FoodRating
					{
						Frid = SelectedFoodRecipeID,
						IsDeleted = false,
						Rating = SelectedRating,
						Review = Comment,
						Uid = UserService.Instance.CurrentUser.Uid.ToString(),
						DateRated = DateTime.UtcNow
					};

					// Call the service to add the rating
					bool isRated = await FoodRatingService.Instance.AddRating(foodRating);

					if (isRated)
					{
						await DisplayAlert("Success", "Recipe rated successfully.");
					}
					else
					{
						await DisplayAlert("Error", "Failed to rate the recipe.");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding rating: {ex.Message}");
				await DisplayAlert("Error", "An error occurred while rating the recipe.");
			}
		}

		private async Task DisplayAlert(string title, string message)
		{
			await Application.Current.MainPage.DisplayAlert(title, message, "OK");
		}
	}
}
