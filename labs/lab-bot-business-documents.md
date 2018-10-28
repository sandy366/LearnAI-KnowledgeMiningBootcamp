# Creating the Business Documents Bot

#### Download the Bot Framework Emulator  

You can download the v4 Preview Bot Framework Emulator for testing your bot locally. The instructions for the rest of the labs will assume you've downloaded the v4 Emulator (as opposed to the v3 Emulator). Download the emulator by going to [this page](https://github.com/Microsoft/BotFramework-Emulator/releases) and downloading the most recent version of the emulator that has the tag "Latest Release" (select the ".exe" file, if you are using windows).  

The emulator installs to `c:\Users\`_your-username_`\AppData\Local\botframework\app-`_version_`\botframework-emulator.exe` or to your Downloads folder, depending on browser.  

Now that you've downloaded and open the Bot Emulator, the next thing you have to set up is [ngrok, which allows us to connect to bots hosted remotely](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-debug-emulator?view=azure-bot-service-4.0#configure-ngrok). While most of the testing we'll do is local, you'll need ngrok to access the published version of the bot in the Emulator. There are two main steps to set this up:
1. Download ngrok from this website: https://ngrok.com/download. Click "Save as" when you download it so you can control where it gets saved (pick your Documents or Downloads folder, it should default to Downloads). Next, navigate to the install location and unzip the file (right click and select "Extract all...") and extract it one level up from the downloaded file (e.g. to  C:\Users\antho\Downloads\).  
2. Open the Bot Framework Emulator and hit the settings button (gear icon in bottom left corner). In the "Path to ngrok" box, hit "Browse," find "ngrok.exe", click "Select ngrok", and then click "Save".

## Next Step

[Text Skills Lab](../labs/lab-final-case.md) or
[Back to Main Menu](../README.md)