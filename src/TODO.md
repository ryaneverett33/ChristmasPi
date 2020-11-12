# TODO

## Release 0.5
- [GENERAL] Create installer package
	- Maybe add bootstrapper script to start setup process
		- install dotnet to /opt/dotnet
		- add to global path /etc/profile.d/dotnet.sh
	- Setup process sometimes doesn't leave setup mode
	- After setup, modes fail to activate
		- unreproducible be just running a setup process
		- possibly related to bootstrap installer
	- Scheduler::unload hangs
	- Scheduler has issues running under systemd
	- Add debug prints to SetupController

## General
- [?] branch animation is the result of a basic animation
- Tweening frames
	- Tween from one color to another over a period of time
	- http://theinstructionlimit.com/flash-style-tweeneasing-functions-in-c c# implementation
	- https://easings.net/en reference of easing functions
- [FRONTEND] Add animation parameters
- [BACKEND] All threads should be be created/release by a manager object
	- should register a RegisterOnShutdownAction to clean up all threads
- [BOTH] Allow reconfiguring animations
- [BACKEND] Add support for smart outlets
- [BACKEND] Implement ServiceInstaller::Dispose()
- [BACKEND] RedirectHandler should try and catch 404 errors
- [BACKEND] Refactor AnimationParameters to use Attributes
- [BACKEND] HelpFormatter should dynamically set `argumentColumnWidth` instead of being a constant
- [BACKEND] Remove OperationMode::GetProperty()
	- use interfaces instead
- [FRONTEND] Add Settings tab
- [FRONTEND] Implement main settings page
- [FRONTEND] Add support for Partial Views
	- https://docs.microsoft.com/en-us/aspnet/core/mvc/views/partial?view=aspnetcore-3.1
- [FRONTEND] Handle overlapping times
	- EX: 5:00-6:55
- [BACKEND] Create a settings controller
- [BACKEND] Allow updating various settings of the tree
- [BACKEND] Allow monitoring of services
	- Check if they're alive
	- Maybe start them if they're not alive?