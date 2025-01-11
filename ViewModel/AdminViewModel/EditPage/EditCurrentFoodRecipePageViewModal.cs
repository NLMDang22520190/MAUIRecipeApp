using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace MAUIRecipeApp.ViewModel.AdminViewModel.EditPage
{
    [QueryProperty(nameof(SelectedFoodRecipeId), "FRID")]
    public partial class EditCurrentFoodRecipePageViewModal: ObservableObject
    {
        [ObservableProperty]
        private string selectedFoodRecipeId;

        [ObservableProperty]
        private FoodRecipe selectedFoodRecipe = new FoodRecipe();

        [ObservableProperty]
        private string recipeName;

        [ObservableProperty] private int cookingTime;

        [ObservableProperty] private int calories;

        [ObservableProperty] private string difficultyLevel;

        [ObservableProperty] private string imgUrl;

        [ObservableProperty] private string healthBenefits;

        [ObservableProperty] private int portion;

        [ObservableProperty] private string videoUrl;

        [ObservableProperty] private bool isHidden;

        [ObservableProperty] private bool isApproved;

        [ObservableProperty] private string updateMsg;

        [ObservableProperty] private bool canUpdate = true;

        private FirestoreDb _db;

        public EditCurrentFoodRecipePageViewModal()
        {

        }

        [RelayCommand]
        public async Task Back()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        public async Task Update()
        {
            canUpdate = false;
            var result = await FirestoreService.Instance.UpdateDocumentAsync("FoodRecipes",
                selectedFoodRecipeId,
                new Dictionary<string, object>
                {
                    { "Calories", Calories },
                    { "CookingTime", CookingTime },
                    { "DifficultyLevel", DifficultyLevel },
                    { "ImgUrl", ImgUrl },
                    { "Portion", Portion },
                    {"HealthBenefits", HealthBenefits},
                    { "RecipeName", RecipeName },
                    { "VideoUrl", VideoUrl },
                    { "IsApproved", IsApproved },
                    { "isDeactivated", IsHidden }
                });
            if (result)
            {
                UpdateMsg = "Update successfully!";
            }
            else
            {
                UpdateMsg = "Update failed!";
            }


            canUpdate = true;
        }


        public void OnAppearing()
        {
            _db = FirestoreService.Instance.Db;
            LoadItem();
        }

        private void LoadItem()
        {
            LoadSelectedFoodRecipe();
        }

        private void LoadSelectedFoodRecipe()
        {
            try
            {
                CollectionReference usersRef = _db.Collection("FoodRecipes");
                DocumentReference docRef = usersRef.Document(selectedFoodRecipeId);
                DocumentSnapshot snapshot = docRef.GetSnapshotAsync().Result;
                if (snapshot.Exists)
                {
                    SelectedFoodRecipe = snapshot.ConvertTo<FoodRecipe>();
                }
                else
                {
                    SelectedFoodRecipe = null;
                }

                if (SelectedFoodRecipe != null)
                {
                    RecipeName = SelectedFoodRecipe.RecipeName;
                    CookingTime = SelectedFoodRecipe.CookingTime != null? (int)SelectedFoodRecipe.CookingTime : 0;
                    Calories = SelectedFoodRecipe.Calories != null ? (int)SelectedFoodRecipe.Calories : 0;
                    DifficultyLevel = SelectedFoodRecipe.DifficultyLevel;
                    ImgUrl = SelectedFoodRecipe.ImgUrl;
                    HealthBenefits = SelectedFoodRecipe.HealthBenefits;
                    Portion = SelectedFoodRecipe.Portion != null ? (int)SelectedFoodRecipe.Portion : 0;
                    VideoUrl = SelectedFoodRecipe.VideoUrl;
                    IsHidden = SelectedFoodRecipe.IsDeleted != null ? (bool)SelectedFoodRecipe.IsDeleted : false;
                    IsApproved = SelectedFoodRecipe.IsApproved != null ? (bool)SelectedFoodRecipe.IsApproved : false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("error: " + e.Message);
            }
        }

    }
}
