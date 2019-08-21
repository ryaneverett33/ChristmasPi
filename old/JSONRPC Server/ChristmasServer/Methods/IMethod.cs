using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ChristmasServer;

namespace ChristmasServer.Methods {
    interface IMethod {
        /// <remarks>
        /// Must be run before runMethod can be ran
        /// </remarks>
        /// <param name="properties">Unparsed params from the RPC call</param>
        /// <returns>Whether or not the Method can run</returns>
        bool isValidArguments(JProperty properties);
        /// <remarks>
        /// Will not run without validating arguments first
        /// </remarks>
        void runMethod();
        /// <returns>What type of Method it is (POST || GET)</returns>
        ReceivedMessage.MessageType getType();
    }
}
