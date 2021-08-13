using System;
using tcp_transfer.src;

namespace tcp_transfer
{
    class Program
    {
        private static server_class srv;
        private static client_class cli;
        static void Main(string[] args)
        {
            Console.Write("What do you want to set up?"+"\n1. Recieve\n2. Send\n3. Exit\n[*]Please enter your choice : ");
            UInt16 choice = Convert.ToUInt16(Console.ReadLine());
            int c = choice;
            switch(c)
            {
                case 1: srv = new server_class();
                        srv.start_server();
                        break;
                case 2: cli = new client_class();
                        cli.connect();
                        break;
                case 3: return;
                default:Console.WriteLine("Please Enter a valid option.");
                        break;
                
            }
        }
    }
}
