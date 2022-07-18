#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Engine;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{
    #region StackBuilderProcessor
    public static class StackBuilderProcessor
    {
        #region Homogeneous
        public static bool GetSolutionList(
            PackableBrick packableProperties, PalletProperties palletProperties, InterlayerProperties interlayerProperties
            , ConstraintSetCasePallet constraintSet
            , ref List<AnalysisLayered> analyses)
        {
            SolverCasePallet solver = new SolverCasePallet(packableProperties, palletProperties, constraintSet);
            analyses = solver.BuildAnalyses(false);
            return analyses.Count > 0;
        }
        public static bool GetSolutionByLayer(
            PackableBrick packableProperties, PalletProperties palletProperties, InterlayerProperties interlayerProperties
            , ConstraintSetCasePallet constraintSet
            , LayerDesc layerDesc
            , Vector3D cameraPosition, bool showCotations, float fontSizeRatio, Size sz
            , ref int layerCount, ref int caseCount, ref int interlayerCount
            , ref double weightTotal, ref double weightLoad, ref double? weightNet
            , ref Vector3D bbLoad, ref Vector3D bbGlob
            , ref double volumeEfficency, ref double? weightEfficiency
            , ref string palletMapPhrase
            , ref byte[] imageBytes
            , ref string[] errors
            )
        {
            List<string> lErrors = new List<string>();

            try
            {
                if (!packableProperties.FitsIn(palletProperties, constraintSet))
                {
                    lErrors.Add($"{packableProperties.Name} does not fit in {palletProperties.Name} with given constraint set!");
                    return false;
                }
                SolutionLayered.SetSolver(new LayerSolver());
                var analysis = new AnalysisCasePallet(packableProperties, palletProperties, constraintSet);
                analysis.AddSolution(new List<LayerDesc>() { layerDesc });

                layerCount = analysis.SolutionLay.LayerCount;
                caseCount = analysis.Solution.ItemCount;
                interlayerCount = analysis.SolutionLay.LayerCount;

                weightLoad = analysis.Solution.LoadWeight;
                weightTotal = analysis.Solution.Weight;

                OptDouble optNetWeight = analysis.Solution.NetWeight;
                weightNet = optNetWeight.Activated ? optNetWeight.Value : (double?)null;
                bbGlob = analysis.Solution.BBoxGlobal.DimensionsVec;
                bbLoad = analysis.Solution.BBoxLoad.DimensionsVec;
                volumeEfficency = analysis.Solution.VolumeEfficiency;
                weightEfficiency = null;
                if (analysis.Solution.WeightEfficiency.Activated)
                    weightEfficiency = analysis.Solution.WeightEfficiency.Value;
                // generate image path
                Graphics3DImage graphics = new Graphics3DImage(sz)
                {
                    FontSizeRatio = fontSizeRatio,
                    CameraPosition = cameraPosition,
                    ShowDimensions = showCotations
                };
                using (ViewerSolution sv = new ViewerSolution(analysis.SolutionLay))
                    sv.Draw(graphics, Transform3D.Identity);
                graphics.Flush();
                Bitmap bmp = graphics.Bitmap;
                ImageConverter converter = new ImageConverter();
                imageBytes = (byte[])converter.ConvertTo(bmp, typeof(byte[]));

                // pallet phrase
                palletMapPhrase = BuildPalletMapPhrase(analysis.SolutionLay);
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
            }
            errors = lErrors.ToArray();
            return (0 == lErrors.Count);
        }
        public static bool GetBestSolution(
            PackableBrick packableProperties, PalletProperties palletProperties, InterlayerProperties interlayer
            , ConstraintSetCasePallet constraintSet, bool allowMultipleLayerOrientations
            , Vector3D cameraPosition, bool showCotations, float fontSizeRatio, Size sz
            , ref int layerCount, ref int caseCount, ref int interlayerCount
            , ref double weightTotal, ref double weightLoad, ref double? weightNet
            , ref Vector3D bbLoad, ref Vector3D bbGlob
            , ref double volumeEfficency, ref double? weightEfficiency
            , ref string palletMapPhrase
            , ref byte[] imageBytes
            , ref string[] errors)
        {
            List<string> lErrors = new List<string>();
            if (!packableProperties.FitsIn(palletProperties, constraintSet))
            {
                lErrors.Add($"{packableProperties.Name} does not fit in {palletProperties.Name} with given constraint set!");
                return false;
            }
            try
            {
                // use a solver and get a list of sorted analyses + select the best one
                SolverCasePallet solver = new SolverCasePallet(packableProperties, palletProperties, constraintSet);
                List<AnalysisLayered> analyses = solver.BuildAnalyses(allowMultipleLayerOrientations);
                if (analyses.Count > 0)
                {
                    // first solution
                    AnalysisLayered analysis = analyses[0];
                    layerCount = analysis.SolutionLay.LayerCount;
                    caseCount = analysis.Solution.ItemCount;
                    interlayerCount = analysis.SolutionLay.LayerCount;

                    weightLoad = analysis.Solution.LoadWeight;
                    weightTotal = analysis.Solution.Weight;

                    OptDouble optNetWeight = analysis.Solution.NetWeight;
                    weightNet = optNetWeight.Activated ? optNetWeight.Value : (double?)null;
                    bbGlob = analysis.Solution.BBoxGlobal.DimensionsVec;
                    bbLoad = analysis.Solution.BBoxLoad.DimensionsVec;
                    volumeEfficency = analysis.Solution.VolumeEfficiency;
                    weightEfficiency = null;
                    if (analysis.Solution.WeightEfficiency.Activated)
                        weightEfficiency = analysis.Solution.WeightEfficiency.Value;
                    palletMapPhrase = BuildPalletMapPhrase(analysis.SolutionLay);

                    Graphics3DImage graphics = null;
                    // generate image path
                    graphics = new Graphics3DImage(sz)
                    {
                        FontSizeRatio = fontSizeRatio,
                        CameraPosition = cameraPosition,
                        ShowDimensions = showCotations
                    };
                    using (ViewerSolution sv = new ViewerSolution(analysis.SolutionLay))
                        sv.Draw(graphics, Transform3D.Identity);
                    graphics.Flush();
                    Bitmap bmp = graphics.Bitmap;
                    ImageConverter converter = new ImageConverter();
                    imageBytes = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
                }
                else
                    lErrors.Add("No solution found!");
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
            }
            errors = lErrors.ToArray();
            return (0 == lErrors.Count);
        }
        public static bool GetBestSolution(PackableBrick packableProperties, BoxProperties caseProperties, InterlayerProperties interlayer
            , ConstraintSetBoxCase constraintSet, bool allowMultipleLayerOrientations
            , Vector3D cameraPosition, bool showCotations, float fontSizeRatio, Size sz
            , ref int layerCount, ref int caseCount, ref int interlayerCount
            , ref double weightTotal, ref double weightLoad, ref double? weightNet
            , ref Vector3D bbLoad, ref Vector3D bbGlob
            , ref double volumeEfficency, ref double? weightEfficiency
            , ref byte[] imageBytes
            , ref string[] errors
            )
        {
            List<string> lErrors = new List<string>();
            if (!packableProperties.FitsIn(caseProperties, constraintSet))
            {
                lErrors.Add($"{packableProperties.Name} does not fit in {caseProperties.Name} with given constraint set!");
                return false;
            }
            try
            {
                SolverBoxCase solver = new SolverBoxCase(packableProperties, caseProperties, constraintSet);
                List<AnalysisLayered> analyses = solver.BuildAnalyses(allowMultipleLayerOrientations);
                if (analyses.Count > 0)
                {
                    AnalysisLayered analysis = analyses[0];
                    layerCount = analysis.SolutionLay.LayerCount;
                    caseCount = analysis.Solution.ItemCount;
                    interlayerCount = analysis.SolutionLay.LayerCount;

                    weightLoad = analysis.Solution.LoadWeight;
                    weightTotal = analysis.Solution.Weight;

                    OptDouble optNetWeight = analysis.Solution.NetWeight;
                    weightNet = optNetWeight.Activated ? optNetWeight.Value : (double?)null;
                    bbGlob = analysis.Solution.BBoxGlobal.DimensionsVec;
                    bbLoad = analysis.Solution.BBoxLoad.DimensionsVec;
                    volumeEfficency = analysis.Solution.VolumeEfficiency;
                    weightEfficiency = null;
                    if (analysis.Solution.WeightEfficiency.Activated)
                        weightEfficiency = analysis.Solution.WeightEfficiency.Value;

                    // generate image path
                    Graphics3DImage graphics = new Graphics3DImage(sz)
                    {
                        FontSizeRatio = fontSizeRatio,
                        CameraPosition = cameraPosition,
                        ShowDimensions = showCotations
                    };
                    using (ViewerSolution sv = new ViewerSolution(analysis.SolutionLay))
                        sv.Draw(graphics, Transform3D.Identity);
                    graphics.Flush();
                    Bitmap bmp = graphics.Bitmap;
                    ImageConverter converter = new ImageConverter();
                    imageBytes = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
                }
                else
                    lErrors.Add("No solution found!");
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
            }
            errors = lErrors.ToArray();
            return (0 == lErrors.Count);
        }
        #endregion

        #region Heterogeneous
        public static bool GetHSolutionBestCasePallet(
            List<ContentItem> items,
            PalletProperties palletProperties,
            HConstraintSetPallet constraintSet,
            Vector3D cameraPosition,
            bool showCotations, float fontSizeRatio, Size sz,
            ref int palletCount,
            ref string algorithm,
            ref byte[] imageBytes,
            ref string[] errors)
        {
            List<string> lErrors = new List<string>();
            try
            {
                var analysis = new HAnalysisPallet(null)
                {
                    Pallet = palletProperties,
                    ConstraintSet = constraintSet,
                    Content = items,
                };
                
                HSolver solver = new HSolver();
                var solutions = solver.BuildSolutions(analysis);
                
                // get first solution (Sharp3DBin packing)
                if (solutions.Count > 0)
                {
                    var sol = solutions[0];
                    algorithm = sol.Algorithm;
                    palletCount = sol.SolItemCount;

                    var tileSize = ImageTiling.TileSize(sz, palletCount);
                    var listTiles = new List<Bitmap>();

                    for (int i = 0; i < palletCount; ++i)
                    {
                        Graphics3DImage graphics = new Graphics3DImage(tileSize)
                        {
                            FontSizeRatio = fontSizeRatio,
                            CameraPosition = cameraPosition,
                            ShowDimensions = showCotations
                        };
                        ViewerHSolution sv = new ViewerHSolution(sol, i);
                        sv.Draw(graphics, Transform3D.Identity);
                        graphics.Flush();
                        listTiles.Add( graphics.Bitmap );
                    }

                    var bmp = ImageTiling.TileImage(sz, listTiles);
                    ImageConverter converter = new ImageConverter();
                    imageBytes = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
                }
                else
                    lErrors.Add("No valid solution found");
            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
            }

            errors = lErrors.ToArray();
            return (0 == lErrors.Count);        
        }
        public static bool GetHSolutionPart(
                   List<ContentItem> items,
                   PalletProperties palletProperties,
                   HConstraintSetPallet constraintSet,
                   int solIndex, int binIndex,
                   Vector3D cameraPosition, bool showCotations, float fontSizeRatio,
                   Size sz,
                   ref double weightLoad, ref double weightTotal,
                   ref Vector3D bbLoad, ref Vector3D bbGlob,
                   ref byte[] imageBytes,
                   ref string[] errors
                   )
        {
            List<string> lErrors = new List<string>();
            try
            {
                var analysis = new HAnalysisPallet(null)
                {
                    Pallet = palletProperties,
                    ConstraintSet = constraintSet,
                    Content = items,
                };
                
                HSolver solver = new HSolver();
                var solutions = solver.BuildSolutions(analysis);

                if (solIndex < solutions.Count)
                {
                    var sol = solutions[solIndex];
                    bbGlob = sol.BBoxGlobal(binIndex).DimensionsVec;
                    bbLoad = sol.BBoxLoad(binIndex).DimensionsVec;
                    weightLoad = sol.LoadWeight(binIndex);
                    weightTotal = sol.Weight(binIndex);

                    Graphics3DImage graphics = new Graphics3DImage(sz)
                    {
                        FontSizeRatio = fontSizeRatio,
                        CameraPosition = cameraPosition,
                        ShowDimensions = showCotations
                    };
                    ViewerHSolution sv = new ViewerHSolution(sol, binIndex);
                    sv.Draw(graphics, Transform3D.Identity);
                    graphics.Flush();
                    Bitmap bmp = graphics.Bitmap;
                    ImageConverter converter = new ImageConverter();
                    imageBytes = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
                }
                else
                    lErrors.Add("No valid solution found");

            }
            catch (Exception ex)
            {
                lErrors.Add(ex.Message);
            }
            errors = lErrors.ToArray();
            return (0 == lErrors.Count);
        }

        #endregion

        #region Helpers
        public static string BuildPalletMapPhrase(SolutionLayered solution)
        {
            if (!(solution.Analysis is AnalysisCasePallet analysis))
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            // 1st part
            Dictionary<LayerPhrase, int> layerPhrases = solution.LayerPhrases;
            bool first = true;
            foreach (LayerPhrase lp in layerPhrases.Keys)
            {
                if (!first) sb.Append("+");
                sb.Append(lp.Count);
                string sDir = string.Empty;
                switch (lp.Axis)
                {
                    case HalfAxis.HAxis.AXIS_X_N:
                    case HalfAxis.HAxis.AXIS_X_P:
                        sDir = "C";
                        break;
                    case HalfAxis.HAxis.AXIS_Y_N:
                    case HalfAxis.HAxis.AXIS_Y_P:
                        sDir = "F";
                        break;
                    case HalfAxis.HAxis.AXIS_Z_N:
                    case HalfAxis.HAxis.AXIS_Z_P:
                        sDir = "B";
                        break;
                }
                sb.Append(sDir);
                if (layerPhrases[lp] > 1)
                    sb.Append($"X{layerPhrases[lp]}");
                first = false;
            }
            // inner space
            sb.Append(" ");
            // 2nd part
            //  EUR -> 80x120 et « CEN » pour palette 100x120
            if (PalletMatchesDimensionsInMM(analysis.PalletProperties, 1200, 800))
                sb.Append("EUR");
            else if (PalletMatchesDimensionsInMM(analysis.PalletProperties, 1200, 1000))
                sb.Append("CEN");
            else
                sb.Append("???");

            return sb.ToString();
        }
        private static bool PalletMatchesDimensionsInMM(PalletProperties palletProperties, double length, double width)
        {
            double lengthInMM = UnitsManager.ConvertLengthTo(palletProperties.Length, UnitsManager.UnitSystem.UNIT_METRIC1);
            double widthInMM = UnitsManager.ConvertLengthTo(palletProperties.Width, UnitsManager.UnitSystem.UNIT_METRIC1);

            return (Math.Abs(lengthInMM - length) < 1.0 && Math.Abs(widthInMM - width) < 1.0)
                || (Math.Abs(lengthInMM - width) < 1.0 && Math.Abs(widthInMM - length) < 1.0);
        }
        #endregion
    }
    #endregion
}
