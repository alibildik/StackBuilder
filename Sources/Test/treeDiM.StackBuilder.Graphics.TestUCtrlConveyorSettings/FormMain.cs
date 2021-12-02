#region Using directives
using System;
using System.Windows.Forms;
using System.Drawing;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
#endregion

namespace treeDiM.StackBuilder.Graphics.TestUCtrlConveyorSettings
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var box = new BoxProperties(null, 400.0, 300.0, 200.0, 1.0, Color.Green) { Facing = 0 };
            uCtrlConveyor.BoxProperties = box;

        }
    }
}
