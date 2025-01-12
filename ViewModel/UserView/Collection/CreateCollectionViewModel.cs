using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MAUIRecipeApp.ViewModel.UserView
{
	public partial class CreateCollectionViewModel : ObservableObject
	{
		private readonly FirestoreDb _db = FirestoreService.Instance.Db;

		[ObservableProperty] private string collectionName;
		[ObservableProperty] private string description;
		[ObservableProperty] private bool isSubmitEnabled;

		[ObservableProperty] private ObservableCollection<FoodRecipe> foodRecipes = new();
		[ObservableProperty] private FoodRecipe selectedRecipe;
		[ObservableProperty] private ObservableCollection<FoodRecipe> recipesWithNames = new();
		[ObservableProperty] private bool isAddRecipeButtonEnabled;

		public CreateCollectionViewModel()
		{
			_ = LoadFoodRecipesAsync(); // Fire and forget to load recipes
		}

		partial void OnSelectedRecipeChanged(FoodRecipe value)
		{
			IsAddRecipeButtonEnabled = value != null; // Enable the Add button when a recipe is selected
		}

		partial void OnCollectionNameChanged(string value) => UpdateSubmitButtonState();
		partial void OnDescriptionChanged(string value) => UpdateSubmitButtonState();

		private void UpdateSubmitButtonState()
		{
			IsSubmitEnabled = ValidateInputs(out _);
		}

		private bool ValidateInputs(out string error)
		{
			if (string.IsNullOrWhiteSpace(CollectionName))
			{
				error = "Collection name is required.";
				return false;
			}

			if (string.IsNullOrWhiteSpace(Description))
			{
				error = "Description is required.";
				return false;
			}

			error = null;
			return true;
		}

		[RelayCommand]
		private async Task SubmitCollectionAsync()
		{
			if (!ValidateInputs(out var error))
			{
				await DisplayWarning("Invalid Inputs", error);
				return;
			}

			// Create the FoodCollection object
			var foodCollection = new FoodCollection
			{
				CollectionName = CollectionName,
				Description = Description,
				IsDeleted = false,
				UploaderId = _db.Collection("Users").Document(UserService.Instance.CurrentUser.Uid)
			};

			// Prepare the dictionary to add to Firestore
			var collectionDict = new Dictionary<string, object>
	{
		{ "CollectionName", foodCollection.CollectionName },
		{ "Description", foodCollection.Description },
		{ "IsDeleted", foodCollection.IsDeleted },
		{ "UploaderId", foodCollection.UploaderId }
	};

			// Add the collection to Firestore and get the ID (collectionId is a string)
			var collectionId = await FirestoreService.Instance.AddDocumentAsync("FoodCollections", collectionDict);

			if (collectionId == null)
			{
				await DisplayWarning("Error", "Failed to create the collection. Please try again.");
				return;
			}

			// Now, use the collectionId to get a DocumentReference
			var collectionDoc = _db.Collection("FoodCollections").Document(collectionId);

			// Add each selected recipe to the FoodCollectionDetail collection
			foreach (var recipe in RecipesWithNames)
			{
				var collectionDetail = new FoodCollectionDetail
				{
					FCID = collectionDoc,  // Now collectionDoc is a DocumentReference
					FRID = _db.Collection("FoodRecipes").Document(recipe.Frid),
				};

				// Prepare the dictionary to add to Firestore
				var recipeDict = new Dictionary<string, object>
		{
			{ "FCID", collectionDetail.FCID },
			{ "FRID", collectionDetail.FRID }
		};

				// Add the recipe detail to Firestore (FoodCollectionDetail)
				var recipeDetailDoc = await FirestoreService.Instance.AddDocumentAsync("FoodCollectionDetail", recipeDict);

				// Check if adding the recipe was successful
				if (recipeDetailDoc == null)
				{
					await DisplayWarning("Error", $"Failed to add recipe {recipe.RecipeName} to the collection. Please try again.");
					return;
				}
			}

			// Display success message and navigate to home page
			await DisplayWarning("Success", "Food collection created successfully!");
			await Shell.Current.GoToAsync("///home");
		}

		[RelayCommand]
		private void AddRecipe()
		{
			if (SelectedRecipe == null)
			{
				Application.Current.MainPage.DisplayAlert("Invalid Recipe", "Please select a valid recipe.", "OK");
				return;
			}

			if (RecipesWithNames.Contains(SelectedRecipe))
			{
				Application.Current.MainPage.DisplayAlert("Duplicate Recipe", "This recipe is already added to the collection.", "OK");
				return;
			}

			RecipesWithNames.Add(SelectedRecipe); // Add the selected recipe
			SelectedRecipe = null; // Clear the selection
			IsAddRecipeButtonEnabled = false; // Disable the button
		}

		[RelayCommand]
		private void RemoveRecipe(FoodRecipe recipe)
		{
			if (recipe != null)
			{
				RecipesWithNames.Remove(recipe);
			}
		}

		private async Task LoadFoodRecipesAsync()
		{
			try
			{
				var recipesRef = _db.Collection("FoodRecipes");
				var snapshot = await recipesRef.WhereEqualTo("IsDeleted", false).GetSnapshotAsync();

				FoodRecipes.Clear();

				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						var recipe = document.ConvertTo<FoodRecipe>();

						// Ensure Frid is populated for each recipe
						if (string.IsNullOrEmpty(recipe.Frid))
						{
							recipe.Frid = document.Id;  // Assign the document ID if Frid is not set
						}

						FoodRecipes.Add(recipe);
					}
				}
			}
			catch (Exception ex)
			{
				await DisplayWarning("Error", $"Failed to load recipes: {ex.Message}");
			}
		}


		private async Task DisplayWarning(string title, string message)
		{
			await Application.Current.MainPage.DisplayAlert(title, message, "OK");
		}
	}
}
