using System;
using Newtonsoft.Json;

namespace tcp_transfer.Dtos
{
    [Serializable]
    public class packet_info
    {
        public string fileName {get; set;}
        public string fileExtension {get; set;}
        public long fileSize{get; set;}
        //private static int mod = 1000000007;

        public packet_info(string fileName, string fileExtension, long fileSize)
        {
            this.fileName = fileName;
            this.fileSize = fileSize;
            this.fileExtension = fileExtension;
        }

        public packet_info()
        {
            this.fileSize = 0;
        }


        public float getPercentageRecieved(long current)
        {
            float percent = (float)(this.fileSize - current) / this.fileSize;
            percent *= 100;
            percent = 100 - percent;
            return percent;
        }



        private string getSizeFinished(float fileSizeBytes, string unit)
        {
            string sizeCompleted = TruncateDecimal(fileSizeBytes, 2).ToString() + " " + unit;
            return sizeCompleted;
        }



        private float TruncateDecimal(float value, int precision)
        {
            float step = (float)Math.Pow(10, precision);
            float tmp = (float)Math.Truncate(step * value);
            return tmp / step;
        }



        public string getFileSizeFromPercentage(float percent)
        {
            percent /= 100;
            float fileSizeBytes = (float)(percent * fileSize);
            string sizeCompleted = "bytes";

            if(fileSizeBytes < 1024)
            {
                sizeCompleted = getSizeFinished(fileSizeBytes, "Bytes");
            }
            else if(fileSizeBytes / 1024 < 1000)
            {
                sizeCompleted = getSizeFinished(fileSizeBytes / 1024, "Kb");
            }
            else if(fileSizeBytes / (1024 * 1024) < 1000)
            {
                sizeCompleted = getSizeFinished(fileSizeBytes / (1024 * 1024), "Mb");
            }
            else if((fileSizeBytes) / (1024 * 1024 * 1024) < 1000)
            {
                sizeCompleted = getSizeFinished(fileSizeBytes / (1024 * 1024 * 1024), "Gb");
            }
            else{

            }
            return sizeCompleted;
        }


        public string get_packet_info_json(packet_info _packet)
        {
            string json_data =  JsonConvert.SerializeObject(_packet);
            return json_data;
        }



        public string get_packet_info_json(string file_name, string file_extension, int file_size)
        {
            packet_info _packet = new packet_info(file_name, file_extension, file_size);
            string json_data =  JsonConvert.SerializeObject(_packet);
            return json_data;
        }
    }
}