#region Using directives
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using Sharp3D.Math.Core;
using log4net;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics.Properties;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public partial class Graphics2DLayerEditor : UserControl
    {
        #region Constructor
        public Graphics2DLayerEditor()
        {
            InitializeComponent();

            // double buffering
            SetDoubleBuffered();
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SelectionChanged += EnableDisableAddRemoveButtons;
        }
        protected override bool IsInputKey(Keys keyData)
        {
            return true;
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
        #region Public properties
        public Layer2DBrickExp Layer  { get; set; }
        public Vector2D PtMin => Vector2D.Zero - new Vector2D(MaxContentDim, MaxContentDim);
        public Vector2D PtMax => Layer.DimContainer + new Vector2D(MaxContentDim, MaxContentDim);
        public double MaxContentDim
        {
            get
            {
                if (null == Content) return 0.0;
                double max = double.MinValue;
                max = Math.Max(Content.OuterDimensions.X, max);
                max = Math.Max(Content.OuterDimensions.Y, max);
                max = Math.Max(Content.OuterDimensions.Z, max);
                return max;
            }
        }
        #endregion
        #region UserControl overrides (Drawing)
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try
            {
                Graphics = new Graphics2DForm(this, e.Graphics);
                if (null == Layer) return;
                Graphics.SetViewport(PtMin, PtMax);

                uint pickId = 0;
                foreach (var bp in Layer.Positions)
                {
                    Box b;
                    if (Content is PackProperties pack)
                        b = new Pack(pickId++, pack, bp);
                    else
                        b = new Box(pickId++, Content as PackableBrick, bp);
                    b.Draw(Graphics);
                }
                Graphics.DrawRectangle(Vector2D.Zero, Layer.DimContainer, Color.OrangeRed);

                // draw frameref
                double marginY = UnitsManager.ConvertLengthFrom(100.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                double frameRefLength = UnitsManager.ConvertLengthFrom(400.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                FrameRef frameRef = new FrameRef(0)
                {
                    Position = new Vector3D(-0.5 * marginY, -0.5 * marginY, 0.0),
                    Length = frameRefLength,
                    LengthAxis = HalfAxis.HAxis.AXIS_X_P,
                    WidthAxis = HalfAxis.HAxis.AXIS_Y_P
                };
                frameRef.Draw(Graphics);

                if (-1 != SelectedIndex)
                {
                    var bp = Layer.Positions[SelectedIndex];

                    Box boxSelected;
                    if (Content is PackProperties pack)
                        boxSelected = new Pack((uint)SelectedIndex, pack, bp);
                    else
                        boxSelected = new Box(((uint)SelectedIndex), Content as PackableBrick, bp);
                    Graphics.DrawBoxSelected(boxSelected);

                    ArrowButtons.Clear();
                    Vector2D ptCenter = new Vector2D(boxSelected.Center.X, boxSelected.Center.Y);



                    // draw translation arrows
                    for (int i = 0; i < 4; ++i)
                    {
                        if (Arrows[i])
                        {
                            Graphics.DrawArrow(ptCenter, i, 100, 5, 10, Color.Red, out Rectangle rect);
                            ArrowButtons.Add(i, rect);
                        }
                    }
                    // draw rotation arrows
                    if (ArrowRotate)
                        Graphics.DrawArcArrow(ptCenter, 75, 10, Color.Red, out _rotateRectangle);

                    // draw position
                    Graphics.DrawText($"({bp.Position.X:0.##}, {bp.Position.Y:0.##}, {bp.Position.Z:0.##}), {HalfAxis.ToString(bp.DirectionLength)}, {HalfAxis.ToString(bp.DirectionWidth)}", 16);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e) {}
        #endregion
        #region Event handlers
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((!Moving) && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                StartMove(dictKeyDirection[e.KeyCode]);
                return;
            }
            if (e.KeyCode == Keys.R)
                Rotate();
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            EndMove();
            Moving = false;
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            foreach (var item in ArrowButtons)
            {
                if (item.Value.Contains(e.Location))
                {
                    StartMove(item.Key);
                    return;
                }
            }
            if (null != _rotateRectangle && _rotateRectangle.Contains(e.Location))
                Rotate();
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (Moving)
            {
                EndMove();
                Moving = false;
            }
            else
            {
                Vector2D pt = Graphics.ReverseTransform(e.Location);
                // selected index
                int initialIndex = SelectedIndex;
                SelectedIndex = BoxInteraction.SelectedPosition(Layer.Positions, Dimensions, pt);
                if (initialIndex != SelectedIndex)
                    SelectionChanged();
            }
            UpdateArrows();
            Invalidate();
        }
        private void OnItemAdd(object sender, EventArgs e)
        {
            BoxPosition bPosNew = new BoxPosition(new Vector3D(PtMin.X, PtMin.Y, 0.0), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
            if (!BoxInteraction.HaveIntersection(Layer.Positions, Dimensions, SelectedIndex, bPosNew))
            {
                Layer.AddPosition(bPosNew);
                SelectedIndex = Layer.Positions.Count - 1;
                Invalidate();
            }
            else
                MessageBox.Show(Resources.ID_CANNOTADD);
        }
        private void OnItemRemove(object sender, EventArgs e)
        {
            if (-1 == SelectedIndex) return;
            Layer.RemovePosition(SelectedIndex);
            SelectedIndex = -1;
            Invalidate();
        }
        #endregion
        #region Moving items
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (-1 == SelectedIndex) return;
            BoxPosition bpos = Layer.Positions[SelectedIndex];
            BoxPosition bposNew = bpos.Translate(MoveDir, StepMove);
            if (!BoxInteraction.HaveIntersection(Layer.Positions, Dimensions, SelectedIndex, bposNew)
                && BoxInteraction.BoxCanMoveInside(Layer.Positions[SelectedIndex], Dimensions, PtMin, PtMax, MoveDir))
               Layer.Positions[SelectedIndex] = bposNew;

            else
            {
                double distance = 0;
                if (BoxInteraction.MinDistance(Layer.Positions, Dimensions, SelectedIndex, MoveDir, ref distance))
                {
                    bposNew = bpos.Translate(MoveDir, distance);
                    Layer.Positions[SelectedIndex] = bposNew;
                }
                EndMove();
            }
            CountMove++;

            UpdateArrows();
            Invalidate();
        }

        private void StartMove(int dir)
        {
            if (dir < 0 || dir > 3) return;
            StartMove( Directions[dir] );
        }
        private void StartMove(HalfAxis.HAxis moveDir)
        { 
            CountMove = 0;
            MoveDir = moveDir;
            timerMove.Start();
            Moving = true;
       
        }
        private void EndMove()
        {
            timerMove.Stop();
            EnableDisableAddRemoveButtons(); 
        }
        private void Rotate()
        {
            if (ArrowRotate)
            {
                BoxPosition pos = Layer.Positions[SelectedIndex].RotateZ90(Dimensions);
                Layer.Positions[SelectedIndex] = pos;
            }
            UpdateArrows();
            EnableDisableAddRemoveButtons();
            Invalidate();
        }
        private double StepMove
        {
            get
            {
                if (CountMove < 3) return 0.1;
                else if (CountMove < 10) return 1.0;
                else if (CountMove < 20) return 10.0;
                else return 30.0;
            }
        }
        private int CountMove { get; set; }
        #endregion
        #region Update toolbars
        private void EnableDisableAddRemoveButtons()
        {
            tsbAdd.Enabled = !BoxInteraction.HaveIntersection(
                Layer.Positions,
                Dimensions,
                -1,
                new BoxPosition(new Vector3D(PtMin.X, PtMin.Y, 0.0), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P)
                );
            tsbRemove.Enabled = (-1 != SelectedIndex);
        }
        #endregion
        #region Update arrows
        private void UpdateArrows()
        {
            Vector3D dim = Dimensions;
            if (-1 != SelectedIndex)
            {
                // allow translations
                Arrows = Enumerable.Repeat(false, 4).ToArray();
                for (int i = 0; i < 4; ++i)
                {
                    double distance = 0.0;
                    if ((!BoxInteraction.MinDistance(Layer.Positions, dim, SelectedIndex, Directions[i], ref distance) || distance > 0.1)
                        && BoxInteraction.BoxCanMoveInside(Layer.Positions[SelectedIndex], dim, PtMin, PtMax, Directions[i]))
                        Arrows[i] = true;
                }
                // allow rotations
                ArrowRotate = !BoxInteraction.HaveIntersection(Layer.Positions, dim, SelectedIndex, Layer.Positions[SelectedIndex].RotateZ90(dim));
            }
            bool allBoxesInside = BoxInteraction.BoxesAreInside(Layer.Positions, dim, Vector2D.Zero, Layer.DimContainer);
            if (AllBoxesInside != allBoxesInside)
            {
                AllBoxesInside = allBoxesInside;
                SaveEnabled?.Invoke(AllBoxesInside);
            }
        }
        #endregion
        #region Helpers
        private Vector3D Dimensions => Content.OuterDimensions;
        #endregion
        #region Delegate / Event
        public delegate void EnableSave(bool enable);
        public event EnableSave SaveEnabled;

        public delegate void SelectionChange();
        public event SelectionChange SelectionChanged;
        #endregion
        #region Data members
        [Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Packable Content { get; set; }
        [Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private Graphics2D Graphics { get; set; } 

        private int SelectedIndex { get; set; } = -1;
        private HalfAxis.HAxis MoveDir { get; set; }
        private static readonly HalfAxis.HAxis[] Directions = { HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_N, HalfAxis.HAxis.AXIS_Y_N };
        private static readonly Dictionary<Keys, HalfAxis.HAxis> dictKeyDirection = new Dictionary<Keys, HalfAxis.HAxis>
        {
            { Keys.Left, HalfAxis.HAxis.AXIS_X_N },
            { Keys.Right, HalfAxis.HAxis.AXIS_X_P },
            { Keys.Up, HalfAxis.HAxis.AXIS_Y_P },
            { Keys.Down, HalfAxis.HAxis.AXIS_Y_N}
        };
        private bool[] Arrows = { false, false, false, false};
        private bool ArrowRotate = false;
        private bool Moving { get; set; } = false;
        private Dictionary<int, Rectangle> ArrowButtons { get; set; } = new Dictionary<int, Rectangle>();
        public bool AllBoxesInside { get; private set; } = true;
        private Rectangle _rotateRectangle;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Graphics2DLayerEditor));
        #endregion
    }
}
