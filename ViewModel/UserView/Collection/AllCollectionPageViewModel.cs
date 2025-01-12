using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.DTO;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.UserView.Collection
{
    public partial class AllCollectionPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<UserSavedCollectionDTO> allCollection =
            new ObservableCollection<UserSavedCollectionDTO>();

        private FirestoreDb db = FirestoreService.Instance.Db;

        public AllCollectionPageViewModel()
        {

        }

        [RelayCommand]
        public async Task CollecionDetail(string UserSavedCollectionId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                                { "SelectedCollection", AllCollection.FirstOrDefault(x=>x.UserSavedCollectionId == UserSavedCollectionId)  }
            };

            await Shell.Current.GoToAsync($"collectiondetailinall", parameters);
        }


        public void OnAppearing()
        {
            //throw new NotImplementedException();
            LoadItem();
        }

        private void LoadItem()
        {
            AllCollection.Clear();
            LoadCollection();
        }

        private async void LoadCollection()
        {
            try
            {
                CollectionReference recipesRef = db.Collection("FoodCollections");
                QuerySnapshot snapshot = await recipesRef.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        FoodCollection collection = document.ConvertTo<FoodCollection>();

                        collection.FoodCollectionId = document.Id;
                        UserSavedCollectionDTO userSavedCollectionDTO = new UserSavedCollectionDTO
                        {
                            UserSavedCollectionId = collection.FoodCollectionId,
                            CollectionName = collection.CollectionName,
                            UploadName = await GetCollectionOwnerName(collection.UUIDString),
                            ImgUrl = await GetCollectionFirstFoodImgUrl(collection.FoodCollectionId),
                            FCIDString = collection.FoodCollectionId,
                            
                        };
                        AllCollection.Add(userSavedCollectionDTO);

                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("error" + e.Message);
            }
        }

        private async Task<string> GetCollectionOwnerName(string id)
        {
            DocumentReference userRef = db.Collection("User").Document(id);
            DocumentSnapshot userSnapshot = await userRef.GetSnapshotAsync();
            if (userSnapshot.Exists)
            {
                User user = userSnapshot.ConvertTo<User>();
                return user.Username;
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
