using System;
using System.Collections.Generic;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;

namespace treeDiM.StackBuilder.Engine
{
    public class LayerSolver : ILayerSolver
    {
        public LayerDesc BestLayerDesc(Vector3D dimBox, Vector3D bulge, Vector2D dimContainer, double offsetZ, ConstraintSetAbstract constraintSet)
        {
            var layers = BuildLayers(dimBox, bulge, dimContainer, offsetZ, constraintSet, true);
            return layers[0].LayerDescriptor;
        }
        public List<Layer2DBrickImp> BuildLayers(
            Vector3D dimBox, Vector3D bulge, Vector2D dimContainer,
            double offsetZ, /* e.g. pallet height */
            ConstraintSetAbstract constraintSet, bool keepOnlyBest)
        {
            // instantiate list of layers
            var listLayers0 = new List<Layer2DBrickImp>();

            // loop through all patterns
            foreach (LayerPatternBox pattern in LayerPatternBox.All)
            {
                // loop through all orientation
                HalfAxis.HAxis[] patternAxes = pattern.IsSymetric ? HalfAxis.Positives : HalfAxis.All;
                foreach (HalfAxis.HAxis axisOrtho in patternAxes)
                {
                    // is orientation allowed
                    if (!constraintSet.AllowOrientation(Layer2DBrick.VerticalAxis(axisOrtho)))
                        continue;
                    // not swapped vs swapped pattern
                    for (int iSwapped = 0; iSwapped < 2; ++iSwapped)
                    {
                        try
                        {
                            // does swapping makes sense for this layer pattern ?
                            if (!pattern.CanBeSwapped && (iSwapped == 1))
                                continue;
                            // instantiate layer
                            var layer = new Layer2DBrickImp(dimBox, bulge, dimContainer, pattern.Name, axisOrtho, iSwapped == 1)
                            {
                                ForcedSpace = constraintSet.MinimumSpace.Value
                            };
                            if (constraintSet.OptMaxHeight.Activated && (layer.NoLayers(constraintSet.OptMaxHeight.Value) < 1))
                                continue;
                            if (!pattern.GetLayerDimensionsChecked(layer, out double actualLength, out double actualWidth))
                                continue;
                            pattern.GenerateLayer(layer, actualLength, actualWidth);
                            if (0 == layer.Count)
                                continue;
                            if (!constraintSet.OptMaxHeight.Activated || (layer.CountInHeight(constraintSet.OptMaxHeight.Value - offsetZ) > 0))
                                listLayers0.Add(layer);
                            // flip facing outside
                            FlipFacingOutside(ref layer, Facing, dimBox, dimContainer);
                        }
                        catch (Exception ex)
                        {
                            string sSwapped = iSwapped == 1 ? "true" : "false";
                            _log.ErrorFormat($"Pattern: {pattern.Name} Orient: {axisOrtho} Swapped: {sSwapped} Message: {ex.Message}");
                        }
                    }
                }
            }
            // keep only best layers
            if (keepOnlyBest)
            {
                // 1. get best count
                int bestCount = 0;
                foreach (Layer2DBrickImp layer in listLayers0)
                    bestCount = Math.Max(layer.CountInHeight(constraintSet.OptMaxHeight.Value - offsetZ), bestCount);

                // 2. remove any layer that does not match the best count given its orientation
                var listLayers1 = new List<Layer2DBrickImp>();
                foreach (Layer2DBrickImp layer in listLayers0)
                {
                    if (layer.CountInHeight(constraintSet.OptMaxHeight.Value - offsetZ) >= bestCount)
                        listLayers1.Add(layer);
                }
                // 3. copy back in original list
                listLayers0.Clear();
                listLayers0.AddRange(listLayers1);
            }
            if (constraintSet.OptMaxHeight.Activated)
                listLayers0.Sort(new LayerComparerCount(constraintSet, offsetZ));

            return listLayers0;
        }
        private void FlipFacingOutside(ref Layer2DBrickImp layer, int facing, Vector3D dimBox, Vector2D dimContainer)
        {
            if (-1 != facing && layer.IsZOriented)
            {
                for (int iPos = 0; iPos < layer.Positions.Count; ++iPos)
                {
                    var bPos = (BoxPosition)layer.Positions[iPos].Clone();
                    bPos.FlipFacingOutside(facing, dimBox, dimContainer);
                    layer.Positions[iPos] = bPos;
                }
            }
        }
        public List<ILayer2D> BuildLayers(
            Packable packable, Vector2D dimContainer,
            double offsetZ, /* e.g. pallet height */
            ConstraintSetAbstract constraintSet, bool keepOnlyBest)
        {
            var listLayers0 = new List<ILayer2D>();

            if (packable is PackableBrick packableBrick)
            {
                // loop through all patterns
                foreach (LayerPatternBox pattern in LayerPatternBox.All)
                {
                    // loop through all orientation
                    HalfAxis.HAxis[] patternAxes = pattern.IsSymetric ? HalfAxis.Positives : HalfAxis.All;
                    foreach (HalfAxis.HAxis axisOrtho in patternAxes)
                    {
                        // is orientation allowed
                        if (!constraintSet.AllowOrientation(Layer2DBrick.VerticalAxis(axisOrtho)))
                            continue;
                        // not swapped vs swapped pattern
                        for (int iSwapped = 0; iSwapped < 2; ++iSwapped)
                        {
                            try
                            {
                                // does swapping makes sense for this layer pattern ?
                                if (!pattern.CanBeSwapped && (iSwapped == 1))
                                    continue;
                                // instantiate layer
                                var layer = new Layer2DBrickImp(packableBrick.OuterDimensions, packableBrick.Bulge, dimContainer, pattern.Name, axisOrtho, iSwapped == 1)
                                {
                                    ForcedSpace = constraintSet.MinimumSpace.Value
                                };
                                if (layer.NoLayers(constraintSet.OptMaxHeight.Value) < 1)
                                    continue;
                                if (!pattern.GetLayerDimensionsChecked(layer, out double actualLength, out double actualWidth))
                                    continue;
                                pattern.GenerateLayer(layer, actualLength, actualWidth);
                                if (0 == layer.Count)
                                    continue;
                                listLayers0.Add(layer);
                                // flip if needed
                                FlipFacingOutside(ref layer, packableBrick.Facing, packableBrick.OuterDimensions, dimContainer);
                            }
                            catch (Exception ex)
                            {
                                _log.ErrorFormat("Pattern: {0} Orient: {1} Swapped: {2} Message: {3}"
                                    , pattern.Name
                                    , axisOrtho.ToString()
                                    , iSwapped == 1 ? "True" : "False"
                                    , ex.Message);
                            }
                        }
                    }
                } 
            }
            else if (packable is CylinderProperties cylinder)
            {
                // loop through all patterns
                foreach (LayerPatternCyl pattern in LayerPatternCyl.All)
                {
                    // not swapped vs swapped pattern
                    for (int iSwapped = 0; iSwapped < 2; ++iSwapped)
                    {
                        try
                        {
                            var layer = new Layer2DCylImp(cylinder.RadiusOuter, cylinder.Height, dimContainer, iSwapped == 1) { PatternName = pattern.Name };
                            if (!pattern.GetLayerDimensions(layer, out double actualLength, out double actualWidth))
                                continue;
                            pattern.GenerateLayer(layer, actualLength, actualWidth);
                            if (0 == layer.Count)
                                continue;
                            listLayers0.Add(layer);
                        }
                        catch (Exception ex)
                        {
                            _log.ErrorFormat("Pattern: {0} Swapped: {1} Message: {2}"
                                , pattern.Name
                                , iSwapped == 1 ? "True" : "False"
                                , ex.Message);
                        }
                    }
                }
            }
            // keep only best layers
            if (keepOnlyBest)
            {
                // 1. get best count
                int bestCount = 0;
                foreach (ILayer2D layer in listLayers0)
                    bestCount = Math.Max(layer.CountInHeight(constraintSet.OptMaxHeight.Value - offsetZ), bestCount);

                // 2. remove any layer that does not match the best count given its orientation
                var listLayers1 = new List<ILayer2D>();
                foreach (ILayer2D layer in listLayers0)
                {
                    if (layer.CountInHeight(constraintSet.OptMaxHeight.Value - offsetZ) >= bestCount)
                        listLayers1.Add(layer);
                }
                // 3. copy back in original list
                listLayers0.Clear();
                listLayers0.AddRange(listLayers1);
            }
            if (constraintSet.OptMaxHeight.Activated)
                listLayers0.Sort(new LayerComparerCount(constraintSet, offsetZ));

            return listLayers0;
        }
        public Layer2DBrickImp BuildLayer(Vector3D dimBox, Vector2D dimContainer, LayerDescBox layerDesc, double minSpace)
        {
            LayerDescBox layerDescBox = layerDesc;
            // instantiate layer
            var layer = new Layer2DBrickImp(dimBox, Vector3D.Zero, dimContainer, layerDescBox.PatternName, layerDescBox.AxisOrtho, layerDesc.Swapped)
            {
                ForcedSpace = minSpace
            };
            // get layer pattern
            LayerPatternBox pattern = LayerPatternBox.GetByName(layerDesc.PatternName);
            // dimensions
            if (!pattern.GetLayerDimensionsChecked(layer, out double actualLength, out double actualWidth))
                return null;
            pattern.GenerateLayer(
                layer
                , actualLength
                , actualWidth);
            return layer;
        }
        public ILayer2D BuildLayer(Packable packable, Vector2D dimContainer, LayerDesc layerDesc, double minSpace)
        {
            ILayer2D layer = null;
            if (packable is PackableBrick packableBrick)
            {
                if (layerDesc is LayerDescBox layerDescBox)
                {
                    // layer instantiation
                    layer = new Layer2DBrickImp(packable.OuterDimensions, packableBrick.Bulge, dimContainer, layerDesc.PatternName, layerDescBox.AxisOrtho, layerDesc.Swapped) { ForcedSpace = minSpace };
                    // get layer pattern
                    LayerPatternBox pattern = LayerPatternBox.GetByName(layerDesc.PatternName);
                    // dimensions
                    if (!pattern.GetLayerDimensionsChecked(layer as Layer2DBrickImp, out double actualLength, out double actualWidth))
                        return null;
                    pattern.GenerateLayer(
                        layer as Layer2DBrickImp
                        , actualLength
                        , actualWidth);

                    Layer2DBrickImp layerImp = layer as Layer2DBrickImp;
                    FlipFacingOutside(ref layerImp, packableBrick.Facing, packableBrick.OuterDimensions, dimContainer);
                }
                return layer;
            }
            else if (packable.IsCylinder)
            {
                // casts
                var cylProperties = packable as RevSolidProperties;
                // layer instantiation
                layer = new Layer2DCylImp(cylProperties.RadiusOuter, cylProperties.Height, dimContainer, layerDesc.Swapped);
                // get layer pattern
                LayerPatternCyl pattern = LayerPatternCyl.GetByName(layerDesc.PatternName);
                if (!pattern.GetLayerDimensions(layer as Layer2DCylImp, out double actualLength, out double actualWidth))
                    return null;
                pattern.GenerateLayer(layer as Layer2DCylImp, actualLength, actualWidth);
            }
            else
            {
                throw new EngineException(string.Format("Unexpected packable {0} (Type = {1})", packable.Name, packable.GetType().ToString()));
            }
            return layer;
        }
        public ILayer2D BuildLayer(Packable packable, Vector2D dimContainer, LayerDesc layerDesc, Vector2D actualDimensions, double minSpace)
        {
            ILayer2D layer = null;
            LayerPattern pattern = null;
            if (packable is PackableBrick packableBrick)
            {
                LayerDescBox layerDescBox = layerDesc as LayerDescBox;
                // instantiate layer
                layer = new Layer2DBrickImp(packable.OuterDimensions, packableBrick.Bulge, dimContainer, layerDescBox.PatternName, layerDescBox.AxisOrtho, layerDesc.Swapped)
                {
                    ForcedSpace = minSpace
                };
                // get layer pattern
                pattern = LayerPatternBox.GetByName(layerDesc.PatternName);
            }
            else if (packable.IsCylinder)
            {
                var cylProperties = packable as RevSolidProperties;
                layer = new Layer2DCylImp(cylProperties.RadiusOuter, cylProperties.Height, dimContainer, layerDesc.Swapped);
                // get layer pattern
                pattern = LayerPatternCyl.GetByName(layerDesc.PatternName);
            }
            else
            {
                throw new EngineException(string.Format("Unexpected packable {0} (Type = {1})", packable.Name, packable.GetType().ToString()));
            }

            pattern.GenerateLayer(
                layer
                , layer.Swapped ? actualDimensions.Y : actualDimensions.X
                , layer.Swapped ? actualDimensions.X : actualDimensions.Y
                );
            if (layer is Layer2DBrickImp layerImp && packable is PackableBrick packableB)
                FlipFacingOutside(ref layerImp, packableB.Facing, packableB.OuterDimensions, dimContainer);
            return layer;
        }
        public Layer2DBrickImp BuildLayer(Vector3D dimBox, Vector3D bulge, Vector2D dimContainer, LayerDescBox layerDesc, Vector2D actualDimensions, double minSpace)
        {
            // instantiate layer
            var layer = new Layer2DBrickImp(dimBox, bulge, dimContainer, layerDesc.PatternName, layerDesc.AxisOrtho, layerDesc.Swapped)
            {
                ForcedSpace = minSpace
            };
            // get layer pattern
            LayerPatternBox pattern = LayerPatternBox.GetByName(layerDesc.PatternName);
            // build layer
            pattern.GenerateLayer(
                layer
                , layer.Swapped ? actualDimensions.Y : actualDimensions.X
                , layer.Swapped ? actualDimensions.X : actualDimensions.Y);

            FlipFacingOutside(ref layer, Facing, dimBox, dimContainer);
            return layer;
        }
        /// <summary>
        /// Used to compute load dimension
        /// </summary>
        public bool GetDimensions(List<LayerDesc> layers, Packable packable, Vector2D dimContainer, double minSpace, ref Vector2D actualDimensions)
        {
            foreach (LayerDesc layerDesc in layers)
            {
                // dimensions
                double actualLength = 0.0, actualWidth = 0.0;

                if (packable is PackableBrick packableBrick)
                {
                    LayerDescBox layerDescBox = layerDesc as LayerDescBox;
                    // instantiate layer
                    var layer = new Layer2DBrickImp(packable.OuterDimensions, packableBrick.Bulge, dimContainer, layerDescBox.PatternName, layerDescBox.AxisOrtho, layerDesc.Swapped)
                    {
                        ForcedSpace = minSpace
                    };
                    // get layer pattern
                    LayerPatternBox pattern = LayerPatternBox.GetByName(layerDesc.PatternName);
                    // dimensions
                    if (!pattern.GetLayerDimensionsChecked(layer, out actualLength, out actualWidth))
                    {
                        _log.Error(string.Format("Failed to get layer dimension : {0}", pattern.Name));
                        break;
                    }
                }
                else if (packable.IsCylinder)
                {
                    var cylProp = packable as RevSolidProperties;
                    // instantiate layer
                    var layer = new Layer2DCylImp(cylProp.RadiusOuter, cylProp.Height, dimContainer, layerDesc.Swapped);
                    // get layer pattern
                    LayerPatternCyl pattern = LayerPatternCyl.GetByName(layerDesc.PatternName);
                    // dimensions
                    if (!pattern.GetLayerDimensions(layer, out actualLength, out actualWidth))
                    {
                        _log.Error(string.Format("Failed to get layer dimension : {0}", pattern.Name));
                        break;
                    }
                }
                else
                {
                    throw new EngineException(string.Format("Unexpected packable {0} (Type = {1})", packable.Name, packable.GetType().ToString()));
                }

                actualDimensions.X = Math.Max(actualDimensions.X, layerDesc.Swapped ? actualWidth : actualLength);
                actualDimensions.Y = Math.Max(actualDimensions.Y, layerDesc.Swapped ? actualLength : actualWidth);
            }
            return true;
        }
        public static bool GetBestCombination(Vector3D dimBox, Vector3D bulge, Vector3D dimContainer
            , ConstraintSetAbstract constraintSet
            , ref List<KeyValuePair<LayerEncap, int>> listLayer)
        {
            var layDescs = new LayerEncap[3];
            var counts = new int[3] { 0, 0, 0 };
            var heights = new double[3] { 0.0, 0.0, 0.0 };
            Vector2D layerDim = new Vector2D(dimContainer.X, dimContainer.Y);

            // loop through all patterns
            foreach (LayerPatternBox pattern in LayerPatternBox.All)
            {
                // loop through all orientation
                HalfAxis.HAxis[] patternAxes = pattern.IsSymetric ? HalfAxis.Positives : HalfAxis.All;
                foreach (HalfAxis.HAxis axisOrtho in patternAxes)
                {
                    // is orientation allowed
                    if (!constraintSet.AllowOrientation(Layer2DBrick.VerticalAxis(axisOrtho)))
                        continue;
                    // not swapped vs swapped pattern
                    for (int iSwapped = 0; iSwapped < 2; ++iSwapped)
                    {
                        try
                        {
                            // does swapping makes sense for this layer pattern ?
                            if (!pattern.CanBeSwapped && (iSwapped == 1))
                                continue;
                            // instantiate layer
                            var layer = new Layer2DBrickImp(dimBox, bulge, layerDim, pattern.Name, axisOrtho, iSwapped == 1)
                            {
                                ForcedSpace = constraintSet.MinimumSpace.Value
                            };
                            if (layer.NoLayers(constraintSet.OptMaxHeight.Value) < 1)
                                continue;
                            if (!pattern.GetLayerDimensionsChecked(layer, out double actualLength, out double actualWidth))
                                continue;
                            pattern.GenerateLayer(layer, actualLength, actualWidth);
                            if (0 == layer.Count)
                                continue;
                            int iAxisIndex = layer.VerticalDirection;
                            if (layer.Count > counts[iAxisIndex])
                            {
                                counts[iAxisIndex] = layer.Count;
                                layDescs[iAxisIndex] = new LayerEncap(layer.LayerDescriptor);
                                heights[iAxisIndex] = layer.BoxHeight;
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"Pattern: {pattern.Name} Orient: {axisOrtho} Swapped: {iSwapped == 1} Message: {ex.Message}");
                        }
                    }
                }
            }
            double stackingHeight = dimContainer.Z;

            // single layer
            int indexIMax = 0, indexJMax = 0, noIMax = 0, noJMax = 0, iCountMax = 0;
            for (int i=0; i<3; ++i)
            {
                int noLayers = 0;
                if (counts[i] > 0)
                    noLayers = (int)Math.Floor(stackingHeight / heights[i]);
                if (counts[i] * noLayers > iCountMax)
                {
                    iCountMax = counts[i] * noLayers;
                    indexIMax = i;
                    noIMax = noLayers;
                }
            }
            // layer combinations
            int[] comb1 = { 0, 1, 2 };
            int[] comb2 = { 1, 2, 0 };
            for (int i = 0; i < 3; ++i)
            {
                int iComb1 = comb1[i];
                int iComb2 = comb2[i];
                // --swap layers so that the thickest stays at the bottom
                if (heights[iComb2] > heights[iComb1])
                {
                    int iTemp = iComb1;
                    iComb1 = iComb2;
                    iComb2 = iTemp;
                }
                // --
                int noI = 0;
                if (counts[iComb1] != 0)
                    noI = (int)Math.Floor(stackingHeight / heights[iComb1]);
                // search all index
                while (noI > 0)
                {
                    double remainingHeight = stackingHeight - noI * heights[iComb1];
                    int noJ = 0;
                    if (counts[iComb2] != 0)
                        noJ = (int)Math.Floor(remainingHeight / heights[iComb2]);
                    if (noI * counts[iComb1] + noJ * counts[iComb2] > iCountMax)
                    {
                        indexIMax = iComb1;  indexJMax = iComb2;
                        noIMax = noI;   noJMax = noJ;
                        iCountMax = noI * counts[iComb1] + noJ * counts[iComb2];
                    }
                    --noI;
                } // while
            }
            if (noIMax > 0)
                listLayer.Add(new KeyValuePair<LayerEncap, int>(layDescs[indexIMax], noIMax));
            if (noJMax > 0)
                listLayer.Add(new KeyValuePair<LayerEncap, int>(layDescs[indexJMax], noJMax));
            return true;
        }
        public List<Layer2DCylImp> BuildLayers(
            double radius, double height
            , Vector2D dimContainer
            , double offsetZ /* e.g. pallet height */
            , ConstraintSetAbstract constraintSet
            , bool keepOnlyBest)
        {
            var listLayers0 = new List<Layer2DCylImp>();
            foreach (LayerPatternCyl pattern in LayerPatternCyl.All)
            {            
                // not swapped vs swapped pattern
                for (int iSwapped = 0; iSwapped < 2; ++iSwapped)
                {
                    // does swapping makes sense for this layer pattern ?
                    if (!pattern.CanBeSwapped && (iSwapped == 1))
                        continue;
                    // instantiate layer
                    var layer = new Layer2DCylImp(radius, height, dimContainer, iSwapped == 1);
                    layer.PatternName = pattern.Name;

                    double actualLength = 0.0, actualWidth = 0.0;
                    if (!pattern.GetLayerDimensions(layer, out actualLength, out actualWidth))
                        continue;
                    pattern.GenerateLayer(layer, actualLength, actualWidth);
                    listLayers0.Add(layer);
                }

                // keep only best layers
                if (keepOnlyBest)
                {
                    // 1. get best count
                    int bestCount = 0;
                    foreach (Layer2DCylImp layer in listLayers0)
                        bestCount = Math.Max(layer.CountInHeight(constraintSet.OptMaxHeight.Value - offsetZ), bestCount);

                    // 2. remove any layer that does not match the best count given its orientation
                    listLayers0.RemoveAll(layer => 
                        !(layer.CountInHeight(constraintSet.OptMaxHeight.Value - offsetZ) >= bestCount));
                }
                listLayers0.Sort(new LayerCylComparerCount(constraintSet.OptMaxHeight.Value - offsetZ));
            }
            return listLayers0;
        }
        #region Helpers
        private bool GetFacingPosition(BoxPosition bPos, Vector3D dims, int iFacing, bool opposite, ref Vector3D posFacing, ref HalfAxis.HAxis hAxis)
        {
            if (bPos.DirectionHeight != HalfAxis.HAxis.AXIS_Z_P)
                return false;
            posFacing = bPos.Position + dims.Z * Vector3D.ZAxis;
            if (!opposite)
            {
                switch (iFacing)
                {
                    case 0:
                        posFacing += 0.5 * dims.X * HalfAxis.ToVector3D(bPos.DirectionLength);
                        hAxis = bPos.DirectionLength;
                        break;
                    case 1:
                        posFacing += dims.X * HalfAxis.ToVector3D(bPos.DirectionLength) + 0.5 * dims.Y * HalfAxis.ToVector3D(bPos.DirectionWidth);
                        hAxis = bPos.DirectionWidth;
                        break;
                    case 2:
                        posFacing += 0.5 * dims.X * HalfAxis.ToVector3D(bPos.DirectionLength) + dims.Y * HalfAxis.ToVector3D(bPos.DirectionWidth);
                        hAxis = HalfAxis.Opposite(bPos.DirectionLength);
                        break;
                    case 3:
                        posFacing += 0.5 * dims.Y * HalfAxis.ToVector3D(bPos.DirectionWidth);
                        hAxis = HalfAxis.Opposite(bPos.DirectionWidth);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (iFacing)
                {
                    case 0:
                        posFacing += 0.5 * dims.X * HalfAxis.ToVector3D(bPos.DirectionLength) + dims.Y * HalfAxis.ToVector3D(bPos.DirectionWidth);
                        hAxis = HalfAxis.Opposite(bPos.DirectionLength);
                        break;
                    case 1:
                        posFacing += 0.5 * dims.Y * HalfAxis.ToVector3D(bPos.DirectionWidth);
                        hAxis = HalfAxis.Opposite(bPos.DirectionWidth);
                        break;
                    case 2:
                        posFacing += 0.5 * dims.X * HalfAxis.ToVector3D(bPos.DirectionLength);
                        hAxis = bPos.DirectionLength;
                        break;
                    case 3:
                        posFacing += dims.X * HalfAxis.ToVector3D(bPos.DirectionLength) + 0.5 * dims.Y * HalfAxis.ToVector3D(bPos.DirectionWidth);
                        hAxis = bPos.DirectionWidth;
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        #endregion
        #region Non-Public Members
        public int Facing { get; set; } = -1;
        protected static readonly ILog _log = LogManager.GetLogger(typeof(LayerSolver));
        #endregion
    }
}
