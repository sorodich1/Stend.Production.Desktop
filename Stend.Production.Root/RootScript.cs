using Newtonsoft.Json;
using NLog;
using Stend.Pruduction;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ZIP.DLMS;

namespace Stend.Production.Root
{
    public class RootScript
    {
        [JsonIgnore]
        public static BindingList<ScriptTree> scriptTree = new BindingList<ScriptTree>();

        [JsonIgnore]
        public static BindingList<PluginBase> pluginBase = new BindingList<PluginBase>();

        [JsonIgnore]
        public static List<PluginBase> childrens = new List<PluginBase>();

        [JsonIgnore]
        public static Spodes Spodes { get; set; }

        [JsonIgnore]
        public static List<PluginBase> _plugins;

        [JsonIgnore]
        public GXDLMSDevice Device { get; set; }

        [JsonIgnore]
        public Logger Log { get; set; }

        string _name = "Скрипт производственной настройки";

        public string Name { get => _name; set => _name = value; }

        List<PluginBase> steps = new List<PluginBase>();

        public PluginBase[] Steps
        {
            get => steps.ToArray();
            set => steps = value?.ToList();
        }

        public void Add(PluginBase step)
        {
            steps.Add(step);
        }

        public void Clear()
        {
            steps = new List<PluginBase>();
        }

        public RootScript(GXDLMSDevice device)
        {
            Device = device;
        }

        public RootScript()
        {
            Device = null;
        }

        public RootScript Clone()
        {
            RootScript script = new RootScript();
            try
            {
                script.Name = this.Name;
                script.Device = this.Device;
                script.Log = this.Log;
                script.steps.AddRange(this.steps);
            }
            catch
            {
                script = null;
            }
            return script;
        }

        public void Execute()
        {
            foreach(var step in steps)
            {
                step.ExecuteStep(Device);
            }
        }
    }
}
