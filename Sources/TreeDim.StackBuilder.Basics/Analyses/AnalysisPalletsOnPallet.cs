#region Using directives
using System.Collections.Generic;
using System.Linq;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class AnalysisPalletsOnPallet : Analysis
    {
        #region Constructor
        public AnalysisPalletsOnPallet(Document doc,
            EMasterPalletSplit masterPalletSplit, ELoadedPalletOrientation loadedPalletOrientation,
            PalletProperties masterPallet,
            LoadedPallet loadedPallet0, LoadedPallet loadedPallet1,
            LoadedPallet loadedPallet2 = null, LoadedPallet loadedPallet3 = null)
            : base(doc)
        {
            Solution = new SolutionPalletsOnPallet() { Analysis = this };
            MasterPalletSplit = masterPalletSplit;
            LoadedPalletOrientation = loadedPalletOrientation;
            PalletProperties = masterPallet;
            if (null == loadedPallet2 && null == loadedPallet3)
                SetHalfPallets(loadedPallet0, loadedPallet1);
            else
                SetQuarterPallets(loadedPallet0, loadedPallet1, loadedPallet2, loadedPallet3);
        }
        #endregion
        #region Analysis override
        public override bool HasValidSolution => null != PalletAnalyses[0] && null != PalletAnalyses[1]
            && ((null != PalletAnalyses[2] && null != PalletAnalyses[3])
            || (null == PalletAnalyses[2] && null == PalletAnalyses[3]));
         protected override void RemoveItselfFromDependancies()
        {
            base.RemoveItselfFromDependancies();
            foreach (var p in PalletAnalyses)
                p?.RemoveDependancy(this);
        }
        #endregion
        #region Public properties
        public SolutionPalletsOnPallet Solution { get; set; }
        public PalletProperties PalletProperties { get; set; }
        public ItemBase Container => PalletProperties;
        #endregion
        #region Mode
        public enum EMode { PALLET_HALF, PALLET_QUARTER }
        public EMode Mode { get; set; } = EMode.PALLET_HALF;
        public int NoLoadedPallets => Mode == EMode.PALLET_HALF ? 2 : 4;
        public void SetHalfPallets(LoadedPallet loadedPallet0, LoadedPallet loadedPallet1)
        {
            Mode = EMode.PALLET_HALF;
            PalletAnalyses[0] = loadedPallet0;
            SetPalletAnalysis(1, loadedPallet1);
            PalletAnalyses[2] = null;
            PalletAnalyses[3] = null;            
        }
        public void SetQuarterPallets(
            LoadedPallet loadedPallet0, LoadedPallet loadedPallet1,
            LoadedPallet loadedPallet2, LoadedPallet loadedPallet3)
        {
            Mode = EMode.PALLET_QUARTER;
            PalletAnalyses[0] = loadedPallet0;
            SetPalletAnalysis(1, loadedPallet1);
            SetPalletAnalysis(2, loadedPallet2);
            SetPalletAnalysis(3, loadedPallet3);
        }
        #endregion
        #region Content accessing methods
        private void SetPalletAnalysis(int index, LoadedPallet loadedPallet)
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

                listInnerPackables.Add(new Pair<Packable, int>(lp, iCount+1));
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
                for (int i = 0; i < 4; ++i)
                {
                    if (null != PalletAnalyses[i])
                        weight += PalletAnalyses[i].ParentAnalysis.Solution.LoadWeight;
                }
                return weight;
            }
        }
        #endregion
        #region Enums
        public enum EMasterPalletSplit          { HORIZONTAL, VERTICAL };
        public enum ELoadedPalletOrientation    { DEFAULT, ROTATED };
        #endregion
        #region Public data members
        public readonly LoadedPallet[] PalletAnalyses = new LoadedPallet[4];
        public EMasterPalletSplit MasterPalletSplit { get; set; } = EMasterPalletSplit.VERTICAL;
        public ELoadedPalletOrientation LoadedPalletOrientation { get; set; } = ELoadedPalletOrientation.ROTATED;

        public HalfAxis.HAxis Axis0 => LoadedPalletOrientation == ELoadedPalletOrientation.DEFAULT ? HalfAxis.HAxis.AXIS_X_P : HalfAxis.HAxis.AXIS_Y_P;
        public HalfAxis.HAxis Axis1 => LoadedPalletOrientation == ELoadedPalletOrientation.DEFAULT ? HalfAxis.HAxis.AXIS_Y_P : HalfAxis.HAxis.AXIS_X_N;
        #endregion
    }

    public class SolutionPalletsOnPallet
    { 
        public AnalysisPalletsOnPallet Analysis { get; set; }
        public List<HSolElement> ContainedElements
        {
            get
            {
                double length = Analysis.PalletProperties.Length;
                double halfLength = 0.5 * Analysis.PalletProperties.Length;
                double halfWidth = 0.5 * Analysis.PalletProperties.Width;
                double height = Analysis.PalletProperties.Height;

                HalfAxis.HAxis axis0 = Analysis.Axis0, axis1 = Analysis.Axis1;

                Vector3D[] coord = new Vector3D[4];
                switch (Analysis.Mode)
                {
                    case AnalysisPalletsOnPallet.EMode.PALLET_HALF:
                        {
                            if (Analysis.MasterPalletSplit == AnalysisPalletsOnPallet.EMasterPalletSplit.HORIZONTAL
                                && Analysis.LoadedPalletOrientation == AnalysisPalletsOnPallet.ELoadedPalletOrientation.DEFAULT)
                            {
                                coord[0] = new Vector3D(0.0, 0.0, height);
                                coord[1] = new Vector3D(0.0, halfWidth, height);
                            }
                            else if (Analysis.MasterPalletSplit == AnalysisPalletsOnPallet.EMasterPalletSplit.HORIZONTAL
                                && Analysis.LoadedPalletOrientation == AnalysisPalletsOnPallet.ELoadedPalletOrientation.ROTATED)
                            {
                                coord[0] = new Vector3D(length, 0.0, height);
                                coord[1] = new Vector3D(length, halfWidth, height);
                            }
                            else if (Analysis.MasterPalletSplit == AnalysisPalletsOnPallet.EMasterPalletSplit.VERTICAL
                                && Analysis.LoadedPalletOrientation == AnalysisPalletsOnPallet.ELoadedPalletOrientation.DEFAULT)
                            {
                                coord[0] = new Vector3D(0.0, 0.0, height);
                                coord[1] = new Vector3D(halfLength, 0.0, height);
                            }
                            else
                            {
                                coord[0] = new Vector3D(halfLength, 0.0, height);
                                coord[1] = new Vector3D(length, 0.0, height);
                            }
                        }
                        break;
                    case AnalysisPalletsOnPallet.EMode.PALLET_QUARTER:
                        {
                            if (Analysis.LoadedPalletOrientation == AnalysisPalletsOnPallet.ELoadedPalletOrientation.DEFAULT)
                            {
                                coord[0] = new Vector3D(0.0, 0.0, height);
                                coord[1] = new Vector3D(halfLength, 0.0, height);
                                coord[2] = new Vector3D(0.0, halfWidth, height);
                                coord[3] = new Vector3D(halfLength, halfWidth, height);
                            }
                            else
                            {
                                coord[0] = new Vector3D(0.0, 0.0, height);
                                coord[1] = new Vector3D(halfLength, 0.0, height);
                                coord[2] = new Vector3D(0.0, halfWidth, height);
                                coord[3] = new Vector3D(halfLength, halfWidth, height);
                            }
                        }
                        break;
                    default:
                        break;
                }
                List<HSolElement> list = new List<HSolElement>();
                for (int i=0; i<Analysis.NoLoadedPallets; ++i)
                    list.Add(
                        new HSolElement()
                    {
                        ContentType = i,
                        Position = new BoxPosition(coord[i], axis0, axis1)
                    }
                    );
                return list;
            }
        }
        public BBox3D BBoxLoad
        {
            get
            {
                var bbox = new BBox3D();
                foreach (var solElt in ContainedElements)
                {
                    LoadedPallet loadedPallet = Analysis.PalletAnalyses[solElt.ContentType];
                    var dim = loadedPallet.Dimensions;
                    bbox.Extend(solElt.Position.BBox(new Vector3D(dim[0], dim[1], dim[2])));
                }
                return bbox;
            }
        }
        public BBox3D BBoxGlobal
        {
            get
            {
                var bbox = BBoxLoad;
                bbox.Extend(Analysis.PalletProperties.BoundingBox);
                return bbox;
            }
        }
        public BBox3D BBoxLoadWDeco => BBoxLoad;
        public double LoadWeight => ContainedElements.Sum(ce => Analysis.PalletAnalyses[ce.ContentType].Weight);
        public double Weight => LoadWeight + Analysis.PalletProperties.Weight;
    }
}
