using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunner
{
    public class TaskControlOptions : Attribute
    {
        TaskControlType taskControlType;
        public TaskControlType TaskControlType
        {
            get { return taskControlType; }
            set { taskControlType = value; }
        }

        int minValue = 0;
        public int MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }

        int maxValue;
        public int MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }

        public TaskControlOptions()
        {
            taskControlType = TaskControlType.Default;
            minValue = 0;
            maxValue = 100000;
            isChecked = false;
        }

        public TaskControlOptions(TaskControlType type)
        {
            taskControlType = type;
        }

        public TaskControlOptions(int minValue, int maxValue)
        {
            taskControlType = TaskControlType.Default;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public TaskControlOptions(bool isChecked)
        {
            taskControlType = TaskControlType.Default;
            this.isChecked = isChecked;
        }
    }

    public enum TaskControlType
    {
        TextBox,
        NumericUpDown,
        OpenFileTextBox,
        SaveFileTextBox,
        FolderTextBox,
        Default,
        Hidden
    }
}
