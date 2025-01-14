using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.UserView
{
	public partial class UserRecipesViewModel : ObservableObject
	{
		private readonly FirestoreDb _db = FirestoreService.Instance.Db;

		[ObservableProperty] private ObservableCollection<FoodRecipe> foodRecipes = new();

		public UserRecipesViewModel()
		{
			LoadFoodRecipesByUserID();
		}


		[RelayCommand]
		public async Task EditRecipe()
        {
           await Shell.Current.GoToAsync("editfoodrecipe");
        }

        // Load recipes created by the current user
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
				FoodRecipes.Clear();

				// Process the documents in the snapshot
				foreach (var document in snapshot.Documents)
				{
					if (document.Exists)
					{
						// Convert Firestore document to FoodRecipe model
						var recipe = document.ConvertTo<FoodRecipe>();
						recipe.Frid = document.Id; // Assign the document ID to the recipe object
						FoodRecipes.Add(recipe);
					}
				}

				// Update the observable collection
				FoodRecipes = new ObservableCollection<FoodRecipe>(FoodRecipes);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Exception: Error loading recipes - {ex.Message}");
			}
		}

		// Command to handle the navigation to recipe details page
		[RelayCommand]
		public async Task FoodDetail(string Frid)
		{
			await Shell.Current.GoToAsync("///home");
			await Shell.Current.GoToAsync($"fooddetail?FRID={Frid}");
		}

		// Cancel the action and navigate back
		[RelayCommand]
		public async Task Cancel()
		{
			await Shell.Current.GoToAsync("///home");
		}
	}
}
