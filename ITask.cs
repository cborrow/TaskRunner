using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunner
{
    public interface ITask
    {
        bool WaitForTaskExit { get; set; }

        string Name { get; set; }
        string Description { get; set; }

        bool Run();
    }
}
