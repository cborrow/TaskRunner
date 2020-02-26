using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TaskRunner.Logging
{
    public class TaskLog : FileLog
    {
        static TaskLog instance;
        public static TaskLog Instance
        {
            get { return instance; }
        }

        public TaskLog() : base()
        {
            instance = this;
        }
    }
}
