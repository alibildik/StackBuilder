#region Using directives
using System;
using System.Linq;
using System.Drawing;

using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
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
    public JJAConfig(double[] dimensions, double weight, int configIndex)
    {
        Dimensions = dimensions.OrderByDescending(x => x).ToArray();
        ConfigIndex = configIndex;
        Weight = weight;
    }
    #endregion
    #region Public properties
    public double Length
    {
        get
        {
            switch (ConfigIndex)
            {
                case 1: return Dimensions[0];
                case 2: return Dimensions[2];
                default: return Dimensions[0];
            }
        }
    }
    public double Width
    {
        get
        {
            switch (ConfigIndex)
            {
                case 1: return Dimensions[1];
                case 2: return Dimensions[1];
                default: return Dimensions[2];
            }
        }
    }

    public double Height
    {
        get
        {
            switch (ConfigIndex)
            {
                case 1: return Dimensions[2];
                case 2: return Dimensions[0];
                default: return Dimensions[1];
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
            for (int i = 1; i < 4; ++i)
            {
                var jjaconfig = new JJAConfig(Dimensions, Weight, i);
                if (jjaconfig.Stability == eStable.STABLE && jjaconfig.Conveyability == eConveyability.CONVEYABLE)
                {
                    switch (i)
                    {
                        case 1: return eConveyFace.BOTTOMTOP;
                        case 2: return eConveyFace.FRONTBACK;
                        case 3: return eConveyFace.LEFTRIGHT;
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
    public Bitmap GetImage(int sizeX = 150, int sizeY = 150, float fontSizeRatio = 0.02f, bool showCotations = true)
    {
        Graphics3DImage graphics = new Graphics3DImage(new Size(sizeX, sizeY))
        {
            FontSizeRatio = fontSizeRatio,
            CameraPosition = Graphics3D.Corner_0,
            Target = Vector3D.Zero
        };
        BoxProperties boxProperties = new BoxProperties(null, Length, Width, Height, Weight, ColorCase);
        Box box = new Box(0, boxProperties);
        graphics.AddBox(box);
        if (showCotations)
            graphics.AddDimensions(new DimensionCube(box.Length, box.Width, box.Height));
        graphics.Flush();
        return graphics.Bitmap;
    }
    public byte[] GetImageBytes(int sizeX = 150, int sizeY = 150, float fontSizeRatio = 0.02f, bool showCotations = true)
    {
        Bitmap bmp = GetImage(sizeX, sizeY, fontSizeRatio, showCotations);
        ImageConverter converter = new ImageConverter();
        return (byte[])converter.ConvertTo(bmp, typeof(byte[]));
    }
    #endregion
    #region Data members
    public double[] Dimensions { get; }
    public double Weight { get; }
    public int ConfigIndex { get; set; }
    public Color ColorCase { get; set; } = Color.Chocolate;
    public Color ColorTape { get; set; } = Color.Beige;
    #endregion
}