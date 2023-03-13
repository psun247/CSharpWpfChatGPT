# C# WPF ChatGPT
C# WPF app that mimics ChatGPT website UI

# Setup
Create an Open AI account to obtain an API key:
https://platform.openai.com/account/api-keys

In App.xaml.cs, modify this line:

string openaiApiKey = "<Your Open AI API Key>" // Use your API key here (something like "sk-Ih...WPd")

Build CSharpWpfChatGPT.sln with Visual Studio 2022.  This app is targeted for .NET 6.

# Whetstone.ChatGPT
I used John Iwasz's excellent Whetstone.ChatGPT to do the heavy lifting for ChatGPT API calls.

https://www.nuget.org/packages/Whetstone.ChatGPT

# Other Supporting Libraries
CommunityToolkit.Mvvm
 
https://www.nuget.org/packages/CommunityToolkit.Mvvm
 
ModernWpfUI
 
https://www.nuget.org/packages/ModernWpfUI/
 
RestoreWindowPlace

https://www.nuget.org/packages/RestoreWindowPlace
