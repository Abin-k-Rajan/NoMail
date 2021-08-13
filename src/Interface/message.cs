using System;

namespace tcp_transfer.src.interfaces
{
    public interface message_interface
    {
        void printError(string message);
        void printMessage(string message);
        void printMessageInline(string message);
        void printMessageBox(string message);
    }


    public class console : message_interface
    {
        private static int current_progress = 0;

        public void printError(string message)
        {
            Console.Error.WriteLine(message);
        }

        public void printMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void printMessageInline(string message)
        {
            Console.Write(message);
        }

        public void printMessageBox(string message)
        {
            printMessage(message);
        }

        public void setProgrogress(float percent)
        {
            int percent_int = (int)Math.Ceiling(percent);
            for(int i = current_progress; i < percent_int - current_progress; i+=10)
            {
                Console.Write("=");
            }
            if(percent == 100)
            {
                Console.WriteLine();
                current_progress = 0;
            }
        }
    }
}