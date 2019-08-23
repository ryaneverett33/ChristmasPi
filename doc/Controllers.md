# Controllers

- AuthController
- TreeController
- AnimationController
- ReactiveController
- SolidController
- SettingsController

## AuthController

Performs basic user authentication.

### Routes

| Type  | Location  | Description  |
| ----- |---------- | ------------ |
| POST  | /api/auth | Authenticates a user |

## TreeController

Gets information about the tree and how it's operating.

| Type  | Location  | Description  |
| ----- |---------- | ------------ |
| GET   | /api/tree | Gets info about the tree |
| GET   | /api/tree/mode | Gets info about the current operating mode |

## AnimationsController

Gets info about animations as well as changes which animation is playing and pausing/stopping it.

| Type  | Location  | Description  |
| ----- |---------- | ------------ |
| GET   | /api/animations | Gets info about the animations |
| POST  | /api/animations/play | Starts playing an animation |
| POST  | /api/animations/pause | Pauses the current playing animation |
| POST  | /api/animations/stop | Stops the current animation and returns to solid color mode |

See [Animations](Animations.md)

## ReactiveController

Handles starting/stopping of the audio-reactive mode as well as updating mode settings.

| Type  | Location  | Description  |
| ----- |---------- | ------------ |
| POST  | /api/reactive/play | Starts the audio-reactive mode |
| POST  | /api/reactive/pause | Pauses the audio reactivity |
| POST  | /api/reactive/stop | Stops the mode and returns to solid color mode |
| POST  | /api/reactive/update | Sets info about the reactive mode |

See [Reactive](Reactive.md)

## SolidController

Handles starting the solid color mode.

| Type  | Location  | Description  |
| ----- |---------- | ------------ |
| POST  | /api/solid/update | Sets the current solid color and switches to the solid color mode |

See [Solid Color](SolidColor.md)

## SettingsController

Handles getting and changing the settings.

| Type  | Location  | Description  |
| ----- |---------- | ------------ |
| POST  | /api/settings/update | Updates a given setting |
| GET   | /api/settings/info | Gets information such as uptime, plugins loaded |
| POST  | /api/settings/action/reload | Starts a server restart command |
| POST  | /api/settings/action/reboot | Starts a physical reboot command |
| POST  | /api/settings/action/confirm | Confirms an action such as reload or reboot |

See [Settings](Settings.md)