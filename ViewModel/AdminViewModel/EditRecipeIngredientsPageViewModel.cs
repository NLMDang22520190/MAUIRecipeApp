using CommunityToolkit.Mvvm.ComponentModel;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIRecipeApp.DTO;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using CommunityToolkit.Mvvm.Input;

namespace MAUIRecipeApp.ViewModel.AdminViewModel
{
   public partial class EditRecipeIngredientsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<RecipeIngredient> recipeIngredients = new ObservableCollection<RecipeIngredient>();

        [ObservableProperty] private ObservableCollection<RecipeIngredientDetailDto> recipeIngredientDetailDtos =
            new ObservableCollection<RecipeIngredientDetailDto>();

        [ObservableProperty]
        private ObservableCollection<string> ingredientNameFilter = new ObservableCollection<string>();

        [ObservableProperty]
        private bool isBackdropPresented;

        [ObservableProperty] private double maxQuantity;

        private readonly FirestoreDb _db;

        public EditRecipeIngredientsPageViewModel()
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

            LoadRecipeIngredients();
        }

        private async void LoadRecipeIngredients()
        {
            try
            {
                // Truy vấn để lấy các mục trong RecipeIngredients với IsDeleted = false
                CollectionReference ingredientsRef = _db.Collection("RecipeIngredients");
                Query query = ingredientsRef.WhereEqualTo("IsDeleted", false);
                QuerySnapshot Snapshot = await query.GetSnapshotAsync();

                List<double?> quantities = new List<double?>();

                foreach (DocumentSnapshot ingredientDoc in Snapshot.Documents)
                {
                    if (ingredientDoc.Exists)
                    {
                        RecipeIngredient ingredient = ingredientDoc.ConvertTo<RecipeIngredient>();

                        // Lấy giá trị frid và iid từ RecipeIngredient
                        string frid = ingredient.Frid;
                        string iid = ingredient.Iid;

                        // Truy vấn để lấy RecipeName từ collection FoodRecipes
                        DocumentReference recipeRef = _db.Collection("FoodRecipes").Document(frid);
                        DocumentSnapshot recipeSnapshot = await recipeRef.GetSnapshotAsync();
                        string recipeName = recipeSnapshot.Exists ? recipeSnapshot.GetValue<string>("RecipeName") : "Unknown Recipe";

                        // Truy vấn để lấy IngredientName từ collection Ingredients
                        DocumentReference ingredientRef = _db.Collection("Ingredients").Document(iid);
                        DocumentSnapshot ingredientSnapshot = await ingredientRef.GetSnapshotAsync();
                        string ingredientName = ingredientSnapshot.Exists ? ingredientSnapshot.GetValue<string>("IngredientName") : "Unknown Ingredient";

                        // Tạo đối tượng RecipeIngredientDetailDto và thêm vào danh sách
                        RecipeIngredientDetailDto detailDto = new RecipeIngredientDetailDto
                        {
                            RecipeName = recipeName,
                            IngredientName = ingredientName,
                            Quantity = ingredient.Quantity ?? 0,
                            IsDeleted = ingredient.IsDeleted
                        };

                        AddIngredientNameToFilter(detailDto);

                        quantities.Add(detailDto.Quantity);

                        recipeIngredientDetailDtos.Add(detailDto);
                    }
                }
                maxQuantity = quantities.Max() ?? 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }

        private void AddIngredientNameToFilter(RecipeIngredientDetailDto dto)
        {
            if (!string.IsNullOrEmpty(dto.IngredientName) &&
                !IngredientNameFilter.Contains(dto.IngredientName))
            {
                IngredientNameFilter.Add(dto.IngredientName);
            }
        }

        [RelayCommand]
        public async void ToggleBackdrop()
        {
            IsBackdropPresented = !IsBackdropPresented;
        }

    }
}
