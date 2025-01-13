using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.UserView
{
	public partial class EditCollectionViewModel : ObservableObject
	{
		private readonly FirestoreDb _db = FirestoreService.Instance.Db;

		[ObservableProperty] private ObservableCollection<FoodCollection> allCollections = new();
		[ObservableProperty] private FoodCollection selectedCollection;

		[ObservableProperty] private string collectionName;
		[ObservableProperty] private string description;
		[ObservableProperty] private ObservableCollection<FoodRecipe> foodRecipes = new();
		[ObservableProperty] private FoodRecipe selectedRecipe;
		[ObservableProperty] private ObservableCollection<FoodRecipe> recipesInCollection = new();
		[ObservableProperty] private bool isAddRecipeButtonEnabled;
		[ObservableProperty] private bool isSubmitEnabled;

		public EditCollectionViewModel()
		{
			_ = LoadCollectionsAsync(); // Load collections
			_ = LoadFoodRecipesAsync(); // Load available recipes
		}

		partial void OnSelectedCollectionChanged(FoodCollection value)
		{
			if (value != null)
			{
				CollectionName = value.CollectionName;
				Description = value.Description;
				_ = LoadRecipesInCollectionAsync(value);
			}
		}

		partial void OnSelectedRecipeChanged(FoodRecipe value)
		{
			IsAddRecipeButtonEnabled = value != null; // Enable button when recipe is selected
		}

		partial void OnCollectionNameChanged(string value) => UpdateSubmitButtonState();
		partial void OnDescriptionChanged(string value) => UpdateSubmitButtonState();

		private void UpdateSubmitButtonState()
		{
			IsSubmitEnabled = !string.IsNullOrWhiteSpace(CollectionName) && !string.IsNullOrWhiteSpace(Description);
		}

		[RelayCommand]
		private async Task SaveChangesAsync()
		{
			if (SelectedCollection == null) return;

			// Update collection data
			SelectedCollection.CollectionName = CollectionName;
			SelectedCollection.Description = Description;

			var collectionDict = new Dictionary<string, object>
			{
				{ "CollectionName", CollectionName },
				{ "Description", Description }
			};

			await _db.Collection("FoodCollections").Document(SelectedCollection.FoodCollectionId).UpdateAsync(collectionDict);

			// Update recipes in the collection
			await UpdateRecipesInCollectionAsync();

			await Application.Current.MainPage.DisplayAlert("Success", "Collection updated successfully!", "OK");
			await Shell.Current.GoToAsync("///home");
		}

		[RelayCommand]
		private void AddRecipe()
		{
			if (SelectedRecipe == null || RecipesInCollection.Contains(SelectedRecipe)) return;

			RecipesInCollection.Add(SelectedRecipe);
			SelectedRecipe = null; // Clear selection
		}

		[RelayCommand]
		private void RemoveRecipe(FoodRecipe recipe)
		{
			if (recipe != null)
			{
				RecipesInCollection.Remove(recipe);
			}
		}

		private async Task LoadCollectionsAsync()
		{
			try
			{
				// Get the current user's ID
				var currentUserUid = UserService.Instance.CurrentUser.Uid;

				// Query to get collections created by the current user
				var snapshot = await _db.Collection("FoodCollections")
					.WhereEqualTo("UploaderId", _db.Collection("Users").Document(currentUserUid)) // Filter by current user's UploaderId
					.WhereEqualTo("IsDeleted", false) // Only non-deleted collections
					.GetSnapshotAsync();

				AllCollections.Clear();

				foreach (var document in snapshot.Documents)
				{
					var collection = document.ConvertTo<FoodCollection>();
					collection.FoodCollectionId = document.Id;
					AllCollections.Add(collection);
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load collections: {ex.Message}", "OK");
			}
		}

		private async Task LoadFoodRecipesAsync()
		{
			var snapshot = await _db.Collection("FoodRecipes").WhereEqualTo("IsDeleted", false).GetSnapshotAsync();
			FoodRecipes.Clear();
			foreach (var document in snapshot.Documents)
			{
				var recipe = document.ConvertTo<FoodRecipe>();
				recipe.Frid = document.Id;
				FoodRecipes.Add(recipe);
			}
		}

		private async Task LoadRecipesInCollectionAsync(FoodCollection collection)
		{
			var snapshot = await _db.Collection("FoodCollectionDetail")
				.WhereEqualTo("FCID", _db.Collection("FoodCollections").Document(collection.FoodCollectionId))
				.GetSnapshotAsync();

			RecipesInCollection.Clear();
			foreach (var document in snapshot.Documents)
			{
				var detail = document.ConvertTo<FoodCollectionDetail>();
				var recipeDoc = await detail.FRID.GetSnapshotAsync();
				if (recipeDoc.Exists)
				{
					var recipe = recipeDoc.ConvertTo<FoodRecipe>();
					recipe.Frid = recipeDoc.Id;
					RecipesInCollection.Add(recipe);
				}
			}
		}

		private async Task UpdateRecipesInCollectionAsync()
		{
			var collectionRef = _db.Collection("FoodCollections").Document(SelectedCollection.FoodCollectionId);

			// Remove existing details
			var existingDetails = await _db.Collection("FoodCollectionDetail")
				.WhereEqualTo("FCID", collectionRef)
				.GetSnapshotAsync();

			foreach (var doc in existingDetails.Documents)
			{
				await doc.Reference.DeleteAsync();
			}

			// Add updated details
			foreach (var recipe in RecipesInCollection)
			{
				var detail = new FoodCollectionDetail
				{
					FCID = collectionRef,
					FRID = _db.Collection("FoodRecipes").Document(recipe.Frid)
				};

				await _db.Collection("FoodCollectionDetail").AddAsync(detail);
			}
		}

		// Return to home screen
		[RelayCommand]
		public async Task Cancel()
		{
			await Shell.Current.GoToAsync("///home");
		}
		
	}
}
