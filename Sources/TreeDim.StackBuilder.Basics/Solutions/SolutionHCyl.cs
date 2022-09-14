#region Using directives
using System;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    #region ILayoutSolver
    public interface ILayoutSolver
    {
        HCylLayout BuildLayoutNonStatic(Packable packable
            , IContainer container
            , ConstraintSetAbstract constraintSet
            , string patternName
            , bool swapped);
    }
    #endregion

    #region SolutionHCyl
    public class SolutionHCyl : SolutionHomo
    {
        #region Constructor
        public SolutionHCyl(AnalysisHCyl analysis)
        {
            Analysis = analysis;
        }
        public SolutionHCyl(AnalysisHCyl analysis, string patternName, bool swapped)
        {
            Analysis = analysis;
            PatternName = patternName;
            Swapped = swapped;

            RebuildSolution();
        }
        #endregion

        #region Private methods
        public void RebuildSolution()
        {
            Analysis.ConstraintSet.Container = AnalysisHCyl.Container as IContainer;
            Layout = Solver.BuildLayoutNonStatic(
                Analysis.Content,
                Analysis.ConstraintSet.Container,
                Analysis.ConstraintSet,
                PatternName,
                Swapped);
        }
        #endregion

        #region Public properties
        public AnalysisHCyl AnalysisHCyl => Analysis as AnalysisHCyl;
        #endregion

        #region Implementation SolutionHomo abstract methods 
        public override int ItemCount => Layout.Positions.Count;
        public override BBox3D BBoxLoad => Layout.BBox;
        public override double AreaEfficiency => 0.0;
        #endregion

        #region Private properties
        public HCylLayout Layout { get; private set; }
        private string PatternName { get; set; }
        private bool Swapped { get; set; }
        #endregion

        #region Static solver instance
        private static ILayoutSolver _solver;
        public static void SetSolver(ILayoutSolver solver) => _solver = solver;
        public static ILayoutSolver Solver
        {
            get
            {
                if (null == _solver)
                    throw new Exception("Solver not initialized->Call SolutionHCyl.SetSolver() static function.");
                return _solver;
            }
        }
        #endregion
    }
    #endregion
}
