using Stend.Production.Root;

namespace Stend.Pruduction
{
    public class ScriptTree
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public int Order { get; set; } = 0;
        public string Name { get => Step?.StepName; }
        public string Description { get => Step?.Description; }
        public PluginBase Step { get; set; }

        public ScriptTree(int id, int? parentId, PluginBase pluginBase)
        {
            ID = id;
            ParentID = parentId;
            Step = pluginBase;
            Order = pluginBase.Order;
        }
    }
}
