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
			var ingredientList = await IngredientService.Instance.LoadAllIngredients();
			Ingredients = new ObservableCollection<Ingredient>(ingredientList.Where(i => !(i.IsDeleted ?? false)));
		}

		private async Task LoadFoodRecipeTypesAsync()
		{
			var recipeTypes = await FoodRecipeTypeService.Instance.GetAllFoodRecipeTypesAsync();
			FoodRecipeTypes = new ObservableCollection<FoodRecipeType>(recipeTypes.Where(rt => rt.IsDeleted == false));
		}

		private bool ValidateInputs(out string error)
		{
			if (string.IsNullOrWhiteSpace(RecipeName)) return SetError(out error, "Recipe name is required.");
			if (!int.TryParse(Calories, out _)) return SetError(out error, "Calories must be a valid number.");
			if (!int.TryParse(CookingTime, out _)) return SetError(out error, "Cooking time must be a valid number.");
			if (string.IsNullOrWhiteSpace(HealthBenefit)) return SetError(out error, "Health benefits are required.");
			if (Portion <= 0) return SetError(out error, "Portion must be greater than zero.");
			if (SelectedFoodType == null) return SetError(out error, "A food type must be selected.");
			if (string.IsNullOrWhiteSpace(ImageUrl)) return SetError(out error, "Image URL is required.");
			if (string.IsNullOrWhiteSpace(VideoUrl)) return SetError(out error, "Video URL is required.");
			error = null;
			return true;
		}

		private static bool SetError(out string error, string message)
		{
			error = message;
			return false;
		}

		private async Task DisplayWarning(string title, string message)
		{
			await Application.Current.MainPage.DisplayAlert(title, message, "OK");
		}
	}
}
