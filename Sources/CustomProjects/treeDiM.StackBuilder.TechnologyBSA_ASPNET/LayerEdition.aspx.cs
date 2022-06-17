#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
#endregion

namespace treeDiM.StackBuilder.TechnologyBSA_ASPNET
{
    public partial class LayerEdition : Page
    {
        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SelectedIndex = -1;
            }
            UpdateImage();
        }
        #endregion
        #region Update image
        protected void UpdateImage()
        {
            ImageWidth = Convert.ToInt32(IBLayer.Width.Value);
            ImageHeight = Convert.ToInt32(IBLayer.Width.Value);
            IBLayer.ImageUrl = "~/HandlerLayerEditor.ashx?param=" + DateTime.Now.Ticks.ToString();
            layerImage.Update();
        }
        #endregion
        #region Event handler
        protected void OnIBLayerClicked(object sender, ImageClickEventArgs e)
        {
            var layerEditorHelper = new LayerEditorHelpers(ImageSize, DimCase, DimContainer)
            {
                Positions = BoxPositions1
            };
            SelectedIndex = layerEditorHelper.GetPickedIndex(new Point(e.X, e.Y));
            UpdateImage();
        }
        protected void OnArrowClicked(object sender, ImageClickEventArgs e)
        {
            HalfAxis.HAxis axis = HalfAxis.HAxis.AXIS_X_N;
            if (sender == ButtonUp) axis = HalfAxis.HAxis.AXIS_Y_P;
            else if (sender == ButtonDown) axis = HalfAxis.HAxis.AXIS_Y_N;
            else if (sender == ButtonLeft) axis = HalfAxis.HAxis.AXIS_X_N;
            else if (sender == ButtonRight) axis = HalfAxis.HAxis.AXIS_X_P;

            var layerEditHelper = new LayerEditorHelpers(ImageSize, DimCase, DimContainer)
            {
                Positions = BoxPositions1,
                SelectedIndex = SelectedIndex
            };
            layerEditHelper.Move(axis, 10.0);
            BoxPositions1 = layerEditHelper.Positions;


            UpdateImage();
        }
        protected void OnArrowMaxClicked(object sender, ImageClickEventArgs e)
        {
            HalfAxis.HAxis axis = HalfAxis.HAxis.AXIS_X_N;
            if (sender == ButtonUpMost) axis = HalfAxis.HAxis.AXIS_Y_P;
            else if (sender == ButtonDownMost) axis = HalfAxis.HAxis.AXIS_Y_N;
            else if (sender == ButtonLeftMost) axis = HalfAxis.HAxis.AXIS_X_N;
            else if (sender == ButtonRightMost) axis = HalfAxis.HAxis.AXIS_X_P;

            var layerEditHelper = new LayerEditorHelpers(ImageSize, DimCase, DimContainer)
            {
                Positions = BoxPositions1,
                SelectedIndex = SelectedIndex
            };
            layerEditHelper.MoveMax(axis);
            BoxPositions1 = layerEditHelper.Positions;

            UpdateImage();
        }        
        protected void OnRotateClicked(object sender, ImageClickEventArgs e)
        {
            var layerEditHelper = new LayerEditorHelpers(ImageSize, DimCase, DimContainer)
            {
                Positions = BoxPositions1,
                SelectedIndex = SelectedIndex
            };
            layerEditHelper.Rotate();
            BoxPositions1 = layerEditHelper.Positions;
            UpdateImage();  
        }
        protected void OnButtonInsert(object sender, ImageClickEventArgs e)
        {
            var layerEditHelper = new LayerEditorHelpers(ImageSize, DimCase, DimContainer)
            {
                Positions = BoxPositions1,
                SelectedIndex = SelectedIndex
            };
            layerEditHelper.Insert();
            BoxPositions1 = layerEditHelper.Positions;
            SelectedIndex = -1;
            UpdateImage();
        }
        protected void OnButtonRemove(object sender, ImageClickEventArgs e)
        {
            var layerEditHelper = new LayerEditorHelpers(ImageSize, DimCase, DimContainer)
            {
                Positions = BoxPositions1,
                SelectedIndex = SelectedIndex
            };
            layerEditHelper.Remove();
            SelectedIndex = -1;
            UpdateImage();
        }
        protected void OnPrevious(object sender, EventArgs e)
        {
            if (ConfigSettings.WebGLMode)
                Response.Redirect("LayerSelectionWebGL.aspx");
            else
                Response.Redirect("LayerSelection.aspx");
        }
        protected void OnNext(object sender, EventArgs e)
        {
            var layerEditHelper = new LayerEditorHelpers(ImageSize, DimCase, DimContainer)
            {
                Positions = BoxPositions1,
                SelectedIndex = SelectedIndex
            };
            if (layerEditHelper.IsValidLayer)
            {
                if (ConfigSettings.WebGLMode)
                    Response.Redirect("ValidationWebGL.aspx");
                else
                    Response.Redirect("Validation.aspx");
            }
        }
        #endregion
        #region Private properties
        private Vector2D DimContainer
        {
            get
            {
                Vector3D vDimContainer = PalletStacking.PalletIndexToDim3D(PalletIndex);
                return new Vector2D(vDimContainer.X, vDimContainer.Y);
            }
        }
        private Size ImageSize => new Size(Convert.ToInt32(IBLayer.Width.Value), Convert.ToInt32(IBLayer.Height.Value));
        private int ImageWidth
        {
            get => (int)Session[SessionVariables.ImageWidth];
            set => Session[SessionVariables.ImageWidth] = value;
        }
        private int ImageHeight
        {
            get => (int)Session[SessionVariables.ImageHeight];
            set => Session[SessionVariables.ImageHeight] = value;
        }

        private Vector3D DimCase => Vector3D.Parse((string)Session[SessionVariables.DimCase]);
        private int PalletIndex => (int)Session[SessionVariables.PalletIndex];
        private List<BoxPosition> BoxPositions1
        {
            get => BoxPositionIndexed.ToListBoxPosition((List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions1]);
            set => Session[SessionVariables.BoxPositions1] = BoxPositionIndexed.FromListBoxPosition(value);
        }
        private List<BoxPosition> BoxPositions2
        {
            get => BoxPositionIndexed.ToListBoxPosition((List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions2]);
            set => Session[SessionVariables.BoxPositions2] = BoxPositionIndexed.FromListBoxPosition(value);
        }
        private List<BoxPosition> BoxPositions3
        {
            get => BoxPositionIndexed.ToListBoxPosition((List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions3]);
            set => Session[SessionVariables.BoxPositions3] = BoxPositionIndexed.FromListBoxPosition(value);
        }
        private List<BoxPosition> BoxPositions4
        {
            get => BoxPositionIndexed.ToListBoxPosition((List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions4]);
            set => Session[SessionVariables.BoxPositions4] = BoxPositionIndexed.FromListBoxPosition(value);
        }
        private int SelectedIndex
        {
            get => (int)Session[SessionVariables.SelectedIndex];
            set => Session[SessionVariables.SelectedIndex] = value;
        }
        #endregion
    }
}