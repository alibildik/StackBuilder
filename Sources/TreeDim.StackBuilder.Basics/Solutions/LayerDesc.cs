namespace treeDiM.StackBuilder.Basics
{
    #region LayerDesc
    public abstract class LayerDesc
    {
        #region Constructor
        protected LayerDesc(string patternName, bool swapped)
        {
            PatternName = patternName;
            Swapped = swapped;
        }
        #endregion
        #region Public properties
        public string PatternName { get; }
        public bool Swapped { get; }
        public string SwappedStr => Swapped ? "t" : "f";
        #endregion
        #region Object override
        public override bool Equals(object obj)
        {
            if (!(obj is LayerDesc layerDesc)) return false;
            return PatternName == layerDesc.PatternName && Swapped == layerDesc.Swapped;
        }
        public override int GetHashCode() => PatternName.GetHashCode() ^ Swapped.GetHashCode();
        public override string ToString() => $"{PatternName}_{(Swapped ? "1" : "0")}";
        #endregion
    }
    #endregion
}
