using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ChristmasServer {
    /// <summary>
    /// Describes a parsed Message received through the server
    /// </summary>
    sealed class ReceivedMessage {
        public enum MessageType {
            PostMessage,
            GetMessage,
            Unknown
        }
        public MessageType type;
        public string rawText;
        public string rawURL;
        public float HTTPVersion;
        public bool isValidURL;
        public bool isValidPostArgs;
        public HttpArgs httpArgs;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recievedMessage">The raw text received by the server</param>
        /// <returns></returns>
        public static ReceivedMessage parseHTTP(string recievedMessage, Logger log, Socket sock) {
            ReceivedMessage message  = new ReceivedMessage();
            message.rawText = recievedMessage;
            string[] lines = recievedMessage.Split('\n');
            string[] firstLineSplit = new string[3];                //First line defines HTTP type and URL

            if (lines.Length == 0) {
                log.logError("Recieved Message contained no data");
                return null;
            }
            //Determine what type the message is
            string[] tmpLineSplit = lines[0].Split(' ');
            if (tmpLineSplit.Length == 0) {
                log.logError("Recieved Message first line contained no data");
                return null;
            }
            else {
                string middle = "";
                for (int i = 0; i < tmpLineSplit.Length; i++) {
                    if (i == 0) {
                        firstLineSplit[0] = tmpLineSplit[0];
                        continue;
                    }
                    if (i == (tmpLineSplit.Length - 1)) {
                        firstLineSplit[2] = tmpLineSplit[i];
                        continue;
                    }
                    middle = middle + tmpLineSplit[i];
                }
                firstLineSplit[1] = middle;
            }
            message.rawURL = firstLineSplit[1];
            switch (firstLineSplit[0]) {
                case "POST":
                    message.type = MessageType.PostMessage;
                    break;
                case "GET":
                    message.type = MessageType.GetMessage;
                    break;
                default:
                    message.type = MessageType.Unknown;
                    break;
            }
            //check for Valid URL
            switch (message.type) {
                case MessageType.GetMessage:
                    if (firstLineSplit[1].Contains("/jsonrpc?request")) {
                        //check if request is valid, and update args as well
                        HttpArgs.isValidGetArgs(firstLineSplit[1], out message.isValidURL, out message.httpArgs);
                    }
                    else {
                        Console.WriteLine("Get Message does not contain /jsonrpc?request");
                        message.isValidURL = false;
                    }
                    break;
                case MessageType.PostMessage:
                    if (firstLineSplit[1] == "/jsonrpc") {
                        //jsonrpc should be in the root directory, therefore the Host should only be /jsonrpc
                        message.isValidURL = true;
                        //get the rest of the body
                        string httpBody = string.Empty;
                        byte[] bodyBuffer = new byte[10000];
                        string StrLength = string.Empty;
                        int contentLength = 0;
                        foreach (string line in lines) {
                            if (line.Contains("content-length")) {
                                string[] splitLine = line.Split(':');
                                StrLength = splitLine[1];
                                break;
                            }
                        }
                        if (StrLength != null) {
                            try {
                                contentLength = int.Parse(StrLength);
                            }
                            catch {
                                Console.WriteLine("Failed to parse content-length");
                            }
                        }
                        else {
                            Console.WriteLine("content-length header not found");
                        }
                        int bodyReadResult = sock.Receive(bodyBuffer);
                        if (bodyReadResult != contentLength) {
                            Console.WriteLine("Failed to read all of HTTP body, probably sent as a multipart");
                        }
                        httpBody = Encoding.ASCII.GetString(bodyBuffer, 0, bodyReadResult);

                        HttpArgs.isValidPostArgs(httpBody, out message.isValidPostArgs, out message.httpArgs);
                    }
                    else {
                        message.isValidURL = false;
                    }
                    break;
                default:
                    message.isValidURL = false;
                    break;
            }
            //GET HTTP version
            tmpLineSplit = firstLineSplit[2].Split('/');
            if (tmpLineSplit.Length == 0) {
                log.logWarning("Recieved Message first line contained no HTTP Version.");
                message.HTTPVersion = -1;
            }
            else {
                try {
                    message.HTTPVersion = float.Parse(tmpLineSplit.Last());
                }
                catch (Exception) {
                    log.logWarning("Recieved Message first line contained a non-parsable HTTP Version");
                    message.HTTPVersion = -1;
                }
            }
            return message;
        }
        /// <summary>
        /// Quick method for removing the URL encoding scheme in the HTTP message. No more %22's and %20
        /// </summary>
        /// <param name="message">The URL that needs to be decoded</param>
        /// <returns>Regularly encoded URL string</returns>
        public static string decodeURL(string message) {
            return Uri.UnescapeDataString(message);
        }
    }
}
