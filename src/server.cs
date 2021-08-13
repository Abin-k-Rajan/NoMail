using System;
using tcp_transfer.connnection;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using tcp_transfer.Dtos;
using tcp_transfer.src.interfaces;

namespace tcp_transfer.src
{
    public class server_class
    {
        private static Connection _connection;
        private static Thread[] threads;
        private static int MAX = 10;
        private static string address;
        private static UInt16 port;  
        private static string[] connected_devices = new string[MAX];
        private static TcpClient[] connected_clients = new TcpClient[MAX];
        private static packet_info _packet_info;
        private static console _console;
        private static string recievePath;


        public server_class()
        {
            _connection = new Connection();
            _connection.set_connection_params();
            address = _connection.connection_string;
            port = _connection.port;
            threads = new Thread[MAX];
            _console = new console();
            recievePath = "C:\\NoMail\\Recieved\\";
        }




        public void start_server()
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Parse(address), port);
                listener.Start();
                _console.printMessage("[.]Waiting for connection on port "+port+" , address "+address);
                int i = 0;
                while(true)
                {
                    TcpClient _client = listener.AcceptTcpClient();
                    _console.printMessage("[.]Socket connected successfully "+_client.GetType());
                    if(i == MAX)
                    {
                        byte[] buff = new byte[1024];
                        continue;
                    }
                    connected_clients[i] = _client;
                    threads[i] = new Thread(() => Connect(_client, i));
                    threads[i].Start();
                    i++;
                }
            }
            catch(SocketException)
            {
                _console.printError("Port is already in use, please use a different port.");
                port = _connection.set_new_port();
                start_server();
            }
            catch(Exception ex)
            {
                _console.printError(ex.ToString());
            }
        }


        private static void path_handle()
        {
            if(Directory.Exists(recievePath))
            {
                return;
            }
            Directory.CreateDirectory(recievePath);
        }





        private static void Connect(TcpClient _client, int index)
        {
            FileHandle fh;
            int count = 0;
            byte[] buffer = new byte[512];
            NetworkStream _stream = _client.GetStream();
            buffer = new byte[_client.ReceiveBufferSize];
            _stream.Read(buffer, 0, _client.ReceiveBufferSize);
            string message = Encoding.ASCII.GetString(buffer);
            long bytesRecieved = 0;
            packet _packet;
            

            _packet_info = JsonConvert.DeserializeObject<packet_info>(message);

            connected_devices[index] = _client.Client.RemoteEndPoint.ToString();
            path_handle();
            fh = new FileHandle(recievePath + _packet_info.fileName);

            _console.printMessage("Recieving file " + _packet_info.fileName + " [ " + _packet_info.getFileSizeFromPercentage(100) + " ] ");


            send_acknowledge("ACK", _stream);
            try{
                
                while(true)
                {    
                    buffer = new byte[1024];
                    _stream.Read(buffer, 0, 1024);
                    _packet = JsonConvert.DeserializeObject<packet>(Encoding.ASCII.GetString(buffer));
                    send_acknowledge("ACK", _stream);


                    _stream.Read(buffer, 0, _packet.bufferSize);
                    if(count % 1024 == 0)
                    {
                        _console.setProgrogress(_packet_info.getPercentageRecieved(count * 1024));
                    }

                    count++;
                    
                    fh.FileWrite(buffer, _packet.bufferSize);
                    bytesRecieved += _packet.bufferSize;


                    if(Encoding.ASCII.GetString(buffer) == "exit")
                    {
                        break;
                    }
                    send_acknowledge("ACK", _stream);
                }
            }
            catch(Exception)
            {
                _client.Dispose();
                _client.Close();
                _console.setProgrogress(100);
                fh.FileSave();
                if(bytesRecieved == _packet_info.fileSize){
                    _console.printMessage(_packet_info.fileName + " recieved. File saved with no error. ");
                }
                else{
                    _console.printMessage(_packet_info.fileName + " was not recieved fully. Consider recieving file again. ");
                    _console.printMessage("Bytes recieved : "+bytesRecieved + " File size in bytes : " + _packet_info.fileSize);
                }
            }
        }




        private static int get_device_index(string ip_address)
        {
            int i = 0;
            foreach(string x in connected_devices)
            {
                if(x == ip_address)
                {
                    return i;
                }
                i++;
            }
            return 0;
        }

        private static int reroute_string_message(string json_data, int index)
        {
            return 0;
        }


        private static void send_acknowledge(string msg, NetworkStream _stream)
        {
            _stream.Write(Encoding.ASCII.GetBytes("ACK"),0, msg.Length);
        }
    }
}