#region Using directives
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;

using WeifenLuo.WinFormsUI.Docking;
using log4net;
using Utilities;
using AutoUpdaterDotNET;
using Syroot.Windows.IO;

using treeDiM.UserControls;
using treeDiM.ThreadCallback;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Reporting;
using treeDiM.StackBuilder.Plugin;
using treeDiM.StackBuilder.ExcelExporter;

using treeDiM.PLMPack.DBClient;
using treeDiM.PLMPack.DBClient.PLMPackSR;

using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormMain
        : Form, IDocumentFactory, IMRUClient, IDocumentListener
    {
        #region Data members
        /// <summary>
        /// Docking manager
        /// </summary>
        private DockContentDocumentExplorer _documentExplorer = new DockContentDocumentExplorer();
        private DockContentLogConsole _logConsole;
        private DockContentStartPage _dockStartPage = new DockContentStartPage();
        private ToolStripProfessionalRenderer _defaultRenderer = new ToolStripProfessionalRenderer(new PropertyGridEx.CustomColorScheme());
        private DeserializeDockContent _deserializeDockContent;
        static readonly ILog _log = LogManager.GetLogger(typeof(FormMain));
        private static FormMain _instance;
        private MruManager _mruManager;
        #endregion
        #region Constructor
        public FormMain()
        {
            // set static instance
            _instance = this;
            
            // load content
            _deserializeDockContent = new DeserializeDockContent(ReloadContent);

            InitializeComponent();

            // load file passed as argument
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 2)
            {
                string joinedString = string.Empty;
                for (int i = 1; i < args.Length; ++i)
                    joinedString += (i > 1 ? " " : "") + args[i];
                if (File.Exists(args[1]))
                    OpenDocument(args[1]);
                else if (File.Exists(joinedString))
                    OpenDocument(joinedString);
            }
            // or show splash sceen 
            else
            {
                bool multithreaded = false;
                if (multithreaded)
                {
                    // --- instantiate and start splach screen thread
                    Thread th = new Thread(new ThreadStart(DoSplash));
                    th.Start();
                    // ---
                }
                else
                    DoSplash();
            }
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // loading plugin
            IPlugin plugin = LoadPlugin();
            if (null != plugin)
            {
                plugin.Initialize();
                plugin.UpdateToolbar(toolStripSplitButtonNew);
                plugin.OpenFile += OpenGeneratedFile;
            }
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

            // Apply a gray professional renderer as a default renderer
            ToolStripManager.Renderer = _defaultRenderer;
            _defaultRenderer.RoundedEdges = true;

            // Set DockPanel properties
            var theme = new VS2015BlueTheme();
            dockPanel.Theme = theme;

            dockPanel.ActiveAutoHideContent = null;
            dockPanel.Parent = this;

            dockPanel.SuspendLayout(true);
            if (File.Exists(configFile))
                dockPanel.LoadFromXml(configFile, _deserializeDockContent);
            else
                timerLogin.Start();
            dockPanel.ResumeLayout(true, true);

            // MRUManager
            _mruManager = new MruManager();
            _mruManager.Initialize(
             this,                              // owner form
             mnuFileMRU,                        // Recent Files menu item
             "Software\\TreeDim\\StackBuilder"); // Registry path to keep MRU list

            UpdateToolbarState();

            // show or hide PLMPackLib button
            bool showMenuPLMPackLib = !string.IsNullOrEmpty(UrlPLMPackLib);
            toolStripMIPLMPackLib.Visible = showMenuPLMPackLib;
            toolStripSeparatorPLMPackLib.Visible = showMenuPLMPackLib;

            // windows settings
            if (null != Settings.Default.FormMainPosition)
                Settings.Default.FormMainPosition.Restore(this);

            // connection/disconnection event handling
            WCFClient.AllowDisconnectedMode = true;
            WCFClient.Connected += OnConnected;
            WCFClient.Disconnected += OnDisconnected;
            WCFClient.ConnectionAvoided += OnConnectionAvoided;

            if (!Program.IsWebSiteReachable)
            {
                CreateBasicLayout();
                UpdateDisconnectButton();
            }
        }
        private void OpenGeneratedFile(string s)
        {
            OpenDocument(s);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
             try
            {
                // save form position
                if (null == Settings.Default.FormMainPosition)
                    Settings.Default.FormMainPosition = new WindowSettings();
                Settings.Default.FormMainPosition.Record(this);
                Settings.Default.Save();
                // Close all documents
                CloseAllDocuments(e);
            }
            catch (ConfigurationErrorsException ex)
            {
                _log.Error(string.Format("Failed to save user configuration: {0}", ex.ToString()));
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
                _log.Error(ex.Message);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            // default behavior
            base.OnClosing(e);
        }
        #endregion
        #region Plugin loading
        private bool HasPlugin
        {
            get
            {
                string pluginPath = Settings.Default.PluginPath;
                return string.IsNullOrEmpty(pluginPath) && File.Exists(pluginPath);
            }
        }
        private IPlugin LoadPlugin()
        {
            string pluginPath = Settings.Default.PluginPath;
            if (string.IsNullOrEmpty(pluginPath)
                || string.Equals(Path.GetExtension(pluginPath), "dll")
                || !File.Exists(pluginPath))
                return null;

            SolutionLayered.SetSolver(new LayerSolver());

            IPlugin plugin = null;
            try
            {
                // Create a new assembly from the plugin file we're adding..
                Assembly pluginAssembly = Assembly.LoadFrom(pluginPath);
                // Next we'll loop through all the Types found in the assembly
                foreach (Type pluginType in pluginAssembly.GetTypes())
                {
                    if (pluginType.IsPublic) // Only look at public types
                    {
                        if (!pluginType.IsAbstract)  // Only look at non-abstract types
                        {
                            // Gets a type object of the interface we need the plugins to match
                            Type typeInterface = pluginType.GetInterface("treeDiM.StackBuilder.Plugin.IPlugin", true);

                            // Make sure the interface we want to use actually exists
                            if (typeInterface != null)
                            {
                                // Create a new available plugin since the type implements the IPlugin interface
                                plugin = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
                                if (null != plugin)
                                {
                                    // Call the initialization sub of the plugin
                                    plugin.Initialize();
                                }
                            }
                            typeInterface = null; // cleanup		
                        }
                    }
                }
                pluginAssembly = null; //more cleanup
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
            return plugin;
        }
        #endregion
        #region SplashScreen
        public void DoSplash()
        {
            using (FormSplashScreen sp = new FormSplashScreen(this))
            {
                sp.TimerInterval = 1000;
                sp.ShowDialog();
            }
        }
        #endregion
        #region Start page
        private string UrlStartPage => Settings.Default.StartPageUrl;
        private string UrlPLMPackLib => Settings.Default.UrlPLMPackLib;

        public void ShowStartPage(object sender, EventArgs e)
        {
            // do not try to show start page
            // if StartPageUrl is empty or web site is reachable
            if (string.IsNullOrEmpty(UrlStartPage) || !Program.IsWebSiteReachable)
                return;
           _log.InfoFormat("Showing start page (URL : {0})", UrlStartPage);

            try
            {
                _dockStartPage.Show(dockPanel, DockState.Document);
                _dockStartPage.Url = new Uri(UrlStartPage);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private void CloseStartPage()
        {
            if (null != _dockStartPage)
                _dockStartPage.Hide();
        }
        private void OnTimerLoginTick(object sender, EventArgs e)
        {
            timerLogin.Stop();
            try
            {
                // show login form
                if (!WCFClient.IsConnected)
                {
                    using (WCFClient wcfClient = new WCFClient())
                    {
                        var client = wcfClient.Client;
                        if (WCFClient.IsConnected)
                            client?.ConnectClient(Application.ProductName);
                    }
                }
                // note : CreateBasicLayout now called by OnConnected()
                // *** AutoUpdater.NET
                var knownFolder = new KnownFolder(KnownFolderType.Downloads);
                AutoUpdater.DownloadPath = knownFolder.Path;
                AutoUpdater.UpdateMode = Mode.Normal;
                AutoUpdater.Start(Settings.Default.AutoUpdaterXMLPath);
                // ***
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Login failed (Exception = {0})", ex.Message));
            }
        }
        #endregion
        #region Docking
        private void CreateBasicLayout()
        {
            _documentExplorer.Show(dockPanel, DockState.DockLeft);
            _documentExplorer.DocumentTreeView.AnalysisNodeClicked += new AnalysisTreeView.AnalysisNodeClickHandler(DocumentTreeView_NodeClicked);
            _documentExplorer.DocumentTreeView.SolutionReportClicked += new AnalysisTreeView.AnalysisNodeClickHandler(OnSolutionReportNodeClicked);
            ShowLogConsole();
            if (AssemblyConf != "debug" && Settings.Default.ShowStartPage)
                ShowStartPage(this, null);
        }

        private void ClearBasicLayout()
        {
            _documentExplorer.Hide();
            _documentExplorer.DocumentTreeView.AnalysisNodeClicked -= DocumentTreeView_NodeClicked;
            _documentExplorer.DocumentTreeView.SolutionReportClicked -= OnSolutionReportNodeClicked;
        }

        public void ShowLogConsole()
        {
            // show or hide log console ?
            if (AssemblyConf == "debug" || Settings.Default.ShowLogConsole)
            {
                if (null == FindDocument("Log console"))
                {
                    _logConsole = new DockContentLogConsole()
                    {
                        TabText = "Log console",
                        Text = "Log console"
                    };
                    _logConsole.Show(dockPanel, DockState.DockBottom);
                }
                else
                    _logConsole.IsHidden = false;
            }
        }

        private IDockContent ReloadContent(string persistString)
        {
            switch (persistString)
            {
                case "frmDocument":
                    return null;
                case "frmSolution":
                    _documentExplorer = new DockContentDocumentExplorer();
                    return _documentExplorer;
                case "frmLogConsole":
                    _logConsole = new DockContentLogConsole();
                    return _logConsole;
                default:
                    return null;
            }
        }
        #endregion
        #region DocumentTreeView event handlers
        // ### AnalysisNodeClicked
        void DocumentTreeView_NodeClicked(object sender, AnalysisTreeViewEventArgs eventArg)
        {
            if (null != eventArg.Analysis)
                CreateOrActivateViewAnalysis(eventArg.Analysis);
            else if (null != eventArg.AnalysisPalletsOnPallet)
                CreateOrActivateViewAnalysis(eventArg.AnalysisPalletsOnPallet);
            else if (null != eventArg.AnalysisPalletColumn)
                CreateOrActivateViewAnalysis(eventArg.AnalysisPalletColumn);
            else if (null != eventArg.HAnalysis)
                CreateOrActivateViewHAnalysis(eventArg.HAnalysis);
            else if (null != eventArg.ItemBase)
            {
                Document doc = eventArg.Document;
                ItemBase itemProp = eventArg.ItemBase;
                if (itemProp.GetType() == typeof(BoxProperties))
                {
                    BoxProperties box = itemProp as BoxProperties;
                    var form = new FormNewBox(doc, eventArg.ItemBase as BoxProperties);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(box)) return;
                        box.ID.SetNameDesc(form.BoxName, form.Description);
                        box.SetLength(form.BoxLength);
                        box.SetWidth(form.BoxWidth);
                        box.SetHeight(form.BoxHeight);
                        box.Bulge = form.Bulge;
                        box.SetWeight(form.Weight);
                        box.SetNetWeight(form.NetWeight);
                        box.HasInsideDimensions = form.HasInsideDimensions;
                        box.InsideLength = form.InsideLength;
                        box.InsideWidth = form.InsideWidth;
                        box.InsideHeight = form.InsideHeight;
                        box.SetAllColors(form.Colors);
                        box.TextureList = form.TextureList;
                        box.TapeWidth = form.TapeWidth;
                        box.TapeColor = form.TapeColor;
                        box.StrapperSet = form.StrapperSet;
                        box.Facing = form.Facing;
                        box.SetCOG(form.COG);
                        box.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(BagProperties))
                {
                    BagProperties bag = itemProp as BagProperties;
                    var form = new FormNewBag(doc, eventArg.ItemBase as BagProperties);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(bag)) return;
                        bag.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        bag.SetLength(form.OuterDimensions.X);
                        bag.SetWidth(form.OuterDimensions.Y);
                        bag.SetHeight(form.OuterDimensions.Z);
                        bag.Bulge = form.Bulge;
                        bag.Radius = form.RoundingRadius;
                        bag.SetWeight(form.Weight);
                        bag.SetNetWeight(form.NetWeight);
                        bag.SetColor(form.ColorFill);
                        bag.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(PackProperties))
                {
                    PackProperties pack = itemProp as PackProperties;
                    var form = new FormNewPack(doc, eventArg.ItemBase as PackProperties)
                    {
                        Boxes = doc.Boxes.ToList()
                    };
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(pack)) return;
                        pack.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        pack.Bulge = form.Bulge;
                        pack.Content = form.SelectedPackable;
                        pack.BoxOrientation = form.BoxOrientation;
                        pack.Arrangement = form.Arrangement;
                        pack.RevSolidLayout = form.RevSolidLayout;
                        pack.Wrap = form.Wrapper;
                        pack.Tray = form.Tray;
                        if (form.HasForcedOuterDimensions)
                            pack.ForceOuterDimensions(form.OuterDimensions);
                        pack.StrapperSet = form.StrapperSet;
                        pack.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(CylinderProperties))
                {
                    CylinderProperties cylinderProperties = itemProp as CylinderProperties;
                    var form = new FormNewCylinder(doc, cylinderProperties);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        if (!UserAcknowledgeDependancies(cylinderProperties)) return;
                        cylinderProperties.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        cylinderProperties.RadiusOuter = form.RadiusOuter;
                        cylinderProperties.RadiusInner = form.RadiusInner;
                        cylinderProperties.Height = form.CylinderHeight;
                        cylinderProperties.SetWeight(form.Weight);
                        cylinderProperties.SetNetWeight(form.NetWeight);
                        cylinderProperties.ColorTop = form.ColorTop;
                        cylinderProperties.ColorWallOuter = form.ColorWallOuter;
                        cylinderProperties.ColorWallInner = form.ColorWallInner;
                        cylinderProperties.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(BottleProperties))
                {
                    var bottleProperties = itemProp as BottleProperties;
                    var form = new FormNewBottle(doc, bottleProperties);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        if (!UserAcknowledgeDependancies(bottleProperties)) return;
                        bottleProperties.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        bottleProperties.Color = form.ColorBottle;
                        bottleProperties.SetWeight(form.Weight);
                        bottleProperties.SetNetWeight(form.NetWeight);
                        bottleProperties.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(BundleProperties))
                {
                    BundleProperties bundle = itemProp as BundleProperties;
                    var form = new FormNewBundle(doc, bundle);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(bundle)) return;
                        bundle.ID.SetNameDesc(form.BundleName, form.Description);
                        bundle.SetLength(form.BundleLength);
                        bundle.SetWidth(form.BundleWidth);
                        bundle.UnitThickness = form.UnitThickness;
                        bundle.UnitWeight = form.UnitWeight;
                        bundle.NoFlats = form.NoFlats;
                        bundle.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(InterlayerProperties))
                {
                    InterlayerProperties interlayer = itemProp as InterlayerProperties;
                    var form = new FormNewInterlayer(doc, interlayer);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(interlayer)) return;
                        interlayer.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        interlayer.Length = form.InterlayerLength;
                        interlayer.Width = form.InterlayerWidth;
                        interlayer.Thickness = form.Thickness;
                        interlayer.Weight = form.Weight;
                        interlayer.Color = form.Color;
                        interlayer.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(PalletProperties))
                {
                    PalletProperties pallet = itemProp as PalletProperties;
                    var form = new FormNewPallet(doc, pallet);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(pallet)) return;
                        pallet.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        pallet.Length = form.PalletLength;
                        pallet.Width = form.PalletWidth;
                        pallet.Height = form.PalletHeight;
                        pallet.Weight = form.Weight;
                        pallet.AdmissibleLoadWeight = form.AdmissibleLoad;
                        pallet.TypeName = form.PalletTypeName;
                        pallet.Color = form.PalletColor;
                        pallet.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(TruckProperties))
                {
                    TruckProperties truck = itemProp as TruckProperties;
                    var form = new FormNewTruck(doc, truck);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(truck)) return;
                        truck.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        truck.Length = form.TruckLength;
                        truck.Width = form.TruckWidth;
                        truck.Height = form.TruckHeight;
                        truck.AdmissibleLoadWeight = form.TruckAdmissibleLoadWeight;
                        truck.Color = form.TruckColor;
                        truck.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(PalletCornerProperties))
                {
                    PalletCornerProperties corner = itemProp as PalletCornerProperties;
                    var form = new FormNewPalletCorners(doc, corner);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(corner)) return;
                        corner.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        corner.Length = form.CornerLength;
                        corner.Width = form.CornerWidth;
                        corner.Thickness = form.CornerThickness;
                        corner.Weight = form.CornerWeight;
                        corner.Color = form.CornerColor;
                        corner.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(PalletCapProperties))
                {
                    PalletCapProperties cap = itemProp as PalletCapProperties;
                    var form = new FormNewPalletCap(doc, cap);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (!UserAcknowledgeDependancies(cap)) return;
                        cap.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        cap.Color = form.CapColor;
                        cap.Length = form.CapLength;
                        cap.Width = form.CapWidth;
                        cap.Height = form.CapHeight;
                        cap.InsideLength = form.CapInnerLength;
                        cap.InsideWidth = form.CapInnerWidth;
                        cap.InsideHeight = form.CapInnerHeight;
                        cap.Weight = form.CapWeight;
                        cap.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(PalletFilmProperties))
                {
                    PalletFilmProperties film = itemProp as PalletFilmProperties;
                    var form = new FormNewPalletFilm(doc, film);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        film.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        film.UseTransparency = form.UseTransparency;
                        film.UseHatching = form.UseHatching;
                        film.HatchSpacing = form.HatchSpacing;
                        film.HatchAngle = form.HatchAngle;
                        film.Color = form.FilmColor;
                        film.Weight = form.Weight;
                        film.EndUpdate();
                        doc.Modify();
                    }
                }
                else if (itemProp.GetType() == typeof(PalletLabelProperties))
                {
                    PalletLabelProperties label = itemProp as PalletLabelProperties;
                    var form = new FormNewPalletLabel(doc, label);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        label.ID.SetNameDesc(form.ItemName, form.ItemDescription);
                        label.Dimensions = form.Dimensions;
                        label.Weight = form.Weight;
                        label.Color = form.Color;
                        label.Bitmap = form.Bitmap;
                        label.EndUpdate();
                        doc.Modify();
                    }
                }
                else
                    Debug.Assert(false);
            }
        }
        private bool UserAcknowledgeDependancies(ItemBase item)
        {
            if (item.HasDependancies)
            {
                if (DialogResult.Cancel == MessageBox.Show(
                    string.Format(Resources.ID_DEPENDINGANALYSES, item.Name, item.DependancyString)
                    , Application.ProductName
                    , MessageBoxButtons.OKCancel))
                    return false;
            }
            return true;
        }

        public static void GenerateReport(Analysis analysis)
        {
            try
            {
                var form = new FormReportDesign(new ReportDataAnalysis(analysis));
                if (DialogResult.OK == form.ShowDialog()) {}
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString()); Program.SendCrashReport(ex);
            }
        }
        public static void GenerateExport(AnalysisLayered analysis)
        {
            try
            {
                FormExporter form = new FormExporter() { Analysis = analysis };
                if (DialogResult.OK == form.ShowDialog()) { }
                analysis.ParentDocument.Modify();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        public static void GenerateExport(AnalysisHetero analysis, string formatName)
        {
            try
            {
                var form = new FormReportDesign( new ReportDataAnalysis(analysis) );
                if (DialogResult.OK == form.ShowDialog()) { }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString()); Program.SendCrashReport(ex);
            }
        }
        private void OnSolutionReportNodeClicked(object sender, AnalysisTreeViewEventArgs eventArg)
        {
            if (null != eventArg.Analysis)
                GenerateReport(eventArg.Analysis);
        }

        void OnEditAnalysis(object sender, AnalysisTreeViewEventArgs eventArg)
        {
            try
            {
                if (eventArg.Document is DocumentSB doc)
                {
                    if (null != eventArg.Analysis)
                    {
                        doc.EditAnalysis(eventArg.Analysis);
                        CreateOrActivateViewAnalysis(eventArg.Analysis);
                    }
                    else if (null != eventArg.HAnalysis)
                    {
                        doc.EditAnalysis(eventArg.HAnalysis);
                        CreateOrActivateViewHAnalysis(eventArg.HAnalysis);
                    }
                    else if (null != eventArg.AnalysisPalletsOnPallet)
                    {
                        doc.EditAnalysis(eventArg.AnalysisPalletsOnPallet);
                        CreateOrActivateViewAnalysis(eventArg.AnalysisPalletsOnPallet);
                    }
                    else if (null != eventArg.AnalysisPalletColumn)
                    {
                        doc.EditAnalysis(eventArg.AnalysisPalletColumn);
                        CreateOrActivateViewAnalysis(eventArg.AnalysisPalletColumn);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString()); Program.SendCrashReport(ex);
            }
        }
        #endregion
        #region Caption text / toolbar state
        private void UpdateFormUI()
        {
            // form text
            string caption = string.Empty;
            if (ActiveDocumentOrFirst is DocumentSB doc)
            {
                caption += Path.GetFileNameWithoutExtension(doc.FilePath);
                caption += " - ";
            }
            // + ProductName
            caption += Application.ProductName + (Program.IsSubscribed ? " **Premium**" : string.Empty);
            Text = caption;
        }
        /// <summary>
        /// enables / disable menu/toolbar items
        /// </summary>
        private void UpdateToolbarState()
        {
            DocumentSB doc = FirstDocument;
            // save
            toolStripMenuItemSave.Enabled = (null != doc) && (doc.IsDirty);
            toolStripButtonSave.Enabled = (null != doc) && (doc.IsDirty);
            // save all
            toolStripMenuItemSaveAll.Enabled = OneDocDirty;
            toolStripButtonSaveAll.Enabled = OneDocDirty;
            // save as
            toolStripMenuItemSaveAs.Enabled = (null != doc);
            // close
            toolStripMenuItemClose.Enabled = (null != doc);
            // new box
            toolStripMenuItemBox.Enabled = (null != doc);
            toolStripButtonBox.Enabled = (null != doc);
            // new case
            toolStripMenuItemCase.Enabled = (null != doc);
            toolStripButtonCase.Enabled = (null != doc);
            // new pack
            toolStripMenuItemPack.Enabled = (null != doc) && doc.CanCreatePack;
            toolStripButtonPack.Enabled = (null != doc) && doc.CanCreatePack;
            // new bundle
            toolStripMenuItemBundle.Enabled = (null != doc);
            toolStripButtonBundle.Enabled = (null != doc);
            // new bag
            toolStripMenuItemBag.Enabled = (null != doc);
            toolStripButtonBag.Enabled = (null != doc);
            // new cylinder
            toolStripMenuItemCylinder.Enabled = (null != doc);
            toolStripButtonCylinder.Enabled = (null != doc);
            // new bottle
            toolStripMenuItemNewBottle.Enabled = (null != doc);
            toolStripButtonBottle.Enabled = (null != doc);
            // new pallet
            toolStripMenuItemPallet.Enabled = (null != doc);
            toolStripButtonPallet.Enabled = (null != doc);
            // pallet decoration
            toolStripSBPalletDeco.Enabled = (null != doc);
            toolStripMenuItemInterlayer.Enabled = (null != doc);
            toolStripMenuItemPalletCap.Enabled = (null != doc);
            toolStripMenuItemPalletFilm.Enabled = (null != doc);
            toolStripMenuItemPalletLabel.Enabled = (null != doc);
            toolStripMenuItemPalletCorners.Enabled = (null != doc);
            toolStripMIInterlayer.Enabled = (null != doc);
            toolStripMIPalletCap.Enabled = (null != doc);
            toolStripMIPalletCorner.Enabled = (null != doc);
            toolStripMIPalletFilm.Enabled = (null != doc);
            // new truck
            toolStripMenuItemTruck.Enabled = (null != doc);
            toolStripButtonTruck.Enabled = (null != doc);
            // new case/pallet analysis
            toolStripMenuItemNewAnalysisCasePallet.Enabled = (null != doc) && doc.CanCreateAnalysisCasePallet;
            toolStripMIAnalysisCasePallet.Enabled = (null != doc) && doc.CanCreateAnalysisCasePallet;
            toolStripMIAnalysisBundlePallet.Enabled = (null != doc) && doc.CanCreateAnalysisBundlePallet;
            toolStripMIHAnalysis.Enabled = (null != doc) && doc.CanCreateAnalysisCasePallet;
            // new cylinder/pallet analysis
            toolStripMenuItemNewAnalysisCylinderPallet.Enabled = (null != doc) && doc.CanCreateAnalysisCylinderPallet;
            toolStripMIAnalysisCylinderPallet.Enabled = (null != doc) && doc.CanCreateAnalysisCylinderPallet;
            toolStripMIAnalysisHCylPallet.Enabled = (null != doc) && doc.CanCreateAnalysisCylinderPallet;
            // new box/case analysis
            toolStripMenuItemNewAnalysisBoxCase.Enabled = (null != doc) && doc.CanCreateAnalysisBoxCase;
            toolStripMIAnalysisBoxCase.Enabled = (null != doc) && doc.CanCreateAnalysisBoxCase;
            toolStripMIAnalysisBundleCase.Enabled = (null != doc) && doc.CanCreateAnalysisBundleCase;
            // new case analysis
            toolStripMenuItemNewAnalysisCylinderCase.Enabled = (null != doc) && doc.CanCreateAnalysisCylinderCase;
            toolStripMIAnalysisCylinderCase.Enabled = (null != doc) && doc.CanCreateAnalysisCylinderCase;
            // new truck analyses
            toolStripMIAnalysisCaseTruck.Enabled = (null != doc) && doc.CanCreateAnalysisCaseTruck;
            toolStripMIHAnalysisCaseTruck.Enabled = (null != doc) && doc.CanCreateAnalysisCaseTruck;
            toolStripMIAnalysisPalletTruck.Enabled = (null != doc) && doc.CanCreateAnalysisPalletTruck;
            toolStripMIAnalysisCylinderTruck.Enabled = (null != doc) && doc.CanCreateAnalysisCylinderTruck;
            toolStripMIAnalysisHCylTruck.Enabled = (null != doc) && doc.CanCreateAnalysisCylinderTruck;
            toolStripMenuItemAnalysisPalletTruck.Enabled = (null != doc) && doc.CanCreateAnalysisPalletTruck;
            // split buttons
            toolStripSBAnalysesTruck.Enabled = (null != doc);
            toolStripSBAnalysisPallet.Enabled = (null != doc);
            toolStripSBAnalysesCase.Enabled = (null != doc);
            toolStripSBOptimisations.Enabled = (null != doc);
            toolStripSBPalletDeco.Enabled = (null != doc);
            // optimisations
            toolStripMIBestCase.Enabled         = (null != doc) && doc.CanCreateOptiMulticase;
            toolStripMenuItemBestCase.Enabled   = (null != doc) && doc.CanCreateOptiMulticase;
            toolStripMIBestPack.Enabled         = (null != doc) && doc.CanCreateOptiPack;
            toolStripMenuItemBestPack.Enabled   = (null != doc) && doc.CanCreateOptiPack;
            // disconnected mode
            toolStripMenuItemEditDB.Enabled = WCFClient.IsConnected;
            editPalletSolutionsDB.Enabled = WCFClient.IsConnected;
            // allow export of project summary to Excel
            toolStripMIExportAnalysesSummaryToExcel.Enabled = (null != doc) && doc.Analyses.Count > 0;
            // BCT
            toolStripSB_BCT.Enabled = WCFClient.IsConnected;
            toolStripSplitButtonExcelProcessors.Enabled = WCFClient.IsConnected;
            toolStripSBExcelOneCasePerRow.Enabled = WCFClient.IsConnected;
       }
        #endregion
        #region IDocumentFactory implementation
        public void NewDocument()
        {
            // set solver
            SolutionLayered.SetSolver(new LayerSolver());
            SolutionHCyl.SetSolver(new CylLayoutSolver());

            FormNewDocument form = new FormNewDocument();
            if (DialogResult.OK == form.ShowDialog())
            {
                AddDocument(new DocumentSB(form.DocName, form.DocDescription, form.Author, _documentExplorer.DocumentTreeView), true);
                _log.Info("New document added!");
            }
            UpdateFormUI();
        }
        public void OpenDocument(string filePath)
        {
            try
            {
                // set solver
                SolutionLayered.SetSolver(new LayerSolver());
                SolutionHCyl.SetSolver(new CylLayoutSolver());

                if (!File.Exists(filePath))
                {
                    // update mruFileManager as we failed to load file
                    if (null != _mruManager)
                        _mruManager.Remove(filePath);
                    MessageBox.Show(string.Format(Resources.ID_FILENOTFOUND, filePath));
                    return;
                }
                AddDocument(new DocumentSB(filePath, _documentExplorer.DocumentTreeView), false);

                // update mruFileManager as file was successfully loaded
                if (null != _mruManager)
                    _mruManager.Add(filePath);

                _log.Debug(string.Format("File {0} loaded!", filePath));
            }
            catch (Exception ex)
            {
                // update mruFileManager as we failed to load file
                if (null != _mruManager)
                    _mruManager.Remove(filePath);

                _log.Error(ex.ToString());
                Program.SendCrashReport(ex);
            }
            UpdateFormUI();
        }
        public void AddDocument(IDocument doc, bool newDoc)
        {
            // add this form as document listener
            DocumentSB docSB = doc as DocumentSB;
            docSB?.AddListener(this);
            // add document 
            Documents.Add(doc);

            // insert database items
            if (newDoc && WCFClient.IsConnected)
                DatabaseHelpers.InsertDefaultItems(docSB);

            doc.Modified += new EventHandler(OnDocumentModified);
            UpdateToolbarState();
        }
        public void SaveDocument()
        {
            IDocument doc = ActiveDocument;
            if (null == doc) return;
            CancelEventArgs e = new CancelEventArgs();
            SaveDocument(doc, e);
        }
        public void SaveDocument(IDocument doc, CancelEventArgs e)
        {
            if (doc.IsNew)
                SaveDocumentAs(doc, e);
            else
                doc.Save();
        }
        public void SaveAllDocuments()
        {
            CancelEventArgs e = new CancelEventArgs();
            SaveAllDocuments(e);
        }
        public void SaveAllDocuments(CancelEventArgs e)
        {
            if (e.Cancel) return;
            foreach (IDocument doc in Documents)
            {
                try { SaveDocument(doc, e); }
                catch (Exception ex) {  _log.Error(ex.Message); }
            }
        }
        public void CloseAllDocuments(CancelEventArgs e)
        {
            // exit if already canceled
            if (e.Cancel) return;

            // try and close every opened documents
            while (Documents.Count > 0)
            {
                if (e.Cancel) return;
                IDocument doc = Documents[0];
                CloseDocument(doc, e);
            }
        }
        public void CloseDocument(IDocument doc, CancelEventArgs e)
        {
            // exit if already canceled
            if (e.Cancel)
                return;
            if (doc.IsDirty)
            {
                DialogResult res = MessageBox.Show(string.Format(Resources.ID_SAVEMODIFIEDFILE, doc.Name), Application.ProductName, MessageBoxButtons.YesNoCancel);
                if (DialogResult.Yes == res)
                {
                    SaveDocument(doc, e);
                    e.Cancel = false;
                }
                else if (DialogResult.No == res)
                    e.Cancel = false;
                else if (DialogResult.Cancel == res)
                    e.Cancel = true;
            }
            if (e.Cancel)
                return;
            // close document
            doc.Close();
            // remove from document list
            Documents.Remove(doc);
            // handle the case
            _log.Debug(string.Format("Document {0} closed", doc.Name));
            // update toolbar
            UpdateToolbarState();
        }
        public void SaveDocumentAs(IDocument doc, CancelEventArgs e)
        {
            saveFileDialogSB.FileName = doc.Name + ".stb";
            if (saveFileDialogSB.ShowDialog() == DialogResult.OK)
                doc.SaveAs(saveFileDialogSB.FileName);
            else
                e.Cancel = true;
        }
        public void SaveDocumentAs()
        {
            IDocument doc = ActiveDocument;
            if (null == doc) return;
            CancelEventArgs e = new CancelEventArgs();
            SaveDocumentAs(doc, e);
        }
        public void CloseDocument()
        {
            IDocument doc = ActiveDocument;
            if (null == doc) return;

            // close active document
            CancelEventArgs e = new CancelEventArgs();
            CloseDocument(doc, e);
        }
        /// <summary>
        /// Access list of documents
        /// </summary>
        public List<IDocument> Documents { get; } = new List<IDocument>();

        public bool HasDocuments => Documents.Count > 0;
        /// <summary>
        /// Get active view based on ActiveMdiChild
        /// </summary>
        public IView ActiveView
        {
            get
            {
                // search ammong existing views
                foreach (IDocument doc in Documents)
                    foreach (IView view in doc.Views)
                    {
                        Form form = view as Form;
                        if (ActiveMdiChild == form)
                            return view;
                    }
                return null;
            }            
        }
        public IDocument ActiveDocument
        {
            get
            {
                IView view = ActiveView;
                if (view != null)
                    return view.Document;
                else if (Documents.Count == 0)
                    return null;
                else if (Documents.Count == 1)
                    return Documents[0];
                else
                {
                    // try and disambiguate
                    IDocument doc = null;
                    var form = new FormDocDisambiguate() { StartPosition = FormStartPosition.CenterParent };
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        doc = form.Document;
                    }
                    if (null != doc)
                        return doc;
                    else
                    {
                        _log.Warn(message: "No active document!");
                        return null;
                    }
                }
            }
        }
        public DocumentSB ActiveDocumentSB => ActiveDocument as DocumentSB;
        public DocumentSB FirstDocument => Documents.Count > 0 ? Documents[0] as DocumentSB : null;
        public DocumentSB ActiveDocumentOrFirst
        {
            get
            {
                IView view = ActiveView;
                if (view != null)
                    return view.Document as DocumentSB;
                else if (Documents.Count > 0)
                    return Documents[0] as DocumentSB;
                else
                    return null;
            }
        }
        /// <summary>
        /// Returns true if at least one document is dirty
        /// </summary>
        public bool OneDocDirty
        {
            get
            {
                foreach (IDocument doc in Documents)
                    if (doc.IsDirty)
                        return true;
                return false;
            }
        }
        #endregion
        #region IDocumentListener implementation
        // new
        public void OnNewDocument(Document doc) { doc.DocumentClosed += OnDocumentClosed; }
        public void OnNewTypeCreated(Document doc, ItemBase itemBase) { }
        public void OnNewAnalysisCreated(Document doc, Analysis analysis) => CreateOrActivateViewAnalysis(analysis);
        public void OnAnalysisUpdated(Document doc, Analysis analysis) => CreateOrActivateViewAnalysis(analysis);
        public void OnNewAnalysisCreated(Document doc, AnalysisHetero analysis) => CreateOrActivateViewHAnalysis(analysis);
        public void OnAnalysisUpdated(Document doc, AnalysisHetero analysis) =>  CreateOrActivateViewHAnalysis(analysis);
        // remove
        public void OnTypeRemoved(Document doc, ItemBase itemBase) { }
        public void OnAnalysisRemoved(Document doc, ItemBase itemBase) { }
        // close
        public void OnDocumentClosed(Document doc)
        {
            doc.DocumentClosed -= OnDocumentClosed;
        }
        #endregion
        #region Connection / disconnection
        private void OnConnectionAvoided()
        {
            // create basic layout
            CreateBasicLayout();
            UpdateDisconnectButton();
        }
        private void OnConnected()
        {
            if (!Program.UseDisconnected && WCFClient.IsConnected)
            {
                using (WCFClient wcfClient = new WCFClient())
                {
                    var client = wcfClient.Client;
                    if (null != client)
                    {
                        DCGroup currentGroup = client.GetCurrentGroup();
                        Program.IsSubscribed = currentGroup.IsSubscribed;
                        if (null != currentGroup)
                            Text = Application.ProductName
                                + (Program.IsSubscribed ? " **Premium**" : string.Empty)
                                + " - (" + currentGroup.Name + "\\" + wcfClient.User.Name + ")";
                    }
                    else
                    {
                        Text = Application.ProductName;
                    }
                }
            }
            toolStripSB_BCT.Enabled = WCFClient.IsConnected;
            toolStripSplitButtonExcelProcessors.Enabled = WCFClient.IsConnected;
            toolStripSBExcelOneCasePerRow.Enabled = WCFClient.IsConnected;


            toolStripButtonPremium.Visible = !Program.IsSubscribed;
            toolStripMIPremium.Visible = !Program.IsSubscribed;
            toolStripMIPremiumSep.Visible = !Program.IsSubscribed;

            // create basic layout
            CreateBasicLayout();
            UpdateDisconnectButton();
         }
        private void OnDisconnected()
        {
            Text = Application.ProductName;
            ClearBasicLayout();
            UpdateDisconnectButton();
        }
        private void UpdateDisconnectButton()
        {
        }
        #endregion
        #region File menu event handlers
        private void FileClose(object sender, EventArgs e)
        {
            IDocument doc = ActiveDocument;
            CancelEventArgs cea = new CancelEventArgs();
            CloseDocument(doc, cea);
        }
        private void FileNew(object sender, EventArgs e)
        {
            CloseStartPage();
            NewDocument();
        }
        private void FileOpen(object sender, EventArgs e)
        {
            CloseStartPage();
            if (DialogResult.OK == openFileDialogSB.ShowDialog())
                foreach(string fileName in openFileDialogSB.FileNames)
                    OpenDocument(fileName);            
        }
        private void FileSave(object sender, EventArgs e)
        { try { SaveDocument(); } catch (Exception ex) { _log.Error(ex.ToString()); } }
        private void FileSaveAs(object sender, EventArgs e)
        { try { SaveDocumentAs(); } catch (Exception ex) { _log.Error(ex.ToString()); } }
        private void FileSaveAll(object sender, EventArgs e)     {   SaveAllDocuments();            }
        private void FileExit(object sender, EventArgs e)
        {
            CancelEventArgs cea = new CancelEventArgs();
            CloseAllDocuments(cea);
            Close();
            Application.Exit(); 
        }
        public void OpenMRUFile(string filePath)
        {
            CloseStartPage();
            // open file
            OpenDocument(filePath); // -> exception handled in OpenDocument
        }
        private void OnBecomeAPremiumUser(object sender, EventArgs e)
        {
            try
            {
                var form = new FormBecomePremiumUser();
                if (DialogResult.Cancel == form.ShowDialog()) {}
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion
        #region Tools
        #region User
        private void OnUserClicked(object sender, EventArgs e)
        {
            toolStripSBUser.ShowDropDown();
        }
        #endregion
        #region Items
        private void ToolAddNewBox(object sender, EventArgs e)
        {
            try { ActiveDocumentSB.CreateNewBoxUI();    }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewCase(object sender, EventArgs e)
        {
            try { ActiveDocumentSB.CreateNewCaseUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewPack(object sender, EventArgs e)
        {
            try { ActiveDocumentSB.CreateNewPackUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewBag(object sender, EventArgs e)
        {
            try { ActiveDocumentSB.CreateNewBagUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewBundle(object sender, EventArgs e)
        {
            try { ActiveDocumentSB.CreateNewBundleUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewCylinder(object sender, EventArgs e)
        {
            try { ActiveDocumentSB.CreateNewCylinderUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewBottle(object sender, EventArgs e)
        {
            try { ActiveDocumentSB.CreateNewBottleUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewPallet(object sender, EventArgs e)
        {
            try { ActiveDocumentSB.CreateNewPalletUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewTruck(object sender, EventArgs e)
        {
            try { ActiveDocumentSB.CreateNewTruckUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        #endregion
        #region Pallet decorations
        private void OnPalletAccessories(object sender, EventArgs e)
        {
            toolStripSBPalletDeco.ShowDropDown();
        }
        private void ToolAddNewInterlayer(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewInterlayerUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewPalletCap(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewPalletCapUI(); }
            catch (Exception ex) { _log.Error(ex.ToString());  Program.SendCrashReport(ex); }
        }
        private void ToolAddNewPalletCorners(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewPalletCornersUI(); }
            catch (Exception ex) { _log.Error(ex.ToString());  Program.SendCrashReport(ex); }
        }
        private void ToolAddNewPalletFilm(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewPalletFilmUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void ToolAddNewPalletLabel(object sender, EventArgs e)
        {
            try { ((DocumentSB)ActiveDocument).CreateNewPalletLabelUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        #endregion
        #region Analyses
        private void OnAnalysisPallet(object sender, EventArgs e)
        {
            toolStripSBAnalysisPallet.ShowDropDown();
        }
        private void OnAnalysisCase(object sender, EventArgs e)
        {
            toolStripSBAnalysesCase.ShowDropDown();
        }
        private void OnAnalysisTrucks(object sender, EventArgs e)
        {
            toolStripSBAnalysesTruck.ShowDropDown();
        }
        private void OnAnalysisOpti(object sender, EventArgs e)
        {
            toolStripSBOptimisations.ShowDropDown();
        }
        private void OnNewAnalysisBoxCase(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewAnalysisBoxCaseUI(); }
            catch (Exception ex){ _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisCylinderCase(object sender, EventArgs e)
        { 
            try { ActiveDocumentSB?.CreateNewAnalysisCylinderCaseUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisPalletTruck(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewAnalysisPalletTruckUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewHAnalysisPalletTruck(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewAnalysisHPalletTruckUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisCasePallet(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewAnalysisCasePalletUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisCylinderPallet(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewAnalysisCylinderPalletUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisBoxCasePallet(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewAnalysisBoxCaseUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisCaseTruck(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewAnalysisCaseTruckUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisCylinderTruck(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewAnalysisCylinderTruckUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisHCylinderTruck(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewAnalysisHCylinderTruckUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewHAnalysisPallet(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewHAnalysisPalletUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewHAnalysisTruck(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewHAnalysisTruckUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewAnalysisHCylPallet(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewAnalysisHCylPalletUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewPalletsOnPallet(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewPalletsOnPalletUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnNewPalletOnPalletVertical(object sender, EventArgs e)
        {
            try { ActiveDocumentSB?.CreateNewPalletOnPalletUI(); }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        #endregion
        #region Optimisation
        private void OnOptiPack(object sender, EventArgs e)
        {
            try
            {
                // show optimisation form
                FormOptimizePack form = new FormOptimizePack(ActiveDocumentSB);
                form.ShowDialog();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnOptiSelectCase(object sender, EventArgs e)
        {
            try
            {
                // load cases from the database
                ProgressWindow progress = new ProgressWindow();
                ThreadPool.QueueUserWorkItem(new WaitCallback(LoadCases), progress);
                progress.ShowDialog();

                FormOptimiseMultiCase form = new FormOptimiseMultiCase()
                {
                    Document = ActiveDocumentSB,
                    ListCases = ListDBCases
                };
                form.ShowDialog();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void LoadCases(object status)
        {
            try
            {
                IProgressCallback callback = status as IProgressCallback;
                callback.Begin();
                ListDBCases.Clear();

                using (WCFClient wcfClient = new WCFClient())
                {
                    var client = wcfClient.Client;
                    if (null != client)
                    {
                        int rangeIndex = 0, number = 0;
                        bool endReached = false;
                        bool firstCall = true;
                        while (!endReached)
                        {
                            try
                            {
                                if (callback.IsAborting)
                                {
                                    endReached = true;
                                    break;
                                }
                                ListDBCases.AddRange(client.GetAllCases(rangeIndex++, ref number));

                                if (firstCall)
                                {
                                    firstCall = false;
                                    callback.SetRange(0, number);
                                }
                                callback.StepTo((rangeIndex - 1) * 20);
                                callback.SetText($"Loading case {rangeIndex * 20} of {number}");

                                endReached = (rangeIndex * 20 > number);
                            }
                            catch (Exception /*ex*/) {}
                        }
                    }
                }
                callback.End();
            }
            catch (Exception /*ex*/) {}
        }
        #endregion
        #region Database / settings / excel sheet
        private void OnShowDatabase(object sender, EventArgs e)
        {
            try
            {
                if (Program.UseDisconnected)
                    return;
                // show database browser
                var form = new FormShowDatabase();
                form.ShowDialog();
                // update toolbar state as database may now be empty
                UpdateToolbarState();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnShowMaterialList(object sender, EventArgs e)
        {
            try
            {
                if (Program.UseDisconnected)
                    return;
                // show material list browser
                EdgeCrushTest.ECT_Forms.EditMaterialList();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnShowSettings(object sender, EventArgs e)
        {
            try
            {
                // show OptionsFormSettings
                FormOptionsSettings form = new FormOptionsSettings();
                form.ShowDialog();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnLoadExcelSheet(object sender, EventArgs e)
        {
            try
            {
                DocumentSB doc = ActiveDocumentSB;
                if (null == doc) return;
                FormExcelLibrary form = new FormExcelLibrary(doc);
                form.ShowDialog();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        #endregion
        #region PLMPackLib
        private void OnPLMPackLib(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(UrlPLMPackLib))
                    Process.Start("rundll32.exe", "dfshim.dll,ShOpenVerbApplication " + UrlPLMPackLib);
            }
            catch (Exception ex) { _log.Error(ex.Message); }
        }
        #endregion
        #region Edge Crush Test
        private void OnComputeCaseBCT(object sender, EventArgs e) => EdgeCrushTest.ECT_Forms.ComputeBCTCase(dockPanel);
        private void OnComputePalletBCT(object sender, EventArgs e) => EdgeCrushTest.ECT_Forms.ComputeBCTPallet(dockPanel);
        #endregion
        #region Export to Excel
        private void OnExportToExcel(object sender, EventArgs e)
        {
            try { ExporterProjetSummary.ExportProjectSummaryToExcel((DocumentSB)ActiveDocument);  }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                _log.Error($"MS Excel not installed? : {ex.Message}");
                MessageBox.Show("MS Excel not installed?");
            }
            catch (Exception ex)
            { _log.Error(ex.ToString()); Program.SendCrashReport(ex); }
        }
        private void OnExelSheetOneCasePerRow(object sender, EventArgs e)
        {
            try
            {
                var form = new FormExcelMassAnalysisCasePallet();
                if (DialogResult.OK == form.ShowDialog())  {}
            }
            catch (Exception ex)
            { _log.Error(ex.ToString()); }
        }
        private void OneExcelSheetOnePackPerRow(object sender, EventArgs e)
        {
            try
            {
                var form = new FormExcelMassAnalysisPackPallet();
                if (DialogResult.OK == form.ShowDialog()) {}
            }
            catch (Exception ex)
            { _log.Error(ex.ToString()); }
        }
        #endregion
        #endregion
        #region Document / View status change handlers
        void OnDocumentModified(object sender, EventArgs e)
        {
            UpdateToolbarState();
        }
        #endregion
        #region Form activation/creation
        public void CreateOrActivateViewAnalysis(Analysis analysis)
        {
            // ---> search among existing views
            // ---> activate if found
            foreach (IDocument doc in Documents)
                foreach (IView view in doc.Views)
                {
                    if (view is DockContentAnalysisEdit dockContentAnalysisEdit && dockContentAnalysisEdit.Analysis == analysis)
                    {
                        dockContentAnalysisEdit.Activate();
                        return;
                    }
                    if (view is DockContentAnalysisPalletTruck dockContentAnalysisPalletTruck && analysis is AnalysisPalletTruck analysisPalletTruck)
                    {
                        if (analysisPalletTruck == dockContentAnalysisPalletTruck.Analysis)
                        {
                            dockContentAnalysisPalletTruck.Activate();
                            return;
                        }
                    }
                    if (view is DockContentAnalysisPalletsOnPallet dockContentAnalysisPalletsOnPallet && analysis is AnalysisPalletsOnPallet analysisPalletsOnPallet)
                    {
                        if (analysisPalletsOnPallet == dockContentAnalysisPalletsOnPallet.Analysis)
                        {
                            dockContentAnalysisPalletsOnPallet.Activate();
                            return;
                        }
                    }
                    if (view is DockContentAnalysisPalletColumn dockContentAnalysisPalletColumn && analysis is AnalysisPalletColumn analysisPalletColumn)
                    {
                        if (analysisPalletColumn == dockContentAnalysisPalletColumn.Analysis)
                        {
                            dockContentAnalysisPalletColumn.Activate();
                            return;
                        }
                    }
                }
            // ---> not found : create new form
            // get document
            DocumentSB parentDocument = (DocumentSB)analysis.ParentDocument;
            // create form and show
            DockContentView formAnalysis = parentDocument.CreateViewAnalysis(analysis);
            if (null != formAnalysis)
                formAnalysis.Show(dockPanel, DockState.Document);
        }
        public void CreateOrActivateViewHAnalysis(AnalysisHetero analysis)
        {
            // ---> search among existing views
            // ---> activate if found
            foreach (IDocument doc in Documents)
                foreach (IView view in doc.Views)
                {
                    if (view is DockContentHAnalysis formAnalysis && formAnalysis.Analysis == analysis)
                    {
                        formAnalysis.Activate();
                        return;
                    }
                }
            // ---> not found : create new form
            DocumentSB parentDocument = analysis.ParentDocument as DocumentSB;
            // create form and show
            DockContentView formHAnalysis = parentDocument.CreateViewHAnalysis(analysis);
            if (null != formHAnalysis)
                formHAnalysis.Show(dockPanel, DockState.Document);
        }
        #endregion
        #region Helpers
        public string AssemblyConf
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyConfigurationAttribute confAttribute = (AssemblyConfigurationAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (!string.IsNullOrEmpty(confAttribute.Configuration))
                        return confAttribute.Configuration.ToLower();
                }
                return "release";
            }
        }
        private IDockContent FindDocument(string text)
        {
            foreach (IDockContent content in dockPanel.Contents)
            {
                if (content.DockHandler.TabText == text)
                    return content;
            }
            return null;
        }
        #endregion
        #region Help menu event handlers
        private void OnAbout(object sender, EventArgs e)
        {
            using (AboutBox form = new AboutBox() { CompanyUrl = Settings.Default.CompanyUrl, SupportEmail = Settings.Default.EmailSupport })
            {   form.ShowDialog(); }
        }
        private void OnOnlineHelp(object sender, EventArgs e)
        {
            try { Process.Start(Settings.Default.HelpPageUrl); }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnDisconnect(object sender, EventArgs e)
        {
            try
            {
                WCFClient.Disconnect();
                // start login again
                timerLogin.Start();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion
        #region Private data members
        private List<DCSBCase> ListDBCases { get; set; } = new List<DCSBCase>();
        #endregion
        #region Static instance accessor
        public static FormMain GetInstance()  { return _instance; }
        #endregion


    }
}
