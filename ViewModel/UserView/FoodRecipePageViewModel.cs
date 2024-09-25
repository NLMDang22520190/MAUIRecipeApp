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
            LoadIngredientDetails();

            //UploaderName = DataProvider.Ins.DB.Users.FirstOrDefault(u => u.Uid == SelectedFoodRecipe.UploaderUid).Username;
        }

        private async void LoadSelectedFoodRecipe()
        {
            DocumentReference docRef = _db.Collection("FoodRecipes").Document(selectedFoodRecipeID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                // Chuyển đổi DocumentSnapshot sang đối tượng FoodRecipe
                SelectedFoodRecipe = snapshot.ConvertTo<FoodRecipe>();

            }
            else
            {
                Debug.WriteLine("Document does not exist!");
            }
        }

        private async void LoadIngredientDetails()
        {  // Collection "RecipeIngredients"
            CollectionReference recipeIngredientsCollection = _db.Collection("RecipeIngredients");

            // Bước 1: Lấy tất cả RecipeIngredients với Frid = selectedFoodRecipeID
            Query recipeIngredientsQuery = recipeIngredientsCollection.WhereEqualTo("Frid", selectedFoodRecipeID);
            QuerySnapshot recipeIngredientsSnapshot = await recipeIngredientsQuery.GetSnapshotAsync();
            Debug.WriteLine("RecipeIngredients count: " + recipeIngredientsSnapshot.Count);
            List<IngredientDetailDto> ingredientDetails = new List<IngredientDetailDto>();

            foreach (DocumentSnapshot recipeIngredientDoc in recipeIngredientsSnapshot.Documents)
            {
                var recipeIngredient = recipeIngredientDoc.ConvertTo<RecipeIngredient>();

                // Bước 2: Lấy chi tiết nguyên liệu từ collection "Ingredients" dựa trên Iid
                DocumentReference ingredientDocRef = _db.Collection("Ingredients").Document(recipeIngredient.Iid.ToString());
                DocumentSnapshot ingredientSnapshot = await ingredientDocRef.GetSnapshotAsync();

                if (ingredientSnapshot.Exists)
                {
                    var ingredient = ingredientSnapshot.ConvertTo<Ingredient>();

                    // Thêm chi tiết nguyên liệu vào danh sách
                    ingredientDetails.Add(new IngredientDetailDto
                    {
                        IngredientName = ingredient.IngredientName,
                        Quantity = recipeIngredient.Quantity ?? 0, // Chuyển Quantity từ double về decimal
                        MeasurementUnit = ingredient.MeasurementUnit
                    });
                }
            }

            IngredientDetails = new ObservableCollection<IngredientDetailDto>(ingredientDetails);   
        }
    }
}
