#region Using directives
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormEditLayer : Form
    {
        #region Constructor
        public FormEditLayer(Layer2DBrickExp layer, Packable content)
        {
            InitializeComponent();

            Layer = layer;
            Content = content as PackableBrick;
            uCtrlLayerEditor.Content = Content;
            uCtrlLayerEditor.Layer = Layer;
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            uCtrlLayerEditor.Content = Content;
            uCtrlLayerEditor.Layer = Layer;
            uCtrlLayerEditor.Invalidate();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            uCtrlLayerEditor.Invalidate();
        }
        #endregion
        #region Public accessors
        public Layer2DBrickExp Layer { get; set; }
        public PackableBrick Content { get; set; }
        #endregion
        #region Event handlers
        private void OnSaveEnabled(bool enable) => bnOK.Enabled = enable;
        #endregion
    }
}
