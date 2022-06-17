namespace treeDiM.StackBuilder.TechnologyBSA_ASPNET
{
    public class LayerDataShort
    {
        public LayerDataShort(string name, int layerIndex, bool hasInterlayer)
        {
            Name = name;
            LayerIndex = layerIndex;
            HasInterlayer = hasInterlayer;
        }
        public string Name { get; set; }
        public int LayerIndex { get; set; }
        public bool HasInterlayer { get; set; }
    }
}