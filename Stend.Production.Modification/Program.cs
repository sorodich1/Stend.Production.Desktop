using Gurux.DLMS.ManufacturerSettings;

using Newtonsoft.Json;
using Stend.Production.Root;
using System;
using System.IO;
using System.Windows.Forms;
using ZIP.DLMS;

namespace Stend.Production.Modification
{
    internal static class Program
    {
        internal static string deviseSettings;
        internal static string scriptSetting;
        public static SettingPort settingPort = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Helpers.Log.Info($"Загружаем базовые настройки подключения устройства из {Helpers.deviceFileName}");
                deviseSettings = File.ReadAllText(Helpers.deviceFileName);
                JsonConvert.DeserializeObject<GXDLMSDevice>(deviseSettings);
            }
            catch(Exception ex)
            {
                Helpers.Log.Error($"Файл настроек устройства не был загружен {ex.Message}");
            }
            try
            {
                var scriptSetting = Helpers.LoadProductionScript(File.ReadAllText(Helpers.scriptFileName));
            }
            catch (Exception ex)
            {
                Helpers.Log.Error($"Файл скрипта мадификации не был загружен {ex.Message}");
            }


            scriptSetting = File.ReadAllText(Helpers.scriptFileName);

            LoadScriptFiels();

            Application.Run(new Form1());
        }

        public static void LoadScriptFiels()
        {
            try
            {
                Console.WriteLine("Тестирование ПО стенда для выпуска счетчиков Вектор-101.");
                Helpers.Log.Info($"Путь к каталогу настроек: {Helpers.dataDir}");

                string json = File.ReadAllText(Helpers.stendSettingFileName);
                settingPort = JsonConvert.DeserializeObject<SettingPort>(json);

                if (settingPort != null)
                {
                    for (int i = settingPort.Object.Count - 1; i >= 0; i--)
                    {
                        if (String.IsNullOrWhiteSpace(settingPort.Object[i].Port))
                            settingPort.Object.RemoveAt(i);
                    }
                }
                if (Helpers.GXManufacturers.Count == 0)
                    GXManufacturerCollection.ReadManufacturerSettings(Helpers.GXManufacturers);
            }
            catch(Exception ex)
            {
                Helpers.Log.Error($"{ex.Message}");
            }
        }
    }
}
