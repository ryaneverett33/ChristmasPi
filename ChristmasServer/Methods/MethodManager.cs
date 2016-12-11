using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChristmasServer;
using Newtonsoft.Json.Linq;

namespace ChristmasServer.Methods
{
    /// <summary>
    /// Handles executing methods
    /// </summary>
    class MethodManager
    {
        Dictionary<string, IMethod> methDict;           //Dictionary containing all the methods available to call
        public MethodManager()
        {
            /// <remarks>
            /// All methods have to be manually put into the manager
            /// </remarks>
            methDict = new Dictionary<string, IMethod>(10, StringComparer.OrdinalIgnoreCase);
            methDict.Add("playAnimation", new playAnimation());
            methDict.Add("stopAnimation", new stopAnimation());
            //methDict.Add("pauseAnimation", new pauseAnimation());
            //methDict.Add("resumeAnimation", new resumeAnimation());
            methDict.Add("updateLights", new updateLights());
            methDict.Add("currentPlaying", new currentlyPlaying());
            methDict.Add("isPlaying", new isPlaying());
            methDict.Add("listTreeInfo", new listTreeInfo());
            methDict.Add("listAnimations", new listAnimations());
        }
        private bool isValidMethod(string methodName,ReceivedMessage.MessageType type, out IMethod method)
        {
            IMethod retrievedMethod;
            bool containsMethod = methDict.TryGetValue(methodName, out retrievedMethod);
            if (!containsMethod) {
                method = null;
                System.Diagnostics.Debug.WriteLine("MethMan::does not contain method");
                return false;
            }
            if (retrievedMethod.getType() == type) {
                method = retrievedMethod;
                return true;
            }
            else {
                System.Diagnostics.Debug.WriteLine("MethMan::Types don't match!");
            }
            System.Diagnostics.Debug.WriteLine("MethMan::Exit case, returning null");
            method = null;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="type"></param>
        /// <exception cref="MethodAccessException">If method does not exist</exception>
        /// <exception cref="ArgumentException">If arguments are invalid</exception>
        public void callMethod(string methodName, JProperty args, ReceivedMessage.MessageType type) {
            IMethod method = null;
            if (isValidMethod(methodName, type, out method)) {
                if (method.isValidArguments(args)) {
                    method.runMethod();
                }
                else {
                    throw new ArgumentException("Arguments are invalid for method");
                }
            }
            else {
                throw new MethodAccessException(methodName + " is not a valid method or the type is incorrect");
            }
        }
    }
}
