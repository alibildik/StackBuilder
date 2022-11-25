using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;

namespace treeDiM.StackBuilder.Engine
{
    class LayerPatternInterlockedFilled : LayerPatternBox
    {
        public override string Name => "Interlocked Filled";
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

            GetSizeXY(boxLength, boxWidth, palletLength, palletWidth
                , out int maxSizeXLength, out int maxSizeYLength, out int maxSizeXWidth, out int maxSizeYWidth
                , out int fillSizeXLength, out int fillSizeYLength, out int fillSizeXWidth, out int fillSizeYWidth);

            double offsetX = 0.5 * (palletLength - actualLength);
            double offsetY = 0.5 * (palletWidth - actualWidth);

            double l1 = Math.Max(maxSizeXLength * boxLength, fillSizeXLength * boxWidth);
            double l2 = Math.Max(maxSizeXWidth * boxWidth, fillSizeXWidth * boxLength);
            double remainingSpaceX = actualLength - l1 - l2;
            Debug.Assert(remainingSpaceX >= 0);

            l1 += 0.5 * remainingSpaceX;
            l2 += 0.5 * remainingSpaceX;

            double spaceXLength = (l1 - maxSizeXLength * boxLength) / ((double)maxSizeXLength - 0.5);
            double spaceXWidth = (l2 - maxSizeXWidth * boxWidth) / ((double) maxSizeXWidth - 0.5);
            double spaceYLength = maxSizeYLength > 1 ? (actualWidth - maxSizeYLength * boxWidth - fillSizeYLength * boxLength) / (maxSizeYLength + fillSizeYLength - 1) : 0.0;
            double spaceYWidth = maxSizeYWidth > 1 ? (actualWidth - maxSizeYWidth * boxLength - fillSizeYWidth * boxWidth) / (maxSizeYWidth + fillSizeYWidth - 1) : 0.0;

