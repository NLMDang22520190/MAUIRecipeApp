using Android.Content;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUIRecipeApp.DTO;
using MAUIRecipeApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.UserView
{
    [QueryProperty(nameof(SelectedFoodRecipeID), "FRID")]
    public partial class FoodRecipePageViewModel : ObservableObject
    {
        [ObservableProperty]
        private int selectedFoodRecipeID;

        [ObservableProperty]
        private FoodRecipe selectedFoodRecipe;

        [ObservableProperty]
        private string uploaderName;

        [ObservableProperty]
        private ObservableCollection<IngredientDetailDto> ingredientDetails;


        public FoodRecipePageViewModel()
        {

        }

        public void OnAppearing()
        {
            LoadFoodRecipe();
        }

        [RelayCommand]
        public async Task Back()
        {

            await Shell.Current.Navigation.PopAsync();
        }

        private void LoadFoodRecipe()
        {
            SelectedFoodRecipe = DataProvider.Ins.DB.FoodRecipes.FirstOrDefault(fr => fr.Frid == selectedFoodRecipeID);

            var ingredients = DataProvider.Ins.DB.RecipeIngredients
                .Include("IidNavigation")
                .Where(ri => ri.Frid == selectedFoodRecipeID)
        .Select(ri => new IngredientDetailDto
        {
            IngredientName = ri.IidNavigation.IngredientName,
            Quantity = (decimal)ri.Quantity,
            MeasurementUnit = ri.IidNavigation.MeasurementUnit
        })
        .ToList();

            IngredientDetails = new ObservableCollection<IngredientDetailDto>(ingredients);

            UploaderName = DataProvider.Ins.DB.Users.FirstOrDefault(u => u.Uid == SelectedFoodRecipe.UploaderUid).Username;
        }


    }
}
