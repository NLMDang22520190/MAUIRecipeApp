using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Google.Cloud.Firestore;
using MAUIRecipeApp.DTO;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;

namespace MAUIRecipeApp.ViewModel.UserView
{
    public partial class UserSavedCollectionPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<UserSavedCollectionDTO> userSavedCollectionList =
            new ObservableCollection<UserSavedCollectionDTO>();

        private FirestoreDb db;

        public UserSavedCollectionPageViewModel()
        {

        }



        public void OnAppearing()
        {
            db = FirestoreService.Instance.Db;
            LoadItem();
        }


        private void LoadItem()
        {
            LoadUserSavedCollection();
        }

        private async void LoadUserSavedCollection()
        {
            try
            {
                CollectionReference recipesRef = db.Collection("UserSavedFoodCollection");
                QuerySnapshot snapshot = await recipesRef.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        UserSavedCollection userSavedCollection = document.ConvertTo<UserSavedCollection>();
                        userSavedCollection.UserSavedCollectionId = document.Id;
                        UserSavedCollectionDTO userSavedCollectionDTO = new UserSavedCollectionDTO
                        {
                        };
                        UserSavedCollectionList.Add(userSavedCollectionDTO);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("error" + e.Message);
            }
        }

        private async Task<string> GetCollectionName(string id)
        {
            DocumentReference docRef = db.Collection("FoodCollections").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                FoodCollection foodCollection = snapshot.ConvertTo<FoodCollection>();
                return foodCollection.CollectionName;
            }
            return string.Empty;
        }
    }

}
