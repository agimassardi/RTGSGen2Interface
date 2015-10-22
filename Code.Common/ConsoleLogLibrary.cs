using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.Common
{
    public class ConsoleLogLibrary
    {
        public string logFilePath { get; private set; }

        public ConsoleLogLibrary(string _logFilePath)
        {
            logFilePath = _logFilePath;
        }

        public void Write(string message)
        {
            using (StreamWriter file = new StreamWriter(logFilePath + "\\log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt", true))
            {
                message = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message;
                Console.WriteLine(message);
                file.WriteLine(message);
            }
        }

        public void WriteErrorLog(string title, OperationResult op)
        {
            string ErrorMessages = "";
            foreach (var message in op.MessageList)
            {
                ErrorMessages += "\t" + message + "\n";
            }
            Write(title + "\n" + ErrorMessages);        
        }

       
    }
}
