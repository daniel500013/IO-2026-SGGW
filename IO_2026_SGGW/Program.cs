using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IO_2026_SGGW
{
    /// <summary>
    /// Klasa startowa aplikacji, zawierająca punkt wejścia procesu.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Główny punkt wejścia aplikacji. Włącza style wizualne Windows Forms i uruchamia
        /// główne okno <see cref="MainForm"/>.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
