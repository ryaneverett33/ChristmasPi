using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using ChristmasServer.Methods;

namespace ChristmasServer {
    class Program {
        ConfigFile config;
        TcpListener listener;
        Thread serverThread;
        Logger log;
        MethodManager methMan;
        static void Main(string[] args) {
            Program p = new Program();
            string configFileLoc;
            bool canContinue;
            bool isProgramAlive = true;
            parseArgs(args, out configFileLoc, out canContinue);
            if (canContinue) {
                p.log = new Logger();   //TODO Log all the data
                try {
                    string FileText = File.ReadAllText(configFileLoc);
                    p.config = JsonConvert.DeserializeObject<ConfigFile>(FileText);
                }
                catch (FileNotFoundException) {
                    Console.WriteLine("[ERROR] Could not find the config file to parse.");
                    isProgramAlive = false;
                    return;
                }
                catch (JsonReaderException e) {
                    Console.WriteLine("[ERROR] An JSONReaderException was thrown when attempting to parse the config file.");
                    Console.WriteLine(e.Message);
                    isProgramAlive = false;
                    return;
                }
                catch (Exception e) {
                    Console.WriteLine("[ERROR] An Exception was thrown during startup");
                    Console.WriteLine(e.Message);
                    isProgramAlive = false;
                    return;
                }
                Console.WriteLine("Attempting to open a new Server on port: {0}", p.config.server_port);
                try {
                    p.listener = new TcpListener(IPAddress.Any,p.config.server_port);
                    p.listener.Start();
                    p.serverThread = new Thread(new ThreadStart(() => serverListen(p)));
                    p.serverThread.Start();
                }
                catch (Exception e) {
                    Console.WriteLine("[Error] Failed to start Server");
                    Console.WriteLine(e.Message);
                    if (p.serverThread != null && p.serverThread.IsAlive) {
                        p.serverThread.Abort();
                    }
                    isProgramAlive = false;
                    return;
                }
                p.methMan = new MethodManager();
                //Listening to kill
                Console.WriteLine("Listening for kill");
                while(isProgramAlive) {
                    ConsoleKeyInfo info = Console.ReadKey(true);
                    if (info.Key == ConsoleKey.Escape || (info.Key == ConsoleKey.C && info.Modifiers == ConsoleModifiers.Control)) {
                        //Quit key called, kill server
                        p.serverThread.Abort();
                        isProgramAlive = false;
                    }
                }
            }
            else {
                Console.WriteLine("Arguments parsing failed, can't continue");
                return;
            }
        }
        static void parseArgs(string[] args, out string configFileLoc, out bool canContinue) {
            bool canGo = true;
            bool fileSet = false;
            string fileloc = null;                     //Appeasing the compiler
            foreach (string arg in args) {
                if (arg.Equals("-h") || arg.Equals("--h")) {
                    printHelp();
                    canContinue = false;
                    configFileLoc = null;
                    canGo = false;
                    printHelp();
                    break;
                }
                else if (File.Exists(arg)) {
                    configFileLoc = arg;
                    fileloc = arg;
                    fileSet = true;
                }
                else {
                    Console.WriteLine("Invalid Argument passed. Type -h to see the help screen");
                    canGo = false;
                    configFileLoc = null;
                    canContinue = false;
                    break;
                }
            }
            if (canGo && fileSet) {
                canContinue = true;
                configFileLoc = fileloc;            //Appeasing the compiler
            }
            else if (!fileSet) {
                canContinue = false;
                configFileLoc = null;
                printHelp();
            }
            else {
                configFileLoc = null;
                canContinue = false;
            }
        }
        static void printHelp() {

        }
        static void serverListen(Program p) {
            try {
                while (true) {
                    TcpClient recieved = p.listener.AcceptTcpClient();
                    //handle recieved info
                    string message = "";
                    NetworkStream recievedStream = recieved.GetStream();
                    byte[] buffer = new byte[10000];
                    int readResult = recievedStream.Read(buffer, 0, 10000);
                    if (readResult > 0) {
                        message = Encoding.UTF8.GetString(buffer,0, readResult);
                        message = ReceivedMessage.decodeURL(message);        //Get rid of %22 and %20
                        handle(ReceivedMessage.parseHTTP(message, p.log), recievedStream, p);
                        //byte[] toSend = Encoding.UTF8.GetBytes("Recieved");
                        //recievedStream.Write(toSend, 0, toSend.Length);
                    }
                    else {
                        //failed to read from stream
                        Console.WriteLine("failed to read from stream");
                    }
                    recieved.Close();
                }
            }
            catch (ThreadInterruptedException) {

            }
            catch (ThreadAbortException) {

            }
        }
        static void handle(ReceivedMessage message, NetworkStream netStream, Program p) {
            switch (message.type) {
                case ReceivedMessage.MessageType.GetMessage:
                    Console.WriteLine("MessageType: {0}, isValidURL: {1}, HttpVer: {2}", message.type, message.isValidURL, message.HTTPVersion);
                    Console.WriteLine("isValidMethod: " + p.methMan.isValidMethod(message.httpArgs.method, ReceivedMessage.MessageType.GetMessage));
                    break;
                case ReceivedMessage.MessageType.PostMessage:
                    Console.WriteLine("MessageType: {0}, isValidURL: {1}, isValidArgs: {2}, HttpVer: {3}", message.type, message.isValidURL, message.isValidPostArgs, message.HTTPVersion);
                    break;
                case ReceivedMessage.MessageType.Unknown:
                    Console.WriteLine("MessageType: {0}, isValidURL: {1}, HttpVer: {2}", message.type, message.isValidURL, message.HTTPVersion);
                    break;
            }
            byte[] outText = Encoding.UTF8.GetBytes(message.rawText);
            netStream.Write(outText,0,outText.Length);
        }
    }
}
