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
    public partial class EditUserSavedRecipePageViewModel: ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<UserSavedRecipe> userSavedRecipes = new ObservableCollection<UserSavedRecipe>();

        private readonly FirestoreDb _db;

        public EditUserSavedRecipePageViewModel()
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
            LoadUserSavedRecipes();
        }

        private async void LoadUserSavedRecipes()
        {
            try
            {
                // Tham chiếu tới collection "UserDetails"
                CollectionReference userDetailsRef = _db.Collection("UserSavedRecipes");
                QuerySnapshot snapshot = await userDetailsRef.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        // Chuyển đổi document thành đối tượng UserDetail
                        UserSavedRecipe userSavedRecipes = document.ConvertTo<UserSavedRecipe>();

                        // Thêm UserDetail vào ObservableCollection
                        UserSavedRecipes.Add(userSavedRecipes);
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
