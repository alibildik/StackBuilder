#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;

using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public class DocumentSB : Document, IDocument
    {
        #region Constructor
        public DocumentSB(string filePath, IDocumentListener listener)
            :base(filePath, listener)
        {
            FilePath = filePath;
            _dirty = false;
        }
        public DocumentSB(string name, string description, string author, IDocumentListener listener)
            : base(name, description, author, DateTime.Now, listener)
        {
            _dirty = false;
        }
        #endregion

        #region Public properties
        public string FilePath { get; set; }
        #endregion

        #region IDocument implementation
        public bool IsDirty => _dirty;
        public bool IsNew => string.IsNullOrEmpty(FilePath); 
        public bool HasValidPath => Directory.Exists(Path.GetDirectoryName(FilePath)); 
        public void Save()
        {
            if (IsNew) return;
            if (!HasValidPath)
                throw new DirectoryNotFoundException($"Directory {Path.GetDirectoryName(FilePath)} could not be found!");
            Write(FilePath);
            _dirty = false;
        }

        public void SaveAs(string filePath)
        {
            FilePath = filePath;
            Save();
        }

        public void Close(CancelEventArgs e)
        {
            while (_views.Count > 0)
                RemoveView(_views[0]);
            base.Close();
        }

        public override void Modify()
        {
            _dirty = true;
            Modified?.Invoke(this, new EventArgs());
        }

        public List<IView> Views { get { return _views; } }

        public IView ActiveView
        {
            set
            {
                _activeView = value;
                _activeView.Activate();
            }
            get
            {
                return _activeView;
            }
        }

        public void AddView(IView view)
        {
            _views.Add(view);
        }

        public void RemoveView(IView view)
        {
            // remove from list of attached views
            _views.Remove(view);
        }
        #endregion

        #region View creation methods
        public DockContentView CreateViewAnalysis(Analysis analysis)
        {
            DockContentView form;
            if (analysis is AnalysisCasePallet) form = new DockContentAnalysisCasePallet(this, analysis as AnalysisCasePallet);
            else if (analysis is AnalysisBoxCase) form = new DockContentAnalysisBoxCase(this, analysis as AnalysisBoxCase);
            else if (analysis is AnalysisCylinderPallet) form = new DockContentAnalysisCylinderPallet(this, analysis as AnalysisCylinderPallet);
            else if (analysis is AnalysisCylinderCase) form = new DockContentAnalysisCylinderCase(this, analysis as AnalysisCylinderCase);
            else if (analysis is AnalysisPalletTruck) form = new DockContentAnalysisPalletTruck(this, analysis as AnalysisPalletTruck);
            else if (analysis is AnalysisCaseTruck) form = new DockContentAnalysisCaseTruck(this, analysis as AnalysisCaseTruck);
            else if (analysis is AnalysisCylinderTruck) form = new DockContentAnalysisCylinderTruck(this, analysis as AnalysisCylinderTruck);
            else if (analysis is AnalysisHCylPallet) form = new DockContentAnalysisHCylPallet(this, analysis as AnalysisHCylPallet);
            else if (analysis is AnalysisHCylTruck) form = new DockContentAnalysisHCylTruck(this, analysis as AnalysisHCylTruck);
            else if (analysis is AnalysisPalletsOnPallet) form = new DockContentAnalysisPalletsOnPallet(this, analysis as AnalysisPalletsOnPallet);
            else
            {
                _log.Error($"Analysis ({analysis.Name}) type not handled");
                return null;
            }
            AddView(form);
            return form;
        }
        public DockContentView CreateViewHAnalysis(AnalysisHetero analysis)
        {
            DockContentView form = null;
            if (analysis is HAnalysisPallet) form = new DockContentHAnalysisCasePallet(this, analysis);
            else if (analysis is HAnalysisTruck) form = new DockContentHAnalysisCaseTruck(this, analysis);
            else
            {
                _log.Error($"Analysis ({analysis.Name}) type not handled");
                return null;
            }
            AddView(form);
            return form;
        }
        #endregion

        #region UI item creation
        /// <summary>
        /// Creates a new BoxProperties object with MODE_BOX
        /// </summary>
        public void CreateNewBoxUI()
        {
            using (FormNewBox form = new FormNewBox(this, FormNewBox.Mode.BOX))
            {
                if (DialogResult.OK == form.ShowDialog())
                {
                    BoxProperties boxProperties = CreateNewBox(form.BoxName, form.Description
                        , form.BoxLength, form.BoxWidth, form.BoxHeight
                        , form.Weight, form.Colors);
                    boxProperties.Bulge = form.Bulge;
                    boxProperties.SetNetWeight(form.NetWeight);
                    boxProperties.TextureList = form.TextureList;
                    boxProperties.Facing = form.Facing;
                }
            }
        }
        /// <summary>
        /// Creates a new BoxProperties object with MODE_CASE
        /// </summary>
        public void CreateNewCaseUI()
        {
            using (FormNewBox form = new FormNewBox(this, FormNewBox.Mode.CASE))
            {
                if (DialogResult.OK == form.ShowDialog())
                {
                    BoxProperties boxProperties = CreateNewCase(form.BoxName, form.Description
                        , form.BoxLength, form.BoxWidth, form.BoxHeight
                        , form.InsideLength, form.InsideWidth, form.InsideHeight, form.HasInsideDimensions
                        , form.Weight, form.Colors);
                    boxProperties.Bulge = form.Bulge;
                    boxProperties.SetNetWeight(form.NetWeight);
                    boxProperties.TapeColor = form.TapeColor;
                    boxProperties.TapeWidth = form.TapeWidth;
                    boxProperties.TextureList = form.TextureList;
                    boxProperties.StrapperSet = form.StrapperSet;
                    boxProperties.Facing = form.Facing;
                 }
            }
        }
        public void CreateNewBagUI()
        {
            using (var form = new FormNewBag(this, null))
            {
                if (DialogResult.OK == form.ShowDialog())
                {
                    var bagProperties = CreateNewBag(
                        form.ItemName, form.ItemDescription
                        , form.OuterDimensions, form.RoundingRadius
                        , form.Weight, form.NetWeight
                        , form.ColorFill);
                    bagProperties.Bulge = form.Bulge;
                }
            }
        }
        /// <summary>
        /// Creates a new PackProperties object
        /// </summary>
        public void CreateNewPackUI()
        {
            using (FormNewPack form = new FormNewPack(this, null) { Boxes = Boxes.ToList() })
            {
                if (DialogResult.OK == form.ShowDialog())
                {
                    PackProperties packProperties = CreateNewPack(
                        form.ItemName, form.ItemDescription
                        , form.SelectedPackable
                        , form.Arrangement
                        , form.BoxOrientation, form.RevSolidLayout
                        , form.Wrapper
                        , form.Tray);
                    if (form.HasForcedOuterDimensions)
                        packProperties.ForceOuterDimensions(form.OuterDimensions);
                    packProperties.StrapperSet = form.StrapperSet;
                    packProperties.Bulge = form.Bulge;
                }
            }
        }
        public void CreateNewCylinderUI()
        {
            using (FormNewCylinder form = new FormNewCylinder(this, null))
            {
                if (DialogResult.OK == form.ShowDialog())
                {
                    CreateNewCylinder(
                        form.ItemName, form.ItemDescription
                        , form.RadiusOuter, form.RadiusInner, form.CylinderHeight
                        , form.Weight, form.NetWeight
                        , form.ColorTop, form.ColorWallOuter, form.ColorWallInner);
                }  
            }
        }
        public void CreateNewBottleUI()
        {
            using (FormNewBottle form = new FormNewBottle(this, null))
            {
                if (DialogResult.OK == form.ShowDialog())
                    CreateNewBottle(
                        form.ItemName, form.ItemDescription
                        , form.Profile, form.Weight, form.NetWeight
                        , form.ColorBottle
                        );
            }
        }
        /// <summary>
        /// Creates a new BundleProperties object
        /// </summary>
        public void CreateNewBundleUI()
        {
            using (FormNewBundle form = new FormNewBundle(this))
            {
                if (DialogResult.OK == form.ShowDialog())
                    CreateNewBundle(
                        form.BundleName, form.Description
                        , form.BundleLength, form.BundleWidth, form.UnitThickness
                        , form.UnitWeight
                        , form.Color
                        , form.NoFlats);
            }
        }
        /// <summary>
        /// Creates a new InterlayerProperties object
        /// </summary>
        public void CreateNewInterlayerUI()
        {
            using (FormNewInterlayer form = new FormNewInterlayer(this, null))
            {
                if (DialogResult.OK == form.ShowDialog())
                    CreateNewInterlayer(
                        form.ItemName, form.ItemDescription
                        , form.InterlayerLength, form.InterlayerWidth, form.Thickness
                        , form.Weight
                        , form.Color);
            }
        }
        /// <summary>
        /// Creates new pallet corners
        /// </summary>
        public void CreateNewPalletCornersUI()
        {
            using (FormNewPalletCorners form = new FormNewPalletCorners(this, null))
            {
                if (DialogResult.OK == form.ShowDialog())
                    CreateNewPalletCorners(form.ItemName, form.ItemDescription,
                        form.CornerLength, form.CornerWidth, form.CornerThickness,
                        form.CornerWeight,
                        form.CornerColor);
            }
        }
        /// <summary>
        /// Creates a new pallet cap
        /// </summary>
        public void CreateNewPalletCapUI()
        {
            using (FormNewPalletCap form = new FormNewPalletCap(this, null))
            {
                if (DialogResult.OK == form.ShowDialog())
                    CreateNewPalletCap(form.ItemName, form.ItemDescription,
                        form.CapLength, form.CapWidth, form.CapHeight,
                        form.CapInnerLength, form.CapInnerWidth, form.CapInnerHeight,
                        form.CapWeight,
                        form.CapColor);
            }
        }
        /// <summary>
        /// Creates new pallet film
        /// </summary>
        public void CreateNewPalletFilmUI()
        {
            using (var form = new FormNewPalletFilm(this, null))
            {
                if (DialogResult.OK == form.ShowDialog())
                    CreateNewPalletFilm(form.ItemName, form.ItemDescription,
                        form.UseTransparency,
                        form.UseHatching, form.HatchSpacing, form.HatchAngle,
                        form.Weight,
                        form.FilmColor);
            }
        }
        public void CreateNewPalletLabelUI()
        {
            using (var form = new FormNewPalletLabel(this, null))
            {
                if (DialogResult.OK == form.ShowDialog())
                    CreateNewPalletLabel(form.ItemName, form.ItemDescription,
                        form.Dimensions,
                        form.Weight,
                        form.Color,
                        form.Bitmap);
            }
        }
        /// <summary>
        /// creates a new PalletProperties object
        /// </summary>
        public void CreateNewPalletUI()
        {
            using (FormNewPallet form = new FormNewPallet(this, null))
            {
                if (DialogResult.OK == form.ShowDialog())
                    CreateNewPallet(form.ItemName, form.ItemDescription
                        , form.PalletTypeName
                        , form.PalletLength, form.PalletWidth, form.PalletHeight
                        , form.Weight
                        , form.PalletColor);
            }
        }
        /// <summary>
        /// Creates a new TruckProperties
        /// </summary>
        public void CreateNewTruckUI()
        {
            using (FormNewTruck form = new FormNewTruck(this, null))
            {
                if (DialogResult.OK == form.ShowDialog())
                    CreateNewTruck(form.ItemName, form.ItemDescription
                        , form.TruckLength, form.TruckWidth, form.TruckHeight
                        , form.TruckAdmissibleLoadWeight
                        , form.TruckColor);
            }
        }
        /// <summary>
        /// Creates a new palet analysis
        /// </summary>
        /// <returns>created palet analysis</returns>
        public void CreateNewAnalysisCasePalletUI()
        {
            if (!CanCreateAnalysisCasePallet && !CanCreateAnalysisBundlePallet) return;
            if (Properties.Settings.Default.DummyMode)
            {
                using (var form = new FormNewAnalysisCasePalletDM(this, null))
                    if (DialogResult.OK == form.ShowDialog()) { } 
            }
            else
            {
                using (var form = new FormNewAnalysisCasePallet(this, null))
                    if (DialogResult.OK == form.ShowDialog()) { } 
            }
        }
        public void CreateNewAnalysisBoxCaseUI()
        {
            if (!CanCreateAnalysisBoxCase && !CanCreateAnalysisBundleCase) return;
            using (var form = new FormNewAnalysisBoxCase(this, null))
                if (DialogResult.OK == form.ShowDialog()) { } 
        }
        public void CreateNewAnalysisCylinderPalletUI()
        {
            if (!CanCreateAnalysisCylinderPallet) return;
            using (var form = new FormNewAnalysisCylinderPallet(this, null))
                if (DialogResult.OK == form.ShowDialog()) { }
        }
        public void CreateNewAnalysisHCylPalletUI()
        {
            if (!CanCreateAnalysisCylinderPallet) return;
            using (var form = new FormNewAnalysisHCylPallet(this, null))
                if (DialogResult.OK == form.ShowDialog()) { }
        }
        public void CreateNewAnalysisCylinderCaseUI()
        {
            if (!CanCreateAnalysisCylinderCase) return;
            using (var form = new FormNewAnalysisCylinderCase(this, null))
                if (DialogResult.OK == form.ShowDialog()) { }
        }
        public void CreateNewPalletsOnPalletUI()
        {
            if (!CanCreatePalletsOnPallet) return;
            using (var form = new FormNewAnalysisPalletsOnPallet(this, null))
                if (DialogResult.OK == form.ShowDialog()) { }
        }
        public void CreateNewAnalysisPalletTruckUI()
        {
            if (!CanCreateAnalysisPalletTruck) return;
            using (var form = new FormNewAnalysisPalletTruck(this, null))
                if (DialogResult.OK == form.ShowDialog()) { }
        }
        public void CreateNewAnalysisHPalletTruckUI()
        {
            if (!CanCreateAnalysisPalletTruck) return;
            using (var form = new FormNewHAnalysisPalletTruck(this, null))
                if (DialogResult.OK == form.ShowDialog()) { }
        }
        public void CreateNewAnalysisCaseTruckUI()
        {
            if (!CanCreateAnalysisCaseTruck) return;
            using (var form = new FormNewAnalysisCaseTruck(this, null))
                if (DialogResult.OK == form.ShowDialog()) { }
        }
        public AnalysisCylinderTruck CreateNewAnalysisCylinderTruckUI()
        {
            if (!CanCreateAnalysisCylinderTruck) return null;
            using (FormNewAnalysisCylinderTruck form = new FormNewAnalysisCylinderTruck(this, null))
                if (DialogResult.OK == form.ShowDialog()) { }
            return null;
        }
        public AnalysisHCylTruck CreateNewAnalysisHCylinderTruckUI()
        {
            if (!CanCreateAnalysisCylinderTruck) return null;
            using (FormNewAnalysisHCylTruck form = new FormNewAnalysisHCylTruck(this, null))
                if (DialogResult.OK == form.ShowDialog()) { }
            return null;
        }
        public void CreateNewHAnalysisPalletUI()
        {
            if (!CanCreateAnalysisCasePallet) return;
            using (FormNewHAnalysis form = new FormNewHAnalysisCasePallet(this, null))
                if (DialogResult.OK == form.ShowDialog()) { }
        }
        public void CreateNewHAnalysisTruckUI()
        {
            if (!CanCreateAnalysisCaseTruck) return;
            using (FormNewHAnalysis form = new FormNewHAnalysisCaseTruck(this, null))
                if (DialogResult.OK == form.ShowDialog()) { }
        }
        public void ExportAnalysesToExcel()
        {
            // open excel file
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application
            {
                Visible = true,
                DisplayAlerts = false
            };
            Workbooks xlWorkBooks = xlApp.Workbooks;
            Workbook xlWorkBook = xlWorkBooks.Add(Type.Missing);
            Worksheet xlWorkSheetCasePallet = xlWorkBook.Worksheets.get_Item(1)as Worksheet;

            // create header
            xlWorkSheetCasePallet.Cells[1, 1] = "Analysis name";
            xlWorkSheetCasePallet.Cells[1, 2] = "Case name";
            xlWorkSheetCasePallet.Cells[1, 3] = "Case description";
            xlWorkSheetCasePallet.Cells[1, 4] = "Ext. length";
            xlWorkSheetCasePallet.Cells[1, 5] = "Ext. width";
            xlWorkSheetCasePallet.Cells[1, 6] = "Ext. height";
            xlWorkSheetCasePallet.Cells[1, 7] = "Max pallet height";
            xlWorkSheetCasePallet.Cells[1, 8] = "Solution case count";
            xlWorkSheetCasePallet.Cells[1, 9] = "Layers";
            xlWorkSheetCasePallet.Cells[1, 10] = "Cases per layer";
            xlWorkSheetCasePallet.Cells[1, 11] = "Load weight";
            xlWorkSheetCasePallet.Cells[1, 12] = "Weight";
            xlWorkSheetCasePallet.Cells[1, 13] = "Volume efficiency";
            xlWorkSheetCasePallet.Cells[1, 14] = "Image";

            Range headerRange = xlWorkSheetCasePallet.get_Range("A1", "N1");
            headerRange.Font.Bold = true;
            xlWorkSheetCasePallet.Columns.AutoFit();


            // *** get all users from Azure database and write them down
            int iRowCasePallet = 2;

            foreach (var analysis in Analyses)
            {
                try
                {
                    if (analysis is AnalysisCasePallet analysisCasePallet)
                    {
                        // analysis name
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 1] = analysisCasePallet.Name;
                        // case
                        BoxProperties caseProperties = analysisCasePallet.Content as BoxProperties;
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 2] = caseProperties.Name;
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 3] = caseProperties.Description;
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 4] = caseProperties.Length;
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 5] = caseProperties.Width;
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 6] = caseProperties.Height;
                        // constraints
                        ConstraintSetCasePallet constraintSet = analysisCasePallet.ConstraintSet as ConstraintSetCasePallet;
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 7] = constraintSet.OptMaxHeight.Value;
                        // solution
                        SolutionLayered sol = analysisCasePallet.SolutionLay;
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 8] = sol.ItemCount;
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 9] = sol.LayerCount;
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 10] = sol.LayerBoxCount(0);
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 11] = sol.LoadWeight;
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 12] = sol.Weight;
                        xlWorkSheetCasePallet.Cells[iRowCasePallet, 13] = sol.VolumeEfficiency;

                        var stackImagePath = Path.Combine(Path.ChangeExtension(Path.GetTempFileName(), "png"));
                        var graphics = new Graphics3DImage(new Size(768, 768))
                        {
                            FontSizeRatio = 0.01f,
                            CameraPosition = Graphics3D.Corner_0
                        };
                        using (ViewerSolution sv = new ViewerSolution(analysisCasePallet.SolutionLay))
                            sv.Draw(graphics, Transform3D.Identity);
                        graphics.Flush();
                        Bitmap bmp = graphics.Bitmap;
                        bmp.Save(stackImagePath, System.Drawing.Imaging.ImageFormat.Png);
                        Range imageCell = (Range)xlWorkSheetCasePallet.Cells[iRowCasePallet, 14];
                        imageCell.RowHeight = 128;
                        imageCell.ColumnWidth = 24;
                        xlWorkSheetCasePallet.Shapes.AddPicture(stackImagePath,
                            LinkToFile: MsoTriState.msoFalse, SaveWithDocument: MsoTriState.msoCTrue,
                            Left: (int)imageCell.Left + 1, Top: (int)imageCell.Top + 1, Width: (int)imageCell.Width - 2, Height: (int)imageCell.Height - 2);

                        ++iRowCasePallet;
                    }
                    else if (analysis is AnalysisBoxCase analysisBoxCase)
                    {
                    }
                    else if (analysis is AnalysisCaseTruck analysisCaseTruck)
                    {
                    }
                    else if (analysis is AnalysisCylinderPallet analysisCylinderPallet)
                    {
                    }
                    else if (analysis is AnalysisCylinderCase analysisCylinderCase)
                    {
                    }
                    else if (analysis is AnalysisCylinderTruck analysisCylinderTruck)
                    {
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                }
            }
        }
        #endregion

        #region UI item edition
        public void EditAnalysis(AnalysisHomo analysis)
        {
            // search for any DockContentAnalysis window and close it
            var seq = (from view in Views
                       where view is DockContentAnalysisEdit && analysis == (view as DockContentAnalysisEdit).Analysis
                       select view);
            if (seq.Count() > 0) seq.First().Close();

            // instantiate a form to edit analysis
            Form form = null;
            if (analysis is AnalysisCasePallet analysisCasePallet) form = new FormNewAnalysisCasePallet(this, analysisCasePallet);
            else if (analysis is AnalysisBoxCase analysisBoxCase) form = new FormNewAnalysisBoxCase(this, analysisBoxCase);
            else if (analysis is AnalysisCylinderPallet analysisCylinderPallet) form = new FormNewAnalysisCylinderPallet(this, analysisCylinderPallet);
            else if (analysis is AnalysisCylinderCase analysisCylinderCase) form = new FormNewAnalysisCylinderCase(this, analysisCylinderCase);
            else if (analysis is AnalysisPalletTruck analysisPalletTruck) form = new FormNewAnalysisPalletTruck(this, analysisPalletTruck);
            else if (analysis is AnalysisCaseTruck analysisCaseTruck) form = new FormNewAnalysisCaseTruck(this, analysisCaseTruck);
            else if (analysis is AnalysisCylinderTruck analysisCylinderTruck) form = new FormNewAnalysisCylinderTruck(this, analysisCylinderTruck);
            else if (analysis is AnalysisHCylPallet analysisHCylPallet) form = new FormNewAnalysisHCylPallet(this, analysisHCylPallet);
            else if (analysis is AnalysisHCylTruck analysisHCylTruck) form = new FormNewAnalysisHCylTruck(this, analysisHCylTruck);
            else
            {
               _log.Error($"Unexepected analysis type = {analysis.GetType()}");
                return;
            }
            if (DialogResult.OK == form.ShowDialog())
                Modify();
        }
        public void EditAnalysis(AnalysisHetero analysis)
        {
            // search for DockContentHAnalysis window and close it
            var seq = (from view in Views
                       where view is DockContentHAnalysisCasePallet && (analysis == (view as DockContentHAnalysisCasePallet).Analysis)
                       select view);
            if (seq.Count() > 0) seq.First().Close();

            // instantiate a form to edit analysis
            Form form = null;
            if (analysis is HAnalysisPallet) form = new FormNewHAnalysisCasePallet(this, analysis);
            else if (analysis is HAnalysisCase) form = new FormNewHAnalysisBoxCase(this, analysis);
            else if (analysis is HAnalysisTruck) form = new FormNewHAnalysisCaseTruck(this, analysis);
            else
            {
                _log.Error($"Unexpected analysis type = {analysis.GetType()}");
                return;
            }
            if (DialogResult.OK == form.ShowDialog())
                Modify();
        }
        public void EditAnalysis(AnalysisPalletsOnPallet analysis)
        {
            // search for DockContentHAnalysis window and close it
            var seq = (from view in Views
                       where view is DockContentAnalysisPalletsOnPallet && (analysis == (view as DockContentAnalysisPalletsOnPallet).Analysis)
                       select view);
            if (seq.Count() > 0) seq.First().Close();

            Form form = new FormNewAnalysisPalletsOnPallet(this, analysis);
            if (DialogResult.OK == form.ShowDialog())
                Modify();
        }
        #endregion

        #region Object override
        public override string ToString() => Name;

        #endregion

        #region Data members
        private bool _dirty = false;
        private List<IView> _views = new List<IView>();
        private IView _activeView;
        public event EventHandler Modified;
        #endregion
    }
}
