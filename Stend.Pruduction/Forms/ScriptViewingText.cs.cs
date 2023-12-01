using DevExpress.XtraBars.Ribbon;

namespace Stend.Pruduction.Forms
{
    public partial class ScriptViewingText : RibbonForm
    {
        public ScriptViewingText()
        {
            InitializeComponent();
            InitilizeRichEditControl();
            ribbonControl1.SelectedPage = homeRibbonPage1;
        }


        public ScriptViewingText(string plainText)
        {
            InitializeComponent();
            InitilizeRichEditControl(plainText);
            ribbonControl1.SelectedPage = homeRibbonPage1;
        }

        void InitilizeRichEditControl(string plainText = "")
        {
            richEditControl1.Text = plainText;
        }
    }
}
