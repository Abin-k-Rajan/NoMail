using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace tcp_transfer.connnection
{
    public class Connection
    {
        public string connection_string;
        public UInt16 port;
        public IPAddress[] address;
        private static int total_ip = 0;

        public Connection()
        {
            connection_string = string.Empty;
            port = 5000;
            address = new IPAddress[Dns.GetHostEntry(Dns.GetHostName()).AddressList.Length];
        } 

        public Connection(int client)
        {
            if(client > 0)
            {
                Console.Write("Please enter the IP address : ");
                connection_string = Console.ReadLine().ToString();
                set_port();
            }
        }

        public void set_connection_params()
        {
            try{                
                int i = 0;
                Console.WriteLine("\n"+"Please choose your IP : ");
                foreach(IPAddress str in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if(str.AddressFamily == AddressFamily.InterNetwork)
                    {
                        address[i++] = str;
                        Console.WriteLine(i + ". "+ str.ToString());
                    }
                }
                total_ip = i;
                set_connection_ip();
                set_port();
                Console.WriteLine("Connection Parameters set as "+connection_string+":"+port);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + "Please choose the right number!");
                set_connection_ip();
            }
        }



        private void set_connection_ip()
        {
            try
            {
                int i = 0;
                Console.Write("\n[*] Please Enter the ID to establish connection : ");
                i = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();
                i--;
                if(i <= 0)
                {
                    i = 0;
                }
                if(i >= total_ip)
                {
                    throw new IndexOutOfRangeException("IP Address not in the list. ");
                }
                connection_string = address[i].ToString();
            }
            catch(IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message+"Please enter the correct index.");
                set_connection_ip();
            }

        }

        private void set_port()
        {
            try
            {
                Console.Write("Please Enter the port number [Number] : ");
                port = Convert.ToUInt16(Console.ReadLine());
                if(port < 0)
                {
                    throw new Exception("Port number cannot be less than 0 or greater that 65536");
                }
            }
            catch(SocketException ex)
            {
                Console.WriteLine("Port already in use, please use a different port." + ex);
                set_port();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + "Please Enter the port again. Press ctrl + C to exit!");
                set_port();
            }
            
        }


        public UInt16 set_new_port()
        {
            set_port();
            return port;
        }
    }
}