using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;

namespace treeDiM.StackBuilder.Exporters
{
    /// <summary>
    /// This uses classes generated from xml schema StackBuilderXMLExport.xsd
    /// Command line is: xsd /c /namespace:stb /language:CS StackBuilderXMLExport.xsd
    /// </summary>
    public class ExporterXML: ExporterRobot
    {
        #region Static members
        public static string FormatName => "xml";
        #endregion

        public ExporterXML() {}
        public override string Name => FormatName;
        public override string Filter => "eXchange Markup Language (*.xml)|*.xml";
        public override string Extension => "xml";
        public override System.Drawing.Bitmap BrandLogo => Properties.Resources.treeDiM;
        public override void Export(AnalysisLayered analysis, ref Stream stream)
        {
            SolutionLayered sol = analysis.SolutionLay;

            // build orderDocument element
            orderDocument document = new orderDocument()
            {
                date = DateTime.Now,
                unit = CurrentUnit,
                author = analysis.ParentDocument != null ? analysis.ParentDocument.Author : string.Empty,
                orderType = new orderDocumentOrderType()
                {
                    orderNumber = analysis.Name,
                    orderLine = new orderLine()
                    {
                        itemId = 1,
                        quantity = analysis.Solution.ItemCount
                    },
                    loadSpace = BuildLoadSpace(analysis),
                    itemList = BuildItemArray(analysis),
                    load = new load()
                    {
                        loadSpaceId = 1,
                        statistics = new statistics()
                        {
                            loadVolume = sol.LoadVolume,
                            weightLoad = sol.LoadWeight,
                            volumeUtilization = sol.VolumeEfficiency,
                            weightUtilization = sol.WeightEfficiency.ToDouble(),
                            cOfG = new cOfG() { x = sol.COfG.X, y = sol.COfG.Y, z = sol.COfG.Z },
                            loadHeight = sol.BBoxLoad.Height
                        },
                        placement = BuildPlacementArray(sol, analysis)
                    }
                }
            };

            // serialization
            XmlSerializer serializer = new XmlSerializer(typeof(orderDocument));
            serializer.Serialize(stream, document);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }
        public override void Export(AnalysisLayered analysis, NumberFormatInfo nfi, ref StringBuilder sb)
        {
        }
        public override void Export(RobotPreparation robotPreparation, NumberFormatInfo nfi, ref StringBuilder sb)
        {
        }
        #region Helpers
        private units CurrentUnit
        {
            get
            {
                switch (UnitsManager.CurrentUnitSystem)
                {
                    case UnitsManager.UnitSystem.UNIT_METRIC1: return units.mmkg;
                    case UnitsManager.UnitSystem.UNIT_METRIC2: return units.cmkg;
                    case UnitsManager.UnitSystem.UNIT_IMPERIAL:
                    case UnitsManager.UnitSystem.UNIT_US: return units.inlb;
                    default: throw new Exception("Unexpected unit system");
                }
            }
        }
        private loadSpace BuildLoadSpace(AnalysisLayered analysis)
        {
            if (analysis is AnalysisPackablePallet analysisPackablePallet)
            {
                PalletProperties palletProperties = analysisPackablePallet.PalletProperties;
                ConstraintSetPackablePallet constraintSet = analysisPackablePallet.ConstraintSet as ConstraintSetPackablePallet;
                return new loadSpace()
                {
                    id = 1,
                    name = palletProperties.Name,
                    length = palletProperties.Length,
                    width = palletProperties.Width,
                    baseHeight = palletProperties.Height,
                    maxLengthOverhang = constraintSet.Overhang.X,
                    maxWidthOverhang = constraintSet.Overhang.Y,
                    maxLoadHeight = constraintSet.OptMaxHeight.Activated ? constraintSet.OptMaxHeight.Value : 0.0,
                    maxLoadWeight = constraintSet.OptMaxWeight.Activated ? constraintSet.OptMaxWeight.Value : 0.0
                };
            }
            else
                throw new Exception($"Unexpected analysis type : {analysis.GetType()}");
        }
        private item[] BuildItemArray(AnalysisLayered analysis)
        {
            Packable packable = analysis.Content;
            List<item> items = new List<item>();
            int itemIndex = 0;

            // case ?
            if (analysis is AnalysisCasePallet analysisCasePallet)
            {
                ConstraintSetCasePallet constraintSet = analysisCasePallet.ConstraintSet as ConstraintSetCasePallet;
                bool[] orient = constraintSet.AllowedOrientations;
                StringBuilder sbOrient = new StringBuilder();
                foreach (bool b in orient)
                { sbOrient.Append(b ? "1" : "0"); }

                items.Add(
                    new item()
                    {
                        id = ++itemIndex,
                        name = packable.Name,
                        length = packable.OuterDimensions.X,
                        width = packable.OuterDimensions.Y,
                        height = packable.OuterDimensions.Z,
                        weight = packable.Weight,
                        maxWeightOnTop = 0.0,
                        permittedOrientations = sbOrient.ToString()
                    }
                );
            }
            // cylinder ?
            else if (analysis is AnalysisCylinderPallet analysisCylinderPallet)
            {
                items.Add(
                    new item()
                    {
                        id = ++itemIndex,
                        name = packable.Name,
                        length = packable.OuterDimensions.X,
                        width = packable.OuterDimensions.Y,
                        height = packable.OuterDimensions.Z,
                        maxWeightOnTop = 0.0,
                        permittedOrientations = "001"
                    }
                );
            }
            else
                throw new Exception($"Unexpected analysis type : {analysis.GetType()}");

            // interlayers
            OffsetIndexInterlayers = itemIndex + 1;
            foreach (var interlayer in analysis.Interlayers)
            {
                items.Add(
                    new item()
                    {
                        id = ++itemIndex,
                        name = interlayer.Name,
                        length = interlayer.Length,
                        width = interlayer.Width,
                        height = interlayer.Thickness,
                        weight = interlayer.Weight
                    }
                );
            }

            return items.ToArray();
        }
        private placement[] BuildPlacementArray(SolutionLayered sol, AnalysisLayered analysis)
        {
            List<placement> lPlacements = new List<placement>();
            List<ILayer> layers = sol.Layers;
            foreach (ILayer layer in layers)
            {
                if (layer is Layer3DBox layerBox)
                {
                    layerBox.Sort(analysis.Content, Layer3DBox.SortType.DIST_MAXCORNER);
                    foreach (BoxPosition bPosition in layerBox)
                    {
                        Vector3D writtenPosition = ConvertPosition(bPosition, analysis.ContentDimensions);
                        lPlacements.Add(
                            new placement()
                            {
                                itemId = 1,
                                x = writtenPosition.X,
                                y = writtenPosition.Y,
                                z = writtenPosition.Z,
                                LSpecified = true,
                                L = ToAxis(bPosition.DirectionLength),
                                WSpecified = true,
                                W = ToAxis(bPosition.DirectionWidth)
                            }
                            );
                    }
                }
                else if (layer is Layer3DCyl layerCyl)
                {
                    layerCyl.Sort(analysis.Content, Layer3DCyl.SortType.DIST_CENTER);
                    foreach (Vector3D vPos in layerCyl)
                    {
                        lPlacements.Add(
                            new placement()
                            {
                                itemId = 1,
                                x = vPos.X,
                                y = vPos.Y,
                                z = vPos.Z,
                                LSpecified = false,
                                WSpecified = false
                            }
                            );
                    }
                }
                else if (layer is InterlayerPos interlayerPos)
                {
                    var interlayerProp = sol.Interlayers[interlayerPos.TypeId];
                    var bPosition = new BoxPosition(new Vector3D(
                            0.5 * (analysis.ContainerDimensions.X - interlayerProp.Length)
                            , 0.5 * (analysis.ContainerDimensions.Y - interlayerProp.Width)
                            , interlayerPos.ZLow),
                            HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                    Vector3D writtenPosition = ConvertPosition(bPosition, interlayerProp.Dimensions);
                    lPlacements.Add(
                        new placement()
                        {
                            itemId = interlayerPos.TypeId + OffsetIndexInterlayers,
                            x = writtenPosition.X,
                            y = writtenPosition.Y,
                            z = writtenPosition.Z
                        }
                        );
                }
            }
            return lPlacements.ToArray();
        }
        private HAxis ToAxis(HalfAxis.HAxis axis)
        {
            switch (axis)
            {
                case HalfAxis.HAxis.AXIS_X_N: return HAxis.XN;
                case HalfAxis.HAxis.AXIS_X_P: return HAxis.XP;
                case HalfAxis.HAxis.AXIS_Y_N: return HAxis.YN;
                case HalfAxis.HAxis.AXIS_Y_P: return HAxis.YP;
                case HalfAxis.HAxis.AXIS_Z_N: return HAxis.ZN;
                case HalfAxis.HAxis.AXIS_Z_P: return HAxis.ZP;
                default: return HAxis.XN;
            }
        }
        #endregion
        #region Data members
        private int OffsetIndexInterlayers { get; set; } = 1;
        #endregion
    }
}
