# TorrentKillSwitch

Installation
1. Build
2. Install Service
2.1 Open Developer Command Prompt in admin
2.2 Type the following command:
2.2.1 cd <physical location of your TorrentKillSwitch.exe file>
2.2.2 InstallUtil.exe “TorrentKillSwitch.exe”
2.3 Configure login of service as necessary for your environment.
2.4 Alter config for service
2.4.1 Change the setting "ProcessName" to what ever your application's actual process name is. 
2.4.2 By default it will take down qbitorrent.

To Use:
Run the GUI exe (requires admin)
Press the button to enable at current safe IP address.
Press button again to disable.

Notes:
This application uses the api https://api.myip.com/ to monitor your current ip address.
Every 5 seconds, it will poll for your api.
If your ip address ever changes from it's value when you first presssed the start button, it will kill your process.
