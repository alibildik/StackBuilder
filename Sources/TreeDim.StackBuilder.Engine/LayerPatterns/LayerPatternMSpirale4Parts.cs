using System;
using System.Diagnostics;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;

namespace treeDiM.StackBuilder.Engine
{
    internal class LayerPatternMSpirale4Parts : LayerPatternBox
    {
        public override string Name => "M Spirale 4 parts";
        public override int GetNumberOfVariants(Layer2DBrickImp layer) => 1;
        public override bool IsSymetric => false;
        public override bool CanBeSwapped => true;
        public override bool CanBeInverted => true;
        public override void GenerateLayer(ILayer2D layer, double actualLength, double actualWidth)
        {
            layer.Clear();
            double palletLength = GetPalletLength(layer);
            double palletWidth = GetPalletWidth(layer);
            double boxLength = GetBoxLength(layer);
            double boxWidth = GetBoxWidth(layer);

            double spiraleLength = boxLength + boxWidth;

            GetSizeXY(boxLength, boxWidth, palletLength, palletWidth
                , out int noSpiraleX, out int noSpiraleY
                , out int noArea2X, out int noArea2Y, out bool area2LengthAligned
                , out int noArea3X, out int noArea3Y, out bool area3LengthAligned
                , out bool area2First);

            Vector2D offset = GetOffset(layer, actualLength, actualWidth);

            // spirale
            for (int i = 0; i < noSpiraleX; ++i)
                for (int j = 0; j < noSpiraleY; ++j)
                {
                    double xBase = i * spiraleLength;
                    double yBase = j * spiraleLength;

                    AddPosition(
                        layer
                        , new Vector2D(xBase, yBase) + offset
                        , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                    AddPosition(
                        layer
                        , new Vector2D(xBase + boxLength + boxWidth, yBase) + offset
                        , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
                    AddPosition(
                        layer
                        , new Vector2D(xBase + boxLength + boxWidth, yBase + boxLength + boxWidth) + offset
                        , HalfAxis.HAxis.AXIS_X_N, HalfAxis.HAxis.AXIS_Y_N);
                    AddPosition(
                        layer
                        , new Vector2D(xBase, yBase + boxLength + boxWidth) + offset
                        , HalfAxis.HAxis.AXIS_Y_N, HalfAxis.HAxis.AXIS_X_P);
                }
            layer.UpdateMaxSpace(Math.Abs(boxLength - boxWidth), Name);

            if (area2First)
            {
                // area 2
                double width = Math.Max(noArea2Y * (area2LengthAligned ? boxWidth : boxLength), noSpiraleY * spiraleLength + noArea3Y * (area3LengthAligned ? boxWidth : boxLength));
                double ySpace = 0.0;
                if (noArea2Y > 1)
                    ySpace = Math.Floor((width - noArea2Y * (area2LengthAligned ? boxWidth : boxLength)) / (noArea2Y - 1));

                double xStart = Math.Max(
                    noSpiraleX * spiraleLength
                    , noArea3X * (area3LengthAligned ? boxLength : boxWidth));

                GenerateRectanglePositions(layer, offset, boxLength, boxWidth
                    , noArea2X, noArea2Y
                    , xStart, 0.0, 0.0, ySpace, area2LengthAligned);

                // area 3
                GenerateRectanglePositions(layer, offset, boxLength, boxWidth, noArea3X, noArea3Y
                    , 0.0, noSpiraleY * spiraleLength, 0.0, 0.0, area3LengthAligned);
            }
            else
            {
                // area 3
                double length = Math.Max(noArea3X * (area3LengthAligned ? boxLength : boxWidth), noSpiraleX * spiraleLength + noArea2X * (area2LengthAligned ? boxLength : boxWidth));
                double xSpace = 0.0;
                if (noArea3X > 1)
                    xSpace = Math.Floor((length - noArea3X * (area3LengthAligned ? boxLength : boxWidth)) / (noArea3X - 1));

                double yStart = Math.Max(
                    noSpiraleY * spiraleLength
                    , noArea2Y * (area2LengthAligned ? boxWidth : boxLength));

                GenerateRectanglePositions(layer, offset, boxLength, boxWidth, noArea3X, noArea3Y
                    , 0.0, yStart, xSpace, 0.0, area3LengthAligned);

                // area 2
                GenerateRectanglePositions(layer, offset, boxLength, boxWidth, noArea2X, noArea2Y
                    , noSpiraleX * spiraleLength, 0.0, 0.0, 0.0, area2LengthAligned); 
            }
        }
        public override bool GetLayerDimensions(ILayer2D layer, out double layerLength, out double layerWidth)
        {
            double palletLength = GetPalletLength(layer);
            double palletWidth = GetPalletWidth(layer);
            double boxLength = GetBoxLength(layer);
            double boxWidth = GetBoxWidth(layer);

            GetSizeXY(boxLength, boxWidth, palletLength, palletWidth
                , out int noSpiraleX, out int noSpiraleY
                , out int noArea2X, out int noArea2Y, out bool area2LengthAligned
                , out int noArea3X, out int noArea3Y, out bool area3LengthAligned
                , out bool area2First);

            double spiraleLength = boxLength + boxWidth;

            if (area2First)
            {
                layerLength = Math.Max(noSpiraleX * spiraleLength, noArea3X * (area3LengthAligned ? boxLength : boxWidth)) + noArea2X * (area2LengthAligned ? boxLength : boxWidth);
                layerWidth = Math.Max(noSpiraleY * spiraleLength + noArea3Y * (area3LengthAligned ? boxWidth : boxLength), noArea2Y * (area2LengthAligned ? boxWidth : boxLength));
            }
            else
            {
                layerLength = Math.Max(noSpiraleX * spiraleLength + noArea2X * (area2LengthAligned ? boxLength : boxWidth), noArea3X * (area3LengthAligned ? boxLength : boxWidth));
                layerWidth = Math.Max(noSpiraleY * spiraleLength, noArea2Y * (area2LengthAligned ? boxWidth : boxLength)) + noArea3Y * (area3LengthAligned ? boxWidth : boxLength);
            }

            Debug.Assert(layerLength <= palletLength);
            Debug.Assert(layerWidth <= palletWidth);

            return noSpiraleX * noSpiraleY > 0;
        }

        #region Non-Public members
        //
        // |---------------------
        // |        3        |   |        
        // |-----------------|   |
        // |                 |   |
        // |                 |   |
        // |        1        | 2 |
        // |                 |   |
        // |                 |   |
        // |----------------------
        //
        // Area 1 spirales (noSpiraleX * noSpiraleY)
        // Area 2 Remaining (noArea2X * noArea2Y) | orientation (area2LengthAligned)
        // Area 3 Remaining (noArea3X * noArea3Y) | orientation (area3LengthAligned) 
        protected int GetSizeXY(double boxLength, double boxWidth, double palletLength, double palletWidth
            , out int noSpiraleX, out int noSpiraleY
            , out int noArea2X, out int noArea2Y, out bool area2LengthAligned
            , out int noArea3X, out int noArea3Y, out bool area3LengthAligned
            , out bool area2First
            )
        {
            double spiraleLength = boxLength + boxWidth;
            noSpiraleX = (int)Math.Floor(palletLength / spiraleLength);
            noSpiraleY = (int)Math.Floor(palletWidth / spiraleLength);

            double area2Length = palletLength - noSpiraleX * spiraleLength;
            double area3Width = palletWidth - noSpiraleY * spiraleLength;

            GetBestRectangle(boxLength, boxWidth, area2Length, palletWidth
                , out int noArea2X_2First, out int noArea2Y_2First, out bool area2LengthAligned_2First);
            GetBestRectangle(boxLength, boxWidth, palletLength - noArea2X_2First * (area2LengthAligned_2First ? boxLength : boxWidth), area3Width
                , out int noArea3X_2First, out int noArea3Y_2First, out bool area3LengthAligned_2First);

            GetBestRectangle(boxLength, boxWidth, palletLength, area3Width
                , out int noArea3X_3First, out int noArea3Y_3First, out bool area3LengthAligned_3First);
            GetBestRectangle(boxLength, boxWidth, area2Length, palletWidth - noArea3Y_3First * (area3LengthAligned_3First ? boxWidth : boxLength)
                , out int noArea2X_3First, out int noArea2Y_3First, out bool area2LengthAligned_3First);

            area2First = noArea2X_2First * noArea2Y_2First + noArea3X_2First * noArea3Y_2First >= noArea2X_3First * noArea2Y_3First + noArea3X_3First * noArea3Y_3First;

            noArea2X = area2First ? noArea2X_2First : noArea2X_3First;
            noArea2Y = area2First ? noArea2Y_2First : noArea2Y_3First;
            noArea3X = area2First ? noArea3X_2First : noArea3X_3First;
            noArea3Y = area2First ? noArea3Y_2First : noArea3Y_3First;
            area2LengthAligned = area2First ? area2LengthAligned_2First : area2LengthAligned_3First;
            area3LengthAligned = area2First ? area3LengthAligned_2First : area3LengthAligned_3First;;

            return noSpiraleX * noSpiraleY * 4 + noArea2X * noArea2Y + noArea2X * noArea2Y;
        }
        protected int GetBestRectangle(double boxLength, double boxWidth, double rectLength, double rectWidth
            , out int noX, out int noY, out bool lengthAligned)
        {
            int noXLengthAligned = (int)Math.Floor(rectLength / boxLength);
            int noYLengthAligned = (int)Math.Floor(rectWidth / boxWidth);

            int noXWidthAligned = (int)Math.Floor(rectLength / boxWidth);
            int noYWidthAligned = (int)Math.Floor(rectWidth / boxLength);

            lengthAligned = noXLengthAligned * noYLengthAligned >= noXWidthAligned * noYWidthAligned;
            noX = lengthAligned ? noXLengthAligned : noXWidthAligned;
            noY = lengthAligned ? noYLengthAligned : noYWidthAligned;

            return noX * noY;
        }

        protected void GenerateRectanglePositions(ILayer2D layer, Vector2D offset
            , double boxLength, double boxWidth
            , int noX, int noY
            , double startX, double startY
            , double spaceX, double spaceY
            , bool lengthAligned)
        {
            double length = lengthAligned ? boxLength : boxWidth;
            double width = lengthAligned ? boxWidth : boxLength;
            for (int i = 0; i < noX; ++i)
                for (int j = 0; j < noY; ++j)
                    AddPosition(layer
                        , new Vector2D(
                            startX + (lengthAligned ? 0.0 : boxWidth) + i * (length + spaceX)
                            , startY + j * (width + spaceY)
                            ) + offset
                        , lengthAligned ? HalfAxis.HAxis.AXIS_X_P : HalfAxis.HAxis.AXIS_Y_P
                        , lengthAligned ? HalfAxis.HAxis.AXIS_Y_P : HalfAxis.HAxis.AXIS_X_N);
        }
        #endregion
    }
}
