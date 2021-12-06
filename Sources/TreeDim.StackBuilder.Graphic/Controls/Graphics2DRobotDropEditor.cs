#region Using directives
using System;
using System.Windows.Forms;
using System.Drawing;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
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
            RobotLayer.RefPointNumbering = (RobotLayer.enuCornerPoint)Properties.Settings.Default.AutomaticNumberingCornerIndex;
            toolStripComboCorner.SelectedIndex = Properties.Settings.Default.AutomaticNumberingCornerIndex;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try
            {
                double marginY = UnitsManager.ConvertLengthFrom(100.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                double frameRefLength = UnitsManager.ConvertLengthFrom(400.0, UnitsManager.UnitSystem.UNIT_METRIC1);

                Graphics = new Graphics2DForm(this, e.Graphics) { };
                if (null == Layer) return;
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
                Graphics.DrawText($"{drop.ID}", FontSizeID, new Vector2D(drop.Center3D.X, drop.Center3D.Y), Color.Red);
        }
        #endregion
        #region Mouse event handlers
        private void OnMouseMove(object sender, MouseEventArgs e) => SetMessage();
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
        #region Toolbar event handlers
        private void OnBuildCaseDropOf2(object sender, EventArgs e) => SetState(new StateBuildBlock(this, 2));
        private void OnBuildCaseDropOf3(object sender, EventArgs e) => SetState(new StateBuildBlock(this, 3));
        private void OnSplitDrop(object sender, EventArgs e) => SetState(new StateSplitDrop(this));
        private void OnReorder(object sender, EventArgs e) => SetState(new StateReoder(this));
        private void UpdateToolBar()
        {
            toolStripBnCaseDrop2.Checked = (CurrentState is StateBuildBlock stateBlock2) && stateBlock2.Number == 2;
            toolStripBnCaseDrop3.Checked = (CurrentState is StateBuildBlock stateBlock3) && stateBlock3.Number == 3;
            toolStripBnSplitDrop.Checked = CurrentState is StateSplitDrop;
            toolStripBnReorder.Checked = CurrentState is StateReoder;
        }
        private void OnNumberingCornerChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutomaticNumberingCornerIndex = toolStripComboCorner.SelectedIndex;
            Properties.Settings.Default.Save();

            RobotLayer.RefPointNumbering = (RobotLayer.enuCornerPoint)toolStripComboCorner.SelectedIndex;
            Layer?.AutomaticRenumber();
            Invalidate();            
        }
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
        public bool StateLoaded => !(CurrentState is StateDefault);

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
        #endregion
        #region Data members
        public RobotLayer Layer { get; set; }
        private Graphics2D Graphics { get; set; }
        private int FontSizeID => 16;
        protected ILog _log = LogManager.GetLogger(typeof(Graphics2DRobotDropEditor));
        private State _currentState;
        #endregion
        #region Delegate and event
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
    }
    public abstract class State
    {
        public virtual void OnMouseDown(int id) {}
        public virtual void OnMouseUp(int id) {}
        public virtual void OnKey(char c) {}
        protected void ExitState() { Host.SetDefaultState(); }
        public virtual bool ShowIDs { get; } = true;
        public virtual bool ShowSelected(RobotDrop drop) => false;
        public virtual bool ShowAllowed(RobotDrop drop) => false;
        public IStateHost Host { get; set; }
        public virtual string Message { get; }
    }
    internal class StateDefault : State
    {
        public StateDefault(IStateHost host) { Host = host; }
        public override bool ShowIDs => true;
        public override string Message => "Ready";
    }
    internal class StateBuildBlock : State
    {
        public StateBuildBlock(IStateHost host, int number)
        {
            Host = host;
            Number = number;
            IndexArray = new int[number];            
        }
        public int Number { get; set; }
        private int ClickCount { get; set; }
        public override void OnMouseUp(int index)
        {
            if (-1 == index) return;
            IndexArray[ClickCount] = index;
            ClickCount += 1;
            if (ClickCount == Number)
            {
                Merge();
                Reset();
            }
        }
        private void Merge()
        {
            Host.Layer.Merge(Number, IndexArray); 
        }
        private void Reset()
        {
            ClickCount = 0;
            for (int i = 0; i < Number; ++i) IndexArray[i] = -1;
        }
        private int[] IndexArray;
        public override string Message
        {
            get
            {
                if (0 == ClickCount) return "Click first case...";
                else if (1 == ClickCount) return "Click second case...";
                else if (2 == ClickCount) return "Click third case...";
                else if (3 == ClickCount) return "Click fourth case...";
                else if (4 == ClickCount) return "Click fifth case...";
                else return "";
            }
        }
    }

    internal class StateSplitDrop : State
    {
        public StateSplitDrop(IStateHost host)
        {
            Host = host; 
        }
        public override void OnMouseUp(int index)
        {
            // not a valid click!
            if (index == -1)
                return;
            // split 
            Host.Layer.Split(index);
        }
    }

    internal class StateReoder : State
    {
        public StateReoder(IStateHost host)
        {
            Host = host;
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
