using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using TaskRunner.Logging;

namespace TaskRunner.Tasks
{
    public class FilesDeleteTask : TaskBase
    {
        string pathToDelete;
        [TaskControlOptions(TaskControlType.OpenFileTextBox)]
        public string PathToDelete
        {
            get { return pathToDelete; }
            set { pathToDelete = value; }
        }

        bool recursiveDelete;
        public bool Recursive
        {
            get { return recursiveDelete; }
            set { recursiveDelete = value; }
        }

        public FilesDeleteTask()
        {
            Name = "Delete file(s)";
            Description = "Deletes one or more files";
        }

        public override bool Run()
        {
            if (recursiveDelete)
            {
                string dir = Path.GetDirectoryName(pathToDelete);
                string sp = Path.GetFileName(pathToDelete);

                TaskLog.Instance.Add("Starting recursive delete of files in " + dir + " that match pattern of " + sp);

                DeleteFiles(dir, sp);
            }
            else
            {
                if (File.Exists(pathToDelete))
                {
                    try
                    {
                        TaskLog.Instance.Add("Deleting file " + pathToDelete);
                        File.Delete(pathToDelete);

                        if (!File.Exists(pathToDelete))
                            return true;
                    }
                    catch (Exception ex)
                    {
                        TaskLog.Instance.Add("Failed to delete the file " + pathToDelete + ", reason given : " + ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return base.Run();
        }

        public override string ToString()
        {
            if(recursiveDelete)
            {
                return string.Format("Recursively delete files that match {0} in {1}", 
                    Path.GetFileName(pathToDelete), Path.GetDirectoryName(pathToDelete));
            }
            else
            {
                return string.Format("Delete the file {0}", pathToDelete);
            }
        }

        protected void DeleteFiles(string path, string searchPattern)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path, searchPattern);

            foreach(string d in dirs)
            {
                DeleteFiles(d, searchPattern);
            }

            foreach(string f in files)
            {
                try
                {
                    TaskLog.Instance.Add("Deleting file " + f);
                    File.Delete(f);
                }
                catch(Exception ex)
                {
                    TaskLog.Instance.Add("Failed to delete the file " + f + ", reason given : " + ex.Message);
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
