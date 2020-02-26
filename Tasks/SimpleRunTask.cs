using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace TaskRunner.Tasks
{
    public class SimpleRunTask : TaskBase
    {
        bool runAsAdmin;
        public bool RunAsAdmin
        {
            get { return runAsAdmin; }
            set { runAsAdmin = value; }
        }

        string path;
        [TaskControlOptions(TaskControlType.OpenFileTextBox)]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        string arguments;
        public string Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        }

        public SimpleRunTask()
        {
            this.Name = "Run a program";
            this.Description = "Runs a program with stated arguments";

            runAsAdmin = false;
        }

        public override bool Run()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.Arguments = this.Arguments;
            psi.FileName = this.Path;

            if (runAsAdmin)
                psi.Verb = "runas";

            if (WaitForTaskExit)
            {
                Process p = Process.Start(psi);

                while (!p.HasExited)
                {
                    p.WaitForExit(5000);
                }

                if (p.ExitCode == 0)
                    return true;
            }
            else
            {
                Thread t = new Thread(new ThreadStart(delegate ()
                {
                    Process p = Process.Start(psi);
                    p.Start();
                }));
                t.Start();
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Path))
            {
                if (!string.IsNullOrEmpty(Arguments))
                    return string.Format("{0} [{1} {2}]", Name, System.IO.Path.GetFileName(Path), Arguments);
                else
                    return string.Format("{0} [{1}]", Name, System.IO.Path.GetFileName(Path));
            }
            return base.ToString();
        }
    }
}
