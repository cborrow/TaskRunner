using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TaskRunner.Logging
{
    public class ApplicationLog : FileLog
    {
        static ApplicationLog instance;
        public static ApplicationLog Instance
        {
            get { return instance; }
        }

        public ApplicationLog() : base()
        {
            instance = this;
        }
    }
}
