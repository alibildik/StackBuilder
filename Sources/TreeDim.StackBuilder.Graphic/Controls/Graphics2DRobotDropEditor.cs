#region Using directives
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics.Properties;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public partial class Graphics2DRobotDropEditor : UserControl, IStateHost
    {
        #region Constructor
        public Graphics2DRobotDropEditor()
        {
            InitializeComponent();

            // double buffering
            SetDoubleBuffered();
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;
        }
        #endregion
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
        #region UserControl overrides (Drawing)
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // set default state
            SetDefaultState();
            // initialize automatic numbering corner combo box
            RobotLayer.RefPointNumbering = (RobotLayer.enuCornerPoint)Settings.Default.AutomaticNumberingCornerIndex;
            cbCorner.SelectedIndex = Settings.Default.AutomaticNumberingCornerIndex;

            // set event
            cbCorner.SelectedValueChanged += new EventHandler(OnNumberingCornerChanged);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try
            {
                if (null == Layer)
                {
                    e.Graphics.DrawString(
                        Resources.ID_NOTSUPPORTED,
                        new Font("Arial", 12),
                        new SolidBrush(Color.Red),
                        new PointF(Size.Width/2, Size.Height/2),
                        new StringFormat()
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        }
                        );
                    return;
                }

                double marginY = UnitsManager.ConvertLengthFrom(100.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                double frameRefLength = UnitsManager.ConvertLengthFrom(400.0, UnitsManager.UnitSystem.UNIT_METRIC1);

                Graphics = new Graphics2DForm(this, e.Graphics) { };

                Vector2D margin = new Vector2D(0.0, marginY);
                Graphics.SetViewport(Layer.MinPoint - margin, Layer.MaxPoint + margin);
                // draw layer boundary rectangle
                Graphics.DrawRectangle(Layer.MinPoint, Layer.MaxPoint, Color.OrangeRed);
                // draw all drops
                foreach (var drop in Layer.Drops)
                    DrawDrop(drop, CurrentState.ShowIDs, CurrentState.ShowSelected(drop), CurrentState.ShowAllowed(drop));
                // draw frameref
                FrameRef frameRef = new FrameRef(0)
                {
                    Position = new Vector3D(-0.5 * marginY, -0.5 * marginY, 0.0),
                    Length = frameRefLength,
                    LengthAxis = HalfAxis.HAxis.AXIS_X_P,
                    WidthAxis = HalfAxis.HAxis.AXIS_Y_P
                };
                frameRef.Draw(Graphics);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void DrawDrop(RobotDrop drop, bool showID, bool selected, bool showAllowed)
        {
            // draw case(s)
            for (int index = 0; index < drop.Number; ++index)
            {
                Box b;
                if (drop.Content is PackProperties pack)
                    b = new Pack(0, pack, drop.InnerBoxPosition(index));
                else
                    b = new Box(0, drop.Content, drop.InnerBoxPosition(index)) { Facing = drop.Content.Facing };
                b.Draw(Graphics);
            }
            // draw drop boundary
            Graphics.DrawContour(drop.Contour, Color.Black, 4.0f);
            // draw 
            if (showID && drop.ID >= 0)
            {
                Graphics.DrawText($"{drop.ID}", FontSizeID, new Vector2D(drop.Center3D.X, drop.Center3D.Y), Color.Blue);
                Graphics.DrawText($"({drop.ConveyorSetting.Number},{drop.ConveyorSetting.Angle},{drop.ConveyorSetting.GripperAngle})", FontSizeID / 2, drop.BottomRightCorner, Color.Black, Graphics2D.TexpPos.TEXT_BOTTOMRIGHT);
            }
        }
        #endregion
        #region Mouse event handlers
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            OnStateMouseMove(ClickToIndex(e.Location));
            SetMessage();
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            OnStateMouseDown(ClickToIndex(e.Location));
            Invalidate();
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            OnStateMouseUp(ClickToIndex(e.Location));
            Invalidate();
        }
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                SetDefaultState();
            else
                OnStateKeyPress(e.KeyChar);
            Invalidate();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
        #endregion
        #region Event handlers
        private void OnBuildCaseDrop(object sender, EventArgs e) => SetState(new StateBuildBlock(this, SelectedConveyorSetting));
        private void OnSplitDrop(object sender, EventArgs e) => SetState(new StateSplitDrop(this));
        private void OnReorder(object sender, EventArgs e) => SetState(new StateReoder(this));
        private void UpdateToolBar()
        {
            chkbMerge.Checked = CurrentState is StateBuildBlock;
            chkbSplit.Checked = CurrentState is StateSplitDrop;
            chkbReorder.Checked = CurrentState is StateReoder;
        }
        private void OnNumberingCornerChanged(object sender, EventArgs e)
        {
            Settings.Default.AutomaticNumberingCornerIndex = cbCorner.SelectedIndex;
            Settings.Default.Save();

            RobotLayer.RefPointNumbering = (RobotLayer.enuCornerPoint)cbCorner.SelectedIndex;
            Layer?.AutomaticRenumber();
            Invalidate();            
        }

        private void OnDropModeChanged(object sender, EventArgs e) {}
        private void OnConveyorSettingChanged(object sender, EventArgs e) => SetState(new StateBuildBlock(this, SelectedConveyorSetting));
        #endregion
        #region IStateHost implementation
        public void SetState(State state)
        {
            CurrentState = state;
            state.Host = this;
            UpdateToolBar();
        }
        public void ExitState() { CurrentState = null; UpdateToolBar(); }
        private State CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                _currentState.Host = this;
                Invalidate();
                UpdateToolBar();
            } 
        }
        public void SetCursor(Cursor cursor) { if (Cursor != cursor) Cursor = cursor; }
        public bool StateLoaded => !(CurrentState is StateDefault);
        public void OnStateMouseMove(int id) => CurrentState?.OnMouseMove(id);
        public void OnStateMouseUp(int id) => CurrentState?.OnMouseUp(id);
        public void OnStateMouseDown(int id) => CurrentState?.OnMouseDown(id);
        public void OnStateKeyPress(char c) => CurrentState?.OnKey(c);
        public void SetDefaultState() { CurrentState = new StateDefault(this); }
        public void SetMessage(string message)
        {
            statusLabel.Text = message;        
        }
        #endregion
        #region Helpers
        private void SetMessage()
        {
            if (null != CurrentState)
                statusLabel.Text = CurrentState.Message;
        }
        public Rectangle DropToRectangle(RobotDrop rd)
        {
            Vector3D[] rectPoints = rd.CornerPoints;
            Vector3D[] cornerPoints = new Vector3D[8];
            for (int i = 0; i < 8; ++i)
                cornerPoints[i] = rectPoints[i];
            Point[] pts = Graphics.TransformPoint(cornerPoints);
            int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
            foreach (var pt in pts)
            {
                minX = Math.Min(minX, pt.X);
                maxX = Math.Max(maxX, pt.X);
                minY = Math.Min(minY, pt.Y);
                maxY = Math.Max(maxY, pt.Y);                
            }
            return new Rectangle(minX, maxY, maxX - minX, maxY - minY);
        }
        public int ClickToIndex(Point pt)
        {
            if (null == Graphics) return -1;
            // get world coordinate
            Vector2D ptWorld = Graphics.ReverseTransform(pt);
            // test each box positions
            int index = 0;
            foreach (var rd in Layer.Drops)
            {
                if (PointIsInside(rd.BoxPositionMain, rd.Dimensions, ptWorld))
                    return index; // <- found!
                ++index;
            }
            // failed to find
            return -1;
        }
        public static bool PointIsInside(BoxPosition bPos, Vector3D dim, Vector2D pt)
        {
            var bbox = bPos.BBox(dim);
            return pt.X >= bbox.PtMin.X && pt.X <= bbox.PtMax.X
                && pt.Y >= bbox.PtMin.Y && pt.Y <= bbox.PtMax.Y;
        }
        public void SetConveyorSettings(PackableBrick packable, List<ConveyorSetting> listConveyorSettings)
        {
            cbConveyorSetting.Items.Clear();
            cbConveyorSetting.Packable = packable;
            cbConveyorSetting.Items.AddRange(listConveyorSettings.ToArray());
            cbConveyorSetting.SelectedIndex = 0;
        }
        protected ConveyorSetting SelectedConveyorSetting => cbConveyorSetting.SelectedItem as ConveyorSetting;
        #endregion
        #region Data members
        public RobotLayer Layer { get; set; }
        private Graphics2D Graphics { get; set; }
        private int FontSizeID => 16;
        private int FontSizeConv => 7;
        protected ILog _log = LogManager.GetLogger(typeof(Graphics2DRobotDropEditor));
        private State _currentState;
        #endregion
    }
    #region State
    public interface IStateHost
    {
        void SetState(State state);
        void ExitState();
        void SetDefaultState();
        void Invalidate();
        RobotLayer Layer { get; }
        void SetCursor(Cursor cursor);
    }
    public abstract class State
    {
        public State(IStateHost host) { Host = host; }
        public virtual void OnMouseMove(int id) {}
        public virtual void OnMouseDown(int id) {}
        public virtual void OnMouseUp(int id) {}
        public virtual void OnKey(char c) {}
        protected void ExitState() { Host.SetDefaultState(); }
        public virtual bool ShowIDs { get; } = true;
        public virtual bool ShowSelected(RobotDrop drop) => false;
        public virtual bool ShowAllowed(RobotDrop drop) => false;
        public IStateHost Host { get; set; }
        public virtual string Message { get; }
        private void SetCursor(Cursor cursor) => Host.SetCursor(cursor);
    }
    internal class StateDefault : State
    {
        public StateDefault(IStateHost host):base(host) { Host.SetCursor(Cursors.Arrow); }
        public override bool ShowIDs => true;
        public override string Message => "Ready";
    }
    internal class StateBuildBlock : State
    { 
        public StateBuildBlock(IStateHost host, ConveyorSetting setting) : base(host) 
        {
            Setting = setting;
            IndexArray = new int[setting.Number];
        }
        public override void OnMouseMove(int index) => Host.SetCursor(index == -1 || Host.Layer.CanBeMerged(index) ? Cursors.Arrow : Cursors.No);
        public override void OnMouseUp(int index)
        {
            if (-1 == index) return;
            if (Host.Layer.CanBeMerged(index))
            {
                IndexArray[ClickCount] = index;
                ClickCount++;
            }
            if (ClickCount == Setting.Number)
            {
                Merge();
                Reset();
            }
        }
        private void Merge() => Host.Layer.Merge(Setting, IndexArray);
        private void Reset()
        {
            ClickCount = 0;
            for (int i = 0; i < Setting.Number; ++i) IndexArray[i] = -1;
        }
        public override string Message => string.Format(Resources.ID_CLICKCASEFORGROUP, ClickCount, Setting.Number);
        #region Data members
        public ConveyorSetting Setting { get; set; }
        private int ClickCount { get; set; }
        private int[] IndexArray;
        #endregion
    }
    internal class StateSplitDrop : State
    {
        public StateSplitDrop(IStateHost host) : base(host)
        {
            Host.SetCursor(Cursors.Arrow);
        }
        public override void OnMouseUp(int index)
        {
            // not a valid click!
            if (index == -1) return;
            // split 
            Host.Layer.Split(index);
        }
        public override void OnMouseMove(int index) => Host.SetCursor(index == -1 || Host.Layer.CanBeSplit(index) ? Cursors.Arrow : Cursors.No);
    }
    internal class StateReoder : State
    {
        public StateReoder(IStateHost host) : base(host)
        {
            Host.SetCursor(Cursors.Arrow);
            Host.Layer.ResetNumbering();
            Host.Invalidate();
        }
        public override void OnMouseUp(int index)
        {
            // not a valid click!
            if (index == -1)
                return;
            // check if ID already set
            if (-1 == Host.Layer.Drops[index].ID)
                Host.Layer.Drops[index].ID = ID++;
            // check if layer is completely numbered
            if (Host.Layer.IsFullyNumbered)
            {
                Host.Layer.CompleteNumbering(Vector3D.Zero);
                Host.Layer.Parent.Update();
                ExitState();
            }
            Host.Invalidate();
        }
        public override bool ShowIDs => true;
        private int ID { get; set; } = 0;
    }
    #endregion
}
