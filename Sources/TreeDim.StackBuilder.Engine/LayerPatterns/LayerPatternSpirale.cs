﻿using System;

using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;

namespace treeDiM.StackBuilder.Engine
{
    class LayerPatternSpirale: LayerPatternBox
    {
        public override string Name => "Spirale";
        public override int GetNumberOfVariants(Layer2DBrickImp layer) => 1;
        public override bool IsSymetric => false;
        public override bool CanBeSwapped => true;
        public override bool CanBeInverted => true;

        public override void GenerateLayer(ILayer2D layer, double actualLength, double actualWidth)
        {
            // initialization
            layer.Clear();
            double palletLength = GetPalletLength(layer);
            double palletWidth = GetPalletWidth(layer);
            double boxLength = GetBoxLength(layer);
            double boxWidth = GetBoxWidth(layer);
            GetOptimalSizesXY(boxLength, boxWidth, palletLength, palletWidth
                , out int sizeX_area1, out int sizeY_area1, out int sizeX_area2, out int sizeY_area2);

            // compute offsets
            double offsetX = 0.5 * (palletLength - actualLength);
            double offsetY = 0.5 * (palletWidth - actualWidth);

            // area1
            for (int i=0; i<sizeX_area1; ++i)
                for (int j = 0; j<sizeY_area1; ++j)
                {
                    AddPosition(layer
                        , new Vector2D(offsetX + i * boxLength, offsetY + j * boxWidth)
                        , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                    AddPosition(layer
                        , new Vector2D(palletLength - offsetX - i * boxLength, palletWidth - offsetY - j * boxWidth)
                        , HalfAxis.HAxis.AXIS_X_N, HalfAxis.HAxis.AXIS_Y_N);
                }
            double spaceX_Area1 = actualLength - 2.0 * sizeX_area1 * boxLength;
            double spaceY_Area1 = actualWidth - 2.0 * sizeY_area1 * boxWidth;

            // area2
            for (int i=0; i<sizeX_area2; ++i)
                for (int j = 0; j<sizeY_area2; ++j)
                {
                    AddPosition(layer
                        , new Vector2D(palletLength - offsetX - i * boxWidth, offsetY + j * boxLength)
                        , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
                    AddPosition(layer
                        , new Vector2D(offsetX + i * boxWidth, palletWidth - offsetY - j * boxLength)
                        , HalfAxis.HAxis.AXIS_Y_N, HalfAxis.HAxis.AXIS_X_P);
                }

            double spaceX_Area2 = actualLength - 2.0 * sizeX_area2 * boxWidth;
            double spaceY_Area2 = actualWidth - 2.0 * sizeY_area2 * boxLength;
            double spaceX = spaceX_Area1 > 0 ? spaceX_Area1 : spaceX_Area2;
            double spaceY = spaceY_Area1 > 0 ? spaceY_Area1 : spaceY_Area2;

            layer.UpdateMaxSpace( Math.Min(spaceX, spaceY), Name );
        }

        public override bool GetLayerDimensions(ILayer2D layer, out double actualLength, out double actualWidth)
        {
            double palletLength = GetPalletLength(layer);
            double palletWidth = GetPalletWidth(layer);
            double boxLength = GetBoxLength(layer);
            double boxWidth = GetBoxWidth(layer);
            GetOptimalSizesXY(boxLength, boxWidth, palletLength, palletWidth
                , out int sizeX_area1, out int sizeY_area1, out int sizeX_area2, out int sizeY_area2);

            // actual length / actual width
            actualLength = sizeX_area1 * boxLength + sizeX_area2 * boxWidth;
            actualWidth = sizeY_area1 * boxWidth + sizeY_area2 * boxLength;
            if (2.0 * sizeX_area1 * boxLength > palletLength
                && 2.0 * sizeY_area1 * boxWidth > actualWidth)
                actualWidth = 2.0 * sizeY_area1 * boxWidth;
            else if (2.0 * sizeY_area1 * boxWidth > palletWidth
                && 2.0 * sizeX_area1 * boxLength > actualLength)
                actualLength = 2.0 * sizeX_area1 * boxLength;
            else if (2.0 * sizeX_area2 * boxWidth > palletLength
                && 2.0 * sizeY_area2 * boxLength > actualWidth)
                actualWidth = 2.0 * sizeY_area2 * boxLength;
            else if (2.0 * sizeY_area2 * boxLength > palletWidth
                && 2.0 * sizeX_area2 * boxWidth > actualLength)
                actualLength = 2.0 * sizeX_area2 * boxWidth;

            return sizeX_area1 > 0 && sizeX_area2 > 0 && sizeY_area1 > 0 && sizeY_area2 > 0;
        }

        #region Non-Public Members

        void GetOptimalSizesXY(
            double boxLength, double boxWidth
            , double palletLength, double palletWidth
            , out int sizeX_area1, out int sizeY_area1
            , out int sizeX_area2, out int sizeY_area2)
        {
            // get optimum combination of sizeXLength, sizeYLength
            int sizeXLengthMax = (int)Math.Floor((palletLength - boxWidth) / boxLength);
            int sizeYLengthMax = (int)Math.Floor((palletWidth - boxLength) / boxWidth);

            // initialization
            int iBoxNumberMax = 0;
            sizeX_area1 = 0;
            sizeY_area1 = 0;
            sizeX_area2 = 0;
            sizeY_area2 = 0;

            for (int i1 = 1; i1 <= sizeXLengthMax; ++i1)
                for (int j1 = 1; j1 <= sizeYLengthMax; ++j1)
                {
                    double L1 = i1 * boxLength;
                    double H1 = j1 * boxWidth;

                    if ( (L1 > 0.5 * palletLength && H1 > 0.5 * palletWidth)
                        || (L1 < 0.5 * palletLength && H1 < 0.5 *palletWidth))
                        continue;

                    int i2 = (int)((palletLength - L1) / boxWidth);
                    int j2 = (int)((palletWidth - H1) / boxLength);

                    if (iBoxNumberMax < 2 * (i1 * j1 + i2 * j2))
                    {
                        iBoxNumberMax = 2 * (i1 * j1 + i2 * j2);
                        sizeX_area1 = i1;
                        sizeY_area1 = j1;
                        sizeX_area2 = i2;
                        sizeY_area2 = j2;
                    }
                }
        }

        #endregion
    }
}
