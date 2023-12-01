using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;
using Newtonsoft.Json;
using NLog;
using Stend.Production.Root;
using Stend.Pruduction.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using ZIP.DLMS;

namespace Stend.Pruduction
{
    public partial class ScriptSettingsMain : RibbonForm, IPluginHost
    {
        #region NLog

        static string _logText = "";
        static bool logChanged = false;
        static bool logCapture = true;
        private static object lockObj = new object();
        public static MemoEdit _logMemo;

        public static void AddLog(string level, string message)
        {
            lock(lockObj)
            {
                if(logCapture)
                {
                    string lvlName = level.Trim().ToUpper();
                    string newMessage = (DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + " | " + lvlName + " | " + message + Environment.NewLine);
                    UpdateLogCash(newMessage, ref _logText);
                    logChanged = true;
                }
            }
        }

        private static void UpdateLogCash(string newMessage, ref string logText)
        {
            logText += newMessage;
            if(logText.Length > 1024 * 32)
            {
                int CRLF = logText.IndexOf(Environment.NewLine, 1024);
                logText = logText.Remove(0, CRLF + 1);
            }
        }

        private static void UpdateLogMsg(MemoEdit logMemo, string logText)
        {
            if(logMemo != null)
            {
                logMemo.Properties.Appearance.BeginUpdate();
                try
                {
                    lock (lockObj)
                    {
                        logMemo.Text = logText;
                        SetLogCaret(logMemo, logText.Length);
                        logChanged = false;
                    }
                }
                finally
                {
                    logMemo.Properties.Appearance.EndUpdate();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(logChanged)
            {
                try
                {
                    if (logCapture)
                    {
                        if(_logMemo.InvokeRequired)
                        {
                            _logMemo.Invoke(new Action(() =>
                            {
                                UpdateLogMsg(_logMemo, _logText);
                            }));
                        }
                        else
                        {
                            UpdateLogMsg(_logMemo, _logText);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private static void SetLogCaret(MemoEdit logMemo, int length = 0)
        {
            logMemo.Reset();
            logMemo.SelectionStart = length;
            logMemo.ScrollToCaret();
        }
        #endregion

       // BindingList<ScriptTree> scriptTree = new BindingList<ScriptTree>();

        RootScript script = null;

        string fileName = $"{Application.StartupPath}\\Data\\Script\\Default.json";
        public static string fileNameXml = $"{Application.StartupPath}\\Data\\Associations.xml";
        public static string fileNameScript = $"{Application.StartupPath}\\Data\\ModeficationFile";

        public static List<GXDLMSObjectCollection> objectsSpodes = new List<GXDLMSObjectCollection>();

        public static List<ScriptTree> pluginBases = new List<ScriptTree>();

        RootScript _rootScript = new RootScript();

        JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
           // PreserveReferencesHandling = PreserveReferencesHandling.Objects
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        Logger IPluginHost.Log { get => Helpers.Log; set => Helpers.Log = value; }

        public ScriptSettingsMain()
        {
            InitializeComponent();

            _logMemo = logMemo;
            timer1.Enabled = true;

            scriptTreeBindingSource.DataSource = RootScript.scriptTree;
            if(script == null)
            {
                script = new RootScript();
                string json = File.ReadAllText($"{Application.StartupPath}\\Data\\Device.json");
                script.Device =  JsonConvert.DeserializeObject<GXDLMSDevice>(json);
            }
            else
            {
                Helpers.Log.Debug(JsonConvert.SerializeObject(script, Formatting.Indented, serializerSettings));
            }

            LoadFileAssociations();
        }

        private void ScriptSettingsMain_Load(object sender, EventArgs e)
        {
            Helpers.LoadPlugins(this, ".PlugIn");
        }

        //Добавить группу 
        private void btAddGroup_ItemClick(object sender, ItemClickEventArgs e)
        {
            int nextID = RootScript.scriptTree.Count > 0 ? RootScript.scriptTree.Max(item => item.ID) + 1 : 1;
            TreeListNode focuseNode = treeList1.FocusedNode;
            ScriptTree focusetTree = (ScriptTree)treeList1.GetDataRecordByNode(focuseNode);

            PluginBase newPlagin = (PluginBase)Activator.CreateInstance((focusetTree == null) ? RootScript._plugins[0].GetType() : focusetTree.Step.GetType()),
                parentPlugin = RootScript.scriptTree.FirstOrDefault(item => item.ID == focusetTree?.ParentID)?.Step;
            newPlagin.Host = this;

            ScriptTree newItem = new ScriptTree(nextID, focusetTree?.ParentID, newPlagin);
            RootScript.scriptTree.Add(newItem);

            //if (RootScript.scriptTree.FirstOrDefault(x => x.ParentID == null).Name == newPlagin.Name)
            //{
            //    RootScript.scriptTree.Remove(newItem);
            //}
            //else
            //{
            //    RootScript.scriptTree.Add(newItem);
            //}

            AddStepDig(parentPlugin, newItem);
        }

        private void AddStepDig(PluginBase parentPlugin, ScriptTree newItem)
        {
            newItem.Step.Parent = parentPlugin;
            using (ScriptTreeForm dig = new ScriptTreeForm(newItem))
            {
                if (dig.ShowDialog() == DialogResult.OK)
                {
                    TreeListNode focusedNode = treeList1.FocusedNode;
                    ScriptTree focusedItem = (ScriptTree)treeList1.GetDataRecordByNode(focusedNode);
                    PluginBase parentPl = focusedItem?.Step;

                    var ind = RootScript.scriptTree.Max(x => x.ID);

                    var obj = RootScript.scriptTree.FirstOrDefault(x => x.ID == ind);

                    foreach (var item in obj.Step.InternalSteps)
                    {

                        int purId = RootScript.scriptTree.Count > 0 ? RootScript.scriptTree.Max(index => index.ID) + 1 : 1;

                        // Находим дочерние элементы для текущего focusedItem
                        ScriptTree childrens = RootScript.scriptTree.FirstOrDefault(x => x.ParentID == focusedItem.ID);

                        PluginBase newPlugin = (PluginBase)Activator.CreateInstance((childrens?.Step.InternalSteps == null) ? RootScript._plugins[0].GetType() : childrens.Step.InternalSteps.GetType());

                        newPlugin.Host = this;

                        var inID = RootScript.scriptTree.LastOrDefault(x => x.ParentID == null);

                        ScriptTree newItems = new ScriptTree(purId, inID.ID, item);
                        RootScript.scriptTree.Add(newItems);
                   //     newItems.Step.Parent = parentPl;
                        treeList1.FocusedNode = treeList1.FindNodeByKeyID(newItems.ID);
                        //scriptTreeBindingSource.DataSource = RootScript.scriptTree;
                        //scriptTreeBindingSource.ResetBindings(false);
                    }
                }
            }
        }



        int id = 0;
        private void AddChildren(int iD, PluginBase step)
        {
            if(step is PluginBase pluginBase)
            {
                if(pluginBase.InternalSteps != null)
                {
                    foreach (var subItem in ((PluginBase)step).InternalSteps)
                    {
                        //RootScript.scriptTree.Add(new ScriptTree(++id, iD, subItem));
                        //AddChildren(id, subItem);
                    }
                }
            }
        }

        //Просмотр созданного скрипта
        private void btViewingScript_ItemClick(object sender, ItemClickEventArgs e)
        {
            string jsonText = UpdateStringScript();
            using (ScriptViewingText pad = new ScriptViewingText(jsonText))
            {
                pad.ShowDialog();
            }
        }


        private string UpdateStringScript(int? parentID = null)
        {
            if (!parentID.HasValue)
                _rootScript.Clear();

            PluginBase parent = RootScript.scriptTree.FirstOrDefault(x => x.ID == parentID)?.Step;

            foreach(var item in RootScript.scriptTree.Where(x => x.ParentID == parentID))
            {
               // item.Step.Parent = parent;

                if(parent == null)
                {
                    _rootScript.Add(item.Step);
                }

                item.Step.InternalSteps = RootScript.scriptTree.Where(x => x.ParentID == item.ID).Select(pl => pl.Step).ToList();

                UpdateStringScript(item.ID);
            }

            return JsonConvert.SerializeObject(_rootScript, Formatting.Indented, serializerSettings);
        }

        private string UpdateTaskScript(int? parentID = null)
        {
            if(!parentID.HasValue)
            {
                script.Clear();
            }

            PluginBase parent = RootScript.scriptTree.FirstOrDefault(item => item.ID == parentID)?.Step;

            PluginBase parent2 = RootScript.scriptTree.FirstOrDefault(item => item.ID == 1)?.Step;

            var tt = RootScript.scriptTree.Where(scriptTree => scriptTree.ParentID == parentID).ToList();

            foreach (var item in RootScript.scriptTree.Where(scriptTree => scriptTree.ParentID == parentID))
            {
                if(parent == null)
                {
                    script.Add(item.Step);
                }
                item.Step.Childrens = RootScript.scriptTree.Where(child => child.ParentID == item.ID).Select(plugin => plugin.Step).ToArray();
                UpdateTaskScript(item.ID);
            }


            return JsonConvert.SerializeObject(script, Formatting.Indented, serializerSettings);
        }

        public bool Register(PluginBase plug)
        {
            return true;
        }

        //private void LoadPlugins(string startupPath)
        //{
        //    RootScript._plugins = new List<PluginBase>();
        //    Helpers.Log.Info("Загрузка действующих плагинов");
        //    string[] pluginFields = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory), "*.dll");
        //    foreach(string pluginsPatch in pluginFields)
        //    {
        //        Type typePlugin = null;
        //        try
        //        {
        //            Assembly assembly = Assembly.LoadFrom(pluginsPatch);
        //            if(assembly != null)
        //            {
        //                typePlugin = assembly.GetType(Path.GetFileNameWithoutExtension(pluginsPatch) + ".PlugIn");
        //            }
        //        }
        //        catch
        //        {
        //            continue;
        //        }
        //        try
        //        {
        //            if(typePlugin != null)
        //            {
        //                foreach(var plaginType in typePlugin.Assembly.DefinedTypes.Where(type => type.IsSubclassOf(typeof(PluginBase))))
        //                {
        //                    var plugin = (PluginBase)Activator.CreateInstance(plaginType);
        //                    plugin.Host = this;
        //                    (plugin as PluginBase)?.AddLog(plugin);
        //                    RootScript._plugins.Add(plugin);
        //                }
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            Helpers.Log.Error("Плагин не создан");
        //            continue;
        //        }
        //    }
        //}

        public static void LoadFileAssociations()
        {
            try
            {
                var typesSpodes = new List<Type>(GXDLMSClient.GetObjectTypes())
                    {
                        typeof(GXDLMSAttributeSettings),
                        typeof(GXDLMSAttribute)
                    };

                var x = new XmlSerializer(typeof(GXDLMSObjectCollection), typesSpodes.ToArray());

                using (Stream stream = (Stream)File.Open(fileNameXml, FileMode.Open))
                {
                    var attributeSettings = x.Deserialize(stream) as GXDLMSObjectCollection;

                    int objId = 0;

                    foreach (var item in attributeSettings)
                    {
                        string desc = $"{item.LogicalName}: " + item.Description;

                        var attr = item.Attributes;

                        List<int> attributes = new List<int>();

                        foreach (var i in item.Attributes)
                        {
                            attributes.Add(i.Index);
                        }

                        int objType = (int)ObjectType.None;

                        objId++;

                        switch (item.ObjectType)
                        {
                            case ObjectType.Data:
                            case ObjectType.Register:
                            case ObjectType.ExtendedRegister:
                            case ObjectType.DemandRegister:
                                objType = 2;
                                break;
                            case ObjectType.ProfileGeneric:
                                objType = 6;
                                break;
                            case ObjectType.Limiter:
                                objType = 8;
                                break;
                        }
                        Spodes spodes = new Spodes()
                        {
                            ID = objId,
                            DescRU = desc,
                            OBIS = item.LogicalName,
                            ParentID = 999999,
                            ObjectType = objType,
                            Attr = attributes
                        };

                        spodes.DevObject = item;

                        Spodes.spodes.Add(spodes);
                    }
                }

                Helpers.Log.Info($"Загружен файл ассоциаций: {fileNameXml}");
            }
            catch(Exception ex)
            {
                Helpers.Log.Error($"Файл не был загружен: {ex.Message}");
            }
        }

        private void btLoadAssFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            using(OpenFileDialog dig = new OpenFileDialog())
            {
                if (dig.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(dig.FileName, fileNameXml, true);

                    LoadFileAssociations();
                }
            }
        }

        private void treeList1_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            bool isValid = false;
            panelControl.Controls.Clear();
            if (e.Node != null)
            {
                try
                {
                    if(e.Node.GetValue(colStep) is PluginBase pluginBase)
                    {
                        pluginBase.Host = this;
                        if(pluginBase.EditControl is UserControl userControl)
                        {
                            UserControl control = (UserControl)Activator.CreateInstance(userControl.GetType(), new object[] { pluginBase });

                            control.Parent = panelControl;
                            control.Dock = DockStyle.Fill;
                        }
                        isValid = true;
                    }
                }
                catch(Exception ex)
                {

                }
            }

        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach(var item in RootScript.scriptTree)
            {
                var step = (ISteps)item.Step;

                if(step != null)
                {
                    try
                    {
                        Helpers.Log.Trace($"Тестирование запуска плагина [{step}]");

                        step.ExecuteStep(null);
                    }
                    catch(Exception ex)
                    {
                        Helpers.Log.Error($"Ошибка выполнения плагина [{step}]: {ex.Message}");
                    }
                }
            }
        }

        private void btSaveSetFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string jsonText = UpdateStringScript();


                File.WriteAllText($"{fileNameScript}\\{textNameScript.Text}.script", jsonText);
            }
            catch(Exception ex)
            {
                Helpers.Log.Error($"Файл не был записан [{fileNameScript}\\{textNameScript.Text}]: {ex.Message}");
            }
        }
    }
}
