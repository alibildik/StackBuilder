#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    #region RobotDrop
    public class RobotDrop
    {
        #region Enums
        public enum PackDir { LENGTH, WIDTH };
        public enum NeighbourDir { RIGHT, TOP, LEFT, BOTTOM}
        #endregion
        #region Constructor
        public RobotDrop(RobotLayer parent) { Parent = parent; }
        #endregion
        #region Public properties
        public PackableBrick Content => Parent.Parent.Content as PackableBrick;
        public Vector3D Dimensions => new Vector3D(
                    (PackDirection == PackDir.LENGTH ? 1 : Number) * SingleLength,
                    (PackDirection == PackDir.LENGTH ? Number : 1) * SingleWidth,
                    SingleHeight
                    );
        public Vector3D Center3D =>
            BoxPositionMain.Position
                    + 0.5 * Dimensions.X * HalfAxis.ToVector3D(BoxPositionMain.DirectionLength)
                    + 0.5 * Dimensions.Y * HalfAxis.ToVector3D(BoxPositionMain.DirectionWidth)
                    + 0.5 * Dimensions.Z * Vector3D.CrossProduct(HalfAxis.ToVector3D(BoxPositionMain.DirectionLength), HalfAxis.ToVector3D(BoxPositionMain.DirectionWidth));
        public Vector2D Center2D => new Vector2D(BoxPositionMain.Position.X, BoxPositionMain.Position.Y) + 0.5 * (Length * VLength + Width * VWidth);
        public int RawAngle
        {
            get
            {
                switch (BoxPositionMain.DirectionLength)
                {
                    case HalfAxis.HAxis.AXIS_X_P: return 0;
                    case HalfAxis.HAxis.AXIS_Y_P: return 90;
                    case HalfAxis.HAxis.AXIS_X_N: return 180;
                    case HalfAxis.HAxis.AXIS_Y_N: return 270;
                    default: return 0;
                }
            }
        }
        public bool IsSingle => Number == 1;
        public double SingleLength => Content.Length;
        public double SingleWidth => Content.Width;
        public double SingleHeight => Content.Height;
        public double Length => (PackDirection == PackDir.LENGTH ? 1 : Number) * SingleLength;
        public double Width => (PackDirection == PackDir.WIDTH ? 1 : Number) * SingleWidth;
        public double TopHeight => BoxPositionMain.Position.Z + Parent.LayerThickness;
        #endregion
        #region Public methods
        public BoxPosition InnerBoxPosition(int index)
        {
            Vector3D offset = Vector3D.Zero;
            switch (PackDirection)
            {
                case PackDir.LENGTH:
                    offset = SingleWidth * index * HalfAxis.ToVector3D(BoxPositionMain.DirectionWidth);
                    break;
                case PackDir.WIDTH:
                    offset = SingleLength * index * HalfAxis.ToVector3D(BoxPositionMain.DirectionLength);
                    break;
            }
            return new BoxPosition(
                BoxPositionMain.Position + offset,
                BoxPositionMain.DirectionLength,
                BoxPositionMain.DirectionWidth);
        }

        public double MaxDistanceToPoint(Vector3D pt) => CornerPoints.Max(corner => (corner - pt).GetLength());
        public Vector3D[] CornerPoints => BoxPositionMain.Points(Dimensions);
        public Vector2D[] Contour
        { 
            get
            {
                var vi = Length * VLength;
                var vj = Width * VWidth;
                var pos = new Vector2D(BoxPositionMain.Position.X, BoxPositionMain.Position.Y);
                return new Vector2D[] { pos, pos + vi, pos + vi + vj, pos + vj, pos };
            }
        }

        #endregion
        #region Static merge methods
        public static RobotDrop Merge2(RobotLayer layer, RobotDrop rd1, RobotDrop rd2)
        {
            Vector3D offset = Vector3D.Zero;
            PackDir packDir;
            if (rd2.IsRelAbove(rd1))
            { packDir = PackDir.LENGTH; }
            else if (rd2.IsRelUnder(rd1))
            { packDir = PackDir.LENGTH; offset = -rd1.SingleWidth * HalfAxis.ToVector3D(rd1.BoxPositionMain.DirectionWidth); }
            else if (rd2.IsRelRight(rd1))
            { packDir = PackDir.WIDTH; }
            else if (rd2.IsRelLeft(rd1))
            { packDir = PackDir.WIDTH; offset = -rd1.SingleLength * HalfAxis.ToVector3D(rd1.BoxPositionMain.DirectionLength); }
            else
                return null;
            return new RobotDrop(layer)
            {
                ID = -1,
                Number = 2,
                BoxPositionMain = new BoxPosition(rd1.BoxPositionMain.Position + offset, rd1.BoxPositionMain.DirectionLength, rd2.BoxPositionMain.DirectionWidth),
                PackDirection = packDir
            };
        }
        public static RobotDrop Merge3(RobotLayer layer, RobotDrop rd1, RobotDrop rd2, RobotDrop rd3)
        {
            Vector3D offset = Vector3D.Zero;
            PackDir packDir;
            if (rd2.IsRelAbove(rd1) && rd3.IsRelAbove(rd2))
            { packDir = PackDir.LENGTH; }
            else if (rd2.IsRelUnder(rd1) && rd3.IsRelUnder(rd2))
            { packDir = PackDir.LENGTH; offset = -2 * rd1.SingleWidth * HalfAxis.ToVector3D(rd1.BoxPositionMain.DirectionWidth); }
            else if (rd2.IsRelRight(rd1) && rd3.IsRelRight(rd2))
            { packDir = PackDir.WIDTH; }
            else if (rd2.IsRelLeft(rd1) && rd3.IsRelLeft(rd2))
            { packDir = PackDir.WIDTH; offset = -2 * rd1.SingleLength * HalfAxis.ToVector3D(rd1.BoxPositionMain.DirectionLength); }
            else
                return null;

            return new RobotDrop(layer)
            {
                ID = -1,
                Number = 3,
                BoxPositionMain = new BoxPosition(rd1.BoxPositionMain.Position + offset, rd1.BoxPositionMain.DirectionLength, rd1.BoxPositionMain.DirectionWidth),
                PackDirection = packDir
            };
        }
        public static bool CanMerge(RobotDrop drop0, RobotDrop drop1)
        {
            if (drop0 == drop1)
                return false;
            if (!drop0.IsSingle && !drop1.IsSingle)
                return false;
            if (!BoxPosition.HaveSameOrientation(drop0.BoxPositionMain, drop1.BoxPositionMain))
                return false;
            return drop1.IsRelAbove(drop0) || drop1.IsRelUnder(drop0) || drop1.IsRelLeft(drop0) || drop1.IsRelRight(drop0);
        }
        public static bool CanMerge(RobotDrop drop0, RobotDrop drop1, RobotDrop drop2)
        {
            if (drop0 == drop1 || drop1 == drop2 || drop0 == drop2)
                return false;
            if (!drop0.IsSingle && !drop1.IsSingle && !drop2.IsSingle)
                return false;
            if (!(BoxPosition.HaveSameOrientation(drop0.BoxPositionMain, drop1.BoxPositionMain) && BoxPosition.HaveSameOrientation(drop1.BoxPositionMain, drop2.BoxPositionMain)))
                return false;
            return (drop1.IsRelAbove(drop0) && drop2.IsRelAbove(drop1))
                || (drop1.IsRelUnder(drop0) && drop2.IsRelUnder(drop1))
                || (drop1.IsRelRight(drop0) && drop2.IsRelRight(drop1))
                || (drop1.IsRelLeft(drop0) && drop2.IsRelLeft(drop1));
        }
        public static List<RobotDrop> Split(RobotLayer layer, RobotDrop drop)
        {
            var list = new List<RobotDrop>();

            Vector3D offset = drop.PackDirection == PackDir.LENGTH
                ? drop.SingleWidth * HalfAxis.ToVector3D(drop.BoxPositionMain.DirectionWidth)
                : drop.SingleLength * HalfAxis.ToVector3D(drop.BoxPositionMain.DirectionLength);

            for (int i = 0; i < drop.Number; ++i)
                list.Add(
                    new RobotDrop(layer)
                    {
                        ID = -1,
                        PackDirection = drop.PackDirection, 
                        BoxPositionMain = new BoxPosition(drop.BoxPositionMain.Position + i * offset, drop.BoxPositionMain.DirectionLength, drop.BoxPositionMain.DirectionWidth), 
                        Number = 1
                    }
                    );

            return list;
        }
        public static bool VectorNearlyEqual(Vector2D v1, Vector2D v2, double tol) => Math.Abs(v2.X - v1.X) < tol && Math.Abs(v2.Y - v1.Y) < tol;
        #endregion
        #region Helpers
        private Vector2D DiffCenter(RobotDrop rd)
        {
            var diffCenter3 = Center3D - rd.Center3D;
            return new Vector2D(diffCenter3.X, diffCenter3.Y);
        }
        private bool IsRelAbove(RobotDrop rd) => VectorNearlyEqual(DiffCenter(rd), rd.TopVector, 0.1 * SingleWidth);
        private bool IsRelUnder(RobotDrop rd) => VectorNearlyEqual(DiffCenter(rd), -rd.TopVector, 0.1 * SingleWidth);
        private bool IsRelRight(RobotDrop rd) => VectorNearlyEqual(DiffCenter(rd), rd.RightVector, 0.1 * SingleLength);
        private bool IsRelLeft(RobotDrop rd) => VectorNearlyEqual(DiffCenter(rd), -RightVector, 0.1 * SingleLength);
        private Vector2D TopVector
        {
            get
            {
                var vY = SingleWidth * HalfAxis.ToVector3D(BoxPositionMain.DirectionWidth);
                return new Vector2D(vY.X, vY.Y);
            }
        }
        private Vector2D RightVector
        {
            get
            {
                var vX = SingleLength * HalfAxis.ToVector3D(BoxPositionMain.DirectionLength);
                return new Vector2D(vX.X, vX.Y);
            }
        }
        private Vector2D VLength
        { 
            get
            {
                var vLength3 = HalfAxis.ToVector3D(BoxPositionMain.DirectionLength);
                return new Vector2D(vLength3.X, vLength3.Y);
            }
        }
        private Vector2D VWidth
        {
            get
            {
                var vWidth3 = HalfAxis.ToVector3D(BoxPositionMain.DirectionWidth);
                return new Vector2D(vWidth3.X, vWidth3.Y);
            }
        }
        private double MinX => Contour.Select(pt => pt.X).Min();
        private double MinY => Contour.Select(pt => pt.Y).Min();
        private double MaxX => Contour.Select(pt => pt.X).Max();
        private double MaxY => Contour.Select(pt => pt.Y).Max();
        private Vector2D PtMin => new Vector2D(MinX, MinY);
        private Vector2D PtMax => new Vector2D(MaxX, MaxY);
        private bool IsPointInside(Vector2D pt) => MinX <= pt.X && pt.X <= MaxX && MinY <= pt.Y && pt.Y <= MaxY;
        private Vector2D Pos2D => new Vector2D(BoxPositionMain.Position.X, BoxPositionMain.Position.Y);
        private Vector2D RightMiddle
        {
            get
            {
                switch (BoxPositionMain.DirectionLength)
                {
                    case HalfAxis.HAxis.AXIS_X_P: return Pos2D + Length * VLength + 0.5 * Width * VWidth;
                    case HalfAxis.HAxis.AXIS_Y_P: return Pos2D + 0.5 * Length * VLength;
                    case HalfAxis.HAxis.AXIS_X_N: return Pos2D + 0.5 * Width * VWidth;
                    case HalfAxis.HAxis.AXIS_Y_N: return Pos2D + 0.5 * Length * VLength + Width * VWidth;
                    default: return Pos2D;
                }
            }
        }
        private Vector2D TopMiddle
        {
            get
            {
                switch (BoxPositionMain.DirectionLength)
                {
                    case HalfAxis.HAxis.AXIS_X_P: return Pos2D + 0.5 * Length * VLength + Width * VWidth;
                    case HalfAxis.HAxis.AXIS_Y_P: return Pos2D + Length * VLength + 0.5 * Width * VWidth;
                    case HalfAxis.HAxis.AXIS_X_N: return Pos2D + 0.5 * Length * VLength;
                    case HalfAxis.HAxis.AXIS_Y_N: return Pos2D + 0.5 * Width * VWidth;
                    default: return Pos2D;
                }
            }
        }
        private Vector2D LeftMiddle
        {
            get
            {
                switch (BoxPositionMain.DirectionLength)
                {
                    case HalfAxis.HAxis.AXIS_X_P: return Pos2D + 0.5 * Width * VWidth;
                    case HalfAxis.HAxis.AXIS_Y_P: return Pos2D + 0.5 * Length * VLength + Width * VWidth;
                    case HalfAxis.HAxis.AXIS_X_N: return Pos2D + Length * VLength + 0.5 * Width * VWidth;
                    case HalfAxis.HAxis.AXIS_Y_N: return Pos2D + 0.5 * Length * VLength;
                    default: return Pos2D;
                }
            }
        }
        private Vector2D BottomMiddle
        {
            get
            {
                switch (BoxPositionMain.DirectionLength)
                {
                    case HalfAxis.HAxis.AXIS_X_P: return Pos2D + 0.5 * Length * VLength;
                    case HalfAxis.HAxis.AXIS_Y_P: return Pos2D + 0.5 * Width * VWidth;
                    case HalfAxis.HAxis.AXIS_X_N: return Pos2D + 0.5 * Length * VLength + Width * VWidth;
                    case HalfAxis.HAxis.AXIS_Y_N: return Pos2D + Length * VLength + 0.5 * Width * VWidth;
                    default: return Pos2D;
                }
            }
        }
        public bool HasNeighbour(NeighbourDir dir, double dist)
        {
            Vector2D ptDir = GetNeighbourPoint(dir, dist);
            int id = -1;
            var listDrop = Parent.Drops.Where(d => d.IsPointInside(ptDir)).ToList();
            if (listDrop.Count > 0 && listDrop.First().ID < ID)
                id = listDrop.First().ID;
            return id != -1;
        }
        private Vector2D GetNeighbourPoint(NeighbourDir dir, double dist)
        {
            switch (dir)
            {
                case NeighbourDir.RIGHT: return OuterPt(RightMiddle, dist);
                case NeighbourDir.TOP: return OuterPt(TopMiddle, dist);
                case NeighbourDir.LEFT: return OuterPt(LeftMiddle, dist);
                case NeighbourDir.BOTTOM: return OuterPt(BottomMiddle, dist);
                default: return Center2D;
            }
        }
        private Vector2D UnitVector(Vector2D pt0, Vector2D pt1) => (pt1 - pt0) / (pt1 - pt0).GetLength();
        private Vector2D OuterPt(Vector2D pt, double dist) => pt + dist * UnitVector(Center2D, pt);
        #endregion
        #region Data members
        public int ID { get; set; }
        public int Number { get; set; }
        public PackDir PackDirection { get; set; }
        public BoxPosition BoxPositionMain { get; set; }
        private RobotLayer Parent { get; set; }
        #endregion
    }
    #endregion
    #region RobotLayer
    public class RobotLayer
    {
        #region Constructor
        public RobotLayer(RobotPreparation parent, int layerID)
        {
            Parent = parent;
            LayerID = layerID;
        }
        #endregion
        #region Public properties
        public RobotPreparation Parent { get; }
        public int LayerID { get; }
        public Vector2D MinPoint => Parent.MinPoint;
        public Vector2D MaxPoint => Parent.MaxPoint;
        #endregion
        #region Numbering
        public void ResetNumbering() { foreach (var d in Drops) d.ID = -1; }
        public void AutomaticRenumber()
        {
            Vector3D refPoint = RefCornerPoint;
            var sortedDrops = Drops.OrderBy(d => d.MaxDistanceToPoint(refPoint)).Reverse();
            int index = 0;
            foreach (var drop in sortedDrops)
                drop.ID = index++;            
        }
        public bool IsFullyNumbered => Drops.Count(d => d.ID == -1) <= 1;
        public void CompleteNumbering(Vector3D refPoint)
        {
            int maxNumbering = Drops.Max(d => d.ID);
            var sortedDrops = Drops.OrderBy(d => d.MaxDistanceToPoint(refPoint)).Reverse();
            foreach (var drop in sortedDrops)
            {
                if (drop.ID == -1)
                    drop.ID = ++maxNumbering;
            }
        }
        public void SortByID() { Drops.Sort(delegate (RobotDrop dropX, RobotDrop dropY) { return dropX.ID.CompareTo(dropY.ID); }); }
        private Vector3D RefCornerPoint
        {
            get
            {
                double x, y;
                switch (RefPointNumbering)
                {
                    case enuCornerPoint.LOWERLEFT: x = MinPoint.X; y = MinPoint.Y; break;
                    case enuCornerPoint.LOWERRIGHT: x = MaxPoint.X; y = MinPoint.Y; break;
                    case enuCornerPoint.UPPERRIGHT: x = MaxPoint.X; y = MaxPoint.Y; break;
                    case enuCornerPoint.UPPERLEFT: x = MinPoint.X; y = MaxPoint.Y; break;
                    default: x = 0.0; y = 0.0; break;
                }
                return new Vector3D(x, y, 0.0);
            }
        }
        #endregion
        #region Merge methods / Split
        public bool Merge(int number, int[] arrIndexes)
        {
            RobotDrop mergeDrop = null;
            if (number == 2)
            {
                if (!RobotDrop.CanMerge(Drops[arrIndexes[0]], Drops[arrIndexes[1]]))
                    return false;
                mergeDrop = RobotDrop.Merge2(this, Drops[arrIndexes[0]], Drops[arrIndexes[1]]);
            }
            else if (number == 3)
            {
                if (!RobotDrop.CanMerge(Drops[arrIndexes[0]], Drops[arrIndexes[1]], Drops[arrIndexes[2]]))
                    return false;
                mergeDrop = RobotDrop.Merge3(this, Drops[arrIndexes[0]], Drops[arrIndexes[1]], Drops[arrIndexes[2]]);
            }
            Drops.Add(mergeDrop);

            // sort indexes in order to remove higher indexes first
            Array.Sort(arrIndexes);
            Array.Reverse(arrIndexes);

            for (int i=0; i<number; ++i)
                Drops.RemoveAt(arrIndexes[i]);

            AutomaticRenumber();
            Parent.Update();

            return true;
        }
        public void Split(int index)
        {
            RobotDrop drop = Drops[index];
            List<RobotDrop> listDrop = RobotDrop.Split(this, drop);

            Drops.RemoveAt(index);
            foreach (var d in listDrop)
                Drops.Add(d);

            AutomaticRenumber();
            Parent.Update();
        }
        #endregion
        #region Data members
        public List<RobotDrop> Drops { get; set; } = new List<RobotDrop>();
        public double LayerThickness { get; set; }
        #endregion
        #region Enums
        public enum enuCornerPoint { LOWERLEFT, LOWERRIGHT, UPPERRIGHT, UPPERLEFT }
        public static enuCornerPoint RefPointNumbering { get; set; }
        #endregion
    }
    #endregion
    #region RobotPreparation
    public class RobotPreparation
    {
        #region Constructor
        public RobotPreparation(AnalysisCasePallet analysis)
        {
            Analysis = analysis;
            // initialize layer types
            List<Layer3DBox> listLayerBoxes = new List<Layer3DBox>();
            Analysis.SolutionLay.GetUniqueSolutionItemsAndOccurence(ref listLayerBoxes, ref ListLayerIndexes, ref ListInterlayerIndexes);
            // build layer types
            int layerID = 0;
            foreach (var layerBox in listLayerBoxes)
            {
                var robotLayer = new RobotLayer(this, layerID++);
                LayerTypes.Add(robotLayer);
                foreach (var b in layerBox)
                {
                    robotLayer.Drops.Add(new RobotDrop(robotLayer) { BoxPositionMain = b, Number = 1, PackDirection = RobotDrop.PackDir.LENGTH });
                }
                robotLayer.AutomaticRenumber();
            }
        }
        #endregion
        #region Public accessors
        public int NumberOfLayers => Analysis.SolutionLay.LayerCount;
        public RobotLayer GetLayerType(int index)
        {
            if (index >= ListLayerIndexes.Count)
                return null;
            RobotLayer layer =  LayerTypes[ListLayerIndexes[index]];
            layer.SortByID();
            return layer;
        }
        public void GetLayers(out List<RobotLayer> layers, out List<Pair<int,double>> interlayers)
        {
            var sol = Analysis.SolutionLay;
            layers = new List<RobotLayer>();
            interlayers = new List<Pair<int,double>>();

            int iLayer = 0;
            double zLayer = 0;
            foreach (var solItem in sol.SolutionItems)
            {
                var currentLayer = sol.LayerTypes[solItem.IndexLayer];
                // 1. interlayer
                if (solItem.HasInterlayer)
                {
                    var interlayer = Analysis.Interlayer(solItem.InterlayerIndex);
                    interlayers.Add(new Pair<int,double>(solItem.InterlayerIndex, zLayer + interlayer.Thickness));
                    zLayer += interlayer.Thickness;
                }
                else
                    interlayers.Add(new Pair<int, double>(-1,0.0) );

                // 2.robot layer
                RobotLayer editedLayer = LayerTypes[ListLayerIndexes[iLayer]];
                editedLayer.SortByID();
                RobotLayer robotLayer = new RobotLayer(this, iLayer) { LayerThickness = currentLayer.LayerHeight };

                foreach (var drop in editedLayer.Drops)
                {
                    var bPos = drop.BoxPositionMain;
                    var v = bPos.Position;

                    robotLayer.Drops.Add(
                        new RobotDrop(robotLayer)
                        {
                            ID = drop.ID,
                            BoxPositionMain = new BoxPosition( new Vector3D(v.X, v.Y, zLayer), bPos.DirectionLength, bPos.DirectionWidth),
                            Number = drop.Number,
                            PackDirection = drop.PackDirection
                        }
                        );
                }
                layers.Add(robotLayer);
                // increase z by layer thickness
                zLayer += currentLayer.LayerHeight;
                // change layer index
                ++iLayer;
            }
        }
        public Vector2D MinPoint { get { Analysis.GetPtMinMax(out Vector2D ptMin, out Vector2D ptMax); return ptMin; } }
        public Vector2D MaxPoint { get { Analysis.GetPtMinMax(out Vector2D ptMin, out Vector2D ptMax); return ptMax; } }
        public Vector3D ContentDimensions => Analysis.ContentDimensions;
        public Vector3D PalletDimensions
        {
            get
            {
                var palletProperties = Analysis.PalletProperties;
                return new Vector3D(palletProperties.Length, palletProperties.Width, palletProperties.Height);
            } 
        }
        public Packable Content => Analysis.Content;
        public int LayerCount => ListLayerIndexes.Count;
        public int DropCount
        {
            get
            {
                int dropCount = 0;
                foreach (var layerIndex in ListLayerIndexes)
                    dropCount += LayerTypes[layerIndex].Drops.Count;
                return dropCount;
            }
        }
        public int NumberOfPlaceCycles
        {
            get
            {
                int cycleCount = 0;
                var sol = Analysis.SolutionLay;
                foreach (var solItem in sol.SolutionItems)
                {
                    var layer = LayerTypes[solItem.IndexLayer];
                    cycleCount += layer.Drops.Count;
                    cycleCount += solItem.HasInterlayer ? 1 : 0;
                }
                return cycleCount;
            }
        }
        /// <summary>
        /// Grabber angle
        /// </summary>
        public int AngleGrabber { get; set; }
        public int AngleItem { get; set; }
        public int Facing => (Analysis.Content as PackableBrick).Facing;
        public Vector3D DockingOffsets { get; set; } = new Vector3D(30.0, 30.0, 40.0);
        #endregion
        #region Delegate / Event / Event triggering
        public delegate void DelegateLayerModified();
        public event DelegateLayerModified LayerModified;
        public void Update()
        {
            LayerModified?.Invoke();
        }
        #endregion
        #region Data members
        public AnalysisCasePallet Analysis { get; protected set; }
        public List<RobotLayer> LayerTypes { get; set; } = new List<RobotLayer>();
        public List<int> ListInterlayerIndexes = new List<int>();
        public List<int> ListLayerIndexes = new List<int>();
        #endregion
    }
    #endregion
}
