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
using CommunityToolkit.Mvvm.Input;

namespace MAUIRecipeApp.ViewModel.AdminViewModel
{
   public partial class EditFoodRecipeTypePageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<FoodRecipeType> foodRecipeTypes = new ObservableCollection<FoodRecipeType>();

        private FirestoreDb _db;

        public EditFoodRecipeTypePageViewModel()
        {
            
        }

        [RelayCommand]
        public async Task EditFoodType(string tofid)
        {
            Debug.WriteLine(tofid);
            await Shell.Current.GoToAsync($"editcurrentfoodtype?TOFID={tofid}");
        }

        [RelayCommand]
        public async Task<bool> AddFoodType(string foodTypeName)
        {
            if (string.IsNullOrEmpty(foodTypeName))
            {
                return false;
            }
            var result = await FirestoreService.Instance.AddDocumentAsync("FoodRecipeTypes", new FoodRecipeType
            {
                FoodTypeName = foodTypeName,
                IsDeleted = false
            });
            if (result)
            {
                LoadItem();
            }
            return result;
        }

        public void SearchFoodType(string searchKey)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                FoodRecipeTypes.Clear();
                LoadItem();
                return;
            }

            var tempFoodTypes = new List<FoodRecipeType>(foodRecipeTypes);
            tempFoodTypes = foodRecipeTypes.ToList();
            FoodRecipeTypes.Clear();
            foreach (var food in tempFoodTypes)
            {
                if (food.FoodTypeName.ToLower().Contains(searchKey.ToLower()))
                {
                    FoodRecipeTypes.Add(food);
                }
            }
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




        private void LoadItem()
        {
            FoodRecipeTypes.Clear();
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
                        recipe.Tofid = document.Id;
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
