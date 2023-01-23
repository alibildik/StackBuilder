#region Using directives
using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

using log4net;

using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class OptionPanelDebugging : GLib.Options.OptionsPanel
    {
        #region Constructor
        public OptionPanelDebugging()
        {
            InitializeComponent();

            CategoryPath = Resources.ID_OPTIONSDEBUGGING;
            DisplayName = Resources.ID_DISPLAYDEBUGGING;
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // initialize controls
            chkShowLogConsole.Checked = Settings.Default.ShowLogConsole;
            chkbShowBoxNumbers.Checked = Settings.Default.ShowBoxIds;
            // events
            OptionsForm.OptionsSaving += new EventHandler(OnOptionsSaving);
        }
        #endregion

        #region Handlers
        private void OnShowLogConsoleCheckChanged(object sender, EventArgs e)
        {
            // show or hide log console
            FormMain form = FormMain.GetInstance();
            form.ShowLogConsole();
        }

        private void OnShowApplicationFolder(object sender, EventArgs e)
        {
            try
            {   Process.Start(Path.GetDirectoryName(Application.ExecutablePath)); }
            catch (Exception ex)
            { _log.Error(ex.Message); }
        }
        private void OnResetDefaultSettings(object sender, EventArgs e)
        {
            try
            {
                Settings.Default.Reset();
            }
            catch (Exception ex)
            { _log.Error(ex.Message); }
        }
        private void OnOptionsSaving(object sender, EventArgs e)
        {
            Settings.Default.ShowBoxIds = chkbShowBoxNumbers.Checked;
            Settings.Default.ShowLogConsole = chkShowLogConsole.Checked;
            Settings.Default.Save();
        }
        #endregion

        #region Data members
        static ILog _log = LogManager.GetLogger(typeof(OptionPanelDebugging));
        #endregion
    }
}
