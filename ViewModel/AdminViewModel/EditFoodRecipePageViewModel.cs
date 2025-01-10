using CommunityToolkit.Mvvm.ComponentModel;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace MAUIRecipeApp.ViewModel.AdminViewModel
{
   public partial class EditFoodRecipePageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<FoodRecipe> foodRecipes = new ObservableCollection<FoodRecipe>();

        private FirestoreDb _db;

        public EditFoodRecipePageViewModel()
        {
           
        }

        [RelayCommand]
        public async Task EditFood(string frid)
        {
            await Shell.Current.GoToAsync($"editcurrentfoodrecipe?FRID={frid}");
        }

        public void OnAppearing()
        {
            _db = FirestoreService.Instance.Db;
            if (_db == null)
            {
                Debug.WriteLine("Firestore DB is null");
                return;
            }
            LoadItem();
        }

        public void SearchFood(string searchKey)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                FoodRecipes.Clear();
                LoadItem();
                return;
            }

            var tempFoods = new List<FoodRecipe>(foodRecipes);
            tempFoods = foodRecipes.ToList();
            FoodRecipes.Clear();
            foreach (var food in tempFoods)
            {
                if (food.RecipeName.ToLower().Contains(searchKey.ToLower()))
                {
                    FoodRecipes.Add(food);
                }
            }
        }

        private void LoadItem()
        {
            FoodRecipes.Clear();
            LoadFoodRecipes();
        }

        private async void LoadFoodRecipes()
        {
            // Truy vấn để lấy các mục có IsDeleted = false
            try
            {
                CollectionReference recipesRef = _db.Collection("FoodRecipes");
                QuerySnapshot snapshot = await recipesRef.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        FoodRecipe recipe = document.ConvertTo<FoodRecipe>();
                        recipe.Frid = document.Id; // Lấy FRID từ Document ID
                        FoodRecipes.Add(recipe); // Thêm vào ObservableCollection
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }

       
    }
}
