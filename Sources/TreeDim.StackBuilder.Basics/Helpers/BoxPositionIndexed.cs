#region Using directives
using System.Collections.Generic;
using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class BoxPositionIndexed
    {
        public BoxPositionIndexed(BoxPosition boxPosition, int index)
        {
            BPos = boxPosition;
            Index = index;
        }
        public BoxPositionIndexed(BoxPositionIndexed boxPosition)
        {
            BPos = boxPosition.BPos;
            Index = boxPosition.Index;

        }
        public BoxPositionIndexed(Vector3D vPos, HalfAxis.HAxis axisLength, HalfAxis.HAxis axisWidth, int index)
        {
            BPos = new BoxPosition(vPos, axisLength, axisWidth);
            Index = index;
        }
        public BoxPositionIndexed Adjusted(Vector3D dimensions)
        {
            var boxPosTemp = new BoxPositionIndexed(BPos.Position, BPos.DirectionLength, BPos.DirectionWidth, Index);
            // reverse if oriented to Z- (AXIS_Z_N)
            if (BPos.DirectionHeight == HalfAxis.HAxis.AXIS_Z_N)
            {
                if (BPos.DirectionLength == HalfAxis.HAxis.AXIS_X_P)
                    boxPosTemp.BPos = new BoxPosition(BPos.Position + new Vector3D(0.0, -dimensions.Y, -dimensions.Z), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                else if (BPos.DirectionLength == HalfAxis.HAxis.AXIS_Y_P)
                    boxPosTemp.BPos = new BoxPosition(BPos.Position + new Vector3D(dimensions.Y, 0.0, -dimensions.Z), HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
                else if (BPos.DirectionLength == HalfAxis.HAxis.AXIS_X_N)
                    boxPosTemp.BPos = new BoxPosition(BPos.Position + new Vector3D(-dimensions.X, 0.0, -dimensions.Z), HalfAxis.HAxis.AXIS_X_P, BPos.DirectionWidth);
                else if (BPos.DirectionLength == HalfAxis.HAxis.AXIS_Y_N)
                    boxPosTemp.BPos = new BoxPosition(BPos.Position + new Vector3D(-dimensions.Y, 0.0, -dimensions.Z), HalfAxis.HAxis.AXIS_Y_N, HalfAxis.HAxis.AXIS_X_P);
            }
            return boxPosTemp;
        }
        public override string ToString() => $"{BPos.Position} | ({HalfAxis.ToString(BPos.DirectionLength)},{HalfAxis.ToString(BPos.DirectionWidth)}) | {Index}";

        #region Static methods
        public static BoxPositionIndexed Parse(string s)
        {
            string[] sArray = s.Split('|');
            var v = Vector3D.Parse(sArray[0]);
            string sOrientation = sArray[1];
            sOrientation = sOrientation.Trim();
            sOrientation = sOrientation.TrimStart('(');
            sOrientation = sOrientation.TrimEnd(')');
            string[] vOrientation = sOrientation.Split(',');
            var index = int.Parse(sArray[2]);
            return new BoxPositionIndexed(v, HalfAxis.Parse(vOrientation[0]), HalfAxis.Parse(vOrientation[1]), index);
        }
        public static List<BoxPositionIndexed> FromListBoxPosition(List<BoxPosition> boxPositions)
        {
            var listBoxPositionIndexed = new List<BoxPositionIndexed>();
            int counter = 0;
            foreach (var bp in boxPositions)
                listBoxPositionIndexed.Add(new BoxPositionIndexed(bp, ++counter));
            return listBoxPositionIndexed;
        }
        public static List<BoxPosition> ToListBoxPosition(List<BoxPositionIndexed> listboxPositionIndexed) => listboxPositionIndexed.ConvertAll(bpi => bpi.BPos);
        public static void Sort(ref List<BoxPositionIndexed> list)
        {
            list.Sort(
                delegate (BoxPositionIndexed bp1, BoxPositionIndexed bp2)
                {
                    if (bp1.Index > bp2.Index) return 1;
                    else if (bp1.Index == bp2.Index) return 0;
                    else return -1;
                });
        }
        public static void ReduceListBoxPositionIndexed(List<BoxPositionIndexed> listBPI, out List<BoxPositionIndexed> listBPIReduced, out Dictionary<int, int> dictIndexNumber)
        {
            listBPIReduced = new List<BoxPositionIndexed>();
            dictIndexNumber = new Dictionary<int, int>();

            foreach (var bpi in listBPI)
            {
                if (dictIndexNumber.ContainsKey(bpi.Index))
                    dictIndexNumber[bpi.Index] += 1;
                else
                {
                    listBPIReduced.Add(bpi);
                    dictIndexNumber.Add(bpi.Index, 1);
                }
            }
            Sort(ref listBPIReduced);
        }        
        public static List<BoxPositionIndexed> MirrorX(List<BoxPositionIndexed> listIn, Vector3D containerDim, Vector3D dimensions) => ApplyTransformation(listIn, containerDim, dimensions, 0);
        public static List<BoxPositionIndexed> MirrorY(List<BoxPositionIndexed> listIn, Vector3D containerDim, Vector3D dimensions) => ApplyTransformation(listIn, containerDim, dimensions, 1);
        public static List<BoxPositionIndexed> Rotate180(List<BoxPositionIndexed> listIn, Vector3D containerDim, Vector3D dimensions) => ApplyTransformation(listIn, containerDim, dimensions, 2);
        public static List<BoxPositionIndexed> ApplyTransformation(List<BoxPositionIndexed> listIn, Vector3D containerDims, Vector3D dimensions, int mode)
        {
            Matrix4D matRot = Matrix4D.Identity;
            Vector3D vTranslation = Vector3D.Zero;

            switch (mode)
            {
                case 0:
                    matRot = new Matrix4D(
                        1.0, 0.0, 0.0, 0.0
                        , 0.0, -1.0, 0.0, 0.0
                        , 0.0, 0.0, 1.0, 0.0
                        , 0.0, 0.0, 0.0, 1.0
                        );
                    vTranslation = new Vector3D(0.0, containerDims.Y, 0.0);
                    break;
                case 1:
                    matRot = new Matrix4D(
                        -1.0, 0.0, 0.0, 0.0
                        , 0.0, 1.0, 0.0, 0.0
                        , 0.0, 0.0, 1.0, 0.0
                        , 0.0, 0.0, 0.0, 1.0
                        );
                    vTranslation = new Vector3D(containerDims.X, 0.0, 0.0);
                    break;
                case 2:
                    matRot = new Matrix4D(
                        -1.0, 0.0, 0.0, 0.0
                        , 0.0, -1.0, 0.0, 0.0
                        , 0.0, 0.0, 1.0, 0.0
                        , 0.0, 0.0, 0.0, 1.0
                        );
                    vTranslation = new Vector3D(containerDims.X, containerDims.Y, 0.0);
                    break;
                default:
                    break;
            }
            var listOut = new List<BoxPositionIndexed>();
            foreach (var lint in listIn)
            {
                var layerPosTemp = new BoxPositionIndexed(lint);
                listOut.Add(ApplyReflection(layerPosTemp, matRot, vTranslation, dimensions, mode)); 
            }
            return listOut;
        }
        private static BoxPositionIndexed ApplyReflection(BoxPositionIndexed bPosition, Matrix4D matRot, Vector3D vTranslation, Vector3D dimensions, int mode)
        {
            Transform3D transfRot = new Transform3D(matRot);
            HalfAxis.HAxis axisLength = HalfAxis.ToHalfAxis(transfRot.transform(HalfAxis.ToVector3D(bPosition.BPos.DirectionLength)));
            HalfAxis.HAxis axisWidth = HalfAxis.ToHalfAxis(transfRot.transform(HalfAxis.ToVector3D(bPosition.BPos.DirectionWidth)));
            matRot.M14 = vTranslation[0];
            matRot.M24 = vTranslation[1];
            matRot.M34 = vTranslation[2];
            Transform3D transfRotTranslation = new Transform3D(matRot);
            Vector3D transPos = transfRotTranslation.transform(
                new Vector3D(bPosition.BPos.Position.X, bPosition.BPos.Position.Y, bPosition.BPos.Position.Z)
                );
            if (mode == 0 || mode == 1)
                transPos -= dimensions.Z * Vector3D.CrossProduct(HalfAxis.ToVector3D(axisLength), HalfAxis.ToVector3D(axisWidth));
            else if (mode == 2)
                transPos += dimensions.Z * Vector3D.CrossProduct(HalfAxis.ToVector3D(axisLength), HalfAxis.ToVector3D(axisWidth));
            var bpi = new BoxPositionIndexed(transPos, axisLength, axisWidth, bPosition.Index);
            return bpi.Adjusted(dimensions);
        }
        #endregion
        #region Data members
        public BoxPosition BPos { get; set; }
        public int Index { get; set; }
        #endregion
    }
    public class BPosIndexedComparer : IComparer<BoxPositionIndexed>
    {
        int IComparer<BoxPositionIndexed>.Compare(BoxPositionIndexed bpi1, BoxPositionIndexed bpi2)
        {
            if (bpi1.Index > bpi2.Index) return 1;
            else if (bpi1.Index == bpi2.Index) return 0;
            else return -1;
        }
    }
}
