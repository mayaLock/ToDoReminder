/*
    Dipayan Sarker
    March 07, 2020
*/

using System;
using ToDoReminder.HelperExtension;

namespace ToDoReminder.TaskCore
{
    /// <summary>
    /// A class for creating Task
    /// </summary>
    public class Task
    {
        // private instance variables
        private DateTime _date;
        private string _description;
        private PriorityType _priority;
        private string _errorMessage;

        /// <summary>
        /// Gets ErrorMessage
        /// </summary>
        public string ErrorMessage { get => this._errorMessage; }

        /// <summary>
        /// Gets or Sets Date
        /// </summary>
        public DateTime Date
        {
            get => this._date;
            set => this._date = value;
        }

        /// <summary>
        /// Gets or Sets Description
        /// </summary>
        public string Description
        {
            get => this._description;
            set => this._description = value;
        }

        /// <summary>
        /// Gets or Sets Priority
        /// </summary>
        public PriorityType Priority
        {
            get => this._priority;
            set => this._priority = value;
        }

        /// <summary>
        /// A constructor with three params
        /// </summary>
        /// <param name="date">input- date</param>
        /// <param name="description">input - description of the task</param>
        /// <param name="priority">type of priority</param>
        public Task(DateTime date, string description, PriorityType priority)
        {
            //initializing instance variables with passed values
            this._date = date;
            this._description = description;
            this._priority = priority;
            this._errorMessage = string.Empty; // default values
        }

        /// <summary>
        /// A defualt constructor
        /// </summary>
        public Task() : this(DateTime.Now, string.Empty, PriorityType.None)
        {

        }

        /// <summary>
        /// Gets long Date as string
        /// </summary>
        /// <returns></returns>
        private string _GetLongDateString()
        {
            return this._date.ToString("dddd, MMMM dd, yyyy");
        }

        /// <summary>
        /// Gets short time as string
        /// </summary>
        /// <returns></returns>
        private string _GetShortTimeString()
        {
            return this._date.ToString("HH:mm");
        }

        /// <summary>
        /// Gets Priority as string
        /// </summary>
        /// <returns></returns>
        private string _GetPriorityString()
        {
            return this._priority.ToStringCustom();
        }

        /// <summary>
        /// Resturns a string represeentation of Task
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this._GetLongDateString(),-30} " +
                $"{this._GetShortTimeString(),-10} " +
                $"{this._GetPriorityString(),-21} " +
                $"{this._description}";
        }

        /// <summary>
        /// Returns a formated string representation of Task
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            string s = string.Empty;
            if (format.Equals("AAA"))
            {
                s = $"{this._GetLongDateString(),-30};" +
                $"{this._GetShortTimeString(),-10};" +
                $"{this._GetPriorityString(),-21};" +
                $"{this._description}";
            }
            return s;
        }

        /// <summary>
        /// Converts the string representation of Task to a Task object.
        /// The return value indeicates whether the conversion was successful or not
        /// </summary>
        /// <param name="taskString"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string taskString, out Task result)
        {
            bool ret = false;
            Task tempTask = null; // default value
            string[] splitedData = taskString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries); // string array that contains 4 elements
            if (splitedData.Length < 4)
            {
                goto END; // if we don't have 4 elements, we leave
            }
            DateTime dateTime;
            if (!DateTime.TryParse($"{splitedData[0].Trim()} {splitedData[1].Trim()}", out dateTime)) // if splitedData[0] & splitedData[1] not parsed
            {
                goto END; // we return
            }
            PriorityType priorityType;
            if (!Enum.TryParse(splitedData[2].Trim().Replace(" ", "_"), out priorityType)) // if PropertyType is not parsed we return
            {
                goto END; // we return
            }
            tempTask = new Task(dateTime, splitedData[3], priorityType); // else we instantiate tempTask with the parsed values
            ret = true;
            END:
            result = tempTask;
            return ret;
        }

        /// <summary>
        /// Returns true if the task has valid elements
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            bool ret = false;
            if (this._priority == PriorityType.None) // if priority is set to default
            {
                this._errorMessage = "Must select a priority"; // we set error message and return
                return ret;
            }
            if (this._date <= DateTime.Now.AddDays(-1) || this._date >= DateTime.Now.AddYears(100))  // if the date is a previous date or more than or equal to 100 years from now 
            {
                this._errorMessage = "Date is set in a past or too distant future";
                return ret;
            }
            if (string.IsNullOrEmpty(this._description)) // if the description is not set
            {
                this._errorMessage = "Description can't be empty";
                return ret;
            }
            ret = true;
            return ret;
        }
    }
}
