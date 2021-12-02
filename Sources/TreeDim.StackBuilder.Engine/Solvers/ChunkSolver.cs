#region Using directives
using System;
using System.Collections.Generic;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Engine
{
    public class Chunk
    {
        public Vector2D Dimensions {get; set; }
        public int Remaining { get; set; }
        public List<BoxPosition> BoxPositions { get; set; }
        public string LayerDescriptor { get; set; }

        public override string ToString()
        {
            return $"LayerDesc={LayerDescriptor} - Dimensions = {Dimensions} - BoxPositions = {BoxPositions.Count} - Remaining = {Remaining}";
        }
    }

    public class ChunkComparer : IComparer<Chunk>
    {
        public int Compare(Chunk chunk0, Chunk chunk1)
        {
            if (chunk0.Dimensions.X < chunk1.Dimensions.X) return -1;
            else if (chunk0.Dimensions.X == chunk1.Dimensions.X) return 0;
            else return 1;
        }
    }

    public class ChunkSolver
    {
        public static List<Chunk> BuildChunks(Vector2D dimContent, Vector2D dimContainer, double forcedSpace, int number)
        {
            var chunks = new List<Chunk>();
            Vector3D dimContent3D = new Vector3D(dimContent.X, dimContent.Y, 0.0);

            foreach (var pattern in LayerPatternBox.All)
            {
                for (int iSwapped = 0; iSwapped < 2; ++iSwapped)
                {
                    bool swapped = (iSwapped == 1);
                    try
                    {
                        Vector2D dimChunk = Vector2D.Zero;
                        int countMin = 0, countMax = 0;
                        if (FindMinLengthForCount(dimContent3D, dimContainer.Y, pattern, swapped, forcedSpace, number,
                            ref dimChunk, ref countMin, ref countMax))
                        {
                            if (GetBoxPositions(dimContent3D, dimChunk, pattern, swapped, forcedSpace, out List<BoxPosition> boxPositions))
                                chunks.Add(new Chunk() { LayerDescriptor = pattern.Name, Dimensions = dimChunk, BoxPositions = boxPositions, Remaining = 0 });
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.ErrorFormat("Pattern: {0} Swapped: {1} Message: {2}"
                            , pattern.Name
                            , iSwapped == 1 ? "True" : "False"
                            , ex.Message
                            );
                    }
                }
            }
            chunks.Sort(new ChunkComparer());
            return chunks;
        }

        internal static bool FindMinLengthForCount(Vector3D dimContent, double width, LayerPatternBox pattern, bool swapped, double forcedSpace, int count
            , ref Vector2D dimChunk, ref int countMin, ref int countMax)
        {
            double lengthMin = 0.0, lengthMax = count * ( Math.Max(dimContent.X, dimContent.Y) + forcedSpace);
            while (lengthMax - lengthMin > MinDiffDichotomy)
            {
                double lengthCurrent = 0.5 * (lengthMin + lengthMax);
                int iCount = 0;
                Vector2D actualDimensions = Vector2D.Zero;
                if (!GetCount(dimContent, lengthCurrent, width
                    , pattern, swapped, forcedSpace, ref iCount, ref actualDimensions))
                    return false;

                if (iCount < count)
                {
                    lengthMin = lengthCurrent;
                    countMin = iCount;
                }
                else if (iCount >= count)
                {
                    lengthMax = lengthCurrent;
                    countMax = iCount;
                    dimChunk = new Vector2D(swapped ? actualDimensions.Y : actualDimensions.X, swapped ? actualDimensions.X : actualDimensions.Y);
                }
            }
            return true;
        }

        private static bool GetCount(Vector3D dimContent, double length, double width, LayerPattern pattern, bool swapped, double forcedSpace
            , ref int iCount, ref Vector2D actualDimensions)
        {
            // instantiate layer 
            var layer = new Layer2DBrickImp(new Vector3D(dimContent.X, dimContent.Y, 0.0), Vector3D.Zero, new Vector2D(length, width), pattern.Name, HalfAxis.HAxis.AXIS_Z_P, swapped)
            {
                ForcedSpace = forcedSpace
            };
            if (!pattern.GetLayerDimensionsChecked(layer, out double actualLength, out double actualWidth))
                return false;
            pattern.GenerateLayer(layer, actualLength, actualWidth);
            iCount = layer.Count;
            actualDimensions = new Vector2D(actualLength, actualWidth);
            return true;
        }
        private static bool GetBoxPositions(Vector3D dimContent, Vector2D dimContainer, LayerPattern pattern, bool swapped, double forcedSpace
            , out List<BoxPosition> boxPositions)
        {
            boxPositions = null;
           // instantiate layer 
            var layer = new Layer2DBrickImp(new Vector3D(dimContent.X, dimContent.Y, 0.0), Vector3D.Zero, dimContainer, pattern.Name, HalfAxis.HAxis.AXIS_Z_P, swapped)
            {
                ForcedSpace = forcedSpace
            };
            if (!pattern.GetLayerDimensionsChecked(layer, out double actualLength, out double actualWidth))
                return false;
            pattern.GenerateLayer(layer, actualLength, actualWidth);

            boxPositions = layer.Positions;
            return true;        
        }

        private static double MinDiffDichotomy = 50.0;
        private static ILog _log = LogManager.GetLogger(typeof(ChunkSolver));
    }
}
