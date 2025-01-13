using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;

namespace MAUIRecipeApp.ViewModel.UserView
{
    public partial class UserSavedRecipePageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<FoodRecipe> foodRecipes = new ObservableCollection<FoodRecipe>();

        private FirestoreDb db = FirestoreService.Instance.Db;

        public UserSavedRecipePageViewModel()
        {
        }

        [RelayCommand]
        public async Task FoodDetail(string Frid)
        {
            await Shell.Current.GoToAsync($"fooddetail?FRID={Frid}");
        }


        public void OnAppearing()
        {
            LoadItem();
        }

        private void LoadItem()
        {
            foodRecipes.Clear();
            LoadFoodRecipes();
        }

        private async void LoadFoodRecipes()
        {
            try
            {
                // Lấy danh sách các FRID trong UserSavedRecipes
                List<string> listSavedRecipeID = await GetListSavedRecipeID();

                // Tham chiếu đến collection FoodRecipes
                CollectionReference recipesRef = db.Collection("FoodRecipes");

                // Truy vấn các công thức không bị xóa
                Query query = recipesRef.WhereEqualTo("IsDeleted", false);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        FoodRecipe recipe = document.ConvertTo<FoodRecipe>();
                        recipe.Frid = document.Id;

                        // Kiểm tra nếu ID của món ăn có trong danh sách listSavedRecipeID
                        if (listSavedRecipeID.Contains(recipe.Frid))
                        {
                            FoodRecipes.Add(recipe);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private async Task<List<string>> GetListSavedRecipeID()
        {
            List<string> listSavedRecipeID = new List<string>();
            CollectionReference userSavedRecipeRef = db.Collection("UserSavedRecipes");
            Query query = userSavedRecipeRef.WhereEqualTo("UUID", db.Document("User/" + UserService.Instance.CurrentUser.Uid));
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    UserSavedRecipe userSavedRecipe = document.ConvertTo<UserSavedRecipe>();
                    listSavedRecipeID.Add(userSavedRecipe.FRIDString);
                }
            }
            return listSavedRecipeID;
        }
    }
}

