using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;

using Newtonsoft.Json;

using Stend.Production.FileWrite;
using Stend.Production.Root;
using Stend.Pruduction.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Stend.Production.Initialization.Controls
{
    public partial class InitControl : XtraUserControl
    {

        PluginBase _step = null;

        List<PluginBase> pluginBases = null;

        UserControl newPluginControl = null;

        public InitControl()
        {
            InitializeComponent();
        }

        public InitControl(PluginBase step) : this()
        {
            _step = step;

            textPlugin.Text = _step.Name;
            textVersion.Text = _step.Version;
            textStepName.Text = _step.StepName;
            memoDescription.Text = _step.Description;

            //pluginBaseBindingSource.DataSource = childs;

             var plugBase = Helpers.LoadPlug(".PluginObject");
            
            foreach (var item in plugBase)
            {
                cbPluginName.Properties.Items.Add(item);
            }


            if (RootScript.Spodes != null)
            {
                //cbPluginName.SelectedIndexChanged -= cbPluginName_SelectedIndexChanged;

                //try
                //{
                //    if (_step != null)
                //    {
                //        cbPluginName.EditValue = (PluginBase)_step;
                //    }
                //}
                //finally
                //{
                   // cbPluginName.SelectedIndexChanged += cbPluginName_SelectedIndexChanged;
                //}

                foreach (var item in plugBase)
                {
                    if (cbPluginName.Text == item.Name)
                    {
                        RootScript.pluginBase.Add(item);
                    }
                    if (cbPluginName.Text == item.Name)
                    {
                        RootScript.pluginBase.Add(item);
                    }
                    if (cbPluginName.Text == item.Name)
                    {
                        RootScript.pluginBase.Add(item);
                    }
                }

                stepBaseBindingSource.DataSource = pluginBases;
                stepBaseBindingSource.ResetBindings(false);

                RootScript.Spodes = null;
            }
        }

        private void gcCreatingList_CustomButtonClick(object sender, BaseButtonEventArgs e)
        {
            try
            {
                _step.AddLog($"Выполнения события: {e.Button.Properties.Tag}");

                switch(Convert.ToInt32(e.Button.Properties.Tag.ToString()))
                {
                    case 1:
                        _step.AddLog("Выполняется добавление объекта");

                        SpodesListObject slo = new SpodesListObject(_step);
                        slo.ShowDialog();

                        stepBaseBindingSource.ResetBindings(false);

                        break;
                    case 2:
                        _step.AddLog("Выполняется удаление объекта");
                        break;
                    case 3:
                        _step.AddLog("Выполняется редактирование объекта");
                        break;
                    case 4:
                        _step.AddLog("Выполняется перемещение объекта на верх");
                        break;
                    case 5:
                        _step.AddLog("Выполняется перемещение объекта в низ");
                        break;
                    case 6:
                        _step.AddLog("Выполняется запись объекта из файла");

                        using (var dig = new OpenFileDialog())
                        {
                            if (dig.ShowDialog() == DialogResult.OK)
                            {
                                var json = File.ReadAllText(dig.FileName);
                                PluginObject.AutoscrollIndication = JsonConvert.DeserializeObject<List<CaptureGridObject>>(json); 
                            }
                        }

                        break;
                }
            }
            catch(Exception ex)
            {
                _step.AddLog($"Ошибка выполнения события: {ex.Message}");
            }
        }

        private void InitControl_DoubleClick(object sender, EventArgs e)
        {


        }

        private void viewChildrensInit_DoubleClick(object sender, EventArgs e)
        {
            int next = RootScript.scriptTree.Count > 0 ? RootScript.scriptTree.Max(item => item.ID) + 1 : 1;

            //PluginBase step = viewChildrensInit.GetFocusedRow() as PluginBase;

            //step.Show();
        }

        private void cbPluginName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPluginName.SelectedItem != null)
            {
                _step = Activator.CreateInstance(cbPluginName.SelectedItem.GetType()) as PluginBase;
                CreatePluginControl();
            }
        }

        private void CreatePluginControl()
        {
            if (_step != null)
                try
                {
                    //lciNoControlLabel.Visibility = LayoutVisibility.Never;
                    //pnlStepSetup.Controls.Remove(newPluginControl);
                    panelControl.Controls.Remove(newPluginControl);
                    if (_step.EditControl is UserControl userControl)
                    {
                        newPluginControl = (UserControl)Activator.CreateInstance(userControl.GetType(), new object[] { _step });
                        newPluginControl.Parent = panelControl;
                        newPluginControl.Dock = DockStyle.Fill;
                    }
                    else
                        throw new Exception("Отсутствует редактор плагина");
                }
                catch (Exception ex)
                {
                    //lblNoControl.Text = ex.Message;
                    //lciNoControlLabel.Visibility = LayoutVisibility.Always;
                }
        }
    }
}
