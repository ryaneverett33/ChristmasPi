using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChristmasServer.Methods {
    class listAnimations : IMethod {
        bool isValid = false;
        public bool isValidArguments(JProperty prop)
        {
            //requires no parameters
            return true;
        }
        public void runMethod()
        {
            if (!isValid) {
                Console.WriteLine("Called runMethod with invalid arguments");
            }
            
        }
        public ReceivedMessage.MessageType getType()
        {
            return ReceivedMessage.MessageType.GetMessage;
        }
    }
}
