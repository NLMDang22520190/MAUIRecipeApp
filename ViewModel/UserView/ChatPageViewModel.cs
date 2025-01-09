using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenAI;
using System.ClientModel;
using CommunityToolkit.Mvvm.Input;
using MAUIRecipeApp.Service;
using Environment = System.Environment;
using System.Collections.ObjectModel;
using MAUIRecipeApp.DTO;

namespace MAUIRecipeApp.ViewModel.UserView
{
    public partial class ChatPageViewModel : ObservableObject
    {
        [ObservableProperty]
        string message;

        [ObservableProperty] string answer;

        [ObservableProperty] List<string> answerHistory = new List<string>();

        [ObservableProperty] List<string> messageHistory = new List<string>();

        [ObservableProperty] ObservableCollection<ChatMessage> chatHistory = new ObservableCollection<ChatMessage>();

      

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
                if(string.IsNullOrEmpty(message))
                {
                    return;
                }
                AddToChatHistory(message, true);
                Answer = "Waiting";
                AddToChatHistory(answer, false);
                var result = await _geminiService.SendMessage(message);
                Answer = "Chat response: ";
                Answer += result.ToString();
                ChangeChatHistory(result);

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

        private void MergeHistories()
        {
            ChatHistory.Clear();
            int maxCount = Math.Max(messageHistory.Count, answerHistory.Count);

            for (int i = 0; i < maxCount; i++)
            {
                if (i < messageHistory.Count)
                {
                    ChatHistory.Add(new ChatMessage { Text = messageHistory[i], IsUserMessage = true });
                }
                if (i < answerHistory.Count)
                {
                    ChatHistory.Add(new ChatMessage { Text = answerHistory[i], IsUserMessage = false });
                }
            }
        }

        private void AddToChatHistory(string msg, bool isUserMessage)
        {
            ChatHistory.Add(new ChatMessage { Text = msg, IsUserMessage = isUserMessage});
        }

        private void ChangeChatHistory(string msg)
        {
           ChatHistory.RemoveAt(chatHistory.Count-1);
           AddToChatHistory(msg, false);
        }


    }


}
