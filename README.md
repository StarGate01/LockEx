# LockEx

**An extended lockscreen app and library for Windows Phone 8.1**

**RTComponent** is a Windows Runtime Component wrapper for ```ShellChromeAPI.dll#Shell_LockScreen_GetNotificationsSnapshot```, and also loads icons from ```UIXMobileAssets{ScreenResolution}.dll```. It is tested and probably stable.

**LockEx** is a Windows Phone 8.1 Silverlight App, which uses RTComponent. It is not finished and in early beta. Also it currently uses the ```ID_CAP_SHELL_DEVICE_LOCK_UI_API``` and ```ID_CAP_CHAMBER_PROFILE_CODE_RW``` capabilities, so it will only work on an interop- and capability-unlocked phone.

**To build**, RTComponent must be linked with ```ShellChromeAPI.lib```, which can be extracted from the Phone (```C:\Windows\System32\ShellChromeAPI.dll```) as a DLL, and then compiled into a LIB.

**[Downloads](https://github.com/StarGate01/LockEx/releases) (Releases)**

**[German forum posts](http://wpvision.de/stargate01/6770-%5Bhack%5D-lockex-lockscreen-bibliothek-und-app-open-source.html)**

**Thanks to / Sources**: 
- https://wp8nativeaccess.codeplex.com/
- http://vilic.info/blog/archives/1138
- http://forum.xda-developers.com/showthread.php?t=1944675
- https://github.com/tpn/winsdk-10/blob/38ad81285f0adf5f390e5465967302dd84913ed2/Include/10.0.10240.0/um/ShellLockScreenAPITypes.h
- http://forum.xda-developers.com/windows-phone-8/development/xap-lockscreen-app-t2837016
- The helpful hackers at XDA
