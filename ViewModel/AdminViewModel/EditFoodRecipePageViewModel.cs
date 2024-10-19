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

        [ObservableProperty]
        private bool isBackdropPresented;

        [ObservableProperty] private int maxCookingTime;

        [ObservableProperty] private int maxCalorie;

        [ObservableProperty] private int maxPortion;

        private readonly FirestoreDb _db;

        public EditFoodRecipePageViewModel()
        {
            _db = FirestoreService.Instance.Db;
            if (_db == null)
            {
                Debug.WriteLine("Firestore DB is null");
                return;
            }
            LoadItem();
        }

        private void LoadItem()
        {
            //// Lọc các phần tử không bị xóa trong FoodRecipeTypes
            //FoodRecipeTypes = new ObservableCollection<FoodRecipeType>(
            //    DataProvider.Ins.DB.FoodRecipeTypes.AsNoTracking()
            //    .Where(item => (bool)!item.IsDeleted).ToList());

            LoadFoodRecipes();
        }

        private async void LoadFoodRecipes()
        {
            // Truy vấn để lấy các mục có IsDeleted = false
            try
            {
                CollectionReference recipesRef = _db.Collection("FoodRecipes");
                Query query = recipesRef.WhereEqualTo("IsDeleted", false);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                // Tạo các danh sách tạm để lưu trữ dữ liệu
                List<int?> cookingTimes = new List<int?>();
                List<int?> calories = new List<int?>();
                List<int?> portions = new List<int?>();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        FoodRecipe recipe = document.ConvertTo<FoodRecipe>();
                        recipe.Frid = document.Id; // Lấy FRID từ Document ID
                        FoodRecipes.Add(recipe); // Thêm vào ObservableCollection

                        // Thêm các giá trị vào danh sách nếu không null
                        if (recipe.CookingTime.HasValue)
                            cookingTimes.Add(recipe.CookingTime);

                        if (recipe.Calories.HasValue)
                            calories.Add(recipe.Calories);

                        if (recipe.Portion.HasValue)
                            portions.Add(recipe.Portion);
                    }
                }

                // Tính toán các giá trị lớn nhất
                MaxCookingTime = cookingTimes.Max() ?? 0;
                MaxCalorie = calories.Max() ?? 0;
                MaxPortion = portions.Max() ?? 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }

        [RelayCommand]
        public async void ToggleBackdrop()
        {
            IsBackdropPresented = !IsBackdropPresented;
            Debug.Write(isBackdropPresented);
        }
    }
}
