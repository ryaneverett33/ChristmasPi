using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;

namespace Server
{
    class Client
    {
        public static void Main(String[] args)
        {
            TcpClient client = new TcpClient();
            string ip = "";
            bool propIP = false;
            int port = -1;
            bool propPort = false;
            string Valid952HostnameRegex = "((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";
            Console.WriteLine("IP Address: ");
            do
            {
                string tmp = Console.ReadLine();
                Match m = Regex.Match(tmp, Valid952HostnameRegex);
                if (m.Success)
                {
                    ip = tmp;
                    propIP = true;
                }
                else
                {
                    Console.WriteLine("Invalid Address, try again");
                }
            }
            while (!propIP);
            Console.WriteLine("Port Number: ");
            do
            {
                string tmp = Console.ReadLine();
                int alsotmp = -1;
                bool canContinue = false;
                try
                {
                    alsotmp = int.Parse(tmp);
                    canContinue = true;
                }
                catch (Exception e)
                {
                    canContinue = false;
                }
                if (canContinue)
                {
                    if (alsotmp <= 65535 && alsotmp >= 0)
                    {
                        port = alsotmp;
                        propPort = true;
                    }
                }
            }
            while (!propPort);
            Console.WriteLine("Connecting to server");
            Thread.Sleep(500);
            bool tryAgain = false; 
            do
            {
                tryAgain = false;
                try
                {
                    client.Connect(ip, port);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Unable to Connect to server");
                    Console.WriteLine("Try Again?");
                    string answer = Console.ReadLine();
                    if (String.Equals(answer, "yes",StringComparison.CurrentCultureIgnoreCase) || 
                        String.Equals(answer, "y",StringComparison.CurrentCultureIgnoreCase))
                    {
                        tryAgain = true;
                    }
                }
            }
            while (!tryAgain);
            if (client.Connected)
            {
                bool canContinue = true;
                Console.WriteLine("Client Successfully connected");
                Console.WriteLine("Enter data to be sent to server");
                Console.WriteLine("Type Esc to quit");
                do
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        canContinue = false;
                        break;
                    }
                    else
                    {
                        NetworkStream stream = client.GetStream();
                        string data = Console.ReadLine();
                        data = key.KeyChar + data + "\n";
                        byte[] bytes = Encoding.UTF8.GetBytes(data);
                        stream.Write(bytes, 0, bytes.Count());
                        client.Close();
                        Thread.Sleep(500);
                        for (int i = 0; i < 5; i ++)
                        {
                            Thread.Sleep(500);
                            Console.WriteLine("Reconnecting");
                            try
                            {
                                client = new TcpClient();
                                client.Connect(ip, port);
                                if (client.Connected)
                                {
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Console.WriteLine("failed to connected at attempt: " + i);
                                if (i == 4)
                                {
                                    canContinue = false;
                                }
                            }
                        }
                    }
                }
                while (canContinue);
            }
            else
            {
                Console.WriteLine("Client is not connected, closing applicatiion");
            }
            try
            {
                client.Close();
            }
            catch (Exception e)
            {

            }
        }
    }
}
