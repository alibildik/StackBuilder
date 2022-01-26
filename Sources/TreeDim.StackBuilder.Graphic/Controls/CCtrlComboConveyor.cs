#region Using directives
using System.Windows.Forms;
using System.Drawing;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics.Controls
{
    public partial class CCtrlComboConveyor : ComboBox
    {
        #region Constructor
        public CCtrlComboConveyor()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }
        #endregion
        #region Public properties
        public PackableBrick Packable { get; set; }
        #endregion
        #region Override ComboBox
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            if (!DesignMode && Items.Count > 0 && e.Index != -1)
            {
                if (Items[e.Index] is ConveyorSetting item)
                {
                    var imageGenerator = new ImageGenConveyorSetting()
                    {
                        
                        Size = new Size(e.Bounds.Width, e.Bounds.Height),
                        ShowBelt = false,
                        ShowFrameRef = false
                    };
                    e.Graphics.DrawImage(
                        imageGenerator.GenerateImage(Packable, item.Number, item.Angle),
                        e.Bounds.Left,
                        e.Bounds.Top
                        );
                }
            }
            base.OnDrawItem(e);
        }
        #endregion
    }
}
