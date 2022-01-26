#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using log4net;

using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics.Controls;
using treeDiM.StackBuilder.Desktop.Properties;
using treeDiM.StackBuilder.Exporters;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewAnalysisCasePallet
        : FormNewAnalysis, IItemBaseFilter
    {
        #region Constructor
        public FormNewAnalysisCasePallet(Document doc, AnalysisLayered analysis)
            : base(doc, analysis)
        {
            InitializeComponent();
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // fill combo box
            cbCases.Initialize(_document, this, AnalysisCast?.Content);
            cbPallets.Initialize(_document, this, AnalysisCast?.PalletProperties);
            // event handling
            uCtrlLayerList.LayerSelected += OnLayerSelected;
            uCtrlLayerList.RefreshFinished += OnLayerSelected;
            uCtrlLayerListEdited.LayerSelected += OnLayerSelected;
            if (null == AnalysisCast)
            {
                tbName.Text = _document.GetValidNewAnalysisName(ItemDefaultName);
                tbDescription.Text = tbName.Text;

                if (string.IsNullOrEmpty(ExporterRobot.DefaultName))
                    uCtrlCaseOrientation.AllowedOrientations = new bool[] { Settings.Default.AllowVerticalX, Settings.Default.AllowVerticalY, Settings.Default.AllowVerticalZ };
                else
                    uCtrlCaseOrientation.AllowedOrientations = new bool[] { false, false, true };

                uCtrlMaximumHeight.Value = Settings.Default.MaximumPalletHeight;
                uCtrlOptMaximumWeight.Value = new OptDouble(false, Settings.Default.MaximumPalletWeight);
                uCtrlOptMaxNumber.Value = new OptInt(false, 1000);

                uCtrlOverhang.ValueX = Settings.Default.OverhangX;
                uCtrlOverhang.ValueY = Settings.Default.OverhangY;
            }
            else
            {
                tbName.Text = AnalysisBase.Name;
                tbDescription.Text = AnalysisBase.Description;

                ConstraintSetCasePallet constraintSet = AnalysisCast.ConstraintSet as ConstraintSetCasePallet;
                uCtrlCaseOrientation.AllowedOrientations = constraintSet.AllowedOrientations;
                uCtrlMaximumHeight.Value = constraintSet.OptMaxHeight.Value;
                uCtrlOptMaximumWeight.Value = constraintSet.OptMaxWeight;
                uCtrlOptMaxNumber.Value = constraintSet.OptMaxNumber;
                uCtrlOverhang.ValueX = constraintSet.Overhang.X;
                uCtrlOverhang.ValueY = constraintSet.Overhang.Y;
                uCtrlOptSpace.Value = constraintSet.MinimumSpace;
            }
            checkBoxBestLayersOnly.Checked = Settings.Default.KeepBestSolutions;
            OnLayerSelected(this, null);
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Settings.Default.AllowVerticalX = uCtrlCaseOrientation.AllowedOrientations[0];
            Settings.Default.AllowVerticalY = uCtrlCaseOrientation.AllowedOrientations[1];
            Settings.Default.AllowVerticalZ = uCtrlCaseOrientation.AllowedOrientations[2];
            Settings.Default.MaximumPalletHeight = uCtrlMaximumHeight.Value;
            if (uCtrlOptMaximumWeight.Value.Activated)
                Settings.Default.MaximumPalletWeight = uCtrlOptMaximumWeight.Value.Value;
            if (uCtrlOptMaxNumber.Value.Activated)
                Settings.Default.MaximumNumber = uCtrlOptMaxNumber.Value.Value;
            Settings.Default.OverhangX = uCtrlOverhang.ValueX;
            Settings.Default.OverhangY = uCtrlOverhang.ValueY;
            Settings.Default.KeepBestSolutions = checkBoxBestLayersOnly.Checked;
        }
        #endregion
        #region FormNewBase overrides
        public override string ItemDefaultName => Resources.ID_ANALYSIS;
        #endregion
        #region FormNewAnalysis override
        public override void UpdateStatus(string message)
        {
            ILayer2D[] layersEditable = uCtrlLayerListEdited.Selected;
            ILayer2D[] layers = uCtrlLayerList.Selected;
            if (layers.Length + layersEditable.Length == 0)
                message = Resources.ID_SELECTATLEASTONELAYOUT;
            base.UpdateStatus(message);
        }
        public override void OnNext()
        {
            try
            {
                var layerEncaps = new List<LayerEncap>();
                foreach (ILayer2D layer2D in uCtrlLayerListEdited.Selected)
                    layerEncaps.Add(new LayerEncap(layer2D));
                foreach (Layer2DBrickImp layer2D in uCtrlLayerList.Selected)
                    layerEncaps.Add(new LayerEncap(layer2D.LayerDescriptor));
                SolutionLayered.SetSolver(new LayerSolver());
                AnalysisCasePallet analysis = AnalysisCast;
                if (null == analysis)
                {
                    _item = _document.CreateNewAnalysisCasePallet(
                        ItemName, ItemDescription
                        , SelectedPackable, SelectedPallet
                        , new List<InterlayerProperties>()
                        , null, null, null
                        , BuildConstraintSet()
                        , layerEncaps
                        , new List<ConveyorSetting>()
                        );
                }
                else
                {
                    analysis.ID.SetNameDesc(ItemName, ItemDescription);
                    analysis.Content = SelectedPackable;
                    analysis.PalletProperties = SelectedPallet;
                    analysis.ConstraintSet = BuildConstraintSet();
                    analysis.AddSolution(layerEncaps);

                    _document.UpdateAnalysis(analysis);
                }
                Close();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion
        #region IItemBaseFilter implementation
        public bool Accept(Control ctrl, ItemBase itemBase)
        {
            if (ctrl == cbCases)
            {
                return itemBase is Packable packable
                    && (
                    (packable is BProperties) ||
                    (packable is PackProperties) ||
                    (packable is LoadedCase)
                    );
            }
            else if (ctrl == cbPallets)
            {
                return itemBase is PalletProperties;
            }
            return false;
        }
        #endregion
        #region Public properties
        public AnalysisCasePallet AnalysisCast
        {
            get { return _item as AnalysisCasePallet; }
            set { _item = value; }
        }
        #endregion
        #region Private properties
        private Packable SelectedPackable => cbCases.SelectedType as Packable;
        private PalletProperties SelectedPallet => cbPallets.SelectedType as PalletProperties;
        #endregion
        #region Event handlers
        protected void OnCaseChanged(object sender, EventArgs e)
        {
            try
            {
                uCtrlCaseOrientation.BProperties = cbCases.SelectedType as PackableBrick;

                FillEditedLayerList();
                OnInputChanged(sender, e);
                OnLayerSelected(sender, e);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        protected void OnLayerSelected(object sender, EventArgs e)
        {
            try
            {
                ILayer2D[] layersEditable = uCtrlLayerListEdited.Selected;
                ILayer2D[] layers = uCtrlLayerList.Selected;
                bnEditLayer.Enabled = (layers.Length == 1);
                bnEditLayerRight.Visible = (layersEditable.Length == 1);
                UpdateStatus(string.Empty);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnInputChanged(object sender, EventArgs e)
        {
            try { FillLayerList(); } catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void FillLayerList()
        { 
            // get case /pallet
            if (!(cbCases.SelectedType is PackableBrick packable) || !(cbPallets.SelectedType is PalletProperties palletProperties))
                return;
            // compute
            LayerSolver solver = new LayerSolver { Facing = packable.Facing };
            List<Layer2DBrickImp> layers = solver.BuildLayers(
                packable.OuterDimensions
                , packable.Bulge
                , new Vector2D(palletProperties.Length + 2.0 * uCtrlOverhang.ValueX, palletProperties.Width + 2.0 * uCtrlOverhang.ValueY)
                , palletProperties.Height
                , BuildConstraintSet()
                , checkBoxBestLayersOnly.Checked);
            // update control
            uCtrlLayerList.Packable = packable;
            uCtrlLayerList.ContainerHeight = uCtrlMaximumHeight.Value - palletProperties.Height;
            uCtrlLayerList.FirstLayerSelected = true;
            uCtrlLayerList.LayerList = layers.Cast<ILayer2D>().ToList();        
        }
        private void FillEditedLayerList()
        {
            if (!(cbCases.SelectedType is Packable packable) || !(cbPallets.SelectedType is PalletProperties palletProperties))
                return;
            uCtrlLayerListEdited.ContainerHeight = uCtrlMaximumHeight.Value - palletProperties.Height;
            uCtrlLayerListEdited.FirstLayerSelected = true;
            uCtrlLayerListEdited.Packable = packable;
            uCtrlLayerListEdited.LayerList = _layersEdited.Cast<ILayer2D>().ToList();
        }
        private void OnBestCombinationClicked(object sender, EventArgs e)
        {
            try
            {
                if (!(cbCases.SelectedType is PackableBrick packable) || !(cbPallets.SelectedType is PalletProperties palletProperties))
                    return;
                ConstraintSetCasePallet constraintSet = BuildConstraintSet();
                // get best combination
                List<KeyValuePair<LayerEncap, int>> listLayer = new List<KeyValuePair<LayerEncap,int>>();
                LayerSolver.GetBestCombination(
                    packable.OuterDimensions,
                    packable.Bulge,
                    palletProperties.GetStackingDimensions(constraintSet),
                    constraintSet,
                    ref listLayer);
                // select best layers
                List<LayerDesc> LayerDesc = new List<LayerDesc>();
                foreach (KeyValuePair<LayerEncap, int> kvp in listLayer)
                    LayerDesc.Add(kvp.Key.LayerDesc);
                uCtrlLayerList.SelectLayers(LayerDesc);

                _item = _document.CreateNewAnalysisCasePallet(
                    ItemName, ItemDescription
                    , SelectedPackable, SelectedPallet
                    , new List<InterlayerProperties>()
                    , null, null, null
                    , constraintSet
                    , listLayer
                    , new List<ConveyorSetting>()
                    );
                Close();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }            
        }
        private void OnEditLayer(object sender, EventArgs e)
        {
            try
            {
                // get content
                if (!(cbCases.SelectedType is PackableBrick packable))  return;
                // get container
                var constraintSet = BuildConstraintSet();
                Vector2D layerDim = new Vector2D(SelectedPallet.Length, SelectedPallet.Width) + 2 * constraintSet.Overhang;
                // get selected layer
                ILayer2D[] layers = uCtrlLayerList.Selected;
                if (layers.Length != 1) return;
                Layer2DBrickImp layer = layers[0] as Layer2DBrickImp;
                using (var form = new FormEditLayer(layer.GenerateLayer2DEdited(), packable))
                {
                    form.TopMost = true;
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        _layersEdited.Add( form.Layer );
                        FillEditedLayerList();
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnEditLayerRight(object sender, EventArgs e)
        {
            try
            {
                // get content
                if (!(cbCases.SelectedType is Packable packable))
                    return;
                // get container
                var constraintSet = BuildConstraintSet();
                Vector2D layerDim = new Vector2D(SelectedPallet.Length, SelectedPallet.Width) + 2 * constraintSet.Overhang;
                // get selected layer
                ILayer2D[] layers = uCtrlLayerListEdited.Selected;
                if (layers.Length != 1) return;
                Layer2DBrickExp layer = layers[0] as Layer2DBrickExp;
                using (var form = new FormEditLayer(layer, packable))
                {
                    form.TopMost = true;
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        _layersEdited.Remove(layer);
                        _layersEdited.Add(form.Layer);
                        FillEditedLayerList();                    
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion
        #region Helpers
        private ConstraintSetCasePallet BuildConstraintSet()
        {
            // constraint set
            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet()
            {
                Overhang = new Vector2D(uCtrlOverhang.ValueX, uCtrlOverhang.ValueY)
            };
            // orientations
            if (SelectedPackable is PackProperties || SelectedPackable is BagProperties)
                constraintSet.SetAllowedOrientations(new bool[] { false, false, true });
            else
                constraintSet.SetAllowedOrientations(uCtrlCaseOrientation.AllowedOrientations);
            // conditions
            constraintSet.SetMaxHeight( new OptDouble(true, uCtrlMaximumHeight.Value));
            constraintSet.OptMaxWeight = uCtrlOptMaximumWeight.Value;
            constraintSet.MinimumSpace = new OptDouble(uCtrlOptSpace.Value);
            return constraintSet;
        }
        #endregion
        #region Data members
        private List<Layer2DBrickExp> _layersEdited = new List<Layer2DBrickExp>(); 
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewAnalysisCasePallet));
        #endregion
    }
}
