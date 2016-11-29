using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChristmasServer.Methods {
    class currentlyPlaying : IMethod {
        public bool isValidArguments(JProperty prop)
        {
            System.Diagnostics.Debug.WriteLine("{0} is not implemented", this.ToString());
            return false;
        }
        public void runMethod()
        {
            System.Diagnostics.Debug.WriteLine("{0} cannot run", this.ToString());
        }
        public ReceivedMessage.MessageType getType()
        {
            return ReceivedMessage.MessageType.GetMessage;
        }
    }
}
