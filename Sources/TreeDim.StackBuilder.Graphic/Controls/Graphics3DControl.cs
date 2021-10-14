#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using Sharp3D.Math.Core;

using log4net;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    #region IDrawingContainer
    public interface IDrawingContainer
    {
        void Draw(Graphics3DControl ctrl, Graphics3D graphics);
    }
    #endregion

    #region Graphics3DControl
    public partial class Graphics3DControl : UserControl, ISupportInitialize
    {
        #region Delegates
        public delegate void ButtonPressedHandler(int iIndex);
        public delegate void VolumeSelectedHandler(int id);
        #endregion

        #region Events
        public event ButtonPressedHandler ButtonPressed;
        public event VolumeSelectedHandler VolumeSelected;
        #endregion

        #region Constructor
        public Graphics3DControl()
        {
            InitializeComponent();
            DrawingContainer = null;

            // double buffering
            SetDoubleBuffered();
            // toolbar event
            ButtonPressed += OnButtonPressed;
            // facing
            ShowFacing = Properties.Settings.Default.ShowFacing;
        }
        #endregion

        #region Cursor / double buffering
        internal static Cursor CursorRotate
        {
            get
            {
                if (null == _cursorRotate)
                {
                    try
                    {
                        var buffer = Properties.Resources.ResourceManager.GetObject("rotate") as byte[];
                        using (var m = new MemoryStream(buffer))
                        { _cursorRotate = new Cursor(m); }
                    }
                    catch (Exception)
                    {
                        _cursorRotate = Cursors.Default;
                    }
                }
                return _cursorRotate;
            }
        } 

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

        #region Accessors
        public IDrawingContainer DrawingContainer { set; private get; }
        public Viewer Viewer { get; set; }
        protected bool ShowDimensions { get; set; } = true;
        protected bool ShowFacing { get; set; } = false;
        private bool Dragging { get; set; } = false;
        public bool ShowToolBar { get; private set; } = false;
        public double AngleHoriz { get; set; } = 45.0 + 180.0;
        private double AngleVert { get; set; } = 45.0;
        #endregion

        #region Overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try
            {
                Graphics3DForm graphics = new Graphics3DForm(this, e.Graphics);
                double angleHorizRad = AngleHoriz * Math.PI / 180.0;
                double angleVertRad = AngleVert * Math.PI / 180.0;
                double cameraDistance = 100000.0;
                graphics.CameraPosition = new Vector3D(
                    cameraDistance * Math.Cos(angleHorizRad) * Math.Cos(angleVertRad)
                    , cameraDistance * Math.Sin(angleHorizRad) * Math.Cos(angleVertRad)
                    , cameraDistance * Math.Sin(angleVertRad));
                // set camera target
                graphics.Target = Vector3D.Zero;
                // set viewport (not actually needed)
                graphics.SetViewport(-500.0f, -500.0f, 500.0f, 500.0f);
                // show images
                graphics.ShowTextures = true;
                graphics.ShowDimensions = ShowDimensions;
                graphics.ShowFacing = ShowFacing;
                graphics.FontSizeRatio = 10.0f / (float)Size.Height;

                if (null != DrawingContainer)
                {
                    try
                    {
                        DrawingContainer.Draw(this, graphics);
                    }
                    catch (Exception ex)
                    {
                        e.Graphics.DrawString(ex.ToString()
                            , new Font("Arial", 12)
                            , new SolidBrush(Color.Red)
                            , new Point(0, 0)
                            , StringFormat.GenericDefault);
                        _log.Error(ex.ToString());
                    }
                }
                else if (null != Viewer)
                {
                    try
                    {
                        Viewer.Draw(graphics, Transform3D.Identity);
                    }
                    catch (Exception ex)
                    {
                        e.Graphics.DrawString(ex.Message
                            , new Font("Arial", 12)
                            , new SolidBrush(Color.Red)
                            , new Point(0, 0)
                            , StringFormat.GenericDefault);
                        _log.Error(ex.Message);
                    }
                }

                graphics.Flush();

                if (null != Viewer)
                {
                    Viewer.CurrentTransformation = graphics.GetCurrentTransformation();
                    Viewer.ViewDir = graphics.ViewDirection;
                }

                // draw toolbar
                if (ShowToolBar)
                    DrawToolBar(e.Graphics);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region ISupportInitialize
        public void BeginInit()
        { 
        }
        public void EndInit()
        { 
        }
        #endregion

        #region Helpers
        public void ScreenShotToClipboard()
        {
            var graphics = new Graphics3DImage(this.Size);
            try
            {
                double angleHorizRad = AngleHoriz * Math.PI / 180.0;
                double angleVertRad = AngleVert * Math.PI / 180.0;
                double cameraDistance = 100000.0;
                graphics.CameraPosition = new Vector3D(
                    cameraDistance * Math.Cos(angleHorizRad) * Math.Cos(angleVertRad)
                    , cameraDistance * Math.Sin(angleHorizRad) * Math.Cos(angleVertRad)
                    , cameraDistance * Math.Sin(angleVertRad));
                // set camera target
                graphics.Target = Vector3D.Zero;
                // set viewport (not actually needed)
                graphics.SetViewport(-500.0f, -500.0f, 500.0f, 500.0f);
                // show images
                graphics.ShowTextures = true;
                graphics.ShowDimensions = ShowDimensions;
                graphics.FontSizeRatio = 10.0f / (float)Size.Height;

                if (null != DrawingContainer)
                {
                    try
                    {
                        DrawingContainer.Draw(this, graphics);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.ToString());
                    }
                }
                else if (null != Viewer)
                {
                    try
                    {
                        Viewer.Draw(graphics, Transform3D.Identity);
                    }
                    catch (Exception ex)
                    {
                        graphics.Graphics.DrawString(ex.Message
                            , new Font("Arial", 12)
                            , new SolidBrush(Color.Red)
                            , new Point(0, 0)
                            , StringFormat.GenericDefault);
                        _log.Error(ex.Message);
                    }
                }

                graphics.Flush();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            Clipboard.SetImage(BitmapHelpers.Crop(graphics.Bitmap));
        }
        #endregion

        #region Toolbar
        public bool ShowHideToolBar(Point pt)
        {
            return -1 != ToolbarButtonIndex(pt);
        }
        public static int ToolbarButtonIndex(Point pt)
        {
            for (int i = 0; i < 11; ++i)
            {
                Rectangle rect = new Rectangle(i * _toolbarButtonOffset, 0, _toolbarButtonOffset, 16);
                if (rect.Contains(pt))
                    return i;
            }
            return -1;
        }
        void DrawToolBar(System.Drawing.Graphics g)
        { 
            int offsetIcon = 0;
            g.DrawImage(Properties.Resources.View_1, new Point(offsetIcon, 1));
            g.DrawImage(Properties.Resources.View_2, new Point(offsetIcon += _toolbarButtonOffset, 1));
            g.DrawImage(Properties.Resources.View_3, new Point(offsetIcon += _toolbarButtonOffset, 1));
            g.DrawImage(Properties.Resources.View_4, new Point(offsetIcon += _toolbarButtonOffset, 1));
            g.DrawImage(Properties.Resources.View_Top, new Point(offsetIcon += _toolbarButtonOffset, 1));
            g.DrawImage(Properties.Resources.View0, new Point(offsetIcon += _toolbarButtonOffset, 1));
            g.DrawImage(Properties.Resources.View90, new Point(offsetIcon += _toolbarButtonOffset, 1));
            g.DrawImage(Properties.Resources.View180, new Point(offsetIcon += _toolbarButtonOffset, 1));
            g.DrawImage(Properties.Resources.View270, new Point(offsetIcon += _toolbarButtonOffset, 1));
            g.DrawImage(ShowFacing ? Properties.Resources.FacingHide : Properties.Resources.FacingShow, new Point(offsetIcon += _toolbarButtonOffset, 1));
            g.DrawImage(ShowDimensions ? Properties.Resources.CotationHide : Properties.Resources.CotationShow, new Point(offsetIcon += _toolbarButtonOffset, 1));
        }
        private void OnButtonPressed(int iIndex)
        {
            Graphics3D.Reset();
            switch (iIndex)
            {
                case 0: AngleHoriz = 0.0; AngleVert = 0.0; break;
                case 1: AngleHoriz = 90.0; AngleVert = 0.0; break;
                case 2: AngleHoriz = 180.0; AngleVert = 0.0; break;
                case 3: AngleHoriz = 270.0; AngleVert = 0.0; break;
                case 4: AngleHoriz = 0.0; AngleVert = 90.0; break;
                case 5: AngleHoriz = 45.0 + 0.0; AngleVert = 45.0; break;
                case 6: AngleHoriz = 45.0 + 90.0; AngleVert = 45.0; break;
                case 7: AngleHoriz = 45.0 + 180.0; AngleVert = 45.0; break;
                case 8: AngleHoriz = 45.0 + 270.0; AngleVert = 45.0; break;
                case 9: ShowFacing = !ShowFacing; break;
                case 10: ShowDimensions = !ShowDimensions; break;
                default: break;
            }
            Invalidate();
        }
        #endregion

        #region Mouse event handlers
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // not dragging ?
            if (!Dragging)
            {
                bool showToolBar = ShowHideToolBar(e.Location);
                if (ShowToolBar != showToolBar)
                {
                    ShowToolBar = showToolBar;
                    Invalidate();
                }
            }
            else
            {
                double angleXDiff = -(e.Location.X - _prevLocation.X) * 360.0 / Size.Width;
                double angleYDiff = (e.Location.Y - _prevLocation.Y) * 90.0 / Size.Height;
                _prevLocation = e.Location;

                if (AngleHoriz + angleXDiff < 0.0)
                    AngleHoriz += angleXDiff + 360.0;
                else if (AngleHoriz + angleXDiff > 360.0)
                    AngleHoriz += angleXDiff - 360.0;
                else
                    AngleHoriz += angleXDiff;

                if (AngleVert + angleYDiff < 0.0)
                    AngleVert = 0.0;
                else if (AngleVert + angleYDiff >= 90.0)
                    AngleVert = 90.0;
                else
                    AngleVert += angleYDiff;

                Invalidate();
            }
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            // clicking toolbar ?
            if (!Dragging)
            { 
                int tbBIndex = ToolbarButtonIndex(e.Location);
                if ((-1 != tbBIndex) && (null != ButtonPressed))
                {
                    ButtonPressed(tbBIndex);
                    return;
                }
            }
            // switch to drag mode
            if (MouseButtons.Left == e.Button)
            {
                Dragging = true;
                _prevLocation = e.Location;
            }
            // set rotate cursor
            Cursor.Current = Dragging ? CursorRotate : Cursors.Default;
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left == e.Button)
            {
                Dragging = false;
                _prevLocation = e.Location;
                // back to default cursor
                Cursor.Current = Cursors.Default;
            }
        }
        #endregion

        #region Event handlers
        private void OnResize(object sender, EventArgs e)
        {
            Invalidate();
        }
        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                // sanity checks
                if (null != Viewer && Viewer.TryPicking(e.Location.X, e.Location.Y, out uint index))
                    VolumeSelected?.Invoke((int)index);
                else
                    VolumeSelected?.Invoke(-1);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            Invalidate();
        }
        #endregion

        #region Data members
        private Point _prevLocation;
        private const int _toolbarButtonOffset = 17;
        internal static Cursor _cursorRotate;
        static readonly ILog _log = LogManager.GetLogger(typeof(Graphics3DControl));
        #endregion
    }
    #endregion
}
