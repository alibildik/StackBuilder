#region Using directive
using System.Windows.Forms;
using System.Drawing;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics.Controls
{
    public partial class CCtrlComboLayer : ComboBox
    {
        #region Data members
        private Packable _packable;
        #endregion

        #region Constructor
        public CCtrlComboLayer()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }
        #endregion

        #region Public properties
        public Packable Packable
        {
            set { _packable = value; }
        }
        #endregion

        #region Override ComboBox
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            if (!DesignMode && Items.Count > 0 && e.Index != -1)
            {
                LayerDropDownItem item = new LayerDropDownItem(
                    Items[e.Index] as ILayer2D
                    , _packable
                    , ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    , new Size(ItemHeight, ItemHeight)
                    , Properties.Settings.Default.LayerView3D
                    );
                e.Graphics.DrawImage(item.Image, e.Bounds.Left, e.Bounds.Top);
            }
            base.OnDrawItem(e);
        }
        #endregion
    }
    public class LayerDropDownItem
    {
        #region Constructor
        public LayerDropDownItem(ILayer2D layer, Packable packable, bool selected, Size imgSize, bool show3D)
        {
            // save layer
            Layer = layer;
            // build image
            if (show3D)
            {
                Graphics3DImage graphics = new Graphics3DImage(imgSize);
                using (ViewerILayer2D solViewer = new ViewerILayer2D(Layer))
                {
                    solViewer.Draw(graphics, packable, 0.0, selected, true);
                    Image = graphics.Bitmap;
                }
            }
            else
            {
                // build image
                Graphics2DImage graphics = new Graphics2DImage(imgSize);
                using (ViewerILayer2D solViewer = new ViewerILayer2D(Layer))
                {
                    solViewer.Draw(graphics, packable, 0.0, selected, true);
                    Image = graphics.Bitmap;
                }
            }
        }
        #endregion
        #region Public properties
        public Image Image { get; set; }
        #endregion
        #region Override object
        public override string ToString() => Layer.ToString();
        #endregion
        #region Data members
        private ILayer2D Layer { get; set; }
        #endregion
    }
}
