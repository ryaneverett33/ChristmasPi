using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Server
{
    class Program
    {
        int tcpPort = 1215;
        int udpPort = 12150;
        bool quit = false;
        public void tcpServer()
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, tcpPort);
            listener.Start();
            while(true)
            {
                Socket incoming = listener.AcceptSocket();
                byte[] temp = new byte[1024];
                int tempLen = incoming.Receive(temp);
                byte[] buffer = new byte[tempLen];
                Array.Copy(temp, buffer, tempLen);
                string contents = System.Text.Encoding.UTF8.GetString(buffer);
                Console.Write(contents);
                incoming.Send(Encoding.ASCII.GetBytes(contents));
                incoming.Close();
            }
            
        }
        static void Main(string[] args)
        {
            Program p = new Program();
            Thread tcpServer = new Thread(new ThreadStart(() => p.tcpServer()));
            tcpServer.Start();
            while(!p.quit)
            {
                
            }
        }
    }
}
