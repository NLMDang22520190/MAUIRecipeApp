﻿using Microsoft.Extensions.Configuration;
using GenerativeAI.Methods;
using GenerativeAI.Models;
using GenerativeAI.Types;
using MAUIRecipeApp.Models;
using Google.Cloud.Firestore;
using System.Diagnostics;

namespace MAUIRecipeApp.Service
{
    public class GeminiService
    {
        private readonly string _geminiKey;
        private readonly ChatSession chatSession;

        private List<FoodRecipe> foodRecipes = new List<FoodRecipe>();

        private readonly FirestoreService _firestoreService;
        private readonly FirestoreDb db;

        public GeminiService(IConfiguration configuration)
        {
            // Lấy giá trị từ appsettings.json
            _geminiKey = configuration["GeminiKey"];
            var model = new GenerativeModel(_geminiKey);
            chatSession = model.StartChat(new StartChatParams());

            _firestoreService = FirestoreService.Instance;
            db = _firestoreService.Db;

            LoadData();
        }

        public async Task<string> SendMessage(string prompt)
        {
            var message = prompt.Trim();
            var result = await chatSession.SendMessageAsync(message);

            return result;
        }

        private async void LoadData()
        {
            await LoadRecipe();
            //await LoadDataToChat();

        }

        public async Task LoadDataToChat()
        {
            var prompt = "Dưới đây là danh sách các món ăn và thông tin chi tiết về chúng. Dựa vào thông tin này, bạn sẽ trả lời các câu hỏi tiếp theo của tôi. Hãy chỉ trả lời khi tôi đặt câu hỏi, không cần tự tạo câu hỏi hoặc trả lời thêm bất cứ điều gì ngoài câu hỏi của tôi:\n\n";

            foreach (var recipe in foodRecipes)
            {
                prompt += $"- Tên món: {recipe.RecipeName}, Lượng calo: {recipe.Calories}, Lợi ích sức khỏe: {recipe.HealthBenefits}, Khẩu phần: {recipe.Portion}\n";
            }

            var result = await chatSession.SendMessageAsync(prompt);
            Debug.WriteLine(result);
        }


        private async Task LoadRecipe()
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
                    foodRecipes.Add(recipe);
                }
            }
        }

        public async Task<List<string>> GetRecommendedFoodForHealth(string healthInfo)
        {
            // Tạo prompt yêu cầu AI trả lời theo định dạng danh sách ID món ăn
            var prompt = $"Dưới đây là danh sách các món ăn và thông tin chi tiết về chúng. Dựa trên thông tin sức khỏe sau của tôi, hãy lấy ra các món ăn phù hợp và trả về danh sách các FoodId phù hợp dưới dạng mảng: [idfood1, idfood2, idfood3,...]\n\n";
            prompt += $"Thông tin sức khỏe của tôi: {healthInfo}\n\n";

            foreach (var recipe in foodRecipes)
            {
                prompt += $" - Tên món: {recipe.RecipeName}, Lượng calo: {recipe.Calories}, Lợi ích sức khỏe: {recipe.HealthBenefits}, Khẩu phần: {recipe.Portion}\n";
            }

            // Yêu cầu chatbot trả về mảng ID món ăn theo định dạng: [idfood1, idfood2, idfood3,...]
            prompt += "\nXin vui lòng chỉ trả về danh sách các FoodId dưới dạng: [idfood1, idfood2, idfood3,...]. Không thêm bất kỳ thông tin gì khác.";

            Debug.WriteLine(prompt);
            // Gửi prompt tới AI và nhận phản hồi
            var result = await chatSession.SendMessageAsync(prompt);
            Debug.WriteLine(result);

            // Phân tích kết quả trả về từ AI, giả sử AI trả về một danh sách các FoodId trong format như: [idfood1, idfood2, idfood3,...]
            var foodIds = ParseFoodIds(result);

            return foodIds;
        }

        private List<string> ParseFoodIds(string response)
        {
            try
            {
                var foodIds = new List<string>();

                // Giả sử response trả về dạng: "[idfood1, idfood2, idfood3,...]"
                // Cắt bỏ dấu ngoặc và chia thành các phần tử
                if (string.IsNullOrEmpty(response))
                {
                    return foodIds;
                }

                // Loại bỏ dấu ngoặc vuông ở hai đầu chuỗi
                var foodIdString = response.Trim('[', ']');
                // Tách chuỗi thành các phần tử (các FoodId)
                var foodIdList = foodIdString.Split(',');

                foreach (var foodId in foodIdList)
                {
                    var Id = (int.Parse(foodId.Trim()));
                    if (Id - 1 <= foodRecipes.Count)
                    {
                        Debug.WriteLine(Id.ToString());
                        foodIds.Add(foodRecipes[int.Parse(foodId.Trim()) - 1].Frid);
                    }
                }

                return foodIds;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
            return new List<string>();

            }

        }


    }
}
