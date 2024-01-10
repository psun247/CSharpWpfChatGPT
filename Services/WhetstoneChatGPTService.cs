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
                Model = ChatGPT35Models.Turbo,
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

        // GPT-3, deprecated on 2024-01-04
        //public async Task<ChatGPTCompletionResponse?> GetResponseDataAsync(string prompt, CancellationToken cancellationToken)
        //{
        //    var gptRequest = new ChatGPTCompletionRequest
        //    {
        //        Model = ChatGPT35Models.Davinci003,
        //        Prompt = prompt,
        //        Temperature = 0.5f,
        //        MaxTokens = 500,
        //    };
        //    return await _chatGPTClient.CreateCompletionAsync(gptRequest, cancellationToken);
        //}

        // After 2024-01-04, must use GPT-3.5 with ChatGPT35Models.Turbo
        public IAsyncEnumerable<ChatGPTChatCompletionStreamResponse?> StreamChatCompletionAsync(string prompt)
        {
            var completionRequest = new ChatGPTChatCompletionRequest
            {
                Model = ChatGPT35Models.Turbo,
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

        // GPT-3, deprecated on 2024-01-04
        //public IAsyncEnumerable<ChatGPTCompletionStreamResponse?> StreamCompletionAsync(string prompt, CancellationToken cancellationToken)
        //{
        //    var completionRequest = new ChatGPTCompletionRequest
        //    {
        //        Model = ChatGPT35Models.Davinci003,
        //        Prompt = prompt,
        //        Temperature = 1.0f,
        //        MaxTokens = 500,
        //        TopP = 0.3f,
        //        FrequencyPenalty = 0.5f,
        //        PresencePenalty = 0
        //    };
        //    return _chatGPTClient.StreamCompletionAsync(completionRequest, cancellationToken);
        //}

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
