using Microsoft.Extensions.Configuration;
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
        private readonly ChatSession _generalChatSession;   // Chat session để gửi tin nhắn thông thường
        private readonly ChatSession _recommendationChatSession; // Chat session riêng để gợi ý món ăn

        private List<FoodRecipe> foodRecipes = new List<FoodRecipe>();

        private readonly FirestoreService _firestoreService;
        private readonly FirestoreDb _db;

        public GeminiService(IConfiguration configuration)
        {
            // Lấy giá trị từ appsettings.json
            _geminiKey = configuration["GeminiKey"];

            // Tạo hai phiên chat riêng biệt
            var model = new GenerativeModel(_geminiKey);
            _generalChatSession = model.StartChat(new StartChatParams());
            _recommendationChatSession = model.StartChat(new StartChatParams());

            _firestoreService = FirestoreService.Instance;
            _db = _firestoreService.Db;

            LoadData();
        }

        public async Task<string> SendGeneralMessage(string prompt)
        {
            var message = prompt.Trim();
            var result = await _generalChatSession.SendMessageAsync(message);
            return result;
        }

        public async Task<List<string>> GetRecommendedFoodForHealth(string healthInfo)
        {
            // Tạo prompt cho phiên gợi ý món ăn
            var prompt = $"Dưới đây là danh sách các món ăn và thông tin chi tiết về chúng. Dựa trên thông tin sức khỏe sau của tôi, hãy lấy ra các món ăn phù hợp và trả về danh sách các FoodId phù hợp dưới dạng mảng: [idfood1, idfood2, idfood3,...]\n\n";
            prompt += $"Thông tin sức khỏe của tôi: {healthInfo}\n\n";

            var id = 0;
            foreach (var recipe in foodRecipes)
            {
                id++;
                prompt += $"id món: {id}  - Tên món: {recipe.RecipeName}, Lượng calo: {recipe.Calories}, Lợi ích sức khỏe: {recipe.HealthBenefits}, Khẩu phần: {recipe.Portion}\n";
            }

            prompt += "\nXin vui lòng chỉ trả về danh sách các FoodId dưới dạng: [idfood1, idfood2, idfood3,...]. Không thêm bất kỳ thông tin gì khác.";

            Debug.WriteLine(prompt);
            // Gửi prompt tới phiên gợi ý món ăn
            var result = await _recommendationChatSession.SendMessageAsync(prompt);
            Debug.WriteLine(result);

            return ParseFoodIds(result);
        }

        private async Task LoadRecipe()
        {
            CollectionReference recipesRef = _db.Collection("FoodRecipes");
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

        private async void LoadData()
        {
            await LoadRecipe();
           // await LoadDataToChat();
        }

        public async Task LoadDataToChat()
        {
            var prompt = "Dưới đây là danh sách các món ăn và thông tin chi tiết về chúng. Nhiệm vụ của bạn là chỉ trả lời các câu hỏi hoặc thực hiện các yêu cầu của tôi theo thông tin đã cung cấp. Bạn không được tự tạo câu hỏi hoặc đưa ra câu trả lời ngoài phạm vi yêu cầu của tôi.\n\n";
            prompt += "Các loại yêu cầu bạn có thể nhận:\n";
            prompt += "- Tạo thực đơn cho 1 ngày hoặc 1 tuần.\n";
            prompt += "- Gợi ý món ăn phù hợp với tình trạng sức khỏe của tôi, ví dụ: \"Tôi bị tiểu đường thì nên ăn món gì?\" hoặc \"Tôi muốn ăn món ít calo phù hợp với chế độ giảm cân.\"\n";
            prompt += "- Trả lời các câu hỏi liên quan đến lợi ích sức khỏe hoặc thông tin về món ăn.\n\n";
            prompt += "Dưới đây là thông tin chi tiết về các món ăn:\n\n";

            foreach (var recipe in foodRecipes)
            {
                prompt += $"- Tên món: {recipe.RecipeName}, Lượng calo: {recipe.Calories}, Lợi ích sức khỏe: {recipe.HealthBenefits}, Khẩu phần: {recipe.Portion}\n";
            }

            prompt += "\nHãy chỉ trả lời khi tôi đưa ra câu hỏi hoặc yêu cầu. Không cung cấp thông tin ngoài yêu cầu.";

            var result = await _generalChatSession.SendMessageAsync(prompt);
            Debug.WriteLine(result);
        }

        private List<string> ParseFoodIds(string response)
        {
            try
            {
                var foodIds = new List<string>();

                if (string.IsNullOrEmpty(response))
                    return foodIds;

                var foodIdString = response.Trim('[', ']');
                var foodIdList = foodIdString.Split(',');

                foreach (var foodId in foodIdList)
                {
                    var id = int.Parse(foodId.Trim());
                    if (id - 1 < foodRecipes.Count)
                    {
                        foodIds.Add(foodRecipes[id - 1].Frid);
                    }
                }

                return foodIds;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error parsing food IDs: " + e.Message);
                return new List<string>();
            }
        }
    }
}
