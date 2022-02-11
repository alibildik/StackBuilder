using System;
using System.Diagnostics;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;

namespace treeDiM.StackBuilder.Engine
{
    internal class LayerPatternBrick : LayerPatternBox
    {
        public override string Name => "Brick";
        public override int GetNumberOfVariants(Layer2DBrickImp layer) => 1;
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

            Vector2D offset = GetOffset(layer, actualLength, actualWidth);
            RecursiveInsertion(layer, offset, actualLength, actualWidth, boxLength, boxWidth);
        }

        public override bool GetLayerDimensions(ILayer2D layer, out double layerLength, out double layerWidth)
        {
            double palletLength = GetPalletLength(layer);
            double palletWidth = GetPalletWidth(layer);
            double boxLength = GetBoxLength(layer);
            double boxWidth = GetBoxWidth(layer);

            int noInLength = (int)Math.Floor(palletLength / boxLength);
            int noInWidth = (int)Math.Floor((palletWidth - 2 * boxWidth) / boxLength);

            layerLength = noInLength * boxLength;
            layerWidth = noInWidth * boxLength + 2 * boxWidth;

            Debug.Assert(layerLength <= palletLength);
            Debug.Assert(layerWidth <= palletWidth);

            return true;
        }

        protected void RecursiveInsertion(ILayer2D layer
            , Vector2D offset
            , double rectLength, double rectWidth
            , double boxLength, double boxWidth
            )
        {
            int noInLength = (int)Math.Floor(rectLength / boxLength);
            int noInWidth = (int)Math.Floor((rectWidth - 2 * boxWidth) / boxLength);
            int noWidthInLength = (int)Math.Floor(rectLength / boxWidth);
            int noLengthInWidth = (int)Math.Floor(rectWidth / boxLength);

            int noLenghtInLength = (int)Math.Floor(rectLength / boxLength);
            int noWidthInWidth = (int)Math.Floor(rectWidth / boxWidth);

            Vector2D internalOffset = Vector2D.Zero;

            if (noWidthInWidth == 1 && noLenghtInLength > 0)
            {
                internalOffset = new Vector2D(0.5 * (rectLength - noInLength * boxLength), 0.5 * (rectWidth - boxWidth));
                for (int i = 0; i < noLenghtInLength; ++i)
                {
                    AddPosition(layer
                        , offset + internalOffset + new Vector2D(i * boxLength, 0.0)
                        , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);

                }
            }
            else if (noLengthInWidth == 1 && noWidthInLength > 0)
            {
                for (int i = 0; i < noWidthInLength; ++i)
                { 
                }
            }

            internalOffset = new Vector2D(
                0.5 * (rectLength - noInLength * boxLength)
                , 0.5 * (rectWidth - noInWidth * boxLength - 2.0 * boxWidth)
                );

            if (noInWidth <= 0 && 2 * boxWidth > rectWidth)
            {
                if (boxWidth > rectWidth)
                    noInLength = 0;

                internalOffset = new Vector2D(
                    0.5 * (rectLength - noInLength * boxLength)
                    , 0.5 * (rectWidth - boxWidth));
            }
            // insert boxes in length
            if (2 * boxWidth <= rectWidth)
            {
                for (int i = 0; i < noInLength; ++i)
                {
                    AddPosition(layer
                        , offset + internalOffset + new Vector2D(i * boxLength, 0.0)
                        , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                    AddPosition(layer
                        , offset + internalOffset + new Vector2D(i * boxLength, noInWidth * boxLength + boxWidth)
                        , HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                }
            }
            // insert boxes in width
            if (2 * boxWidth <= rectLength)
            { 
                for (int i = 0; i < noInWidth; ++i)
                {
                    AddPosition(layer
                        , offset + internalOffset + new Vector2D(boxWidth, boxWidth + i * boxLength)
                        , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
                    AddPosition(layer
                        , offset + internalOffset + new Vector2D(noInLength * boxLength, boxWidth + i * boxLength)
                        , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
                }
            }
/*
            // remaining rectLength
            double remRectLength = rectLength - (noInWidth > 0 && 2 * boxWidth <= rectWidth ? 1 : 0) * 2 * boxWidth;
            double remRectWidth = rectWidth - (noInLength > 0 ? 1 : 0) * 2 * boxWidth;

            int noInRemLength = (int)Math.Floor(remRectLength / boxLength);
            int noInRemWidth = (int)Math.Floor((remRectWidth - 2 * boxWidth) / boxLength);

            // length aligned
            int noLength1 = (int)Math.Floor(remRectLength / boxLength);
            int noWidth1 = (int)Math.Floor(remRectWidth / boxWidth);
            if (noWidth1 == 1)
            {
                internalOffset += new Vector2D(
                    0.5 * (remRectLength - noLength1 * boxLength)
                    , 0.5 * (remRectWidth - boxWidth));

                for (int i = 0; i < noLength1; ++i)
                {
                    AddPosition(layer
                        , offset + internalOffset + new Vector2D(i * boxLength, 0.0)
                        , HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N);
                }
            }
            // width aligned
*/

            // new internal rectangle
            if (noInLength * boxLength -2 * boxWidth >= boxLength) 
                RecursiveInsertion(layer
                    , offset + internalOffset + new Vector2D(boxWidth, boxWidth)
                    , noInLength * boxLength - 2 * boxWidth, noInWidth * boxLength
                    , boxLength, boxWidth); 
                           
        }
    }
}
