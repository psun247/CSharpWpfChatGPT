# C# WPF ChatGPT
C# WPF app that mimics ChatGPT website UI

![image](https://github.com/psun247/CSharpWpfChatGPT/assets/31531761/6de00aa6-6beb-4a8e-8108-84daa13b941b)

![image](https://github.com/psun247/CSharpWpfChatGPT/assets/31531761/107506c4-7c82-4b2f-b2f0-dd5b1e26e0bf)

# Setup
Create an Open AI account to obtain an API key:
https://platform.openai.com/account/api-keys

You can use the key as a command line parameter (without compiling the project):
1. Click CSharpWpfChatGPT_v1.1 under Releases on the right side of this page
2. Download CSharpWpfChatGPT_v1.1_net6.0-windows.zip
3. Unzip the file and run CSharpWpfChatGPT.exe /<the key obtained above>

Or in App.xaml.cs, modify this line:
return "<Your Open AI API Key is something like \"sk-Ih...WPd\">";

Build CSharpWpfChatGPT.sln with Visual Studio 2022.  This app is targeted for .NET 6 and 7.

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
