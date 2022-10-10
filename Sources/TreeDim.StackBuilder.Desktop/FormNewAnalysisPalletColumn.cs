#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Graphics.Controls;

using treeDiM.StackBuilder.Desktop.Properties;

using log4net;
using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewAnalysisPalletColumn : FormNewAnalysis, IDrawingContainer, IItemBaseFilter
    {
        #region Constructor
        public FormNewAnalysisPalletColumn(Document doc, AnalysisPalletColumn analysisPalletColumn)
            : base(doc, analysisPalletColumn)
        {
            InitializeComponent();
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (null != _item)
            {
                tbName.Text = _item.Name;
                tbDescription.Text = _item.Description;
            }
            else if (null != _document)
            {
                tbName.Text = _document.GetValidNewAnalysisName(ItemDefaultName);
                tbDescription.Text = tbName.Text;                
            }
            // graphics 3D Control
            graphCtrl.DrawingContainer = this;
            // list of pallets
            cbInputPallet1.Initialize(_document, this, AnalysisCast?.PalletAnalyses[0]);
            cbInputPallet2.Initialize(_document, this, AnalysisCast?.PalletAnalyses[1]);

            OnInputChanged(this, e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
        public override void UpdateStatus(string message)
        {
            if (!Program.IsSubscribed)
                message = Resources.ID_GOPREMIUMORUNSELECT;
            base.UpdateStatus(message);
        }
        public override void OnNext()
        {
            try
            {
                AnalysisPalletColumn analysis = AnalysisCast;
                if (null == analysis)
                {
                    _item = _document.CreateNewAnalysisPalletColumn(
                        ItemName, ItemDescription
                        , LoadedPallet0, LoadedPallet1);
                }
                else
                {
                    analysis.ID.SetNameDesc(ItemName, ItemDescription);
                    analysis.SetPalletAnalysis(0, LoadedPallet0);
                    analysis.SetPalletAnalysis(1, LoadedPallet1);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            base.OnNext();
            Close();
        }
        #endregion
        #region IDrawingContainer implementation
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            if (null == LoadedPallet0 || null == LoadedPallet1) return;
            var analysis = new AnalysisPalletColumn(null,
                LoadedPallet0, LoadedPallet1);
            if (!analysis.HasValidSolution) return;

            var viewer = new ViewerSolutionPalletColumn(analysis.Solution);
            viewer.Draw(graphics, Transform3D.Identity);
        }
        #endregion
        #region FormNewBase override
        public override string ItemDefaultName => Resources.ID_ANALYSIS;
        #endregion
        #region IItemBaseFilter
        public bool Accept(Control ctrl, ItemBase itemBase) => (ctrl == cbInputPallet1 || ctrl == cbInputPallet2)  && itemBase is LoadedPallet;
        #endregion
        #region Public properties
        public AnalysisPalletColumn AnalysisCast => _item as AnalysisPalletColumn;
        private LoadedPallet LoadedPallet0 => cbInputPallet1.SelectedType as LoadedPallet;
        private LoadedPallet LoadedPallet1 => cbInputPallet2.SelectedType as LoadedPallet;
        #endregion
        #region Handlers
        private void OnInputChanged(object sender, EventArgs e)
        {
            graphCtrl.Invalidate();
        }
        #endregion
        #region Static data members
        private static readonly ILog _log = LogManager.GetLogger(typeof(FormNewAnalysisPalletColumn));
        #endregion
    }
}
