using System;
using System.Windows.Forms;

namespace Stend.Pruduction
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ScriptSettingsMain());
        }
    }
}
