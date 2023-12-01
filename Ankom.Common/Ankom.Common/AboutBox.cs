using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using AssemblyInfoAlias = Ankom.Common.AnkomAssemblyInfo;

namespace Ankom.Common
{
    public partial class AboutBox : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public AboutBox(Assembly workAssembly = null)
        {
            InitializeComponent();

            if (workAssembly == null)
            {
                workAssembly = Assembly.GetCallingAssembly();
            }
            AssemblyInfoAlias assembly = new AssemblyInfoAlias(workAssembly);
#if ANKOM
            this.Text = assembly.AssemblyTitle;
            this.label1.Text = String.Format("Модуль: {0}", this.Text);
            this.labelProductName.Text = assembly.AssemblyProduct;
            this.labelVersion.Text = $"Версия {assembly.AssemblyVersionMajor}.{assembly.AssemblyVersionMinor}.{assembly.AssemblyVersionBuild}.{assembly.AssemblyVersionRevisionHex}";
            this.labelCopyright.Text = assembly.AssemblyCopyright;
            this.labelCompanyName.Text = assembly.AssemblyCompany;
            this.textBoxDescription.Text = assembly.AssemblyDescription;
#else
            this.Text = assembly.AssemblyTitle;
            this.label1.Text = $"Модуль: {assembly.AssemblyTitle}";
            this.labelProductName.Text = AnkomInfo.ProductName + Environment.NewLine + AnkomInfo.AssemblyCompany;
            this.labelVersion.Text = $"Версия программы {assembly.AssemblyVersionMajor}.{assembly.AssemblyVersionMinor}.{assembly.AssemblyVersionBuild}.{assembly.AssemblyVersionRevisionHex}";
            this.labelCopyright.Text = $"{assembly.AssemblyCopyright}, {AnkomInfo.AssemblyCompany}";
            this.labelCompanyName.Text = $"Организация: {AnkomInfo.AssemblyCompany}";
            this.textBoxDescription.Text = assembly.AssemblyDescription;
#endif
        }

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Form frm = (Form)sender;
            ControlPaint.DrawBorder(e.Graphics, frm.ClientRectangle,
            Color.LightBlue, 3, ButtonBorderStyle.Solid,
            Color.LightBlue, 3, ButtonBorderStyle.Solid,
            Color.LightBlue, 3, ButtonBorderStyle.Solid,
            Color.LightBlue, 3, ButtonBorderStyle.Solid);
        }

        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == 0x84)
        //    {
        //        base.WndProc(ref m);
        //        if ((int)m.Result == 0x1)
        //            m.Result = (IntPtr)0x2;
        //        return;
        //    }

        //    base.WndProc(ref m);
        //}
    }
}
