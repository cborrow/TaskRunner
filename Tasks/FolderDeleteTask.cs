using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using TaskRunner.Logging;

namespace TaskRunner.Tasks
{
    public class FolderDeleteTask : TaskBase
    {
        string pathToDelete;
        [TaskControlOptions(TaskControlType.FolderTextBox)]
        public string PathToDelete
        {
            get { return pathToDelete; }
            set { pathToDelete = value; }
        }

        bool deleteNonEmpty;
        public bool DeleteEvenIfNotEmpty
        {
            get { return deleteNonEmpty; }
            set { deleteNonEmpty = value; }
        }

        public FolderDeleteTask()
        {
            Name = "Delete a folder";
            Description = "Deletes a folder with option to recursive delete all files";
        }

        public override bool Run()
        {
            if(Directory.Exists(pathToDelete))
            {
                string[] dirs = Directory.GetDirectories(pathToDelete);
                string[] files = Directory.GetFiles(pathToDelete);

                if(deleteNonEmpty)
                {
                    TaskLog.Instance.Add("Deleting directory and all contents : " + pathToDelete);

                    Directory.Delete(pathToDelete, true);
                }
                else
                {
                    if (files.Length > 0 || dirs.Length > 0)
                    {
                        //Fail as directory is not empty and non empty delete flag is not set
                        TaskLog.Instance.Add("Directory is not empty cannot delete as DeleteEvenIfEmpty is set to false : " + pathToDelete);
                    }
                    else
                    {
                        TaskLog.Instance.Add("Deleteing empty directory : " + pathToDelete);
                        Directory.Delete(pathToDelete);
                    }
                }
            }

            return base.Run();
        }
        public override string ToString()
        {
            if(deleteNonEmpty)
            {
                return string.Format("Delete the directory {0} and all of its contents", pathToDelete);
            }
            else
            {
                return string.Format("Delete the empty directory {0}", pathToDelete);
            }
        }
    }
}
