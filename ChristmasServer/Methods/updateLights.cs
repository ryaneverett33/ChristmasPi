using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChristmasServer.Methods {
    class updateLights : IMethod {
        bool isValid = false;
        public bool isValidArguments(JProperty prop)
        {
            System.Diagnostics.Debug.WriteLine("{0} is not implemented", this.ToString());
            return false;
        }
        public void runMethod()
        {
            if (!isValid) {
                Console.WriteLine("Called runMethod with invalid arguments");
            }
            System.Diagnostics.Debug.WriteLine("{0} cannot run", this.ToString());
        }
        public ReceivedMessage.MessageType getType()
        {
            return ReceivedMessage.MessageType.PostMessage;
        }
    }
}
