
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChristmasServer.Methods;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ChristmasServer.Tests {
    public class MethodResultTesting {
        public void testBasic() {
            MethodResult res = new MethodResult(null, true, null, ReceivedMessage.MessageType.PostMessage);
            string result = @"{ 
	            'returns' : {
                        'success' : true,
			            'failureReason' : null
                    }
                }";
            string resResult = res.ToString();
            Debug.WriteLine(resResult);
        }
        public static void Main(string[] args) {
            MethodResultTesting meth = new MethodResultTesting();
            meth.testBasic();
        }
    }
    
}
