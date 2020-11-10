# Bugs
{"SourceContext":"ChristmasPi.Operations.Modes.AnimationMode"}23:36:04.475 +00:00 [ERR] StartAnimation() encountered an exception while stopping animation
System.Threading.ThreadStateException: Thread is running or terminated; it cannot restart.
   at System.Threading.Thread.StartInternal()
   at System.Threading.Thread.Start()
   at ChristmasPi.Hardware.Renderers.RenderThread.Start() in /Users/reverett/Documents/Repos/ChristmasPi/src/Hardware/Renderers/RenderThread.cs:line 45
   at ChristmasPi.Hardware.Renderers.WS281xRenderer.Start() in /Users/reverett/Documents/Repos/ChristmasPi/src/Hardware/Renderers/WS281xRenderer.cs:line 73
   at ChristmasPi.Animation.Animator.Start() in /Users/reverett/Documents/Repos/ChristmasPi/src/Animation/Animator.cs:line 83
   at ChristmasPi.Operations.Modes.AnimationMode.StartAnimation(String animationName) in /Users/reverett/Documents/Repos/ChristmasPi/src/Operations/Modes/Animati$
 {"SourceContext":"ChristmasPi.Operations.Modes.AnimationMode","ExceptionDetail":{"HResult":-2146233056,"Message":"Thread is running or terminated; it cannot res$
 {"SourceContext":"ChristmasPi.Operations.Modes.AnimationMode"}23:36:13.991 +00:00 [INF] Activated Solid Color Mode

- [LOGGING] RedirectHandler is getting the wrong context
- [EASINGS] Easings don't return the proper value