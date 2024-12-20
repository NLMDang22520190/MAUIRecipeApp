using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace MAUIRecipeApp.View.UserView
{
	public partial class SubmitNewRecipeView : ContentPage
	{
		// ObservableCollection for dynamic data binding
		public ObservableCollection<Ingredient> Ingredients { get; set; } = new ObservableCollection<Ingredient>();

		public SubmitNewRecipeView()
		{
			InitializeComponent();

			// Assign IngredientsList to the ObservableCollection
			IngredientsList.ItemsSource = Ingredients;
		}

		// Add Ingredient Handler
		private void OnAddIngredient(object sender, EventArgs e)
		{
			// Check for valid ingredient input
			if (!string.IsNullOrWhiteSpace(IngredientNameEntry.Text) && !string.IsNullOrWhiteSpace(IngredientAmountEntry.Text))
			{
				// Add new ingredient to the collection
				Ingredients.Add(new Ingredient
				{
					Name = IngredientNameEntry.Text.Trim(),
					Amount = IngredientAmountEntry.Text.Trim(),
				});

				// Clear input fields after adding
				IngredientNameEntry.Text = string.Empty;
				IngredientAmountEntry.Text = string.Empty;
			}
			else
			{
				// Show error if input is invalid
				DisplayAlert("Error", "Please enter valid ingredient details.", "OK");
			}
		}

		// Remove Ingredient Handler
		private void OnRemoveIngredient(object sender, EventArgs e)
		{
			if (sender is Button button && button.BindingContext is Ingredient ingredient)
			{
				Ingredients.Remove(ingredient);
			}
		}

		// Edit Ingredient Handler
		private async void OnEditIngredient(object sender, EventArgs e)
		{
			if (sender is Button button && button.BindingContext is Ingredient ingredient)
			{
				string newName = await DisplayPromptAsync("Edit Ingredient", "Enter new name", initialValue: ingredient.Name);
				string newAmount = await DisplayPromptAsync("Edit Ingredient", "Enter new amount", initialValue: ingredient.Amount);
				string newUnit = await DisplayPromptAsync("Edit Ingredient", "Enter new unit", initialValue: ingredient.Unit);

				if (!string.IsNullOrWhiteSpace(newName) && !string.IsNullOrWhiteSpace(newAmount))
				{
					ingredient.Name = newName;
					ingredient.Amount = newAmount;
					ingredient.Unit = newUnit;
				}
			}
		}

		// Submit Recipe Handler
		private async void OnSubmitRecipe(object sender, EventArgs e)
		{
			var recipe = new Recipe
			{
				Name = NameEntry.Text,
				Calories = CaloriesEntry.Text,
				CookingTime = CookingTimeEntry.Text,
				Difficulty = (int)DifficultySlider.Value,
				Ingredients = Ingredients.ToList(),
				HealthBenefits = HealthBenefitsEditor.Text,
				FoodType = FoodTypePicker.SelectedItem?.ToString()
			};

			await DisplayAlert("Success", "Recipe submitted successfully!", "OK");
			ResetForm();
		}

		// Reset Form
		private void ResetForm()
		{
			NameEntry.Text = string.Empty;
			CaloriesEntry.Text = string.Empty;
			CookingTimeEntry.Text = string.Empty;
			HealthBenefitsEditor.Text = string.Empty;
			Ingredients.Clear();
			FoodTypePicker.SelectedItem = null;
			DifficultySlider.Value = 1;
		}

		// Handle DifficultySlider Value Changed
		private void DifficultySlider_ValueChanged(object sender, ValueChangedEventArgs e)
		{
			if (sender is Slider slider)
			{
				slider.Value = Math.Round(e.NewValue);
			}
		}
	}

	// Converter Class
	public class AmountUnitConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Ingredient ingredient)
			{
				return $"{ingredient.Amount} {ingredient.Unit}";
			}
			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

	// Ingredient Class
	public class Ingredient
	{
		public string Name { get; set; }
		public string Amount { get; set; }
		public string Unit { get; set; } // New property for units
	}

	// Recipe Class
	public class Recipe
	{
		public string Name { get; set; }
		public string Calories { get; set; }
		public string CookingTime { get; set; }
		public int Difficulty { get; set; }
		public List<Ingredient> Ingredients { get; set; }
		public string HealthBenefits { get; set; }
		public string FoodType { get; set; }
	}
}
