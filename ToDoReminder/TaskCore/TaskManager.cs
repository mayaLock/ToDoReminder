/*
    Dipayan Sarker
    March 07, 2020
*/

using System.Collections.Generic;
using System.Linq;
using ToDoReminder.HelperExtension;

namespace ToDoReminder.TaskCore
{
    /// <summary>
    /// A class to manipulate and mannage tasks
    /// </summary>
    public class TaskManager
    {
        // private instance variable
        private List<Task> _taskList;

        /// <summary>
        /// Returns the number of tasks in the taskManager
        /// </summary>
        public int Count { get => this._taskList.Count; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TaskManager()
        {
            this._taskList = new List<Task>(); // we initialize the tasklist with an empty list of task
        }

        /// <summary>
        /// Adds a task in the list of tasks
        /// Returns true if the addition is successful
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Add(Task task)
        {
            bool ret = false;
            if (task.Validate()) // if the task is valid
            {
                this._taskList.Add(task); // we add the task in the list
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Changes a task with a new provided task at a given index
        /// Returns true of successful
        /// </summary>
        /// <param name="task"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Change(Task task, int index)
        {
            bool ret = false;
            if (this.ValidateTaskListIndex(index) && task.Validate()) // if the index is in bound and provided task is a valid task
            {
                this._taskList[index] = task; // we set the task in the list
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Deletes a task from the list with a given index
        /// Returns true if delete is successful
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Delete(int index)
        {
            bool ret = false;
            if (this.ValidateTaskListIndex(index)) // if the index is within bound
            {
                this._taskList.RemoveAt(index); // we remove the task from the list
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Gets an array of string of all the tasks from the taskManager
        /// </summary>
        /// <returns></returns>
        public string[] GetTaskListStringArray()
        {
            return this._taskList.Select(task => task.ToString()).ToArray();
        }

        /// <summary>
        /// Gets an array of formated string of all the tasks from the taskManager
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string[] GetTaskListStringArray(string format)
        {
            return this._taskList.Select(task => task.ToString(format)).ToArray();
        }

        /// <summary>
        /// Gets a task object with a given index number
        /// Returns null value if the index is invalid
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Task GetTask(int index)
        {
            Task task = null;
            if (this.ValidateTaskListIndex(index)) // if index within list bound
            {
                task = this._taskList.ElementAt(index); // we get the task from the list and set it to task variable
            }
            return task;
        }

        /// <summary>
        /// Removes all the tasks from task manager
        /// </summary>
        public void Clear()
        {
            this._taskList.Clear();
        }

        /// <summary>
        /// Converts the string array representation of Tasks to a TaskManager object.
        /// The return value indeicates whether the conversion was successful or not
        /// </summary>
        /// <param name="taskStringArray"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string[] taskStringArray, out TaskManager result)
        {
            bool ret = false;
            TaskManager taskManager = new TaskManager(); // we create an empty TaskManager
            if (taskStringArray.Length < 1) // if taskManager is empty we return
            {
                goto END;
            }
            foreach (string taskString in taskStringArray) // we try to parse every string from the string array as a new Task
            {
                Task task;
                if (!Task.TryParse(taskString, out task))
                {
                    goto END; // if we fail then we return
                }
                taskManager.Add(task); // else we add the task in the taskManager
            }
            ret = true;
            END:
            result = taskManager;
            return ret;
        }
    }
}
