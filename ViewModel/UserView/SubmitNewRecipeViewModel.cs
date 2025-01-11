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
	public partial class SubmitNewRecipeViewModel : ObservableObject
	{
		#region Properties

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

		public SubmitNewRecipeViewModel()
		{
			LoadIngredientsAsync();
			LoadFoodRecipeTypesAsync();
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
		private async Task MoveToEditIngredientAsync()
		{
			await Shell.Current.GoToAsync("///editingredients");
		}

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
				string tempFrid = $"FR{DateTime.UtcNow:yyyyMMddHHmmss}";
				var newRecipe = new FoodRecipe
				{
					Frid = tempFrid,
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
					UploaderUid = null,
					VideoUrl = VideoUrl,
					IsDeleted = false
				};

				bool isSuccess = await FoodRecipeService.Instance.AddFoodRecipe(newRecipe);
				if (!isSuccess)
				{
					await DisplayWarning("Submission Error", "Failed to submit the recipe.");
					return;
				}

				if (SelectedFoodType != null)
				{
					bool isMappingSuccess = await FoodRecipeTypeMappingService.Instance.AddMapping(tempFrid, SelectedFoodType.Tofid);
					if (!isMappingSuccess)
					{
						await DisplayWarning("Submission Error", "Failed to map recipe to food type.");
						return;
					}
				}

				await DisplayWarning("Success", "Recipe submitted successfully.");
				await Shell.Current.GoToAsync("///userrecipes");
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
				Frid = Guid.NewGuid().ToString(),
				Iid = SelectedIngredient.Iid,
				Name = SelectedIngredient.IngredientName,
				Quantity = ingredientQuantity,
				IsDeleted = false
			};

			IngredientsWithNames.Add(recipeIngredient);

			// Clear the input fields after adding
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

		private async Task LoadIngredientsAsync()
		{
			try
			{
				var ingredientList = await IngredientService.Instance.LoadAllIngredients();
				Ingredients.Clear();

				foreach (var ingredient in ingredientList.Where(i => i.IsDeleted != true))
				{
					Ingredients.Add(ingredient);
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load ingredients: {ex.Message}", "OK");
			}
		}

		private async Task LoadFoodRecipeTypesAsync()
		{
			try
			{
				var foodRecipeTypesList = await FoodRecipeTypeService.Instance.GetAllFoodRecipeTypesAsync();
				FoodRecipeTypes.Clear();

				foreach (var foodRecipeType in foodRecipeTypesList.Where(frt => frt.IsDeleted != true))
				{
					FoodRecipeTypes.Add(foodRecipeType);
				}
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

		#endregion
	}
}
