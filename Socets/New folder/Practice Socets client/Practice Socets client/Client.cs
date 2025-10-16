
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Practice_Socets_client
{
    /*Разработайте два консольных приложения, использующих сокеты. Одно приложение — сервер, второе — клиент. Клиентское приложение посылает приветствие серверу. Сервер отвечает. И клиент, и сервер отображают полученное сообщение. Пример вывода:*/
    internal class Client
    {
        Socket sock;
        public Action<string> reseive;

        public void CloseSock()
        {
            try
            {
                sock?.Shutdown(SocketShutdown.Both);
            }
            catch 
            {
            //nothing
            }
            sock?.Close();

        }
        public void Connect()
        {
            try
            {

                IPAddress ipAdress = IPAddress.Loopback;//our ip
                                                        // With the correct constructor usage:
                IPEndPoint ipEndPoint = new IPEndPoint(ipAdress /* IP-адрес */, 49152 /* порт */);

                sock = new Socket(AddressFamily.InterNetwork /*схема адресации*/, SocketType.Stream /*тип сокета*/, ProtocolType.Tcp /*протокол*/);
                sock.Connect(ipEndPoint);
                byte[] msg = Encoding.Default.GetBytes(Dns.GetHostName() /* имя узла локального компьютера */);// конвертируем строку, содержащую имя хоста, в массив байтов
                int bytesSent = sock.Send(msg); // отправляем серверу сообщение через сокет
                Console.WriteLine("Клиент " + Dns.GetHostName() + " установил соединение с " + sock.RemoteEndPoint?.ToString());

                ///
                Thread receiveThread = new Thread(Recieve);
                receiveThread.IsBackground = true;
                receiveThread.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());

            }

        }

        public void Send(object msg_)
        {
            try
            {
                if (sock == null) { return; }
                string messenge = msg_.ToString()!;
                byte[] msg = Encoding.Default.GetBytes(messenge!);
                //Thread.Sleep(1000);

                int bytesSent = sock.Send(msg); // отправляем серверу сообщение через сокет
                if (messenge.IndexOf("<end>") > -1) // если клиент отправил эту команду, то принимаем сообщение от сервера
                {
                    byte[] bytes = new byte[1024];
                    int bytesRec = sock.Receive(bytes); // принимаем данные, переданные сервером. Если данных нет, поток блокируется
                    Console.WriteLine("Сервер (" + sock.RemoteEndPoint.ToString() + ") ответил: " + Encoding.Default.GetString(bytes, 0, bytesRec) /*конвертируем массив байтов в строку*/);

                }
                Console.WriteLine(bytesSent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //finally
            //{
            //    CloseSock();
            //}
        }

        public void Recieve()
        {
            try
            {
                //string client = null;
                string data = null;
                byte[] bytes = new byte[1024];// max amount to transfer data buffer

                /*
                 * // Получим от клиента DNS-имя хоста.
                // Метод Receive получает данные от сокета и заполняет массив байтов, переданный в качестве аргумента
                //int bytesRec = sock.Receive(bytes); // Возвращает фактически считанное число байтов
                //client = Encoding.Default.GetString(bytes, 0, bytesRec); // конвертируем массив байтов в строку
                ///client += "(" + handler.RemoteEndPoint.ToString() + ")";////Возвращает удаленную конечную точку.
                //sock.ReceiveTimeout=5000;*/
                while (true)
                {
                    int bytesRec = sock.Receive(bytes);
                    if (bytesRec == 0)
                    {
                        //sock.Shutdown(SocketShutdown.Both);
                        //sock.Close();
                        break;
                    }
                    data = Encoding.Default.GetString(bytes, 0, bytesRec); // конвертируем массив байтов в строку     
                    reseive?.Invoke(data);
                    Console.WriteLine("Сервер ответил " + data);
                }
            }
            catch (SocketException)
            {
                //nothing all in fin
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //finally
            //{
            //    CloseSock();
                
            //}
        }



    }
}
