using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MAUIRecipeApp.ViewModel.UserView
{
    public partial class UserRecipesViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<FoodRecipe> foodRecipes = new ObservableCollection<FoodRecipe>();

        [ObservableProperty]
        private bool showAllRecipes = true;

        // Add this property to control the button text
        public string ShowAllRecipesButtonText => ShowAllRecipes ? "Show User Recipes" : "Show All Recipes";

        public ICommand NavigateToTesting { get; }

        public IAsyncRelayCommand ToggleRecipesView { get; }

        public UserRecipesViewModel()
        {
            // Initialize commands
            ToggleRecipesView = new AsyncRelayCommand(ToggleRecipeSource);

            NavigateToTesting = new Command(async () => await MoveToTesting());

            // Load initial data
            _ = LoadAllRecipesAsync();
        }

        // Load all recipes
        private async Task LoadAllRecipesAsync()
        {
            try
            {
                var allRecipes = await FoodRecipeService.Instance.GetAllFoodRecipes();

                FoodRecipes.Clear();
                foreach (var recipe in allRecipes)
                {
                    FoodRecipes.Add(recipe);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Error loading recipes", "OK");
            }
        }

        // Load recipes for the current user
        private async Task LoadUserRecipesAsync()
        {
            try
            {
                var currentUser = UserService.Instance.CurrentUser;
                if (currentUser == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "User not found", "OK");
                    return;
                }

                var userRecipes = await FoodRecipeService.Instance.GetFoodRecipesByUserId(0);

                FoodRecipes.Clear();
                foreach (var recipe in userRecipes)
                {
                    FoodRecipes.Add(recipe);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Error loading user recipes", "OK");
            }
        }

        // Toggle between all recipes and user-specific recipes
        private async Task ToggleRecipeSource()
        {
            ShowAllRecipes = !ShowAllRecipes; // Toggle the boolean value

            if (ShowAllRecipes)
            {
                await LoadAllRecipesAsync();
            }
            else
            {
                await LoadUserRecipesAsync();
            }

            // Notify that the button text has changed (because ShowAllRecipes value changed)
            OnPropertyChanged(nameof(ShowAllRecipesButtonText));
        }

        private async Task MoveToTesting()
		{
			await Shell.Current.GoToAsync("///testing");
		}
    }
}
