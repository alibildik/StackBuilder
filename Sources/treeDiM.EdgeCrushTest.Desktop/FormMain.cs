#region Using directive
using System;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;

using log4net;
using WeifenLuo.WinFormsUI.Docking;
#endregion

namespace treeDiM.EdgeCrushTest.Desktop
{
    public partial class FormMain : Form
    {
        #region Constructor
        public FormMain()
        {
            InitializeComponent(); 
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Set DockPanel properties
            var theme = new VS2015BlueTheme();
            dockPanel.Theme = theme;

            if (AssemblyConf == "debug")
                OnShowLogConsole(this, null);
            
            // or show splash sceen 
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
            timerMain.Start();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
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
        #region Event handlers
        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void OnAbout(object sender, EventArgs e)
        {
            using (AboutBox form = new AboutBox() {})
            { form.ShowDialog(); }
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            timerMain.Stop();
            var dockContentReverseBCT = new DockContentBCTCase();
            dockContentReverseBCT.Show(dockPanel, DockState.Document);
            var dockContentComputeBCT = new DockContentBCTPalletisation();
            dockContentComputeBCT.Show(dockPanel, DockState.Document);
        }
        private void OnMode1(object sender, EventArgs e)
        {
            CloseAllDockedForms();
            var dockContentComputeBCT = new DockContentBCTPalletisation();
            dockContentComputeBCT.Show(dockPanel, DockState.Document);
        }
        private void OnMode2(object sender, EventArgs e)
        {
            CloseAllDockedForms();
            var dockContentReverseBCT = new DockContentBCTCase();
            dockContentReverseBCT.Show(dockPanel, DockState.Document);
        }
        private void OnEditMaterialList(object sender, EventArgs e)
        {
            CloseAllDockedForms();
            ECT_Forms.EditMaterialList();
        }
        private void OnEditPalletList(object sender, EventArgs e)
        {
            CloseAllDockedForms();
            ECT_Forms.EditPalletList();
        }
        private void OnShowLogConsole(object sender, EventArgs e)
        {
            if (null == FindDocument("Log console"))
            {
                _logConsole = new DockContentLogConsole
                {
                    TabText = "Log console",
                    Text = "Log console"
                };
                _logConsole.Show(dockPanel, DockState.DockBottom);
            }
        }
        #endregion
        #region Helpers
        private IDockContent FindDocument(string text)
        {
            foreach (IDockContent content in dockPanel.Contents)
            {
                if (content.DockHandler.TabText == text)
                    return content;
            }
            return null;
        }
        private void CloseAllDockedForms()
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    form.Close();
            }
            else
            {
                foreach (IDockContent document in dockPanel.DocumentsToArray())
                {
                    // IMPORANT: dispose all panes.
                    document.DockHandler.DockPanel = null;
                    document.DockHandler.Close();
                }
            }
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
        #endregion
        #region Data members
        private static ILog _log = LogManager.GetLogger(typeof(FormMain));
        private DockContentLogConsole _logConsole;

        #endregion


    }
}
