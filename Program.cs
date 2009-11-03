using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Histogram
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.ConsoleTraceListener(true));
			
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
