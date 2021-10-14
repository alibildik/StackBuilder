#region Using directives
using System.Linq;
using System.Collections.Generic;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    /// <summary>
    /// Heterogeneous solution
    /// </summary>
    public class HSolution
    {
        public HSolution(string algo) { Algorithm = algo; }
        public IEnumerable<HSolItem> SolItems { get => hSolItems; }
        public IEnumerable<HUnloadedElt> UnloadedElts { get => hUnloadedElts; }
        public AnalysisHetero Analysis { get; set; }
        public HSolItem CreateSolItem() { hSolItems.Add(new HSolItem()); return hSolItems[hSolItems.Count - 1]; }
        public string Algorithm { get; private set; } = string.Empty;
        public int LoadedCasesCount
        {
            get
            {
                int iCount = 0;
                foreach (HSolItem solItem in SolItems)
                    iCount += solItem.Count;
                return iCount;
            }
        }
        public string LoadedCasesCountString
        {
            get
            {
                string sRes = string.Empty;
                int iCount = 0;
                foreach (HSolItem solItem in SolItems)
                {
                    if (0 != iCount) sRes += "+";
                    sRes += string.Format("{0}", solItem.Count);
                    iCount += solItem.Count;
                }
                if (SolItemCount > 1)
                    sRes += string.Format(" = {0}", iCount);
                return sRes;
            }
        }
        public int UnloadedCasesCount => UnloadedElts.ToList().Count;
        public double LoadedVolumePercentage => 100.0;
        public bool HasSolItems => hSolItems.Count > 0;
        public int SolItemCount => hSolItems.Count;
        public HSolItem SolItem(int itemIndex) => hSolItems[itemIndex];
        public virtual BBox3D BBoxGlobal(int solItemIndex)
        {
            BBox3D bbox = new BBox3D();
            bbox.Extend(Analysis.AdditionalBoudingBox(SolItem(solItemIndex).ContainerType));
            bbox.Extend( BBoxLoad(solItemIndex));
            return bbox;
        }
        public virtual BBox3D BBoxLoad(int solItemIndex)
        {
            return SolItem(solItemIndex).BBox(this);
        }
        public virtual double LoadWeight(int solItemIndex)
        {
            return SolItem(solItemIndex).LoadWeight(this);
        }
        public virtual double Weight(int solItemIndex)
        {
            return LoadWeight(solItemIndex) + Analysis.WeightContainer(SolItem(solItemIndex).ContainerType);
        }
        #region Data members
        private readonly List<HSolItem> hSolItems = new List<HSolItem>();
        private readonly List<HUnloadedElt> hUnloadedElts = new List<HUnloadedElt>();
        #endregion
    }
    /// <summary>
    /// Heterogeneous solution item
    /// </summary>
    public class HSolItem
    {
        public int ContainerType { get; set; } = 0;
        public IEnumerable<HSolElement> ContainedElements { get => hSolElt; }

        public void InsertContainedElt(int contentType, BoxPosition bPosition)
        {
            hSolElt.Add(new HSolElement() { ContentType = contentType, Position = bPosition });
        }
        public int Count => hSolElt.Count;
        public BBox3D BBox(HSolution sol)
        {
            AnalysisHetero analysis = sol.Analysis;
            BBox3D bbox = new BBox3D();
            foreach (var solElt in ContainedElements)
                bbox.Extend(solElt.Position.BBox(analysis.ContentTypeByIndex(solElt.ContentType).OuterDimensions));
            return bbox;
        }
        public void Recenter(HSolution hSol, Vector3D dims, Vector2D overhang)
        {
            BBox3D bbox = BBox(hSol);
            Vector3D offset = new Vector3D((0.5 * dims.X - bbox.Center.X - overhang.X),  (0.5 *dims.Y - bbox.Center.Y - overhang.Y), 0.0);
            foreach (var solElt in hSolElt)
                solElt.Position = solElt.Position.Transform( Transform3D.Translation(offset) );
        }
        public double LoadWeight(HSolution sol)
        {
            AnalysisHetero analysis = sol.Analysis;
            return ContainedElements.Sum(item => analysis.ContentTypeByIndex(item.ContentType).Weight);
        }
        public Dictionary<int, int> SolutionItems
        {
            get
            {
                var solutionItems = new Dictionary<int, int>();
                foreach (var solElt in ContainedElements)
                {
                    int no = 0;
                    if (solutionItems.Keys.Contains(solElt.ContentType))
                        no = solutionItems[solElt.ContentType];
                    solutionItems[solElt.ContentType] = no + 1;
                }
                return solutionItems;
            }
        }      
        private readonly List<HSolElement> hSolElt = new List<HSolElement>();
    }
    /// <summary>
    /// Solution element
    /// </summary>
    public class HSolElement
    {
        public int ContentType { get; set; }
        public BoxPosition Position { get; set; }
    }
    /// <summary>
    /// Unloaded element
    /// </summary>
    public class HUnloadedElt
    {
        public int ContentType { get; set; }
        public int Number { get; set; }
    }
}
