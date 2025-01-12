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
    [QueryProperty(nameof(SelectedCollection), "SelectedCollection")]

    public partial class CollectionDetailInAllPageViewModel : ObservableObject
    {
        [ObservableProperty] private UserSavedCollectionDTO selectedCollection;

        [ObservableProperty] private ObservableCollection<FoodRecipe> foodRecipes = new ObservableCollection<FoodRecipe>();

        [ObservableProperty] private string collectionName = string.Empty;

        [ObservableProperty] private string selectedCollectionId = string.Empty;

        [ObservableProperty] private bool isSaved = false;

        private FirestoreDb db = FirestoreService.Instance.Db;
        public CollectionDetailInAllPageViewModel()
        {
        }

        [RelayCommand]
        public async Task Back()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        public async Task FoodDetail(string Frid)
        {
            await Shell.Current.GoToAsync($"collectiondetailinall/fooddetail?FRID={Frid}");
        }

        [RelayCommand]
        public async Task SaveCollection()
        {
            
            if (IsSaved)
            {
                return;
            }
            try
            {
                var addResult = await FirestoreService.Instance.AddDocumentAsync("UserSavedFoodCollection", new UserSavedCollection
                {
                    AlternateName = string.Empty,
                    FCID = db.Document($"FoodCollections/{selectedCollectionId}"),
                    UserSavedId = db.Document($"User/{UserService.Instance.CurrentUser.Uid}")
                });
                var result = !string.IsNullOrEmpty(addResult);
                if (result)
                {
                    IsSaved = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }



        public void OnAppearing()
        {
            LoadItem();
        }

        private void LoadItem()
        {
            LoadCollectionDetail();
        }

        private void LoadCollectionDetail()
        {
            CollectionName = selectedCollection.CollectionName;
            SelectedCollectionId = selectedCollection.FCIDString;
            FoodRecipes.Clear();
            LoadFoodRecipes();
            CheckSave();
        }

        private async void LoadFoodRecipes()
        {
            CollectionReference foodDetailRef = db.Collection("FoodCollectionDetail");
            Query query = foodDetailRef.WhereEqualTo("FCID", db.Document($"FoodCollections/{SelectedCollection.FCIDString}"));
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                FoodCollectionDetail foodCollectionDetail = document.ConvertTo<FoodCollectionDetail>();
                FoodRecipe foodRecipe = await GetFoodRecipe(foodCollectionDetail.FRIDString);
                if (foodRecipe != null)
                {
                    FoodRecipes.Add(foodRecipe);
                }
            }
        }

        private async Task<FoodRecipe> GetFoodRecipe(string FRID)
        {
            DocumentReference docRef = db.Collection("FoodRecipes").Document(FRID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                FoodRecipe foodRecipe = snapshot.ConvertTo<FoodRecipe>();
                foodRecipe.Frid = snapshot.Id;
                return foodRecipe;
            }
            return null;
        }

        private async void CheckSave()
        {
            CollectionReference userSavedCollectionRef = db.Collection("UserSavedFoodCollection");
            Query query = userSavedCollectionRef.WhereEqualTo("UserSavedId", db.Document($"User/{UserService.Instance.CurrentUser.Uid}")).WhereEqualTo("FCID", db.Document($"FoodCollections/{SelectedCollection.FCIDString}"));
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            if (snapshot.Count > 0)
            {
                IsSaved = true;
            }
        }
    }
}
