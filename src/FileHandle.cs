using System.IO;
using System.Text;
using System;
using tcp_transfer.src.errors;
using tcp_transfer.Dtos;

namespace tcp_transfer.src
{
    public class FileHandle
    {
        private string file_path;
        private int file_size;
        private FileAttributes attributes;
        private static FileStream fs;
        private static Error err;
        private static BinaryWriter _binaryWriter;
        private static BinaryReader _binaryReader;
        private static bool end_check = false;
        

        

        public FileHandle(string file_path, int file_size, FileAttributes attributes)
        {
            this.file_path = file_path;
            this.file_size = file_size;
            this.attributes = attributes;
            init_err();
        }

        public FileHandle(string file_path, int file_size)
        {
            this.file_path = file_path;
            this.file_size = file_size;
            init_err();
        }

        public FileHandle(string file_path)
        {
            this.file_path = file_path;
            init_err();
        }


        private void init_err()
        {
            err = new Error();
        }



        public int FileCreate()
        {
            fs = File.Create(file_path);
            _binaryWriter = new BinaryWriter(fs);
            if(fs != null)
            {
                end_check = false;
                return err.SUCCESS;
            }
            return -err.EIO;
        }




        public int FileOpen()
        {
            fs = new FileStream(file_path, FileMode.Open);
            _binaryReader = new BinaryReader(fs);
            if(fs != null)
            {
                end_check = false;
                return err.SUCCESS;
            }
            return -err.EIO;
        }




        public int FileWrite(byte[] buffer, int len)
        {
            int res = 0;
            if(fs == null)
            {
                res = FileCreate();
                if(res < 0)
                {
                    return res;
                }
            }
            _binaryWriter.Write(buffer, 0, len);
            return err.SUCCESS;
        }




        public int FileRead(byte[] buffer)
        {
            int res = 0;
            if(end_check == true)
            {    
                return -err.EINVARG;
            }
            if(fs == null)
            {
                res = FileCreate();
                if(res < 0)
                {
                    return -err.EINVARG;
                }
            }
            int len = 1024;
            res = fs.Read(buffer, 0, len);
            if(_binaryReader.BaseStream.Position == _binaryReader.BaseStream.Length)
            {
                end_check = true;
                len = (int)_binaryReader.BaseStream.Length % 1024;
            }
            return len;
        }


        private int getLength(int maxLen)
        {
            byte[] temp = new byte[2];
            int count = 0;
            while(fs.Read(temp, 0, 1) > 0 && count < maxLen)
            {
                count++;
            }
            fs.Seek(-count, SeekOrigin.Current);
            Console.WriteLine(count);
            return count;
        }


        public int FileSave()
        {
            //  HAVE TO IMPLEMENT FILE ATTRIBUTES
            fs.Close();
            return err.SUCCESS;
        }


        public packet_info GetFileInformation()
        {
            FileInfo fileInfo = new FileInfo(file_path);
            packet_info _packet_info = new packet_info();
            _packet_info.fileName = fileInfo.Name;
            _packet_info.fileExtension = fileInfo.Extension;
            _packet_info.fileSize = fileInfo.Length;
            return _packet_info;
        }
    }
}