#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public partial class Graphics3DConveyor : UserControl
    {
        public Graphics3DConveyor()
        {
            InitializeComponent();
            // double buffering
            SetDoubleBuffered();
            SetStyle(ControlStyles.Selectable, true);
            TabStop = false;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (null == Packable) return;

            var imageGenerator = new ImageGenConveyorSetting() { };
            var graphics = new Graphics3DForm(this, e.Graphics)
            {
                ShowFacing = true,
                FontSizeRatio = 0.1f,
                Target = Vector3D.Zero,
                CameraPosition = imageGenerator.CameraPosition
            };
            imageGenerator.Draw(graphics, Packable, MaxDropNumber, CaseAngle, GripperAngle);

        }
        #region Double buffering
        private void SetDoubleBuffered()
        {
            System.Reflection.PropertyInfo aProp =
                typeof(Control).GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            aProp.SetValue(this, true, null);
        }
        #endregion

        #region Data members
        public int MaxDropNumber { get; set; }
        public int CaseAngle { get; set; }
        public int GripperAngle { get; set; }
        public PackableBrick Packable { get; set; }
        #endregion
    }
}
