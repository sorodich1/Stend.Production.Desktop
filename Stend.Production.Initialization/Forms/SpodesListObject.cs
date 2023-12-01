﻿using DevExpress.XtraEditors;
using Stend.Production.Initialization.Controls;
using Stend.Production.Root;
using System;
using System.Windows.Forms;

namespace Stend.Pruduction.Forms
{
    public partial class SpodesListObject : XtraForm
    {
        public Spodes Spodes { get; private set; }
        private readonly PluginBase _plugin;
        public SpodesListObject(PluginBase plugin)
        {
            _plugin = plugin;

            InitializeComponent();



            spodesBindingSource.DataSource = Spodes.spodes;
        }

        private void gViewSpodes_DoubleClick(object sender, EventArgs e)
        {
            var selectedObgect = gViewSpodes.GetFocusedRow() as Spodes;

            if(selectedObgect != null)
            {
                if (_plugin.EditControl is UserControl userControl)
                {
                    RootScript.Spodes = selectedObgect;

                    UserControl ctrl = (UserControl)Activator.CreateInstance(userControl.GetType(), new object[] { _plugin });
                }
            }
            Close();
        }
    }
}