namespace Practice_Socets_client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.reseive = (msg) => { Console.WriteLine(msg); };
            client.Connect();
            Console.WriteLine("Введите сообщение серверу (введите 'exit' для выхода):");
            string input;
            while ((input = Console.ReadLine()) != "exit")
            {
                client.Send(input);
            }

            client.CloseSock();


        }
    }
}
