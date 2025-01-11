using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAUIRecipeApp.Models;

namespace MAUIRecipeApp.Service
{
    public class FeatureFoodService
    {
        
        private static FeatureFoodService _instance;  // Dùng để lưu trữ instance duy nhất của AuthService
        private static readonly object _lock = new object(); // Lock object để đảm bảo thread safety

        private List<FoodRecipe> suggestedFoodRecipes = new List<FoodRecipe>();

        private readonly FirestoreDb _db;

        public FeatureFoodService()
        {
            _db = FirestoreService.Instance.Db;
        }

        public async Task<List<FoodRecipe>> LoadSuggestedFood(GeminiService gemini, ObservableCollection<FoodRecipe> foodRecipes)
        {
            if(suggestedFoodRecipes.Count > 0)
            {
                return suggestedFoodRecipes;
            }

            var currentUser = UserService.Instance.CurrentUser;
            var healthInfo = "Tôi có tình trạng sức khoẻ là " + currentUser.HealthCondition + " và bị dị ứng với " + currentUser.Allergies +
                             ", cân nặng " + currentUser.Weight.ToString() + ", chiều cao " + currentUser.Height.ToString();
            var recommendedFoodIds = await gemini.GetRecommendedFoodForHealth(healthInfo);

            // In ra danh sách các FoodId phù hợp
            foreach (var foodId in recommendedFoodIds)
            {
                Debug.WriteLine(foodId);
            }

            // Lấy ra các món ăn phù hợp từ danh sách FoodId
            foreach (var foodId in recommendedFoodIds)
            {
                var food = foodRecipes.FirstOrDefault(x => x.Frid == foodId);
                if (food != null)
                {
                    suggestedFoodRecipes.Add(food);
                }
            }
            return suggestedFoodRecipes;
        }

        public void ClearSuggestedFood()
        {
            suggestedFoodRecipes.Clear();
        }


        public static FeatureFoodService Instance
        {
            get
            {
                // Kiểm tra xem instance đã được tạo chưa, nếu chưa thì tạo mới
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new FeatureFoodService();
                    }
                    return _instance;
                }
            }
        }
    }
}
