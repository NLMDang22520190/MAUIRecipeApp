using Microsoft.Extensions.Configuration;
using GenerativeAI.Methods;
using GenerativeAI.Models;
using GenerativeAI.Types;

namespace MAUIRecipeApp.Service
{
    public class GeminiService
    {
        private readonly string _geminiKey;
        private readonly ChatSession chatSession;

        public GeminiService(IConfiguration configuration)
        {
            // Lấy giá trị từ appsettings.json
            _geminiKey = configuration["GeminiKey"];
            var model = new GenerativeModel(_geminiKey);
            chatSession = model.StartChat(new StartChatParams());
        }

        public async Task<string> SendMessage(string prompt)
        {
            var message = prompt.Trim();
            var result = await chatSession.SendMessageAsync(message);

            return result;
        }
    }
}