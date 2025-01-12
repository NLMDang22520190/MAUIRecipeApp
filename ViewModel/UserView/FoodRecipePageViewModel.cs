using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.DTO;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

		private ObservableCollection<Star> _stars;
		public ObservableCollection<Star> Stars
		{
			get => _stars;
			set
			{
				if (_stars != value)
				{
					_stars = value;
					OnPropertyChanged(nameof(Stars));
				}
			}
		}

		private ObservableCollection<UserRating> _userRatings;
		public ObservableCollection<UserRating> UserRatings
		{
			get => _userRatings;
			set
			{
				if (_userRatings != value)
				{
					_userRatings = value;
					OnPropertyChanged(nameof(UserRatings));
				}
			}
		}

		private FirestoreDb _db;
		public FoodRecipePageViewModel()
		{
			_db = FirestoreService.Instance.Db;

			Stars = new ObservableCollection<Star>
			{
				new Star { Glyph = "\uF005", Color = Colors.Gray }, // Default unselected star
				new Star { Glyph = "\uF005", Color = Colors.Gray },
				new Star { Glyph = "\uF005", Color = Colors.Gray },
				new Star { Glyph = "\uF005", Color = Colors.Gray },
				new Star { Glyph = "\uF005", Color = Colors.Gray },
			};

			UserRatings = new ObservableCollection<UserRating>();

		}

		public void OnAppearing()
		{
			_db = FirestoreService.Instance.Db;
			LoadFoodRecipe();
			LoadAllRatings();
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
			Debug.WriteLine($"Rate command executed with rating: {rating}");
			SelectedRating = int.Parse(rating);

			if (!int.TryParse(rating, out int selectedRating))
			{
				Debug.WriteLine("Invalid rating value.");
				return;
			}

			for (int i = 0; i < Stars.Count; i++)
			{
				if (i < selectedRating)
				{
					// Circle star

					Stars[i].Color = Colors.Gold;
				}
				else
				{
					// Empty star
					Stars[i].Color = Colors.Gray;
				}
			}

			Debug.WriteLine($"Updated Stars collection with rating: {selectedRating}");
		}


		[RelayCommand]
		private async Task SubmitRating()
		{
			try
			{
				if (SelectedFoodRecipe != null && UserService.Instance.CurrentUser != null)
				{
					// Check if user has already rated this recipe
					var existingRating = await CheckUserRating();

					if (existingRating != null)
					{
						// Inform the user they have already rated the recipe
						await DisplayAlert("Info", "You have already rated this recipe.");
						return;
					}

					// Ensure the comment isn't empty (if required)
					if (string.IsNullOrEmpty(Comment))
					{
						await DisplayAlert("Error", "Please provide a comment.");
						return;
					}

					// Create a FoodRating object to store the rating and comment
					var foodRating = new FoodRating
					{
						// Explicitly create DocumentReference for Frid and Uid
						Frid = _db.Collection("FoodRecipes").Document(SelectedFoodRecipeID), // Reference to FoodRecipe
						Uid = _db.Collection("User").Document(UserService.Instance.CurrentUser.Uid.ToString()), // Reference to User
						Rating = SelectedRating,
						Review = Comment,
						DateRated = DateTime.UtcNow,
						IsDeleted = false,
					};

					// Convert the FoodRating object to a Firestore document
					Dictionary<string, object> ratingData = new Dictionary<string, object>
			{
				{ "Frid", foodRating.Frid },
				{ "Uid", foodRating.Uid },
				{ "Rating", foodRating.Rating },
				{ "Review", foodRating.Review },
				{ "DateRated", foodRating.DateRated },
				{ "IsDeleted", foodRating.IsDeleted }
			};

					// Add the rating to the "FoodRatings" collection
					var addedDocRef = await _db.Collection("FoodRatings").AddAsync(ratingData);

					if (addedDocRef != null)
					{
						await DisplayAlert("Success", "Recipe rated successfully.");
						LoadAllRatings(); // Reload ratings after submission
					}
					else
					{
						await DisplayAlert("Error", "Failed to submit the rating.");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error adding rating: {ex.Message}");
				await DisplayAlert("Error", "An error occurred while rating the recipe.");
			}
		}

		private async Task<FoodRating> CheckUserRating()
		{
			FirestoreDb db = FirestoreService.Instance.Db;

			var ratingQuery = db.Collection("FoodRatings")
								.WhereEqualTo("Frid", SelectedFoodRecipeID)
								.WhereEqualTo("Uid", UserService.Instance.CurrentUser.Uid.ToString());

			var ratingSnapshot = await ratingQuery.GetSnapshotAsync();

			if (ratingSnapshot.Count > 0)
			{
				// Return the existing rating
				return ratingSnapshot.Documents[0].ConvertTo<FoodRating>();
			}

			return null; // No existing rating found
		}

		private async void LoadAllRatings()
		{
			try
			{
				// Collection "FoodRatings"
				CollectionReference foodRatingsCollection = _db.Collection("FoodRatings");

				// Get all ratings for the selected recipe
				Query foodRatingsQuery = foodRatingsCollection.WhereEqualTo("Frid", _db.Collection("FoodRecipes").Document(SelectedFoodRecipeID));
				QuerySnapshot foodRatingsSnapshot = await foodRatingsQuery.GetSnapshotAsync();

				List<UserRating> userRatingsList = new List<UserRating>();

				foreach (DocumentSnapshot ratingDoc in foodRatingsSnapshot.Documents)
				{
					// Convert the Firestore document to FoodRating object
					var foodRating = ratingDoc.ConvertTo<FoodRating>();

					if (foodRating == null)
					{
						Debug.WriteLine("Error converting FoodRating object.");
						continue;
					}

					// Retrieve the user's name using the Uid
					string userName = await GetUserName(foodRating.Uid.Id);

					// Create the UserRating DTO
					UserRating userRating = new UserRating
					{
						UserName = userName,
						Rating = foodRating.Rating,
						Comment = foodRating.Review,
						DateRated = foodRating.DateRated
					};

					userRatingsList.Add(userRating);
				}

				// Update the ObservableCollection
				UserRatings = new ObservableCollection<UserRating>(userRatingsList);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading ratings: {ex.Message}");
			}
		}

		private async Task<string> GetUserName(string userId)
		{
			try
			{
				DocumentReference userRef = _db.Collection("User").Document(userId);
				DocumentSnapshot userSnapshot = await userRef.GetSnapshotAsync();

				if (userSnapshot.Exists)
				{
					var user = userSnapshot.ConvertTo<User>();
					return user.Username ?? "Unknown";
				}
				return "Unknown";
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading user name: {ex.Message}");
				return "Unknown";
			}
		}

		private async Task DisplayAlert(string title, string message)
		{
			await Application.Current.MainPage.DisplayAlert(title, message, "OK");
		}

		public class Star : INotifyPropertyChanged
		{
			private string _glyph;
			private Color _color;

			public string Glyph
			{
				get => _glyph;
				set
				{
					if (_glyph != value)
					{
						_glyph = value;
						OnPropertyChanged(nameof(Glyph));
					}
				}
			}

			public Color Color
			{
				get => _color;
				set
				{
					if (_color != value)
					{
						_color = value;
						OnPropertyChanged(nameof(Color));
					}
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			protected void OnPropertyChanged(string propertyName)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
