using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.UserView
{
	public partial class EditFoodRecipeViewModel : ObservableObject
	{
		#region Properties

		[ObservableProperty] private string recipeId;

		[ObservableProperty] private ObservableCollection<FoodRecipeType> foodRecipeTypes = new();
		[ObservableProperty] private FoodRecipeType selectedFoodType;

		[ObservableProperty] private ObservableCollection<Ingredient> ingredients = new();
		[ObservableProperty] private ObservableCollection<IngredientInfos> ingredientsWithNames = new();
		[ObservableProperty] private Ingredient selectedIngredient;
		[ObservableProperty] private double ingredientQuantity;

		[ObservableProperty] private string recipeName;
		[ObservableProperty] private string calories;
		[ObservableProperty] private string cookingTime;
		[ObservableProperty] private int difficulty;
		[ObservableProperty] private int portion;
		[ObservableProperty] private string imageUrl;
		[ObservableProperty] private string videoUrl;

		[ObservableProperty] private bool isSubmitEnabled;

		#endregion

		public EditFoodRecipeViewModel(string recipeId = null)
		{
			RecipeId = "FR20250108091553";

			LoadIngredientsAsync();
			LoadFoodRecipeTypesAsync();

			if (!string.IsNullOrWhiteSpace(recipeId))
			{
				LoadRecipeDataAsync("FR20250108091553");
			}
		}

		#region Property Change Handlers

		partial void OnRecipeNameChanged(string value) => UpdateSubmitButtonState();
		partial void OnCaloriesChanged(string value) => UpdateSubmitButtonState();
		partial void OnCookingTimeChanged(string value) => UpdateSubmitButtonState();
		partial void OnPortionChanged(int value) => UpdateSubmitButtonState();
		partial void OnSelectedFoodTypeChanged(FoodRecipeType value) => UpdateSubmitButtonState();
		partial void OnImageUrlChanged(string value) => UpdateSubmitButtonState();
		partial void OnVideoUrlChanged(string value) => UpdateSubmitButtonState();

		private void UpdateSubmitButtonState()
		{
			IsSubmitEnabled = ValidateInputs(out _);
		}

		#endregion

		#region Commands

		[RelayCommand]
		private async Task SubmitRecipeAsync()
		{
			if (!ValidateInputs(out string validationError))
			{
				await DisplayWarning("Validation Error", validationError);
				return;
			}

			try
			{
				var updatedRecipe = new FoodRecipe
				{
					Frid = RecipeId,
					RecipeName = RecipeName,
					Calories = int.TryParse(Calories, out var caloriesValue) ? caloriesValue : (int?)null,
					CookingTime = int.TryParse(CookingTime, out var cookingTimeValue) ? cookingTimeValue : (int?)null,
					DifficultyLevel = Difficulty switch
					{
						0 => "Easy",
						1 => "Medium",
						2 => "Hard",
						_ => "Unknown"
					},
					Portion = Portion,
					ImgUrl = ImageUrl,
					VideoUrl = VideoUrl,
					IsDeleted = false
				};

				bool isSuccess = await FoodRecipeService.Instance.UpdateFoodRecipe(RecipeId, updatedRecipe);
				if (!isSuccess)
				{
					await DisplayWarning("Update Error", "Failed to update the recipe.");
					return;
				}

				await DisplayWarning("Success", "Recipe updated successfully.");
			}
			catch (Exception ex)
			{
				await DisplayWarning("Error", $"An error occurred: {ex.Message}");
			}
		}

		[RelayCommand]
		private void AddIngredientToSelectedList()
		{
			if (SelectedIngredient == null || string.IsNullOrWhiteSpace(SelectedIngredient.IngredientName))
			{
				Application.Current.MainPage.DisplayAlert("Invalid Ingredient", "Please select a valid ingredient.", "OK");
				return;
			}

			if (ingredientQuantity <= 0)
			{
				Application.Current.MainPage.DisplayAlert("Invalid Quantity", "Please enter a valid quantity.", "OK");
				return;
			}

			var recipeIngredient = new IngredientInfos
			{
				Frid = RecipeId,
				Iid = SelectedIngredient.Iid,
				Name = SelectedIngredient.IngredientName,
				Quantity = ingredientQuantity,
				IsDeleted = false
			};

			IngredientsWithNames.Add(recipeIngredient);

			ingredientQuantity = 0;
			SelectedIngredient = null;
		}

		[RelayCommand]
		private void RemoveIngredient(IngredientInfos recipeIngredient)
		{
			if (recipeIngredient == null)
			{
				Application.Current.MainPage.DisplayAlert("Invalid Ingredient", "Please select a valid ingredient.", "OK");
				return;
			}

			IngredientsWithNames.Remove(recipeIngredient);
		}

		#endregion

		#region Data Loading

		private async Task LoadRecipeDataAsync(string recipeId)
		{
			try
			{
				var recipe = await FoodRecipeService.Instance.GetFoodRecipeById(recipeId);
				if (recipe == null)
				{
					await DisplayWarning("Error", "Recipe not found.");
					return;
				}

				RecipeName = recipe.RecipeName;
				Calories = recipe.Calories.ToString();
				CookingTime = recipe.CookingTime.ToString();
				Difficulty = recipe.DifficultyLevel switch
				{
					"Easy" => 0,
					"Medium" => 1,
					"Hard" => 2,
					_ => 0
				};
				Portion = (int)recipe.Portion;
				ImageUrl = recipe.ImgUrl;
				VideoUrl = recipe.VideoUrl;

				var ingredientInfos = await FoodRecipeService.Instance.GetIngredientsWithNameByRecipeId(recipeId);
				IngredientsWithNames = new ObservableCollection<IngredientInfos>(ingredientInfos);
			}
			catch (Exception ex)
			{
				await DisplayWarning("Error", $"Failed to load recipe data: {ex.Message}");
			}
		}

		private async Task LoadIngredientsAsync()
		{
			try
			{
				var ingredientList = await IngredientService.Instance.LoadAllIngredients();
				Ingredients = new ObservableCollection<Ingredient>(ingredientList.Where(i => i.IsDeleted != true));
			}
			catch (Exception ex)
			{
				await DisplayWarning("Error", $"Failed to load ingredients: {ex.Message}");
			}
		}

		private async Task LoadFoodRecipeTypesAsync()
		{
			try
			{
				var foodRecipeTypesList = await FoodRecipeTypeService.Instance.GetAllFoodRecipeTypesAsync();
				FoodRecipeTypes = new ObservableCollection<FoodRecipeType>(foodRecipeTypesList.Where(frt => frt.IsDeleted != true));
			}
			catch (Exception ex)
			{
				await DisplayWarning("Error", $"An error occurred: {ex.Message}");
			}
		}

		#endregion

		#region Validation

		private bool ValidateInputs(out string validationError)
		{
			validationError = string.Empty;

			if (string.IsNullOrWhiteSpace(RecipeName)) return SetError(out validationError, "Recipe Name is required.");
			if (!int.TryParse(Calories, out _)) return SetError(out validationError, "Valid Calories value is required.");
			if (!int.TryParse(CookingTime, out _)) return SetError(out validationError, "Valid Cooking Time is required.");
			if (Difficulty < 0 || Difficulty > 2) return SetError(out validationError, "Invalid difficulty level.");
			if (Portion <= 0) return SetError(out validationError, "Portion must be greater than zero.");
			if (string.IsNullOrWhiteSpace(ImageUrl)) return SetError(out validationError, "Image URL is required.");
			if (string.IsNullOrWhiteSpace(VideoUrl)) return SetError(out validationError, "Video URL is required.");
			if (SelectedFoodType == null) return SetError(out validationError, "Food Type must be selected.");

			return true;
		}

		private static bool SetError(out string validationError, string message)
		{
			validationError = message;
			return false;
		}

		private async Task DisplayWarning(string title, string message)
		{
			await Application.Current.MainPage.DisplayAlert(title, message, "OK");
		}

		[RelayCommand]
		private async Task LoadRecipeInfosAsync(string recipeId)
		{
			LoadRecipeDataAsync(recipeId);
		}

		#endregion
	}
}
