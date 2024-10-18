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
   public partial class EditIngredientsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Ingredient> ingredients = new ObservableCollection<Ingredient>();

        private readonly FirestoreDb _db;

        public EditIngredientsPageViewModel()
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

            LoadIngredients();
        }

        private async void LoadIngredients()
        {
            // Truy vấn để lấy các mục có IsDeleted = false
            try
            {
                CollectionReference recipesRef = _db.Collection("Ingredients");
                //Query query = recipesRef.WhereEqualTo("IsDeleted", false);
                QuerySnapshot snapshot = await recipesRef.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        Ingredient recipe = document.ConvertTo<Ingredient>();
                        recipe.Iid = document.Id; // Lấy FRID từ Document ID
                        Ingredients.Add(recipe); // Thêm vào ObservableCollection
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
