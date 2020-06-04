/*
    Dipayan Sarker
    March 07, 2020
*/

using ToDoReminder.TaskCore;

namespace ToDoReminder.HelperExtension
{
    /// <summary>
    /// A static helper extension class
    /// </summary>
    public static class HelperUtilEx
    {
        /// <summary>
        /// Returns a custom string after replacing "_" with " " (space)
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static string ToStringCustom(this PriorityType priority)
        {
            return priority.ToString().Replace("_", " ");
        }

        /// <summary>
        /// Returns boolean value whether TaskManager index is within the
        /// bound of Task List
        /// </summary>
        /// <param name="taskManager"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool ValidateTaskListIndex(this TaskManager taskManager, int index)
        {
            return (index > -1) && (index < taskManager.Count);
        }
    }
}
