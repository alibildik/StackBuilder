using System;
using System.Collections.Generic;
using System.Text;

using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;

namespace treeDiM.StackBuilder.Engine
{
    class LayerPatternInterlocked : LayerPatternBox
    {
        public override string Name => "Interlocked";
        public override bool IsSymetric => true;
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
                , out int maxSizeXLength, out int maxSizeXWidth, out int maxSizeYLength, out int maxSizeYWidth);

            double offsetX = 0.5 * (palletLength - actualLength);
            double offsetY = 0.5 * (palletWidth - actualWidth);

            double spaceX = maxSizeXLength + maxSizeXWidth > 1 ? (actualLength - (maxSizeXLength * boxLength + maxSizeXWidth * boxWidth)) / (maxSizeXLength + maxSizeXWidth - 1) : 0.0;
            double spaceYLength = maxSizeYLength  > 1 ? (actualWidth - maxSizeYLength * boxWidth ) / (maxSizeYLength - 1) : 0.0;
            double spaceYWidth = maxSizeYWidth > 1 ? (actualWidth - maxSizeYWidth * boxLength) / (maxSizeYWidth - 1) : 0.0;

            for (int i=0; i<maxSizeXLength; ++i)
                for (int j = 0; j < maxSizeYLength; ++j)
                {
                    AddPosition(
                        layer
                        , new Vector2D(
                            offsetX + i * (boxLength + spaceX)
                            , offsetY + j * (boxWidth + spaceYLength))
                        , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                }
            for (int i = 0; i < maxSizeXWidth; ++i)
                for (int j = 0; j < maxSizeYWidth; ++j)
                    AddPosition(
                        layer
                        , new Vector2D(
                            offsetX + maxSizeXLength * (boxLength + spaceX) + i * (boxWidth + spaceX) + boxWidth
                            , offsetY + j * (boxLength + spaceYWidth))
                        , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);

            // maximum space
            layer.UpdateMaxSpace( spaceYLength, Name );
            layer.UpdateMaxSpace( spaceYWidth, Name );
            layer.UpdateMaxSpace( spaceX, Name );
        }

        public override bool GetLayerDimensions(ILayer2D layer, out double actualLength, out double actualWidth)
        {
            double palletLength = GetPalletLength(layer);
            double palletWidth = GetPalletWidth(layer);
            double boxLength = GetBoxLength(layer);
            double boxWidth = GetBoxWidth(layer);

            GetSizeXY(boxLength, boxWidth, palletLength, palletWidth
                , out int maxSizeXLength, out int maxSizeXWidth, out int maxSizeYLength, out int maxSizeYWidth);

            actualLength = maxSizeXLength * boxLength + maxSizeXWidth * boxWidth;
            actualWidth = Math.Max(maxSizeYLength * boxWidth, maxSizeYWidth * boxLength);

            return maxSizeXLength > 0
                && maxSizeYLength > 0
                && maxSizeXWidth > 0
                && maxSizeYWidth > 0;
        }

        public override int GetNumberOfVariants(Layer2DBrickImp layer) => 1;

        #region Non-Public Members

        private void GetSizeXY(double boxLength, double boxWidth, double palletLength, double palletWidth,
            out int optSizeXLength, out int optSizeXWidth, out int optSizeYLength, out int optSizeYWidth)
        {
            int optFound = 0;
            optSizeXLength = 0; optSizeXWidth = 0; optSizeYLength = 0; optSizeYWidth = 0;
            // get maximum number of box in length
            int sizeXLength = (int)Math.Floor(palletLength / boxLength);
            // make sure that we can actually add one column of turned boxes
            while (palletLength < sizeXLength * boxLength + boxWidth)
                --sizeXLength;
            while (sizeXLength >= 1)
            {
                // get number of column of turned boxes
                int sizeXWidth = (int)Math.Floor((palletLength - sizeXLength * boxLength) / boxWidth);
                // get maximum number in width
                // for boxes with length aligned with pallet length
                int sizeYLength = (int)Math.Floor(palletWidth / boxWidth);
                // for turned boxes
                int sizeYWidth = (int)Math.Floor(palletWidth / boxLength);

                int countLayer = sizeXLength * sizeYLength + sizeXWidth * sizeYWidth;

                if (countLayer > optFound)
                {
                    optFound = countLayer;
                    optSizeXLength = sizeXLength;
                    optSizeXWidth = sizeXWidth;
                    optSizeYLength = sizeYLength;
                    optSizeYWidth = sizeYWidth;
                }
                --sizeXLength;
            }
        }

        #endregion
    }
}
