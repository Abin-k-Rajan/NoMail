using System;
using Newtonsoft.Json;
using System.Text;

namespace tcp_transfer.Dtos
{
    [Serializable]
    public class packet
    {
        public int bufferSize = 0;

        public packet(int bufferSize)
        {
            this.bufferSize = bufferSize;
        }

        public packet()
        {

        }


        private static string get_packet_json(packet _packet)
        {
            string json_data =  JsonConvert.SerializeObject(_packet);
            return json_data;
        }



        public byte[] GetPacket(int size)
        {
            packet _packet = new packet(size);
            string _message = get_packet_json(_packet);
            return Encoding.ASCII.GetBytes(_message);
        }
    }
}