            if (1 == maxSizeYLength % 2)
            {
                // LENGTH
                for (int i = 0; i < maxSizeXLength; ++i)
                    for (int j = 0; j < maxSizeYLength; ++j)
                    {
                        AddPosition(
                            layer
                            , new Vector2D(
                                offsetX + i * (boxLength + spaceXLength)
                                , offsetY + j * (boxWidth + spaceYLength))
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                    }
            }
            else
            {
                // LENGTH
                for (int i = 0; i < maxSizeXLength; ++i)
                    for (int j = 0; j < maxSizeYLength / 2; ++j)
                    {
                        AddPosition(
                            layer
                            , new Vector2D(
                                offsetX + i * (boxLength + spaceXLength)
                                , offsetY + j * (boxWidth + spaceYLength))
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                    }
                for (int i = 0; i < maxSizeXLength; ++i)
                    for (int j = maxSizeYLength / 2; j < maxSizeYLength; ++j)
                    {
                        AddPosition(
                            layer
                            , new Vector2D(
                                offsetX + i * (boxLength + spaceXLength)
                                , offsetY + j * (boxWidth + spaceYLength) + (fillSizeYLength > 0 ? fillSizeYLength * (spaceYLength + boxLength) : 0.0))
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                    }

                double spaceXFill = fillSizeXLength > 1 ? (l1 - fillSizeXLength * boxWidth) / ((double)fillSizeXLength - 0.5) : 0;
                layer.UpdateMaxSpace(spaceXFill, Name);
                for (int i = 0; i < fillSizeXLength; ++i)
                    for (int j = 0; j < fillSizeYLength; ++j)
                    {
                        AddPosition(
                            layer
                            , new Vector2D(
                                offsetX + boxWidth + i * (boxWidth + spaceXFill)
                                , offsetY + (maxSizeYLength / 2) * (boxWidth + spaceYLength) + j * boxLength)
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N
                            );
                    }
            }
            // WIDTH
            if (1 == maxSizeYWidth % 2)
            {
                for (int i = 0; i < maxSizeXWidth; ++i)
                    for (int j = 0; j < maxSizeYWidth; ++j)
                    {
                        AddPosition(
                            layer
                            , new Vector2D(
                                offsetX + maxSizeXLength * (boxLength + spaceXLength) + i * (boxWidth + spaceXWidth) + boxWidth
                                , offsetY + j * (boxLength + spaceYWidth))
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
                    }
            }
            else
            {
                for (int i = 0; i < maxSizeXWidth; ++i)
                    for (int j = 0; j < maxSizeYWidth / 2; ++j)
                    {
                        AddPosition(
                            layer
                            , new Vector2D(
                                offsetX + actualLength - (i) * (boxWidth + spaceXWidth) /*- boxWidth*/
                                , offsetY + j * (boxLength + spaceYWidth))
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
                    }

                for (int i = 0; i < maxSizeXWidth; ++i)
                    for (int j = maxSizeYWidth / 2; j < maxSizeYWidth; ++j)
                    {
                        AddPosition(
                            layer
                            , new Vector2D(
                                offsetX + actualLength - (i) * (boxWidth + spaceXWidth) /*- boxWidth*/
                                , offsetY + j * (boxLength + spaceYWidth) + (fillSizeYWidth > 0 ? fillSizeYWidth * (boxWidth + spaceYWidth) : 0.0))
                            , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
                    }

                double spaceXFill = fillSizeXWidth > 1 ? (l2 - fillSizeXWidth * boxLength) / ((double)fillSizeXWidth - 0.5) : 0.0;
                layer.UpdateMaxSpace( spaceXFill, Name );
                for (int i = 0; i < fillSizeXWidth; ++i)
                    for (int j = 0; j < fillSizeYWidth; ++j)
                    {
                        AddPosition(
                            layer
                            , new Vector2D(
                                offsetX + actualLength - (i+1) * (boxLength + spaceXFill)
                                , offsetY + (maxSizeYWidth / 2) * (boxLength + spaceYWidth) + j * boxWidth)
                            , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P
                            );   
                    }
            }
            // maximum space
            layer.UpdateMaxSpace( spaceYLength, Name );
            layer.UpdateMaxSpace( spaceYWidth, Name );
        }
        public override bool GetLayerDimensions(ILayer2D layer, out double actualLength, out double actualWidth)
        {
            double palletLength = GetPalletLength(layer);
            double palletWidth = GetPalletWidth(layer);
            double boxLength = GetBoxLength(layer);
            double boxWidth = GetBoxWidth(layer);

            GetSizeXY(boxLength, boxWidth, palletLength, palletWidth
                , out int maxSizeXLength, out int maxSizeYLength, out int maxSizeXWidth, out int maxSizeYWidth
                , out int fillSizeXLength, out int fillSizeYLength, out int fillSizeXWidth, out int fillSizeYWidth);

            actualLength = Math.Max(maxSizeXLength * boxLength, fillSizeXLength * boxWidth) + Math.Max(maxSizeXWidth * boxWidth, fillSizeXWidth * boxLength);
            actualWidth = Math.Max(maxSizeYLength * boxWidth + fillSizeYLength * boxLength, maxSizeYWidth * boxLength + fillSizeYWidth * boxWidth);

            //Debug.Assert(actualLength <= palletLength);
            //Debug.Assert(actualWidth <= palletWidth);

            return maxSizeXLength > 0 && maxSizeYLength > 0
                && maxSizeXWidth > 0 && maxSizeYWidth > 0
                && (
                    ((maxSizeYLength % 2 == 0) && (fillSizeXLength * fillSizeYLength > 0))
                    || ((maxSizeYWidth % 2 == 0) && (fillSizeXWidth * fillSizeYWidth > 0))
                    );
        }
        #region Non-Public Members
        void GetSizeXY(double boxLength, double boxWidth, double palletLength, double palletWidth,
            out int optSizeXLength,  out int optSizeYLength, out int optSizeXWidth, out int optSizeYWidth,
            out int optFillSizeXLength, out int optFillSizeYLength, out int optFillSizeXWidth, out int optFillSizeYWidth)
        {
            /*

            ┌─────────────┬─────────┐
            |             |         |
            |             |         |
   YLength  |     1       |    2    | YWidth
            |             |         |
            |             |         |
            └─────────────┴─────────┘
                X1            X2
            */
            int optFound = 0;
            optSizeXLength = 0; optSizeXWidth = 0;
            optSizeYLength = 0; optSizeYWidth = 0;
            optFillSizeXLength = 0; optFillSizeYLength = 0;
            optFillSizeXWidth = 0; optFillSizeYWidth = 0;

            // get maximum number of box in length
            int sizeX1 = (int)Math.Floor(palletLength / boxLength);
            // make sure that we can actually add one column of turned boxes
            while (palletLength < sizeX1 * boxLength + boxWidth)
                --sizeX1;
            while (sizeX1 >= 1)
            {
                // get number of column of turned boxes
                int sizeX2 = (int)Math.Floor((palletLength - sizeX1 * boxLength) / boxWidth);
                // get maximum number in width
                // for boxes with length aligned with pallet length
                int sizeY1 = (int)Math.Floor(palletWidth / boxWidth);
                // for turned boxes
                int sizeY2 = (int)Math.Floor(palletWidth / boxLength);
                while (sizeY2 > 0)
                {
                    // ensure symetry
                    if (sizeY2 % 2 != 0)
                        --sizeY2;

                    double spaceX1 = palletLength - sizeX2 * boxWidth;
                    int fillSizeX1 = (int)Math.Floor(spaceX1 / boxWidth);
                    double spaceY1 = palletWidth - sizeY1 * boxWidth;
                    int fillSizeY1 = (int)Math.Floor(spaceY1 / boxLength);

                    if (fillSizeX1 * fillSizeY1 == 0) { fillSizeX1 = 0;  fillSizeY1 = 0; }

                    double spaceX2 = palletLength - sizeX1 * boxLength;
                    int fillSizeX2 = (int)Math.Floor(spaceX2 / boxLength);
                    double spaceY2 = palletWidth - sizeY2 * boxLength;
                    int fillSizeY2 = (int)Math.Floor(spaceY2 / boxWidth);

                    if (fillSizeX2 * fillSizeY2 == 0) { fillSizeX2 = 0; fillSizeY2 = 0; }

                    int countLayer = sizeX1 * sizeY1 + sizeX2 * sizeY2;

                    double actualLength = Math.Max(sizeX1 * boxLength, fillSizeX1 * boxWidth) + Math.Max(sizeX2 * boxWidth, fillSizeX2 * boxLength);
                    double actualWidth = Math.Max(sizeY1 * boxWidth + fillSizeY1 * boxLength, sizeY2 * boxLength + fillSizeY2 * boxWidth);

                    if (countLayer > optFound && actualLength <= palletLength && actualWidth <= palletWidth)
                    {
                        optFound = countLayer;
                        optSizeXLength = sizeX1;
                        optSizeXWidth = sizeX2;
                        optSizeYLength = sizeY1;
                        optSizeYWidth = sizeY2;

                        bool filledLength = (fillSizeX1 * fillSizeY1 > 0);
                        optFillSizeXLength = filledLength ? fillSizeX1 : 0;
                        optFillSizeYLength = filledLength ? fillSizeY1 : 0;
                        bool filledWidth = (fillSizeX2 * fillSizeY2 > 0);
                        optFillSizeXWidth = filledWidth ? fillSizeX2 : 0;
                        optFillSizeYWidth = filledWidth ? fillSizeY2 : 0;
                    }
                    --sizeY2;
                }
                // decrement
                --sizeX1;
            }
        }
        #endregion
    }
}
