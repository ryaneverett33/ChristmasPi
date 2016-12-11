using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ChristmasServer.Methods {
    class MethodResult {
        JObject returns;
        bool success { get; set; } 
        string failureReason { get; set; }
        ReceivedMessage.MessageType type;
        public MethodResult(JObject returns, bool success, string failureReason, ReceivedMessage.MessageType type) {
            this.returns = returns;
            this.success = success;
            this.failureReason = failureReason;
            this.type = type;
        }
        //serializes the object
        public override string ToString() {
            if (type == ReceivedMessage.MessageType.PostMessage) {
                //doesn't include success failure
                JObject json = new JObject();
                if (!success && failureReason == null) {
                    throw new FormatException("Object is not initialized properly");
                }
                json.Add(new JProperty("success",success));
                json.Add(new JProperty("failureReason", failureReason));
                return new JObject(new JProperty("returns", json)).ToString();
            }
            else if (type == ReceivedMessage.MessageType.GetMessage) {
                //only includes success failure
                JObject json = new JObject();
                json.Add("returns", returns);
                return json.ToString();
            }
            else {
                return null;
            }
        }
    }
}
