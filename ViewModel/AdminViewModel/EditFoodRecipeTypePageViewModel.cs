using CommunityToolkit.Mvvm.ComponentModel;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;

namespace MAUIRecipeApp.ViewModel.AdminViewModel
{
   public partial class EditFoodRecipeTypePageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<FoodRecipeType> foodRecipeTypes = new ObservableCollection<FoodRecipeType>();

        private readonly FirestoreDb _db;

        public EditFoodRecipeTypePageViewModel()
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

            LoadFoodRecipeTypes();
        }

        private async void LoadFoodRecipeTypes()
        {
            // Truy vấn để lấy các mục có IsDeleted = false
            try
            {
                CollectionReference recipesRef = _db.Collection("FoodRecipeTypes");
                Query query = recipesRef.WhereEqualTo("IsDeleted", false);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        FoodRecipeType recipe = document.ConvertTo<FoodRecipeType>();
                        FoodRecipeTypes.Add(recipe); // Thêm vào ObservableCollection
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
