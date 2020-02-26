using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using TaskRunner.Logging;

namespace TaskRunner
{
    public partial class Form1 : Form
    {
        NewTaskDialog newTaskDialog;
        ApplicationLog applicationLog;

        public Form1()
        {
            InitializeComponent();

            newTaskDialog = new NewTaskDialog();
            applicationLog = new ApplicationLog();
            applicationLog.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "TaskRunner", "AppLog_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_"+ DateTime.Now.Year + ".log");
            applicationLog.PreprendDateTime = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            applicationLog.Flush();
            base.OnClosing(e);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(panel1.Visible)
            {
                panel1.Visible = false;
                linkLabel1.Text = "Show Additional Options";
                this.Height -= panel1.Height;
            }
            else
            {
                this.Height += panel1.Height;
                linkLabel1.Text = "Hide Additional Options";
                panel1.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(newTaskDialog.ShowDialog() == DialogResult.OK)
            {
                ITask t = newTaskDialog.Task;

                if(t != null)
                {
                    applicationLog.Add("Created new task " + t.ToString());
                    listBox1.Items.Add(t);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Edit
            ITask selectedTask = (ITask)listBox1.SelectedItem;
            int index = listBox1.SelectedIndex;

            if(selectedTask != null)
            {
                applicationLog.Add("Editing task " + selectedTask.ToString());

                if (newTaskDialog.ShowDialog(selectedTask) == DialogResult.OK)
                {
                    listBox1.Items[index] = newTaskDialog.Task;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Remove
            ITask selectedTask = (ITask)listBox1.SelectedItem;
            int index = listBox1.SelectedIndex;

            if (selectedTask != null)
            {
                applicationLog.Add("Deleted task " + selectedTask.ToString());
                listBox1.Items.Remove(selectedTask);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Import
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Export
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Run

            TaskLog tl = new TaskLog();
            tl.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TaskRunner", 
                "Task Logs", DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".log");
            tl.PreprendDateTime = true;

            for(int i = 0; i < listBox1.Items.Count; i++)
            {
                ITask task = (ITask)listBox1.Items[i];

                if(task != null)
                {
                    applicationLog.Add("Running task " + task.ToString());

                    if (task.Run())
                    {
                        //Log the task result.
                        //Also allow the running task access to either the same log file or a seperate log file
                    }
                }
            }

            tl.Flush();
            MessageBox.Show("All tasks have completed. Check the task log files for any errors", "TaskRunner", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
