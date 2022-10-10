#region Using directives
using System;
using System.Drawing;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public class ViewerSolutionPalletColumn : Viewer
    {
        #region Constructor
        public ViewerSolutionPalletColumn(SolutionPalletColumn solution)
        {
            Solution = solution;
        }
        #endregion
        #region Viewer override
        public override void Draw(Graphics3D graphics, Transform3D transform)
        {
            if (null == Solution) return;
            AnalysisPalletColumn analysis = Solution.Analysis;

            // ### draw loaded pallets
            uint pickId = 0;
            BBox3D bbox = new BBox3D();
            foreach (HSolElement solElt in Solution.ContainedElements)
            {
                if (Analysis.ContentTypeByIndex(solElt.ContentType) is LoadedPallet loadedPallet)
                {
                    BBox3D solBBox = loadedPallet.ParentAnalysis.Solution.BBoxGlobal;
                    graphics.AddImage(++pickId, new SubContent(loadedPallet.ParentAnalysis), solBBox.DimensionsVec, solElt.Position.Transform(transform));
                    // bbox used for picking
                    bbox.Extend(new BBox3D(solElt.Position.Transform(transform), solBBox.DimensionsVec));
                }
            }
            // ###

            // ### dimensions
            if (graphics.ShowDimensions)
            {
                foreach (HSolElement solElt in Solution.ContainedElements)
                {
                    if (Analysis.ContentTypeByIndex(solElt.ContentType) is LoadedPallet loadedPallet)
                    {
                        BBox3D solBBox = loadedPallet.ParentAnalysis.Solution.BBoxGlobal;
                        graphics.AddDimensions(new DimensionCube(new BBox3D(solElt.Position.Transform(transform), solBBox.DimensionsVec), Color.Red, false));
                    }
                }
                graphics.AddDimensions(new DimensionCube(BoundingBoxDim(0), Color.Black, true));
            }
            // ###
        }
        public override void Draw(Graphics2D graphics)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Accessors
        public SolutionPalletColumn Solution { get; private set; }
        public AnalysisPalletColumn Analysis => Solution.Analysis;
        #endregion
        #region Helpers
        private BBox3D BoundingBoxDim(int index) => Solution.BBoxGlobal;
        #endregion
    }
}
