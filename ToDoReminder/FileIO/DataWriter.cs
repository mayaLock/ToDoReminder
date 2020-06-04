/*
    Dipayan Sarker
    March 07, 2020
*/

using System.IO;

namespace ToDoReminder.FileIO
{
    /// <summary>
    /// A class to write string array to a text file
    /// </summary>
    public class DataWriter
    {
        // private instance variable
        private string _fileName;
        private string[] _content;

        /// <summary>
        /// Gets or Sets FileName
        /// </summary>
        public string FileName
        {
            get => this._fileName;
            set => this._fileName = value;
        }

        /// <summary>
        /// Gets or Sets Content
        /// </summary>
        public string[] Content
        {
            get => this._content;
            set => this._content = value;
        }

        /// <summary>
        /// A constructor with two default values
        /// </summary>
        /// <param name="fileName">input- file name</param>
        /// <param name="content">input- content of the file</param>
        public DataWriter(string fileName, string[] content)
        {
            this._fileName = fileName;
            this._content = content;
        }

        /// <summary>
        /// A default constructor
        /// </summary>
        public DataWriter() : this(string.Empty, null)
        {

        }

        /// <summary>
        /// Writes an array of string to a text file
        /// Returns true if the write is successful
        /// </summary>
        /// <returns></returns>
        public bool Write()
        {
            bool ret = false;
            if (this._content is null) // if content is null we return false
            {
                return ret;
            }
            if (string.IsNullOrEmpty(this._fileName) || this._content.Length < 1 || !this._fileName.EndsWith(".txt")) // we check if the file name is empty or not; or string array has at least one element or valid file extension
            {
                return ret;
            }
            File.WriteAllLines(this._fileName, this._content); // finally we write to the file
            ret = true;
            return ret;
        }      
    }
}
