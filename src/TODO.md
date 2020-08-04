# TODO

## Release 0.5
- [FRONTEND] Services start reindeer when installing
- [FRONTEND] Services reindeer gif transition to checkmark
- [FRONTEND] Add Checkmark icon and transition gif
- [BACKEND] Implement services installation
- [FRONTEND] Replace hardware/unknown.png or give proper credit
- [FRONTEND] Implement mobile version of schedule
- [FRONTEND] Schedule add rule modal should work for mobile
- [BACKEND] Refactor other controllers to use RedirectHandler
- [BACKEND] Cleanup animations
	- Implement debug property for animations
	- Add debug configuration for allowing debug animations
	- Implement controller logic for filtering out debug animations
	- Select which animations should be debug animations

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