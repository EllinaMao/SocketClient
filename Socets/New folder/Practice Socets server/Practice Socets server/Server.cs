using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Practice_Socets_server
{
    public class Server
    {
        string ServerAnswer = "default";
        public delegate void ServerHandler(string message);
        public event ServerHandler? ServerRecieveEvent;


        public void ThreadForReceive(object param)////дочерний поток занимается общением с клиентом
        {
            Socket handler = (Socket)param;//////сокет которій поймал сервер
            try
            {
                string client = null;
                string data = null;
                byte[] bytes = new byte[1024];//buffer

                // Получим от клиента DNS-имя хоста.
                // Метод Receive получает данные от сокета и заполняет массив байтов, переданный в качестве аргумента
                int bytesRec = handler.Receive(bytes); // Возвращает фактически считанное число байтов
                client = Encoding.Default.GetString(bytes, 0, bytesRec); // конвертируем массив байтов в строку
                client += "(" + handler.RemoteEndPoint.ToString() + ")";////Возвращает удаленную конечную точку.
                while (true)
                {
                    bytesRec = handler.Receive(bytes); // принимаем данные, переданные клиентом. Если данных нет, поток блокируется
                    if (bytesRec == 0)
                    {
                        return;
                    }
                    data = Encoding.Default.GetString(bytes, 0, bytesRec); // конвертируем массив байтов в строку                  
                    ServerRecieveEvent?.Invoke(data);////Візов собітія
                    if (data.IndexOf("<end>") > -1) // если клиент отправил эту команду, то заканчиваем обработку сообщений
                    {
                        break;
                    }

                    string unsw = "I am server. I receive from client: " + data;
                    byte[] unswB = Encoding.Default.GetBytes(unsw);
                    handler.Send(unswB);
                }
                string theReply = "Я завершаю обработку сообщений";
                byte[] msg = Encoding.Default.GetBytes(theReply); // конвертируем строку в массив байтов
                handler.Send(msg); // отправляем клиенту сообщение
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                try
                {
                    handler.Shutdown(SocketShutdown.Both);
                }
                catch { }
                handler.Close();
            }
        }

             //  ожидать запросы на соединение будем в отдельном потоке
        public void ThreadForAccept()
        {
            try
            {
                // установим для сокета адрес локальной конечной точки
                // уникальный адрес для обслуживания TCP/IP определяется комбинацией IP-адреса хоста с номером порта обслуживания
                IPEndPoint ipEndPoint = new IPEndPoint(
                    IPAddress.Any /* Предоставляет IP-адрес, указывающий, что сервер должен контролировать действия клиентов на всех сетевых интерфейсах.*/,
                    49152 /* порт */);

                // потоковый сокет
                Socket sListener = new Socket(AddressFamily.InterNetwork /*схема адресации*/, SocketType.Stream /*тип сокета*/, ProtocolType.Tcp /*протокол*/ );
                /* Значение InterNetwork указывает на то, что при подключении объекта Socket к конечной точке предполагается использование IPv4-адреса.
                   SocketType.Stream поддерживает надежные двусторонние байтовые потоки в режиме с установлением подключения, без дублирования данных и 
                   без сохранения границ данных. Объект Socket этого типа взаимодействует с одним узлом и требует предварительного установления подключения 
                   к удаленному узлу перед началом обмена данными. Тип Stream использует протокол Tcp и схему адресации AddressFamily.
                 */

                // Чтобы сокет клиента мог идентифицировать потоковый сокет TCP, сервер должен дать своему сокету имя
                sListener.Bind(ipEndPoint); // Свяжем объект Socket с локальной конечной точкой.
                //
                // Установим объект Socket в состояние прослушивания.
                sListener.Listen(10 /* Максимальная длина очереди ожидающих подключений.*/ );
                while (true)
                {

                    Socket handler = sListener.Accept();////Socket handler  инфа от клиента кот  подключился

                    // обслуживание текущего запроса будем выполнять в отдельном потоке
                    Thread thread = new Thread(new ParameterizedThreadStart(ThreadForReceive));
                    thread.IsBackground = true;
                    thread.Start(handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Сервер: " + ex.Message);
            }

        }

    }
    }

