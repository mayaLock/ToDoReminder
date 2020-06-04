/*
    Dipayan Sarker
    March 07, 2020
*/

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ToDoReminder.HelperExtension;
using ToDoReminder.TaskCore;
using ToDoReminder.FileIO;
using System.Windows.Xps.Packaging;
using System.IO;

namespace ToDoReminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        // private instance variables
        private bool _canTick;
        private bool _canAdd;
        private TaskManager _taskManager;
        private TaskCore.Task _task;
        private event EventHandler _HandleTimer;
        private event EventHandler _TaskManagerCountChanged;
        private event EventHandler _CanAddValueChanged;
        private string _xPsDocumentFileName;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.datePicker.CustomFormat = "yyyy-MM-dd\t\t\t\t\t\t\t\tHH:mm"; // set date picker custom format string
            this.comBoxProirity.ItemsSource = Enum.GetValues(typeof(PriorityType)).OfType<PriorityType>().ToList().Where(priority => priority != PriorityType.None).Select(priority => priority.ToStringCustom()); // set item source of the comboBox            
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainWindow_Closing); // window closing event subscription
            this._StartUp();
            this._canTick = true; // set default
            this._OnHandleTimer(); // signal timer handle to start handling tick event
            this._taskManager = new TaskManager(); // instantiate TaskManager
            this.datePicker.MouseEnter += new EventHandler(this.DatePicker_MouseEnter); // datePicker mouse enter event event subscription
            this._task = new TaskCore.Task(); // instantiate Task
            this._TaskManagerCountChanged += new EventHandler(this.MainWindow_TaskManagerCountChanged); // TaskManager countChanged event subscription
            this._OnTaskManagerCountChanged(); // singal that TaskManagerCountChanged to handle its event
            this._CanAddValueChanged += new EventHandler(this.MainWindow_CanAddValueChanged); // CanAddValueChanged event subscription
            this._canAdd = true; // set defualt
            this._OnCanAddValueChanged(); // signal CanValueChanged event handler
            this._xPsDocumentFileName = "xpsSavedDocument.xps"; // XPS file name that will be saved on the disk

        }

        /// <summary>
        /// An event handler for CanAddValueChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_CanAddValueChanged(object sender, EventArgs e)
        {
            if (this._canAdd) // if canAdd we enable add button
            {
                this.btnAdd.IsEnabled = true;
            }
            else // else we disable it
            {
                this.btnAdd.IsEnabled = false;
            }
        }

        /// <summary>
        /// An event handler for TaskManagerCountChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_TaskManagerCountChanged(object sender, EventArgs e)
        {
            if (this._taskManager.Count < 1) // if taskManager has no task then we disable btnChange and btnDelete
            {
                this.btnChange.IsEnabled = false;
                this.btnDelete.IsEnabled = false;
            }
        }

        /// <summary>
        /// Raises CanAddValueChanged event
        /// </summary>
        private void _OnCanAddValueChanged()
        {
            this._CanAddValueChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises TaskManagerCountChanged event
        /// </summary>
        private void _OnTaskManagerCountChanged()
        {
            this._TaskManagerCountChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// An event handler for datePicker MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatePicker_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip tooltip = new System.Windows.Forms.ToolTip(); // we create a new instance of ToolTip (WINFORM ToolTip, since datePicker is a WINFORM control
            tooltip.BackColor = System.Drawing.Color.White; // we chage the tooltip background
            tooltip.SetToolTip(this.datePicker, "Click to open calender for date, write in time here"); // finally we associate the tooltip with datePicker
        }

        /// <summary>
        /// Raises HandleTimer event
        /// </summary>
        private void _OnHandleTimer()
        {
            this._HandleTimer?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// An event handler for MainWindow Closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = System.Windows.MessageBox.Show("Are you sure to exit?", "Think Twice!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel ? true : false; // ask user if they are ready to close the app or not
            if (!e.Cancel) // if we close
            {
                this._canTick = false; // we make canTick to false
                this._OnHandleTimer(); // and signal that to the event handler
            }
        }

        /// <summary>
        /// Initiates start up tasks
        /// </summary>
        private void _StartUp()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) }; // we instantiate the timer with 1 second interval
            dispatcherTimer.Tick += (s, e) =>
            {
                this.lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
            };
            this._HandleTimer += (s, e) => // event handler for HandleTImer event
            {
                if (this._canTick) // if canTick we start the timer
                {
                    dispatcherTimer.Start();
                }
                else // else we stop the timer
                {
                    dispatcherTimer.Stop();
                }
            };
            this._SetDefaultValues(); // we set default value to different input controls
        }

        /// <summary>
        /// Sets default values to user input controls
        /// </summary>
        private void _SetDefaultValues()
        {
            this.comBoxProirity.SelectedIndex = -1;
            this.txtToDo.Clear();
            this.datePicker.Value = DateTime.Now;
        }

        /// <summary>
        /// An event handler for btnAdd Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!this._ReadInputFromControls()) // if invalid input we show error message and leave
            {
                System.Windows.MessageBox.Show(this._task.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!this._taskManager.Add(this._task)) // if add to taskManager fails we show error and then return
            {
                System.Windows.MessageBox.Show("Couldn't add new task!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this._UpdateUI(); // finally we update the UI
        }

        /// <summary>
        /// An event handler for btnChange Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (this.lstBoxAllItem.SelectedIndex != -1) // if listBox item is selected
            {
                if (!this._ReadInputFromControls()) // we show error if invalid input
                {
                    System.Windows.MessageBox.Show(this._task.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }                
                if (!this._taskManager.Change(this._task, this.lstBoxAllItem.SelectedIndex)) // we show error if change of task fails
                {
                    System.Windows.MessageBox.Show("Couldn't change existing task!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                this._UpdateUI(); // update the UI               
            }
            else // if no item selected we show error message
            {
                System.Windows.MessageBox.Show("Must select item before changing!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// An event handler for btnDelete Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.lstBoxAllItem.SelectedIndex != -1) // if listBox item is selected
            {
                bool canDelete = System.Windows.MessageBox.Show("Are you sure to delete the selected task?", "Conformation", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK ? true : false; // we ask the user if they want to delete the item or not
                if (!canDelete)
                {
                    return; // if no then we return
                }
                if (!this._taskManager.Delete(this.lstBoxAllItem.SelectedIndex)) // if delete fails then we show error message
                {
                    System.Windows.MessageBox.Show("Couldn't delete existing task!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                this._UpdateUI(); // update the UI
            }
            else // if no item selected we show error message
            {
                System.Windows.MessageBox.Show("Must select item before deleting!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// Updates the UI controls with output value
        /// </summary>
        private void _UpdateUI()
        {
            this._OnTaskManagerCountChanged(); // signal taskManager value changed event
            this.lstBoxAllItem.Items.Clear(); // clear listBox item
            foreach (string task in this._taskManager.GetTaskListStringArray())
            {
                this.lstBoxAllItem.Items.Add(task); // add string representation of task to listBox items
            }
            this._task = new TaskCore.Task(); // create an empty task
            this._SetDefaultValues(); // set defaut values to controls
        }

        /// <summary>
        /// Updates the UI controls with array of String values
        /// </summary>
        /// <param name="data"></param>
        private void _UpdateUI(string[] data)
        {
            this._taskManager.Clear(); // remove all the task from the taskManager
            if (!TaskManager.TryParse(data, out this._taskManager)) // if tryParse fails we show error message and then return
            {
                System.Windows.MessageBox.Show("Couldn't parse file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this._UpdateUI(); // call the default UpdateUI() 
        }

        /// <summary>
        /// An event handler for MenuItemNew Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            this._NewTask();
        }

        /// <summary>
        /// An event handler for MenuItemOpen Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            this._OpenTaskFromFile();

        }

        /// <summary>
        /// Opens a text file filled with saved task string
        /// Then Displays the new data in the listBox
        /// </summary>
        private void _OpenTaskFromFile()
        {
            if (this._taskManager.Count > 0) // we ask the user if they want to save the unsaved listBox tasks before opening the text file
            {
                bool res = System.Windows.MessageBox.Show("Do you want to save unsaved tasks?", "Conformation", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK ? true : false;
                if (res) // if user wants to save the file then we open save dialog
                {
                    this._SaveTaskToFile();
                }                
            } // else we open OpenDiaFile dialog and set different options
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.AddExtension = true;
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text Files (*.txt)|*.txt";
            openFileDialog.CheckFileExists = true;
            Nullable<bool> result = openFileDialog.ShowDialog();
            string[] data = null;
            if (result is true) // if user clciks on open button
            {
                string fileName = openFileDialog.FileName;
                DataReader dataReader = new DataReader(fileName); // we instantiate the DataReader and try to read the file
                try
                {
                    data = dataReader.Read();
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("Couldn't open file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                this._UpdateUI(data); // finally we update the UI with data
            }
        }

        /// <summary>
        /// An event handler for MenuItemExit Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // closes the current window
        }

        /// <summary>
        /// An event handler for MenuItemAbout Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow(this);
            aboutWindow.ShowDialog();
        }

        /// <summary>
        /// An event handler for MenuItemSave Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            this._SaveTaskToFile();
        }

        /// <summary>
        /// Saves listBox item to a text file
        /// </summary>
        private void _SaveTaskToFile()
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog(); // we instantiate SaveFileDialog and set values
            saveFileDialog.FileName = "Untitled";
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            saveFileDialog.AddExtension = true;
            saveFileDialog.OverwritePrompt = true;
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result is true) // if user clicks the save button
            {
                string fileName = saveFileDialog.FileName;
                DataWriter dataWriter = new DataWriter(); // we instantiate the DataWriter and set values
                dataWriter.FileName = fileName;
                dataWriter.Content = this._taskManager.GetTaskListStringArray("AAA"); // we get special "AAA" string format that fetches values seperated by semicolons (;) 
                if (!dataWriter.Write()) // if write fail we show error message
                {
                    System.Windows.MessageBox.Show("Couldn't save file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        /// <summary>
        /// An event handler for print menu item click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemPrint_Click(object sender, RoutedEventArgs e)
        {
            this._PrintXPS();
        }

        /// <summary>
        /// Opens Print dialog and then writes a new XPS file with the elements of the listbox control
        /// If no printer is connected then System.Printing.PrintQueueException is thrown
        /// </summary>
        private void _PrintXPS()
        {
            System.Windows.Controls.PrintDialog printDialog = new System.Windows.Controls.PrintDialog(); // creates print dialog
            Nullable<bool> result = printDialog.ShowDialog();
            if (result is true)
            {
                if (File.Exists(this._xPsDocumentFileName)) // if file exists then we delete that file
                {
                    File.Delete(this._xPsDocumentFileName);
                }
                XpsDocument xpsd = new XpsDocument(this._xPsDocumentFileName, FileAccess.ReadWrite); // we create an empty XpsDocument
                System.Windows.Xps.XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd); // then we create an Xps document writer
                xw.Write(this.lstBoxAllItem); // we pass the listbox as the item that we want the XpsDocumentWriter to write
                xpsd.Close(); // finally we close the document and show a message to the user abd return
                System.Windows.MessageBox.Show($"{this._xPsDocumentFileName} has been saved!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        /// <summary>
        /// Returns true if reading of input from controls is successful
        /// </summary>
        /// <returns></returns>
        private bool _ReadInputFromControls()
        {
            this._task.Date = this.datePicker.Value;
            if (this.comBoxProirity.SelectedIndex != -1)
            {
                this._task.Priority = (PriorityType)Enum.Parse(typeof(PriorityType), this.comBoxProirity.SelectedItem.ToString().Replace(" ", "_"));
            }
            this._task.Description = this.txtToDo.Text;
            return this._task.Validate(); // returns the value of the Task.Validate() that checks if the members are valid or not
        }

        /// <summary>
        /// An event handler for listBoxAllItem SlectionChaged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstBoxAllItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstBoxAllItem.SelectedIndex != -1) // if item is elected
            {
                this.btnChange.IsEnabled = true; // we enable buttons
                this.btnDelete.IsEnabled = true;
                this._canAdd = false; // we disable add button
                this._OnCanAddValueChanged(); // we signal that to the event handler

                this._task = this._taskManager.GetTask(this.lstBoxAllItem.SelectedIndex);
                if (this._task != null) // if we have valid Task object then we update the UI controls value
                {
                    this.datePicker.Value = this._task.Date;
                    this.txtToDo.Text = this._task.Description;
                    this.comBoxProirity.SelectedIndex = ((int)this._task.Priority) - 1;
                }
            }
            else // else we disable buttons
            {
                this.btnChange.IsEnabled = false;
                this.btnDelete.IsEnabled = false;
                this._canAdd = true; // we signal add button should be enabled
                this._OnCanAddValueChanged();
            }
        }

        /// <summary>
        /// Event handler for NewCommand executed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this._NewTask();
        }
        
        /// <summary>
        /// A wapper for SetDefaultValues()
        /// </summary>
        private void _NewTask()
        {
            this._SetDefaultValues();
        }

        /// <summary>
        /// Event handler for SaveCommand executed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this._SaveTaskToFile();
        }

        /// <summary>
        /// Event handler for OpenCommand executed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this._OpenTaskFromFile();
        }

        /// <summary>
        /// Event handler for PrintCommand executed event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this._PrintXPS();
        }
    }
}
