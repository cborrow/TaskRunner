using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace TaskRunner
{
    public partial class NewTaskDialog : Form
    {
        Type taskType;
        ITask task;
        public ITask Task
        {
            get { return task; }
            set
            {
                task = value;
                taskType = value.GetType();
                BuildTaskOptions();
            }
        }

        public NewTaskDialog()
        {
            InitializeComponent();

            LoadTaskList();
        }

        public DialogResult ShowDialog(ITask task)
        {
            Reset();
            this.Task = task;
            return base.ShowDialog();
        }

        public new DialogResult ShowDialog()
        {
            Reset();
            return base.ShowDialog();
        }
        
        public void LoadTaskList()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Namespace, "TaskRunner.Tasks")).ToArray();

            foreach(Type t in types)
            {
                if (t.Name == "TaskBase" || t.Name.StartsWith("<>"))
                    continue;

                comboBox1.Items.Add(t.Name);
            }
        }

        public void Reset()
        {
            //textBox1.Text = string.Empty;
            //textBox2.Text = string.Empty;

            comboBox1.Text = "Please select a task type";
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Size = new Size(575, 21);
            this.Height = 336;
        }

        public string FormatControlText(string name)
        {
            string text = string.Empty;

            for(int i =0; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]) && i > 0)
                    text += " " + name[i];
                else
                    text += name[i];
            }

            return text;
        }

        public void BuildTaskOptions()
        {
            flowLayoutPanel1.Controls.Clear();

            if(taskType != null)
            {
                foreach(PropertyInfo pi in taskType.GetProperties())
                {
                    if (pi.Name == "Name" || pi.Name == "Description")
                        continue;

                    if(pi.PropertyType == typeof(string))
                    {
                        if (IsHiddenControl(pi))
                            continue;

                        Label l = new Label();
                        l.Size = new Size((flowLayoutPanel1.Width / 2) - 35, 25);
                        l.Text = FormatControlText(pi.Name);
                        flowLayoutPanel1.Controls.Add(l);

                        TextBox tb = new TextBox();
                        tb.Name = pi.Name;
                        tb.Text = (string)pi.GetValue(task);
                        tb.Size = new Size((flowLayoutPanel1.Width / 2) + 25, 25);
                        flowLayoutPanel1.Controls.Add(tb);

                        if (HasCustomAttributes(pi))
                        {
                            Button b = new Button();
                            b.Name = pi.Name + "_button";
                            b.Text = "Browse...";
                            b.Tag = pi.Name;
                            b.Size = new Size(75, 26);

                            if (GetTaskControlType(pi) == TaskControlType.OpenFileTextBox)
                            {
                                b.Click += openFileButtonClick;
                            }
                            else if (GetTaskControlType(pi) == TaskControlType.SaveFileTextBox)
                            {
                                b.Click += saveFileButtonClick;
                            }
                            else if (GetTaskControlType(pi) == TaskControlType.FolderTextBox)
                            {
                                b.Click += folderBrowserButtonClick;
                            }

                            flowLayoutPanel1.Controls.Add(b);
                        }
                    }
                    else if(pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(int))
                    {
                        if (IsHiddenControl(pi))
                            continue;

                        Label l = new Label();
                        l.Size = new Size((flowLayoutPanel1.Width / 2) - 35, 25);
                        l.Text = FormatControlText(pi.Name);
                        flowLayoutPanel1.Controls.Add(l);

                        NumericUpDown nup = new NumericUpDown();
                        nup.Name = pi.Name;
                        nup.Size = new Size((flowLayoutPanel1.Width / 2) + 25, 25);
                        nup.Minimum = GetTaskMinValue(pi);
                        nup.Maximum = GetTaskMaxValue(pi);

                        if (pi.PropertyType == typeof(decimal))
                            nup.Value = (decimal)pi.GetValue(task);
                        else if (pi.PropertyType == typeof(int))
                            nup.Value = (int)pi.GetValue(task);

                        flowLayoutPanel1.Controls.Add(nup);
                    }
                    else if(pi.PropertyType == typeof(bool))
                    {
                        if (IsHiddenControl(pi))
                            continue;

                        CheckBox cb = new CheckBox();
                        cb.Name = pi.Name;
                        cb.Text = FormatControlText(pi.Name);
                        cb.Size = new Size((flowLayoutPanel1.Width - 25), 25);
                        cb.Checked = (bool)pi.GetValue(task);

                        flowLayoutPanel1.Controls.Add(cb);
                    }
                }
            }
        }

        private void openFileButtonClick(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                Button b = (Button)sender;
                Control c = flowLayoutPanel1.Controls[(string)b.Tag];

                if (c != null)
                    c.Text = file;
            }
        }

        private void saveFileButtonClick(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string file = saveFileDialog1.FileName;
                Button b = (Button)sender;
                Control c = flowLayoutPanel1.Controls[(string)b.Tag];

                if (c != null)
                    c.Text = file;
            }
        }

        private void folderBrowserButtonClick(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                Button b = (Button)sender;
                Control c = flowLayoutPanel1.Controls[(string)b.Tag];

                if(c != null)
                    c.Text = path;
            }
        }

        public bool HasCustomAttributes(PropertyInfo pi)
        {
            if (pi.CustomAttributes.Count() > 0)
                return true;
            return false;
        }

        public TaskControlType GetTaskControlType(PropertyInfo pi)
        {
            CustomAttributeData cad = pi.CustomAttributes.First();

            if (cad != null)
            {
                if (cad.AttributeType == typeof(TaskControlOptions))
                {
                    CustomAttributeTypedArgument cata = cad.ConstructorArguments.First();

                    if (cata.ArgumentType == typeof(TaskControlType))
                    {
                        TaskControlType tit = (TaskControlType)cata.Value;
                        return tit;
                    }
                }
            }
            return TaskControlType.Default;
        }

        public int GetTaskMinValue(PropertyInfo pi)
        {
            CustomAttributeData cad = pi.CustomAttributes.First();

            if(cad != null)
            {
                if(cad.AttributeType == typeof(TaskControlOptions))
                {
                    CustomAttributeTypedArgument firstArg = cad.ConstructorArguments.First();

                    if(firstArg.ArgumentType == typeof(int))
                    {
                        int val = (int)firstArg.Value;
                        return val;
                    }
                }
            }
            return 0;
        }

        public int GetTaskMaxValue(PropertyInfo pi)
        {
            CustomAttributeData cad = pi.CustomAttributes.First();

            if (cad != null)
            {
                if (cad.AttributeType == typeof(TaskControlOptions))
                {
                    CustomAttributeTypedArgument firstArg = cad.ConstructorArguments.First();

                    if (firstArg.ArgumentType == typeof(int) && cad.ConstructorArguments.Count > 1)
                    {
                        CustomAttributeTypedArgument secondArg = cad.ConstructorArguments[1];

                        if (secondArg.ArgumentType == typeof(int))
                        {
                            int val = (int)secondArg.Value;
                            return val;
                        }
                    }
                }
            }
            return 100000;
        }

        public bool IsHiddenControl(PropertyInfo pi)
        {
            if (HasCustomAttributes(pi) && GetTaskControlType(pi) == TaskControlType.Hidden)
                return true;
            return false;
        }

        public void CreateTaskUIOption(PropertyInfo pi, TaskControlType taskItemType, Type itemType)
        {
            if(taskItemType == TaskControlType.Default)
            {

            }
            else if(taskItemType == TaskControlType.OpenFileTextBox)
            {

            }
            else if(taskItemType == TaskControlType.SaveFileTextBox)
            {

            }
            else if(taskItemType == TaskControlType.FolderTextBox)
            {

            }
            else if(taskItemType == TaskControlType.NumericUpDown)
            {

            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (flowLayoutPanel1.Bottom > button3.Top)
            {
                this.Height += (button3.Top - flowLayoutPanel1.Bottom) + 5;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string typeName = comboBox1.Text;

            try
            {
                Type type = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Name, typeName)).First();
                Task = (ITask)Activator.CreateInstance(type);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to create task of type " + typeName + "\n" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*if (textBox1.Text != string.Empty)
                task.Path = textBox1.Text;
            if (textBox2.Text != string.Empty)
                task.Arguments = textBox2.Text;*/

            foreach(Control c in flowLayoutPanel1.Controls)
            {
                PropertyInfo pi = taskType.GetProperty(c.Name);

                if (pi == null)
                    continue;

                if(c.GetType() == typeof(TextBox))
                {
                    pi.SetValue(task, c.Text);
                }
                else if(c.GetType() == typeof(CheckBox))
                {
                    pi.SetValue(task, ((CheckBox)c).Checked);
                }
                else if(c.GetType() == typeof(NumericUpDown))
                {
                    pi.SetValue(task, ((NumericUpDown)c).Value);
                }
            }

            DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //textBox1.Text = openFileDialog1.FileName;
            }
        }
    }
}
