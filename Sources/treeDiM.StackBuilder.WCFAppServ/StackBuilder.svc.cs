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
                    new ImageDefinition() { ShowDimensions = expectedFormat.ShowCotations, ImageSize = new Size(expectedFormat.Size.CX, expectedFormat.Size.CY) },
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
                        CameraPosition = Graphics3D.Corner_0,
                        ShowDimensions = expectedFormat.ShowCotations,
                        ImageSize = new Size(expectedFormat.Size.CX, expectedFormat.Size.CY),
                        FontSizeRatio = 0.03f
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
        public DCSBCaseConfig[] JJA_GetCaseConfigs(DCSBDim3D dimensions, double weight
            , DCCompSize imageSize, bool showCotations)
        {
            DCSBCaseConfig[] caseConfigs = new DCSBCaseConfig[3];

            for (int i = 0; i < 3; ++i)
            {
                var jjaconfig = new JJAConfig(new double[] { dimensions.M0, dimensions.M1, dimensions.M2 }, weight, i + 1);
                caseConfigs[i].ConfigId = (DCSBConfigId)i;
                caseConfigs[i].Dim3D = new DCSBDim3D(jjaconfig.Length, jjaconfig.Width, jjaconfig.Height);
                caseConfigs[i].Volume = jjaconfig.Volume;
                caseConfigs[i].AreaBottomTop = jjaconfig.AreaBottomTop;
                caseConfigs[i].AreaFrontBack = jjaconfig.AreaFrontBack;
                caseConfigs[i].AreaLeftRight = jjaconfig.AreaLeftRight;
                caseConfigs[i].Stable = (DCSBStabilityEnum)jjaconfig.Stability;
                caseConfigs[i].Conveyability = (DCSBConveyability)jjaconfig.Conveyability;
                caseConfigs[i].ConveyFace = (DCSBOrientationName)jjaconfig.ConveyFace;
                caseConfigs[i].Image = new DCCompFileOutput()
                {
                    Bytes = jjaconfig.GetImageBytes(imageSize.CX, imageSize.CY, 0.02f, showCotations),
                    Format = new DCCompFormat()
                    {
                        Format = EOutFormat.IMAGE,
                        Size = new DCCompSize()
                        {
                            CX = imageSize.CX,
                            CY = imageSize.CY
                        }
                    }
                };
            }
            return caseConfigs;
        }
        public DCSBLoadResultContainer[] JJA_GetMultiContainerResults(DCSBDim3D dimensions, double weight, int noItemPerCase, DCSBContainer[] containers)
        {
            DCSBLoadResultContainer[] loadResultsContainers = new DCSBLoadResultContainer[3 * containers.Length];




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
                // build constraint set
                ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet()
                {
                    Overhang = Vector2D.Zero,
                    OptMaxWeight = new OptDouble(true, dcsbPallet.MaxPalletHeight),
                    OptMaxNumber = OptInt.Zero
                };
                constraintSet.SetMaxHeight(new OptDouble());
                constraintSet.SetAllowedOrientations(new bool[] { false, false, true });
                if (!constraintSet.Valid) throw new Exception("Invalid constraint set");

                for (int iConfig = 0; iConfig < 3; ++iConfig)
                {
                    var jjaConfig = new JJAConfig(new double[] { dimensions.M0, dimensions.M1, dimensions.M2 }, weight, iConfig + 1);
                    // build box
                    var boxProperties = new BoxProperties(null, jjaConfig.Length, jjaConfig.Width, jjaConfig.Height)
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
                        loadResultsPallets[3 * iPallet + iConfig] = new DCSBLoadResultPallet()
                        {
                            ConfigId = (DCSBConfigId)(iConfig + 1),
                            Pallet = dcsbPallet,
                            PalletMapPhrase = palletMapPhrase,
                            NumberOfLayers = layerCount,
                            NumberPerLayer = casePerLayerCount,
                            UpalItem = casePerLayerCount * noItemPerCase,
                            UpalCase = caseCount,
                            IsoBasePercentage = areaEfficiency,
                            IsoVolPercentage = volumeEfficiency,
                            LoadWeight = weightLoad,
                            MaxLoadValidity = weightTotal < dcsbPallet.MaxPalletWeight
                        };
                    }
                }
            }
            return loadResultsPallets;
        }
        public DCSBLoadResultSingleContainer JJA_GetLoadResultSingleContainer(DCSBDim3D dimensions, double weight, int noItemPerCase
            , DCSBContainer container, DCSBConfigId configId
            , DCCompFormat expectedFormat)
        {
            DCSBLoadResultSingleContainer loadResultSingleContainer = new DCSBLoadResultSingleContainer();
            return loadResultSingleContainer;
        }
        public DCSBLoadResultSinglePallet JJA_GetLoadResultSinglePallet(
            DCSBDim3D dimensions, double weight, int pcb
            , DCSBPalletWHeight sbPallet, DCSBConfigId configId
            , DCCompFormat expectedFormat)
        {
            var lErrors = new List<string>();
            var jjaConfig = new JJAConfig(new double[] { dimensions.M0, dimensions.M1, dimensions.M2 }, weight, (int)configId);

            // build boxProperties
            var boxProperties = new BoxProperties(null, jjaConfig.Length, jjaConfig.Width, jjaConfig.Height)
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

            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet()
            {
                Overhang = Vector2D.Zero,
                OptMaxWeight = OptDouble.Zero,
                OptMaxNumber = OptInt.Zero
            };
            constraintSet.SetMaxHeight(new OptDouble(true, sbPallet.MaxPalletHeight));
            constraintSet.SetAllowedOrientations(new bool[] { false, false, true });
            if (!constraintSet.Valid) throw new Exception("Invalid constraint set");

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
                    CameraPosition = Graphics3D.Corner_0,
                    ImageSize = new Size(expectedFormat.Size.CX, expectedFormat.Size.CY)
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
                return new DCSBLoadResultSinglePallet()
                {
                    Status = new DCSBStatus()
                    {
                        Status = DCSBStatusEnu.Success,
                        Error = string.Empty
                    },
                    Result = new DCSBLoadResultPallet()
                    {
                        ConfigId = configId,
                        Pallet = sbPallet,
                        PalletMapPhrase = palletMapPhrase,
                        NumberOfLayers = layerCount,
                        NumberPerLayer = casePerLayerCount,
                        UpalItem = caseCount * pcb,
                        UpalCase = caseCount,
                        IsoBasePercentage = areaEfficiency,
                        IsoVolPercentage = volumeEfficiency,
                        LoadWeight = weightLoad,
                        MaxLoadValidity = weightTotal <= sbPallet.MaxPalletWeight
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
            }
            return new DCSBLoadResultSinglePallet()
            {
                Status = new DCSBStatus()
                {
                    Status = DCSBStatusEnu.FailureLengthOrWidthExceeded,
                },
                Result = new DCSBLoadResultPallet()
                {
                },
                OutFile = null
            };
        }
        #endregion

        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(StackBuilderServ));
        #endregion
    }
}

