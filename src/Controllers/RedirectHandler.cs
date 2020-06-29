using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Data;
using ChristmasPi.Util;
using ChristmasPi.Operations;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Data.Interfaces;

namespace ChristmasPi.Controllers {
    public class RedirectHandler {
        #region Singleton Methods
        private static readonly RedirectHandler _instance = new RedirectHandler();
        private static RedirectHandler Instance { get { return _instance; } }
        #endregion
        public delegate void RegisterLookupHandler();
        private RegisterLookupHandler onRegisteringLookups;

        // Errors
        public bool NotAdminError = false;

        // General redirect
        private bool DoSetup = false;

        private bool hasError { get {
                return Instance.NotAdminError;
            } }
        private bool hasRedirect { get {
                return Instance.DoSetup;
            } }
        // Map actual action to friendly name
        // Ex: Actual action "SetupHardware" -> "hardware"
        private Dictionary<string, Dictionary<string, string>> actionLookupTable;

        public static void Init() {
            Instance.DoSetup = ConfigurationManager.Instance.CurrentTreeConfig.setup.firstrun;
            if (!ConfigurationManager.Instance.DebugConfiguration.IgnorePrivileges)
                Instance.NotAdminError = !OSUtils.IsAdmin();
            if (Instance.actionLookupTable == null)
                Instance.actionLookupTable = new Dictionary<string, Dictionary<string, string>>();
            Instance.onRegisteringLookups.Invoke();
        }
        public static bool IsActionLookupRegistered(string controller) {
            if (controller == null || controller.Length == 0)
                throw new ArgumentNullException("controller");
            if (Instance.actionLookupTable != null)
                return Instance.actionLookupTable.ContainsKey(controller);
            return false;
        }

        public static void AddOnRegisteringLookupHandler(RegisterLookupHandler handler) {
            if (handler == null)
                throw new ArgumentNullException("handler");
            Instance.onRegisteringLookups += handler;
        }

        public static void RegisterActionLookup(string controller, Dictionary<string, string> lookupTable) {
            Console.WriteLine($"Registering lookup for {controller}");
            if (controller == null || controller.Length == 0)
                throw new ArgumentNullException("controller");
            if (lookupTable == null || lookupTable.Count == 0)
                throw new ArgumentNullException("Lookup Table");
            if (Instance.actionLookupTable == null)
                throw new ApplicationException("RedirectHandler has not been initialized yet");
            if (Instance.actionLookupTable.ContainsKey(controller))
                throw new ArgumentException("Controller already registered a lookup table");
            Instance.actionLookupTable[controller] = lookupTable;
        }

        public static IActionResult ShouldRedirect(RouteData routeData, string method) {
            string controller = (string)routeData.Values["controller"];
            string action = (string)routeData.Values["action"];
            if (Instance.shouldRedirectBuiltin(controller, action, method) is string url)
                return new RedirectResult(url);
            if (Instance.shouldRedirectControllers(controller, action, method) is string urlControllers)
                return new RedirectResult(urlControllers);
            return null;
        }

        private string shouldRedirectBuiltin(string controller, string action, string method) {
            // check if not admin
            if (controller != "Error" &&  NotAdminError)
                return "/error/notadmin";
            if (controller != "Setup" && DoSetup)
                return "/setup/";
            // TODO other checks
            return null;
        }

        private string shouldRedirectControllers(string controller, string action, string method) {
            switch (controller) {
                case "Setup":
                    if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode)) {
                        return null;
                    }
                    if (actionLookupTable.ContainsKey(controller)) {
                        Console.WriteLine($"Looking up action for controller: {controller}, action: {action}");
                        Dictionary<string, string> lookupTable = actionLookupTable[controller];
                        if (!lookupTable.ContainsKey(action)) {
                            Console.WriteLine("LOGTHIS - RedirectHandler::shouldRedirectControllers() Lookup table failed");
                            Console.WriteLine("\tLookup table for controller {0} does not have a mapping for action {1}", controller, action);
                            return (OperationManager.Instance.CurrentOperatingMode as IRedirectable).ShouldRedirect(controller, action, method);
                        }
                        else
                            return (OperationManager.Instance.CurrentOperatingMode as IRedirectable).ShouldRedirect(controller, lookupTable[action], method);
                    }
                    else {
                        Console.WriteLine($"Does not have a lookup table for controller: {controller}");
                        return (OperationManager.Instance.CurrentOperatingMode as IRedirectable).ShouldRedirect(controller, action, method);
                    }
                default:
                    return null;
            }
        }

        public static void SetupComplete() {
            Instance.DoSetup = false;
        }
    }
}
