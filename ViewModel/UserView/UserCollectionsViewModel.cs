using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.UserView
{
	public partial class UserCollectionsViewModel : ObservableObject
	{
		private readonly FirestoreDb _db = FirestoreService.Instance.Db;

		[ObservableProperty] private ObservableCollection<FoodCollection> allCollections = new();

		public UserCollectionsViewModel()
		{
			_ = LoadCollectionsAsync();
		}

		// Load collections created by the current user
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

		// Command to handle the navigation to collection details page
		[RelayCommand]
		public async Task CollectionDetail(string foodCollectionId)
		{
		}

		// Command to cancel and navigate back to the home page
		[RelayCommand]
		public async Task Cancel()
		{
			await Shell.Current.GoToAsync("///home");
		}

		[RelayCommand]
		public async Task Test()
		{
			_ = LoadCollectionsAsync();
		}
	}
}
