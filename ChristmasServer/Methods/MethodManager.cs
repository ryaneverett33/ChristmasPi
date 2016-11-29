using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChristmasServer;

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
            methDict = new Dictionary<string, IMethod>(10);
            methDict.Add("playAnimation", new playAnimation());
            methDict.Add("stopAnimation", new stopAnimation());
            methDict.Add("pauseAnimation", new pauseAnimation());
            methDict.Add("resumeAnimation", new resumeAnimation());
            methDict.Add("updateLights", new updateLights());
            methDict.Add("currentPlaying", new currentlyPlaying());
            methDict.Add("isPlaying", new isPlaying());
            methDict.Add("listTreeInfo", new listTreeInfo());
        }
        public bool isValidMethod(string methodName,ReceivedMessage.MessageType type)
        {
            IMethod retrievedMethod;
            bool containsMethod = methDict.TryGetValue(methodName, out retrievedMethod);
            if (!containsMethod) return false;
            if (retrievedMethod.getType() == type) return true;
            return false;
        }

    }
}
