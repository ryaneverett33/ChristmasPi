using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChristmasServer.Methods {
    interface IMethod {
        /// <notes>
        /// Must be run before runMethod can be ran
        /// </notes>
        /// <param name="properties">Unparsed params from the RPC call</param>
        /// <returns></returns>
        bool isValidArguments(JProperty properties);
        /// <notes>
        /// Will not run without validating arguments first
        /// </notes>
        void runMethod();
    }
}
