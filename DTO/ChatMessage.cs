using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.DTO
{
    public class ChatMessage
    {
        public string Text { get; set; }
        public bool IsUserMessage { get; set; } // true nếu là tin nhắn người dùng, false nếu là tin nhắn chatbot
    }
}
