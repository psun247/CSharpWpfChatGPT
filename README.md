# Project Note
To see the latest ChatGPT with GPT-3.5 model turbo and WPF Shazam, check out my ShazamDesk project: 
https://github.com/psun247/ShazamDesk

# C# WPF ChatGPT
C# WPF app that mimics ChatGPT website UI

![image](https://github.com/psun247/CSharpWpfChatGPT/assets/31531761/6a73a4e9-a29d-450d-9ef0-d1151651a4bb)

![image](https://github.com/psun247/CSharpWpfChatGPT/assets/31531761/97370b00-367e-4882-a52b-dd4beaa631b4)

# Setup
Create an Open AI account to obtain an API key:
https://platform.openai.com/account/api-keys

You can use the key as a command line parameter (without compiling the project):
1. Click CSharpWpfChatGPT_v1.2 under Releases on the right side of this page
2. Download CSharpWpfChatGPT_v1.2_net6.0-windows.zip
3. Unzip the file and run CSharpWpfChatGPT.exe /the key obtained above (CSharpWpfChatGPT.exe /sk-Ih...WPd)

Or in App.xaml.cs, modify this line:
return "<Your Open AI API Key is something like \"sk-Ih...WPd\">";

Build CSharpWpfChatGPT.sln with Visual Studio 2022.  This app is targeted for .NET 6 and 8.  If you don't have .NET 8 installed, remove net8.0-windows in CSharpWpfChatGPT.csproj.

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
