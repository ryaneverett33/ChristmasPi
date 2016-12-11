using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChristmasServer {
    sealed class HttpArgs {
        public string method;
        public JProperty parameters;                    //the params bit of an RPC request
        public float rpcVersion;

        /// <summary>
        /// Determines if the Arguments of the URL are valid, and updates an HttpArgs if they are.
        /// </summary>
        /// <param name="line" cref="Program.serverListen(Program)">firstLineSplit[0] : The Http Host line</param>
        /// <param name="isValidURL">out bool describing whether the arguments in the URL are valid or not</param>
        /// <param name="args">out HttpArgs containing the filled out arguments</param>
        public static void isValidGetArgs(string line, out bool isValidURL, out HttpArgs args) {
            //find first instance of '{', remove until then
            int bracketLoc = line.IndexOf('{');
            string strippedJson = line.Remove(0, bracketLoc);
            HttpArgs newArgs = new HttpArgs();
            bool encounteredError = false;
            try {
                var jObject = JObject.Parse(strippedJson);
                foreach (JProperty prop in jObject.Properties()) {
                    //Console.WriteLine(child.ToString());
                    //Console.WriteLine("Name: {0}, value: {1}", prop.Name, prop.Value.ToString());
                    if (prop.Name.Equals("method", StringComparison.CurrentCultureIgnoreCase)) {
                        if (prop.Value.Type == JTokenType.String) {
                            newArgs.method = prop.Value.ToString();
                        } 
                        else {
                            //TODO: LOG THIS ERROR
                            Console.WriteLine("JSON has key method, but value is not a string. ERROR");
                            encounteredError = true;
                        }
                    }
                    else if (prop.Name.Equals("jsonrpc", StringComparison.CurrentCultureIgnoreCase)) {
                        if (prop.Value.Type == JTokenType.String) {
                            //Value is a float hidden as a string. Sigh.
                            string rpcString = prop.Value.ToString();
                            try {
                                newArgs.rpcVersion = float.Parse(rpcString);
                            }
                            catch (Exception) {
                                //TODO: LOG THIS ERROR
                                Console.WriteLine("JSON has key jsonrpc, but its value could not be parsed as a float. ERROR");
                                encounteredError = true;
                            }
                        }
                        else {
                            //TODO: LOG THIS ERROR
                            Console.WriteLine("JSON has key jsonrpc, but value is not a string. ERROR");
                            encounteredError = true;
                        }
                    }
                    else if (prop.Name.Equals("params", StringComparison.CurrentCultureIgnoreCase)) {
                        newArgs.parameters = prop;
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine("Could not parse JSON");
                Console.WriteLine(e.Message);
                isValidURL = false;
                args = null;
                return;
            }
            //check if args are valid or not
            if (encounteredError) {
                Console.WriteLine("HttpArgs:: Encountered Error");
                isValidURL = false;
                args = null;
            }
            else {
                //check if three keys have been set (method, parameters, version)
                if (newArgs.method != null && newArgs.parameters != null && newArgs.rpcVersion != 0) {
                    args = newArgs;
                    isValidURL = true;
                }
                else {
                    //TODO: LOG THIS ERROR
                    Console.WriteLine("RPC Args do not contain all three keys, invalid arguments. ERROR");
                    args = null;
                    isValidURL = false;
                }
            }
        }
        /// <summary>
        /// Determines whether or not the POST arguments are valid, and if they are updates them
        /// </summary>
        /// <param name="line"></param>
        /// <param name="isValidArgs"></param>
        /// <param name="args"></param>
        /// <seealso cref="isValidGetArgs(string, out bool, out HttpArgs)"/>
        public static void isValidPostArgs(string line, out bool isValidArgs, out HttpArgs args) {
            bool encounteredError = false;
            HttpArgs newArgs = new HttpArgs();
            if (line.Contains('\n')) {
                Console.WriteLine("isValidPostArgs::contained multiple lines, invalid");
                isValidArgs = false;
                args = null;
                return;
            }
            try {
                var jObject = JObject.Parse(line);
                foreach (JProperty prop in jObject.Properties()) {
                    if (prop.Name.Equals("method", StringComparison.CurrentCultureIgnoreCase)) {
                        if (prop.Value.Type == JTokenType.String) {
                            newArgs.method = prop.Value.ToString();
                        }
                        else {
                            //TODO: LOG THIS ERROR
                            Console.WriteLine("JSON has key method, but value is not a string. ERROR");
                            encounteredError = true;
                        }
                    }
                    else if (prop.Name.Equals("jsonrpc", StringComparison.CurrentCultureIgnoreCase)) {
                        if (prop.Value.Type == JTokenType.String) {
                            //Value is a float hidden as a string. Sigh.
                            string rpcString = prop.Value.ToString();
                            try {
                                newArgs.rpcVersion = float.Parse(rpcString);
                            }
                            catch (Exception) {
                                //TODO: LOG THIS ERROR
                                Console.WriteLine("JSON has key jsonrpc, but its value could not be parsed as a float. ERROR");
                                encounteredError = true;
                            }
                        }
                        else {
                            //TODO: LOG THIS ERROR
                            Console.WriteLine("JSON has key jsonrpc, but value is not a string. ERROR");
                            encounteredError = true;
                        }
                    }
                    else if (prop.Name.Equals("params", StringComparison.CurrentCultureIgnoreCase)) {
                        newArgs.parameters = prop;
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine("Could not parse JSON");
                Console.WriteLine(e.Message);
                encounteredError = true;
                isValidArgs = false;
                args = null;
                return;
            }
            if (encounteredError) {
                isValidArgs = false;
                args = null;
            }
            else {
                if (newArgs.method != null && newArgs.parameters != null && newArgs.rpcVersion != 0) {
                    args = newArgs;
                    isValidArgs = true;
                }
                else {
                    //TODO: LOG THIS ERROR
                    Console.WriteLine("RPC Args do not contain all three keys, invalid arguments. ERROR");
                    isValidArgs = false;
                    args = null;
                }
            }
        }
    }
}
