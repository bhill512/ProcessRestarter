# ProcessRestarter
Lately my Plex process has been dying. I decided to write an app that checks the status of the process and if it's not running, start it. I'm sure there's many out there, 
but it was fun
- You'll need the [dotnet runtime found here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime)
## Install
I have a release up that you can grab. Go to the [releases page](https://github.com/bhill512/ProcessRestarter/releases) and download the latest
- ensure you have the dotnet runtime installed
- unzip the contents to your desired directory
- open the ```appsettings.json``` file and in the ```"Processes"``` section add an object for the process you want to monitor
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
- make sure you have the exe name as it appears is Task Manager without the .exe
- then for the location have the full path to the exe
- run ```ProcessRestarter.exe```
- watch it work


