using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Cloud.Firestore;
using MAUIRecipeApp.Models;
using MAUIRecipeApp.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel.UserView
{
    public partial class HomePageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<FoodRecipeType> foodRecipeTypes = new ObservableCollection<FoodRecipeType>();

        [ObservableProperty]
        ObservableCollection<FoodRecipe> foodRecipes = new ObservableCollection<FoodRecipe>();

        [ObservableProperty] private string timeString;
        [ObservableProperty] private string userName;

        private readonly FirestoreService _firestoreService;
        private readonly FirestoreDb db;

        public HomePageViewModel(FirestoreService firestoreService)
        {
            _firestoreService = firestoreService;
            db = _firestoreService.Db;
            if (db == null)
            {
                Debug.WriteLine("Firestore DB is null");
                return;
            }
            LoadItem();
            UpdateTimeString();
            userName = UserService.Instance.CurrentUser.Username;
        }

        [RelayCommand]
        public async Task FoodDetail(string Frid)
        {
            await Shell.Current.GoToAsync($"fooddetail?FRID={Frid}");
        }

        [RelayCommand]
        public async Task SubmitNewRecipe()
        {
            await Shell.Current.GoToAsync("///submitnewrecipe");
        }

        private void LoadItem()
        {
            LoadFoodRecipes();
            LoadFoodRecipeTypes();
        }

        private async void LoadFoodRecipes()
        {
            CollectionReference recipesRef = db.Collection("FoodRecipes");
            Query query = recipesRef.WhereEqualTo("IsDeleted", false);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    FoodRecipe recipe = document.ConvertTo<FoodRecipe>();
                    recipe.Frid = document.Id;
                    FoodRecipes.Add(recipe);
                }
            }
        }

        private async void LoadFoodRecipeTypes()
        {
            try
            {
                CollectionReference recipeTypesRef = db.Collection("FoodRecipeTypes");
                Query query = recipeTypesRef.WhereEqualTo("IsDeleted", false);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        FoodRecipeType recipeType = document.ConvertTo<FoodRecipeType>();
                        recipeType.Tofid = document.Id;
                        FoodRecipeTypes.Add(recipeType);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("error: " + ex.Message);
            }
        }

        private void UpdateTimeString()
        {
            var currentHour = DateTime.Now.Hour;

            if (currentHour >= 5 && currentHour < 12)
            {
                TimeString = "Good Morning!";
            }
            else if (currentHour >= 12 && currentHour < 17)
            {
                TimeString = "Good Afternoon!";
            }
            else if (currentHour >= 17 && currentHour < 21)
            {
                TimeString = "Good Evening!";
            }
            else
            {
                TimeString = "Good Night!";
            }
        }
    }
}
