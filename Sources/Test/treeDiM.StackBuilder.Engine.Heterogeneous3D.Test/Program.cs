#region Using directives
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
#endregion

namespace treeDiM.StackBuilder.Engine.Heterogeneous3D.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string outputDir = @"D:\GitHub\StackBuilder\Sources\Test\treeDiM.StackBuilder.Engine.Heterogeneous3D.Test\Output";
            DirectoryInfo directory = new DirectoryInfo(outputDir);
            directory.Empty();

            var palletProperties = new PalletProperties(null, "EUR2", 1200.0, 1000.0, 140.0)
            {
                Color = Color.Yellow,
                Weight = 22.0
            };
            bool allowXY = false;

            var items = new List<ContentItem>
            {
                new ContentItem(new BoxProperties(null, 190, 200, 420, 10.0, Color.Red), 2) { AllowedOrientations = new bool[] { allowXY, allowXY, true } },
                new ContentItem(new BoxProperties(null, 250, 200, 300, 10.0, Color.Blue), 1) { AllowedOrientations = new bool[] { allowXY, allowXY, true } },
                new ContentItem(new BoxProperties(null, 250, 200, 250, 10.0, Color.Green), 1) { AllowedOrientations = new bool[] { allowXY, allowXY, true } },
                new ContentItem(new BoxProperties(null, 250, 200, 290, 10.0, Color.Green), 1) { AllowedOrientations = new bool[] { allowXY, allowXY, true } },
                new ContentItem(new BoxProperties(null, 80, 200, 210, 10.0, Color.White), 4) { AllowedOrientations = new bool[] { allowXY, allowXY, true } }, // 9

                new ContentItem(new BoxProperties(null, 360, 460, 840, 10.0, Color.Purple), 1) { AllowedOrientations = new bool[] { allowXY, allowXY, true } },
                new ContentItem(new BoxProperties(null, 160, 460, 100, 10.0, Color.AliceBlue), 2) { AllowedOrientations = new bool[] { allowXY, allowXY, true } },
                new ContentItem(new BoxProperties(null, 160, 460, 320, 10.0, Color.Beige), 2) { AllowedOrientations = new bool[] { allowXY, allowXY, true } },
                new ContentItem(new BoxProperties(null, 200, 300, 150, 10.0, Color.Brown), 1) { AllowedOrientations = new bool[] { allowXY, allowXY, true } },
                new ContentItem(new BoxProperties(null, 200, 300, 690, 10.0, Color.Chartreuse), 1) { AllowedOrientations = new bool[] { allowXY, allowXY, true } }, // 7

                new ContentItem(new BoxProperties(null, 200, 300, 210, 10.0, Color.Cyan), 4) { AllowedOrientations = new bool[] { allowXY, allowXY, true } },
                new ContentItem(new BoxProperties(null, 120, 300, 70, 10.0, Color.LightPink), 12) { AllowedOrientations = new bool[] { allowXY, allowXY, true } },
                new ContentItem(new BoxProperties(null, 520, 600, 420, 10.0, Color.LightGray), 2) { AllowedOrientations = new bool[] { allowXY, allowXY, true }, PriorityLevel = 0},
                new ContentItem(new BoxProperties(null, 260, 360, 210, 10.0, Color.Yellow), 4) { AllowedOrientations = new bool[] { allowXY, allowXY, true } },
                new ContentItem(new BoxProperties(null, 260, 360, 840, 10.0, Color.Orange), 1) { AllowedOrientations = new bool[] { allowXY, allowXY, true } } // 23
            };

            var sortedItems = items.OrderBy(i => i.Pack.OuterDimensions.X * i.Pack.OuterDimensions.Y * i.Number).ToList();

            var constraintSet = new HConstraintSetPallet() { MaximumHeight = 1300.0};

            var analysis = new HAnalysisPallet(null)
            {
                Pallet = palletProperties,
                ConstraintSet = constraintSet,
                Content = sortedItems
            };
            HSolver solver = new HSolver();
            var solutions = solver.BuildSolutions(analysis);

            int solIndex = 0;
            foreach (var sol in solutions)
            {
                Vector3D[] orientations = new Vector3D[] { Graphics3D.Corner_0, Graphics3D.Corner_90, Graphics3D.Corner_180, Graphics3D.Corner_270 };
                for (int iOrientation = 0; iOrientation < 4; ++iOrientation)
                {
                    List<Bitmap> images = new List<Bitmap>();
                    for (int binIndex = 0; binIndex < sol.SolItemCount; ++binIndex)
                    {
                        Vector3D bbGlob = sol.BBoxGlobal(binIndex).DimensionsVec;
                        Vector3D bbLoad = sol.BBoxLoad(binIndex).DimensionsVec;
                        double weightLoad = sol.LoadWeight(binIndex);
                        double weightTotal = sol.Weight(binIndex);


                        Graphics3DImage graphics = new Graphics3DImage(ImageTiling.TileSize(new Size(4000, 4000), sol.SolItemCount))
                        {
                            FontSizeRatio = 0.02f,
                            CameraPosition = orientations[iOrientation],
                            ShowDimensions = false
                        };
                        ViewerHSolution sv = new ViewerHSolution(sol, binIndex);
                        sv.Draw(graphics, Transform3D.Identity);
                        graphics.Flush();
                        images.Add(graphics.Bitmap);
                    }
                    Bitmap gBmp = ImageTiling.TileImage(new Size(2000, 2000), images);
                    string sbSolutionName = $"sol_{sol.Algorithm}_{solIndex}_{iOrientation}.png";
                    gBmp.Save(Path.Combine(outputDir, sbSolutionName));

                    Console.WriteLine($"Saving {sbSolutionName}...");
               }
                ++solIndex;
            }
        }
    }

    public static class ExtensionClass
    {
        public static void Empty(this DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles()) file.Delete();
            foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }
    }

}
