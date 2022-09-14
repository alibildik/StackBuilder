#region Using directives
using System.Collections.Generic;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    #region ILayer2D
    public interface ILayer2D
    {
        #region Properties
        string PatternName { get; }
        string Name { get; }
        bool Swapped { get; }
        double LayerHeight { get; }
        int Count { get; }
        double Length { get; }
        double Width { get; }
        LayerDesc LayerDescriptor { get; }
        double MaximumSpace { get; }
        BBox3D BBox { get; }
        double AreaEfficiency { get; }
        #endregion
        #region Methods
        void Clear();
        int CountInHeight(double height);
        int NoLayers(double height);
        string Tooltip(double height);
        void UpdateMaxSpace(double space, string patternName);
        #endregion
    }
    #endregion
    #region Comparer for Layer2D (LayerComparerCount)
    public class LayerComparerCount : IComparer<ILayer2D>
    {
        #region Data members
        private readonly double _offsetZ = 0;
        private ConstraintSetAbstract ConstraintSet { get; set; }
        #endregion

        #region Constructor
        public LayerComparerCount(ConstraintSetAbstract constraintSet, double offsetZ)
        {
            _offsetZ = offsetZ;
            ConstraintSet = constraintSet;
        }
        #endregion

        #region Public properties
        public bool AllowMultipleLayers
        {
            get
            {
                if (ConstraintSet.OptMaxLayerNumber.Activated)
                    return ConstraintSet.OptMaxLayerNumber.Value <= 1;
                return true;
            }
        }
        public double Height { get { return ConstraintSet.OptMaxHeight.Value - _offsetZ; } }
        #endregion

        #region Implement IComparer
        public int Compare(ILayer2D layer0, ILayer2D layer1)
        {
            int layer0Count = AllowMultipleLayers ? layer0.CountInHeight(Height) : layer0.Count;
            int layer1Count = AllowMultipleLayers ? layer1.CountInHeight(Height) : layer1.Count;

            if (layer0Count < layer1Count) return 1;
            else if (layer0Count == layer1Count)
            {
                if ((layer0 is Layer2DBrickImp layerBox0) && (layer1 is Layer2DBrickImp layerBox1))
                {
                    if (layerBox0.AxisOrtho < layerBox1.AxisOrtho)
                        return 1;
                    else if (layerBox0.AxisOrtho == layerBox1.AxisOrtho)
                        return 0;
                    else return -1;
                }
                else
                    return 0;
            }
            else return -1;
        }
        #endregion
    }
    #endregion
}
