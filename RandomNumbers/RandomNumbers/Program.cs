using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using RandomNumbers.Utils;
using RandomNumbers.Tests;

namespace RandomNumbers
{
    /// <summary>
    /// Main class for the application, gets everything started with the GUI and Controller
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// View object stored in static location
        /// </summary>
        private static Front view;
        /// <summary>
        /// Controller object stored in static location
        /// </summary>
        private static Control control;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            view = new Front();
           control = new Control(ref view);
            
            Application.Run(view);
        }
    }
}
