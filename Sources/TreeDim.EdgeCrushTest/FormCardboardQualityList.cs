#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;

using log4net;
#endregion

namespace treeDiM.EdgeCrushTest
{
    public partial class FormCardboardQualityList : Form
    {
        #region Constructor
        public FormCardboardQualityList()
        {
            InitializeComponent();
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
                FillGrid();
        }
        #endregion

        #region Helpers
        private void FillGrid()
        {
            // remove all existing rows
            grid.Rows.Clear();
            // *** IViews
            // caption header
            var captionHeader = new SourceGrid.Cells.Views.RowHeader()
            {
                Background = new DevAge.Drawing.VisualElements.RowHeader()
                {
                    BackColor = Color.SteelBlue,
                    Border = DevAge.Drawing.RectangleBorder.NoBorder
                },
                ForeColor = Color.Black,
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter
            };
            // view column header
            var viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader()
            {
                Background = new DevAge.Drawing.VisualElements.ColumnHeader()
                {
                    BackColor = Color.LightGray,
                    Border = DevAge.Drawing.RectangleBorder.NoBorder
                },
                ForeColor = Color.Black,
                Font = new Font("Arial", 10, FontStyle.Regular),
            };
            viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
            // set first row
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.ColumnsCount = 8;
            grid.FixedRows = 1;
            grid.Rows.Insert(0);
            // header
            int iCol = 0;
            SourceGrid.Cells.ColumnHeader columnHeader;
            // name
            columnHeader = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_NAME)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            grid[0, iCol++] = columnHeader;
            // profile
            columnHeader = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_PROFILE)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            grid[0, iCol++] = columnHeader;
            // thickness
            columnHeader = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_THICKNESS)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            grid[0, iCol++] = columnHeader;
            // ECT
            columnHeader = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_ECT)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            grid[0, iCol++] = columnHeader;
            // RigidityX
            columnHeader = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_RIGIDITYX)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            grid[0, iCol++] = columnHeader;
            // RigidityY
            columnHeader = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_RIGIDITYY)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            grid[0, iCol++] = columnHeader;
            // edit
            columnHeader = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_EDIT)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            grid[0, iCol++] = columnHeader;
            // delete
            columnHeader = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_DELETE)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            grid[0, iCol++] = columnHeader;

            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);
            SourceGrid.Cells.Controllers.CustomEvents buttonEdit = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonEdit.Click += new EventHandler(OnEditItem);
            // ROWS
            int iIndex = 0;
            var dictQuality = CardboardQualityAccessor.Instance.CardboardQualities;
            foreach (var q in dictQuality)
            {
                grid.Rows.Insert(++iIndex);
                iCol = 0;

                var quality = q;
                grid[iIndex, iCol++] = new SourceGrid.Cells.Cell(quality.Name);
                grid[iIndex, iCol++] = new SourceGrid.Cells.Cell(quality.Profile);
                grid[iIndex, iCol++] = new SourceGrid.Cells.Cell($"{quality.Thickness:0.##}");
                grid[iIndex, iCol++] = new SourceGrid.Cells.Cell($"{quality.ECT:0.##}");
                grid[iIndex, iCol++] = new SourceGrid.Cells.Cell($"{quality.RigidityDX:0.##}");
                grid[iIndex, iCol++] = new SourceGrid.Cells.Cell($"{quality.RigidityDY:0.##}");
                grid[iIndex, iCol] = new SourceGrid.Cells.Button(Properties.Resources.ID_EDIT);
                grid[iIndex, iCol++].AddController(buttonEdit);
                grid[iIndex, iCol] = new SourceGrid.Cells.Button("")
                {
                    Image = Properties.Resources.Delete
                };
                grid[iIndex, iCol++].AddController(buttonDelete);
            }

            grid.AutoStretchColumnsToFitWidth = true;
            grid.AutoSizeCells();
            grid.Columns.StretchToFit();
        }
        private QualityData SelectedMaterial
        {
            get
            {
                SourceGrid.RangeRegion region = grid.Selection.GetSelectionRegion();
                int[] indexes = region.GetRowsIndex();
                // no selection -> exit
                if (indexes.Length == 0)
                    return null;
                return CardboardQualityAccessor.Instance.CardboardQualities[indexes[0] - 1];
            }
        }
        #endregion

        #region Event handlers
        private void OnNewCardboardQuality(object sender, EventArgs e)
        {
            var form = new FormEditCardboardQualityData() { Mode = FormEditCardboardQualityData.EMode.MODE_CREATE };
            if (DialogResult.OK == form.ShowDialog())
                CardboardQualityAccessor.Instance.AddQuality(form.QualityName, form.Profile, form.Thickness, form.ECT, form.StiffnessX, form.StiffnessY);
            FillGrid();
        }
        private void OnDeleteItem(object sender, EventArgs e)
        {
            SourceGrid.CellContext context = (SourceGrid.CellContext)sender;
            int iSel = context.Position.Row - 1;
            CardboardQualityAccessor.Instance.RemoveQuality(iSel);
            FillGrid();
        }
        private void OnEditItem(object sender, EventArgs e)
        {
            SourceGrid.CellContext context = (SourceGrid.CellContext)sender;
            int iSel = context.Position.Row - 1;
            QualityData q = CardboardQualityAccessor.Instance.CardboardQualities[iSel];
            var form = new FormEditCardboardQualityData()
            {
                Mode = FormEditCardboardQualityData.EMode.MODE_MODIFY,
                QualityName = q.Name,
                Profile = q.Profile,
                Thickness = q.Thickness,
                ECT = q.ECT,
                StiffnessX = q.RigidityDX,
                StiffnessY = q.RigidityDY
            };
            if (DialogResult.OK == form.ShowDialog())
            {
                CardboardQualityAccessor.Instance.RemoveQuality(iSel);
                CardboardQualityAccessor.Instance.AddQuality(
                    form.QualityName,
                    form.Profile,
                    form.Thickness,
                    form.ECT,
                    form.StiffnessX,
                    form.StiffnessY);
                FillGrid();
            }
        }
        private void OnUploadDefaultCardboardQualities(object sender, EventArgs e)
        {
            CardboardQualityAccessor.Instance.UploadAllDefaultCardboardQualities();
            FillGrid();
        }
        #endregion

        #region Data members
        private static readonly ILog _log = LogManager.GetLogger(typeof(FormCardboardQualityList));
        #endregion


    }
}
