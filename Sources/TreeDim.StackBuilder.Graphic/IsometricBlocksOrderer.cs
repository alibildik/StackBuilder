#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Sharp3D
using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics.IsometricBlocks
{
    // see tutorial and javascript code provided by Shaun Lebron
    // https://github.com/shaunlebron/IsometricBlocks
    //

    #region IsoPos
    internal struct IsoPos
    {
        public IsoPos(double x, double y, double h, double v)
        { X = x; Y = y;  H = h; V = v; }
        public double X { get; set; }
        public double Y { get; set; }
        public double H { get; set; }
        public double V { get; set; }
    }
    internal struct BB
    {
        public BB(Vector3D ptMin, Vector3D ptMax)
        { PtMin = ptMin; PtMax = ptMax; }
        public Vector3D PtMin { get; set; }
        public Vector3D PtMax { get; set; }
    }
    internal struct IsoBB
    {
        public IsoBB(IsoPos posMin, IsoPos posMax) { PosMin = posMin; PosMax = posMax; }
        public IsoPos PosMin { get; set; }
        public IsoPos PosMax { get; set; }
    }
    #endregion

    #region Camera
    internal class Camera
    {
        #region Data members
        private Vector3D _origin;
        private readonly double _scale;
        #endregion

        #region Constructor
        public Camera(Vector3D origin, double scale)
        {
            _origin = origin; _scale = scale;
        }
        #endregion

        #region Public methods

        public IsoPos SpaceToIso(Vector3D spacePos)
        {
            var z = spacePos.Z;

            var x = spacePos.X + z;
            var y = spacePos.Y + z;

            return new IsoPos(
                x,
                y,
                (x - y) * Math.Sqrt(3.0) / 2.0,     // Math.cos(Math.PI/6)
                (x + y) / 2.0                       // Math.sin(Math.PI/6)
                );
		}
        private Vector2D IsoToScreen(IsoPos isoPos)
        {
            return new Vector2D(
                isoPos.H * this._scale + this._origin.X,
                -isoPos.V * this._scale + this._origin.Y
                );
		}
        private Vector2D SpaceToScreen(Vector3D spacePos)
        {
            return IsoToScreen(SpaceToIso(spacePos));
        }
        #endregion
    }
    #endregion

    #region Block
    internal class Block
    {
        #region Constructor
        public Block(Box b)
        {
            InternalBox = b;
        }
        #endregion

        #region Accessors
        public Box InternalBox { get; set; }
        public Vector3D Pos => InternalBox.PtMin;
        public Vector3D Size => InternalBox.BBox.DimensionsVec;
        public BB Bounds => new BB(InternalBox.BBox.PtMin, InternalBox.BBox.PtMax);
        public Dictionary<string, Vector3D> NamedSpaceVert
        {
            get
            {
                var p = Pos;
                var s = Size;
                var namedSpaceVert = new Dictionary<string, Vector3D>
                {
                    ["rightDown"] = new Vector3D(p.X + s.X, p.Y, p.Z),
                    ["leftDown"] = new Vector3D(p.X, p.Y + s.Y, p.Z),
                    ["backDown"] = new Vector3D(p.X + s.X, p.Y + s.Y, p.Z),
                    ["frontDown"] = new Vector3D(p.X, p.Y, p.Z),
                    ["rightUp"] = new Vector3D(p.X + s.X, p.Y, p.Z + s.Z),
                    ["leftUp"] = new Vector3D(p.X, p.Y + s.Y, p.Z + s.Z),
                    ["backUp"] = new Vector3D(p.X + s.X, p.Y + s.Y, p.Z + s.Z),
                    ["frontUp"] = new Vector3D(p.X, p.Y, p.Z + s.Z)
                };
                return namedSpaceVert;
            }
        }
        public Dictionary<string, IsoPos> GetIsoVert(Camera camera)
        {
            var verts = NamedSpaceVert;
            return new Dictionary<string, IsoPos>
            {
                ["rightDown"] = camera.SpaceToIso(verts["rightDown"]),
                ["leftDown"] = camera.SpaceToIso(verts["leftDown"]),
                ["backDown"] = camera.SpaceToIso(verts["backDown"]),
                ["frontDown"] = camera.SpaceToIso(verts["frontDown"]),
                ["rightUp"] = camera.SpaceToIso(verts["rightUp"]),
                ["leftUp"] = camera.SpaceToIso(verts["leftUp"]),
                ["backUp"] = camera.SpaceToIso(verts["backUp"]),
                ["frontUp"] = camera.SpaceToIso(verts["frontUp"])
            };
        }

        public IsoBB GetIsoBounds(Camera camera)
        {
            var isoVerts = GetIsoVert(camera);
            double xmin = isoVerts["frontDown"].X;
            double xmax = isoVerts["backUp"].X;
            double ymin = isoVerts["frontDown"].Y;
            double ymax = isoVerts["backUp"].Y;
            double hmin = isoVerts["leftDown"].H;
            double hmax = isoVerts["rightDown"].H;
            return new IsoBB(new IsoPos(xmin, ymin, hmin, 0.0), new IsoPos(xmax, ymax, hmax, 0.0));
        }
        #endregion

        #region Helpers
        public enum SepAxis
        {
            X,
            Y,
            Z,
            H,
            V,
            Unknown
        }
        private static bool AreRangesDisjoint(double amin, double amax, double bmin, double bmax)
        {
            return (amax <= bmin || bmax <= amin);
        }
        public static SepAxis GetIsoSepAxis(Block block_a, Block block_b, Camera camera)
        {
            IsoBB a = block_a.GetIsoBounds(camera);
            IsoBB b = block_b.GetIsoBounds(camera);

            SepAxis sepAxis = SepAxis.Unknown;
            if (AreRangesDisjoint(a.PosMin.X, a.PosMax.X, b.PosMin.X, b.PosMax.X))
            {
                sepAxis = SepAxis.X;
            }
            if (AreRangesDisjoint(a.PosMin.Y, a.PosMax.Y, b.PosMin.Y, b.PosMax.Y))
            {
                sepAxis = SepAxis.Y;
            }
            if (AreRangesDisjoint(a.PosMin.H, a.PosMax.H, b.PosMin.H, b.PosMax.H))
            {
                sepAxis = SepAxis.H;
            }
            return sepAxis;
        }
        public static SepAxis GetSpaceSepAxis(Block block_a, Block block_b)
        {
            BB a = block_a.Bounds, b = block_b.Bounds;
            SepAxis sepAxis = SepAxis.Unknown;
            if (AreRangesDisjoint(a.PtMin.X, a.PtMax.X, b.PtMin.X, b.PtMax.X))
            {
                sepAxis = SepAxis.X;
            }
            else if (AreRangesDisjoint(a.PtMin.Y, a.PtMax.Y, b.PtMin.Y, b.PtMax.Y))
            {
                sepAxis = SepAxis.Y;
            }
            else if (AreRangesDisjoint(a.PtMin.Z, a.PtMax.Z, b.PtMin.Z, b.PtMax.Z))
            {
                sepAxis = SepAxis.Z;
            }
            return sepAxis;
        }
        public static Block GetFrontBlock(Block block_a, Block block_b, Camera camera)
        {
            // If no isometric separation axis is found,
            // then the two blocks do not overlap on the screen.
            // This means there is no "front" block to identify.
            if (SepAxis.Unknown == GetIsoSepAxis(block_a, block_b, camera))
                return null;

            // Find a 3D separation axis, and use it to determine
            // which block is in front of the other.
            var a = block_a.Bounds;
            var b = block_b.Bounds;
            switch (GetSpaceSepAxis(block_a, block_b))
            {
                case SepAxis.X: return (a.PtMin.X < b.PtMin.X) ? block_a : block_b;
                case SepAxis.Y: return (a.PtMin.Y < b.PtMin.Y) ? block_a : block_b;
                case SepAxis.Z: return (a.PtMin.Z < b.PtMin.Z) ? block_b : block_a;
                default: throw new Exception("blocks must be non-intersecting");
            }
        }
        #endregion

        #region Data members
        public List<Block> blocksBehind = new List<Block>();
        public List<Block> blocksInFront = new List<Block>();
        #endregion
    }
    #endregion

    #region IsometricBlocksOrderer
    public class IsometricBlocksOrderer : BoxOrderer
    {
        public override List<BoxGeneric> GetSortedList()
        {
            List<Block> blocks = new List<Block>();
            foreach (Box box in Boxes)
                blocks.Add(new Block(box));

            // Initialize the list of blocks that each block is behind.
            foreach (Block block in blocks)
            {
                block.blocksBehind.Clear();
                block.blocksInFront.Clear();
            }

            // foreach pair of blocks, determine which is in front and behind.
            int numBlock = blocks.Count;
            for (int i = 0; i < numBlock; i++)
            {
                Block a = blocks[i];
                for (int j = i+1; j < numBlock; j++)
                {
                    Block b = blocks[j];
                    Block frontBlock = Block.GetFrontBlock(a, b, camera);
                    if (a == frontBlock)
                    {
                        if (a == frontBlock)
                        {
                            a.blocksBehind.Add(b);
                            b.blocksInFront.Add(a);
                        }
                        else
                        {
                            b.blocksBehind.Add(a);
                            a.blocksInFront.Add(b);
                        }
                    }
                }
            }
            // Get list of blocks we can safely draw right now.
            // These are the blocks with nothing behind them.
            var blocksToDraw = new List<Block>();
            for (int i = 0; i < numBlock; ++i)
                if (blocks[i].blocksBehind.Count == 0)
                    blocksToDraw.Add(blocks[i]);

            // While there are still blocks we can draw...
            var blocksDrawn = new List<Block>();
            while (blocksToDraw.Count > 0)
            {
                // draw block by removing one from "to draw" and adding
                // it to the end of our "drawn" list
                var block = blocksToDraw.First();
                blocksToDraw.Remove(block);
                blocksDrawn.Add(block);

                // Tell blocks in front of the one we just drew
                // that they can stop waiting on it
                for (int j = 0; j < block.blocksInFront.Count; j++)
                {
                    var frontBlock = block.blocksInFront[j];

                    // add this front to our "to draw" list if there is
                    // nothing else behind it waiting to be drawn.
                    frontBlock.blocksBehind.Remove(block);
                    if (frontBlock.blocksBehind.Count == 0)
                        blocksToDraw.Add(frontBlock);
                }
            }
            // convert to sorted box list
            var boxesDrawn = new List<BoxGeneric>();
            foreach (Block bdrawn in blocksDrawn)
                boxesDrawn.Add(bdrawn.InternalBox);
            return boxesDrawn;
        }

        readonly Camera camera = new Camera(Vector3D.Zero, 1.0);
    }
    #endregion
}
