using System.Collections.Generic;

using Sharp3D.Math.Core;

namespace treeDiM.StackBuilder.Basics
{
    public abstract class AnalysisHomo : Analysis
    {
        #region Protected constructor
        protected AnalysisHomo(Document doc, Packable packable) : base(doc) { Content = packable; }
        #endregion

        #region Properties
        public Packable Content
        {
            get { return _packable; }
            set
            {
                if (value == _packable) return;
                _packable?.RemoveDependancy(this);
                _packable = value;
                if (null != ParentDocument)
                    _packable?.AddDependancy(this);
            }
        }
        public virtual Vector3D ContentDimensions       => _packable.OuterDimensions;
        public virtual double ContentVolume             => _packable.Volume;
        public virtual double ContentWeight             => _packable.Weight;
        public override bool HasValidSolution           => null != Solution;
        public ConstraintSetAbstract ConstraintSet      { get; set; }
        #endregion

        #region Equivalent packable
        /// <summary>
        /// can analysis solution be reused in other analysis
        /// </summary>
        public abstract bool HasEquivalentPackable { get; }
        /// <summary>
        /// get equivalent packable
        /// </summary>
        public abstract PackableLoaded EquivalentPackable { get; }
        #endregion

        #region Absctract properties
        public abstract ItemBase Container { get; }
        public abstract Vector2D ContainerDimensions { get; }
        public abstract Vector3D Offset { get; }
        public abstract double ContainerWeight { get; }
        public abstract double ContainerLoadingVolume { get; }
        public virtual double DecorationWeight { get; } = 0.0;
        public abstract BBox3D BBoxLoadWDeco(BBox3D loadBBox);
        public abstract BBox3D BBoxGlobal(BBox3D loadBBox);
        #endregion

        #region Abstract dimensions
        public abstract void RecomputeSolution();
        #endregion

        #region Analysis overrides
        public override void OnEndUpdate(ItemBase updatedAttribute)
        {
            base.OnEndUpdate(updatedAttribute);

            KillListeners();
            // ---
            // recompute using same layer(s) when possible
            RecomputeSolution();
            // ---
            EndUpdate();
        }
        protected override void RemoveItselfFromDependancies()
        {
            base.RemoveItselfFromDependancies();
            _packable?.RemoveDependancy(this);
        }
        #endregion

        #region Data members
        protected Packable _packable;
        public SolutionHomo Solution { get; protected set; }
        #endregion
    }

    public abstract class AnalysisLayered : AnalysisHomo
    {
        #region Protected constructor
        protected AnalysisLayered(Document doc, Packable packable) : base(doc, packable) {}
        #endregion

        #region Layer methods
        public SolutionLayered SolutionLay            => Solution as SolutionLayered;
        public virtual bool AlternateLayersPref         => true;
        public List<Layer2DBrickExp> EditedLayers { get; set; } = new List<Layer2DBrickExp>();
        public List<InterlayerProperties> Interlayers { get; private set; } = new List<InterlayerProperties>();
        public virtual bool AllowInterlayer(InterlayerProperties interlayer)
        {
            if (null == interlayer)
                return false;
            return true;    
        }
        public void AddInterlayer(InterlayerProperties interlayer)
        {
            Interlayers.Add(interlayer);
            if (null != ParentDocument)
                interlayer?.AddDependancy(this);
        }
        public int GetInterlayerIndex(InterlayerProperties interlayer)
        {
            if (null == interlayer) return -1;
            if (!Interlayers.Contains(interlayer))
                Interlayers.Add(interlayer);
            return Interlayers.FindIndex(item => item == interlayer);
        }
        public virtual InterlayerProperties Interlayer(int index) => Interlayers[index];
        #endregion

        #region Solution
        public void AddSolution(LayerDesc layerDesc, bool mirrorLength = true, bool mirrorWidth = true)
        {
            Solution = new SolutionLayered(this, layerDesc, mirrorLength, mirrorWidth);
        }
        public void AddSolution(ILayer2D layer, bool mirrorLength = true, bool mirrorWidth = true)
        {
            Solution = new SolutionLayered(this, layer, mirrorLength, mirrorWidth);
        }
        public void AddSolution(List<LayerDesc> layerDescs)
        {
            Solution = new SolutionLayered(this, layerDescs.ConvertAll(l => new LayerEncap(l)));
        }
        public void AddSolution(List<LayerEncap> layers)
        {
            Solution = new SolutionLayered(this, layers);
        }
        public void AddSolution(List<KeyValuePair<LayerDesc, int>> listLayers)
        {
            Solution = new SolutionLayered(this,
                listLayers.ConvertAll(l => new KeyValuePair<LayerEncap, int>(new LayerEncap(l.Key), l.Value))
                );
        }
        public void AddSolution(List<KeyValuePair<LayerEncap,int>> listLayers)
        {
            Solution = new SolutionLayered(this, listLayers);
        }
        public void AddSolution(List<LayerEncap> layers, List<SolutionItem> listSolItems)
        {
            Solution = new SolutionLayered(this, layers) { SolutionItems = listSolItems };
        }
        #endregion

        #region AnalysisHomo overrides
        public override void RecomputeSolution()
        {
            // workaround
            if (Content is PackProperties)
            {
                if (ConstraintSet is ConstraintSetCasePallet constraintSet)
                    constraintSet.SetAllowedOrientations(new bool[] { false, false, true });
            }
            // get best layers
            List<ILayer2D> bestLayers = SolutionLayered.Solver.BuildLayers(Content, ContainerDimensions, Offset.Z, ConstraintSet, true);
            List<LayerEncap> bestLayerDescs = bestLayers.ConvertAll(l => new LayerEncap(l.LayerDescriptor));

            bool allFound = true;
            foreach (LayerEncap l in SolutionLay.LayerEncaps)
            {
                if (null == bestLayerDescs.Find(bld => bld.Equals(l)))
                {
                    allFound = false;
                    break;
                }
            }
            if (allFound)
                SolutionLay.RebuildLayers();
            else // recomputes whole new solution
                Solution = new SolutionLayered(this, bestLayerDescs);
        }
        protected override void RemoveItselfFromDependancies()
        {
            base.RemoveItselfFromDependancies();
            foreach (InterlayerProperties interlayer in Interlayers)
                interlayer.RemoveDependancy(this);
        }
        #endregion
    }
}
