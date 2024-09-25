using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using MAUIRecipeApp.DTO;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
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
        private string selectedFoodRecipeID;

        [ObservableProperty]
        private FoodRecipe selectedFoodRecipe;

        [ObservableProperty]
        private string uploaderName;

        [ObservableProperty]
        private ObservableCollection<IngredientDetailDto> ingredientDetails;

        private FirestoreDb _db;
        public FoodRecipePageViewModel()
        {

        }

        public void OnAppearing()
        {
            _db = FirestoreService.Instance.Db;
            LoadFoodRecipe();
        }

        [RelayCommand]
        public async Task Back()
        {

            await Shell.Current.Navigation.PopAsync();
        }

        private void LoadFoodRecipe()
        {
            //SelectedFoodRecipe = DataProvider.Ins.DB.FoodRecipes.FirstOrDefault(fr => fr.Frid == selectedFoodRecipeID);
            LoadSelectedFoodRecipe();

            //    var ingredients = DataProvider.Ins.DB.RecipeIngredients
            //        .Include("IidNavigation")
            //        .Where(ri => ri.Frid == selectedFoodRecipeID)
            //.Select(ri => new IngredientDetailDto
            //{
            //    IngredientName = ri.IidNavigation.IngredientName,
            //    Quantity = (decimal)ri.Quantity,
            //    MeasurementUnit = ri.IidNavigation.MeasurementUnit
            //})
            //.ToList();

            //IngredientDetails = new ObservableCollection<IngredientDetailDto>(ingredients);

            //UploaderName = DataProvider.Ins.DB.Users.FirstOrDefault(u => u.Uid == SelectedFoodRecipe.UploaderUid).Username;
        }

        private async void LoadSelectedFoodRecipe()
        {
            DocumentReference docRef = _db.Collection("FoodRecipes").Document(selectedFoodRecipeID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                // Chuyển đổi DocumentSnapshot sang đối tượng FoodRecipe
                Debug.WriteLine("Document data for {0} document:" + snapshot.Id);
                SelectedFoodRecipe = snapshot.ConvertTo<FoodRecipe>();
                Debug.WriteLine("Selected Food Recipe: " + SelectedFoodRecipe.Frid);

            }
            else
            {
                Debug.WriteLine("Document does not exist!");
            }
        }

    }
}
