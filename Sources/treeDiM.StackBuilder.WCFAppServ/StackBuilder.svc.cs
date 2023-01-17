#region Using directives
using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using log4net;

using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Graphics;
using System.ServiceModel;
using System.ComponentModel;
using System.Web.UI.WebControls;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class StackBuilderServ : IStackBuilder
    {
        #region Solution list
        public DCSBSolution[] SB_GetSolutionList(
            DCSBCase sbCase, DCSBPallet sbPallet, DCSBInterlayer sbInterlayer,
            DCSBConstraintSet sbConstraintSet)
        {
            var listSolution = new List<DCSBSolution>();
            var lErrors = new List<string>();
            try
            {
                BoxProperties boxProperties = new BoxProperties(null, sbCase.DimensionsOuter.M0, sbCase.DimensionsOuter.M1, sbCase.DimensionsOuter.M2)
                {
                    InsideLength = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M0 : 0.0,
                    InsideWidth = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M1 : 0.0,
                    InsideHeight = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M2 : 0.0,
                    TapeColor = Color.FromArgb(sbCase.TapeColor),
                    TapeWidth = new OptDouble(sbCase.TapeWidth != 0.0, sbCase.TapeWidth)
                };
                boxProperties.SetWeight(sbCase.Weight);
                boxProperties.SetNetWeight(new OptDouble(sbCase.NetWeight.HasValue, sbCase.NetWeight.Value));
                if (null != sbCase.Colors && sbCase.Colors.Length >= 6)
                {
                    for (int i = 0; i < 6; ++i)
                        boxProperties.SetColor((HalfAxis.HAxis)i, Color.FromArgb(sbCase.Colors[i]));
                }
                else
                    boxProperties.SetAllColors(Enumerable.Repeat(Color.Chocolate, 6).ToArray());

                PalletProperties palletProperties = null;
                if (null != sbPallet.Dimensions)
                    palletProperties = new PalletProperties(null, sbPallet.PalletType,
                        sbPallet.Dimensions.M0, sbPallet.Dimensions.M1, sbPallet.Dimensions.M2)
                    {
                        Weight = sbPallet.Weight,
                        Color = Color.FromArgb(sbPallet.Color)
                    };
                else
                    palletProperties = new PalletProperties(null, "EUR2", 1200.0, 1000.0, 150.0);

                InterlayerProperties interlayerProperties = null;
                if (null != sbInterlayer)
                    interlayerProperties = new InterlayerProperties(null,
                        sbInterlayer.Name, sbInterlayer.Description,
                        sbInterlayer.Dimensions.M0, sbInterlayer.Dimensions.M1, sbInterlayer.Dimensions.M2,
                        sbInterlayer.Weight, Color.FromArgb(sbInterlayer.Color));

                OptDouble oMaxWeight = null != sbConstraintSet.MaxWeight ? new OptDouble(sbConstraintSet.MaxWeight.Active, sbConstraintSet.MaxWeight.Value_d) : OptDouble.Zero;
                OptDouble oMaxHeight = null != sbConstraintSet.MaxHeight ? new OptDouble(sbConstraintSet.MaxHeight.Active, sbConstraintSet.MaxHeight.Value_d) : OptDouble.Zero;
                OptInt oMaxNumber = null != sbConstraintSet.MaxNumber ? new OptInt(sbConstraintSet.MaxNumber.Active, sbConstraintSet.MaxNumber.Value_i) : OptInt.Zero;
                ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet()
                {
                    Overhang = new Vector2D(sbConstraintSet.Overhang.M0, sbConstraintSet.Overhang.M1),
                    OptMaxWeight = oMaxWeight,
                    OptMaxNumber = oMaxNumber
                };
                constraintSet.SetMaxHeight(oMaxHeight);
                constraintSet.SetAllowedOrientations(new bool[] { sbConstraintSet.Orientation.X, sbConstraintSet.Orientation.Y, sbConstraintSet.Orientation.Z });
                if (!constraintSet.Valid)
                    throw new Exception("Invalid constraint set");

                List<AnalysisLayered> analyses = new List<AnalysisLayered>();
                if (StackBuilderProcessor.GetSolutionList(
                    boxProperties, palletProperties, interlayerProperties,
                    constraintSet,
                    ref analyses
                    ))
                {
                    foreach (var analysis in analyses)
                    {
                        Vector3D bbGlob = analysis.Solution.BBoxGlobal.DimensionsVec;
                        Vector3D bbLoad = analysis.Solution.BBoxLoad.DimensionsVec;
                        OptDouble optNetWeight = analysis.Solution.NetWeight;
                        double? weightNet = optNetWeight.Activated ? optNetWeight.Value : (double?)null;

                        List<string> layerDescs = new List<string>();
                        foreach (var lp in analysis.SolutionLay.LayerPhrases.Keys)
                            layerDescs.Add(lp.LayerDescriptor.ToString());

                        DCSBSolution solution = new DCSBSolution()
                        {
                            LayerCount = analysis.SolutionLay.LayerCount,
                            CaseCount = analysis.Solution.ItemCount,
                            InterlayerCount = analysis.SolutionLay.InterlayerCount,
                            WeightLoad = analysis.Solution.LoadWeight,
                            WeightTotal = analysis.Solution.Weight,
                            NetWeight = weightNet,
                            BBoxLoad = new DCSBDim3D(bbLoad.X, bbLoad.Y, bbLoad.Z),
                            BBoxTotal = new DCSBDim3D(bbGlob.X, bbGlob.Y, bbGlob.Z),
                            Efficiency = analysis.Solution.VolumeEfficiency,
                            OutFile = null,
                            LayerDescs = layerDescs.ToArray(),
                            PalletMapPhrase = StackBuilderProcessor.BuildPalletMapPhrase(analysis.SolutionLay),
                            Errors = lErrors.ToArray()
                        };
                        listSolution.Add(solution);
                    }
                }
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
                _log.Error(ex.ToString());
            }
            return listSolution.ToArray();
        }
        #endregion

        #region Get specific solution
        public DCSBSolution SB_GetSolution(DCSBCase sbCase, DCSBPallet sbPallet, DCSBInterlayer sbInterlayer,
            DCSBConstraintSet sbConstraintSet, string sLayerDesc,
            DCCompFormat expectedFormat
            )
        {
            List<string> lErrors = new List<string>();
            BoxProperties boxProperties = new BoxProperties(null, sbCase.DimensionsOuter.M0, sbCase.DimensionsOuter.M1, sbCase.DimensionsOuter.M2)
            {
                InsideLength = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M0 : 0.0,
                InsideWidth = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M1 : 0.0,
                InsideHeight = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M2 : 0.0,
                TapeColor = Color.FromArgb(sbCase.TapeColor),
                TapeWidth = new OptDouble(sbCase.TapeWidth != 0.0, sbCase.TapeWidth)
            };
            boxProperties.SetWeight(sbCase.Weight);
            boxProperties.SetNetWeight(new OptDouble(sbCase.NetWeight.HasValue, sbCase.NetWeight.Value));
            if (null != sbCase.Colors && sbCase.Colors.Length >= 6)
            {
                for (int i = 0; i < 6; ++i)
                    boxProperties.SetColor((HalfAxis.HAxis)i, Color.FromArgb(sbCase.Colors[i]));
            }
            else
                boxProperties.SetAllColors(Enumerable.Repeat(Color.Chocolate, 6).ToArray());

            PalletProperties palletProperties;
            if (null != sbPallet.Dimensions)
                palletProperties = new PalletProperties(null, sbPallet.PalletType,
                    sbPallet.Dimensions.M0, sbPallet.Dimensions.M1, sbPallet.Dimensions.M2)
                {
                    Weight = sbPallet.Weight,
                    Color = Color.FromArgb(sbPallet.Color)
                };
            else
                palletProperties = new PalletProperties(null, "EUR2", 1200.0, 1000.0, 150.0);

            InterlayerProperties interlayerProperties = null;
            if (null != sbInterlayer)
                interlayerProperties = new InterlayerProperties(null,
                    sbInterlayer.Name, sbInterlayer.Description,
                    sbInterlayer.Dimensions.M0, sbInterlayer.Dimensions.M1, sbInterlayer.Dimensions.M2,
                    sbInterlayer.Weight, Color.FromArgb(sbInterlayer.Color));

            OptDouble oMaxWeight = null != sbConstraintSet.MaxWeight ? new OptDouble(sbConstraintSet.MaxWeight.Active, sbConstraintSet.MaxWeight.Value_d) : OptDouble.Zero;
            OptDouble oMaxHeight = null != sbConstraintSet.MaxHeight ? new OptDouble(sbConstraintSet.MaxHeight.Active, sbConstraintSet.MaxHeight.Value_d) : OptDouble.Zero;
            OptInt oMaxNumber = null != sbConstraintSet.MaxNumber ? new OptInt(sbConstraintSet.MaxNumber.Active, sbConstraintSet.MaxNumber.Value_i) : OptInt.Zero;
            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet()
            {
                Overhang = new Vector2D(sbConstraintSet.Overhang.M0, sbConstraintSet.Overhang.M1),
                OptMaxWeight = oMaxWeight,
                OptMaxNumber = oMaxNumber
            };
            constraintSet.SetMaxHeight(oMaxHeight);
            constraintSet.SetAllowedOrientations(new bool[] { sbConstraintSet.Orientation.X, sbConstraintSet.Orientation.Y, sbConstraintSet.Orientation.Z });
            if (!constraintSet.Valid)
                throw new Exception("Invalid constraint set");

            LayerDesc layerDesc = LayerDescBox.Parse(sLayerDesc);
            int layerCount = 0, caseCount = 0, interlayerCount = 0;
            double weightTotal = 0.0, weightLoad = 0.0, volumeEfficiency = 0.0;
            double? weightEfficiency = 0.0;
            double? weightNet = null;
            Vector3D bbLoad = new Vector3D();
            Vector3D bbGlob = new Vector3D();
            string palletMapPhrase = string.Empty;
            byte[] imageBytes = null;
            string[] errors = null;
            string[] listLayerDesc = new string[] { layerDesc.ToString() };
            StackBuilderProcessor.GetSolutionByLayer(
                boxProperties, palletProperties, interlayerProperties,
                constraintSet, layerDesc,
                new ImageDefinition()
                {
                    CameraPosition = Graphics3D.Corner_0,
                    ImageSize = new Size(expectedFormat.Size.CX, expectedFormat.Size.CY),
                    FontSizeRatio = 0.03f,
                    ShowDimensions = expectedFormat.ShowCotations
                },
                ref layerCount, ref caseCount, ref interlayerCount,
                ref weightTotal, ref weightLoad, ref weightNet,
                ref bbLoad, ref bbGlob,
                ref volumeEfficiency, ref weightEfficiency,
                ref palletMapPhrase,
                ref imageBytes, ref errors
            );

            return new DCSBSolution()
            {
                LayerCount = layerCount,
                CaseCount = caseCount,
                InterlayerCount = interlayerCount,
                WeightLoad = weightLoad,
                WeightTotal = weightTotal,
                NetWeight = weightNet,
                BBoxLoad = new DCSBDim3D(bbLoad.X, bbLoad.Y, bbLoad.Z),
                BBoxTotal = new DCSBDim3D(bbGlob.X, bbGlob.Y, bbGlob.Z),
                Efficiency = volumeEfficiency,
                OutFile = null,
                LayerDescs = listLayerDesc,
                PalletMapPhrase = palletMapPhrase,
                Errors = lErrors.ToArray()
            };
        }
        #endregion

        #region Best solution
        public DCSBSolution SB_GetCasePalletBestSolution(
            DCSBCase sbCase, DCSBPallet sbPallet, DCSBInterlayer sbInterlayer,
            DCSBConstraintSet sbConstraintSet,
            DCCompFormat expectedFormat)
        {
            List<string> lErrors = new List<string>();
            try
            {
                BoxProperties boxProperties = new BoxProperties(null, sbCase.DimensionsOuter.M0, sbCase.DimensionsOuter.M1, sbCase.DimensionsOuter.M2)
                {
                    InsideLength = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M0 : 0.0,
                    InsideWidth = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M1 : 0.0,
                    InsideHeight = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M2 : 0.0,
                    TapeColor = Color.FromArgb(sbCase.TapeColor),
                    TapeWidth = new OptDouble(sbCase.TapeWidth != 0.0, sbCase.TapeWidth)
                };
                boxProperties.SetWeight(sbCase.Weight);
                boxProperties.SetNetWeight(new OptDouble(sbCase.NetWeight.HasValue, sbCase.NetWeight.Value));
                if (null != sbCase.Colors && sbCase.Colors.Length >= 6)
                {
                    for (int i = 0; i < 6; ++i)
                        boxProperties.SetColor((HalfAxis.HAxis)i, Color.FromArgb(sbCase.Colors[i]));
                }
                else
                    boxProperties.SetAllColors(Enumerable.Repeat(Color.Chocolate, 6).ToArray());

                PalletProperties palletProperties = null;
                if (null != sbPallet.Dimensions)
                    palletProperties = new PalletProperties(null, sbPallet.PalletType,
                        sbPallet.Dimensions.M0, sbPallet.Dimensions.M1, sbPallet.Dimensions.M2)
                    {
                        Weight = sbPallet.Weight,
                        Color = Color.FromArgb(sbPallet.Color)
                    };
                else
                    palletProperties = new PalletProperties(null, "EUR2", 1200.0, 1000.0, 150.0);

                InterlayerProperties interlayerProperties = null;
                if (null != sbInterlayer)
                    interlayerProperties = new InterlayerProperties(null,
                        sbInterlayer.Name, sbInterlayer.Description,
                        sbInterlayer.Dimensions.M0, sbInterlayer.Dimensions.M1, sbInterlayer.Dimensions.M2,
                        sbInterlayer.Weight, Color.FromArgb(sbInterlayer.Color));

                OptDouble oMaxWeight = null != sbConstraintSet.MaxWeight ? new OptDouble(sbConstraintSet.MaxWeight.Active, sbConstraintSet.MaxWeight.Value_d) : OptDouble.Zero;
                OptDouble oMaxHeight = null != sbConstraintSet.MaxHeight ? new OptDouble(sbConstraintSet.MaxHeight.Active, sbConstraintSet.MaxHeight.Value_d) : OptDouble.Zero;
                OptInt oMaxNumber = null != sbConstraintSet.MaxNumber ? new OptInt(sbConstraintSet.MaxNumber.Active, sbConstraintSet.MaxNumber.Value_i) : OptInt.Zero;
                ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet()
                {
                    Overhang = new Vector2D(sbConstraintSet.Overhang.M0, sbConstraintSet.Overhang.M1),
                    OptMaxWeight = oMaxWeight,
                    OptMaxNumber = oMaxNumber
                };
                constraintSet.SetMaxHeight(oMaxHeight);
                constraintSet.SetAllowedOrientations(new bool[] { sbConstraintSet.Orientation.X, sbConstraintSet.Orientation.Y, sbConstraintSet.Orientation.Z });
                if (!constraintSet.Valid)
                    throw new Exception("Invalid constraint set");

                Vector3D cameraPosition = Graphics3D.Corner_0;
                int casePerLayerCount = 0, layerCount = 0, caseCount = 0, interlayerCount = 0;
                double weightTotal = 0.0, weightLoad = 0.0, areaEfficiency = 0.0, volumeEfficiency = 0.0;
                double? weightEfficiency = 0.0;
                double? weightNet = null;
                Vector3D bbLoad = new Vector3D();
                Vector3D bbGlob = new Vector3D();
                string palletMapPhrase = string.Empty;
                byte[] imageBytes = null;
                string[] errors = null;

                if (StackBuilderProcessor.GetBestSolution(
                    boxProperties, palletProperties, interlayerProperties,
                    constraintSet, sbConstraintSet.AllowMultipleLayerOrientations,
                    new ImageDefinition()
                    {
                        ShowImage = expectedFormat.Format == EOutFormat.IMAGE,
                        ShowDimensions = expectedFormat.ShowCotations,
                        ImageSize = new Size(expectedFormat.Size.CX, expectedFormat.Size.CY),
                        FontSizeRatio = expectedFormat.FontSizeRatio
                    },
                    ref casePerLayerCount, ref layerCount,
                    ref caseCount, ref interlayerCount,
                    ref weightTotal, ref weightLoad, ref weightNet,
                    ref bbLoad, ref bbGlob,
                    ref areaEfficiency, ref volumeEfficiency, ref weightEfficiency,
                    ref palletMapPhrase,
                    ref imageBytes, ref errors))
                {
                    foreach (string err in errors)
                        lErrors.Add(err);
                    return new DCSBSolution()
                    {
                        LayerCount = layerCount,
                        CaseCount = caseCount,
                        InterlayerCount = interlayerCount,
                        WeightLoad = weightLoad,
                        WeightTotal = weightTotal,
                        NetWeight = weightNet,
                        BBoxLoad = new DCSBDim3D(bbLoad.X, bbLoad.Y, bbLoad.Z),
                        BBoxTotal = new DCSBDim3D(bbGlob.X, bbGlob.Y, bbGlob.Z),
                        Efficiency = volumeEfficiency,
                        OutFile = new DCCompFileOutput()
                        {
                            Bytes = imageBytes,
                            Format = new DCCompFormat()
                            {
                                Format = EOutFormat.IMAGE,
                                Size = new DCCompSize()
                                {
                                    CX = expectedFormat.Size.CX,
                                    CY = expectedFormat.Size.CY
                                }
                            }
                        },
                        PalletMapPhrase = palletMapPhrase,
                        Errors = lErrors.ToArray()
                    };
                }
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
                _log.Error(ex.ToString());
            }
            return new DCSBSolution() { Errors = lErrors.ToArray() };
        }
        public DCSBSolution SB_GetBundlePalletBestSolution(
            DCSBBundle sbBundle, DCSBPallet sbPallet, DCSBInterlayer sbInterlayer
            , DCSBConstraintSet sbConstraintSet
            , DCCompFormat expectedFormat)
        {
            List<string> lErrors = new List<string>();
            try
            {
                BundleProperties bundleProperties = new BundleProperties(null
                    , sbBundle.Name, sbBundle.Description
                    , sbBundle.DimensionsUnit.M0, sbBundle.DimensionsUnit.M1, sbBundle.DimensionsUnit.M2
                    , sbBundle.UnitWeight, sbBundle.Number
                    , Color.FromArgb(sbBundle.Color));
                PalletProperties palletProperties = null;
                if (null != sbPallet.Dimensions)
                    palletProperties = new PalletProperties(null, sbPallet.PalletType,
                        sbPallet.Dimensions.M0, sbPallet.Dimensions.M1, sbPallet.Dimensions.M2)
                    {
                        Weight = sbPallet.Weight,
                        Color = Color.FromArgb(sbPallet.Color)
                    };
                else
                    palletProperties = new PalletProperties(null, "EUR2", 1200.0, 1000.0, 150.0);

                InterlayerProperties interlayerProperties = null;
                if (null != sbInterlayer)
                    interlayerProperties = new InterlayerProperties(null,
                        sbInterlayer.Name, sbInterlayer.Description,
                        sbInterlayer.Dimensions.M0, sbInterlayer.Dimensions.M1, sbInterlayer.Dimensions.M2,
                        sbInterlayer.Weight, Color.FromArgb(sbInterlayer.Color));

                OptDouble oMaxWeight = null != sbConstraintSet.MaxWeight ? new OptDouble(sbConstraintSet.MaxWeight.Active, sbConstraintSet.MaxWeight.Value_d) : OptDouble.Zero;
                OptDouble oMaxHeight = null != sbConstraintSet.MaxHeight ? new OptDouble(sbConstraintSet.MaxHeight.Active, sbConstraintSet.MaxHeight.Value_d) : OptDouble.Zero;
                OptInt oMaxNumber = null != sbConstraintSet.MaxNumber ? new OptInt(sbConstraintSet.MaxNumber.Active, sbConstraintSet.MaxNumber.Value_i) : OptInt.Zero;
                ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet()
                {
                    Overhang = new Vector2D(sbConstraintSet.Overhang.M0, sbConstraintSet.Overhang.M1),
                    OptMaxWeight = oMaxWeight,
                    OptMaxNumber = oMaxNumber
                };
                constraintSet.SetMaxHeight(oMaxHeight);
                constraintSet.SetAllowedOrientations(new bool[] { false, false, true });
                if (!constraintSet.Valid)
                    throw new Exception("Invalid constraint set");

                int casePerLayerCount = 0, layerCount = 0, caseCount = 0, interlayerCount = 0;
                double weightTotal = 0.0, weightLoad = 0.0, areaEfficency = 0.0, volumeEfficiency = 0.0;
                double? weightEfficiency = 0.0;
                double? weightNet = (double?)null;
                Vector3D bbLoad = new Vector3D();
                Vector3D bbGlob = new Vector3D();
                byte[] imageBytes = null;
                string[] errors = null;
                string palletMapPhrase = string.Empty;

                if (StackBuilderProcessor.GetBestSolution(
                    bundleProperties, palletProperties, interlayerProperties,
                    constraintSet, false,
                    new ImageDefinition()
                    {
                        ShowImage = expectedFormat.Format == EOutFormat.IMAGE,
                        CameraPosition = Graphics3D.Corner_0,
                        ShowDimensions = expectedFormat.ShowCotations,
                        ImageSize = new Size(expectedFormat.Size.CX, expectedFormat.Size.CY),
                        FontSizeRatio = expectedFormat.FontSizeRatio
                    },
                    ref casePerLayerCount, ref layerCount,
                    ref caseCount, ref interlayerCount,
                    ref weightTotal, ref weightLoad, ref weightNet,
                    ref bbLoad, ref bbGlob,
                    ref areaEfficency, ref volumeEfficiency, ref weightEfficiency,
                    ref palletMapPhrase,
                    ref imageBytes, ref errors))
                {
                    foreach (string err in errors)
                        lErrors.Add(err);
                    return new DCSBSolution()
                    {
                        LayerCount = layerCount,
                        CaseCount = caseCount,
                        InterlayerCount = interlayerCount,
                        WeightLoad = weightLoad,
                        WeightTotal = weightTotal,
                        NetWeight = weightNet,
                        BBoxLoad = new DCSBDim3D(bbLoad.X, bbLoad.Y, bbLoad.Z),
                        BBoxTotal = new DCSBDim3D(bbGlob.X, bbGlob.Y, bbGlob.Z),
                        Efficiency = volumeEfficiency,
                        OutFile = new DCCompFileOutput()
                        {
                            Bytes = imageBytes,
                            Format = new DCCompFormat()
                            {
                                Format = EOutFormat.IMAGE,
                                Size = new DCCompSize()
                                {
                                    CX = expectedFormat.Size.CX,
                                    CY = expectedFormat.Size.CY
                                }
                            }
                        },
                        Errors = lErrors.ToArray()
                    };
                }
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
                _log.Error(ex.ToString());
            }
            return new DCSBSolution() { Errors = lErrors.ToArray() };
        }
        public DCSBSolution SB_GetBundleCaseBestSolution(
            DCSBBundle sbBundle, DCSBCase sbCase
            , DCSBConstraintSet sbConstraintSet
            , DCCompFormat expectedFormat)
        {
            List<string> lErrors = new List<string>();
            try
            {
                BundleProperties bundleProperties = new BundleProperties(null
                    , sbBundle.Name, sbBundle.Description
                    , sbBundle.DimensionsUnit.M0, sbBundle.DimensionsUnit.M1, sbBundle.DimensionsUnit.M2
                    , sbBundle.UnitWeight, sbBundle.Number
                    , Color.FromArgb(sbBundle.Color));
                BoxProperties caseProperties = new BoxProperties(null)
                {
                    InsideLength = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M0 : 0.0,
                    InsideWidth = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M1 : 0.0,
                    InsideHeight = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M2 : 0.0,
                    TapeColor = Color.FromArgb(sbCase.TapeColor),
                    TapeWidth = new OptDouble(sbCase.TapeWidth != 0.0, sbCase.TapeWidth)
                };
                caseProperties.SetWeight(sbCase.Weight);
                if (null != sbCase.Colors && sbCase.Colors.Length >= 6)
                {
                    for (int i = 0; i < 6; ++i)
                        caseProperties.SetColor((HalfAxis.HAxis)i, Color.FromArgb(sbCase.Colors[i]));
                }
                else
                    caseProperties.SetAllColors(Enumerable.Repeat<Color>(Color.Chocolate, 6).ToArray());

                OptDouble oMaxWeight = null != sbConstraintSet.MaxWeight ? new OptDouble(sbConstraintSet.MaxWeight.Active, sbConstraintSet.MaxWeight.Value_d) : OptDouble.Zero;
                OptInt oMaxNumber = null != sbConstraintSet.MaxNumber ? new OptInt(sbConstraintSet.MaxNumber.Active, sbConstraintSet.MaxNumber.Value_i) : OptInt.Zero;
                ConstraintSetBoxCase constraintSet = new ConstraintSetBoxCase(caseProperties)
                {
                    OptMaxWeight = oMaxWeight,
                    OptMaxNumber = oMaxNumber
                };
                if (!constraintSet.Valid)
                    throw new Exception("Invalid constraint set");
                constraintSet.SetAllowedOrientations(new bool[] { false, false, true });

                Vector3D cameraPosition = Graphics3D.Corner_0;
                int layerCount = 0, caseCount = 0, interlayerCount = 0;
                double weightTotal = 0.0, weightLoad = 0.0, volumeEfficiency = 0.0;
                double? weightEfficiency = 0.0;
                double? weightNet = (double?)null;
                Vector3D bbLoad = new Vector3D();
                Vector3D bbGlob = new Vector3D();
                byte[] imageBytes = null;
                string[] errors = null;

                if (StackBuilderProcessor.GetBestSolution(
                    bundleProperties, caseProperties, null,
                    constraintSet, false,
                    cameraPosition, expectedFormat.ShowCotations, 0.03f,
                    new Size(expectedFormat.Size.CX, expectedFormat.Size.CY),
                    ref layerCount, ref caseCount, ref interlayerCount,
                    ref weightTotal, ref weightLoad, ref weightNet,
                    ref bbLoad, ref bbGlob,
                    ref volumeEfficiency, ref weightEfficiency,
                    ref imageBytes, ref errors))
                {
                    foreach (string err in errors)
                        lErrors.Add(err);
                    return new DCSBSolution()
                    {
                        LayerCount = layerCount,
                        CaseCount = caseCount,
                        InterlayerCount = interlayerCount,
                        WeightLoad = weightLoad,
                        WeightTotal = weightTotal,
                        NetWeight = weightNet,
                        BBoxLoad = new DCSBDim3D(bbLoad.X, bbLoad.Y, bbLoad.Z),
                        BBoxTotal = new DCSBDim3D(bbGlob.X, bbGlob.Y, bbGlob.Z),
                        Efficiency = volumeEfficiency,
                        OutFile = new DCCompFileOutput()
                        {
                            Bytes = imageBytes,
                            Format = new DCCompFormat()
                            {
                                Format = EOutFormat.IMAGE,
                                Size = new DCCompSize()
                                {
                                    CX = expectedFormat.Size.CX,
                                    CY = expectedFormat.Size.CY
                                }
                            }
                        },
                        Errors = lErrors.ToArray()
                    };
                }
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
                _log.Error(ex.ToString());
            }
            return new DCSBSolution() { Errors = lErrors.ToArray() };
        }
        public DCSBSolution SB_GetBoxCaseBestSolution(
            DCSBCase sbBox, DCSBCase sbCase, DCSBInterlayer sbInterlayer
            , DCSBConstraintSet sbConstraintSet
            , DCCompFormat expectedFormat)
        {
            var lErrors = new List<string>();
            try
            {
                BoxProperties boxProperties = new BoxProperties(null
                    , sbBox.DimensionsOuter.M0, sbBox.DimensionsOuter.M1, sbBox.DimensionsOuter.M2)
                {
                };
                boxProperties.SetWeight(sbBox.Weight);
                boxProperties.SetNetWeight(new OptDouble(sbBox.NetWeight.HasValue, sbBox.NetWeight.Value));
                if (null != sbBox.Colors && sbBox.Colors.Length >= 6)
                    for (int i = 0; i < 6; ++i)
                        boxProperties.SetColor((HalfAxis.HAxis)i, Color.FromArgb(sbBox.Colors[i]));
                else
                    boxProperties.SetAllColors(Enumerable.Repeat<Color>(Color.Turquoise, 6).ToArray());
                BoxProperties caseProperties = new BoxProperties(null)
                {
                    InsideLength = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M0 : 0.0,
                    InsideWidth = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M1 : 0.0,
                    InsideHeight = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M2 : 0.0,
                    TapeColor = Color.FromArgb(sbCase.TapeColor),
                    TapeWidth = new OptDouble(sbCase.TapeWidth != 0.0, sbCase.TapeWidth)
                };
                caseProperties.SetWeight(sbCase.Weight);
                if (null != sbCase.Colors && sbCase.Colors.Length >= 6)
                {
                    for (int i = 0; i < 6; ++i)
                        caseProperties.SetColor((HalfAxis.HAxis)i, Color.FromArgb(sbCase.Colors[i]));
                }
                else
                    caseProperties.SetAllColors(Enumerable.Repeat<Color>(Color.Chocolate, 6).ToArray());

                OptDouble oMaxWeight = null != sbConstraintSet.MaxWeight ? new OptDouble(sbConstraintSet.MaxWeight.Active, sbConstraintSet.MaxWeight.Value_d) : OptDouble.Zero;
                OptInt oMaxNumber = null != sbConstraintSet.MaxNumber ? new OptInt(sbConstraintSet.MaxNumber.Active, sbConstraintSet.MaxNumber.Value_i) : OptInt.Zero;
                ConstraintSetBoxCase constraintSet = new ConstraintSetBoxCase(caseProperties)
                {
                    OptMaxWeight = oMaxWeight,
                    OptMaxNumber = oMaxNumber
                };
                constraintSet.SetAllowedOrientations(new bool[] { false, false, true });
                if (!constraintSet.Valid)
                    throw new Exception("Invalid constraint set");

                Vector3D cameraPosition = Graphics3D.Corner_0;
                int layerCount = 0, caseCount = 0, interlayerCount = 0;
                double weightTotal = 0.0, weightLoad = 0.0, volumeEfficiency = 0.0;
                double? weightEfficiency = 0.0;
                double? weightNet = null;
                Vector3D bbLoad = new Vector3D();
                Vector3D bbGlob = new Vector3D();
                byte[] imageBytes = null;
                string[] errors = null;

                if (StackBuilderProcessor.GetBestSolution(
                    boxProperties, caseProperties, null,
                    constraintSet, sbConstraintSet.AllowMultipleLayerOrientations,
                    cameraPosition, expectedFormat.ShowCotations, 0.03f,
                    new Size(expectedFormat.Size.CX, expectedFormat.Size.CY),
                    ref layerCount, ref caseCount, ref interlayerCount,
                    ref weightTotal, ref weightLoad, ref weightNet,
                    ref bbLoad, ref bbGlob,
                    ref volumeEfficiency, ref weightEfficiency,
                    ref imageBytes, ref errors))
                {
                    foreach (string err in errors)
                        lErrors.Add(err);
                    return new DCSBSolution()
                    {
                        LayerCount = layerCount,
                        CaseCount = caseCount,
                        InterlayerCount = interlayerCount,
                        WeightLoad = weightLoad,
                        WeightTotal = weightTotal,
                        NetWeight = weightNet,
                        BBoxLoad = new DCSBDim3D(bbLoad.X, bbLoad.Y, bbLoad.Z),
                        BBoxTotal = new DCSBDim3D(bbGlob.X, bbGlob.Y, bbGlob.Z),
                        Efficiency = volumeEfficiency,
                        OutFile = new DCCompFileOutput()
                        {
                            Bytes = imageBytes,
                            Format = new DCCompFormat()
                            {
                                Format = EOutFormat.IMAGE,
                                Size = new DCCompSize()
                                {
                                    CX = expectedFormat.Size.CX,
                                    CY = expectedFormat.Size.CY
                                }
                            }
                        },
                        Errors = lErrors.ToArray()
                    };
                }
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
                _log.Error(ex.ToString());
            }
            return new DCSBSolution() { Errors = lErrors.ToArray() };
        }

        #endregion

        #region Heterogeneous Pallet Stacking
        public DCSBHSolution SB_GetHSolutionBestCasePallet(DCSBContentItem[] sbConstentItems
            , DCSBPallet sbPallet
            , DCSBHConstraintSet sbConstraintSet
            , DCCompFormat expectedFormat)
        {
            var lErrors = new List<string>();

            Vector3D cameraPosition = Graphics3D.Corner_0;

            int palletCount = 0;
            byte[] imageBytes = null;
            string algorithm = string.Empty;
            string[] errors = null;

            try
            {
                if (StackBuilderProcessor.GetHSolutionBestCasePallet(
                    ConvertDCSBContentItems(sbConstentItems),
                    ConvertDCSBPallet(sbPallet),
                    ConvertDCSBConstraintSet(sbConstraintSet),
                    cameraPosition, expectedFormat.ShowCotations, 0.03f,
                    new Size(expectedFormat.Size.CX, expectedFormat.Size.CY),
                    ref palletCount,
                    ref algorithm,
                    ref imageBytes,
                    ref errors))
                {
                    foreach (string err in errors)
                        lErrors.Add(err);

                    return new DCSBHSolution()
                    {
                        SolIndex = 0,
                        PalletCount = palletCount,
                        Algorithm = string.Empty,
                        OutFile = new DCCompFileOutput()
                        {
                            Bytes = imageBytes,
                            Format = new DCCompFormat()
                            {
                                Format = EOutFormat.IMAGE,
                                Size = new DCCompSize()
                                {
                                    CX = expectedFormat.Size.CX,
                                    CY = expectedFormat.Size.CY
                                }
                            }
                        },
                        Errors = lErrors.ToArray()
                    };
                }
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
                _log.Error(ex.ToString());
            }
            return new DCSBHSolution()
            {
                Errors = lErrors.ToArray()
            };
        }
        /*
        public DCSBHSolutionList SB_GetHCasePalletSolution(DCSBContentItem[] sbContentItems, DCSBPallet sbPallet, DCSBHConstraintSet sbConstraintSet)
        {
            var lErrors = new List<string>();
            var sbSolutions = new List<DCSBHSolution>();

            try
            {
                // list of content items
                var contentItems = new List<ContentItem>();
                foreach (var sbci in sbContentItems)
                {
                    contentItems.Add(
                        new ContentItem(ConvertDCSBCase(sbci.Case), sbci.Number, sbci.Orientation.ToArray())
                        );
                }
                // pallet
                var palletProperties = ConvertDCSBPallet(sbPallet);
                // constraint set
                var constraintSet = new HConstraintSetPallet()
                {
                    Overhang = new Vector2D(sbConstraintSet.Overhang.M0, sbConstraintSet.Overhang.M1),
                    MaximumHeight = sbConstraintSet.MaxHeight.Value_d
                };
                // solve
                var solver = new HSolver();
                var solutions = solver.BuildSolutions(
                    new HAnalysisPallet(null)
                    {
                        Content = contentItems,
                        Pallet = palletProperties,
                        ConstraintSet = constraintSet
                    }
                    );
                // analyse each solution
                int solIndex = 0;
                foreach (var sol in solutions)
                {
                    sbSolutions.Add(
                        new DCSBHSolution()
                        {
                            SolIndex = solIndex,
                            PalletCount = sol.SolItemCount,
                            Algorithm = sol.Algorithm
                        }
                        );
                }
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
                _log.Error(ex.ToString());
            }
            return new DCSBHSolutionList()
            {
                Solutions = sbSolutions.ToArray(),
                Errors = lErrors.ToArray()
            };
        }
        */

        public DCSBHSolutionItem SB_GetHSolutionPart(
            DCSBContentItem[] sbContentItems
            , DCSBPallet sbPallet, DCSBHConstraintSet sbConstraintSet
            , int solIndex, int binIndex
            , DCCompFormat expectedFormat)
        {
            var lErrors = new List<string>();

            Vector3D cameraPosition = Graphics3D.Corner_0;
            byte[] imageBytes = null;
            string[] errors = null;
            double weightLoad = 0.0, weightTotal = 0.0;
            Vector3D bbLoad = new Vector3D();
            Vector3D bbGlob = new Vector3D();

            try
            {
                if (StackBuilderProcessor.GetHSolutionPart(
                   ConvertDCSBContentItems(sbContentItems),
                   ConvertDCSBPallet(sbPallet),
                   ConvertDCSBConstraintSet(sbConstraintSet),
                   solIndex, binIndex,
                   cameraPosition, expectedFormat.ShowCotations, 0.03f,
                   new Size(expectedFormat.Size.CX, expectedFormat.Size.CY),
                   ref weightLoad, ref weightTotal,
                   ref bbLoad, ref bbGlob,
                   ref imageBytes,
                   ref errors)
                   )
                {
                    foreach (string err in errors)
                        lErrors.Add(err);
                    return new DCSBHSolutionItem()
                    {
                        SolIndex = solIndex,
                        BinIndex = binIndex,
                        WeightLoad = weightLoad,
                        WeightTotal = weightTotal,
                        BBoxLoad = new DCSBDim3D(bbLoad.X, bbLoad.Y, bbLoad.Z),
                        BBoxTotal = new DCSBDim3D(bbGlob.X, bbGlob.Y, bbGlob.Z),
                        OutFile = new DCCompFileOutput()
                        {
                            Bytes = imageBytes,
                            Format = new DCCompFormat()
                            {
                                Format = EOutFormat.IMAGE,
                                Size = new DCCompSize()
                                {
                                    CX = expectedFormat.Size.CX,
                                    CY = expectedFormat.Size.CY
                                }
                            }
                        },
                        Errors = lErrors.ToArray()
                    };
                }
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
                _log.Error(ex.ToString());
            }
            return new DCSBHSolutionItem()
            {
                Errors = lErrors.ToArray()
            };
        }

        private BoxProperties ConvertDCSBCase(DCSBCase sbCase)
        {
            var boxProperties = new BoxProperties(null, sbCase.DimensionsOuter.M0, sbCase.DimensionsOuter.M1, sbCase.DimensionsOuter.M2)
            {
                InsideLength = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M0 : 0.0,
                InsideWidth = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M1 : 0.0,
                InsideHeight = null != sbCase.DimensionsInner ? sbCase.DimensionsInner.M2 : 0.0,
                TapeColor = Color.FromArgb(sbCase.TapeColor),
                TapeWidth = new OptDouble(sbCase.TapeWidth != 0.0, sbCase.TapeWidth)
            };
            boxProperties.SetWeight(sbCase.Weight);
            boxProperties.SetNetWeight(new OptDouble(sbCase.NetWeight.HasValue, sbCase.NetWeight.Value));
            if (null != sbCase.Colors && sbCase.Colors.Length >= 6)
            {
                for (int i = 0; i < 6; ++i)
                    boxProperties.SetColor((HalfAxis.HAxis)i, Color.FromArgb(sbCase.Colors[i]));
            }
            else
                boxProperties.SetAllColors(Enumerable.Repeat(Color.Chocolate, 6).ToArray());
            return boxProperties;
        }
        private PalletProperties ConvertDCSBPallet(DCSBPallet sbPallet)
        {
            if (null == sbPallet.Dimensions)
                throw new Exception("Pallet dimensions were not defined!");
            return new PalletProperties(null,
                sbPallet.PalletType,
                    sbPallet.Dimensions.M0,
                    sbPallet.Dimensions.M1,
                    sbPallet.Dimensions.M2)
            {
                Weight = sbPallet.Weight,
                Color = Color.FromArgb(sbPallet.Color)
            };
        }
        private List<ContentItem> ConvertDCSBContentItems(DCSBContentItem[] dcsbItems)
        {
            var listContentItems = new List<ContentItem>();
            foreach (var dcsbItem in dcsbItems)
                listContentItems.Add(
                    new ContentItem(ConvertDCSBCase(dcsbItem.Case), dcsbItem.Number)
                    {
                        AllowedOrientations = new bool[]
                        {
                            dcsbItem.Orientation.X,
                            dcsbItem.Orientation.Y,
                            dcsbItem.Orientation.Z
                        },
                        PriorityLevel = dcsbItem.PriorityIndex
                    }
                    );
            return listContentItems;
        }
        private HConstraintSetPallet ConvertDCSBConstraintSet(DCSBHConstraintSet constraintSet)
        {
            return new HConstraintSetPallet()
            {
                MaximumHeight = constraintSet.MaxHeight.Value_d,
                Overhang = new Vector2D(constraintSet.Overhang.M0, constraintSet.Overhang.M1)
            };
        }
        #endregion

        #region JJA Specific methods
        public DCSBCaseConfig[] JJA_GetCaseConfigs(DCSBDim3D dimensions, double weight, int pcb, DCCompFormat format)
        {
            DCSBCaseConfig[] caseConfigs = new DCSBCaseConfig[3];
            for (int iAxis = 0; iAxis < 3; ++iAxis)
            {
                var jjaconfig = new JJAConfig(new double[] { dimensions.M0, dimensions.M1, dimensions.M2 }, weight, pcb, JJAConfig.Axis(iAxis));
                caseConfigs[iAxis] = new DCSBCaseConfig
                {
                    Orientation = (DCSBOrientation)iAxis,
                    Length = jjaconfig.Length,
                    Width = jjaconfig.Width,
                    Height = jjaconfig.Height,
                    Weight = jjaconfig.Weight,
                    Pcb = jjaconfig.Pcb,
                    Dim3D = new DCSBDim3D(jjaconfig.Length, jjaconfig.Width, jjaconfig.Height),
                    Volume = jjaconfig.Volume,
                    AreaBottomTop = jjaconfig.AreaBottomTop,
                    AreaFrontBack = jjaconfig.AreaFrontBack,
                    AreaLeftRight = jjaconfig.AreaLeftRight,
                    Stable = (DCSBStabilityEnum)jjaconfig.Stability,
                    Conveyability = (DCSBConveyability)jjaconfig.Conveyability,
                    ConveyFace = (DCSBOrientationName)jjaconfig.ConveyFace,
                    Image = new DCCompFileOutput()
                    {
                        Bytes = jjaconfig.GetImageBytes(iAxis, format.Size.CX, format.Size.CY, format.FontSizeRatio, format.ShowCotations),
                        Format = format
                    }
                };
            }
            return caseConfigs;
        }
        public DCSBLoadResultContainer[] JJA_GetMultiContainerResults(DCSBDim3D dimensions, double weight, int noItemPerCase, DCSBContainer[] containers)
        {
            var lErrors = new List<string>();
            DCSBLoadResultContainer[] loadResultsContainers = new DCSBLoadResultContainer[3 * containers.Length];

            for (int iContainer = 0; iContainer < containers.Length; ++iContainer)
            {
                var dcsbContainer = containers[iContainer];
                // build container
                var containerProperties = new TruckProperties(null, dcsbContainer.Dimensions.M0, dcsbContainer.Dimensions.M1, dcsbContainer.Dimensions.M2)
                {
                    Color = Color.FromArgb(dcsbContainer.Color),
                    AdmissibleLoadWeight = dcsbContainer.MaxLoadWeight.HasValue ? dcsbContainer.MaxLoadWeight.Value : 0.0
                };
                for (int iOrientation = 0; iOrientation < 3; ++iOrientation)
                {
                    // build box
                    var boxProperties = new BoxProperties(null, dimensions.M0, dimensions.M1, dimensions.M2)
                    {
                        InsideLength = 0.0,
                        InsideWidth = 0.0,
                        InsideHeight = 0.0,
                        TapeColor = Color.Beige,
                        TapeWidth = new OptDouble(true, 50.0)
                    };
                    boxProperties.SetWeight(weight);
                    boxProperties.SetNetWeight(new OptDouble(false, 0.0));
                    boxProperties.SetAllColors(Enumerable.Repeat(Color.Chocolate, 6).ToArray());

                    // constraint set
                    var constraintSet = new ConstraintSetCaseTruck(containerProperties) { };
                    constraintSet.SetAllowedOrientations(new bool[] { iOrientation == 0, iOrientation == 1, iOrientation == 2 });
                    if (!constraintSet.Valid)
                    {
                        lErrors.Add($"Invalid constraint set");
                        continue;
                    }

                    // --- checking validity
                    DCSBStatusEnu statusValidity = DCSBStatusEnu.Success;
                    string errorMessage = string.Empty;
                    if (boxProperties.Height >= containerProperties.Height)
                    {
                        statusValidity = DCSBStatusEnu.FailureHeightExceeded;
                        errorMessage = $"Container height exceeded: {boxProperties.Height} > {containerProperties.Height}";
                    }
                    if (
                        (boxProperties.Length > containerProperties.Length || boxProperties.Width > containerProperties.Width)
                        && (boxProperties.Length > containerProperties.Width || boxProperties.Width > containerProperties.Length)
                        )
                    {
                        statusValidity = DCSBStatusEnu.FailureLengthOrWidthExceeded;
                        errorMessage = $"Length and/or width exceeded";
                    }
                    if (dcsbContainer.MaxLoadWeight.HasValue && boxProperties.Weight > dcsbContainer.MaxLoadWeight.Value)
                    {
                        statusValidity = DCSBStatusEnu.FailureWeightExceeded;
                        errorMessage = $"Maximum load weight exceeded: {boxProperties.Weight} > {dcsbContainer.MaxLoadWeight.Value}";
                    }
                    if (DCSBStatusEnu.Success != statusValidity)
                    {
                        loadResultsContainers[3 * iContainer + iOrientation] = new DCSBLoadResultContainer()
                        {
                            Orientation = (DCSBOrientation)iOrientation,
                            Status = new DCSBStatus() { Status = DCSBStatusEnu.FailureHeightExceeded, Error = errorMessage }
                        };
                    }
                    else // we are valid : compute
                    {
                        int casePerLayerCount = 0, layerCount = 0;
                        int caseCount = 0; int interlayerCount = 0;
                        double weightTotal = 0, weightLoad = 0;
                        double? weightNet = null;
                        var bbLoad = Vector3D.Zero;
                        var bbGlob = Vector3D.Zero;
                        double areaEfficiency = 0, volumeEfficiency = 0;
                        double? weightEfficiency = null;
                        string palletMapPhrase = string.Empty;
                        byte[] imageBytes = null;
                        string[] errors = null;

                        if (StackBuilderProcessor.GetBestSolution(
                            boxProperties, containerProperties,
                            constraintSet, false,
                            ImageDefinition.NoImage,
                            ref casePerLayerCount, ref layerCount,
                            ref caseCount, ref interlayerCount,
                            ref weightTotal, ref weightLoad, ref weightNet,
                            ref bbLoad, ref bbGlob,
                            ref areaEfficiency, ref volumeEfficiency, ref weightEfficiency,
                            ref palletMapPhrase,
                            ref imageBytes, ref errors)
                            )
                        {
                            foreach (string err in errors)
                                lErrors.Add(err);
                            loadResultsContainers[3 * iContainer + iOrientation] = new DCSBLoadResultContainer()
                            {
                                Status = new DCSBStatus() { Status = DCSBStatusEnu.Success },
                                Orientation = (DCSBOrientation)iOrientation,
                                Container = dcsbContainer,
                                NumberOfLayers = layerCount,
                                NumberPerLayer = casePerLayerCount,
                                TotalWeight = weightTotal,
                                LoadWeight = weightLoad,
                                IsoBasePercentage = areaEfficiency,
                                IsoVolPercentage = volumeEfficiency,
                                MaxLoadValidity = weightLoad < dcsbContainer.MaxLoadWeight,
                                UpalItem = casePerLayerCount * noItemPerCase,
                                UpalCase = caseCount
                            };
                        }
                        else
                        {
                            loadResultsContainers[3 * iContainer + iOrientation] = new DCSBLoadResultContainer()
                            {
                                Orientation = (DCSBOrientation)iOrientation,
                                Status = new DCSBStatus() { Status = DCSBStatusEnu.FailureHeightExceeded, Error = string.Join("|", errors.ToArray()) }
                            };
                        }
                    }
                }
            }
            return loadResultsContainers;
        }
        public DCSBLoadResultPallet[] JJA_GetMultiPalletResults(DCSBDim3D dimensions, double weight, int noItemPerCase, DCSBPalletWHeight[] pallets)
        {
            var lErrors = new List<string>();
            DCSBLoadResultPallet[] loadResultsPallets = new DCSBLoadResultPallet[3 * pallets.Length];

            for (int iPallet = 0; iPallet < pallets.Length; ++iPallet)
            {
                var dcsbPallet = pallets[iPallet];
                // build pallet
                var palletProperties = new PalletProperties(null, dcsbPallet.PalletType
                    , dcsbPallet.Dimensions.M0, dcsbPallet.Dimensions.M1, dcsbPallet.Dimensions.M2)
                {
                    Weight = dcsbPallet.Weight,
                    Color = Color.FromArgb(dcsbPallet.Color)
                };

                for (int iOrientation = 0; iOrientation < 3; ++iOrientation)
                {
                    // build box
                    var boxProperties = new BoxProperties(null, dimensions.M0, dimensions.M1, dimensions.M2)
                    {
                        InsideLength = 0.0,
                        InsideWidth = 0.0,
                        InsideHeight = 0.0,
                        TapeColor = Color.Beige,
                        TapeWidth = new OptDouble(true, 50.0)
                    };
                    boxProperties.SetWeight(weight);
                    boxProperties.SetNetWeight(new OptDouble(false, 0.0));
                    boxProperties.SetAllColors(Enumerable.Repeat(Color.Chocolate, 6).ToArray());

                    // build constraint set
                    ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet()
                    {
                        Overhang = Vector2D.Zero,
                        OptMaxWeight = new OptDouble(false, dcsbPallet.MaxPalletLoad),
                        OptMaxNumber = OptInt.Zero
                    };
                    constraintSet.SetMaxHeight(new OptDouble() { Activated = true, Value = dcsbPallet.MaxPalletHeight });
                    constraintSet.SetAllowedOrientations(new bool[] { iOrientation == 0, iOrientation == 1, iOrientation == 2 });

                if (!constraintSet.Valid) throw new Exception("Invalid constraint set");
                    int casePerLayerCount = 0, layerCount = 0, caseCount = 0, interlayerCount = 0;
                    double weightTotal = 0.0, weightLoad = 0.0;
                    double areaEfficiency = 0.0, volumeEfficiency = 0.0;
                    double? weightNet = null;
                    double? weightEfficiency = null;
                    string palletMapPhrase = string.Empty;
                    byte[] imageBytes = null;
                    var bbLoad = Vector3D.Zero;
                    var bbGlob = Vector3D.Zero;
                    string[] errors = null;

                    if (StackBuilderProcessor.GetBestSolution(
                        boxProperties, palletProperties, null,
                        constraintSet, false,
                        ImageDefinition.NoImage,
                        ref casePerLayerCount, ref layerCount,
                        ref caseCount, ref interlayerCount,
                        ref weightTotal, ref weightLoad, ref weightNet,
                        ref bbLoad, ref bbGlob,
                        ref areaEfficiency, ref volumeEfficiency, ref weightEfficiency,
                        ref palletMapPhrase,
                        ref imageBytes, ref errors))
                    {
                        foreach (string err in errors)
                            lErrors.Add(err);
                        loadResultsPallets[3 * iPallet + iOrientation] = new DCSBLoadResultPallet()
                        {
                            Status = new DCSBStatus() { Status = DCSBStatusEnu.Success },
                            Orientation = (DCSBOrientation)iOrientation,
                            Pallet = dcsbPallet,
                            PalletMapPhrase = palletMapPhrase,
                            NumberOfLayers = layerCount,
                            NumberPerLayer = casePerLayerCount,
                            UpalItem = casePerLayerCount * noItemPerCase,
                            UpalCase = caseCount,
                            IsoBasePercentage = areaEfficiency,
                            IsoVolPercentage = volumeEfficiency,
                            LoadWeight = weightLoad,
                            TotalWeight = weightTotal, 
                            MaxLoadValidity = weightTotal < dcsbPallet.MaxPalletLoad
                        };
                    }
                    else
                        loadResultsPallets[3 * iPallet + iOrientation] = new DCSBLoadResultPallet()
                        {
                            Orientation = (DCSBOrientation)iOrientation,
                            Status = new DCSBStatus() { Status = DCSBStatusEnu.FailureHeightExceeded, Error = string.Join("|", errors.ToArray()) }
                        };
                }
            }
            return loadResultsPallets;
        }
        public DCSBLoadResultSingleContainer JJA_GetLoadResultSingleContainer(DCSBDim3D dimensions, double weight, int pcb
            , DCSBContainer container, DCSBOrientation orientation
            , DCCompFormat expectedFormat)
        {
            //var jjaConfig = new JJAConfig(new double[] { dimensions.M0, dimensions.M1, dimensions.M2 }, weight, pcb, JJAConfig.Axis((int)orientation));

            // build boxProperties
            var boxProperties = new BoxProperties(null, dimensions.M0, dimensions.M1, dimensions.M2)
            {
                InsideLength = 0.0,
                InsideWidth = 0.0,
                InsideHeight = 0.0,
                TapeColor = Color.Beige,
                TapeWidth = new OptDouble(true, 50.0)
            };
            boxProperties.SetWeight(weight);
            boxProperties.SetNetWeight(new OptDouble(false, 0.0));
            boxProperties.SetAllColors(Enumerable.Repeat(Color.Chocolate, 6).ToArray());

            // container
            var containerProperties = new TruckProperties(null, container.Dimensions.M0, container.Dimensions.M1, container.Dimensions.M2)
            {
                Color = Color.FromArgb(container.Color),
                AdmissibleLoadWeight = container.MaxLoadWeight.HasValue ? container.MaxLoadWeight.Value : 0.0
            };

            // --- checking validity
            DCSBStatusEnu statusValidity = DCSBStatusEnu.Success;
            string errorMessage = string.Empty;
            if (boxProperties.Height >= containerProperties.Height)
            {
                statusValidity = DCSBStatusEnu.FailureHeightExceeded;
                errorMessage = $"Container height exceeded: {boxProperties.Height} > {containerProperties.Height}";
            }
            if (
                (boxProperties.Length > containerProperties.Length || boxProperties.Width > containerProperties.Width)
                && (boxProperties.Length > containerProperties.Width || boxProperties.Width > containerProperties.Length)
                )
            {
                statusValidity = DCSBStatusEnu.FailureLengthOrWidthExceeded;
                errorMessage = $"Length and/or width exceeded";
            }
            if (container.MaxLoadWeight.HasValue && boxProperties.Weight > container.MaxLoadWeight.Value)
            {
                statusValidity = DCSBStatusEnu.FailureWeightExceeded;
                errorMessage = $"Maximum load weight exceeded: {boxProperties.Weight} > {container.MaxLoadWeight.Value}";
            }

            if (statusValidity != DCSBStatusEnu.Success)
                return new DCSBLoadResultSingleContainer()
                {
                    Status = new DCSBStatus()
                    {
                        Status = statusValidity,
                        Error = errorMessage
                    },
                    Result = null,
                    OutFile = null
                };
            // ---

            // constraint set
            var constraintSet = new ConstraintSetCaseTruck(containerProperties) { };
            constraintSet.SetAllowedOrientations(new bool[] { DCSBOrientation.FrontBack == orientation, DCSBOrientation.LeftRight == orientation, DCSBOrientation.BottomTop == orientation });

            int casePerLayerCount = 0, layerCount = 0, caseCount = 0, interlayerCount = 0;
            double weightTotal = 0.0, weightLoad = 0.0, volumeEfficiency = 0.0;
            double areaEfficiency = 0.0;
            double? weightEfficiency = 0.0;
            double? weightNet = (double?)null;
            Vector3D bbLoad = new Vector3D();
            Vector3D bbGlob = new Vector3D();
            byte[] imageBytes = null;
            string[] errors = null;
            string palletMapPhrase = string.Empty;

            if (StackBuilderProcessor.GetBestSolution(
                boxProperties, containerProperties,
                constraintSet, false,
                new ImageDefinition()
                {
                    ShowImage = expectedFormat.Format == EOutFormat.IMAGE,
                    CameraPosition = Graphics3D.Corner_0,
                    ImageSize = new Size(expectedFormat.Size.CX, expectedFormat.Size.CY),
                    FontSizeRatio = expectedFormat.FontSizeRatio
                },
                ref casePerLayerCount, ref layerCount,
                ref caseCount, ref interlayerCount,
                ref weightTotal, ref weightLoad, ref weightNet,
                ref bbLoad, ref bbGlob,
                ref areaEfficiency, ref volumeEfficiency, ref weightEfficiency,
                ref palletMapPhrase,
                ref imageBytes, ref errors
                )
                )
                return new DCSBLoadResultSingleContainer()
                {
                    Status = new DCSBStatus()
                    {
                        Status = DCSBStatusEnu.Success,
                        Error = string.Empty
                    },
                    Result = new DCSBLoadResultContainer()
                    {
                        Orientation = orientation,
                        Container = container,
                        NumberOfLayers = layerCount,
                        NumberPerLayer = casePerLayerCount,
                        UpalItem = caseCount * pcb,
                        UpalCase = caseCount,
                        IsoBasePercentage = areaEfficiency,
                        IsoVolPercentage = volumeEfficiency,
                        LoadWeight = weightLoad,
                        TotalWeight = weightTotal,
                        MaxLoadValidity = weightTotal <= container.MaxLoadWeight

                    },
                    OutFile = new DCCompFileOutput()
                    {
                        Bytes = imageBytes,
                        Format = new DCCompFormat()
                        {
                            Format = EOutFormat.IMAGE,
                            Size = new DCCompSize()
                            {
                                CX = expectedFormat.Size.CX,
                                CY = expectedFormat.Size.CY
                            }
                        }
                    }
                };
            else
                return new DCSBLoadResultSingleContainer()
                {
                    Status = new DCSBStatus()
                    {
                        Status = DCSBStatusEnu.FailureLengthOrWidthExceeded,
                        Error = string.Empty
                    },
                    Result = null,
                    OutFile = null
                };
        }
        public DCSBLoadResultSinglePallet JJA_GetLoadResultSinglePallet(
            DCSBDim3D dimensions, double weight, int pcb
            , DCSBPalletWHeight sbPallet, DCSBOrientation orientation
            , DCCompFormat expectedFormat)
        {
            // build boxProperties
            var boxProperties = new BoxProperties(null, dimensions.M0, dimensions.M1, dimensions.M2)
            {
                InsideLength = 0.0,
                InsideWidth = 0.0,
                InsideHeight = 0.0,
                TapeColor = Color.Beige,
                TapeWidth = new OptDouble(true, 50.0)
            };
            boxProperties.SetWeight(weight);
            boxProperties.SetNetWeight(new OptDouble(false, 0.0));
            boxProperties.SetAllColors(Enumerable.Repeat(Color.Chocolate, 6).ToArray());

            PalletProperties palletProperties;
            if (null != sbPallet.Dimensions)
                palletProperties = new PalletProperties(null, sbPallet.PalletType,
                    sbPallet.Dimensions.M0, sbPallet.Dimensions.M1, sbPallet.Dimensions.M2)
                {
                    Weight = sbPallet.Weight,
                    AdmissibleLoadWeight = sbPallet.MaxPalletLoad,
                    Color = Color.FromArgb(sbPallet.Color)
                };
            else
                palletProperties = new PalletProperties(null, "EUR2", 1200.0, 1000.0, 150.0);

            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet()
            {
                Overhang = Vector2D.Zero,
                OptMaxWeight = OptDouble.Zero,
                OptMaxNumber = OptInt.Zero
            };
            constraintSet.SetMaxHeight(new OptDouble(true, sbPallet.MaxPalletHeight));
            constraintSet.SetAllowedOrientations(new bool[] { DCSBOrientation.FrontBack == orientation, DCSBOrientation.LeftRight == orientation, DCSBOrientation.BottomTop == orientation });
            if (!constraintSet.Valid) throw new Exception("Invalid constraint set");
            double boxLength, boxWidth, boxHeight;
            switch (orientation)
            {
                case DCSBOrientation.FrontBack:
                    boxLength = Math.Max(boxProperties.Width, boxProperties.Height);
                    boxWidth = Math.Min(boxProperties.Width, boxProperties.Height);
                    boxHeight = boxProperties.Length;
                    break;
                case DCSBOrientation.LeftRight:
                    boxLength = Math.Max(boxProperties.Length, boxProperties.Height);
                    boxWidth = Math.Min(boxProperties.Length, boxProperties.Height);
                    boxHeight = boxProperties.Width;
                    break;
                default:
                    boxLength = boxProperties.Length;
                    boxWidth = boxProperties.Width;
                    boxHeight = boxProperties.Height;
                    break;
            }
            // --- checking validity
            DCSBStatusEnu statusValidity = DCSBStatusEnu.Success;
            string errorMessage = string.Empty;
            if (boxHeight + palletProperties.Height >= sbPallet.MaxPalletHeight)
            {
                statusValidity = DCSBStatusEnu.FailureHeightExceeded;
                errorMessage = $"Maximum height exceeded: {boxHeight + palletProperties.Height} > {sbPallet.MaxPalletHeight}";
            }
            if (
                (boxLength > palletProperties.Length || boxWidth > palletProperties.Width)
                && (boxLength > palletProperties.Width || boxWidth > palletProperties.Length)
                )
            {
                statusValidity = DCSBStatusEnu.FailureLengthOrWidthExceeded;
                errorMessage = $"Length and/or width exceeded";
            }
            if (boxProperties.Weight > sbPallet.MaxPalletLoad)
            {
                statusValidity = DCSBStatusEnu.FailureWeightExceeded;
                errorMessage = $"Maximum load weight exceeded: {boxProperties.Weight} > {sbPallet.MaxPalletLoad}";
            }

            if (statusValidity != DCSBStatusEnu.Success)
                return new DCSBLoadResultSinglePallet()
                {
                    Status = new DCSBStatus()
                    {
                        Status = statusValidity,
                        Error = errorMessage
                    },
                    Result = null,
                    OutFile = null,                     
                    SuggestPalletLength = null,
                    SuggestPalletWidth = null,
                    SuggestPalletDim = null,
                    SuggestPalletHeight = null,
                    SuggestCaseDim1 = null,
                    SuggestCaseDim2 = null
                };
            // ---

            int casePerLayerCount = 0, layerCount = 0, caseCount = 0, interlayerCount = 0;
            double weightTotal = 0.0, weightLoad = 0.0, volumeEfficiency = 0.0;
            double areaEfficiency = 0.0;
            double? weightEfficiency = 0.0;
            double? weightNet = (double?)null;
            Vector3D bbLoad = new Vector3D();
            Vector3D bbGlob = new Vector3D();
            byte[] imageBytes = null;
            string[] errors = null;
            string palletMapPhrase = string.Empty;

            if (StackBuilderProcessor.GetBestSolution(
                boxProperties, palletProperties, null,
                constraintSet, false,
                new ImageDefinition()
                {
                    ShowImage = expectedFormat.Format == EOutFormat.IMAGE,
                    CameraPosition = Graphics3D.Corner_0,
                    ImageSize = new Size(expectedFormat.Size.CX, expectedFormat.Size.CY),
                    FontSizeRatio = expectedFormat.FontSizeRatio
                },
                ref casePerLayerCount, ref layerCount,
                ref caseCount, ref interlayerCount,
                ref weightTotal, ref weightLoad, ref weightNet,
                ref bbLoad, ref bbGlob,
                ref areaEfficiency, ref volumeEfficiency, ref weightEfficiency,
                ref palletMapPhrase,
                ref imageBytes, ref errors))
            {
                // build suggestions
                var dimBox = new Vector3D(dimensions.M0, dimensions.M1, dimensions.M2);
                var dimContainer = new Vector2D(palletProperties.Length + 2.0 * constraintSet.Overhang.X, palletProperties.Width + 2.0 * constraintSet.Overhang.Y);
                var layerSolver = new LayerSolver();
                int perLayerCountFrom = 0, perLayerCountTo = 0;
                var dimContainerTo = Vector2D.Zero;

                // increase pallet dimensions
                DCSBSuggestIncreasePalletXY suggestPalletLength = null;
                int iDir = 0;
                if (layerSolver.FindMinPalletDimXYIncreaseForGain(dimBox, dimContainer, palletProperties.Height, constraintSet, iDir, ref perLayerCountFrom, ref perLayerCountTo, ref dimContainerTo))
                    suggestPalletLength = new DCSBSuggestIncreasePalletXY()
                    {
                        Success = true,
                        Dim = iDir,
                        PalletDimFrom = new DCSBDim2D(dimContainer.X, dimContainer.Y),
                        PalletDimTo = new DCSBDim2D(dimContainerTo.X, dimContainerTo.Y),
                        PerLayerCountFrom = perLayerCountFrom,
                        PerLayerCountTo = perLayerCountTo,
                        CaseCountFrom = perLayerCountFrom * layerCount,
                        CaseCountTo = perLayerCountTo * layerCount
                    };

                DCSBSuggestIncreasePalletXY suggestPalletWidth = null;
                iDir = 1;
                if (layerSolver.FindMinPalletDimXYIncreaseForGain(dimBox, dimContainer, palletProperties.Height, constraintSet, iDir, ref perLayerCountFrom, ref perLayerCountTo, ref dimContainerTo))
                    suggestPalletWidth = new DCSBSuggestIncreasePalletXY()
                    {
                        Success = true,
                        Dim = iDir,
                        PalletDimFrom = new DCSBDim2D(dimContainer.X, dimContainer.Y),
                        PalletDimTo = new DCSBDim2D(dimContainerTo.X, dimContainerTo.Y),
                        PerLayerCountFrom = perLayerCountFrom,
                        PerLayerCountTo = perLayerCountTo,
                        CaseCountFrom = perLayerCountFrom * layerCount,
                        CaseCountTo = perLayerCountTo * layerCount
                    };

                DCSBSuggestIncreasePalletXY suggestPalletDim= null;
                iDir = -1;
                if (layerSolver.FindMinPalletDimXYIncreaseForGain(dimBox, dimContainer, palletProperties.Height, constraintSet, iDir, ref perLayerCountFrom, ref perLayerCountTo, ref dimContainerTo))
                    suggestPalletDim = new DCSBSuggestIncreasePalletXY()
                    {
                        Success = true,
                        Dim = iDir,
                        PalletDimFrom = new DCSBDim2D(dimContainer.X, dimContainer.Y),
                        PalletDimTo = new DCSBDim2D(dimContainerTo.X, dimContainerTo.Y),
                        PerLayerCountFrom = perLayerCountFrom,
                        PerLayerCountTo = perLayerCountTo,
                        CaseCountFrom = perLayerCountFrom * layerCount,
                        CaseCountTo = perLayerCountTo * layerCount
                    };

                int layerCountFrom = 0, layerCountTo = 0;
                int caseCountFrom = 0, caseCountTo = 0;
                double heightFrom = 0.0, heightTo = 0.0;
                DCSBSuggestIncreasePalletZ suggestPalletZ = null;
                if (layerSolver.FindMinDimZIncreaseForGain(dimBox, dimContainer, palletProperties.Height, constraintSet
                    , ref layerCountFrom, ref layerCountTo
                    , ref caseCountFrom, ref caseCountTo
                    , ref heightFrom, ref heightTo))
                    suggestPalletZ = new DCSBSuggestIncreasePalletZ()
                    {
                        Success = true,
                        LayerCountFrom = layerCountFrom,
                        LayerCountTo = layerCountTo,
                        CaseCountFrom = caseCountFrom,
                        CaseCountTo = caseCountTo,
                        HeightFrom = heightFrom,
                        HeightTo = heightTo
                    };
                // decrease box dim1
                DCSBSuggestDecreaseCaseXY suggestCaseDim1 = null;
                var dimBoxTo = Vector3D.Zero;
                iDir = 0;
                if (layerSolver.FindMinDimXYBoxDecreaseForGain(dimBox, dimContainer, palletProperties.Height, constraintSet, iDir
                    , ref perLayerCountFrom, ref perLayerCountTo, ref dimBoxTo))
                {
                    suggestCaseDim1 = new DCSBSuggestDecreaseCaseXY()
                    {
                        Success = true,
                        PerLayerCountFrom = perLayerCountFrom,
                        PerLayerCountTo = perLayerCountTo,
                        CaseCountFrom = caseCountFrom,
                        CaseCountTo = caseCountTo,
                        CaseDimFrom = new DCSBDim3D(dimBox.X, dimBox.Y, dimBox.Z),
                        CaseDimTo = new DCSBDim3D(dimBoxTo.X, dimBoxTo.Y, dimBoxTo.Z)
                    };
                }
                DCSBSuggestDecreaseCaseXY suggestCaseDim2 = null;
                iDir = 1;
                if (layerSolver.FindMinDimXYBoxDecreaseForGain(dimBox, dimContainer, palletProperties.Height, constraintSet, iDir
                    , ref perLayerCountFrom, ref perLayerCountTo, ref dimBoxTo))
                {
                    suggestCaseDim2 = new DCSBSuggestDecreaseCaseXY()
                    {
                        Success = true,
                        PerLayerCountFrom = perLayerCountFrom,
                        PerLayerCountTo = perLayerCountTo,
                        CaseCountFrom = caseCountFrom,
                        CaseCountTo = caseCountTo,
                        CaseDimFrom = new DCSBDim3D(dimBox.X, dimBox.Y, dimBox.Z),
                        CaseDimTo = new DCSBDim3D(dimBoxTo.X, dimBoxTo.Y, dimBoxTo.Z)
                    };
                }

                return new DCSBLoadResultSinglePallet()
                {
                    Status = new DCSBStatus()
                    {
                        Status = DCSBStatusEnu.Success,
                        Error = string.Join(" | ", errors)
                    },
                    Result = new DCSBLoadResultPallet()
                    {
                        Orientation = (DCSBOrientation)orientation,
                        Pallet = sbPallet,
                        PalletMapPhrase = palletMapPhrase,
                        NumberOfLayers = layerCount,
                        NumberPerLayer = casePerLayerCount,
                        UpalItem = caseCount * pcb,
                        UpalCase = caseCount,
                        IsoBasePercentage = areaEfficiency,
                        IsoVolPercentage = volumeEfficiency,
                        LoadWeight = weightLoad,
                        TotalWeight = weightTotal,
                        MaxLoadValidity = weightTotal <= sbPallet.MaxPalletLoad
                    },
                    OutFile = new DCCompFileOutput()
                    {
                        Bytes = imageBytes,
                        Format = new DCCompFormat()
                        {
                            Format = EOutFormat.IMAGE,
                            Size = new DCCompSize()
                            {
                                CX = expectedFormat.Size.CX,
                                CY = expectedFormat.Size.CY
                            }
                        }
                    },
                    SuggestPalletLength = suggestPalletLength,
                    SuggestPalletWidth = suggestPalletWidth,
                    SuggestPalletDim = suggestPalletDim,
                    SuggestPalletHeight = suggestPalletZ,
                    SuggestCaseDim1 = suggestCaseDim1,
                    SuggestCaseDim2 = suggestCaseDim2
                };
            }

    

            return new DCSBLoadResultSinglePallet()
            {
                Status = new DCSBStatus()
                {
                    Status = DCSBStatusEnu.FailureLengthOrWidthExceeded,
                    Error = string.Join(" | ", errors)
                },
                Result = new DCSBLoadResultPallet()
                {
                },
                OutFile = null,
                SuggestPalletLength = null,
                SuggestPalletWidth = null,
                SuggestPalletDim = null,
                SuggestPalletHeight = null,
                SuggestCaseDim1 = null,
                SuggestCaseDim2 = null

            };
        }
        #endregion

        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(StackBuilderServ));
        #endregion
    }
}

