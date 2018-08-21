# Mamesaver

v1.1
Licensed under The MIT License

[![Build status](https://ci.appveyor.com/api/projects/status/2b8n7te1bq8rf1pp?svg=true)](https://ci.appveyor.com/project/mmihajlovic/mamesaver)

### INTRODUCTION

Mamesaver is a Windows screen saver that runs MAME with a random game for specified intervals of time.

![General Settings](Resources/SettingsPanel1.png) ![Game List](Resources/SettingsPanel1.png)


### REQUIREMENTS
* Microsoft .NET 4.6
* A working installation of MAME. I used 0.200 but older ones will probably work too.
* Some working ROMS which can run in MAME successfully.
* Each ROM that is installed should have been run at least once to remove the initial disclaimer screen as this is the last thing you want to see when the screen saver is running.

### INSTALLATION
Install using the msi installer, or just copy the Mamesaver.scr file to your WINDOWS\SYSTEM32 or WINNT\SYSTEM32 directory.

### USAGE

Right-click on your desktop and choose Properties. Once the Display Properties dialog has opened navigate to the Screen Saver tab. There you need to choose Mamesaver as your screen saver.

### CONFIGURATION

The first time you choose Mamesaver as your screen saver, you will need to configure it to tell it the right path to Mame and to rebuild your game list. To do this, you need to click on the Settings button on the screen saver dialog (Display Properties dialog) as mentioned in USAGE.

You have a couple of choices for settings on Mamesaver. The most important setting in the beginning is the Mame Path on the General tab. Once this is set you can navigate to the Game List tab and click on Rebuild List. This can take a couple of minutes so please be patient. Once this has been run, please select which games you want Mamesaver to run. When you have finished, you may click the Ok button.

Other than those main settings, you can also set the interval at which the game is changed for another random one, minimum 2 minutes, maximum 1440 minutes (24 hours), and the command line options which are sent to MAME. I recommend leaving the -skip_gameinfo option.

### COMMENTS

* The game list will only contain games which have passed the Mame ROM audit and have drivers with a status of good. This means that games, which might be partially working but maybe have no sound or some other part of the driver is not working, will not be displayed in this list. The main reason for this is that MAME shows a dialog which expects user input at the beginning of the game, which is the exact opposite of what you would want a screen saver to do.

### CONTRIBUTORS
![](https://avatars2.githubusercontent.com/u/229311?s=44&v=4) Mladen Mihajlovic

![](https://avatars2.githubusercontent.com/u/1732347?s=44&v=4) Matt Painter

![](https://avatars2.githubusercontent.com/u/1904424?s=44&v=4) Andy van Stokkum

### OPEN SOURCE
The source code if provided as-is under the MIT Licence. If you have any suggestions and comments, please feel free to create issues here on github.
