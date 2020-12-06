# TODO

## 0.5 Release
- Tweening frames
	- Tween from one color to another over a period of time
	- http://theinstructionlimit.com/flash-style-tweeneasing-functions-in-c c# implementation
	- https://easings.net/en reference of easing functions
- [BOTH] Allow reconfiguring animations
- [BACKEND] Implement ServiceInstaller::Dispose()
- Update documentation
- [BACKEND] Refactor method calls that return HTTP Status codes
	- This is insanely dumb and I am dumber having written them in the first place
- [BACKEND] Remove OperationMode::GetProperty()
	- use interfaces instead
- [BACKEND] Add support for custom animations

## General
- [?] branch animation is the result of a basic animation
- [ROUTING] Can navigate to an auxiliary page without having started setup
- [FRONTEND] Add animation parameters
- [FRONTEND] Make urls friendly if a setup post action fails
	- /setup/lights/submit -> /setup/lights
- [BUILD] Create release should automatically pull release number from Constants.VERSION
- [FRONTEND] Renable reset button?
- [FRONTEND] hardware should use builtin validation
	- https://developer.mozilla.org/en-US/docs/Learn/Forms/Form_validation
- [BACKEND] All threads should be be created/release by a manager object
	- should register a RegisterOnShutdownAction to clean up all threads
- [BACKEND] Add support for smart outlets
- [BACKEND] RedirectHandler should try and catch 404 errors
- [BACKEND] Refactor AnimationParameters to use Attributes
- [BACKEND] HelpFormatter should dynamically set `argumentColumnWidth` instead of being a constant
- [FRONTEND] Add Settings tab
- [FRONTEND] Implement main settings page
- [FRONTEND] Add support for Partial Views
	- https://docs.microsoft.com/en-us/aspnet/core/mvc/views/partial?view=aspnetcore-3.1
- [FRONTEND] Handle overlapping times
	- EX: 5:00-6:55
- [BACKEND] Create a settings controller
- [BACKEND] Split Rendering into it's own service
	- Server should no longer run as root, but the renderservice should
	- Interact via WCF https://github.com/dotnet/wcf
- [BACKEND] Allow updating various settings of the tree
- [BACKEND] Allow monitoring of services
	- Check if they're alive
	- Maybe start them if they're not alive?