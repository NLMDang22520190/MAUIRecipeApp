using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIRecipeApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.UserView
{
    public partial class HomePageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<FoodRecipeType> foodRecipeTypes;

        [ObservableProperty]
        ObservableCollection<FoodRecipe> foodRecipes;

        public HomePageViewModel()
        {
            LoadItem();
        }




        [RelayCommand]
        public async Task FoodDetail()
        {
            await Shell.Current.GoToAsync("//fooddetail");
        }

        private void LoadItem()
        {
            // Lọc các phần tử không bị xóa trong FoodRecipeTypes
            FoodRecipeTypes = new ObservableCollection<FoodRecipeType>(
                DataProvider.Ins.DB.FoodRecipeTypes.AsNoTracking()
                .Where(item => (bool)!item.IsDeleted).ToList());

            // Lọc các phần tử không bị xóa trong FoodRecipes
            FoodRecipes = new ObservableCollection<FoodRecipe>(
                DataProvider.Ins.DB.FoodRecipes.AsNoTracking()
                .Where(item => (bool)!item.IsDeleted).ToList());
        }
    }
}
