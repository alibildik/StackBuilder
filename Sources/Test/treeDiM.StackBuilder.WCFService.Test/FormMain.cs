#region Using directives
using System;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
#endregion

namespace treeDiM.StackBuilder.WCFService.Test
{
    public partial class FormMain : Form
    {
        #region Constructor
        public FormMain()
        {
            InitializeComponent();
        }
        #endregion
        #region Override form
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var logConsole = new DockContentLogConsole();
            logConsole.Show(dockPanel, DockState.DockBottom);
            var formHomo = new FormTestHomogeneous();
            formHomo.Show(dockPanel, DockState.Document);
            var formHetero = new FormTestHeterogeneous();
            formHetero.Show(dockPanel, DockState.Document);
            var formTestJJA = new FormTestJJA();
            formTestJJA.Show(dockPanel, DockState.Document);
        }
        #endregion
    }
}
