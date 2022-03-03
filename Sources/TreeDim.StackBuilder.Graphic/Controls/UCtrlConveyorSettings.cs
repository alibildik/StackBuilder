#region Using directives
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics.Controls
{
    #region UCtrlConveyorSettings
    public partial class UCtrlConveyorSettings : UserControl
    {
        #region Events
        public delegate void ConveyorSettingAddRemoveDelegate(object sender, EventArgs e);
        public event ConveyorSettingAddRemoveDelegate ConveyorSettingAddedRemoved;
        #endregion
        #region Constructor
        public UCtrlConveyorSettings()
        {
            InitializeComponent();
        }
        #endregion
        #region Event handlers
        private void OnAddSettingsChanged(object sender, EventArgs e)
        {
            graph3DConveyor.Packable = BoxProperties;
            graph3DConveyor.CaseAngle = AngleCase;
            graph3DConveyor.MaxDropNumber = MaxDropNumber;
            graph3DConveyor.GripperAngle = GripperAngle;
            graph3DConveyor.Invalidate();

            UpdateModifyAddButton();
        }
        private void FillListBox()
        {
            ListSettings = ListSettings.OrderBy(i => i.Number).ToList();

            lbConveyorSetting.Packable = BoxProperties;
            lbConveyorSetting.Items.Clear();
            lbConveyorSetting.Items.AddRange(ListSettings.ToArray());
            lbConveyorSetting.SelectedIndex = lbConveyorSetting.Items.Count  > 0 ? 0 : -1;

            // ListSettings updated => Can we still add?
            UpdateModifyAddButton();
            UpdateRemoveButton();
        }

        private ConveyorSetting CurrentlyEnteredSetting => new ConveyorSetting(AngleCase, MaxDropNumber, GripperAngle);
        private int SelectedIndexLb => lbConveyorSetting.SelectedIndex;
        private void UpdateModifyAddButton()
        {
            // sanity check
            if (null == ListSettings) return;
            // setting already exists ?
            bnModify.Enabled = ConveyorSetting.CanUpdate(ListSettings, CurrentlyEnteredSetting, SelectedIndexLb);
            bnAdd.Enabled = ConveyorSetting.CanAdd(ListSettings, CurrentlyEnteredSetting);

            nudMaxNumber.Enabled = ConveyorSetting.CanEditNumber(ListSettings, SelectedIndexLb);            
        }
        private void UpdateRemoveButton()
        {
            bnRemove.Enabled = (null == ListSettings) || ConveyorSetting.CanRemove(ListSettings, SelectedIndexLb);
        }
        private void OnSettingApply(object sender, EventArgs e)
        {
            int iSel = lbConveyorSetting.SelectedIndex;
            if (iSel != -1 && ConveyorSetting.CanUpdate(ListSettings, CurrentlyEnteredSetting, iSel))
                ListSettings[iSel] = CurrentlyEnteredSetting;

            FillListBox();
            ConveyorSettingAddedRemoved?.Invoke(this, e);
        }
        private void OnSettingAdd(object sender, EventArgs e)
        {
            if (!ConveyorSetting.CanAdd(ListSettings, CurrentlyEnteredSetting))
                return;
            // add new conveyor setting
            ListSettings.Add(CurrentlyEnteredSetting);
            // updating listbox
            FillListBox();
            ConveyorSettingAddedRemoved?.Invoke(this, e);
        }
        private void OnSettingRemove(object sender, EventArgs e)
        {
            if (SelectedIndexLb != -1) ListSettings.RemoveAt(SelectedIndexLb);

            FillListBox();
            ConveyorSettingAddedRemoved?.Invoke(this, e);
        }

        private void OnSelectedSettingChanged(object sender, EventArgs e)
        {
            // selected setting
            int iSel = lbConveyorSetting.SelectedIndex;
            if (-1 == iSel) return;
            var setting = ListSettings[iSel];
            if (setting != null)
            {
                MaxDropNumber = setting.Number;
                AngleCase = setting.Angle;
                GripperAngle = setting.GripperAngle;
            }
            UpdateRemoveButton();
        }
        #endregion
        #region Public properties
        public PackableBrick BoxProperties
        {
            get => _packable;
            set
            {
                _packable = value;
                if (null == _packable) return;
                OnAddSettingsChanged(this, null);
            }
        }
        public int AngleCase
        {
            get => (int)nudCaseAngle.Value;
            set => nudCaseAngle.Value = value;
        }
        public int MaxDropNumber
        {
            get => (int)nudMaxNumber.Value;
            set => nudMaxNumber.Value = value;
        }
        public int GripperAngle
        {
            get => (int)nudGripperAngle.Value;
            set => nudGripperAngle.Value = value;
        }
        #endregion
        #region Initialization method
        public void SetConveyorSettings(PackableBrick packable, List<ConveyorSetting> listSettings)
        {
            BoxProperties = packable;
            ListSettings = listSettings;
            FillListBox();
        }
        #endregion
        #region Data members
        public List<ConveyorSetting> ListSettings { get; set; }
        private PackableBrick _packable;

        #endregion
    }
    #endregion
}
