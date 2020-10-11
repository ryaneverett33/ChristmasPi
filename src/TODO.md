# TODO

## Release 0.5
- [FRONTEND] Services reindeer gif transition to checkmark
	- Start reindeer when installing /
	- Add appropriate gifs, reindeer->checkmark gif and reindeer->failure gif
	- Transition to checkmark/failure on service installation finish
- [FRONTEND] Implement mobile version of schedule
- [BACKEND] Cleanup animations
	- Implement debug property for animations /
	- Add debug configuration for allowing debug animations /
	- Implement controller logic for filtering out debug animations /
	- Select which animations should be debug animations
- [GENERAL] Create installer package
	- Maybe add bootstrapper script to start setup process
		- install dotnet to /opt/dotnet
		- add to global path /etc/profile.d/dotnet.sh
	- Add builder for creating build packages (zip up the build directory) /
- [FRONTEND] If a service fails to install with a restart pending, an alert should be raised before redirecting

## General
- [?] branch animation is the result of a basic animation
- Tweening frames
	- Tween from one color to another over a period of time
	- http://theinstructionlimit.com/flash-style-tweeneasing-functions-in-c c# implementation
	- https://easings.net/en reference of easing functions
- [FRONTEND] Add animation parameters
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
- [BACKEND] Create a settings controller
- [BACKEND] Allow updating various settings of the tree
- [BACKEND] Allow monitoring of services
	- Check if they're alive
	- Maybe start them if they're not alive?