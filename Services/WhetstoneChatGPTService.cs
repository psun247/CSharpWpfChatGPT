using System.Threading;
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

        public async Task<ChatGPTCompletionResponse?> GetResponseDataAsync(string prompt, CancellationToken cancellationToken)
        {
            var gptRequest = new ChatGPTCompletionRequest
            {
                Model = ChatGPTCompletionModels.Davinci,
                Prompt = prompt,
                Temperature = 0.5f,
                MaxTokens = 500,
            };
            return await _chatGPTClient.CreateCompletionAsync(gptRequest, cancellationToken);
        }

        public IAsyncEnumerable<ChatGPTCompletionResponse?> StreamCompletionAsync(string prompt, CancellationToken cancellationToken)
        {
            var completionRequest = new ChatGPTCompletionRequest
            {
                Model = ChatGPTCompletionModels.Davinci,
                Prompt = prompt,
                Temperature = 1.0f,
                MaxTokens = 500,
                TopP = 0.3f,
                FrequencyPenalty = 0.5f,
                PresencePenalty = 0
            };

            return _chatGPTClient.StreamCompletionAsync(completionRequest, cancellationToken);
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
