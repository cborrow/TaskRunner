using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace TaskRunner.Logging
{
    public class FileLog
    {
        StringBuilder sb;
        Timer timer;

        string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        bool preprendDateTime;
        public bool PreprendDateTime
        {
            get { return preprendDateTime; }
            set { preprendDateTime = value; }
        }

        public FileLog()
        {
            path = string.Empty;
            preprendDateTime = false;

            sb = new StringBuilder();
            timer = new Timer(WriteLogsToFile, null, 1000, 7500);
        }

        public virtual void Add(string message)
        {
            string log = string.Empty;
            if (preprendDateTime)
                log = DateTime.Now.ToString() + " " + message;
            else
                log = message;

            sb.AppendLine(log);
        }

        public void Flush()
        {
            WriteLogsToFile(null);
        }

        protected void WriteLogsToFile(object stateInfo)
        {
            if(!Directory.Exists(System.IO.Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            }

            if(sb.Length > 0)
            {
                try
                {
                    File.WriteAllText(path, sb.ToString());
                    sb.Clear();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Failed to write to log file.\n" + ex.Message);
                }
            }
        }
    }
}
