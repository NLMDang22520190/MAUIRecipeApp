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
	public partial class UserSavedRecipesViewModel : ObservableObject
	{
		[ObservableProperty]
		private ObservableCollection<FoodRecipe> savedRecipes = new ObservableCollection<FoodRecipe>();

		private readonly FirestoreService _firestoreService;
		private readonly FirestoreDb db;

		public UserSavedRecipesViewModel(FirestoreService firestoreService)
		{

			string UserID = "/User/UUID001";
			_firestoreService = firestoreService;
			db = _firestoreService.Db;
			if (db == null)
			{
				Debug.WriteLine("Firestore DB is null");
				return;
			}
			LoadSavedRecipes(UserID);
		}

		[RelayCommand]
		public async Task RecipeDetail(string frid)
		{
			await Shell.Current.GoToAsync($"fooddetail?FRID={frid}");
		}

		private async void LoadSavedRecipes(string UserID)
		{

			try
			{
				CollectionReference savedRecipesRef = db.Collection("UserSavedRecipes");
				Query query = savedRecipesRef.WhereEqualTo("UUID", UserID);
				QuerySnapshot snapshot = await query.GetSnapshotAsync();

				foreach (DocumentSnapshot document in snapshot.Documents)
				{
					if (document.Exists)
					{
						FoodRecipe recipe = document.ConvertTo<FoodRecipe>();
						recipe.Frid = document.Id;
						SavedRecipes.Add(recipe);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading saved recipes: {ex.Message}");
			}
		}
	}
}
