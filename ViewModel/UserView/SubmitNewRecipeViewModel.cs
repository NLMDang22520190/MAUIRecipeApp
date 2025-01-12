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
	public partial class SubmitNewRecipeViewModel : ObservableObject
	{
		private FirestoreDb _db = FirestoreService.Instance.Db;

		[ObservableProperty] private ObservableCollection<FoodRecipeType> foodRecipeTypes = new();
		[ObservableProperty] private FoodRecipeType selectedFoodType;

		[ObservableProperty] private ObservableCollection<Ingredient> ingredients = new();
		[ObservableProperty] private ObservableCollection<IngredientInfos> ingredientsWithNames = new();
		[ObservableProperty] private Ingredient selectedIngredient;
		[ObservableProperty] private double ingredientQuantity;

		[ObservableProperty] private string recipeName;
		[ObservableProperty] private string calories;
		[ObservableProperty] private string cookingTime;
		[ObservableProperty] private string healthBenefit;
		[ObservableProperty] private int difficulty;
		[ObservableProperty] private int portion;
		[ObservableProperty] private string imageUrl;
		[ObservableProperty] private string videoUrl;

		[ObservableProperty] private bool isSubmitEnabled;

		public SubmitNewRecipeViewModel()
		{
			LoadIngredientsAsync();
			LoadFoodRecipeTypesAsync();
		}

		partial void OnRecipeNameChanged(string value) => UpdateSubmitButtonState();
		partial void OnCaloriesChanged(string value) => UpdateSubmitButtonState();
		partial void OnCookingTimeChanged(string value) => UpdateSubmitButtonState();
		partial void OnHealthBenefitChanged(string value) => UpdateSubmitButtonState();
		partial void OnPortionChanged(int value) => UpdateSubmitButtonState();
		partial void OnSelectedFoodTypeChanged(FoodRecipeType value) => UpdateSubmitButtonState();

		private void UpdateSubmitButtonState()
		{
			IsSubmitEnabled = ValidateInputs(out _);
		}

		[RelayCommand]
		private async Task SubmitRecipeAsync()
		{
			if (UserService.Instance.CurrentUser != null)
			{
				// Validate inputs
				if (!ValidateInputs(out var error))
				{
					await DisplayWarning("Invalid Inputs", error);
					return;
				}

				// Log input values
				Debug.WriteLine($"Recipe Name: {RecipeName}, Calories: {Calories}, Cooking Time: {CookingTime}, Portion: {Portion}, Food Type: {SelectedFoodType}");

				// Create new recipe
				var recipe = new FoodRecipe
				{
					RecipeName = RecipeName,
					Calories = int.Parse(Calories),
					CookingTime = int.Parse(CookingTime),
					HealthBenefits = HealthBenefit,
					DifficultyLevel = "Easy",
					Portion = Portion,
					ImgUrl = ImageUrl,
					VideoUrl = VideoUrl,
					IsDeleted = false,
					IsApproved = false,
					UploaderUid = _db.Collection("User").Document(UserService.Instance.CurrentUser.Uid),
				};

				// Convert selected food recipe to dictionary
				Dictionary<string, object> foodRecipeT = new Dictionary<string, object>
				{
					{ "UploaderUid", recipe.UploaderUid },
					{ "RecipeName", recipe.RecipeName },
					{ "Calories", recipe.Calories },
					{ "CookingTime", recipe.CookingTime },
					{ "HealthBenefits", recipe.HealthBenefits },
					{ "DifficultyLevel", recipe.DifficultyLevel },
					{ "Portion", recipe.Portion },
					{ "ImgUrl", recipe.ImgUrl },
					{ "VideoUrl", recipe.VideoUrl },
					{ "IsDeleted", recipe.IsDeleted },
					{ "IsApproved", recipe.IsApproved }
				};

				// Add recipe to firestore using firestore service and await the result
				var recipeId = await FirestoreService.Instance.AddDocumentAsync("FoodRecipes", foodRecipeT);
				Debug.WriteLine($"Recipe ID: {recipeId}");

				if (recipeId == null)
				{
					await DisplayWarning("Error", "Failed to submit recipe. Please try again.");
					return;
				}

				// Add recipe type to firestore using firestore service
				FoodRecipeType recipeType = SelectedFoodType;

				// Convert selected food recipe type to dictionary
				Dictionary<string, object> recipeTypeDict = new Dictionary<string, object>
				{
					{ "Frid", recipeId },
					{ "Tofid", recipeType.Tofid },
					{ "IsDeleted", false }
				};

				// Add food recipe type mapping to firestore and await the result
				var recipeTypeId = await FirestoreService.Instance.AddDocumentAsync("FoodRecipeTypeMappings", recipeTypeDict);
				Debug.WriteLine($"Recipe Type ID: {recipeTypeId}");

				if (recipeTypeId == null)
				{
					await DisplayWarning("Error", "Failed to submit recipe. Please try again.");
					return;
				}

				// Add ingredients to firestore using firestore service
				foreach (var ingredient in IngredientsWithNames)
				{
					// Create object for recipe ingredient
					RecipeIngredient recipeIngredient = new RecipeIngredient
					{
						Frid = recipeId.ToString(),
						Iid = ingredient.Iid,
						Quantity = ingredient.Quantity,
						IsDeleted = false
					};

					// Convert selected ingredient to dictionary
					Dictionary<string, object> ingredientDict = new Dictionary<string, object>
					{
						{ "Frid", recipeId },
						{ "Iid", recipeIngredient.Iid },
						{ "Quantity", recipeIngredient.Quantity },
						{ "IsDeleted", recipeIngredient.IsDeleted }
					};

					// Add recipe ingredient to firestore and await the result
					var ingredientId = await FirestoreService.Instance.AddDocumentAsync("RecipeIngredients", ingredientDict);
					Debug.WriteLine($"Ingredient ID: {ingredientId}");

					if (ingredientId == null)
					{
						await DisplayWarning("Error", "Failed to submit recipe. Please try again.");
						return;
					}
				}

				await DisplayWarning("Success", "Recipe submitted successfully!");

				// Return to home page
				await Shell.Current.GoToAsync("///home");
			}
		}

		[RelayCommand]
		private void AddIngredient()
		{
			// Check if the selected ingredient is null or empty
			if (SelectedIngredient == null || string.IsNullOrWhiteSpace(SelectedIngredient.IngredientName))
			{
				Application.Current.MainPage.DisplayAlert("Invalid Ingredient", "Please select a valid ingredient.", "OK");
				return;
			}

			// Check if the quantity is valid (greater than zero)
			if (ingredientQuantity <= 0)
			{
				Application.Current.MainPage.DisplayAlert("Invalid Quantity", "Please enter a valid quantity.", "OK");
				return;
			}

			// Check if the SelectedIngredient has a valid ID
			if (SelectedIngredient.Iid == null)
			{
				Application.Current.MainPage.DisplayAlert("Invalid Ingredient", "The selected ingredient does not have a valid ID.", "OK");
				return;
			}

			// Create the ingredient info to add
			var ingredientInfo = new IngredientInfos
			{
				Iid = SelectedIngredient.Iid,
				Name = SelectedIngredient.IngredientName,
				Quantity = ingredientQuantity,
				IsDeleted = false
			};

			// Add the ingredient to the list of ingredients
			IngredientsWithNames.Add(ingredientInfo);

			// Reset ingredient fields
			ingredientQuantity = 0;
			SelectedIngredient = null;
		}

		[RelayCommand]
		private void RemoveIngredient(IngredientInfos ingredient)
		{
			IngredientsWithNames.Remove(ingredient);
		}

		private async Task LoadIngredientsAsync()
		{
			try
			{
				// Get a reference to the Ingredients collection in Firestore
				CollectionReference ingredientsRef = _db.Collection("Ingredients");

				// Query the collection to get all ingredients where IsDeleted is false
				QuerySnapshot snapshot = await ingredientsRef.WhereEqualTo("IsDeleted", false).GetSnapshotAsync();

				// Clear the existing collection before adding new items
				Ingredients.Clear();

				// Iterate through the documents in the snapshot
				foreach (DocumentSnapshot document in snapshot.Documents)
				{
					if (document.Exists)
					{
						// Convert Firestore document to Ingredient object
						Ingredient ingredient = document.ConvertTo<Ingredient>();

						// Set the Iid (Ingredient ID) to the document ID
						ingredient.Iid = document.Id;

						// Add the ingredient to the ObservableCollection
						Ingredients.Add(ingredient);
					}
				}
			}
			catch (Exception ex)
			{
				// Log the error to the debug output
				Debug.WriteLine($"Error loading ingredients: {ex.Message}");
			}
		}

		private async Task LoadFoodRecipeTypesAsync()
		{
			var recipeTypes = await FoodRecipeTypeService.Instance.GetAllFoodRecipeTypesAsync();
			FoodRecipeTypes = new ObservableCollection<FoodRecipeType>(recipeTypes.Where(rt => rt.IsDeleted == false));
		}

		private bool ValidateInputs(out string error)
		{
			// Validate Recipe Name
			if (string.IsNullOrWhiteSpace(RecipeName))
			{
				error = "Recipe name is required.";
				Debug.WriteLine($"Validation failed: {error}");
				return false;
			}

			// Validate Calories
			if (!int.TryParse(Calories, out _))
			{
				error = "Calories must be a valid number.";
				Debug.WriteLine($"Validation failed: {error}");
				return false;
			}

			// Validate Cooking Time
			if (!int.TryParse(CookingTime, out _))
			{
				error = "Cooking time must be a valid number.";
				Debug.WriteLine($"Validation failed: {error}");
				return false;
			}

			// Validate Health Benefit
			if (string.IsNullOrWhiteSpace(HealthBenefit))
			{
				error = "Health benefits are required.";
				Debug.WriteLine($"Validation failed: {error}");
				return false;
			}

			// Validate Portion
			if (Portion <= 0)
			{
				error = "Portion must be greater than zero.";
				Debug.WriteLine($"Validation failed: {error}");
				return false;
			}

			// Validate Food Type
			if (SelectedFoodType == null)
			{
				error = "A food type must be selected.";
				Debug.WriteLine($"Validation failed: {error}");
				return false;
			}

			// Validate Image URL
			if (string.IsNullOrWhiteSpace(ImageUrl))
			{
				error = "Image URL is required.";
				Debug.WriteLine($"Validation failed: {error}");
				return false;
			}

			// Validate Video URL
			if (string.IsNullOrWhiteSpace(VideoUrl))
			{
				error = "Video URL is required.";
				Debug.WriteLine($"Validation failed: {error}");
				return false;
			}

			// If all validations pass
			error = null;
			return true;
		}


		private bool SetError(out string error, string errorMessage)
		{
			error = errorMessage;
			return false;
		}

		private async Task DisplayWarning(string title, string message)
		{
			await Application.Current.MainPage.DisplayAlert(title, message, "OK");
		}
	}
}
