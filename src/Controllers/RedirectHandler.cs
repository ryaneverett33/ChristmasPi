using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Data;
using ChristmasPi.Util;
using ChristmasPi.Operations;
using ChristmasPi.Operations.Interfaces;

namespace ChristmasPi.Controllers {
    public class RedirectHandler {
        #region Singleton Methods
        private static readonly RedirectHandler _instance = new RedirectHandler();
        private static RedirectHandler Instance { get { return _instance; } }
        #endregion

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

        public static void Init() {
            Instance.DoSetup = ConfigurationManager.Instance.CurrentTreeConfig.setup.firstrun;
            if (!ConfigurationManager.Instance.DebugConfiguration.IgnorePrivileges)
                Instance.NotAdminError = !OSUtils.IsAdmin();
        }

        public static bool ShouldRedirect(RouteData routeData) {
            string controller = (string)routeData.Values["controller"];
            string action = (string)routeData.Values["action"];
            Console.WriteLine("Controller: {0}, Action: {1}, isSetupMode: {2}", controller, action, OperationManager.Instance.CurrentOperatingMode is ISetupMode);
            
            // never redirect on the error controller
            if (controller.Equals("Error"))
                return false;
            //if (controller.Equals("Setup") && !action.Equals("Index") && !(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
            //    return true;
            if (controller.Equals("Setup") && !Instance.hasError)
                return false;
            return Instance.hasError | Instance.hasRedirect;
        }

        public static IActionResult Handle() {
            if (Instance.hasError) {
                if (Instance.NotAdminError)
                    return new RedirectResult("/error/notadmin");
                return null;
            }
            else {
                if (Instance.DoSetup)
                    return new RedirectResult("/setup/");
                return new RedirectResult("/");
            }
        }

        public static void SetupComplete() {
            Instance.DoSetup = false;
        }

        private static IActionResult handleNoAdmins() {
            return new RedirectResult("/error/notadmin");
        }
    }
}
