# Gw2Sharp
Gw2Sharp is a Xamarin.Forms app used to display data from the Guild Wars 2 API.  
For now, it lets you check recent gold to gem ratio from in-game currency exchange.
This app also allows you to browse item details by typing item names and checking item's API data.  
It uses local SQLite database to store item names and ids so that it can convert typed item name to correct item id.
# Building
Build the solution with Visual Studio 2017 or newer.
# Installation guide
### Follow these steps to install on Windows 10:

1.  **Turn on app sideloading:**
    1. Open Windows 10 Settings.
    2. Click Update & Security > For developers.
    3. On Use developer features, select Sideload apps.

2.  **Import the security certificate:**
    1. Download the [security certificate.](https://github.com/iyarashii/Gw2Sharp/releases/download/v1.0.0.0/Gw2Sharp.UWP_1.0.0.0_x86_x64_arm.cer)
    2. Open the security certificate and select Install Certificate.
    3. On the Certificate Import Wizard, select Local Machine.
    4. Import the certificate to the Trusted Root Certification Authorities folder.

3.  **Install the app:**
    1. Download [appx bundle.](https://github.com/iyarashii/Gw2Sharp/releases/download/v1.0.0.0/Gw2Sharp.UWP_1.0.0.0_x86_x64_arm.appxbundle)
    2. Double-click downloaded appx bundle and click install.
### Follow these steps to install on Android:
1. Download the newest [apk](https://github.com/iyarashii/Gw2Sharp/releases/download/v1.0.0.0/iyarashii.github.Gw2Sharp.apk) to your Android device.
2. Run the apk on the device.
# Usage
After successful installation run the app go to '*Settings*' and click '*Update local item name & id database*' wait for it to finish.   
You should repeat this action after every Guild Wars 2 update to keep the local database up-to-date.
