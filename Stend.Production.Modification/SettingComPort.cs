using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using Gurux.Serial;
using Newtonsoft.Json;
using Stend.Production.Root;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Stend.Production.Modification
{
    public partial class SettingComPort : XtraForm
    {
        public static SettingPort stendSettings = new SettingPort();


        public SettingComPort()
        {
            InitializeComponent();

            stendSettings = JsonConvert.DeserializeObject<SettingPort>(File.ReadAllText(Helpers.oldSettingDir));
            placeSettingBindingSource.DataSource = stendSettings.Object;
            string[] ports = GXSerial.GetPortNames();
            repPort.Items.Add(" ");
            repPort.Items.AddRange(ports);
        }

        private void viewComPort_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            foreach (var item in stendSettings.Object)
            {
                if (item.Port == " ")
                {
                    item.Port = item.Serial.Replace(" ", string.Empty);
                }
                var set = new HashSet<string>();
                var PortCount = stendSettings.Object.Where(x => x.Port != null && x.Port != "" ? !set.Add(x.Port) : false).ToList();
                if (PortCount.Count > 0)
                {
                    labelError.AppearanceItemCaption.ForeColor = Color.Red;
                    labelError.Text = "Установка одинаковых последовательных портов запрещена";
                    btOK.Enabled = false;
                }
                else
                {
                    labelError.AppearanceItemCaption.ForeColor = Color.Black;
                    labelError.Text = "";
                    btOK.Enabled = true;
                }
            }
        }
    }
}