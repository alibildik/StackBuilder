#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Sharp3D.Math.Core;

using log4net;

using treeDiM.Basics;
using MIConvexHull;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    #region ILayerSolver
    public interface ILayerSolver
    {
        LayerDesc BestLayerDesc(Vector3D dimBox, Vector3D bulge, Vector2D dimContainer, double offsetZ, ConstraintSetAbstract constraintSet);
        List<Layer2DBrickImp> BuildLayers(Vector3D dimBox, Vector3D bulge, Vector2D dimContainer, double offsetZ, ConstraintSetAbstract constraintSet, bool keepOnlyBest);
        List<ILayer2D> BuildLayers(Packable packable, Vector2D dimContainer, double offsetZ, ConstraintSetAbstract constraintSet, bool keepOnlyBest);
        Layer2DBrickImp BuildLayer(Vector3D dimBox, Vector2D actualDimensions, LayerDescBox layerDesc, double minSpace);
        Layer2DBrickImp BuildLayer(Vector3D dimBox, Vector3D bulge, Vector2D dimContainer, LayerDescBox layerDesc, Vector2D actualDimensions, double minSpace);
        ILayer2D BuildLayer(Packable packable, Vector2D dimContainer, LayerDesc layerDesc, double minSpace);
        ILayer2D BuildLayer(Packable packable, Vector2D dimContainer, LayerDesc layerDesc, Vector2D actualDimensions, double minSpace);
        bool GetDimensions(List<LayerDesc> layers, Packable packable, Vector2D dimContainer, double minSpace, ref Vector2D actualDimensions);
    }
    #endregion

    #region SolutionItem
    public class SolutionItem
    {
        #region Constructors
        public SolutionItem(int indexLayer, int indexInterlayer, bool symetryX, bool symetryY)
        {
            IndexLayer = indexLayer;
            InterlayerIndex = indexInterlayer;
            SymetryX = symetryX;
            SymetryY = symetryY;
        }
        public SolutionItem(SolutionItem solItem)
        {
            IndexLayer = solItem.IndexLayer;
            InterlayerIndex = solItem.InterlayerIndex;
            SymetryX = solItem.SymetryX;
            SymetryY = solItem.SymetryY;
        }
        #endregion

        #region Public properties
        public bool SymetryX { get; set; } = false;
        public bool SymetryY { get; set; } = false;
        public bool HasInterlayer   { get { return InterlayerIndex != -1; } }
        public int InterlayerIndex { get; set; } = -1;
        public int IndexLayer { get; set; } = 0;
        public int IndexEditedLayer { get; set; } = -1;
        #endregion

        #region Public methods
        public void SetInterlayer(int indexInterlayer)
        {
            InterlayerIndex = indexInterlayer;
        }
        public void InverseSymetry(int axis)
        {
            if (axis == 0) SymetryX = !SymetryX;
            else if (axis == 1) SymetryY = !SymetryY;
            else throw new Exception("Invalid axis of symetry");
        }
        #endregion

        #region Object overrides
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}"
                , IndexLayer
                , InterlayerIndex
                , SymetryX ? 1 : 0
                , SymetryY ? 1 : 0);
        }
        #endregion

        #region Public static parse methods
        public static SolutionItem Parse(string value)
        {
            Regex r = new Regex(@"(?<l>.*),(?<i>.*),(?<x>.*),(?<y>.*)", RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                return new SolutionItem(
                    int.Parse(m.Result("${l}"))
                    , int.Parse(m.Result("${i}"))
                    , int.Parse(m.Result("${x}")) == 1
                    , int.Parse(m.Result("${y}")) == 1);
            }
            else
                throw new Exception("Failed to parse SolutionItem!");
        }
        public static bool TryParse(string value, out SolutionItem solItem)
        {
            Regex r = new Regex(@"(?<l>),(?<i>),(?<x>),(?<y>)", RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                solItem = new SolutionItem(
                    int.Parse(m.Result("${l}"))
                    , int.Parse(m.Result("${i}"))
                    , int.Parse(m.Result("${x}")) == 1
                    , int.Parse(m.Result("${y}")) == 1);
                return true;
            }
            solItem = null;
            return false;
        }
        #endregion
    }
    #endregion

    #region SolutionItemSimplified
    public class SolutionItemSimplified
    {
        public SolutionItemSimplified(int indexLayer, bool editedLayer, bool symX, bool symY)
        {
        }
        public int IndexLayer { get; set; } = -1;
        public bool EditedLayer { get; set;} = false;
        public bool SymetryX { get; set; } = false;
        public bool SymetryY { get; set; } = false;

    }
    #endregion

    #region LayerEncap
    public class LayerEncap
    {
        public LayerEncap(LayerDesc layerDesc) { LayerDesc = layerDesc; }
        public LayerEncap(ILayer2D layer2D) { Layer2D = layer2D; }
        public LayerDesc LayerDesc { get; set; }
        public ILayer2D Layer2D { get; set; }
        public override bool Equals(object obj)
        {
            if (!(obj is LayerEncap lObj)) return false;
            if ((null == LayerDesc && null != lObj.LayerDesc) || (null != LayerDesc && null == lObj.LayerDesc))
                return false;
            return ( null != LayerDesc && LayerDesc.Equals(lObj.LayerDesc))
                   || ( null != Layer2D && Layer2D.Equals(lObj.Layer2D));
        }
        public override int GetHashCode()
        {
            if (null != LayerDesc)
                return LayerDesc.GetHashCode();
            else if (null != Layer2D)
                return Layer2D.GetHashCode();
            else
                return 0;
        }
        public override string ToString()
        {
            if (null != LayerDesc)
                return LayerDesc.ToString();
            else if (null != Layer2D)
                return Layer2D.ToString();
            else
                throw new Exception("Unexpected LayerEncap type!");
        }
        public ILayer2D BuildLayer(ILayerSolver solver, Packable packable, Vector2D containerDim, Vector2D actualDimensions, double minimumSpace)
        {
            if (null != LayerDesc)
                return solver.BuildLayer(packable, containerDim, LayerDesc, actualDimensions, minimumSpace);
            else if (null != Layer2D)
                return Layer2D;
            else
                throw new Exception("Invalid LayerEncap");            
        }
        public Vector2D GetDimensions(ILayerSolver solver, Packable packable, Vector2D containerDim, double minimumSpace)
        {
            Vector2D dimensions = Vector2D.Zero;
            if (null != LayerDesc)
                solver.GetDimensions(new List<LayerDesc>() { LayerDesc }, packable, containerDim, minimumSpace, ref dimensions);
            if (null != Layer2D)
            {
                BBox3D bb = Layer2D.BBox;
                dimensions.X = bb.Length;
                dimensions.Y = bb.Width;
            }
            return dimensions;
        }
    }
    #endregion

    #region LayerSummary
    public class LayerSummary
    {
        #region Constructor
        public LayerSummary(SolutionLayered sol, int indexLayer, bool symetryX, bool symetryY)
        {
            Sol = sol;
            IndexLayer = indexLayer;
            SymetryX = symetryX;
            SymetryY = symetryY;
        }
        #endregion
        #region Public properties
        public int ItemCount
        { get { return Sol.LayerBoxCount(IndexLayer); } }
        public Vector3D LayerDimensions
        { get { return new Vector3D(Sol.LayerTypes[IndexLayer].Length, Sol.LayerTypes[IndexLayer].Width, Sol.LayerTypes[IndexLayer].LayerHeight); } }
        public double LayerWeight
        { get { return Sol.LayerWeight(IndexLayer); } }
        public double LayerNetWeight
        { get { return Sol.LayerNetWeight(IndexLayer); } }
        public double Space
        { get { return Sol.LayerMaximumSpace(IndexLayer); } }
        public List<int> LayerIndexes { get; } = new List<int>();
        public string LayerIndexesString => string.Join(",", LayerIndexes.ToArray());
        public ILayer Layer3D => Sol.GetILayer(IndexLayer, SymetryX, SymetryY);
        public bool SymetryX { get; }
        public bool SymetryY { get; }
        public int IndexLayer { get; }
        public SolutionLayered Sol { get; set; }
        #endregion
        #region Public methods
        public bool IsLayerTypeOf(SolutionItem solItem)
        {
            return IndexLayer == solItem.IndexLayer
                && SymetryX == solItem.SymetryX
                && SymetryY == solItem.SymetryY;
        }
        public void AddIndex(int index)
        {
            LayerIndexes.Add(index);
        }
        #endregion
    }
    #endregion

    #region LayerPhrase
    // used for JJA
    public struct LayerPhrase : IEquatable<LayerPhrase>
    {
        public LayerDesc LayerDescriptor        { get; set; }
        public HalfAxis.HAxis Axis              { get; set; }
        public int Count                        { get; set; }
        public bool Equals(LayerPhrase other) => LayerDescriptor == other.LayerDescriptor && Axis == other.Axis && Count == other.Count;
    }
    #endregion

    #region SolutionLayered
    public class SolutionLayered : SolutionHomo
    {
        #region Constructor
        public SolutionLayered(AnalysisLayered analysis, LayerDesc layerDesc, bool mirrorLength, bool mirrorWidth)
        {
            Analysis = analysis;
            LayerEncaps = new List<LayerEncap>() { new LayerEncap(layerDesc) };
            LayersMirrorX = mirrorLength;
            LayersMirrorY = mirrorWidth;

            RebuildLayers();
            InitializeSolutionItemList();
        }
        public SolutionLayered(AnalysisLayered analysis, ILayer2D layer, bool mirrorLength, bool mirrorWidth)
        {
            Analysis = analysis;
            LayerEncaps = new List<LayerEncap>() { new LayerEncap(layer) };
            LayersMirrorX = mirrorLength;
            LayersMirrorY = mirrorWidth;

            RebuildLayers();
            InitializeSolutionItemList();
        }
        public SolutionLayered(AnalysisLayered analysis, List<LayerEncap> layerEncaps)
        {
            Analysis = analysis;
            LayerEncaps = layerEncaps;
            LayersMirrorX = AnalysisCast.AlternateLayersPref;
            LayersMirrorY = AnalysisCast.AlternateLayersPref;

            RebuildLayers();
            InitializeSolutionItemList();
        }
        public SolutionLayered(AnalysisLayered analysis, List<KeyValuePair<LayerEncap, int>> layerList)
        {
            Analysis = analysis;
            LayerEncaps = layerList.ConvertAll(l => l.Key);

            RebuildLayers();
            InitializeSolutionItemList(layerList);
            RebuildLayers();
        }
        #endregion

        #region Reprocessing
        public void RebuildLayers()
        {
            // sanity checks
            if ((null == LayerEncaps) || (0 == LayerEncaps.Count))
                throw new Exception("No layer descriptors/edited layers available");

            // build list of used layers
            List<LayerEncap> usedLayers = new List<LayerEncap>();
            if (null != _solutionItems && _solutionItems.Count > 0)
            {
                foreach (SolutionItem item in _solutionItems)
                {
                    if (!usedLayers.Contains(LayerEncaps[item.IndexLayer]))
                        usedLayers.Add(LayerEncaps[item.IndexLayer]);
                }
            }
            // if there is no layer used (e.g. startup), choose the first one
            if (0 == usedLayers.Count)
                usedLayers.Add(LayerEncaps[0]);
            // get actual dimensions
            Vector2D actualDimensions = Vector2D.Zero;
            foreach (var layer in usedLayers)
            {
                Vector2D dim = layer.GetDimensions(Solver, Analysis.Content, Analysis.ContainerDimensions, ConstraintSet.MinimumSpace.Value);
                actualDimensions.X = Math.Max(actualDimensions.X, dim.X);
                actualDimensions.Y = Math.Max(actualDimensions.Y, dim.Y);
            }
            // actually build layers
            LayerTypes.Clear();
            foreach (LayerEncap layer in LayerEncaps)
                LayerTypes.Add(layer.BuildLayer(Solver, Analysis.Content, Analysis.ContainerDimensions, actualDimensions, ConstraintSet.MinimumSpace.Value));
        }
        private void InitializeSolutionItemList()
        {
            _solutionItems = new List<SolutionItem>();

            ConstraintSetAbstract constraintSet = Analysis.ConstraintSet;
            double zTop = Analysis.Offset.Z;
            double weight = Analysis.ContainerWeight;
            int number = 0;
            bool allowMultipleLayers = true;
            if (constraintSet is ConstraintSetPalletTruck constraintSetPalletTruck)
                allowMultipleLayers = (constraintSetPalletTruck.OptMaxLayerNumber.Activated
                    && constraintSetPalletTruck.OptMaxLayerNumber.Value > 1);

            bool symetryX = false, symetryY = false;

            while (!constraintSet.CritHeightReached(zTop)
                && !constraintSet.CritWeightReached(weight)
                && !constraintSet.CritNumberReached(number)
                && !constraintSet.CritLayerNumberReached(SolutionItems.Count))
            {
                number += LayerTypes[0].Count;
                weight += LayerTypes[0].Count * Analysis.ContentWeight;
                zTop += LayerTypes[0].LayerHeight;

                if (!constraintSet.CritHeightReached(zTop) && (allowMultipleLayers || _solutionItems.Count < 1))
                    _solutionItems.Add(new SolutionItem(0, -1, symetryX, symetryY));
                else
                    break;
                symetryX = LayersMirrorX ? !symetryX : symetryX;
                symetryY = LayersMirrorY ? !symetryY : symetryY;
            }
        }
        private void InitializeSolutionItemList(List<KeyValuePair<LayerEncap, int>> listLayers)
        {
            _solutionItems = new List<SolutionItem>();
            foreach (KeyValuePair<LayerEncap, int> kvp in listLayers)
            {
                bool symetryX = false, symetryY = false;
                for (int i = 0; i < kvp.Value; ++i)
                {
                    _solutionItems.Add(new SolutionItem(GetLayerIndexFromLayerDesc(kvp.Key), -1, symetryX, symetryY));
                    symetryX = LayersMirrorX ? !symetryX : symetryX;
                    symetryY = LayersMirrorY ? !symetryY : symetryY;
                }
            }
        }
        public void RebuildSolutionItemList()
        {
            try
            {
                ConstraintSetAbstract constraintSet = Analysis.ConstraintSet;
                double zTop = Analysis.Offset.Z;
                double weight = Analysis.ContainerWeight;
                int number = 0;

                List<SolutionItem> solutionItems = new List<SolutionItem>();
                foreach (SolutionItem solItem in _solutionItems)
                {
                    number += LayerTypes[index: solItem.IndexLayer].Count;
                    weight += LayerTypes[index: solItem.IndexLayer].Count * Analysis.ContentWeight;
                    zTop += LayerTypes[solItem.IndexLayer].LayerHeight + ((-1 != solItem.InterlayerIndex) ? AnalysisCast.Interlayer(solItem.InterlayerIndex).Thickness : 0.0);

                    solutionItems.Add(solItem);

                    if (constraintSet.OneCriterionReached(zTop, weight, number, solutionItems.Count))
                        break;
                }
                // add layers until 
                while (!constraintSet.OneCriterionReached(zTop, weight, number, solutionItems.Count))
                {
                    SolutionItem solItem = null;
                    if (solutionItems.Count > 0)
                        solItem = solutionItems.Last();
                    else
                        solItem = new SolutionItem(0, -1, false, false);

                    if (solItem.IndexLayer >= LayerTypes.Count)
                        throw new Exception(string.Format("Layer index out of range!"));

                    number += LayerTypes[solItem.IndexLayer].Count;
                    weight += LayerTypes[solItem.IndexLayer].Count * Analysis.ContentWeight;

                    // using zTopAdded because zTop must not be incremented if SolutionItem object is 
                    // not actually added
                    double zTopIfAdded = zTop + LayerTypes[solItem.IndexLayer].LayerHeight
                        + ((-1 != solItem.InterlayerIndex) ? AnalysisCast.Interlayer(solItem.InterlayerIndex).Thickness : 0.0);

                    // only checking on height because weight / number can be modified without removing 
                    // a layer (while outputing solution as a list of case)
                    if (!constraintSet.CritHeightReached(zTopIfAdded))
                    {
                        solutionItems.Add(new SolutionItem(solItem));
                        zTop = zTopIfAdded;
                    }
                    else
                        break;
                }
                // remove unneeded layer 
                while (constraintSet.CritHeightReached(zTop) && solutionItems.Count > 0)
                {
                    SolutionItem solItem = solutionItems.Last();

                    if (solItem.IndexLayer >= LayerTypes.Count)
                        throw new Exception(string.Format("Layer index out of range!"));
                    number -= LayerTypes[solItem.IndexLayer].Count;
                    weight -= LayerTypes[solItem.IndexLayer].Count * Analysis.ContentWeight;
                    zTop -= LayerTypes[solItem.IndexLayer].LayerHeight
                        + ((-1 != solItem.InterlayerIndex) ? AnalysisCast.Interlayer(solItem.InterlayerIndex).Thickness : 0.0);

                    solutionItems.Remove(solItem);
                }
                _solutionItems.Clear();
                _solutionItems = solutionItems;

                // reset bounding box to force recompute
                _bbox.Reset();
                _strappers.Clear();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private int GetInterlayerIndex(InterlayerProperties interlayer)
        {
            return AnalysisCast.GetInterlayerIndex(interlayer);
        }
        #endregion

        #region Apply selection
        public void SelectLayer(int index)
        {
            SelectedLayerIndex = index;
            // rebuild layers
            RebuildLayers();
            // rebuild solution item list
            RebuildSolutionItemList();
        }
        public SolutionItem SelectedSolutionItem
        {
            get
            {
                if (!HasValidSelection) return null;
                return _solutionItems[SelectedLayerIndex];
            }
        }
        public void SetLayerTypeOnSelected(int iLayerType)
        {
            // check selected layer
            if (!HasValidSelection) return;
            if (SelectedLayerIndex < 0 || SelectedLayerIndex >= _solutionItems.Count)
            {
                _log.Error(string.Format("Calling SetLayerTypeOnSelected() with SelectedLayerIndex = {0}", SelectedLayerIndex));
                return;
            }
            // get selected solution item
            SolutionItem item = _solutionItems[SelectedLayerIndex];
            item.IndexLayer = iLayerType;
            // rebuild layers
            RebuildLayers();
            // rebuild solution item list
            RebuildSolutionItemList();
        }
        public void SetInterlayerOnSelected(InterlayerProperties interlayer)
        {
            // check selected layer
            if (!HasValidSelection) return;
            // get solution item
            SolutionItem item = _solutionItems[SelectedLayerIndex];
            item.SetInterlayer(GetInterlayerIndex(interlayer));
            // rebuild solution item list
            RebuildSolutionItemList();
        }
        public void ApplySymetryOnSelected(int axis)
        {
            // check selected layer
            if (!HasValidSelection) return;
            // get solution item
            SolutionItem item = _solutionItems[SelectedLayerIndex];
            item.InverseSymetry(axis);
        }
        public InterlayerProperties SelectedInterlayer
        {
            get
            {
                if (!HasValidSelection) return null;
                if (-1 == _solutionItems[SelectedLayerIndex].InterlayerIndex) return null;
                if (_solutionItems[SelectedLayerIndex].InterlayerIndex >= AnalysisCast.Interlayers.Count) return null;
                return AnalysisCast.Interlayer(_solutionItems[SelectedLayerIndex].InterlayerIndex);
            }
        }
        #endregion

        #region Public properties
        public int SelectedLayerIndex { get; private set; } = -1;
        public bool LayersMirrorX { get; set; } = true;
        public bool LayersMirrorY { get; set; } = true;
        public AnalysisLayered AnalysisCast => Analysis as AnalysisLayered;
        public List<LayerEncap> LayerEncaps { get; }
        public List<InterlayerProperties> Interlayers => AnalysisCast.Interlayers;
        public List<SolutionItem> SolutionItems
        {
            get => _solutionItems;
            set
            {
                _solutionItems = value;
                _bbox.Reset();
                _strappers.Clear();
            }
        }
        /// <summary>
        /// returns 3D layer. This is only used in LayerSummary
        /// </summary>
        internal ILayer GetILayer(int layerIndex, bool symX, bool symY)
        {
            ILayer2D currentLayer = LayerTypes[layerIndex];

            if (currentLayer is Layer2DBrick layer2DBox)
            {
                Layer3DBox boxLayer = new Layer3DBox(0.0, layerIndex);
                foreach (var layerPos in layer2DBox.Positions)
                {
                    BoxPosition layerPosTemp = AdjustLayerPosition(layerPos, symX, symY);
                    boxLayer.Add(
                        new BoxPosition(
                            layerPosTemp.Position + Analysis.Offset,
                            layerPosTemp.DirectionLength, layerPosTemp.DirectionWidth)
                        );
                }
                return boxLayer;
            }
            else if (currentLayer is Layer2DBrickExpIndexed layer2Dindexed)
            {
                Layer3DBoxIndexed boxLayer = new Layer3DBoxIndexed(0.0, layerIndex);
                foreach (var layerPos in layer2Dindexed.Positions)
                { 
                    BoxPositionIndexed layerPosTemp = AdjustLayerPosition(layerPos, symX, symY);
                    boxLayer.Add(
                        new BoxPositionIndexed(
                            layerPosTemp.BPos.Position + Analysis.Offset,
                            layerPosTemp.BPos.DirectionLength, layerPosTemp.BPos.DirectionWidth,
                            layerPosTemp.Index
                            )
                        );
                }
                return boxLayer;
            }
            else if (currentLayer is Layer2DCylImp)
            {
                Layer2DCylImp layer2DCyl = currentLayer as Layer2DCylImp;
                Layer3DCyl cylLayer = new Layer3DCyl(0.0);
                foreach (Vector2D vPos in layer2DCyl)
                {
                    cylLayer.Add(
                        AdjustPosition(new Vector3D(vPos.X, vPos.Y, 0.0), symX, symY)
                        + Analysis.Offset);
                }
                return cylLayer;
            }
            else
                return null;
        }
        public List<ILayer> Layers
        {
            get
            {
                List<ILayer> llayers = new List<ILayer>();

                int iBoxCount = 0, iInterlayerCount = 0;
                double zLayer = 0.0, weight = Analysis.ContainerWeight;
                bool stop = false;

                // build layers
                int indexLastLayer = 0;
                foreach (SolutionItem solItem in _solutionItems)
                {
                    if (solItem.HasInterlayer && AnalysisCast.Interlayers.Count > 0)
                    {
                        InterlayerProperties interlayer = AnalysisCast.Interlayer(solItem.InterlayerIndex);
                        llayers.Add(new InterlayerPos(zLayer + Analysis.Offset.Z, solItem.InterlayerIndex));
                        zLayer += interlayer.Thickness;
                        ++iInterlayerCount;
                    }

                    System.Diagnostics.Debug.Assert(solItem.IndexLayer < LayerTypes.Count);
                    ILayer2D currentLayer = LayerTypes[solItem.IndexLayer];

                    if (currentLayer is Layer2DBrick layer2DBox)
                    {
                        var boxLayer = new Layer3DBox(zLayer, solItem.IndexLayer);
                        var packable = AnalysisCast.Content as PackableBrick;
                        foreach (var layerPos in layer2DBox.Positions)
                        {
                            if (!ConstraintSet.CritNumberReached(iBoxCount + 1)
                                && !ConstraintSet.CritWeightReached(weight + Analysis.ContentWeight))
                            {
                                var layerPosTemp = AdjustLayerPosition(layerPos, solItem.SymetryX, solItem.SymetryY);
                                var boxPos = new BoxPosition(
                                    layerPosTemp.Position + Analysis.Offset + zLayer * Vector3D.ZAxis
                                    , layerPosTemp.DirectionLength
                                    , layerPosTemp.DirectionWidth
                                    );
                                boxPos.FlipFacingOutside(packable.Facing, packable.OuterDimensions, AnalysisCast.ContainerDimensions);
                                boxLayer.Add(boxPos);

                                ++iBoxCount;
                                weight += Analysis.ContentWeight;
                            }
                            else
                                stop = true;
                        }
                        if (boxLayer.Count > 0)
                            llayers.Add(boxLayer);
                    }
                    else if (currentLayer is Layer2DBrickIndexed layer2DBoxIndexed)
                    {
                        var boxLayer = new Layer3DBoxIndexed(zLayer, solItem.IndexLayer);
                        foreach (var layerPos in layer2DBoxIndexed.Positions)
                        {
                            if (!ConstraintSet.CritNumberReached(iBoxCount + 1)
                                && !ConstraintSet.CritWeightReached(weight + Analysis.ContentWeight))
                            {
                                var layerPosTemp = AdjustLayerPosition(layerPos, solItem.SymetryX, solItem.SymetryY);
                                boxLayer.Add(new BoxPositionIndexed(
                                    layerPosTemp.BPos.Position + Analysis.Offset + zLayer * Vector3D.ZAxis
                                    , layerPosTemp.BPos.DirectionLength
                                    , layerPosTemp.BPos.DirectionWidth
                                    , layerPosTemp.Index + indexLastLayer
                                    ));

                                ++iBoxCount;
                                weight += Analysis.ContentWeight;
                            }
                            else
                                stop = true;

                        }
                        if (boxLayer.Count > 0)
                        {
                            llayers.Add(boxLayer);
                            indexLastLayer = boxLayer.Last().Index + 1;
                        }
                    }
                    else if (currentLayer is Layer2DCylImp layer2DCyl)
                    {
                        var cylLayer = new Layer3DCyl(zLayer);
                        foreach (Vector2D vPos in layer2DCyl)
                        {
                            if (!ConstraintSet.CritNumberReached(iBoxCount + 1)
                                && !ConstraintSet.CritWeightReached(weight + Analysis.ContentWeight))
                            {
                                cylLayer.Add(
                                    AdjustPosition(new Vector3D(vPos.X, vPos.Y, zLayer), solItem.SymetryX, solItem.SymetryY)
                                    + Analysis.Offset);
                                ++iBoxCount;
                                weight += Analysis.ContentWeight;
                            }
                            else
                                stop = true;
                        }
                        if (cylLayer.Count > 0)
                            llayers.Add(cylLayer);
                    }

                    zLayer += currentLayer.LayerHeight;
                    if (stop)
                        break;
                }
                return llayers;
            }
        }
        public override BBox3D BBoxLoad
        {
            get
            {
                if (!_bbox.IsValid)
                {
                    bool firstLayer = true;
                    foreach (ILayer layer in Layers)
                    {
                        if (layer is Layer3DBox || layer is Layer3DBoxIndexed || layer is Layer3DCyl)
                            _bbox.Extend(layer.BoundingBox(Analysis.Content));
                        else if (layer is InterlayerPos && !firstLayer)
                        {
                            InterlayerPos interLayerPos = layer as InterlayerPos;
                            InterlayerProperties interlayerProp = Interlayers[interLayerPos.TypeId];
                            Vector3D vecMin = new Vector3D(
                                0.5 * (Analysis.ContainerDimensions.X - interlayerProp.Length)
                                , 0.5 * (Analysis.ContainerDimensions.Y - interlayerProp.Width)
                                , layer.ZLow);
                            _bbox.Extend(new BBox3D(vecMin, vecMin + interlayerProp.Dimensions));
                        }
                        firstLayer = false;
                    }

                    if (Analysis is AnalysisCasePallet analysisCasePallet && analysisCasePallet.HasTopInterlayer)
                    {
                        double z = _bbox.PtMax.Z;
                        InterlayerProperties interlayerProp = analysisCasePallet.TopInterlayerProperties;
                        Vector3D vecMin = new Vector3D(
                            0.5 * (Analysis.ContainerDimensions.X - interlayerProp.Length)
                            , 0.5 * (Analysis.ContainerDimensions.X - interlayerProp.Width)
                            , z);
                        _bbox.Extend(new BBox3D(vecMin, vecMin + interlayerProp.Dimensions));
                    }

                    // sanity check
                    if (!_bbox.IsValid)
                        _bbox.Extend(Vector3D.Zero);
                }
                return _bbox.Clone();
            }
        }

        public IEnumerable<StrapperData> Strappers
        {
            get
            {
                var bboxLoad = BBoxLoad;
                if (_strappers.Count == 0 && Analysis is AnalysisCasePallet analysisCasePallet && null != analysisCasePallet.StrapperSet)
                {
                    analysisCasePallet.StrapperSet.SetDimension(bboxLoad.PtMax - bboxLoad.PtMin);
                    foreach (var strapper in analysisCasePallet.StrapperSet.Strappers)
                    {
                        double topDeckThickness = UnitsManager.ConvertLengthFrom(22, UnitsManager.UnitSystem.UNIT_METRIC1);
                        _strappers.Add(
                            new StrapperData(
                                strapper,
                                IntersectionWPlane(strapper.Abscissa + bboxLoad.PtMin[strapper.Axis], strapper.Axis, topDeckThickness)
                                )
                            );
                    }
                }
                return _strappers;
            }
        }
        public void ClearStrapperSets() { _strappers.Clear();}

        public int InterlayerCount
        {
            get
            {
                int layerCount = 0, interlayerCount = 0, boxCount = 0;
                GetCounts(ref layerCount, ref interlayerCount, ref boxCount);
                return interlayerCount;
            }
        }
        public List<Pair<InterlayerProperties, int>> InterlayerCounts
        {
            get
            {
                var list = new List<Pair<InterlayerProperties, int>>();

                var groups =
                    from sol in _solutionItems
                        group sol by sol.InterlayerIndex into g
                        select new
                        {
                            InterlayerIndex = g.Key,
                            Count = g.Count()
                        };

                foreach (var g in groups)
                    if (-1 != g.InterlayerIndex)
                        list.Add(new Pair<InterlayerProperties, int>(AnalysisCast.Interlayer(g.InterlayerIndex), g.Count));
                return list;
            }
        }
        public override int ItemCount
        {
            get
            {
                int layerCount = 0, interlayerCount = 0, itemCount = 0;
                GetCounts(ref layerCount, ref interlayerCount, ref itemCount);
                return itemCount;
            }
        }
        public int LayerCount
        {
            get
            {
                int layerCount = 0, interlayerCount = 0, itemCount = 0;
                GetCounts(ref layerCount, ref interlayerCount, ref itemCount);
                return layerCount;
            }
        }
        private void GetCounts(ref int layerCount, ref int interlayerCount, ref int itemCount)
        {
            layerCount = 0;
            interlayerCount = 0;
            itemCount = 0;
            foreach (ILayer layer in Layers)
            {
                if (layer is Layer3DBox blayer)
                {
                    ++layerCount;
                    itemCount += blayer.BoxCount;
                }
                else if (layer is Layer3DBoxIndexed bilayer)
                {
                    ++layerCount;
                    itemCount += bilayer.BoxCount;
                }
                else if (layer is Layer3DCyl clayer)
                {
                    ++layerCount;
                    itemCount += clayer.CylinderCount;
                }
                else if (layer is InterlayerPos iLayer)
                {
                    ++interlayerCount;
                }
            }
        }
        public double LoadOnLowestCase
        {
            get
            {
                if (LayerCount < 2)
                    return 0.0;

                int layerCount = 0;
                double loadTopLayers = 0.0;
                int noInFirstLayer = 0;
                foreach (ILayer layer in Layers)
                {
                    if (layer is Layer3DBox blayer)
                    {
                        if (0 == layerCount)
                            noInFirstLayer = blayer.BoxCount;
                        else
                            loadTopLayers += blayer.BoxCount * Analysis.ContentWeight;
                        ++layerCount;
                    }
                    if (layer is Layer3DCyl clayer)
                    {
                        if (0 == layerCount)
                            noInFirstLayer = clayer.CylinderCount;
                        else
                            loadTopLayers += clayer.CylinderCount * Analysis.ContentWeight;
                        ++layerCount;
                    }
                }
                return loadTopLayers / noInFirstLayer;
            }
        }

        public void GetLayerIndexes(ref List<int> layers, ref List<int> interlayerIndexes)
        {
            layers.Clear();
            interlayerIndexes.Clear();
            List<SolutionItem> listSolItem = new List<SolutionItem>();
            GetUniqueSolutionItems(ref listSolItem, ref layers, ref interlayerIndexes);
        }
        private void GetUniqueSolutionItems(ref List<SolutionItem> listSolItem, ref List<int> layers, ref List<int> interlayerIndexes)
        {
            listSolItem.Clear();
            foreach (var solItem in _solutionItems)
            {
                int index = listSolItem.FindIndex(
                    si => si.IndexLayer == solItem.IndexLayer
                    && si.IndexEditedLayer == solItem.IndexEditedLayer
                    && si.SymetryX == solItem.SymetryX
                    && si.SymetryY == solItem.SymetryY);

                if (-1 != index)
                    layers.Add(index);
                else
                {
                    listSolItem.Add(solItem);
                    layers.Add(listSolItem.Count - 1);
                }
                interlayerIndexes.Add(solItem.HasInterlayer ? solItem.InterlayerIndex : -1);
            }
            if (Analysis is AnalysisCasePallet analysisCasePallet)
                interlayerIndexes.Add(analysisCasePallet.HasTopInterlayer ? GetInterlayerIndex(analysisCasePallet.TopInterlayerProperties) : -1);
        }
        public void GetUniqueSolutionItemsAndOccurence(ref List<Layer3DBox> listLayerTypes, ref List<int> layers, ref List<int> interlayerIndexes)
        {
            List<SolutionItem> listSolItem = new List<SolutionItem>();
            /*
            foreach (var solItem in _solutionItems)
            {
                int index = listSolItem.FindIndex(
                    si => si.IndexLayer == solItem.IndexLayer
                    && si.IndexEditedLayer == solItem.IndexEditedLayer
                    && si.SymetryX == solItem.SymetryX
                    && si.SymetryY == solItem.SymetryY);

                if (-1 != index)
                    layers.Add(index);
                else
                {
                    listSolItem.Add(solItem);
                    layers.Add(listSolItem.Count - 1);
                }
                interlayerIndexes.Add(solItem.HasInterlayer ? solItem.InterlayerIndex : -1);
            }
            if (Analysis is AnalysisCasePallet analysisCasePallet)
                interlayerIndexes.Add(analysisCasePallet.HasTopInterlayer ? GetInterlayerIndex(analysisCasePallet.TopInterlayerProperties) : -1);
            */
            GetUniqueSolutionItems(ref listSolItem, ref layers, ref interlayerIndexes);

            double zLayer = 0;
            foreach (var solItem in listSolItem)
            {
                if (solItem.IndexLayer != -1)
                {
                    ILayer2D currentLayer = LayerTypes[solItem.IndexLayer];
                    if (currentLayer is Layer2DBrick layer2DBox)
                    {
                        var boxLayer = new Layer3DBox(zLayer, solItem.IndexLayer);
                        foreach (var layerPos in layer2DBox.Positions)
                        {
                            BoxPosition layerPosTemp = AdjustLayerPosition(layerPos, solItem.SymetryX, solItem.SymetryY);
                            BoxPosition boxPos = new BoxPosition(
                                layerPosTemp.Position + Analysis.Offset + zLayer * Vector3D.ZAxis
                                , layerPosTemp.DirectionLength
                                , layerPosTemp.DirectionWidth
                                );
                            if (AnalysisCast.Content is BoxProperties packable)
                                boxPos.FlipFacingOutside(packable.Facing, packable.OuterDimensions, AnalysisCast.ContainerDimensions);
                            boxLayer.Add(boxPos);
                        }
                        listLayerTypes.Add(boxLayer);
                    }
                }
            }
        }
        public Dictionary<LayerPhrase, int> LayerPhrases
        {
            get
            {
                // initialize loaded weight & count
                double weight = Analysis.ContainerWeight;
                int iBoxCount = 0;
                bool stop = false;

                Dictionary<LayerPhrase, int> dict = new Dictionary<LayerPhrase, int>();
                foreach (SolutionItem solItem in _solutionItems)
                {
                    Layer2DBrick layer = LayerTypes[solItem.IndexLayer] as Layer2DBrick;
                    if (!ConstraintSet.OptMaxWeight.Activated && !ConstraintSet.OptMaxNumber.Activated)
                    {
                        LayerPhrase lp = new LayerPhrase()
                        {
                            LayerDescriptor = layer.LayerDescriptor,
                            Count = layer.Count,
                            Axis = layer.VerticalAxisProp
                        };
                        if (dict.ContainsKey(lp))
                            dict[lp] += 1;
                        else
                            dict.Add(lp, 1);
                    }
                    else
                    {
                        int iLayerBoxCount = 0;
                        foreach (BoxPosition layerPos in layer.Positions)
                        {
                            if (!ConstraintSet.CritNumberReached(iBoxCount + 1)
                                && !ConstraintSet.CritWeightReached(weight + Analysis.ContentWeight))
                            {
                                ++iLayerBoxCount;
                                ++iBoxCount;
                                weight += Analysis.ContentWeight;
                            }
                            else
                                stop = true;
                        }
                        if (iLayerBoxCount > 0)
                        {
                            LayerPhrase lp = new LayerPhrase()
                            {
                                LayerDescriptor = layer.LayerDescriptor,
                                Count = iLayerBoxCount,
                                Axis = layer.VerticalAxisProp 
                            };
                            if (dict.ContainsKey(lp))
                                dict[lp] += 1;
                            else
                                dict.Add(lp, 1);
                        }
                    }
                    if (stop)
                        break;
                }
                return dict;
            }
        }
        private Dictionary<int, List<int>> DictCountLayers
        {
            get
            {
                var dict = new Dictionary<int, List<int>>();
                for (int i=0; i<Layers.Count; ++i)
                {
                    ILayer layer = Layers[i];
                    int boxCount = layer.BoxCount;
                    var list = new List<int>() { i };
                    if (dict.ContainsKey(boxCount))
                        dict[boxCount].AddRange(list);
                    else
                        dict[boxCount] = list;
                }
                return dict;
            }
        }
        public string TiHiString
        {
            get
            {
                // *** get dictionnary Number of cases <-> List of layers
                var dict = DictCountLayers;
                
                // *** build return string
                string s = string.Empty;
                foreach (KeyValuePair<int, List<int>> kvp in dict)
                {
                    if (!string.IsNullOrEmpty(s))
                        s += " + ";
                    s += $"{kvp.Key} x {kvp.Value.Count}";
                }
                return s;
            }
        }
        public bool HasConstantTI => 1 == DictCountLayers.Count;
        public int ConstantTI
        {
            get
            {
                var dict = DictCountLayers;
                if (dict.Count > 1)
                    throw new Exception("Solution does not have a constantTI");
                else
                    return DictCountLayers.First().Key;
            }
        }
        public bool IsZOriented
        {
            get
            {
                foreach (var solItem in _solutionItems)
                {
                    ILayer2D currentLayer = LayerTypes[solItem.IndexLayer];
                    if (currentLayer is Layer2DBrick layer2DBox && !layer2DBox.IsZOriented)
                        return false;
                }
                return true;
            }
        }
        #endregion

        #region Layer type methods
        public List<int> LayerTypeUsed(int layerTypeIndex)
        {
            List<int> solItemIndexes = new List<int>();
            int index = 0;
            foreach (SolutionItem solItem in _solutionItems)
            {
                if (solItem.IndexLayer == layerTypeIndex)
                    solItemIndexes.Add(index+1);
                ++index;
            }
            return solItemIndexes;            
        }
        public int NoLayerTypesUsed
        {
            get
            {
                int noLayerTypesUsed = 0;
                for (int i = 0; i < LayerTypes.Count; ++i)
                {
                    List<int> listLayerUsingType = LayerTypeUsed(i);
                    noLayerTypesUsed += listLayerUsingType.Count > 0 ? 1 : 0;
                }
                return noLayerTypesUsed;
            }
        }
        public string LayerCaption(int layerTypeIndex)
        {

            List<int> layerIndexes = LayerTypeUsed(layerTypeIndex);
            if (layerIndexes.Count == LayerCount)
                return Properties.Resources.ID_LAYERSALL;


            StringBuilder sb = new StringBuilder();
            sb.Append(layerIndexes.Count > 1 ? Properties.Resources.ID_LAYERS : Properties.Resources.ID_LAYER);
            int iCountIndexes = layerIndexes.Count;
            for (int j = 0; j < iCountIndexes; ++j)
            {
                sb.AppendFormat("{0}", layerIndexes[j]);
                if (j != iCountIndexes - 1)
                {
                    sb.Append(",");
                    if (j != 0 && 0 == j % 10)
                        sb.Append("\n");
                }
            }
            return sb.ToString();
        }
        public int LayerBoxCount(int layerTypeIndex) => LayerTypes[layerTypeIndex].Count;
        public double LayerWeight(int layerTypeIndex) => LayerBoxCount(layerTypeIndex) * Analysis.ContentWeight;
        public double LayerNetWeight(int layerTypeIndex) => LayerBoxCount(layerTypeIndex) * Analysis.Content.NetWeight.Value;
        public double LayerMaximumSpace(int LayerTypeIndex) => LayerTypes[LayerTypeIndex].MaximumSpace;
        public override double InterlayersWeight
        {
            get
            {
                double interlayerWeight = 0;
                foreach (SolutionItem solItem in _solutionItems)
                {   interlayerWeight += solItem.HasInterlayer ? Interlayers[solItem.InterlayerIndex].Weight : 0.0; }
                return interlayerWeight;
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// if box bottom oriented to Z+, reverse box
        /// </summary>
        private BoxPosition AdjustLayerPosition(BoxPosition layerPos, bool reflectionX, bool reflectionY)
        {            
            Vector3D dimensions = Analysis.ContentDimensions;
            Vector2D containerDims = Analysis.ContainerDimensions;

            // implement change
            BoxPosition layerPosTemp = new BoxPosition(layerPos);

            // apply symetry in X
            if (reflectionX)
            {
                Matrix4D matRot = new Matrix4D(
                  1.0, 0.0, 0.0, 0.0
                  , 0.0, -1.0, 0.0, 0.0
                  , 0.0, 0.0, 1.0, 0.0
                  , 0.0, 0.0, 0.0, 1.0
                  );
                Vector3D vTranslation = new Vector3D(0.0, containerDims.Y, 0.0);
                layerPosTemp = ApplyReflection(layerPosTemp, matRot, vTranslation);
            }
            // apply symetry in Y
            if (reflectionY)
            {
                Matrix4D matRot = new Matrix4D(
                    -1.0, 0.0, 0.0, 0.0
                    , 0.0, 1.0, 0.0, 0.0
                    , 0.0, 0.0, 1.0, 0.0
                    , 0.0, 0.0, 0.0, 1.0
                    );
                Vector3D vTranslation = new Vector3D(containerDims.X, 0.0, 0.0);
                layerPosTemp = ApplyReflection(layerPosTemp, matRot, vTranslation);
            }
            return layerPosTemp.Adjusted(dimensions);
        }
        private BoxPositionIndexed AdjustLayerPosition(BoxPositionIndexed layerPos, bool reflectionX, bool reflectionY)
        {
            Vector3D dimensions = Analysis.ContentDimensions;
            Vector2D containerDims = Analysis.ContainerDimensions;

            // implement change
            var layerPosTemp = new BoxPositionIndexed(layerPos);

            // apply symetry in X
            if (reflectionX)
            {
                Matrix4D matRot = new Matrix4D(
                  1.0, 0.0, 0.0, 0.0
                  , 0.0, -1.0, 0.0, 0.0
                  , 0.0, 0.0, 1.0, 0.0
                  , 0.0, 0.0, 0.0, 1.0
                  );
                Vector3D vTranslation = new Vector3D(0.0, containerDims.Y, 0.0);
                layerPosTemp = ApplyReflection(layerPosTemp, matRot, vTranslation);
            }
            // apply symetry in Y
            if (reflectionY)
            {
                Matrix4D matRot = new Matrix4D(
                    -1.0, 0.0, 0.0, 0.0
                    , 0.0, 1.0, 0.0, 0.0
                    , 0.0, 0.0, 1.0, 0.0
                    , 0.0, 0.0, 0.0, 1.0
                    );
                Vector3D vTranslation = new Vector3D(containerDims.X, 0.0, 0.0);
                layerPosTemp = ApplyReflection(layerPosTemp, matRot, vTranslation);
            }
            return layerPosTemp.Adjusted(dimensions);
        }

        private Vector3D AdjustPosition(Vector3D v, bool reflectionX, bool reflectionY)
        {
            Vector2D containerDims = Analysis.ContainerDimensions;
            Vector3D posTemp = new Vector3D(v);
            // apply symetry in X
            if (reflectionX)
            {
                Matrix4D matRot = new Matrix4D(
                  1.0, 0.0, 0.0, 0.0
                  , 0.0, -1.0, 0.0, 0.0
                  , 0.0, 0.0, 1.0, 0.0
                  , 0.0, 0.0, 0.0, 1.0
                  );
                Vector3D vTranslation = new Vector3D(0.0, containerDims.Y, 0.0);
                posTemp = ApplyReflection(posTemp, matRot, vTranslation);
            }
            // apply symetry in Y
            if (reflectionY)
            {
                Matrix4D matRot = new Matrix4D(
                    -1.0, 0.0, 0.0, 0.0
                    , 0.0, 1.0, 0.0, 0.0
                    , 0.0, 0.0, 1.0, 0.0
                    , 0.0, 0.0, 0.0, 1.0
                    );
                Vector3D vTranslation = new Vector3D(containerDims.X, 0.0, 0.0);
                posTemp = ApplyReflection(posTemp, matRot, vTranslation);
            }
            return posTemp;
        }
        private BoxPosition ApplyReflection(BoxPosition layerPos, Matrix4D matRot, Vector3D vTranslation)
        {
            Vector3D dimensions = Analysis.ContentDimensions;
            Transform3D transfRot = new Transform3D(matRot);
            HalfAxis.HAxis axisLength = HalfAxis.ToHalfAxis(transfRot.transform(HalfAxis.ToVector3D(layerPos.DirectionLength)));
            HalfAxis.HAxis axisWidth = HalfAxis.ToHalfAxis(transfRot.transform(HalfAxis.ToVector3D(layerPos.DirectionWidth)));
            matRot.M14 = vTranslation[0];
            matRot.M24 = vTranslation[1];
            matRot.M34 = vTranslation[2];
            Transform3D transfRotTranslation = new Transform3D(matRot);

            Vector3D transPos = transfRotTranslation.transform( new Vector3D(layerPos.Position.X, layerPos.Position.Y, layerPos.Position.Z) );
            return new BoxPosition(
                new Vector3D(transPos.X, transPos.Y, transPos.Z)
                    - dimensions.Z * Vector3D.CrossProduct(HalfAxis.ToVector3D(axisLength), HalfAxis.ToVector3D(axisWidth))
                , axisLength
                , axisWidth);
        }
        private BoxPositionIndexed ApplyReflection(BoxPositionIndexed layerPos, Matrix4D matRot, Vector3D vTranslation)
        {
            Vector3D dimensions = Analysis.ContentDimensions;
            Transform3D transfRot = new Transform3D(matRot);
            HalfAxis.HAxis axisLength = HalfAxis.ToHalfAxis(transfRot.transform(HalfAxis.ToVector3D(layerPos.BPos.DirectionLength)));
            HalfAxis.HAxis axisWidth = HalfAxis.ToHalfAxis(transfRot.transform(HalfAxis.ToVector3D(layerPos.BPos.DirectionWidth)));
            matRot.M14 = vTranslation[0];
            matRot.M24 = vTranslation[1];
            matRot.M34 = vTranslation[2];
            Transform3D transfRotTranslation = new Transform3D(matRot);

            Vector3D transPos = transfRotTranslation.transform(new Vector3D(layerPos.BPos.Position.X, layerPos.BPos.Position.Y, layerPos.BPos.Position.Z));
            return new BoxPositionIndexed(
                new Vector3D(transPos.X, transPos.Y, transPos.Z)
                    - dimensions.Z * Vector3D.CrossProduct(HalfAxis.ToVector3D(axisLength), HalfAxis.ToVector3D(axisWidth))
                , axisLength
                , axisWidth
                , layerPos.Index);
        }
        private Vector3D ApplyReflection(Vector3D vPos, Matrix4D matRot, Vector3D vTranslation)
        {
            matRot.M14 = vTranslation[0];
            matRot.M24 = vTranslation[1];
            matRot.M34 = vTranslation[2];
            Transform3D transfRotTranslation = new Transform3D(matRot);
            return transfRotTranslation.transform(vPos);  
        }
        private bool HasValidSelection
        {
            get { return SelectedLayerIndex >= 0 && SelectedLayerIndex < _solutionItems.Count; }
        }
        public List<LayerSummary> ListLayerSummary
        {
            get
            {
                List<LayerSummary> _layerSummaries = new List<LayerSummary>();
                int layerCount = 0;
                foreach (SolutionItem solItem in _solutionItems)
                {
                    LayerSummary layerSum = _layerSummaries.Find(delegate(LayerSummary lSum) { return lSum.IsLayerTypeOf(solItem); });
                    if (null == layerSum)
                    {
                        layerSum = new LayerSummary(this, solItem.IndexLayer, solItem.SymetryX, solItem.SymetryY);
                        _layerSummaries.Add(layerSum);
                    }
                    layerSum.AddIndex(++layerCount);
                }
                return _layerSummaries;
            }
        }
        private int GetLayerIndexFromLayerDesc(LayerEncap layerEncap)
        {
            int index =LayerTypes.FindIndex(l => l.LayerDescriptor.ToString() == layerEncap.ToString());
            if (-1 == index)
                throw new Exception("No valid layer with desc {layerDesc}");
            return index;
        }
        private List<Vector3D> IntersectionWPlane(double abs, int axis, double topDeckThickness)
        {
            var points = new List<Vector3D>();
            var dim = Analysis.ContentDimensions;
            // layers
            foreach (ILayer layer in Layers)
            {
                if (layer is Layer3DBox layerBox)
                {
                    foreach (var bp in layerBox)
                        bp.IntersectPlane(dim, abs, axis, ref points);
                }
                else if (layer is InterlayerPos)
                {
                    InterlayerPos interLayerPos = layer as InterlayerPos;
                    InterlayerProperties interlayerProp = Interlayers[interLayerPos.TypeId];
                    var bp = new BoxPosition(new Vector3D(
                        0.5 * (Analysis.ContainerDimensions.X - interlayerProp.Length)
                        , 0.5 * (Analysis.ContainerDimensions.Y - interlayerProp.Width)
                        , 0.0)
                        + Analysis.Offset, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                    bp.IntersectPlane(dim, abs, axis, ref points);
                }
            }
            // pallet top deck
            if (Analysis.Container is PalletProperties palletProperties)
            {
                Vector3D dimTopDeck = new Vector3D(palletProperties.Length, palletProperties.Width, topDeckThickness);
                var bp = new BoxPosition(new Vector3D(0.0, 0.0, palletProperties.Height - topDeckThickness), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                bp.IntersectPlane(dimTopDeck, abs, axis, ref points);
            }
            // pallet cap
            if (Analysis is AnalysisCasePallet analysisCasePallet)
            {
                BBox3D loadBBox = BBoxLoad;
                if (analysisCasePallet.HasPalletCap)
                {
                    PalletCapProperties capProperties = analysisCasePallet.PalletCapProperties;
                    Vector3D dimCap = new Vector3D(capProperties.Length, capProperties.Width, capProperties.Height);
                    var bp = new BoxPosition(new Vector3D(
                        0.5 * (analysisCasePallet.PalletProperties.Length - capProperties.Length),
                        0.5 * (analysisCasePallet.PalletProperties.Width - capProperties.Width),
                        loadBBox.PtMax.Z - capProperties.InsideHeight)
                        , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P
                        );
                    bp.IntersectPlane(dimCap, abs, axis, ref points);
                }
                if (analysisCasePallet.HasPalletCorners)
                {
                    PalletCornerProperties cornerProperties = analysisCasePallet.PalletCornerProperties;
                    double th = cornerProperties.Thickness;
                    // positions
                    Vector3D[] cornerPositions =
                    {
                        loadBBox.PtMin + new Vector3D(-th, -th, 0.0)
                        , new Vector3D(loadBBox.PtMax.X, loadBBox.PtMin.Y, loadBBox.PtMin.Z) + new Vector3D(th, -th, 0.0)
                        , new Vector3D(loadBBox.PtMax.X, loadBBox.PtMax.Y, loadBBox.PtMin.Z) + new Vector3D(th, th, 0.0)
                        , new Vector3D(loadBBox.PtMin.X, loadBBox.PtMax.Y, loadBBox.PtMin.Z) + new Vector3D(-th, th, 0.0)
                    };
                    // length axes
                    HalfAxis.HAxis[] lAxes =
                    {
                        HalfAxis.HAxis.AXIS_X_P,
                        HalfAxis.HAxis.AXIS_Y_P,
                        HalfAxis.HAxis.AXIS_X_N,
                        HalfAxis.HAxis.AXIS_Y_N
                    };
                    // width axes
                    HalfAxis.HAxis[] wAxes =
                    {
                        HalfAxis.HAxis.AXIS_Y_P,
                        HalfAxis.HAxis.AXIS_X_N,
                        HalfAxis.HAxis.AXIS_Y_N,
                        HalfAxis.HAxis.AXIS_X_P
                    };
                    double height = Math.Min(analysisCasePallet.PalletCornerProperties.Length, loadBBox.Height);
                    Vector3D dimCorner = new Vector3D(cornerProperties.Width, cornerProperties.Width, height);

                    for (int i = 0; i < 4; ++i)
                    {
                        var bp = new BoxPosition(cornerPositions[i], lAxes[i], wAxes[i]);
                        bp.IntersectPlane(dimCorner, abs, axis, ref points);
                    }
                }
            }
            // build convexhull
            var convexHullResult = ConvexHull.Create2D(ListVector3DToListArray(points, axis), 1.0E-10);
            if (null != convexHullResult.Result && convexHullResult.Result.Count > 2)
                return ListVertex2DToVector3D(convexHullResult.Result, abs, axis);
            else
                return new List<Vector3D>();
        }

        private List<double[]> ListVector3DToListArray(List<Vector3D> points, int axis)
        {
            var listResult = new List<double[]>();
            foreach (var pt in points)
            {
                double x, y;
                switch (axis)
                {
                    case 0: x = pt.Y; y = pt.Z; break;
                    case 1: x = pt.X; y = pt.Z; break;
                    case 2: x = pt.X; y = pt.Y; break;
                    default: x = 0.0; y = 0.0; break;
                }
                listResult.Add(new double[] { x, y });
            }
            return listResult;
        }
        private List<Vector3D> ListVertex2DToVector3D(IList<DefaultVertex2D> vertices, double abs, int axis)
        {
            var listPoints = new List<Vector3D>();
            foreach (var v in vertices)
            {
                Vector3D point;
                switch (axis)
                {
                    case 0: point = new Vector3D(abs, v.X, v.Y); break;
                    case 1: point = new Vector3D(v.X, abs, v.Y); break;
                    case 2: point = new Vector3D(v.X, v.Y, abs); break;
                    default: point = Vector3D.Zero; break;
                }
                listPoints.Add(point);
            }
            if (1 == axis) listPoints.Reverse();

            return listPoints;
        }
        #endregion

        #region Static solver instance
        private static ILayerSolver _solver;
        public static void SetSolver(ILayerSolver solver) => _solver = solver; 
        public static ILayerSolver Solver
        {
            get
            {
                if (null == _solver)
                    throw new Exception("Solver not initialized -> Call Solution.SetSolver() static function.");
                return _solver;
            }
        }
        #endregion

        #region Data members
        private List<SolutionItem> _solutionItems;
        internal List<ILayer2D> LayerTypes { get; set; } = new List<ILayer2D>();
        // cached data
        private BBox3D _bbox = BBox3D.Initial;
        private List<StrapperData> _strappers = new List<StrapperData>();
        #endregion

        #region Static members
        private static ILog _log = LogManager.GetLogger(typeof(SolutionLayered));
        #endregion
    }
    #endregion

    #region StrapperData class
    public class StrapperData
    {
        public StrapperData(PalletStrapper strapper, List<Vector3D> points) { Strapper = strapper; Points = points; }
        public PalletStrapper Strapper { get; set; }
        public List<Vector3D> Points { get; set; }
    }
    #endregion
}
