#region Using directives
using System;
using System.Linq;
using System.Drawing;

using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using System.Diagnostics;
using System.Web.UI;
#endregion

internal class JJAConfig
{
    #region Enums
    public enum eStable
    {
        STABLE,
        UNSTABLE
    }
    public enum eConveyability
    {
        CONVEYABLE,
        UNCONVEYABLEDIM,
        UNCONVEYABLEWEIGHT
    }
    public enum eConveyFace
    {
        BOTTOMTOP,
        FRONTBACK,
        LEFTRIGHT, 
        UNCONVEYABLE
    }
    public enum eAutomatable
    {
        AUTOMATIC,
        MANUAL
    }
    public enum ePrepPac
    { 
        PREPPAC,
        PREPHORSPAC
    }
    #endregion
    #region Constructor
    public JJAConfig(double[] dimensions, double weight, int pcb, HalfAxis.HAxis axisOrtho)
    {
        Debug.Assert( dimensions.Length == 3 );

        Dimensions = dimensions;
        AxisOrtho = axisOrtho;
        Weight = weight;
        Pcb = pcb;
    }
    #endregion
    #region Public properties
    public double Length
    {
        get
        {
            switch (AxisOrtho)
            {
                case HalfAxis.HAxis.AXIS_Z_P: return Dimensions[0];
                case HalfAxis.HAxis.AXIS_X_P: return Math.Max(Dimensions[1], Dimensions[2]);
                case HalfAxis.HAxis.AXIS_Y_P: return Math.Max(Dimensions[0], Dimensions[2]);
                default: throw new InvalidOperationException($"Invalid Config index: {AxisOrtho}");
            }
        }
    }
    public double Width
    {
        get
        {
            switch (AxisOrtho)
            {
                case HalfAxis.HAxis.AXIS_Z_P: return Dimensions[1];
                case HalfAxis.HAxis.AXIS_X_P: return Math.Min(Dimensions[1], Dimensions[2]);
                case HalfAxis.HAxis.AXIS_Y_P: return Math.Min(Dimensions[0], Dimensions[2]);
                default: throw new InvalidOperationException($"Invalid Config index: {AxisOrtho}");
            }
        }
    }

