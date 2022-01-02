#region Using directives
using System.Collections.Generic;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class Block2D
    {
        #region Contructor
        public Block2D(Vector3D itemDimension)
        {
            ItemDimension = itemDimension;
        }
        #endregion
        #region Public properties
        public Vector2D Dimensions
        {
            get
            {
                BBox2D box = BBox2D.Initial;
                foreach (var pos in Positions)
                {
                     box.Extend(pos.BBox(ItemDimension).ToBBox2D());
                }
                return box.Dimensions; 
            }
        }
        public bool LastStandingOut(double percentage) => false;
        public Block2D[] SplitInTwo(double percentage)
        { 
            return new Block2D[] { new Block2D(ItemDimension), new Block2D(ItemDimension) }; 
        }
        #endregion
        #region Data members
        public List<BoxPosition> Positions { get; set; }
        public Vector3D ItemDimension { get; set; }
        #endregion
    }
}
