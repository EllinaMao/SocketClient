using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace IP_Address
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");/// локальна адреса вашого хост

            for (int i = 1; i < 1024; i++)////діапазон від 0 до 1023 системні зарезервоані порти
            {
                Console.WriteLine("Checking port {0}", i);
                try
                {
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, i);
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(ipEndPoint);
                    Console.WriteLine("Port {0} is listening", i);
                }
                catch (SocketException ignored)
                {
                    if (ignored.ErrorCode == 10061) // отказ сервера
                        Console.WriteLine(ignored.Message);
                }
            }
        }
    }
}
