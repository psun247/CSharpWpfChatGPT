using System.Threading.Tasks;
using System.Collections.Generic;
using Whetstone.ChatGPT.Models;
using Whetstone.ChatGPT;

namespace CSharpWpfChatGPT.Services
{
    public class WhetstoneChatGPTService
    {
        private ChatGPTClient _chatGPTClient;

        public WhetstoneChatGPTService(string openaiApiKey)
        {
            _chatGPTClient = new ChatGPTClient(openaiApiKey);
        }
       
        // After 2024-01-04, must use GPT-3.5 with ChatGPT35Models.Turbo because Davinci003 (etc) is deprecated.
        // https://platform.openai.com/docs/deprecations/deprecation-history
        public async Task<ChatGPTChatCompletionResponse?> CreateChatCompletionAsync(string prompt)
        {
            var gptRequest = new ChatGPTChatCompletionRequest
            {
                Model = ChatGPT4Models.GPT4, // ChatGPT35Models.Turbo,
                Messages = new List<ChatGPTChatCompletionMessage>()
                {
                    new ChatGPTChatCompletionMessage()
                    {
                        Role = ChatGPTMessageRoles.System,
                        Content = "You are a helpful assistant."
                    },
                    new ChatGPTChatCompletionMessage()
                    {
                        Role = ChatGPTMessageRoles.User,
                        Content = prompt
                    },
                },
                Temperature = 0.9f,
                MaxTokens = 500,
            };
            return await _chatGPTClient.CreateChatCompletionAsync(gptRequest);
        }       

        // After 2024-01-04, must use GPT-3.5 with ChatGPT35Models.Turbo
        public IAsyncEnumerable<ChatGPTChatCompletionStreamResponse?> StreamChatCompletionAsync(string prompt)
        {
            var completionRequest = new ChatGPTChatCompletionRequest
            {
                Model = ChatGPT4Models.GPT4, //ChatGPT35Models.Turbo,
                Messages = new List<ChatGPTChatCompletionMessage>()
                {
                    new ChatGPTChatCompletionMessage()
                    {
                        Role = ChatGPTMessageRoles.System,
                        Content = "You are a helpful assistant."
                    },
                    new ChatGPTChatCompletionMessage()
                    {
                        Role = ChatGPTMessageRoles.User,
                        Content = prompt
                    },
                },
                Temperature = 0.9f,
                MaxTokens = 500,
            };
            return _chatGPTClient.StreamChatCompletionAsync(completionRequest);
        }        

        public async Task<byte[]?> CreateImageAsync(string prompt)
        {
            ChatGPTCreateImageRequest imageRequest = new()
            {
                Prompt = prompt,
                Size = CreatedImageSize.Size1024,
                ResponseFormat = CreatedImageFormat.Base64
            };

            byte[]? imageBytes = null;
            ChatGPTImageResponse? imageResponse = await _chatGPTClient.CreateImageAsync(imageRequest);
            if (imageResponse != null)
            {
                var imageData = imageResponse.Data?[0];
                if (imageData != null)
                {
                    imageBytes = await _chatGPTClient.DownloadImageAsync(imageData);
                }
            }
            return imageBytes;
        }
    }
}