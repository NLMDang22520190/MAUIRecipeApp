using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
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
    public partial class HomePageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<FoodRecipeType> foodRecipeTypes = new ObservableCollection<FoodRecipeType>();

        [ObservableProperty]
        ObservableCollection<FoodRecipe> foodRecipes = new ObservableCollection<FoodRecipe>();

        private FirestoreDb db;

        public HomePageViewModel()
        {
            db = FirestoreService.Instance.Db;
            if (db == null)
            {
                Debug.WriteLine("Firestore DB is null");
                return;
            }
            LoadItem();
        }




        [RelayCommand]
        public async Task FoodDetail(string Frid)
        {
            await Shell.Current.GoToAsync($"fooddetail?FRID={Frid}");
        }

        private void LoadItem()
        {
            //// Lọc các phần tử không bị xóa trong FoodRecipeTypes
            //FoodRecipeTypes = new ObservableCollection<FoodRecipeType>(
            //    DataProvider.Ins.DB.FoodRecipeTypes.AsNoTracking()
            //    .Where(item => (bool)!item.IsDeleted).ToList());

            LoadFoodRecipes();
            LoadFoodRecipeTypes();
        }

        private async void LoadFoodRecipes()
        {
            // Truy vấn để lấy các mục có IsDeleted = false
            CollectionReference recipesRef = db.Collection("FoodRecipes");
            Query query = recipesRef.WhereEqualTo("IsDeleted", false);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

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

        private async void LoadFoodRecipeTypes()
        {
            try {
               
                CollectionReference recipeTypesRef = db.Collection("FoodRecipeTypes");
                Query query = recipeTypesRef.WhereEqualTo("IsDeleted", false);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        FoodRecipeType recipeType = document.ConvertTo<FoodRecipeType>();
                        recipeType.Tofid = document.Id; // Lấy FRTID từ Document ID
                        FoodRecipeTypes.Add(recipeType); // Thêm vào ObservableCollection
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("error: " + ex.Message);
            }

        }
    }
}
