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
using CommunityToolkit.Maui.Core.Extensions;

namespace MAUIRecipeApp.ViewModel.UserView
{
    public partial class HomePageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<FoodRecipeType> foodRecipeTypes = new ObservableCollection<FoodRecipeType>();

        [ObservableProperty]
        ObservableCollection<FoodRecipe> foodRecipes = new ObservableCollection<FoodRecipe>();

        [ObservableProperty]
        ObservableCollection<FoodRecipeTypeMapping> foodRecipeTypeMappings = new ObservableCollection<FoodRecipeTypeMapping>();

        [ObservableProperty]
        ObservableCollection<FoodRecipe> paginatedFoodRecipes = new ObservableCollection<FoodRecipe>();

        [ObservableProperty]
        ObservableCollection<FoodRecipe> filteredFoodRecipes = new ObservableCollection<FoodRecipe>();

        [ObservableProperty]
        ObservableCollection<FoodRecipe> suggestedFoodRecipes = new ObservableCollection<FoodRecipe>();

        [ObservableProperty] private string timeString = string.Empty;
        [ObservableProperty] private string userName = string.Empty;
        
        [ObservableProperty] private int totalPages = 1;
        [ObservableProperty] private int itemsPerPage = 6;

        [ObservableProperty]
        private int currentPage = 1;

        private bool _firstLoad = false;

        [ObservableProperty]
        private string selectedFoodTypeId = string.Empty;

        private readonly FirestoreService _firestoreService;
        private readonly FirestoreDb db;
        private readonly GeminiService _geminiService;



        public HomePageViewModel(GeminiService gemini)
        {
            this._geminiService = gemini;
            _firestoreService = FirestoreService.Instance;
            db = _firestoreService.Db;
            if (db == null)
            {
                Debug.WriteLine("Firestore DB is null");
                return;
            }
            LoadItem();
            UpdateTimeString();
            
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

        [RelayCommand]
        public async Task CreateCollection()
		{
			await Shell.Current.GoToAsync("///createcollection");
		}

        [RelayCommand]

        public async Task SetPage()
        {
            try
            {
                if (_firstLoad)
                {
                    if (CurrentPage + 1 > TotalPages)
                    {
                        CurrentPage = 1;
                    }
                    else
                    {
                        CurrentPage++;
                    }
                }
                else
                {
                    _firstLoad = true;
                }
                PaginatedFoodRecipes = FilteredFoodRecipes.Skip((CurrentPage - 1) * itemsPerPage).Take(itemsPerPage).ToObservableCollection();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("error: " + ex.Message);
            }
        
        }

        [RelayCommand]
        public async Task FilterByType(string typeId)
        {
            try
            {
                if (string.IsNullOrEmpty(typeId))
                {
                    FilteredFoodRecipes = foodRecipes;
                }
                else
                {
                    // Lọc danh sách foodRecipes dựa trên sự kết hợp của foodRecipeTypeMappings
                    FilteredFoodRecipes = foodRecipes
                        .Where(x => foodRecipeTypeMappings
                            .Any(mapping => mapping.Frid == x.Frid && mapping.Tofid == typeId))
                        .ToObservableCollection();
                }

                TotalPages = (int)Math.Ceiling((double)filteredFoodRecipes.Count / itemsPerPage);
                await SetPage();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("error: " + ex.Message);
            }
        }

        [RelayCommand]
        public async Task RefreshFoodRecipe()
        {
            await LoadFoodRecipes();
        }
        

        private async void LoadItem()
        {
            userName = UserService.Instance.CurrentUser.Username;
            await LoadFoodRecipes();
            LoadFoodRecipeTypes();
            LoadFoodRecipeTypeMappings();
            filteredFoodRecipes = foodRecipes;
            TotalPages = (int)Math.Ceiling((double)filteredFoodRecipes.Count / itemsPerPage);
            await SetPage();
            LoadSuggestedFood();
        }


        private async void LoadSuggestedFood()
        {
            var result = await FeatureFoodService.Instance.LoadSuggestedFood(_geminiService, foodRecipes);
            SuggestedFoodRecipes = result.ToObservableCollection();
        }

        private async Task LoadFoodRecipes()
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
                        foodRecipeTypes.Add(recipeType);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("error: " + ex.Message);
            }
        }

        private async void LoadFoodRecipeTypeMappings()
        {
            try
            {
                CollectionReference recipeTypeMappingRef = db.Collection("FoodRecipeTypeMappings");
                Query query = recipeTypeMappingRef.WhereEqualTo("IsDeleted", false);
                QuerySnapshot snapshot = await query.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        FoodRecipeTypeMapping recipeType = document.ConvertTo<FoodRecipeTypeMapping>();
                       // recipeType.Tofid = document.Id;
                        FoodRecipeTypeMappings.Add(recipeType);
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
