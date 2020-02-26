using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunner.Tasks
{
    public class TaskBase : ITask
    {
        bool waitForTaskExit;
        public bool WaitForTaskExit
        {
            get { return waitForTaskExit; }
            set { waitForTaskExit = value; }
        }

        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public virtual bool Run()
        {
            return false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