    public double Height
    {
        get
        {
            switch (AxisOrtho)
            {
                case HalfAxis.HAxis.AXIS_Z_P: return Dimensions[2];
                case HalfAxis.HAxis.AXIS_X_P: return Dimensions[0];
                case HalfAxis.HAxis.AXIS_Y_P: return Dimensions[1];
                default: throw new InvalidOperationException($"Invalid AxisOrtho index: {HalfAxis.ToAbbrev(AxisOrtho)}");
            }
        }
    }
    public double Volume => Dimensions.Aggregate(1.0, (a, b) => a * b);
    public double AreaBottomTop => Length * Width;
    public double AreaFrontBack => Width * Height;
    public double AreaLeftRight => Length * Height;
    public eStable Stability => Height / Math.Min(Length, Width) < 1.7 ? eStable.STABLE : eStable.UNSTABLE;
    public eConveyFace ConveyFace
    {
        get
        {
            HalfAxis.HAxis[] axes = new HalfAxis.HAxis[] { HalfAxis.HAxis.AXIS_Z_P, HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_X_P };
            for (int i = 0; i < 3; ++i)
            {

                var jjaconfig = new JJAConfig(Dimensions, Weight, Pcb, axes[i]);
                if (jjaconfig.Stability == eStable.STABLE && jjaconfig.Conveyability == eConveyability.CONVEYABLE)
                {
                    switch (i)
                    {
                        case 0: return eConveyFace.BOTTOMTOP;
                        case 1: return eConveyFace.FRONTBACK;
                        case 2: return eConveyFace.LEFTRIGHT;
                        default: break;
                    }
                }
            }            
            return eConveyFace.UNCONVEYABLE;
        }
    }
    public eConveyability Conveyability
    {
        get
        {
            double[] dimsMax = { 1300.0, 665.0, 900.0};
            double[] dimsMin = { 240.0, 240.0, 100.0};
            double weightMin = 0.3, weightMax = 47.0;

            if (Length > dimsMax[0] || Length < dimsMin[0]
                || Width > dimsMax[1] || Width < dimsMin[1]
                || Height > dimsMax[2] || Height < dimsMin[2])
                return eConveyability.UNCONVEYABLEDIM;
            if (Weight > weightMax || Weight < weightMin)
                return eConveyability.UNCONVEYABLEWEIGHT;
            return eConveyability.CONVEYABLE;
        }
    }
    public eAutomatable Automatable
    {
        get
        {
            if (Stability != eStable.STABLE)
                return eAutomatable.MANUAL;

            double[] dimsMax = { 800.0, 600.0, 600.0};
            double[] dimsMin = { 240.0, 240.0, 100.0};
            double weightMin = 0.3, weightMax = 47.0;

            if (Length > dimsMax[0] || Length < dimsMin[0]
                || Width > dimsMax[1] || Width < dimsMin[1]
                || Height > dimsMax[2] || Height < dimsMin[2]
                || Weight > weightMax || Weight < weightMin)
                return eAutomatable.MANUAL;

            return eAutomatable.AUTOMATIC;
        }
    }
    public ePrepPac PrepPac
    {
        get
        {
            double[] dimsMax = { 800.0, 600.0, 420.0 };
            double[] dimsMin = { 240.0, 240.0, 100.0 };
            double weightMin = 0.3, weightMax = 47.0;

            if (Length > dimsMax[0] || Length < dimsMin[0]
                || Width > dimsMax[1] || Width < dimsMin[1]
                || Height > dimsMax[2] || Height < dimsMin[2]
                || Weight > weightMax || Weight < weightMin)
                return ePrepPac.PREPHORSPAC;

            return ePrepPac.PREPPAC;
        }
    }
    #endregion
    #region Image
    public Bitmap GetImage(int orientation, int sizeX = 150, int sizeY = 150, float fontSizeRatio = 0.02f, bool showCotations = true)
    {
        Graphics3DImage graphics = new Graphics3DImage(new Size(sizeX, sizeY))
        {
            FontSizeRatio = fontSizeRatio,
            CameraPosition = Graphics3D.Corner_0,
            Target = Vector3D.Zero
        };
        BoxPosition bp = new BoxPosition();
        Vector3D vDim = Vector3D.Zero;
        switch (orientation)
        {
            case 0:
                bp = new BoxPosition(new Vector3D(0.0, 0.0, 0.0),  HalfAxis.HAxis.AXIS_Z_P, HalfAxis.HAxis.AXIS_X_P);
                vDim = new Vector3D(Dimensions[1], Dimensions[2], Dimensions[0]);
                break;
            case 1:
                bp = new BoxPosition(new Vector3D(0.0, 0.0, Dimensions[1]), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Z_N);
                vDim = new Vector3D(Dimensions[0], Dimensions[2], Dimensions[1]);
                break;
            case 2:
                bp = new BoxPosition(Vector3D.Zero, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                vDim = new Vector3D(Dimensions[0], Dimensions[1], Dimensions[2]); 
                break;
            default: break;
        }

        BoxProperties boxProperties = new BoxProperties(null, Dimensions[0], Dimensions[1], Dimensions[2], Weight, ColorCase);
        Box box = new Box(0, boxProperties, bp);
        graphics.AddBox(box);
        if (showCotations)
            graphics.AddDimensions(new DimensionCube(vDim));
        graphics.Flush();
        return graphics.Bitmap;
    }
    public byte[] GetImageBytes(int orientation, int sizeX = 150, int sizeY = 150, float fontSizeRatio = 0.02f, bool showCotations = true)
    {
        Bitmap bmp = GetImage(orientation, sizeX, sizeY, fontSizeRatio, showCotations);
        ImageConverter converter = new ImageConverter();
        return (byte[])converter.ConvertTo(bmp, typeof(byte[]));
    }
    public static HalfAxis.HAxis Axis(int orientation)
    {
        var axes = new HalfAxis.HAxis[] { HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P, HalfAxis.HAxis.AXIS_Z_P };
        if (orientation < 0 || orientation > 2) throw new Exception($"Invalid orientation value {orientation}");
        return axes[orientation];
    }
    #endregion
    #region Data members
    public double[] Dimensions { get; }
    public double Weight { get; }
    public int Pcb { get; }
    public HalfAxis.HAxis AxisOrtho { get; set; }
    public Color ColorCase { get; set; } = Color.Chocolate;
    public Color ColorTape { get; set; } = Color.Beige;
    #endregion
}