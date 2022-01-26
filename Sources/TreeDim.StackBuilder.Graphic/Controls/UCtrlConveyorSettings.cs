#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics.Controls
{
    #region UCtrlConveyorSettings
    public partial class UCtrlConveyorSettings : UserControl
    {
        #region Events
        public delegate void ValueChangedDelegate(object sender, EventArgs e);
        public event ValueChangedDelegate ValueChanged;

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
        private void OnSettingsChanged(object sender, EventArgs e)
        {
            graph3DConveyor.Packable = BoxProperties;
            graph3DConveyor.CaseAngle = AngleCase;
            graph3DConveyor.MaxDropNumber = MaxDropNumber;
            graph3DConveyor.Invalidate();
            ValueChanged?.Invoke(this, e);
        }
        private void FillListBox()
        {
            lbConveyorSetting.Packable = BoxProperties;
            lbConveyorSetting.Items.Clear();
            lbConveyorSetting.Items.AddRange(ListSettings.ToArray());
            lbConveyorSetting.SelectedIndex = lbConveyorSetting.Items.Count  > 0 ? 0 : 1;
        }
        private void OnSettingAdd(object sender, EventArgs e)
        {
            ListSettings.Add(new ConveyorSetting(AngleCase, MaxDropNumber));
            FillListBox();
            ConveyorSettingAddedRemoved?.Invoke(this, e);
        }
        private void OnSettingRemove(object sender, EventArgs e)
        {
            int iSel = lbConveyorSetting.SelectedIndex;
            ListSettings.RemoveAt(iSel);
            FillListBox();
            ConveyorSettingAddedRemoved?.Invoke(this, e);
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
                OnSettingsChanged(this, null);
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
        private List<ConveyorSetting> ListSettings { get; set; }
        private PackableBrick _packable;
        #endregion
    }
    #endregion
}
