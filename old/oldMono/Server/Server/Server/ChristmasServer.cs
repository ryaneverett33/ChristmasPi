using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Server {
    class ChristmasServer {
        int port = -1;
        Logger log = new Logger();              //create a new log file
        Animations animations;
        Manager manager;
        TcpClient client;
        TcpListener listener;
        bool alive = true;
        private void handleArgs(string[] args) {
            if (args.Length > 0) {
                for (int i = 0; i < args.Length; i++) {
                    if (args[i].Equals("port") || args[i].Equals("-p") || args[i].Equals("p")) {
                        string tmp;
                        try {
                            tmp = args[i + 1];
                            port = int.Parse(tmp);
                        }
                        catch (Exception e) { log.logWarning("Failed to read Port argument"); } 
                    }
                }
                //Check if the parameters failed to set, revert to default
                if (port == -1) port = 1225;
            }
            else {
                //Set global parameters to default
                port = 1225;
            }
        }
        private void setupGPIO() {
            // TODO
            // Setup GPIO info
            
        }
        private bool setupServer() {
            // TODO
            // Setup Server info
            try {
                listener = new TcpListener(System.Net.IPAddress.Any, port);
                listener.Start();
                return true;
            }
            catch (Exception e) {
                log.logWarning(e.Message);
                return false;
            }
        }
        private void handle(Socket sock) {
            byte[] data = new byte[1200];
            int recieved;
            try {
                recieved = sock.Receive(data);
            }
            catch {
                return;
            }
            string message = Encoding.UTF8.GetString(data,0,recieved - 1);
            string[] names = manager.getAnimations();
            bool found = false;
            Console.WriteLine(message);
            log.logOK("Recieved message");
            foreach (string name in names) {
                log.writeLine("Animation: "+name);
                if (name.Equals(message) || name == message) {
                    Console.WriteLine("Found!");
                    found = true;
                    break;
                }
            }
            if (found) {
                bool status = manager.playAnimation(message);
                if (!status) {
                    try {
                        sock.Send(Encoding.UTF8.GetBytes("Unable to play animation."));
                    }
                    catch (Exception e) {
                        log.logWarning(e.Message);
                    }
                    log.logError("Unable to play animation: " + message);
                }
                else {
                    Console.WriteLine("Playing " + message);
                    manager.playAnimation(message);
                    try {
                        sock.Send(Encoding.UTF8.GetBytes("Animation successfully started."));
                    }
                    catch (Exception e) {
                        log.logWarning(e.Message);
                    }
                    log.logOK(message + " successfully started");
                }
            }
            else {
                log.logWarning(message);
                try {
                    sock.Send(Encoding.UTF8.GetBytes("Animation not found. Nothing started"));
                }
                catch (Exception e) {
                    log.logWarning(e.Message);
                }
            }
            sock.Disconnect(true);
        }
        public static void Main(string[] args) {
            ChristmasServer cs = new ChristmasServer();                         //Make reference to self
            //Gather properties supplied by arguments
            cs.handleArgs(args);
            cs.log.createNewLog();
            cs.log.writeLine("Opened log for writing");
            //Setup GPIO stuff
            /*if(!cs.setupGPIO()) {
                cs.log.logFatal("Failed to setup GPIO data. Closing");
                cs.alive = false;
                Environment.Exit(-1);
            }*/
            if(!cs.setupServer()) {
                cs.log.logFatal("Failed to setup Server data. Closing");
                cs.alive = false;
                Environment.Exit(1);
            }
            cs.animations = new Animations(cs.log);
            cs.manager = new Manager(cs.log, cs.animations);
            while (cs.alive) {
                try {
                    Socket sock = cs.listener.AcceptSocket();
                    Thread handler = new Thread(new ThreadStart(() => cs.handle(sock)));
                    handler.Start();
                }
                catch (SocketException e) {
                    cs.log.logWarning(e.Message);
                }
            }
        }
    }
}
