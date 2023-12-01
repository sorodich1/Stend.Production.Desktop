using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using Gurux.DLMS.ManufacturerSettings;
using Newtonsoft.Json;
using Stend.Production.Root;
using Stend.Pruduction;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace Stend.Production.Modification
{
    public partial class Form1 : RibbonForm
    {
        public Form1()
        {
            InitializeComponent();

            placeSettingBindingSource.DataSource = Program.settingPort.Object;
        }

        private void btSetModific_ItemClick(object sender, ItemClickEventArgs e)
        {
            using(var setting = new ScriptSettingsMain())
            {
                if(setting.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                }
            }
        }

        private void btStart_ItemClick(object sender, ItemClickEventArgs e)
        {
            Program.settingPort = null;
            string json;


            json = File.ReadAllText(Helpers.stendSettingFileName);

            Program.settingPort = JsonConvert.DeserializeObject<SettingPort>(json);

            if (Program.settingPort != null)
            {
                for (int i = Program.settingPort.Object.Count - 1; i >= 0; i--)
                {
                    if (String.IsNullOrWhiteSpace(Program.settingPort.Object[i].Port))
                    {
                        Program.settingPort.Object.RemoveAt(i);
                    }
                }
            }
            DoManufacture(Program.settingPort, Program.deviseSettings, Program.scriptSetting);
        }

        private void DoManufacture(SettingPort settingPort, string deviseSettings, string scriptSetting)
        {
            try
            {
                if (settingPort.Object.Count == 0)
                {
                    //simpleLog.AppearanceItemCaption.ForeColor = System.Drawing.Color.Red;
                    //simpleLog.Text = $"Отсутствуют устройства для калибровки";
                }

                if (Helpers.GXManufacturers.Count == 0)
                {
                    GXManufacturerCollection.ReadManufacturerSettings(Helpers.GXManufacturers);
                }

                if (settingPort != null)
                {
                    // Идентификатор исполнения(модель) прибора учета - ОБЯЗАТЕЛЬНЫЙ общий параметр
                    // процесса производства, выбираемый оператором стенда перед запуском прошивки
                    //string modelID = settingPort.CommonParameters.FirstOrDefault(item => item.OBIS == "0.0.96.1.9.255")?.Value;
                    foreach(var st in settingPort.Object)
                    {
                        try
                        {
                            st.OnStatusChanged -= Item_OnStatusChanged;

                            // настраиваем параметры прошивки для места стенда item
                            // item.StendSettings = stendSettings;
                            // Создаем устройство для прошивки и настраиваем параметры подключения
                            st.SetDevice(deviseSettings);
                            //// Конфирурируеv логгер для устройства, установленного на указанном месте стенда
                            //item.Device.Logger = Program.ConfigureDeviceLogger(item);
                            st.Device.Manufacturers = Helpers.GXManufacturers;

                            Helpers.LoadDeviceAssociations(st.Device, $"{Helpers.AssocExt}");

                            var steps = Helpers.LoadProductionScript(scriptSetting);

                            if (st != null)
                            {
                                st.Steps = new BindingList<PluginBase>();
                                foreach(var item in steps?.Steps)
                                {
                                    item.Owner = st;
                                    st.Steps.Add(item);
                                }
                            }
                            st.Execute();
                        }
                        catch(Exception ex)
                        {
                            Helpers.Log.Error($"Ошибка настройки устройства {st.Serial} [{st.Port}]: {ex.Message}");
                        }
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void Item_OnStatusChanged(object sender, StatusChangedEventArgs args)
        {
            Invoke((MethodInvoker)delegate
            {
                if(sender is PlaceSetting place)
                {
                    string stepName =
                        args.Step != null ? $" {args.Step.StepName} - " : String.Empty;

                    if (place.Status == EnumStepStatus.Fatal || place.Status == EnumStepStatus.Cancel)
                    {
                        Helpers.Log.Error($"[{place}]: {stepName}{args.EventText}");
                        CheckUserActionPrompt(place);
                        placeSettingBindingSource.ResetBindings(false);
                        return;
                    }
                    else
                    {
                        Helpers.Log.Info($"[{place}]: {stepName}{args.EventText}");
                    }
                    CheckUserActionPrompt(place);
                    placeSettingBindingSource.ResetBindings(false);
                    // проверяем переход ПУ к шагу ожидания действия оператора
                    //if (!(place.StendSettings.Tag is UserActionStatus) && place.ActiveStep is UserActionPromptStep step &&
                    //step.Status == EnumStepStatus.Run)
                    //{
                    //    place.StendSettings.Tag = UserActionStatus.Start;
                    //    placeSettingBindingSource.ResetBindings(false);
                    //}
                    placeSettingBindingSource.ResetBindings(false);
                }
            });
        }

        public static void CheckUserActionPrompt(PlaceSetting place)
        {
            //if(place.ActiveStep is UserActionPromptStep step && !(step.Status == EnumStepStatus.Fatal || step.Status == EnumStepStatus.Cancel) && !(place.StendSettings.Tag is UserActionStatus))
            //{
            //    place.StendSettings.Tag = UserActionStatus.Start;
            //}
            //if (place.StendSettings.Tag is UserActionStatus userActtionStatus)
            //{
            //    if (userActtionStatus == UserActionStatus.Start)
            //    {
            //        // ожидаем, пока все ПУ не приступят к выполнению шага UserActionPromptStep или не будет отменена их настройка
            //        int placeCount = place.StendSettings.Objects.Count(item
            //            => item.Status == EnumStepStatus.Fatal || item.Status == EnumStepStatus.Cancel || item.ActiveStep is UserActionPromptStep);

            //        if (placeCount == UserActionPromptStep.places.Count)
            //        {
            //            if (Helpers.PromptMessage(/*place.ActiveStep.StepName*/"" + Environment.NewLine
            //                    + "Выполните необходимые действия и нажмите <OK>") == DialogResult.OK)
            //            {
            //                place.StendSettings.Tag = UserActionStatus.Ok;
            //            }
            //            else
            //            {
            //                place.StendSettings.Tag = UserActionStatus.Abort;
            //            }
            //        }
            //    }
            //    else if(userActtionStatus == UserActionStatus.Abort)
            //    {
            //        UserActionPromptStep.places.Clear();
            //        int placeCount = place.StendSettings.Objects.Count(item => item.Status == EnumStepStatus.Fatal);
            //        if (placeCount == UserActionPromptStep.places.Count)
            //        {
            //            place.StendSettings.Tag = null;
            //        }
            //    }
            //}
        }

        private void btSettingsStend_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (SettingComPort settingTest = new SettingComPort())
            {
                if (settingTest.ShowDialog() == DialogResult.OK)
                {
                    Program.settingPort.Object = SettingComPort.stendSettings.Object;
                    string json = JsonConvert.SerializeObject(Program.settingPort, Formatting.Indented);
                    File.WriteAllText(Helpers.stendSettingFileName, json);

                    for (int i = Program.settingPort.Object.Count - 1; i >= 0; i--)
                    {
                        if (String.IsNullOrWhiteSpace(Program.settingPort.Object[i].Port))
                            Program.settingPort.Object.RemoveAt(i);
                    }

                    //string js = JsonConvert.SerializeObject(Program.settingPort, Formatting.Indented);

                    //File.WriteAllText($"{Helpers.dataDir}\\NewDataFile\\{comFile.Text}.stend", js);

                    placeSettingBindingSource.DataSource = Program.settingPort;
                }
            }
        }
    }
}
