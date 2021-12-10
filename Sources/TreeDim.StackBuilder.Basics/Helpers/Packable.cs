using System;
using System.Text;
using System.Collections.Generic;

using Sharp3D.Math.Core;

using treeDiM.Basics;

namespace treeDiM.StackBuilder.Basics
{
    public abstract class Packable : ItemBase
    {
        public Packable(Document doc)
            : base(doc)
        {
        }

        /// <summary>
        /// Is actually formatted as "TypeName(Name)"
        /// </summary>
        public virtual string DetailedName => $"{TypeName}({ID.Name})";
        public abstract double Weight { get; }
        public abstract OptDouble NetWeight { get; }
        public abstract double Volume { get; }
        public abstract Vector3D OuterDimensions { get; }
        public virtual Vector3D COG => 0.5 * OuterDimensions;
        public virtual bool IsCase => false;
        public virtual bool IsPallet => false;
        public virtual bool IsTruck => false;
        public abstract bool IsBrick { get; }
        public bool IsCylinder => !IsBrick;

        public virtual bool InnerAnalysis(ref AnalysisHomo analysis)
        {
            analysis = null;
            return false;
        }
        public virtual bool InnerContent(ref List<Pair<Packable, int>> listInnerPackables)
        { listInnerPackables = null; return false; }

        public bool FitsIn(IContainer container, ConstraintSetAbstract constraintSet)
        {
            Vector3D vPackable = OuterDimensions;
            Vector3D vContainer = container.GetStackingDimensions(constraintSet);

            if (constraintSet.AllowOrientation(HalfAxis.HAxis.AXIS_X_N) || constraintSet.AllowOrientation(HalfAxis.HAxis.AXIS_X_P))
            {
                if ((vPackable.X <= vContainer.Z)
                    && (
                    (vPackable.Y <= vContainer.X && vPackable.Z <= vContainer.Y)
                    || (vPackable.Z <= vContainer.X && vPackable.Y <= vContainer.Y)
                    ))
                    return true;
            }
            if (constraintSet.AllowOrientation(HalfAxis.HAxis.AXIS_Y_N) || constraintSet.AllowOrientation(HalfAxis.HAxis.AXIS_Z_P))
            {
                if ((vPackable.Y <= vContainer.Z)
                    && (
                    (vPackable.X <= vContainer.X && vPackable.Z <= vContainer.Y)
                    || (vPackable.Z <= vContainer.X && vPackable.X <=vContainer.Y)
                    ))
                    return true;
            }
            if (constraintSet.AllowOrientation(HalfAxis.HAxis.AXIS_Z_N) || constraintSet.AllowOrientation(HalfAxis.HAxis.AXIS_Z_P))
            {
                if ((vPackable.Z <= vContainer.Z)
                    && (
                    (vPackable.X <= vContainer.X && vPackable.Y <= vContainer.Y)
                    || (vPackable.Y <= vContainer.X && vPackable.X <= vContainer.Y)
                    ))
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            var sBuilder = new StringBuilder();
            sBuilder.Append(base.ToString());
            sBuilder.Append($"Packable => Weight = {Weight}  NetWeight = {NetWeight}\n");
            return sBuilder.ToString();
        }

        #region Non-Public Members

        protected abstract string TypeName { get; }

        #endregion
    }
}