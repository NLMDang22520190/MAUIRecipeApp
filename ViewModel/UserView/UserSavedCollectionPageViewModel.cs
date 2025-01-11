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

        [RelayCommand]
        public async Task CollecionDetail(string UserSavedCollectionId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                                { "SelectedCollection", userSavedCollectionList.FirstOrDefault(x=>x.UserSavedCollectionId == UserSavedCollectionId)  }
            };

            await Shell.Current.GoToAsync($"collectiondetail", parameters);
        }

        [RelayCommand]

        public async Task DeleteCollection(string UserSavedCollectionId)
        {
            DocumentReference docRef = db.Collection("UserSavedFoodCollection").Document(UserSavedCollectionId);
            await docRef.DeleteAsync();
            LoadItem();
        }


        public void OnAppearing()
        {
            db = FirestoreService.Instance.Db;
            LoadItem();
        }


        private void LoadItem()
        {
            UserSavedCollectionList.Clear();
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
                        if (userSavedCollection.UIDString == UserService.Instance.CurrentUser.Uid)
                        {
                            userSavedCollection.UserSavedCollectionId = document.Id;
                            UserSavedCollectionDTO userSavedCollectionDTO = new UserSavedCollectionDTO
                            {
                                UserSavedCollectionId = userSavedCollection.UserSavedCollectionId,
                                CollectionName = string.IsNullOrEmpty(userSavedCollection.AlternateName) ?
                                    await GetCollectionName(userSavedCollection.FCIDString) : userSavedCollection.AlternateName,
                                UploadName = await GetCollectionOwnerName(userSavedCollection.FCIDString),
                                ImgUrl = await GetCollectionFirstFoodImgUrl(userSavedCollection.FCIDString),
                                FCIDString = userSavedCollection.FCIDString
                            };
                            UserSavedCollectionList.Add(userSavedCollectionDTO);
                        }
                       
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

        private async Task<string> GetCollectionOwnerName(string id)
        {
            DocumentReference docRef = db.Collection("FoodCollections").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                FoodCollection foodCollection = snapshot.ConvertTo<FoodCollection>();
                DocumentReference userRef = db.Collection("User").Document(foodCollection.UUIDString);
                DocumentSnapshot userSnapshot = await userRef.GetSnapshotAsync();
                if (userSnapshot.Exists)
                {
                    User user = userSnapshot.ConvertTo<User>();
                    return user.Username;
                }
            }
            return string.Empty;
        }

        private async Task<string> GetCollectionFirstFoodImgUrl(string FCID)
        {


            CollectionReference foodDetailRef = db.Collection("FoodCollectionDetail");
            Query query = foodDetailRef.WhereEqualTo("FCID", db.Document($"FoodCollections/{FCID}")).Limit(1);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    FoodCollectionDetail foodCollectionDetail = document.ConvertTo<FoodCollectionDetail>();
                    DocumentReference foodRef = db.Collection("FoodRecipes").Document(foodCollectionDetail.FRIDString);
                    DocumentSnapshot foodSnapshot = await foodRef.GetSnapshotAsync();
                    if (foodSnapshot.Exists)
                    {
                        FoodRecipe foodRecipe = foodSnapshot.ConvertTo<FoodRecipe>();
                        return foodRecipe.ImgUrl;
                    }
                }
            }
            return string.Empty;
        }
    }
}


