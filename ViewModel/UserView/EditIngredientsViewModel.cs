using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MAUIRecipeApp.ViewModel.UserView
{
	public partial class EditIngredientsViewModel : ObservableObject
	{
		[ObservableProperty]
		private string searchText;

		private ObservableCollection<Ingredient> _ingredients;

		public ObservableCollection<Ingredient> Ingredients
		{
			get => _ingredients;
			set => SetProperty(ref _ingredients, value);
		}

		[ObservableProperty]
		private Ingredient selectedIngredient;

		public ICommand NavigateToSubmitNewRecipe { get; }
		public ICommand NavigateToTestScreens { get; }
		public ICommand FilterIngredientsCommand { get; }

		public EditIngredientsViewModel()
		{
			NavigateToSubmitNewRecipe = new Command(async () => await MoveToSubmitNewRecipe());
			NavigateToTestScreens = new Command(async () => await MoveToTestScreens());

			// Initialize the Ingredients collection
			Ingredients = new ObservableCollection<Ingredient>();

			// Load ingredients when the ViewModel is initialized
			LoadIngredients();
		}

		// Load ingredients from Firestore
		private async void LoadIngredients()
		{
			try
			{
				var ingredientCollection = FirestoreService.Instance.Db.Collection("Ingredients");
				var snapshot = await ingredientCollection.GetSnapshotAsync();

				// Clear the existing Ingredients list
				Ingredients.Clear();

				foreach (var doc in snapshot.Documents)
				{
					var ingredient = doc.ConvertTo<Ingredient>();
					if (ingredient.IsDeleted != true)
					{
						Ingredients.Add(ingredient); // Add ingredient object
					}
				}

				Debug.WriteLine("Ingredients loaded successfully");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error loading ingredients: {ex.Message}");
			}
		}

		// Navigation commands
		private async Task MoveToSubmitNewRecipe()
		{
			await Shell.Current.GoToAsync("///submitnewrecipe");
		}

		private async Task MoveToTestScreens()
		{
			await Shell.Current.GoToAsync("///testing");
		}
	}
}
