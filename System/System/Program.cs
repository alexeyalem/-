using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace System
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Process thisProc = Process.GetCurrentProcess();
            thisProc.PriorityClass = ProcessPriorityClass.RealTime;
            Application.Run(new Form1());
        }
    }
}
