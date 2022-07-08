#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;

using treeDiM.PLMPack.DBClient;
using treeDiM.PLMPack.DBClient.PLMPackSR;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormShowDatabase : Form, IDrawingContainer
    {
        #region Constructor
        public FormShowDatabase()
        {
            InitializeComponent();
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            graphCtrl.DrawingContainer = this;

            bnImport.Enabled = FormMain.GetInstance().HasDocuments;

            gridPallets.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            gridPalletCorners.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            gridPalletCaps.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            gridPalletFilms.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            gridBoxes.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            gridCases.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            gridBundles.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            gridCylinders.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            gridInterlayers.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            gridTrucks.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            gridBags.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);
            gridBottles.Selection.SelectionChanged += new SourceGrid.RangeRegionChangedEventHandler(OnSelChangeGrid);

            tabCtrlDBItems.SelectedIndex = 0;
            OnSelectedTabChanged(this, null);
        }
        #endregion

        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            try
            {
                if (null == _selectedItem)
                    return;
                // get unit system
                UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)_selectedItem.UnitSystem;
                // pallet
                if (_selectedItem is DCSBPallet dcsbPallet)
                {
                    double palletLength = UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M0, us);
                    double palletWidth = UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M1, us);
                    double palletHeight = UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M2, us);

                    PalletProperties palletProperties = new PalletProperties(null, dcsbPallet.PalletType, palletLength, palletWidth, palletHeight)
                    {
                        Color = Color.FromArgb(dcsbPallet.Color)
                    };
                    Pallet pallet = new Pallet(palletProperties);
                    pallet.Draw(graphics, Transform3D.Identity);
                    graphics.AddDimensions(new DimensionCube(palletLength, palletWidth, palletHeight));
                }
                // pallet cap
                if (_selectedItem is DCSBPalletCap dcsbCap)
                {
                    double length = UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsOuter.M0, us);
                    double width = UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsOuter.M1, us);
                    double height = UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsOuter.M2, us);
                    double innerLength = UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsInner.M0, us);
                    double innerWidth = UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsInner.M1, us);
                    double innerHeight = UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsInner.M2, us);
                    double weight = UnitsManager.ConvertMassFrom(dcsbCap.Weight, us);

                    PalletCapProperties palletCapProperties = new PalletCapProperties(
                        null, dcsbCap.Name, dcsbCap.Description, length, width, height,
                        innerLength, innerWidth, innerHeight,
                        weight, Color.FromArgb(dcsbCap.Color));
                    PalletCap palletCap = new PalletCap(0, palletCapProperties, BoxPosition.Zero);
                    palletCap.Draw(graphics);
                    graphics.AddDimensions(new DimensionCube(length, width, height));
                }
                // pallet corner
                if (_selectedItem is DCSBPalletCorner dcsbCorner)
                {
                    double length = UnitsManager.ConvertLengthFrom(dcsbCorner.Length, us);
                    double width = UnitsManager.ConvertLengthFrom(dcsbCorner.Width, us);
                    double thickness = UnitsManager.ConvertLengthFrom(dcsbCorner.Thickness, us);
                    double weight = UnitsManager.ConvertMassFrom(dcsbCorner.Weight, us);

                    PalletCornerProperties palletCornerProperties = new PalletCornerProperties(
                        null, dcsbCorner.Name, dcsbCorner.Description,
                        length, width, thickness,
                        weight, Color.FromArgb(dcsbCorner.Color));
                    Corner palletCorner = new Corner(0, palletCornerProperties);
                    palletCorner.Draw(graphics);
                    graphics.AddDimensions(new DimensionCube(width, width, length));
                }
                // pallet film
                if (_selectedItem is DCSBPalletFilm dcsbFilm)
                {
                }
                // interlayer
                if (_selectedItem is DCSBInterlayer dcsbInterlayer)
                {
                    double length = UnitsManager.ConvertLengthFrom(dcsbInterlayer.Dimensions.M0, us);
                    double width = UnitsManager.ConvertLengthFrom(dcsbInterlayer.Dimensions.M1, us);
                    double thickness = UnitsManager.ConvertLengthFrom(dcsbInterlayer.Dimensions.M2, us);
                    double weight = UnitsManager.ConvertMassFrom(dcsbInterlayer.Weight, us);

                    InterlayerProperties interlayerProperties = new InterlayerProperties(
                        null, dcsbInterlayer.Name, dcsbInterlayer.Description
                        , length, width, thickness
                        , weight, Color.FromArgb(dcsbInterlayer.Color));
                    Box box = new Box(0, interlayerProperties);
                    graphics.AddBox(box);
                    graphics.AddDimensions(new DimensionCube(length, width, thickness));
                }
                // case
                if (_selectedItem is DCSBCase dcsbCase)
                {
                    double length = UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M0, us);
                    double width = UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M1, us);
                    double height = UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M2, us);
                    Color[] faceColors = new Color[6];
                    for (int i = 0; i < 6; ++i)
                        faceColors[i] = Color.FromArgb(dcsbCase.Colors[i]);
                    List<Pair<HalfAxis.HAxis, Texture>> textures = new List<Pair<HalfAxis.HAxis, Texture>>();

                    BoxProperties boxProperties = new BoxProperties(null, length, width, height);
                    boxProperties.SetAllColors(faceColors);
                    boxProperties.TextureList = textures;
                    boxProperties.TapeWidth = new OptDouble(dcsbCase.ShowTape, UnitsManager.ConvertLengthFrom(dcsbCase.TapeWidth, us));
                    boxProperties.TapeColor = Color.FromArgb(dcsbCase.TapeColor);
                    Box box = new Box(0, boxProperties);
                    graphics.AddBox(box);
                    graphics.AddDimensions(new DimensionCube(length, width, height));
                }
                // bundle
                if (_selectedItem is DCSBBundle dcsbBundle)
                {
                    double length = UnitsManager.ConvertLengthFrom(dcsbBundle.DimensionsUnit.M0, us);
                    double width = UnitsManager.ConvertLengthFrom(dcsbBundle.DimensionsUnit.M1, us);
                    double unitThickness = UnitsManager.ConvertLengthFrom(dcsbBundle.DimensionsUnit.M2, us);

                    BundleProperties bundleProperties = new BundleProperties(
                        null, dcsbBundle.Name, dcsbBundle.Description
                        , length, width, unitThickness
                        , dcsbBundle.UnitWeight, dcsbBundle.Number
                        , Color.FromArgb(dcsbBundle.Color));
                    Box box = new Box(0, bundleProperties);
                    graphics.AddBox(box);
                    graphics.AddDimensions(new DimensionCube(length, width, unitThickness * dcsbBundle.Number));
                }
                // cylinder
                if (_selectedItem is DCSBCylinder dcsbCylinder)
                {
                    double height = UnitsManager.ConvertLengthFrom(dcsbCylinder.Height, us);
                    double radiusOuter = UnitsManager.ConvertLengthFrom(dcsbCylinder.RadiusOuter, us);
                    double radiusInner = UnitsManager.ConvertLengthFrom(dcsbCylinder.RadiusInner, us);
                    double weight = UnitsManager.ConvertMassFrom(dcsbCylinder.Weight, us);

                    CylinderProperties cylProperties = new CylinderProperties(
                        null, dcsbCylinder.Name, dcsbCylinder.Description
                        , radiusOuter, radiusInner, height, weight
                        , Color.FromArgb(dcsbCylinder.ColorTop)
                        , Color.FromArgb(dcsbCylinder.ColorOuter)
                        , Color.FromArgb(dcsbCylinder.ColorInner)
                        );
                    Cylinder cyl = new Cylinder(0, cylProperties);
                    graphics.AddCylinder(cyl);
                    graphics.AddDimensions(new DimensionCube(new Vector3D(-radiusOuter, -radiusOuter, 0.0), 2.0 * radiusOuter, 2.0 * radiusOuter, height, Color.Black, false));
                }
                // truck
                if (_selectedItem is DCSBTruck dcsbtruck)
                {
                    double truckLength = UnitsManager.ConvertLengthFrom(dcsbtruck.DimensionsInner.M0, us);
                    double truckWidth = UnitsManager.ConvertLengthFrom(dcsbtruck.DimensionsInner.M1, us);
                    double truckHeight = UnitsManager.ConvertLengthFrom(dcsbtruck.DimensionsInner.M2, us);
                    TruckProperties truckProperties = new TruckProperties(null, truckLength, truckWidth, truckHeight)
                    {
                        Color = Color.FromArgb(dcsbtruck.Color)
                    };
                    Truck truck = new Truck(truckProperties);
                    truck.DrawBegin(graphics);
                    truck.DrawEnd(graphics);
                    graphics.AddDimensions(new DimensionCube(truckLength, truckWidth, truckHeight));
                }
                // bag
                if (_selectedItem is DCSBBag dcsbBag)
                {
                    double length = UnitsManager.ConvertLengthFrom(dcsbBag.DimensionsOuter.M0, us);
                    double width = UnitsManager.ConvertLengthFrom(dcsbBag.DimensionsOuter.M1, us);
                    double height = UnitsManager.ConvertLengthFrom(dcsbBag.DimensionsOuter.M2, us);
                    double radius = UnitsManager.ConvertLengthFrom(dcsbBag.Radius, us);

                    var bagProperties = new BagProperties(null, dcsbBag.Name, dcsbBag.Description, length, width, height, radius)
                    {
                        ColorFill = Color.FromArgb(dcsbBag.Color)
                    };
                    var bag = new BoxRounded(0, bagProperties, BoxPosition.Zero);
                    graphics.AddBox(bag);
                    graphics.AddDimensions(new DimensionCube(Vector3D.Zero, length, width, height, Color.Black, false));
                }
                // bottle
                if (_selectedItem is DCSBBottle dcsbBottle)
                {
                    var profile = new List<Vector2D>();
                    double diameter = 0.0;
                    double height = 0.0;
                    foreach (var t in dcsbBottle.Diameters)
                    {
                        height = Math.Max(height, t.Item1);
                        diameter = Math.Max(diameter, t.Item2);
                        profile.Add(new Vector2D(UnitsManager.ConvertLengthFrom(t.Item1, us), UnitsManager.ConvertLengthFrom(t.Item2, us)));
                    }
                    var bottleProp = new BottleProperties(
                        null, dcsbBottle.Name, dcsbBottle.Description,
                        profile,
                        UnitsManager.ConvertMassFrom(dcsbBottle.Weight, us),
                        Color.FromArgb(dcsbBottle.Color));
                    var bottle = new Bottle(0, bottleProp);
                    graphics.AddCylinder(bottle);
                    graphics.AddDimensions(new DimensionCube(new Vector3D(-0.5 * diameter, -0.5 * diameter, 0.0), diameter, diameter, height, Color.Black, false));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region Private properties
        private Document Document
        {
            get
            {
                if (null == _doc)
                    _doc = FormMain.GetInstance().ActiveDocumentSB;
                return _doc;
            }
        }
        private DCSBTypeEnum CurrentType
        {
            get
            {
                string tabName = tabCtrlDBItems.SelectedTab.Name;
                if (string.Equals(tabName, "tabPagePallet")) return DCSBTypeEnum.TPallet;
                else if (string.Equals(tabName, "tabPagePalletCorner")) return DCSBTypeEnum.TPalletCorner;
                else if (string.Equals(tabName, "tabPagePalletCap")) return DCSBTypeEnum.TPalletCap;
                else if (string.Equals(tabName, "tabPagePalletFilm")) return DCSBTypeEnum.TPalletFilm;
                else if (string.Equals(tabName, "tabPageInterlayer")) return DCSBTypeEnum.TInterlayer;
                else if (string.Equals(tabName, "tabPageBundle")) return DCSBTypeEnum.TBundle;
                else if (string.Equals(tabName, "tabPageBox")) return DCSBTypeEnum.TCase;
                else if (string.Equals(tabName, "tabPageCase")) return DCSBTypeEnum.TCase;
                else if (string.Equals(tabName, "tabPageCylinder")) return DCSBTypeEnum.TCylinder;
                else if (string.Equals(tabName, "tabPageTruck")) return DCSBTypeEnum.TTruck;
                else throw new Exception("Invalid type!");
            }
        }
        #endregion 

        #region Data grids
        #region Helpers
        private void GridInitialize(SourceGrid.Grid grid, List<string> captions)
        {
            // remove all existing rows
            grid.Rows.Clear();
            // *** IViews 
            // captionHeader
            SourceGrid.Cells.Views.RowHeader captionHeader = new SourceGrid.Cells.Views.RowHeader();
            DevAge.Drawing.VisualElements.RowHeader veHeaderCaption = new DevAge.Drawing.VisualElements.RowHeader()
            {
                BackColor = Color.SteelBlue,
                Border = DevAge.Drawing.RectangleBorder.NoBorder
            };
            captionHeader.Background = veHeaderCaption;
            captionHeader.ForeColor = Color.Black;
            captionHeader.Font = new Font("Arial", 10, FontStyle.Bold);
            captionHeader.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            // viewColumnHeader
            SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader();
            DevAge.Drawing.VisualElements.ColumnHeader backHeader = new DevAge.Drawing.VisualElements.ColumnHeader()
            {
                BackColor = Color.LightGray,
                Border = DevAge.Drawing.RectangleBorder.NoBorder
            };
            viewColumnHeader.Background = backHeader;
            viewColumnHeader.ForeColor = Color.Black;
            viewColumnHeader.Font = new Font("Arial", 10, FontStyle.Regular);
            viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
            // viewNormal
            CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
            // ***
            // set first row
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.ColumnsCount = 4 + captions.Count;
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
            // description
            columnHeader = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_DESCRIPTION)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            grid[0, iCol++] = columnHeader;
            // listed captions
            foreach (string s in captions)
            {
                columnHeader = new SourceGrid.Cells.ColumnHeader(s)
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                grid[0, iCol++] = columnHeader;
            }
            // auto import
            columnHeader = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_AUTOIMPORT)
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
        }
        private void GridFinalize(SourceGrid.Grid grid)
        {
            grid.AutoStretchColumnsToFitWidth = true;
            grid.AutoSizeCells();
            grid.Columns.StretchToFit();

            // select first solution
            if (grid.RowsCount > 1)
                grid.Selection.SelectRow(1, true);
            else
            {
                // grid empty -> clear drawing
                _selectedItem = null;
                graphCtrl.Invalidate();
            }
        }
        private void UpdateGrid()
        {
            try
            {
                _numberOfItems = 0;

                string tabName = tabCtrlDBItems.SelectedTab.Name;
                using (WCFClient wcfClient = new WCFClient())
                {
                    if (string.Equals(tabName, "tabPagePallet"))
                        FillGridPallets(wcfClient);
                    else if (string.Equals(tabName, "tabPagePalletCorner"))
                        FillGridPalletCorners(wcfClient);
                    else if (string.Equals(tabName, "tabPagePalletCap"))
                        FillGridPalletCaps(wcfClient);
                    else if (string.Equals(tabName, "tabPagePalletFilm"))
                        FillGridPalletFilms(wcfClient);
                    else if (string.Equals(tabName, "tabPageInterlayer"))
                        FillGridInterlayers(wcfClient);
                    else if (string.Equals(tabName, "tabPageBundle"))
                        FillGridBundles(wcfClient);
                    else if (string.Equals(tabName, "tabPageBox"))
                        FillGridBoxes(wcfClient);
                    else if (string.Equals(tabName, "tabPageCase"))
                        FillGridCases(wcfClient);
                    else if (string.Equals(tabName, "tabPageCylinder"))
                        FillGridCylinders(wcfClient);
                    else if (string.Equals(tabName, "tabPageTruck"))
                        FillGridTrucks(wcfClient);
                    else if (string.Equals(tabName, "tabPageBags"))
                        FillGridBags(wcfClient);
                    else if (string.Equals(tabName, "tabPageBottles"))
                        FillGridBottles(wcfClient);
                }
                UpdateButtons();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private void UpdateButtons()
        {
            // update buttons
            bnPrev.Enabled = RangeIndex > 0;
            bnNext.Enabled = (RangeIndex + 1) * 20 < _numberOfItems;
            lbCount.Text = string.Format(Properties.Resources.ID_DATABASEITEMCOUNT
                , (RangeIndex * 20) + 1
                , Math.Min((RangeIndex + 1) * 20, _numberOfItems)
                , _numberOfItems);
        }
        private string SearchString => tbSearch.Text.Trim();
        private bool SearchDescription { get => chkbSearchDescription.Checked; set => chkbSearchDescription.Checked = value; }
        #endregion
        #region Pallets
        void FillGridPallets(WCFClient wcfClient)
        {
            // initialize grid
            List<string> captions = new List<string>
            {
                string.Format(Properties.Resources.ID_DIMENSIONS, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_WEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_MASS)),
                string.Format(Properties.Resources.ID_ADMISSIBLELOAD_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_MASS))
            };
            GridInitialize(gridPallets, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);

            _pallets = wcfClient.Client.GetAllPalletsSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBPallet p in _pallets)
            {
                UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)p.UnitSystem;

                gridPallets.Rows.Insert(++iIndex);
                int iCol = 0;
                gridPallets[iIndex, iCol++] = new SourceGrid.Cells.Cell(p.Name);
                gridPallets[iIndex, iCol++] = new SourceGrid.Cells.Cell(p.Description);
                gridPallets[iIndex, iCol++] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##} x {1:0.##} x {2:0.##}",
                        UnitsManager.ConvertLengthFrom(p.Dimensions.M0, us),
                        UnitsManager.ConvertLengthFrom(p.Dimensions.M1, us),
                        UnitsManager.ConvertLengthFrom(p.Dimensions.M2, us))
                    );
                gridPallets[iIndex, iCol++] = new SourceGrid.Cells.Cell(
                       string.Format("{0:0.###}", UnitsManager.ConvertMassFrom(p.Weight, us))
                    );
                gridPallets[iIndex, iCol++] = new SourceGrid.Cells.Cell(p.AdmissibleLoad.HasValue ? string.Format("{0:0.###}", UnitsManager.ConvertMassFrom(p.AdmissibleLoad.Value, us)) : "-");
                gridPallets[iIndex, iCol] = new SourceGrid.Cells.CheckBox(null, p.AutoInsert);
                gridPallets[iIndex, iCol++].AddController(checkBoxEvent);
                gridPallets[iIndex, iCol] = new SourceGrid.Cells.Button("") { Image = Properties.Resources.Delete };
                gridPallets[iIndex, iCol++].AddController(buttonDelete);
            }
            GridFinalize(gridPallets);
        }
        #endregion
        #region Interlayers
        private void FillGridInterlayers(WCFClient wcfClient)
        {
            // initialize grid
            List<string> captions = new List<string>
            {
                string.Format(Properties.Resources.ID_DIMENSIONS, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_WEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_MASS))
            };
            GridInitialize(gridInterlayers, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);

            _interlayers = wcfClient.Client.GetAllInterlayersSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBInterlayer i in _interlayers)
            {
                UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)i.UnitSystem;

                gridInterlayers.Rows.Insert(++iIndex);
                int iCol = 0;
                gridInterlayers[iIndex, iCol++] = new SourceGrid.Cells.Cell(i.Name);
                gridInterlayers[iIndex, iCol++] = new SourceGrid.Cells.Cell(i.Description);
                gridInterlayers[iIndex, iCol++] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##} x {1:0.##} x {2:0.##}",
                        UnitsManager.ConvertLengthFrom(i.Dimensions.M0, us),
                        UnitsManager.ConvertLengthFrom(i.Dimensions.M1, us),
                        UnitsManager.ConvertLengthFrom(i.Dimensions.M2, us))
                    );
                gridInterlayers[iIndex, iCol++] = new SourceGrid.Cells.Cell(
                        UnitsManager.ConvertMassFrom(i.Weight, us)
                    );
                gridInterlayers[iIndex, iCol] = new SourceGrid.Cells.CheckBox(null, i.AutoInsert);
                gridInterlayers[iIndex, iCol++].AddController(checkBoxEvent);
                gridInterlayers[iIndex, iCol] = new SourceGrid.Cells.Button("")
                {
                    Image = Properties.Resources.Delete
                };
                gridInterlayers[iIndex, iCol++].AddController(buttonDelete);
            }
            GridFinalize(gridInterlayers);
        }
        #endregion
        #region PalletCaps
        private void FillGridPalletCaps(WCFClient wcfClient)
        {
            // initialize grid
            List<string> captions = new List<string>
            {
                string.Format(Properties.Resources.ID_DIMEXT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_DIMINT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_WEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_MASS))
            };
            GridInitialize(gridPalletCaps, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);

            _palletCaps = wcfClient.Client.GetAllPalletCapsSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBPalletCap pc in _palletCaps)
            {
                UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)pc.UnitSystem;

                gridPalletCaps.Rows.Insert(++iIndex);
                int iCol = 0;
                gridPalletCaps[iIndex, iCol++] = new SourceGrid.Cells.Cell(pc.Name);
                gridPalletCaps[iIndex, iCol++] = new SourceGrid.Cells.Cell(pc.Description);
                gridPalletCaps[iIndex, iCol++] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##} x {1:0.##} x {2:0.##}",
                        UnitsManager.ConvertLengthFrom(pc.DimensionsOuter.M0, us),
                        UnitsManager.ConvertLengthFrom(pc.DimensionsOuter.M1, us),
                        UnitsManager.ConvertLengthFrom(pc.DimensionsOuter.M2, us)));
                gridPalletCaps[iIndex, iCol++] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##} x {1:0.##} x {2:0.##}",
                        UnitsManager.ConvertLengthFrom(pc.DimensionsInner.M0, us),
                        UnitsManager.ConvertLengthFrom(pc.DimensionsInner.M1, us),
                        UnitsManager.ConvertLengthFrom(pc.DimensionsInner.M2, us)));
                gridPalletCaps[iIndex, iCol++] = new SourceGrid.Cells.Cell(string.Format("{0:0.###}", UnitsManager.ConvertMassFrom(pc.Weight, us)));
                gridPalletCaps[iIndex, iCol] = new SourceGrid.Cells.CheckBox(null, pc.AutoInsert);
                gridPalletCaps[iIndex, iCol++].AddController(checkBoxEvent);
                gridPalletCaps[iIndex, iCol] = new SourceGrid.Cells.Button("")
                {
                    Image = Properties.Resources.Delete
                };
                gridPalletCaps[iIndex, iCol++].AddController(buttonDelete);
            }
            GridFinalize(gridPalletCaps);
        }
        #endregion
        #region PalletCormers
        private void FillGridPalletCorners(WCFClient wcfClient)
        {
            // initialize grid
            List<string> captions = new List<string>
            {
                string.Format(Properties.Resources.ID_LENGTH_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_WIDTH_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_THICKNESS_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_WEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_MASS))
            };
            GridInitialize(gridPalletCorners, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);

            _palletCorners = wcfClient.Client.GetAllPalletCornersSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBPalletCorner c in _palletCorners)
            {
                UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)c.UnitSystem;
                gridPalletCorners.Rows.Insert(++iIndex);
                int iCol = 0;
                gridPalletCorners[iIndex, iCol++] = new SourceGrid.Cells.Cell(c.Name);
                gridPalletCorners[iIndex, iCol++] = new SourceGrid.Cells.Cell(c.Description);
                gridPalletCorners[iIndex, iCol++] = new SourceGrid.Cells.Cell(string.Format("{0:0.##}", UnitsManager.ConvertLengthFrom(c.Length, us)));
                gridPalletCorners[iIndex, iCol++] = new SourceGrid.Cells.Cell(string.Format("{0:0.##}", UnitsManager.ConvertLengthFrom(c.Width, us)));
                gridPalletCorners[iIndex, iCol++] = new SourceGrid.Cells.Cell(string.Format("{0:0.##}", UnitsManager.ConvertLengthFrom(c.Thickness, us)));
                gridPalletCorners[iIndex, iCol++] = new SourceGrid.Cells.Cell(string.Format("{0:0.###}", UnitsManager.ConvertMassFrom(c.Weight, us)));
                gridPalletCorners[iIndex, iCol] = new SourceGrid.Cells.CheckBox(null, c.AutoInsert);
                gridPalletCorners[iIndex, iCol++].AddController(checkBoxEvent);
                gridPalletCorners[iIndex, iCol] = new SourceGrid.Cells.Button("")
                {
                    Image = Properties.Resources.Delete
                };
                gridPalletCorners[iIndex, iCol++].AddController(buttonDelete);
            }
            GridFinalize(gridPalletCorners);
        }
        #endregion
        #region PalletFilms
        private void FillGridPalletFilms(WCFClient wcfClient)
        {
            // initialize grid
            List<string> captions = new List<string>
            {
                Properties.Resources.ID_TRANSPARENCY,
                string.Format(Properties.Resources.ID_HATCHINGSPACING, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                Properties.Resources.ID_HATCHINGANGLE, 
                Properties.Resources.ID_LINEARMASS
            };
            GridInitialize(gridPalletFilms, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);

            _palletFilms = wcfClient.Client.GetAllPalletFilmsSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBPalletFilm c in _palletFilms)
            {
                UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)c.UnitSystem;
                gridPalletFilms.Rows.Insert(++iIndex);
                int iCol = 0;
                gridPalletFilms[iIndex, iCol++] = new SourceGrid.Cells.Cell(c.Name);
                gridPalletFilms[iIndex, iCol++] = new SourceGrid.Cells.Cell(c.Description);
                gridPalletFilms[iIndex, iCol++] = new SourceGrid.Cells.Cell(c.UseTransparency);
                gridPalletFilms[iIndex, iCol++] = new SourceGrid.Cells.Cell(c.UseHatching ? string.Format("{0:0.#}", c.HatchingSpace) : "-");
                gridPalletFilms[iIndex, iCol++] = new SourceGrid.Cells.Cell(c.UseHatching ? string.Format("{0:0.#}", c.HatchingAngle) : "-");
                gridPalletFilms[iIndex, iCol++] = new SourceGrid.Cells.Cell(string.Format("{0:0.###}", c.LinearMass));
                gridPalletFilms[iIndex, iCol] = new SourceGrid.Cells.CheckBox(null, c.AutoInsert);
                gridPalletFilms[iIndex, iCol++].AddController(checkBoxEvent);
                gridPalletFilms[iIndex, iCol] = new SourceGrid.Cells.Button("")
                {
                    Image = Properties.Resources.Delete
                };
                gridPalletFilms[iIndex, iCol++].AddController(buttonDelete);
            }
            GridFinalize(gridPalletFilms);
        }
        #endregion
        #region Boxes
        private void FillGridBoxes(WCFClient wcfClient)
        {
            // initialize grid
            List<string> captions = new List<string>
            {
                string.Format(Properties.Resources.ID_DIMEXT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_WEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_MASS))
            };
            GridInitialize(gridBoxes, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);

            _boxes = wcfClient.Client.GetAllBoxesSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBCase c in _boxes)
            {
                UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)c.UnitSystem;

                gridBoxes.Rows.Insert(++iIndex);
                int iCol = 0;
                gridBoxes[iIndex, iCol++] = new SourceGrid.Cells.Cell(c.Name);
                gridBoxes[iIndex, iCol++] = new SourceGrid.Cells.Cell(c.Description);
                gridBoxes[iIndex, iCol++] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##} x {1:0.##} x {2:0.##}",
                        UnitsManager.ConvertLengthFrom(c.DimensionsOuter.M0, us),
                        UnitsManager.ConvertLengthFrom(c.DimensionsOuter.M1, us),
                        UnitsManager.ConvertLengthFrom(c.DimensionsOuter.M2, us)));
                gridBoxes[iIndex, iCol++] = new SourceGrid.Cells.Cell(UnitsManager.ConvertMassFrom(c.Weight, us));
                gridBoxes[iIndex, iCol] = new SourceGrid.Cells.CheckBox(null, c.AutoInsert);
                gridBoxes[iIndex, iCol++].AddController(checkBoxEvent);
                gridBoxes[iIndex, iCol] = new SourceGrid.Cells.Button("") { Image = Properties.Resources.Delete };
                gridBoxes[iIndex, iCol++].AddController(buttonDelete);
            }
            GridFinalize(gridBoxes);
        }
        #endregion
        #region Cases
        private void FillGridCases(WCFClient wcfClient)
        {
            // initialize grid
            List<string> captions = new List<string>
            {
                string.Format(Properties.Resources.ID_DIMEXT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_DIMINT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_WEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_MASS))
            };
            GridInitialize(gridCases, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);

            _cases = wcfClient.Client.GetAllCasesSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBCase c in _cases)
            {
                UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)c.UnitSystem;

                gridCases.Rows.Insert(++iIndex);
                int iCol = 0;
                gridCases[iIndex, iCol++] = new SourceGrid.Cells.Cell(c.Name);
                gridCases[iIndex, iCol++] = new SourceGrid.Cells.Cell(c.Description);
                gridCases[iIndex, iCol++] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##} x {1:0.##} x {2:0.##}",
                        UnitsManager.ConvertLengthFrom(c.DimensionsOuter.M0, us),
                        UnitsManager.ConvertLengthFrom(c.DimensionsOuter.M1, us),
                        UnitsManager.ConvertLengthFrom(c.DimensionsOuter.M2, us)));
                gridCases[iIndex, iCol++] = new SourceGrid.Cells.Cell(
                    c.HasInnerDims ?
                    string.Format("{0:0.##} x {1:0.##} x {2:0.##}",
                        UnitsManager.ConvertLengthFrom(c.DimensionsInner.M0, us),
                        UnitsManager.ConvertLengthFrom(c.DimensionsInner.M1, us),
                        UnitsManager.ConvertLengthFrom(c.DimensionsInner.M2, us)) : "-");
                gridCases[iIndex, iCol++] = new SourceGrid.Cells.Cell(UnitsManager.ConvertMassFrom(c.Weight, us));
                gridCases[iIndex, iCol] = new SourceGrid.Cells.CheckBox(null, c.AutoInsert);
                gridCases[iIndex, iCol++].AddController(checkBoxEvent);
                gridCases[iIndex, iCol] = new SourceGrid.Cells.Button("")
                {
                    Image = Properties.Resources.Delete
                };
                gridCases[iIndex, iCol++].AddController(buttonDelete);
            }
            GridFinalize(gridCases);
        }
        #endregion
        #region Bundles
        private void FillGridBundles(WCFClient wcfClient)
        {
            // initialize grid
            List<string> captions = new List<string>
            {
                string.Format(Properties.Resources.ID_LENGTH_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_WIDTH_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_THICKNESS_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_NUMBER),
                string.Format(Properties.Resources.ID_WEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_MASS))
            };
            GridInitialize(gridBundles, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);

            _bundles = wcfClient.Client.GetAllBundlesSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBBundle b in _bundles)
            {
                UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)b.UnitSystem;

                gridBundles.Rows.Insert(++iIndex);
                int iCol = 0;
                gridBundles[iIndex, iCol++] = new SourceGrid.Cells.Cell(b.Name);
                gridBundles[iIndex, iCol++] = new SourceGrid.Cells.Cell(b.Description);
                gridBundles[iIndex, iCol++] = new SourceGrid.Cells.Cell(UnitsManager.ConvertLengthFrom(b.DimensionsUnit.M0, us));
                gridBundles[iIndex, iCol++] = new SourceGrid.Cells.Cell(UnitsManager.ConvertLengthFrom(b.DimensionsUnit.M1, us));
                gridBundles[iIndex, iCol++] = new SourceGrid.Cells.Cell(UnitsManager.ConvertLengthFrom(b.DimensionsUnit.M2, us));
                gridBundles[iIndex, iCol++] = new SourceGrid.Cells.Cell(b.Number);
                gridBundles[iIndex, iCol++] = new SourceGrid.Cells.Cell(UnitsManager.ConvertMassFrom(b.Number * b.UnitWeight, us));
                gridBundles[iIndex, iCol] = new SourceGrid.Cells.CheckBox(null, b.AutoInsert);
                gridBundles[iIndex, iCol++].AddController(checkBoxEvent);
                gridBundles[iIndex, iCol] = new SourceGrid.Cells.Button("")
                {
                    Image = Properties.Resources.Delete
                };
                gridBundles[iIndex, iCol++].AddController(buttonDelete);
            }
            GridFinalize(gridBundles);
        }
        #endregion
        #region Cylinders
        void FillGridCylinders(WCFClient wcfClient)
        {
            // initialize grid
            List<string> captions = new List<string>
            {
                string.Format(Properties.Resources.ID_HEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_DIAMETEROUTER_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_DIAMETERINNER_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_WEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_MASS))
            };
            GridInitialize(gridCylinders, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);

            _cylinders = wcfClient.Client.GetAllCylindersSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBCylinder cyl in _cylinders)
            {
                UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)cyl.UnitSystem;

                gridCylinders.Rows.Insert(++iIndex);
                int iCol = 0;
                gridCylinders[iIndex, iCol++] = new SourceGrid.Cells.Cell(cyl.Name);
                gridCylinders[iIndex, iCol++] = new SourceGrid.Cells.Cell(cyl.Description);
                gridCylinders[iIndex, iCol++] = new SourceGrid.Cells.Cell(UnitsManager.ConvertLengthFrom(cyl.Height, us));
                gridCylinders[iIndex, iCol++] = new SourceGrid.Cells.Cell(UnitsManager.ConvertLengthFrom(2.0 * cyl.RadiusOuter, us));
                gridCylinders[iIndex, iCol++] = new SourceGrid.Cells.Cell(UnitsManager.ConvertLengthFrom(2.0 * cyl.RadiusInner, us));
                gridCylinders[iIndex, iCol++] = new SourceGrid.Cells.Cell(UnitsManager.ConvertMassFrom(cyl.Weight, us));
                gridCylinders[iIndex, iCol] = new SourceGrid.Cells.CheckBox(null, cyl.AutoInsert);
                gridCylinders[iIndex, iCol++].AddController(checkBoxEvent);
                gridCylinders[iIndex, iCol] = new SourceGrid.Cells.Button("")
                {
                    Image = Properties.Resources.Delete
                };
                gridCylinders[iIndex, iCol++].AddController(buttonDelete);
            }
            GridFinalize(gridCylinders);
        }
        #endregion
        #region Trucks
        private void FillGridTrucks(WCFClient wcfClient)
        {
            // initialize grid
            List<string> captions = new List<string>
            {
                string.Format(Properties.Resources.ID_DIMENSIONS, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH))
            };
            GridInitialize(gridTrucks, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);
            // get all trucks
            _trucks = wcfClient.Client.GetAllTrucksSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBTruck t in _trucks)
            {
                gridTrucks.Rows.Insert(++iIndex);
                gridTrucks[iIndex, 0] = new SourceGrid.Cells.Cell(t.Name);
                gridTrucks[iIndex, 1] = new SourceGrid.Cells.Cell(t.Description);
                gridTrucks[iIndex, 2] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##} x {1:0.##} x {2:0.##}",
                    UnitsManager.ConvertLengthFrom(t.DimensionsInner.M0, (UnitsManager.UnitSystem)t.UnitSystem),
                    UnitsManager.ConvertLengthFrom(t.DimensionsInner.M1, (UnitsManager.UnitSystem)t.UnitSystem),
                    UnitsManager.ConvertLengthFrom(t.DimensionsInner.M2, (UnitsManager.UnitSystem)t.UnitSystem))
                    );
                gridTrucks[iIndex, 3] = new SourceGrid.Cells.CheckBox(null, t.AutoInsert);
                gridTrucks[iIndex, 3].AddController(checkBoxEvent);
                gridTrucks[iIndex, 4] = new SourceGrid.Cells.Button("")
                {
                    Image = Properties.Resources.Delete
                };
                gridTrucks[iIndex, 4].AddController(buttonDelete);
            }
            GridFinalize(gridTrucks);
        }
        #endregion
        #region Bags
        private void FillGridBags(WCFClient wcfClient)
        {
            // initialize grid
            var captions = new List<string>
            {
                string.Format(Properties.Resources.ID_DIMENSIONS, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_RADIUSROUNDING_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_WEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_MASS))
            };
            GridInitialize(gridBags, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);
            // get all bags
            _bags = wcfClient.Client.GetAllBagsSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBBag b in _bags)
            {
                gridBags.Rows.Insert(++iIndex);
                gridBags[iIndex, 0] = new SourceGrid.Cells.Cell(b.Name);
                gridBags[iIndex, 1] = new SourceGrid.Cells.Cell(b.Description);
                gridBags[iIndex, 2] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##} x {1:0.##} x {2:0.##}",
                    UnitsManager.ConvertLengthFrom(b.DimensionsOuter.M0, (UnitsManager.UnitSystem)b.UnitSystem),
                    UnitsManager.ConvertLengthFrom(b.DimensionsOuter.M1, (UnitsManager.UnitSystem)b.UnitSystem),
                    UnitsManager.ConvertLengthFrom(b.DimensionsOuter.M2, (UnitsManager.UnitSystem)b.UnitSystem))
                    );
                gridBags[iIndex, 3] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##}", UnitsManager.ConvertLengthFrom(b.Radius, (UnitsManager.UnitSystem)b.UnitSystem))
                    );
                gridBags[iIndex, 4] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##}", UnitsManager.ConvertMassFrom(b.Weight, (UnitsManager.UnitSystem)b.UnitSystem))
                    );
                gridBags[iIndex, 5] = new SourceGrid.Cells.CheckBox(null, b.AutoInsert);
                gridBags[iIndex, 5].AddController(checkBoxEvent);
                gridBags[iIndex, 6] = new SourceGrid.Cells.Button("") { Image = Properties.Resources.Delete };
                gridBags[iIndex, 6].AddController(buttonDelete);
            }
            GridFinalize(gridBags);
        }
        #endregion
        #region Bottles
        private void FillGridBottles(WCFClient wcfClient)
        {
            // initialize grid
            var captions = new List<string>
            {
                string.Format(Properties.Resources.ID_HEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_DIAMETEROUTER_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_LENGTH)),
                string.Format(Properties.Resources.ID_WEIGHT_WU, UnitsManager.UnitString(UnitsManager.UnitType.UT_MASS))
            };
            GridInitialize(gridBottles, captions);
            // handling checkbox event
            SourceGrid.Cells.Controllers.CustomEvents checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
            checkBoxEvent.Click += new EventHandler(OnAutoImport);
            // handling delete event
            SourceGrid.Cells.Controllers.CustomEvents buttonDelete = new SourceGrid.Cells.Controllers.CustomEvents();
            buttonDelete.Click += new EventHandler(OnDeleteItem);
            // get all bags
            _bottles = wcfClient.Client.GetAllBottlesSearch(SearchString, SearchDescription, RangeIndex, ref _numberOfItems, false);
            int iIndex = 0;
            foreach (DCSBBottle b in _bottles)
            {
                float height = 0.0f;
                float diameter = 0.0f;
                foreach (var t in b.Diameters)
                {
                    height = Math.Max(t.Item1, height);
                    diameter = Math.Max(t.Item2, diameter);                
                }
                gridBottles.Rows.Insert(++iIndex);
                gridBottles[iIndex, 0] = new SourceGrid.Cells.Cell(b.Name);
                gridBottles[iIndex, 1] = new SourceGrid.Cells.Cell(b.Description);
                gridBottles[iIndex, 2] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##}", UnitsManager.ConvertLengthFrom((double)height, (UnitsManager.UnitSystem)b.UnitSystem))
                    );
                gridBottles[iIndex, 3] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##}", UnitsManager.ConvertLengthFrom((double)diameter, (UnitsManager.UnitSystem)b.UnitSystem))
                );
                gridBottles[iIndex, 4] = new SourceGrid.Cells.Cell(
                    string.Format("{0:0.##}", UnitsManager.ConvertMassFrom(b.Weight, (UnitsManager.UnitSystem)b.UnitSystem))
                );
                gridBottles[iIndex, 5] = new SourceGrid.Cells.CheckBox(null, b.AutoInsert);
                gridBottles[iIndex, 5].AddController(checkBoxEvent);
                gridBottles[iIndex, 6] = new SourceGrid.Cells.Button("") { Image = Properties.Resources.Delete };
                gridBottles[iIndex, 6].AddController(buttonDelete);
            }
            GridFinalize(gridBottles);
        }
        #endregion
        #endregion

        #region Event handlers
        private void OnAutoImport(object sender, EventArgs e)
        {
            try
            {
                SourceGrid.CellContext context = (SourceGrid.CellContext)sender;
                int iSel = context.Position.Row - 1;
                SourceGrid.Grid g = context.Grid as SourceGrid.Grid;

                using (WCFClient wcfClient = new WCFClient())
                {
                    if (g == gridPallets)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TPallet, _pallets[iSel].ID, !_pallets[iSel].AutoInsert);
                        FillGridPallets(wcfClient);
                    }
                    else if (g == gridInterlayers)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TInterlayer, _interlayers[iSel].ID, !_interlayers[iSel].AutoInsert);
                        FillGridInterlayers(wcfClient);
                    }
                    else if (g == gridPalletCorners)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TPalletCorner, _palletCorners[iSel].ID, !_palletCorners[iSel].AutoInsert);
                        FillGridPalletCorners(wcfClient);
                    }
                    else if (g == gridPalletCaps)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TPalletCap, _palletCaps[iSel].ID, !_palletCaps[iSel].AutoInsert);
                        FillGridPalletCaps(wcfClient);
                    }
                    else if (g == gridPalletFilms)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TPalletFilm, _palletFilms[iSel].ID, !_palletFilms[iSel].AutoInsert);
                        FillGridPalletFilms(wcfClient);
                    }
                    else if (g == gridInterlayers)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TInterlayer, _interlayers[iSel].ID, !_interlayers[iSel].AutoInsert);
                        FillGridInterlayers(wcfClient);
                    }
                    else if (g == gridBoxes)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TCase, _boxes[iSel].ID, !_boxes[iSel].AutoInsert);
                        FillGridBoxes(wcfClient);
                    }
                    else if (g == gridCases)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TCase, _cases[iSel].ID, !_cases[iSel].AutoInsert);
                        FillGridCases(wcfClient);
                    }
                    else if (g == gridBundles)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TBundle, _bundles[iSel].ID, !_bundles[iSel].AutoInsert);
                        FillGridBundles(wcfClient);
                    }
                    else if (g == gridCylinders)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TCylinder, _cylinders[iSel].ID, !_cylinders[iSel].AutoInsert);
                        FillGridCylinders(wcfClient);
                    }
                    else if (g == gridTrucks)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TTruck, _trucks[iSel].ID, !_trucks[iSel].AutoInsert);
                        FillGridTrucks(wcfClient);
                    }
                    else if (g == gridBags)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TBag, _bags[iSel].ID, !_bags[iSel].AutoInsert);
                        FillGridBags(wcfClient);
                    }
                    else if (g == gridBottles)
                    {
                        wcfClient.Client.SetAutoInsert(DCSBTypeEnum.TBottle, _bottles[iSel].ID, _bottles[iSel].AutoInsert);
                        FillGridBottles(wcfClient);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private void OnDeleteItem(object sender, EventArgs e)
        {
            try
            {
                SourceGrid.CellContext context = (SourceGrid.CellContext)sender;
                int iSel = context.Position.Row - 1;
                SourceGrid.Grid g = context.Grid as SourceGrid.Grid;

                using (WCFClient wcfClient = new WCFClient())
                {
                        if (g == gridPallets)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TPallet, _pallets[iSel].ID);
                            FillGridPallets(wcfClient);
                        }
                        else if (g == gridInterlayers)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TInterlayer, _interlayers[iSel].ID);
                            FillGridInterlayers(wcfClient);
                        }
                        else if (g == gridPalletCorners)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TPalletCorner, _palletCorners[iSel].ID);
                            FillGridPalletCorners(wcfClient);
                        }
                        else if (g == gridPalletCaps)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TPalletCap, _palletCaps[iSel].ID);
                            FillGridPalletCaps(wcfClient);
                        }
                        else if (g == gridPalletFilms)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TPalletFilm, _palletFilms[iSel].ID);
                            FillGridPalletFilms(wcfClient);
                        }
                        else if (g == gridBoxes)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TCase, _boxes[iSel].ID);
                            FillGridBoxes(wcfClient);
                        }
                        else if (g == gridCases)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TCase, _cases[iSel].ID);
                            FillGridCases(wcfClient);
                        }
                        else if (g == gridBundles)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TBundle, _bundles[iSel].ID);
                            FillGridBundles(wcfClient);
                        }
                        else if (g == gridCylinders)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TCylinder, _cylinders[iSel].ID);
                            FillGridCylinders(wcfClient);
                        }
                        else if (g == gridTrucks)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TTruck, _trucks[iSel].ID);
                            FillGridTrucks(wcfClient);
                        }
                        else if (g == gridBags)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TBag, _bags[iSel].ID);
                            FillGridBags(wcfClient);
                        }
                        else if (g == gridBottles)
                        {
                            wcfClient.Client.RemoveItemById(DCSBTypeEnum.TBottle, _bottles[iSel].ID);
                            FillGridBottles(wcfClient);
                        }
                }
                UpdateButtons();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private void OnSelectedTabChanged(object sender, EventArgs e)
        {
            // clear search string
            tbSearch.Clear();
            OnSearchFieldChanged(this, null);
            // update grid
            RangeIndex = 0;
            UpdateGrid();
        }
        private void OnSelChangeGrid(object sender, SourceGrid.RangeRegionChangedEventArgs e)
        {
            try
            {
                SourceGrid.Selection.RowSelection select = sender as SourceGrid.Selection.RowSelection;
                SourceGrid.Grid g = select.Grid as SourceGrid.Grid;

                SourceGrid.RangeRegion region = g.Selection.GetSelectionRegion();
                int[] indexes = region.GetRowsIndex();
                if (indexes.Length < 1 || indexes[0] < 1)
                    _selectedItem = null;
                else
                {
                    int iSel = indexes[0] - 1;
                    if (g == gridPallets) _selectedItem = _pallets[iSel];
                    if (g == gridPalletCorners) _selectedItem = _palletCorners[iSel];
                    if (g == gridPalletCaps) _selectedItem = _palletCaps[iSel];
                    if (g == gridPalletFilms) _selectedItem = _palletFilms[iSel];
                    if (g == gridInterlayers) _selectedItem = _interlayers[iSel];
                    if (g == gridBoxes) _selectedItem = _boxes[iSel];
                    if (g == gridCases) _selectedItem = _cases[iSel];
                    if (g == gridBundles) _selectedItem = _bundles[iSel];
                    if (g == gridCylinders) _selectedItem = _cylinders[iSel];
                    if (g == gridTrucks) _selectedItem = _trucks[iSel];
                    if (g == gridBags) _selectedItem = _bags[iSel];
                    if (g == gridBottles) _selectedItem = _bottles[iSel];
                }
                graphCtrl.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private void OnImport(object sender, EventArgs e)
        {
            // sanity check
            if (null == Document || null == _selectedItem) return;
            // checking name
            string name = _selectedItem.Name;
            if (!GetValidName(ref name)) return; // user exited without entering a valid name
            // unit system
            UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)_selectedItem.UnitSystem;
            try
            {
                // pallet
                if (_selectedItem is DCSBPallet dcsbPallet)
                {
                    PalletProperties palletProperties = Document.CreateNewPallet(
                        name, dcsbPallet.Description,
                        dcsbPallet.PalletType,
                        UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M0, us),
                        UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M1, us),
                        UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M2, us),
                        UnitsManager.ConvertMassFrom(dcsbPallet.Weight, us),
                        Color.FromArgb(dcsbPallet.Color)
                        );
                    if (dcsbPallet.AdmissibleLoad.HasValue)
                        palletProperties.AdmissibleLoadWeight = UnitsManager.ConvertMassFrom(dcsbPallet.AdmissibleLoad.Value, us);
                }
                // interlayer
                if (_selectedItem is DCSBInterlayer dcsbInterlayer)
                {
                    InterlayerProperties interlayerProp = Document.CreateNewInterlayer(
                        name, dcsbInterlayer.Description,
                        UnitsManager.ConvertLengthFrom(dcsbInterlayer.Dimensions.M0, us),
                        UnitsManager.ConvertLengthFrom(dcsbInterlayer.Dimensions.M1, us),
                        UnitsManager.ConvertLengthFrom(dcsbInterlayer.Dimensions.M2, us),
                        UnitsManager.ConvertMassFrom(dcsbInterlayer.Weight, us),
                        Color.FromArgb(dcsbInterlayer.Color)
                        );
                }
                // case
                if (_selectedItem is DCSBCase dcsbCase)
                {
                    Color[] colors = new Color[6];
                    for (int i = 0; i < 6; ++i)
                        colors[i] = Color.FromArgb(dcsbCase.Colors[i]);

                    BoxProperties bProperties = null;
                    if (dcsbCase.IsCase)
                        bProperties = Document.CreateNewCase(name, dcsbCase.Description,
                            UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M0, us),
                            UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M1, us),
                            UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M2, us),
                            UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsInner.M0, us),
                            UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsInner.M1, us),
                            UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsInner.M2, us),
                            dcsbCase.DimensionsInner.M0 > 0.0 &&  dcsbCase.DimensionsInner.M1 > 0.0 && dcsbCase.DimensionsInner.M2 > 0.0,
                            UnitsManager.ConvertMassFrom(dcsbCase.Weight, us),
                            colors);
                    else
                        bProperties = Document.CreateNewBox(
                            name, dcsbCase.Description,
                            UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M0, us),
                            UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M1, us),
                            UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M2, us),
                            UnitsManager.ConvertMassFrom(dcsbCase.Weight, us),
                            colors);
                    bProperties.TapeWidth = new OptDouble(dcsbCase.ShowTape, UnitsManager.ConvertLengthFrom(dcsbCase.TapeWidth, us));
                    bProperties.TapeColor = Color.FromArgb(dcsbCase.TapeColor);
                    bProperties.SetNetWeight(
                        new OptDouble(!dcsbCase.HasInnerDims && dcsbCase.NetWeight.HasValue
                            , dcsbCase.NetWeight ?? 0.0));
                    List<Pair<HalfAxis.HAxis, Texture>> textures = new List<Pair<HalfAxis.HAxis, Texture>>();
                    bProperties.TextureList = textures;
                }
                // bundle
                if (_selectedItem is DCSBBundle dcsbBundle)
                {
                    BundleProperties bundle = Document.CreateNewBundle(name, dcsbBundle.Description,
                        UnitsManager.ConvertLengthFrom(dcsbBundle.DimensionsUnit.M0, us),
                        UnitsManager.ConvertLengthFrom(dcsbBundle.DimensionsUnit.M1, us),
                        UnitsManager.ConvertLengthFrom(dcsbBundle.DimensionsUnit.M2, us),
                        UnitsManager.ConvertMassFrom(dcsbBundle.UnitWeight, us),
                        Color.FromArgb(dcsbBundle.Color),
                        dcsbBundle.Number);
                }
                // cylinder
                if (_selectedItem is DCSBCylinder dcsbCylinder)
                {
                    CylinderProperties cylProp = Document.CreateNewCylinder(
                        name, dcsbCylinder.Description,
                        UnitsManager.ConvertLengthFrom(dcsbCylinder.RadiusOuter, us),
                        UnitsManager.ConvertLengthFrom(dcsbCylinder.RadiusInner, us),
                        UnitsManager.ConvertLengthFrom(dcsbCylinder.Height, us),
                        UnitsManager.ConvertMassFrom(dcsbCylinder.Weight, us),
                        OptDouble.Zero,
                        Color.FromArgb(dcsbCylinder.ColorTop), Color.FromArgb(dcsbCylinder.ColorOuter), Color.FromArgb(dcsbCylinder.ColorInner)
                        );
                }
                // trucks
                if (_selectedItem is DCSBTruck dcsbTruck)
                {
                    var truckProp = Document.CreateNewTruck(
                        name, dcsbTruck.Description,
                        UnitsManager.ConvertLengthFrom(dcsbTruck.DimensionsInner.M0, us),
                        UnitsManager.ConvertLengthFrom(dcsbTruck.DimensionsInner.M1, us),
                        UnitsManager.ConvertLengthFrom(dcsbTruck.DimensionsInner.M2, us),
                        UnitsManager.ConvertMassFrom(dcsbTruck.AdmissibleLoad, us),
                        Color.FromArgb(dcsbTruck.Color)
                        );
                }
                if (_selectedItem is DCSBBag dcsbBag)
                {
                    var bagProp = Document.CreateNewBag(
                        name, dcsbBag.Description,
                        new Vector3D(UnitsManager.ConvertLengthFrom(dcsbBag.DimensionsOuter.M0, us), UnitsManager.ConvertLengthFrom(dcsbBag.DimensionsOuter.M1, us), UnitsManager.ConvertLengthFrom(dcsbBag.DimensionsOuter.M2, us)),
                        UnitsManager.ConvertLengthFrom(dcsbBag.Radius, us),
                        UnitsManager.ConvertMassFrom(dcsbBag.Weight, us),
                        dcsbBag.NetWeight.HasValue ? new OptDouble(true, UnitsManager.ConvertMassFrom(dcsbBag.NetWeight.Value, us)) : OptDouble.Zero,
                        Color.FromArgb(dcsbBag.Color)
                        );                    
                }
                if (_selectedItem is DCSBBottle dcsbBottle)
                {
                    var profile = new List<Vector2D>();
                    foreach (var t in dcsbBottle.Diameters)
                        profile.Add(new Vector2D(UnitsManager.ConvertLengthFrom(t.Item1, us), UnitsManager.ConvertLengthFrom(t.Item2, us)));

                    var bottleProp = Document.CreateNewBottle(
                        name, dcsbBottle.Description,
                        profile,
                        UnitsManager.ConvertMassFrom(dcsbBottle.Weight, us),
                        dcsbBottle.NetWeight.HasValue ? new OptDouble(true, UnitsManager.ConvertMassFrom(dcsbBottle.NetWeight.Value, us)) : OptDouble.Zero,
                        Color.FromArgb(dcsbBottle.Color)
                        );
                }
                // pallet cap
                if (_selectedItem is DCSBPalletCap dcsbCap)
                {
                    PalletCapProperties palletCapProperties = Document.CreateNewPalletCap(
                        dcsbCap.Name, dcsbCap.Description,
                        UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsOuter.M0, us),
                        UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsOuter.M1, us),
                        UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsOuter.M2, us),
                        UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsInner.M0, us),
                        UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsInner.M1, us),
                        UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsInner.M2, us),
                        UnitsManager.ConvertMassFrom(dcsbCap.Weight, us),
                        Color.FromArgb(dcsbCap.Color)
                        );
                }
                // pallet corner
                if (_selectedItem is DCSBPalletCorner dcsbPalletCorner)
                {
                    PalletCornerProperties palletCornerProperties = Document.CreateNewPalletCorners(
                        name, dcsbPalletCorner.Description,
                        UnitsManager.ConvertLengthFrom(dcsbPalletCorner.Length, us),
                        UnitsManager.ConvertLengthFrom(dcsbPalletCorner.Width, us),
                        UnitsManager.ConvertLengthFrom(dcsbPalletCorner.Thickness, us),
                        UnitsManager.ConvertMassFrom(dcsbPalletCorner.Weight, us),
                        Color.FromArgb(dcsbPalletCorner.Color)
                        );
                }
                // pallet film
                if (_selectedItem is DCSBPalletFilm dcsbPalletFilm)
                {
                    PalletFilmProperties palletFilm = Document.CreateNewPalletFilm(name, dcsbPalletFilm.Description,
                        dcsbPalletFilm.UseTransparency, dcsbPalletFilm.UseHatching,
                        dcsbPalletFilm.HatchingSpace, dcsbPalletFilm.HatchingAngle,
                        dcsbPalletFilm.LinearMass,
                        Color.FromArgb(dcsbPalletFilm.Color));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
            if (Properties.Settings.Default.CloseDbBrowserAfterImport)
                Close();
        }

        private bool GetValidName(ref string name)
        {
            if (Document.IsValidNewTypeName(name, null))
                return true;
            // not a valid name, show dialog box
            FormValidTypeName form = new FormValidTypeName(Document)
            {
                ItemName = name
            };
            if (DialogResult.OK == form.ShowDialog())
            {
                name = form.ItemName;
                return true;
            }
            else
                return false;
        }
        private void OnImportFromExcelFile(object sender, EventArgs e)
        {
            try
            {
                FormImportExcelCatalog form = new FormImportExcelCatalog();
                form.ShowDialog();
                OnSelectedTabChanged(this, null);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private void OnButtonPrev(object sender, EventArgs e)
        {
            if (RangeIndex > 0)
                RangeIndex--;
            UpdateGrid();
        }
        private void OnButtonNext(object sender, EventArgs e)
        {
            if ((RangeIndex + 1) * 20 < _numberOfItems)
                RangeIndex++;
            UpdateGrid();
        }
        private void OnSearchFieldChanged(object sender, EventArgs e)
        {
            bnSearch.Enabled = !string.IsNullOrEmpty(SearchString);
        }
        private void OnSearch(object sender, EventArgs e)
        {
            UpdateGrid();
        }
        private void OnRemoveAll(object sender, EventArgs e)
        {
            if (MessageBox.Show("", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            try
            {
                string tabName = tabCtrlDBItems.SelectedTab.Name;
                using (WCFClient wcfClient = new WCFClient())
                {
                    wcfClient.Client.RemoveAll(CurrentType);
                }
                UpdateGrid();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Private properties
        private int RangeIndex { get; set; }
        #endregion

        #region Data members
        protected static ILog _log = LogManager.GetLogger(typeof(FormShowDatabase));
        private DCSBItem _selectedItem;
        private int _numberOfItems = 0;

        private DCSBPallet[] _pallets = null;
        private DCSBPalletCap[] _palletCaps = null;
        private DCSBPalletCorner[] _palletCorners = null;
        private DCSBPalletFilm[] _palletFilms = null;
        private DCSBBundle[] _bundles = null;
        private DCSBCase[] _cases = null;
        private DCSBCase[] _boxes = null;
        private DCSBCylinder[] _cylinders = null;
        private DCSBInterlayer[] _interlayers = null;
        private DCSBTruck[] _trucks = null;
        private DCSBBag[] _bags = null;
        private DCSBBottle[] _bottles = null;
        private DocumentSB _doc = null;
        #endregion
    }
}
