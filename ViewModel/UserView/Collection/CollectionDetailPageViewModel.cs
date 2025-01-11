using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.DTO;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;

namespace MAUIRecipeApp.ViewModel.UserView.Collection
{
    [QueryProperty(nameof(SelectedCollection), "SelectedCollection")]
    public partial class CollectionDetailPageViewModel: ObservableObject
    {
        [ObservableProperty] private UserSavedCollectionDTO selectedCollection;

        [ObservableProperty] private ObservableCollection<FoodRecipe> foodRecipes = new ObservableCollection<FoodRecipe>();

        [ObservableProperty] private string collectionName = string.Empty;

        private FirestoreDb db = FirestoreService.Instance.Db;

        public CollectionDetailPageViewModel()
        {
            // LoadData();
        }

        [RelayCommand]
        public async Task Back()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        public async Task FoodDetail(string Frid)
        {
            await Shell.Current.GoToAsync($"collectiondetail/fooddetail?FRID={Frid}");
        }

        [RelayCommand]

        public async Task UpdateName(string newName)
        {
            DocumentReference docRef = db.Collection("UserSavedFoodCollection").Document(selectedCollection.UserSavedCollectionId);
            await docRef.UpdateAsync("AlternateName", newName);
            CollectionName = newName;
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
            // Load Food Recipes
            FoodRecipes.Clear();
            LoadFoodRecipes();
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

    }
}
