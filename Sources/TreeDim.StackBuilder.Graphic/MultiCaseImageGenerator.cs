#region Using directives
using Sharp3D.Math.Core;
using System.Drawing;
using System.Linq;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class MultiCaseImageGenerator
    {
        public enum CaseAlignement { SHARING_LENGTH, SHARING_WIDTH }

        public MultiCaseImageGenerator(Size imgSize, Vector2D ptMin, Vector2D ptMax)
        {
            ImageSize = imgSize;
            PtMin = ptMin;
            PtMax = ptMax;
        }
        public Bitmap GenerateCaseImage(Packable content, int number, CaseAlignement caseAlignment)
        {
            Graphics2DImage graphics = new Graphics2DImage(ImageSize);
            graphics.SetViewport(PtMin - new Vector2D(0.5f, 0.5f), PtMax + new Vector2D(0.5, 0.5));
            graphics.MarginRatio = 0.0f;

            double length = 0.0, width = 0.0;
            if (content is PackableBrick packable)
            {
                length = packable.Length;
                width = packable.Width;
            }

            uint pickId = 0;
            for (int i = 0; i < number; ++i)
            {
                Vector3D v = Vector3D.Zero;
                switch (caseAlignment)
                {
                    case CaseAlignement.SHARING_LENGTH: v = new Vector3D(0.0, i * width, 0.0); break;
                    case CaseAlignement.SHARING_WIDTH: v = new Vector3D(i * length, 0.0, 0.0); break;
                    default: break;
                }

                Box b;
                if (content is PackProperties pack)
                    b = new Pack(pickId++, pack, new BoxPosition(v));
                else if (content is PackableBrick brick)
                    b = new Box(pickId++, brick, new BoxPosition(v));
                else
                    return null;
                b.Draw(graphics);
            }
            return graphics.Bitmap;
        }

        public Bitmap GeneratePalletImage(PalletProperties palletProperties)
        {
            Graphics2DImage graphics = new Graphics2DImage(ImageSize);
            graphics.SetViewport(PtMin - new Vector2D(0.5f, 0.5f), PtMax + new Vector2D(0.5, 0.5));
            graphics.MarginRatio = 0.0f;

            Pallet pallet = new Pallet(palletProperties);
            pallet.Draw(graphics);

            return graphics.Bitmap;
        }

        public static void GenerateDefaultCaseImage(Vector3D dimCase, Size imgSize, int number, CaseAlignement caseAlignment, string filename)
        {
            Size imgSizeTotal = new Size();
            Vector2D ptMin = Vector2D.Zero;
            Vector2D ptMax = Vector2D.Zero;
            switch (caseAlignment)
            {
                case CaseAlignement.SHARING_LENGTH:
                    imgSizeTotal = new Size(imgSize.Width, number * imgSize.Height);
                    ptMax = new Vector2D(dimCase.X, number * dimCase.Y);
                    break;
                case CaseAlignement.SHARING_WIDTH:
                    imgSizeTotal = new Size(imgSize.Width * number, imgSize.Height);
                    ptMax = new Vector2D(number * dimCase.X, dimCase.Y);
                    break;
                default: break;
            }

            var bProperties = new BoxProperties(null, dimCase.X, dimCase.Y, dimCase.Z)
            {
                TapeWidth = new treeDiM.Basics.OptDouble(true, 50.0),
                TapeColor = Color.Tan,
                Facing = 1
            };
            bProperties.SetAllColors(Enumerable.Repeat(Color.Beige, 6).ToArray());

            MultiCaseImageGenerator imageGenerator = new MultiCaseImageGenerator(imgSizeTotal, ptMin, ptMax);
            Bitmap bmp = imageGenerator.GenerateCaseImage(bProperties, number, caseAlignment);
            bmp.Save(filename);
        }

        public static void GenerateDefaultPalletImage(Vector3D dimPallet, string typeName, Size imgSize, string filename)
        {
            // instantiate pallet
            var palletProperties = new PalletProperties(null, typeName, dimPallet.X, dimPallet.Y, dimPallet.Z);

            MultiCaseImageGenerator imageGenerator = new MultiCaseImageGenerator(imgSize, Vector2D.Zero, new Vector2D(dimPallet.X, dimPallet.Y));
            Bitmap bmp = imageGenerator.GeneratePalletImage(palletProperties);
            bmp.Save(filename);
        }
        #region Data members
        public Size ImageSize { get; set; }
        public Vector2D PtMin { get; set; }
        public Vector2D PtMax { get; set; }
        #endregion
    }
}
