using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OODAWebTroj
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HideOnStartupApplicationContext(new MainFrame()));
        }
        internal class HideOnStartupApplicationContext : ApplicationContext
        {
            public HideOnStartupApplicationContext(Form mainForm)
            {
                mainForm.FormClosed += mainFormInternal_FormClosed;
            }

            private static void mainFormInternal_FormClosed(object sender, FormClosedEventArgs e)
            {
                Application.Exit();

            }
        }
    }
}
