using System;
using System.Net;
using System.Net.Sockets;

namespace IP_Address
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");/// локальна адреса вашого хост

            //for (int i = 1; i < 1024; i++)////діапазон від 1 до 1023 системні зарезервоані порти
            //{
            while (true)
            {
                Console.WriteLine("Checking port {0}", 13);
                try
                {

                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 13);
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(ipEndPoint);

                    Console.WriteLine("Port {0} is listening", 13);

                    Console.ReadLine();
                }
                catch (SocketException ignored)
                {
                    //if (ignored.ErrorCode == 10061) // отказ сервера
                    Console.WriteLine(ignored.Message);
                }
            }
        }
    }
}
