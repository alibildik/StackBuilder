#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;

using Sharp3D.Math.Core;
using Sharp3DBinPacking;
using Sharp3D.Boxologic;
using log4net;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Engine
{
    public class HSolver : IHSolver
    {
        public List<HSolution> BuildSolutions(AnalysisHetero analysis)
        {
            // dim container + offset
            Vector3D dimContainer = analysis.DimContainer(0), offset = analysis.Offset(0);
            // content items
            List<ContentItem> contentItems = new List<ContentItem>(analysis.Content);
            // solutions
            List<HSolution> solutions = new List<HSolution>();

            // *** Sharp3DBinPacking : begin
            // create cuboid list
            List<Cuboid> listCuboids = new List<Cuboid>();
            bool bAllowAllOrientations = true;
            foreach (ContentItem ci in contentItems)
            {
                if (!ci.AllowOrientX && !ci.AllowOrientY && !ci.AllowOrientZ)
                    continue;
                for (int i = 0; i < ci.Number; ++i)
                {
                    if (ci.Pack is BoxProperties b)
                        listCuboids.Add(
                            new Cuboid((decimal)b.Length, (decimal)b.Width, (decimal)b.Height)
                            {
                                Tag = b,
                                AllowOrientX = ci.AllowOrientX,
                                AllowOrientY = ci.AllowOrientY,
                                AllowOrientZ = ci.AllowOrientZ,
                                PriorityLevel = ci.PriorityLevel
                            }
                        );
                }
                if (!ci.AllowOrientX || !ci.AllowOrientY || !ci.AllowOrientZ)
                    bAllowAllOrientations = false;
            }

            // Create a bin packer instance
            // The default bin packer will test all algorithms and try to find the best result
            // BinPackerVerifyOption is used to avoid bugs, it will check whether the result is correct
            var binPacker = BinPacker.GetDefault(BinPackerVerifyOption.BestOnly, bAllowAllOrientations);
            // The result contains bins which contains packed cuboids whith their coordinates
            var parameter = new BinPackParameter(
                (decimal)dimContainer.X, (decimal)dimContainer.Y, (decimal)dimContainer.Z,
                listCuboids.ToArray())
            { ShuffleCount = 10 };

            var binPackResult = binPacker.Pack(parameter);
            {
                HSolution sol = new HSolution("Sharp3DBinPacking") { Analysis = analysis };
                foreach (var bins in binPackResult.BestResult)
                {
                    HSolItem hSolItem = sol.CreateSolItem();
                    foreach (var cuboid in bins)
                    {
                        CuboidToSolItem(contentItems, offset, cuboid, out int index, out BoxPosition pos);
                        hSolItem.InsertContainedElt(index, pos);
                    }
                }
                solutions.Add(sol);
            }
            // *** Sharp3DBinPacking : end

            // *** BoxoLogic : begin
            Dictionary<BProperties, uint> dict1 = new Dictionary<BProperties, uint>();
            List<BoxItem> listItems = new List<BoxItem>();
            foreach (ContentItem ci in contentItems)
            {
                for (int i = 0; i < ci.Number; ++i)
                {
                    if (ci.Pack is BoxProperties b)
                        listItems.Add(
                            new BoxItem()
                            {
                                ID = BoxToID(b, ref dict1),
                                Boxx = (decimal)b.Length,
                                Boxy = (decimal)b.Width,
                                Boxz = (decimal)b.Height,
                                AllowX = ci.AllowOrientX,
                                AllowY = ci.AllowOrientY,
                                AllowZ = ci.AllowOrientZ,
                                N = 1,
                                Order = ci.PriorityLevel
                            }
                        );
                }
            }
            var bl = new Boxlogic() { OutputFilePath = string.Empty };
            var solArray = new SolutionArray();
            bl.Run(listItems.ToArray(), (decimal)dimContainer.X, (decimal)dimContainer.Y, (decimal)dimContainer.Z, ref solArray);
            foreach (var solution in solArray.Solutions)
            {
                HSolution sol = new HSolution($"Boxologic - Variant {solution.Variant}") { Analysis = analysis };
                HSolItem hSolItem = sol.CreateSolItem();

                Transform3D transform = Transform3D.Identity;
                switch (solution.Variant)
                {
                    case 1: transform = Transform3D.Translation(new Vector3D(0.0, dimContainer.Y, 0.0)) * Transform3D.RotationX(90.0); break;
                    case 2: transform = Transform3D.Translation(new Vector3D(dimContainer.X, 0.0, 0.0)) * Transform3D.RotationZ(90.0); break;
                    case 3: transform = Transform3D.Translation(new Vector3D(dimContainer.X, 0.0, 0.0)) * Transform3D.RotationZ(90.0); break;
                    case 4: transform = Transform3D.Translation(new Vector3D(dimContainer.X, 0.0, 0.0)) * Transform3D.RotationY(-90.0); break;
                    case 5: transform = Transform3D.Translation(new Vector3D(0.0, dimContainer.Y, 0.0)) * Transform3D.RotationX(90.0); break;
                    default: transform = Transform3D.Identity; break;
                }

                foreach (var item in solution.ItemsPacked)
                {
                    BoxInfoToSolItem(dict1, contentItems, offset, item, transform, out int index, out BoxPosition pos);
                    hSolItem.InsertContainedElt(index, pos.Adjusted(new Vector3D((double)item.DimX, (double)item.DimY, (double)item.DimZ)));
                }
                hSolItem.Recenter(sol, dimContainer);
                solutions.Add(sol);

                // ----
                var contentItemsClone = new List<ContentItem>();
                foreach (var ci in contentItems)
                    contentItemsClone.Add(
                        new ContentItem(ci.Pack, ci.Number, new bool[] { ci.AllowOrientX, ci.AllowOrientY, ci.AllowOrientZ })
                        { PriorityLevel = ci.PriorityLevel }
                        );

                foreach (var item in solution.ItemsPacked)
                {
                    var bProperties = IDToBox(item.Id, dict1);
                    var ci = contentItemsClone.Find(c => c.Pack == bProperties);
                    if (ci == null)
                        System.Diagnostics.Debug.Assert(null != ci);
                    ci.Number -= 1;
                }

                RunBoxologic(solution.Variant, sol, dimContainer, offset, contentItemsClone);
                // ---
            }
            // *** BoxoLogic : end

            return solutions;
        }
        private void RunBoxologic(int variant, HSolution hSol, Vector3D dimContainer, Vector3D offset, List<ContentItem> contentItems)
        {
            var dict2 = new Dictionary<BProperties, uint>();
            // ContentItem -> BoxItem
            uint iCountBoxItems = 0;
            List<BoxItem> boxItems = new List<BoxItem>();
            foreach (ContentItem ci in contentItems)
            {
                for (int i = 0; i < ci.Number; ++i)
                {
                    if (ci.Pack is BProperties b)
                        boxItems.Add(
                            new BoxItem()
                            {
                                ID = BoxToID(b, ref dict2),
                                Boxx = (decimal)b.Length,
                                Boxy = (decimal)b.Width,
                                Boxz = (decimal)b.Height,
                                AllowX = ci.AllowOrientX,
                                AllowY = ci.AllowOrientY,
                                AllowZ = ci.AllowOrientZ,
                                N = 1,
                                Order = ci.PriorityLevel
                            }
                    );
                }
                iCountBoxItems += ci.Number;
            }

            // solve
            var bl = new Boxlogic() { OutputFilePath = string.Empty };
            var solArray = new SolutionArray();
            bl.Run(boxItems.ToArray(), (decimal)dimContainer.X, (decimal)dimContainer.Y, (decimal)dimContainer.Z, ref solArray);
            if (solArray.Solutions.Count == 0) return;
            foreach (var solution in solArray.Solutions)
            {
                if (solution.Variant != variant)
                    continue;
                HSolItem hSolItem = hSol.CreateSolItem();
                Transform3D transform = Transform3D.Identity;
                switch (solution.Variant)
                {
                    case 1: transform = Transform3D.Translation(new Vector3D(0.0, dimContainer.Y, 0.0)) * Transform3D.RotationX(90.0); break;
                    case 2: transform = Transform3D.Translation(new Vector3D(dimContainer.X, 0.0, 0.0)) * Transform3D.RotationZ(90.0); break;
                    case 3: transform = Transform3D.Translation(new Vector3D(dimContainer.X, 0.0, 0.0)) * Transform3D.RotationZ(90.0); break;
                    case 4: transform = Transform3D.Translation(new Vector3D(dimContainer.X, 0.0, 0.0)) * Transform3D.RotationY(-90.0); break;
                    case 5: transform = Transform3D.Translation(new Vector3D(0.0, dimContainer.Y, 0.0)) * Transform3D.RotationX(90.0); break;
                    default: transform = Transform3D.Identity; break;
                }
                foreach (var item in solution.ItemsPacked)
                {
                    BoxInfoToSolItem(dict2, contentItems, offset, item, transform, out int index, out BoxPosition pos);
                    hSolItem.InsertContainedElt(index, pos.Adjusted(new Vector3D((double)item.DimX, (double)item.DimY, (double)item.DimZ)));
                }
                hSolItem.Recenter(hSol, dimContainer);

                var contentItemsClone = new List<ContentItem>();
                foreach (var ci in contentItems)
                    contentItemsClone.Add(
                        new ContentItem(ci.Pack, ci.Number, new bool[] { ci.AllowOrientX, ci.AllowOrientY, ci.AllowOrientZ }) { PriorityLevel = ci.PriorityLevel });

                foreach (var item in solution.ItemsPacked)
                {
                    var ci = contentItemsClone.Find(c => c.Pack == IDToBox(item.Id, dict2));
                    System.Diagnostics.Debug.Assert(null != ci);
                    ci.Number -= 1;
                }
                // remaining number of items
                int iCount = contentItemsClone.Sum(c => (int)c.Number);

                if (iCount > 0)
                    RunBoxologic(variant, hSol, dimContainer, offset, contentItemsClone);
            }
        }

        private uint BoxToID(BProperties b, ref Dictionary<BProperties, uint> dictionary)
        {
            if (!dictionary.ContainsKey(b))
                dictionary.Add(b, (uint)dictionary.Count);
            return dictionary[b];
        }
        private BProperties IDToBox(uint id, Dictionary<BProperties, uint> dictionary)
        {
            if (!dictionary.ContainsValue(id))
                return null;
            return dictionary.FirstOrDefault(x => x.Value == id).Key;
        }

        private bool BoxInfoToSolItem(Dictionary<BProperties, uint> dictionnary, List<ContentItem> contentItems, Vector3D offset, SolItem solItem, Transform3D transform, out int index, out BoxPosition pos)
        {
            index = 0;
            pos = BoxPosition.Zero;
            BProperties b = IDToBox(solItem.Id, dictionnary);

            if (null != b)
            {
                try
                {
                    index = contentItems.FindIndex(c => c.Pack == b);
                    pos = BoxPosition.FromPositionDimension(
                        new Vector3D((double)solItem.X, (double)solItem.Y, (double)solItem.Z),
                        new Vector3D((double)solItem.BX, (double)solItem.BY, (double)solItem.BZ),
                        new Vector3D((double)solItem.DimX, (double)solItem.DimY, (double)solItem.DimZ)
                        );
                    pos = pos.Transform(Transform3D.Translation(offset) * transform);

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                }
            }
            return false;
        }

        private bool CuboidToSolItem(List<ContentItem> contentItems, Vector3D offset, Cuboid cuboid, out int index, out BoxPosition pos)
        {
            index = 0;
            pos = BoxPosition.Zero;

            if (cuboid.Tag is BoxProperties bProperties)
            {
                try
                {
                    index = contentItems.FindIndex(ci => ci.MatchDimensions((double)cuboid.Width, (double)cuboid.Depth, (double)cuboid.Height));
                    pos = BoxPosition.FromPositionDimension(
                        new Vector3D((double)cuboid.X, (double)cuboid.Y, (double)cuboid.Z) + offset,
                        new Vector3D((double)cuboid.Width, (double)cuboid.Height, (double)cuboid.Depth),
                        new Vector3D(bProperties.Length, bProperties.Width, bProperties.Height)
                        );
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                }
            }
            return false;
        }

        private static ILog _log = LogManager.GetLogger(typeof(HSolver));
    }
}