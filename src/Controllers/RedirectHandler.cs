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
using Serilog;

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
        private Dictionary<string, Func<string, string, string, string>> functionRuleTable;

        public static void Init() {
            Instance.DoSetup = ConfigurationManager.Instance.CurrentTreeConfig.setup.firstrun;
            if (!ConfigurationManager.Instance.RuntimeConfiguration.IgnorePrivileges)
                Instance.NotAdminError = !OSUtils.IsAdmin();
            if (Instance.actionLookupTable == null)
                Instance.actionLookupTable = new Dictionary<string, Dictionary<string, string>>();
            if (Instance.functionRuleTable == null)
                Instance.functionRuleTable = new Dictionary<string, Func<string, string, string, string>>();
            Instance.onRegisteringLookups.Invoke();
        }
        public static bool IsActionLookupRegistered(string controller) {
            if (controller == null || controller.Length == 0)
                throw new ArgumentNullException("controller");
            if (Instance.actionLookupTable != null)
                return Instance.actionLookupTable.ContainsKey(controller);
            return false;
        }
        public static bool IsRuleFunctionRegistered(string controller) {
            if (controller == null || controller.Length == 0)
                throw new ArgumentNullException("controller");
            if (Instance.functionRuleTable != null)
                return Instance.functionRuleTable.ContainsKey(controller);
            return false;
        }

        public static void AddOnRegisteringLookupHandler(RegisterLookupHandler handler) {
            if (handler == null)
                throw new ArgumentNullException("handler");
            Instance.onRegisteringLookups += handler;
        }

        public static void RegisterActionLookup(string controller, Dictionary<string, string> lookupTable) {
            Log.ForContext<RedirectHandler>().Debug("Registering lookup for {controller}", controller);
            if (controller == null || controller.Length == 0)
                throw new ArgumentNullException("controller");
            if (lookupTable == null || lookupTable.Count == 0)
                throw new ArgumentNullException("Lookup Table");
            if (Instance.functionRuleTable == null)
                throw new ApplicationException("RedirectHandler has not been initialized yet");
            if (Instance.actionLookupTable.ContainsKey(controller))
                throw new ArgumentException("Controller already registered a lookup table");
            Instance.actionLookupTable[controller] = lookupTable;
        }
        
        public static void RegisterLookupRules(string controller, Func<string, string, string, string> ruleFunc) {
            Log.ForContext<RedirectHandler>().Debug("Registering rules for {controller}", controller);
            if (controller == null || controller.Length == 0)
                throw new ArgumentNullException("controller");
            if (ruleFunc == null)
                throw new ArgumentNullException("Lookup Function");
            if (Instance.functionRuleTable == null)
                throw new ApplicationException("RedirectHandler has not been initialized yet");
            if (Instance.functionRuleTable.ContainsKey(controller))
                throw new ArgumentException("Controller already registered a rule function");
            Instance.functionRuleTable[controller] = ruleFunc;
        }

        public static IActionResult ShouldRedirect(RouteData routeData, string method) {
            string controller = (string)routeData.Values["controller"];
            string action = (string)routeData.Values["action"];
            Log.ForContext<RedirectHandler>().Debug("ShouldRedirect for controller {controller}, action {action}, and method {method}",
                                                        controller, action, method);
            if (Instance.shouldRedirectBuiltin(controller, action, method) is string url) {
                Log.ForContext<RedirectHandler>().Debug("Redirecting builtin to {url}", url);
                return new RedirectResult(url);
            }
            if (Instance.shouldRedirectControllers(controller, action, method) is string urlControllers) {
                Log.ForContext<RedirectHandler>().Debug("Redirecting for controller to {urlControllers}", urlControllers);
                return new RedirectResult(urlControllers);
            }   
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
            if (!functionRuleTable.ContainsKey(controller)) {
                Log.ForContext<RedirectHandler>().Debug("Function rule doesn't contain a function for {controller}", controller);
                return null;
            }
            if (actionLookupTable.ContainsKey(controller)) {
                Log.ForContext<RedirectHandler>().Debug("Looking up action for controller: {controller}, action: {action}, method: {method}", controller, action, method);
                Dictionary<string, string> lookupTable = actionLookupTable[controller];
                string useAction;
                if (!lookupTable.ContainsKey(action)) {
                    Log.ForContext<RedirectHandler>().Debug("Lookup table failed");
                    Log.ForContext<RedirectHandler>().Debug("Lookup table for controller {controller} does not have a mapping for action {action}",
                                                                        controller, action);
                    useAction = action;
                }
                else {
                    useAction = lookupTable[action];
                    Log.ForContext<RedirectHandler>().Debug("Performed successful lookup for {action}, new action is {useAction}", action, useAction);
                }
                Func<string, string, string, string> ruleFunc = functionRuleTable[controller];
                return ruleFunc(controller, useAction, method);
            }
            else {
                Log.ForContext<RedirectHandler>().Debug("Does not have a lookup table for controller: {controller}", controller);
                Func<string, string, string, string> ruleFunc = functionRuleTable[controller];
                return ruleFunc(controller, action, method);
            }
        }

        public static void SetupComplete() {
            Instance.DoSetup = false;
        }
    }
}
