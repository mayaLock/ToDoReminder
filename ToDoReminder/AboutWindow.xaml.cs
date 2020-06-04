/*
    Dipayan Sarker
    March 07, 2020
*/

using System.Linq;
using System.Windows;

namespace ToDoReminder
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        /// <summary>
        /// Gets VersionNumber string
        /// </summary>
        public string VersionNumber { get => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        
        /// <summary>
        /// Gets Description String
        /// </summary>
        public string DescriptionText 
        { get => System.Reflection.Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), false)
                .Cast<System.Reflection.AssemblyDescriptionAttribute>()
                .FirstOrDefault().Description; 
        }

        /// <summary>
        /// Constructor with one default value
        /// </summary>
        /// <param name="owner"></param>
        public AboutWindow(Window owner)
        {
            InitializeComponent();
            this.Owner = owner; // set the ownder to center the window to the owner
            this.DataContext = this; // set dataContent to fix binding
            this.btnOk.Click += (s, e) => // btnOk mouseClick event
            {
                this.DialogResult = true;
                this.Close();
            };
        }
    }
}
