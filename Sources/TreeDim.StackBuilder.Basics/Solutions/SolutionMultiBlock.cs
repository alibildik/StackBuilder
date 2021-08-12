#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using treeDiM.Basics;
#endregion

namespace treeDiM.StackBuilder.Basics.Solutions
{
    public class AnalysisMultiBlock
    {
        public PackableBrickNamed Content { set; get; }
        public double ContainerLoadingVolume { get; }
    }

    internal class Block
    {
        public int ItemCount { get; }
        public BBox3D BBox
        {
            get;
        }
        
    }
    public class SolutionMultiBlock
    {
        public BBox3D BBoxLoadWDeco
        {
            get
            {
                var bbox = BBox3D.Initial;
                return bbox;
            }
        }
        public BBox3D BBoxGlobal => BBox3D.Initial;

        public OptDouble NetWeight => ItemCount * Analysis.Content.NetWeight;
        public double VolumeEfficiency => 100.0;

        public int ItemCount => BlockList.Sum(b => b.ItemCount);

        private List<Block> BlockList { get; set; } = new List<Block>();
        private AnalysisMultiBlock Analysis { get; set; }
    }
}
