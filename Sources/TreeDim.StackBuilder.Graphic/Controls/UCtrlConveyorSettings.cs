#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;

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
        #endregion
        #region Public properties
        public PackableBrick BoxProperties
        {
            get => _packable;
            set
            {
                _packable = value;
                if (null == _packable)
                    return;
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
        public int AngleGrabber
        {
            get => (int)nudGrabberAngle.Value;
            set => nudGrabberAngle.Value = value;
        }
        #endregion
        #region Data members
        private PackableBrick _packable;
        #endregion
    }
    #endregion
}
