using Newtonsoft.Json;
using System.Collections.Generic;

namespace Stend.Production.Root
{
    public class RootScriptObject
    {
        [JsonProperty("Script")]
        public Parameterisation Script { get; set; }
    }
    public class Parameterisation
    {
        List<PluginBase> steps = null;
        public List<PluginBase> baseStep { get => steps; set => steps = value; }
    }
}
