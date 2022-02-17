#region Using directives
using System.Windows.Forms;
using System.Collections.Generic;

using treeDiM.StackBuilder.Basics;
using System;
#endregion

namespace treeDiM.StackBuilder.Graphics.Controls
{
    public partial class FormEditConveyorSetting : Form
    {
        #region Constructor
        public FormEditConveyorSetting()
        {
            InitializeComponent();
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ItemSettings = ListSettings[SelectedIndex].Clone();

            Number = ItemSettings.Number;
            CaseAngle = ItemSettings.Angle;
            GripperAngle = ItemSettings.GripperAngle;

            bool canRemove = ConveyorSetting.CanRemove(ListSettings, SelectedIndex);
            lbMaxNumber.Enabled = canRemove;
            nudMaxNumber.Enabled = canRemove;

            OnItemChanged(this, EventArgs.Empty);
        }
        #endregion
        #region Private properties
        private int Number { get => (int)nudMaxNumber.Value; set => nudMaxNumber.Value = value; }
        private int CaseAngle { get => (int)nudCaseAngle.Value; set => nudCaseAngle.Value = value; }
        private int GripperAngle { get => (int)nudGripperAngle.Value; set => nudGripperAngle.Value = value; }
        #endregion
        #region Event handlers
        private void OnItemChanged(object sender, EventArgs e)
        {
            graph3DConveyor.Packable = Packable;
            graph3DConveyor.MaxDropNumber = Number;
            graph3DConveyor.CaseAngle = CaseAngle;
            graph3DConveyor.GripperAngle = GripperAngle;
            graph3DConveyor.Invalidate();

            bnOK.Enabled = !ConveyorSetting.FindSetting(ListSettings, new ConveyorSetting(CaseAngle, Number,GripperAngle), SelectedIndex);

            ItemSettings = new ConveyorSetting(CaseAngle, Number, GripperAngle);
        }
        #endregion
        #region Data members
        public int SelectedIndex { get; set; }
        public PackableBrick Packable { get; set; }
        public ConveyorSetting ItemSettings { get; set; }
        public List<ConveyorSetting> ListSettings { get; set; }
        #endregion
    }
}
