# ProcessRestarter
Lately my Plex process has been dying. I decided to write an app that checks the status of the process and if it's not running, start it. I'm sure there's many out there, 
but it was fun
- You'll need the [dotnet runtime found here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime)
## Install and Setup
I have a release up that you can grab. Go to the [releases page](https://github.com/bhill512/ProcessRestarter/releases) and download the latest
- ensure you have the [dotnet runtime installed](https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime)
- unzip the contents to your desired directory
- open the ```appsettings.json``` file and in the ```"Processes"``` section add an object for the process you want to monitor\
for example:
```
    "Processes": [
        {
            "Name": "Plex Media Server",
            "Location": "C:\\Program Files (x86)\\Plex\\Plex Media Server\\Plex Media Server.exe"
        },
        {
            "Name": "Tautulli",
            "Location": "C:\\Program Files\\Tautulli\\Tautulli.exe"
        }
    ],
```
- also be sure to add your Plex server IP address in the ```ServerUrl``` area and your ```XPlexToken``` to the ```appsettings.json``` file
```
    "Plex": {
        "ServerUrl": "http://your_server_ip:32400",
        "XPlexToken": "your_x_plex_token"
    },
```
- instructions on finding your xplextoken can be found [here](https://www.plexopedia.com/plex-media-server/general/plex-token/)
- for the ```Name``` use the exe name as it appears in Task Manager without the .exe
- for the ```Location``` have the full path to the exe
- run ```ProcessRestarter.exe```
- watch it work

### Note
- Please shut down the process restarter when you go to update any process that you're monitoring with it. It will try to start the process during an update and the update will fail
