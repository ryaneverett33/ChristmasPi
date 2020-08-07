# TODO

## Release 0.5
- [FRONTEND] Services reindeer gif transition to checkmark
	- Start reindeer when installing
	- Add appropriate gifs, reindeer->checkmark gif and reindeer->failure gif
	- Transition to checkmark/failure on service installation finish
- [BACKEND] Implement services installation
	- On successful ChristmasPi service installation, the server should restart gracefully
- [FRONTEND] Replace hardware/unknown.png or give proper credit
- [FRONTEND] Implement mobile version of schedule
- [FRONTEND] Schedule add rule modal should work for mobile
- [BACKEND] Refactor other controllers to use RedirectHandler
- [LOGGING] RedirectHandler should have it's own log
- [BACKEND] Save setup progress and restore setup progress
	- Allow for setup wizard to restart (after installing service)
- [BACKEND] Add wait for old ChristmasPi instance to close
	- Add configuration argument for specifying PID to wait for (or flag to just wait in general)
- [BACKEND] Cleanup animations
	- Implement debug property for animations
	- Add debug configuration for allowing debug animations
	- Implement controller logic for filtering out debug animations
	- Select which animations should be debug animations
- [GENERAL] Create installer package
	- Maybe add bootstrapper script to start setup process

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
- [FRONTEND] Add Settings tab
- [FRONTEND] Implement main settings page
- [BACKEND] Create a settings controller
- [BACKEND] Allow updating various settings of the tree
- [BACKEND] Allow monitoring of services
	- Check if they're alive
	- Maybe start them if they're not alive?