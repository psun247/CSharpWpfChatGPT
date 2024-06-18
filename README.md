# Please read my project article at codeproject.com
https://www.codeproject.com/Tips/5377103/ChatGPT-API-in-Csharp-WPF-XAML-MVVM

# Feel free to explore my other repositories on GitHub!
https://github.com/psun247

# ChatGPT API in C# WPF / XAML / MVVM
C# WPF app that communicates with OpenAI GPT-3.5 Turbo API (added History tab and SQL Server support on 2024/06/17)

![Screenshot1](https://github.com/psun247/CSharpWpfChatGPT/assets/31531761/93cd7b42-2b38-492c-9659-e07e5d6b0a13)

![Screenshot2](https://github.com/psun247/CSharpWpfChatGPT/assets/31531761/58e7d9e0-3984-4806-88c9-8a51a3d0118e)

![Screenshot3](https://github.com/psun247/CSharpWpfChatGPT/assets/31531761/6ba94def-2aa5-4168-a293-eeb493e2a529)

# Setup
Create an Open AI account to obtain an API key (free):
https://platform.openai.com/account/api-keys

You can use the key as a command line parameter (without compiling the project):
1. Click CSharpWpfChatGPT_v1.3 under Releases on the right side of this page
2. Download CSharpWpfChatGPT_v1.3_net6.0-windows.zip
3. Unzip the file and run CSharpWpfChatGPT.exe /the key obtained above (CSharpWpfChatGPT.exe /sk-Ih...WPd)

Or in App.xaml.cs, modify this:
"<Your Open AI API Key is something like \"sk-Ih...WPd\">";

Build CSharpWpfChatGPT.sln with Visual Studio 2022 (Community version okay).  This app is targeted for .NET 6 and 8.  If you don't have .NET 8 installed, remove net8.0-windows in CSharpWpfChatGPT.csproj.

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
