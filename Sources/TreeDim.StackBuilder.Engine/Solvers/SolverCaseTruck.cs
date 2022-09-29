#region Using directives
using System.Collections.Generic;
using System.Linq;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Engine
{
    public class SolverCaseTruck : ISolver
    {
        public SolverCaseTruck(PackableBrick packable, TruckProperties truckProperties, ConstraintSetCaseTruck constraintSet)
        {
            _packable = packable;
            TruckProperties = truckProperties;
            ConstraintSet = constraintSet;
        }
        public Layer2DBrickImp BuildBestLayer()
        {
            // build layer list
            var solver = new LayerSolver();
            List<Layer2DBrickImp> layers = solver.BuildLayers(
                    _packable.OuterDimensions
                    , _packable.Bulge
                    , new Vector2D(TruckProperties.InsideLength, TruckProperties.InsideWidth)
                    , 0.0 /* offsetZ */
                    , ConstraintSet
                    , true
                );
            return layers.Count > 0 ? layers.First() : null;
        }
        public List<AnalysisLayered> BuildAnalyses(bool allowMultipleLayerOrientations)
        {
            var analyses = new List<AnalysisLayered>();
            // get best set of layers
            if (allowMultipleLayerOrientations)
            {
                var listLayerEncap = new List<KeyValuePair<LayerEncap, int>>();
                LayerSolver.GetBestCombination(
                    _packable.OuterDimensions,
                    _packable.Bulge,
                    TruckProperties.GetStackingDimensions(ConstraintSet),
                    ConstraintSet,
                    ref listLayerEncap);

                var layerEncaps = new List<LayerEncap>();
                foreach (var vp in listLayerEncap)
                    layerEncaps.Add(vp.Key);

                var analysis = new AnalysisCaseTruck(null, _packable, TruckProperties, ConstraintSet);
                analysis.AddSolution(layerEncaps);
                // only add analysis if it has a valid solution
                if (analysis.Solution.ItemCount > 0)
                    analyses.Add(analysis);
            }
            else
            {
                // build layer list
                var solver = new LayerSolver();
                List<Layer2DBrickImp> layers = solver.BuildLayers(
                     _packable.OuterDimensions
                     , _packable.Bulge
                     , new Vector2D(TruckProperties.InsideLength, TruckProperties.InsideWidth)
                     , 0.0 /* offsetZ */
                     , ConstraintSet
                     , true
                 );
                SolutionLayered.SetSolver(solver);
                // loop on layers
                foreach (Layer2DBrickImp layer in layers)
                {
                    var layerDescs = new List<LayerDesc> { layer.LayerDescriptor };
                    var analysis = new AnalysisCaseTruck(null, _packable, TruckProperties, ConstraintSet);
                    analysis.AddSolution(layerDescs);
                    // only add analysis if it has a valid solution
                    if (analysis.Solution.ItemCount > 0)
                        analyses.Add(analysis);
                }
            }
            return analyses;
        }
        #region Non-Public Members
        private PackableBrick _packable;
        private TruckProperties TruckProperties { get; set; }
        private ConstraintSetCaseTruck ConstraintSet { get; set; }
        #endregion
    }
}
