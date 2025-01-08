using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using CommunityToolkit.Mvvm.Input;
using MAUIRecipeApp.Service;
using Environment = System.Environment;

namespace MAUIRecipeApp.ViewModel.UserView
{
    public partial class ChatPageViewModel : ObservableObject
    {
        [ObservableProperty]
        string message;

        [ObservableProperty] string answer;

        private readonly GeminiService _geminiService;


        public ChatPageViewModel(GeminiService gemini)
        {
           this._geminiService = gemini;
        }

        [RelayCommand]
        public async Task GetRecommendationAsync()
        {
            try
            {
                Answer = "Waiting";
                var result = await _geminiService.SendMessage(message);
                Answer = "Chat response: ";
                Answer += result.ToString();

                //var client = _chatGptClient.GetChatClient("gpt-3.5-turbo");
                //string prompt = $"{message}";

                //var response = await client.CompleteChatAsync(prompt);
                //System.Diagnostics.Debug.WriteLine(response);
                //var returnMessage = response.Value.Content.ToString();

                //answer = returnMessage ?? "No recommendation available.";
            }
            catch (Exception ex)
            {
                answer = "Error: " + ex.Message;
            }

        }

    }
}
