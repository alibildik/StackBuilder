
using System;
using treeDiM.StackBuilder.Basics;

using Sharp3D.Math.Core;

namespace treeDiM.StackBuilder.Engine.LayerPatterns
{
    internal class LayerPatternPinWheel : LayerPatternBox
    {
        public override string Name => "Pin wheel";
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

            GetSizeXY(boxLength, boxWidth, palletLength, palletWidth,
                out double wheelSize, out int sizeLength, out int sizeWidth, out int i1, out int j1, out int i2, out int j2);

            double offsetX = 0.5 * (palletLength - actualLength);
            double offsetY = 0.5 * (palletWidth - actualWidth);
 
            for (int i = 0; i < sizeLength; i++)
                for (int j = 0; j < sizeWidth; j++)
                {
                    AddPosition(
                        layer
                        , new Vector2D(offsetX + i * boxLength, offsetY + j * boxWidth)
                        , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P
                        );
                    AddPosition(
                        layer
                        , new Vector2D(offsetX + wheelSize - j * boxWidth, offsetY + i * boxLength)
                        , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N
                        );
                    AddPosition(
                        layer
                        , new Vector2D(offsetX + wheelSize - i * boxLength, offsetY + wheelSize - j * boxWidth)
                        , HalfAxis.HAxis.AXIS_X_N, HalfAxis.HAxis.AXIS_Y_N
                        );
                    AddPosition(
                        layer
                        , new Vector2D(offsetX + j * boxWidth, offsetY + wheelSize - i * boxLength)
                        , HalfAxis.HAxis.AXIS_Y_N, HalfAxis.HAxis.AXIS_X_P
                        );
                }
            for (int i = 0; i < i1; ++i)
                for (int j = 0; j < j1; ++j)
                    AddPosition(
                        layer
                        , new Vector2D(offsetX + wheelSize + i * boxLength, offsetY + j * boxWidth)
                        , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P
                        );

            for (int i = 0; i < i2; ++i)
                for (int j = 0; j < j2; ++j)
                    AddPosition(
                        layer
                        , new Vector2D()
                        , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
        }
        public override bool GetLayerDimensions(ILayer2D layer, out double actualLength, out double actualWidth)
        {
            double palletLength = GetPalletLength(layer);
            double palletWidth = GetPalletWidth(layer);
            double boxLength = GetBoxLength(layer);
            double boxWidth = GetBoxWidth(layer);

            GetSizeXY(boxLength, boxWidth, palletLength, palletWidth
                , out double wheelLength
                , out int sizeLength, out int sizeWidth
                , out int i1, out int j1, out int i2, out int j2);

            actualLength = wheelLength + i1 * boxLength + i2 * boxWidth;
            actualWidth = Math.Max(wheelLength, Math.Max(j1 * boxWidth, j2 * boxLength));

            return sizeLength > 0 && sizeWidth > 0;
        }

        public override int GetNumberOfVariants(Layer2DBrickImp layer) => 1;

        private void GetSizeXY(double boxLength, double boxWidth, double palletLength, double palletWidth,
            out double wheelSize, out int sizeLength, out int sizeWidth, out int i1, out int j1, out int i2, out int j2)
        {
            sizeLength = 0; sizeWidth = 0; i1 = j1 = 0; i2 = j2 = 0;
            wheelSize = 0.0;
            double smallestDist = Math.Min(palletLength, palletWidth);
            double largestDist = Math.Max(palletLength, palletWidth);

            // can a single wheel fit ?
            if (boxLength + boxWidth > smallestDist) return;

            int iTmpWidth = (int)Math.Floor((smallestDist - boxLength) / boxWidth);

            // wheel
            for (int i = iTmpWidth; i >= 1; --i)
            { 
                int iTmpLength = (int)Math.Floor((smallestDist - i * boxWidth) / boxLength);
                if (iTmpLength * iTmpWidth >= sizeLength * sizeWidth)
                {
                    sizeLength = iTmpLength; sizeWidth = iTmpWidth;
                    wheelSize = sizeLength * boxLength + sizeWidth * boxWidth;
                }
            }

            if (Math.Abs(palletLength - largestDist) < 1.0E-03)
            {
                double rectLength = palletLength - wheelSize;
                double rectWidth = palletWidth;

                i1 = (int)Math.Floor(rectLength / boxLength);
                j1 = (int)Math.Floor(rectWidth / boxWidth);
                if (i1 == 0 || j1 == 0) { i1 = j1 = 0; }

                i2 = (int)Math.Floor((rectLength - i1 * boxLength) / boxWidth);
                j2 = (int)Math.Floor(rectWidth / boxLength);
                if (i2 == 0 || j2 == 0) { i2 = j2 = 0; }
            }
        }
    }
}
