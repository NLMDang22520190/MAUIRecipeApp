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

namespace MAUIRecipeApp.ViewModel.AdminViewModel
{
    public partial class EditFoodRecipeMappingPageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<FoodRecipeTypeMapping> foodRecipeTypeMappings = new ObservableCollection<FoodRecipeTypeMapping>();

        [ObservableProperty]
        ObservableCollection<FoodTypeMapDetailDto> foodTypeMapDetailDtos = new ObservableCollection<FoodTypeMapDetailDto>();

        private readonly FirestoreDb _db;

        public EditFoodRecipeMappingPageViewModel()
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

            LoadFoodRecipeMappings();
        }

        private async void LoadFoodRecipeMappings()
        {
            try
            {
                // Truy vấn để lấy các mục trong FoodRecipeTypeMappings
                CollectionReference mappingsRef = _db.Collection("FoodRecipeTypeMappings");
                QuerySnapshot mappingSnapshot = await mappingsRef.GetSnapshotAsync();

                foreach (DocumentSnapshot mappingDoc in mappingSnapshot.Documents)
                {
                    if (mappingDoc.Exists)
                    {
                        FoodRecipeTypeMapping mapping = mappingDoc.ConvertTo<FoodRecipeTypeMapping>();

                        // Lấy giá trị frid và tofid từ mapping
                        string frid = mapping.Frid;
                        string tofid = mapping.Tofid;

                        // Truy vấn để lấy RecipeName từ collection FoodRecipes
                        DocumentReference recipeRef = _db.Collection("FoodRecipes").Document(frid);
                        DocumentSnapshot recipeSnapshot = await recipeRef.GetSnapshotAsync();
                        string recipeName = recipeSnapshot.Exists ? recipeSnapshot.GetValue<string>("RecipeName") : "Unknown Recipe";

                        // Truy vấn để lấy FoodTypeName từ collection FoodRecipeTypes
                        DocumentReference foodTypeRef = _db.Collection("FoodRecipeTypes").Document(tofid);
                        DocumentSnapshot foodTypeSnapshot = await foodTypeRef.GetSnapshotAsync();
                        string foodTypeName = foodTypeSnapshot.Exists ? foodTypeSnapshot.GetValue<string>("FoodTypeName") : "Unknown Food Type";

                        // Tạo đối tượng FoodTypeMapDetailDto và thêm vào danh sách
                        FoodTypeMapDetailDto detailDto = new FoodTypeMapDetailDto
                        {
                            RecipeName = recipeName,
                            FoodTypeName = foodTypeName,
                            IsDeleted = mapping.IsDeleted
                        };

                        foodTypeMapDetailDtos.Add(detailDto);
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
