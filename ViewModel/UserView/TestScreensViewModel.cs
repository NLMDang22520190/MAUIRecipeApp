using CommunityToolkit.Mvvm.ComponentModel;
using MAUIRecipeApp.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MAUIRecipeApp.ViewModel.UserView
{
	public partial class TestScreensViewModel : ObservableObject, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		[ObservableProperty] private string currentUser = UserService.Instance.CurrentUser.Username.ToString();

		public ICommand NavigateToSubmitNewRecipe { get; }
		public ICommand NavigateToEditIngredients { get; }
		public ICommand NavigateToUserRecipes { get; }
		public ICommand NavigateToUserSavedRecipes { get; }
		public ICommand NavigateToEditCollection { get; }
		public ICommand NavigateToUserCollections { get; }

		public TestScreensViewModel()
		{
			NavigateToSubmitNewRecipe = new Command(async () => await MoveToSubmitNewRecipe());
			NavigateToEditIngredients = new Command(async () => await MoveToEditIngredients());
			NavigateToUserRecipes = new Command(async () => await MoveToUserRecipes());
			NavigateToUserSavedRecipes = new Command(async () => await MoveToUserSavedRecipe());
			NavigateToEditCollection = new Command(async () => await MoveToEditCollection());
			NavigateToUserCollections = new Command(async () => await MoveToUserCollections());
		}

		private async Task MoveToSubmitNewRecipe()
		{
			await Shell.Current.GoToAsync("///submitnewrecipe");
		}

		private async Task MoveToEditIngredients()
		{
			await Shell.Current.GoToAsync("///editfoodrecipe");
		}

		private async Task MoveToUserRecipes()
		{
			await Shell.Current.GoToAsync("///userrecipes");
		}

		private async Task MoveToUserSavedRecipe()
		{
			await Shell.Current.GoToAsync("///usersavedrecipes");
		}

		private async Task MoveToEditCollection()
		{
			await Shell.Current.GoToAsync("///editcollection");
		}

		private async Task MoveToUserCollections()
		{
			await Shell.Current.GoToAsync("///usercollections");
		}
	}
}
