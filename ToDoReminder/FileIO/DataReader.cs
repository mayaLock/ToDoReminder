/*
    Dipayan Sarker
    March 07, 2020
*/

using System.IO;

namespace ToDoReminder.FileIO
{
    /// <summary>
    /// A class to read string data from text files
    /// </summary>
    public class DataReader
    {
        // private instance variables
        private string _fileName;

        /// <summary>
        /// Gets or Sets FIleName
        /// </summary>
        public string FileName
        {
            get => this._fileName;
            set => this._fileName = value;
        }

        /// <summary>
        /// Default constructor with one value
        /// </summary>
        /// <param name="fileName">input- file name</param>
        public DataReader(string fileName)
        {
            this._fileName = fileName;
        }

        /// <summary>
        /// Default constructor with no default values
        /// </summary>
        public DataReader() : this(string.Empty)
        {

        }

        /// <summary>
        /// Reads array of strings from a text file
        /// Returns null if invalid file
        /// </summary>
        /// <returns></returns>
        public string[] Read()
        {
            if (!this._fileName.EndsWith(".txt") || string.IsNullOrEmpty(this._fileName)) // if invalid file extension or file name is empty we return null
            {
                return null;
            }
            return File.ReadAllLines(this._fileName); // else we return the content of the text file as string array
        }
    }
}
