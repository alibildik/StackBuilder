#region Using directives
using Sharp3D.Math.Core;

using treeDiM.Basics;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public abstract class SolutionHomo
    {
        public AnalysisHomo Analysis { get; protected set; }
        public ConstraintSetAbstract ConstraintSet => Analysis.ConstraintSet;
        public BBox3D BBoxLoadWDeco => Analysis.BBoxLoadWDeco(BBoxLoad);
        public BBox3D BBoxGlobal => Analysis.BBoxGlobal(BBoxLoad);
        public double LoadVolume => ItemCount * Analysis.Content.Volume;
        public bool HasNetWeight => Analysis.Content.NetWeight.Activated;
        public double LoadWeight => ItemCount * Analysis.ContentWeight;
        public virtual double InterlayersWeight => 0.0;
        public virtual double Weight => LoadWeight + InterlayersWeight + Analysis.ContainerWeight + Analysis.DecorationWeight;
        public OptDouble NetWeight => ItemCount * Analysis.Content.NetWeight;
        public double VolumeEfficiency => 100.0 * (ItemCount * Analysis.ContentVolume) / Analysis.ContainerLoadingVolume;
        public abstract double AreaEfficiency { get; }
        public OptDouble WeightEfficiency
        {
            get
            {
                if (!Analysis.ConstraintSet.OptMaxWeight.Activated)
                    return new OptDouble(false, 0.0);
                else
                    return new OptDouble(true, 100.0 * (ItemCount * Analysis.ContentWeight) / Analysis.ConstraintSet.OptMaxWeight.Value);
            }
        }
        public Vector3D LoadCOfG => Vector3D.Zero;
        public Vector3D COfG => Vector3D.Zero;

        #region Abstract properties
        public abstract int ItemCount { get; }
        public abstract BBox3D BBoxLoad { get; }
        #endregion
    }
}
