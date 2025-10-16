namespace Practice_Socets_server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.ServerRecieveEvent += (message) =>
            {
                Console.WriteLine("Сервер получил: " + message);
            };
            Thread acceptThread = new Thread(server.ThreadForAccept);
            acceptThread.IsBackground = true;
            acceptThread.Start();

            Console.WriteLine("Сервер запущен. Нажмите Enter для выхода.");
            Console.ReadLine();
        }
    }
}
