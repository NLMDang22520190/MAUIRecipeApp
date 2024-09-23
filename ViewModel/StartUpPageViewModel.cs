using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using MAUIRecipeApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace MAUIRecipeApp.ViewModel
{
    public partial class StartUpPageViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task Login()
        {


            // Tải tệp JSON
            try
            {
                var pathToServiceAccountKey = "C:\\Users\\TEKATOJI\\source\\repos\\NLMDang22520190\\MAUIRecipeApp\\recipeapp-3c612-firebase-adminsdk-blmv2-aabcd1703d.json";

                var credential = GoogleCredential.FromFile(pathToServiceAccountKey);
                FirestoreDb db = FirestoreDb.Create("recipeapp-3c612", new FirestoreClientBuilder { ChannelCredentials = credential.ToChannelCredentials() }.Build());

                List<FoodRecipe> foodRecipes = new List<FoodRecipe>
{
    new FoodRecipe { RecipeName = "Spaghetti Bolognese", Calories = 600, DifficultyLevel = "Medium", HealthBenefits = "Rich in protein", CookingTime = 45, Portion = 4, UploaderUid = 6 },
    new FoodRecipe { RecipeName = "Caesar Salad", Calories = 300, DifficultyLevel = "Easy", HealthBenefits = "Low in calories", CookingTime = 20, Portion = 2, UploaderUid = 6 },
    new FoodRecipe { RecipeName = "Beef Stroganoff", Calories = 700, DifficultyLevel = "Hard", HealthBenefits = "High in protein", CookingTime = 60, Portion = 4, UploaderUid = 6 },
    new FoodRecipe { RecipeName = "Vegan Burger", Calories = 450, DifficultyLevel = "Medium", HealthBenefits = "Rich in fiber", CookingTime = 30, Portion = 3, UploaderUid = 6 },
    new FoodRecipe { RecipeName = "Chicken Curry", Calories = 550, DifficultyLevel = "Medium", HealthBenefits = "Rich in antioxidants", CookingTime = 50, Portion = 5, UploaderUid = 6 },
    new FoodRecipe { RecipeName = "Pancakes", Calories = 350, DifficultyLevel = "Easy", HealthBenefits = "Low in fat", CookingTime = 25, Portion = 4, UploaderUid = 6 },
    new FoodRecipe { RecipeName = "Grilled Salmon", Calories = 400, DifficultyLevel = "Easy", HealthBenefits = "Good source of omega-3", CookingTime = 35, Portion = 2, UploaderUid = 6 },
    new FoodRecipe { RecipeName = "Lentil Soup", Calories = 250, DifficultyLevel = "Easy", HealthBenefits = "High in fiber", CookingTime = 40, Portion = 6, UploaderUid = 6 },
    new FoodRecipe { RecipeName = "Chocolate Cake", Calories = 800, DifficultyLevel = "Hard", HealthBenefits = "Energy booster", CookingTime = 90, Portion = 8, UploaderUid = 6 },
    new FoodRecipe { RecipeName = "Vegetable Stir-fry", Calories = 300, DifficultyLevel = "Easy", HealthBenefits = "Low in fat", CookingTime = 20, Portion = 2, UploaderUid = 6 }
};


                DocumentReference foodRecipesCollection = db.Collection("FoodRecipes").Document("eyeKzS1gGXQJ9OmvTYaK");
                DocumentSnapshot snapshot = await foodRecipesCollection.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Debug.WriteLine($"Document {snapshot.Id} data: {snapshot.ToDictionary()}");
                }
                else
                {
                    Debug.WriteLine("Document does not exist.");
                }
                Debug.WriteLine("Document data for {0} document:", snapshot.Id);
                //if (snapshot.Exists)
                //{
                //    Debug.WriteLine("Document data for {0} document:", snapshot.Id);
                //    FoodRecipe city = snapshot.ConvertTo<FoodRecipe>();
                //    Debug.WriteLine("Name: {0}", city.RecipeName);
                //    Debug.WriteLine("State: {0}", city.Calories);
                   
                //}

                //foreach (var recipe in foodRecipes)
                //{
                //    Debug.WriteLine("1vaicaloz");
                //    await db.Collection("FoodRecipes").AddAsync(recipe);
                    
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n Error: {ex.Message}");
            }

            await Shell.Current.GoToAsync("//login");
        }

        [RelayCommand]

        private async Task SignUp()
        {
            await Shell.Current.GoToAsync("//signup");
        }
    }
}
