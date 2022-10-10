#region Using directives
using System.Collections.Generic;
using System.Linq;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class AnalysisPalletColumn : Analysis
    {
        #region Constructor
        public AnalysisPalletColumn(Document doc, LoadedPallet pallet0, LoadedPallet pallet1 = null)
            : base(doc)
        {
            Solution = new SolutionPalletColumn() { Analysis = this };
            PalletAnalyses[0] = pallet0;
            PalletAnalyses[1] = pallet1;
        }
        #endregion
        #region Analysis override
        public override bool HasValidSolution => null != PalletAnalyses[0];
        protected override void RemoveItselfFromDependancies()
        {
            base.RemoveItselfFromDependancies();
            foreach (var p in PalletAnalyses)
                p?.RemoveDependancy(this);
        }
        #endregion
        #region Content accessing methods
        public void SetPalletAnalysis(int index, LoadedPallet loadedPallet)
        {
            if (null != loadedPallet)
            {
                for (int i = 0; i < index; ++i)
                    if (PalletAnalyses[i].ParentAnalysis == loadedPallet.ParentAnalysis)
                    {
                        PalletAnalyses[index] = PalletAnalyses[i];
                        return;
                    }
            }
            PalletAnalyses[index] = loadedPallet;
        }
        public LoadedPallet ContentTypeByIndex(int index) => PalletAnalyses[index];
        public virtual bool InnerContent(ref List<Pair<Packable, int>> listInnerPackables)
        {
            if (null == listInnerPackables)
                listInnerPackables = new List<Pair<Packable, int>>();

            foreach (var lp in PalletAnalyses)
            {
                if (null == lp) continue;
                var analysis1 = lp.ParentAnalysis;

                int iCount = 0;
                int index = -1;
                foreach (var ip in listInnerPackables)
                {
                    var packable = ip.first;
                    if (packable is LoadedPallet lp2 && lp2.ParentAnalysis == analysis1)
                        iCount += ip.second;
                    ++index;
                }
                if (iCount > 0)
                    listInnerPackables.RemoveAt(index);

                listInnerPackables.Add(new Pair<Packable, int>(lp, iCount + 1));
            }

            return listInnerPackables.Count > 0;
        }
        public virtual bool InnerAnalyses(ref List<AnalysisHomo> analyses)
        {
            foreach (var lp in PalletAnalyses)
            {
                if (null == lp) continue;
                var analysis1 = lp.ParentAnalysis;
                if (!analyses.Contains(analysis1))
                    analyses.Add(analysis1);
            }
            return analyses.Count > 0;
        }
        public double LoadWeight
        {
            get
            {
                double weight = 0.0;
                for (int i=0; i<PalletAnalyses.Length; ++i)
                    weight += PalletAnalyses[i].Weight;
                return weight;
            }
        }
        #endregion
        #region Public Data members
        public SolutionPalletColumn Solution { get; set; }
        public readonly LoadedPallet[] PalletAnalyses = new LoadedPallet[2];
        #endregion
    }
    public class SolutionPalletColumn
    {
        public SolutionPalletColumn() {}
        public AnalysisPalletColumn Analysis { get; set; }
        public List<LoadedPallet> Pallets
        {
            get
            { 
                var pallets = new List<LoadedPallet>();
                for (int i = 0; i < Analysis.PalletAnalyses.Length; ++i)
                {
                    var loadedPallet = Analysis.PalletAnalyses[i];
                    if (loadedPallet != null)
                        pallets.Add(loadedPallet);
                }
                return pallets;
            }
        } 
        public List<HSolElement> ContainedElements
        {
            get
            {
                List<HSolElement> list = new List<HSolElement>();
                double length = Length;
                double width = Width;
                double z = 0.0;

                foreach (var pallet in Pallets)
                {
                    list.Add(
                        new HSolElement() {
                            Position = new BoxPosition()
                            {
                                Position = new Vector3D(
                                    0.5 * (pallet.Length - length)
                                    , 0.5 * (pallet.Width - width)
                                    , z)
                                , DirectionLength = HalfAxis.HAxis.AXIS_X_P
                                , DirectionWidth = HalfAxis.HAxis.AXIS_Y_P
                            }
                    });
                    z += pallet.Height;
                }
                return list;
            }
        }
        public BBox3D BBoxGlobal
        {
            get
            {
                var bbox = new BBox3D();
                foreach (var solElt in ContainedElements)
                {
                    var loadedPallet = Analysis.PalletAnalyses[solElt.ContentType];
                    var dim = loadedPallet.Dimensions;
                    bbox.Extend(solElt.Position.BBox(new Vector3D(dim[0], dim[1], dim[2])));
                }
                return bbox;
            }
        }
        public double LoadWeight => ContainedElements.Sum(ce => Analysis.PalletAnalyses[ce.ContentType].Weight);
        public double Length => (from pal in Pallets select pal.Length).Max();
        public double Width => (from pal in Pallets select pal.Width).Max();
        public double Height => (from pal in Pallets select pal.Height).Sum();
        public double Weight => (from pal in Pallets select pal.Weight).Sum();
    }
}
