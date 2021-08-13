using System;
using tcp_transfer.connnection;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using tcp_transfer.Dtos;
using tcp_transfer.src.interfaces;

namespace tcp_transfer.src
{

    public class client_class
    {
        private static Connection _connection;
        private static string address;
        private static int port;
        private static console _console;




        public client_class()
        {
            _connection = new Connection(1);
            address = _connection.connection_string;
            port = _connection.port;
            _console = new console();
        }



        private static bool recieve_acknowledge_message(NetworkStream stream)
        {
            try
            {
                byte[] ack = new byte[10];
                stream.Read(ack, 0, ack.Length);
                return true;
            }
            catch(Exception ex)
            {
                _console.printMessage(ex.ToString());
                stream.Close();
            }
            return false;
        }


        public void connect()
        {
            TcpClient client = new TcpClient(address, port);
            _console.printMessageInline("Please enter the file path : ");
            string file_name = Console.ReadLine();
            FileHandle fh = new FileHandle(file_name);
            NetworkStream stream = client.GetStream();
            byte[] _message = new byte[1048];
            int count = 0;

            fh.FileOpen();

            packet_info _packet_info = fh.GetFileInformation();
            packet _packet = new packet();


            string json_data = _packet_info.get_packet_info_json(_packet_info);
            _message = Encoding.ASCII.GetBytes(json_data);
            stream.Write(_message, 0, _message.Length);
            recieve_acknowledge_message(stream);

            _console.printMessage("Server is ready to recieve");
            _console.printMessage("Sending file " + _packet_info.fileName + " [ " + _packet_info.getFileSizeFromPercentage(100) + " ] ");


            while(true)
            {  
                _message = new byte[1024];
                int len = fh.FileRead(_message);

                stream.Write(_packet.GetPacket(len), 0, _packet.GetPacket(len).Length);
                recieve_acknowledge_message(stream);

                if(len < 0)
                {
                    break;
                }
                if(count % 1024 == 0)
                {
                    _console.setProgrogress(_packet_info.getPercentageRecieved(count * 1024));
                }

                count++;

                stream.Write(_message, 0, len);
                if(recieve_acknowledge_message(stream) == false)
                {
                    _console.printMessage("Server is Busy, closing connection");
                    stream.Close();
                    return;
                }
            }
            _console.setProgrogress(100);

        }
    }

}