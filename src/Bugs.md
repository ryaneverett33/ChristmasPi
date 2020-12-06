# Bugs
- [LOGGING] RedirectHandler is getting the wrong context
- [EASINGS] Easings don't return the proper value
- Dec 05 16:52:30 raspberrypi systemd[1]: Scheduler.service: Main process exited, code=killed, status=6/ABRT
    Dec 05 16:52:30 raspberrypi dotnet[1157]: Successfully loaded schedule
    Dec 05 16:52:30 raspberrypi dotnet[1157]: Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.
    Dec 05 16:52:30 raspberrypi dotnet[1157]:    at ChristmasPi.Scheduler.Scheduler.getCurrentSchedule() in /Users/reverett/Documents/Repos/ChristmasPi/src/Scheduler.cs:line 270
    Dec 05 16:52:30 raspberrypi dotnet[1157]:    at ChristmasPi.Scheduler.Scheduler.Run() in /Users/reverett/Documents/Repos/ChristmasPi/src/Scheduler.cs:line 108
    Dec 05 16:52:30 raspberrypi dotnet[1157]:    at ChristmasPi.Scheduler.Program.Main(String[] args) in /Users/reverett/Documents/Repos/ChristmasPi/src/Scheduler.cs:line 17