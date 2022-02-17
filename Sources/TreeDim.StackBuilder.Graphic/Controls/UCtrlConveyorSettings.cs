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

            UpdateAddButton();
        }
        private void FillListBox()
        {
            ListSettings = ListSettings.OrderBy(i => i.Number).ToList();

            lbConveyorSetting.Packable = BoxProperties;
            lbConveyorSetting.Items.Clear();
            lbConveyorSetting.Items.AddRange(ListSettings.ToArray());
            lbConveyorSetting.SelectedIndex = lbConveyorSetting.Items.Count  > 0 ? 0 : -1;

            // ListSettings updated => Can we still add?
            UpdateAddButton();
            UpdateRemoveButton();
        }
        private void UpdateAddButton()
        { 
            bnAdd.Enabled = (null == ListSettings) || !ConveyorSetting.FindSetting(ListSettings, new ConveyorSetting(AngleCase, MaxDropNumber, GripperAngle), -1);        
        }
        private void UpdateRemoveButton()
        {
            bnRemove.Enabled = (null == ListSettings) || ConveyorSetting.CanRemove(ListSettings, lbConveyorSetting.SelectedIndex);
        }
        private void OnSettingAdd(object sender, EventArgs e)
        {
            ListSettings.Add(new ConveyorSetting(AngleCase, MaxDropNumber, GripperAngle));

            FillListBox();

            ConveyorSettingAddedRemoved?.Invoke(this, e);
        }
        private void OnSettingRemove(object sender, EventArgs e)
        {
            int iSel = lbConveyorSetting.SelectedIndex;
            if (iSel != -1) ListSettings.RemoveAt(iSel);
            FillListBox();
            ConveyorSettingAddedRemoved?.Invoke(this, e);
        }
        private void OnSettingEdit(object sender, EventArgs e)
        {
            int iSel = lbConveyorSetting.SelectedIndex;
            if (iSel != -1)
            {
                var form = new FormEditConveyorSetting()
                {
                    Packable = BoxProperties,
                    ListSettings = ListSettings,
                    SelectedIndex = iSel 
                };
                if (DialogResult.OK == form.ShowDialog())
                    ListSettings[iSel] = form.ItemSettings;
            }
            FillListBox();
            ConveyorSettingAddedRemoved?.Invoke(this, e);
        }
        private void OnSelectedSettingChanged(object sender, EventArgs e) => UpdateRemoveButton();
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
