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

        private List<FoodRecipe> allFoodRecipes = new List<FoodRecipe>();

        private FirestoreDb _db;

        public EditFoodRecipePageViewModel()
        {
           
        }

        [RelayCommand]
        public async Task EditFood(string frid)
        {
            await Shell.Current.GoToAsync($"editcurrentfoodrecipe?FRID={frid}");
        }

        [RelayCommand]
        public async Task ShowAllHidden()
        {
            var tempFoods = new List<FoodRecipe>(allFoodRecipes);
            FoodRecipes.Clear();
            foreach (var food in tempFoods)
            {
                if (food.IsDeleted == true)
                {
                    FoodRecipes.Add(food);
                }
            }
        }

        [RelayCommand]
        public async Task ShowAllNotApproved()
        {
            var tempFoods = new List<FoodRecipe>(allFoodRecipes);
            FoodRecipes.Clear();
            foreach (var food in tempFoods)
            {
                if (food.IsApproved == false)
                {
                    FoodRecipes.Add(food);
                }
            }
        }

        [RelayCommand]
        public async Task ShowAll()
        {
            FoodRecipes.Clear();
            LoadItem();
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
                LoadItem();
                return;
            }

            var tempFoods = new List<FoodRecipe>(allFoodRecipes);
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
            allFoodRecipes.Clear();
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
                        allFoodRecipes.Add(recipe); // Thêm vào ObservableCollection
                    }
                }

                FoodRecipes = new ObservableCollection<FoodRecipe>(allFoodRecipes);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }

       
    }
}
