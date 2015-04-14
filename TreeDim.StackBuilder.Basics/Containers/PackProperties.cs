﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Sharp3D.Math.Core;
#endregion

namespace TreeDim.StackBuilder.Basics
{
    #region Wrappers
    public abstract class PackWrapper
    { 
        public enum WType
        { 
            WT_POLYETHILENE
            , WT_PAPER
            , WT_CARDBOARD
            , WT_TRAY
        }
        public PackWrapper(double thickness) { _thickness = thickness; }
        public double Weight { get; set; }
        public Color Color { get; set; }
        virtual public bool Transparent { get { return false; } } 

        abstract public WType Type { get;}
        virtual public double Thickness(int dir) { return _thickness; }
        // data members
        protected double _thickness;
    }
    public class WrapperPolyethilene : PackWrapper
    {
        /// <summary>
        /// constructor
        /// </summary>
        public WrapperPolyethilene(double thickness) : base(thickness) { }
        public override bool Transparent { get { return _transparent; } }
        // implement abstract methods
        public override PackWrapper.WType Type { get { return PackWrapper.WType.WT_POLYETHILENE; } }
        // data members
        public bool _transparent = true;
    }
    public class WrapperPaper : PackWrapper
    {
        /// <summary>
        /// constructor
        /// </summary>
        public WrapperPaper(double thickness) : base(thickness) { }
        // implement abstract methods
        public override PackWrapper.WType Type { get { return PackWrapper.WType.WT_PAPER; } }
        // data members
    }
    public class WrapperCardboard : PackWrapper
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="thickness">Cardboard thickness</param>
        public WrapperCardboard(double thickness)  : base(thickness) { }
        public void SetNoWalls(int noWallX, int noWallY, int noWallZ)
        { walls[0] = noWallX; walls[1] = noWallY; walls[2] = noWallZ; }
        // implement abstract methods
        public override PackWrapper.WType Type { get { return PackWrapper.WType.WT_CARDBOARD; } }
        public override double Thickness(int dir) { return walls[dir] * _thickness; }
        // data members
        private int[] walls = new int[3];
    }
    public class WrapperTray : PackWrapper
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="thickness">Cardboard thickness</param>
        public WrapperTray(double thickness) : base(thickness) {}
        public void SetNoWalls(int noWallX, int noWallY, int noWallZ)
        { walls[0] = noWallX; walls[1] = noWallY; walls[2] = noWallZ; }
        /// <summary>
        /// Tray height
        /// </summary>
        public double Height { get { return _height; } set { _height = value; } }
        // implement abstract methods
        public override PackWrapper.WType Type { get { return PackWrapper.WType.WT_TRAY; } }
        public override double Thickness(int dir) { return walls[dir] * _thickness; }

        // data members
        private int[] walls = new int[3];
        private double _height;
    }
    #endregion

    #region PackProperties
    public class PackProperties : ItemBase
    {
        #region Data members
        private BoxProperties _boxProperties;
        private HalfAxis.HAxis _orientation;
        private PackArrangement _arrangement;
        private PackWrapper _wrapper;
        private bool _forceOuterDimensions = false;
        private Vector3D _outerDimensions;
        #endregion
        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="doc">Reference of parent <see cref="Document"/></param>
        /// <param name="box">Reference </param>
        public PackProperties(Document doc
            , BoxProperties box
            , PackArrangement arrangement
            , HalfAxis.HAxis orientation
            , PackWrapper wrapper)
            : base(doc)
        {
            _boxProperties = box;
            _boxProperties.AddDependancy(this);

            _arrangement = arrangement;
            _orientation = orientation;
            _wrapper = wrapper;
        }
        #endregion
        #region Public properties
        /// <summary>
        /// set/get reference to wrapped box
        /// </summary>
        public BoxProperties Box
        {
            get { return _boxProperties; }
            set { _boxProperties = value; }
        }
        public PackArrangement Arrangement
        {
            get { return _arrangement; }
            set { _arrangement = value; }
        }
        /// <summary>
        /// set/get orientation of 
        /// </summary>
        public HalfAxis.HAxis BoxOrientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }
        public PackWrapper Wrap
        {
            get { return _wrapper; }
            set { _wrapper = value; }
        }
        public Vector3D InnerOffset
        {
            get
            { return new Vector3D(
                0.5*(Length-InnerLength),
                0.5*(Width-InnerWidth),
                0.5*(Height-InnerHeight)
                );
            }
        }
        #endregion
        #region Outer dimensions
        public void ForceOuterDimensions(Vector3D outerDimensions)
        {
            _forceOuterDimensions = true;
            _outerDimensions = outerDimensions;
        }
        public double Length
        {
            get
            {
                if (_forceOuterDimensions)
                    return _outerDimensions.X;
                else
                    return InnerLength + 2.0 * _wrapper.Thickness(0); 
            }
        }
        public double Width
        {
            get
            {
                if (_forceOuterDimensions)
                    return _outerDimensions.Y;
                else
                    return InnerWidth + 2.0 * _wrapper.Thickness(1);
            }
        }
        public double Height
        {
            get
            {
                if (_forceOuterDimensions)
                    return _outerDimensions.Z;
                else
                    return InnerHeight + 2.0 * _wrapper.Thickness(2);
            }
        }
        #endregion
        #region Inner dimensions
        public double InnerLength { get { return _arrangement._iLength * _boxProperties.Dim(Dim0); } }
        public double InnerWidth { get { return _arrangement._iWidth * _boxProperties.Dim(Dim1); } }
        public double InnerHeight { get { return _arrangement._iHeight * _boxProperties.Dim(Dim2); } }
        #endregion
        #region Weight
        public double Weight { get { return InnerWeight + _wrapper.Weight; } }
        public double InnerWeight { get { return _arrangement.Number * _boxProperties.Weight; } }
        public OptDouble NetWeight { get { return _arrangement.Number * _boxProperties.NetWeight; } }
        #endregion
        #region Helpers
        public int Dim0 { get { return PackProperties.DimIndex0(_orientation); } }
        public int Dim1 { get { return PackProperties.DimIndex1(_orientation); } }
        public int Dim2 { get { return 3 - Dim0 - Dim1; } }
        private static int DimIndex0(HalfAxis.HAxis axis)
        {
            switch (axis)
            {
                case HalfAxis.HAxis.AXIS_X_N: return 1;
                case HalfAxis.HAxis.AXIS_X_P: return 2;
                case HalfAxis.HAxis.AXIS_Y_N: return 0;
                case HalfAxis.HAxis.AXIS_Y_P: return 2;
                case HalfAxis.HAxis.AXIS_Z_N: return 1;
                case HalfAxis.HAxis.AXIS_Z_P: return 0;
                default: return 0;
            }
        }
        private static int DimIndex1(HalfAxis.HAxis axis)
        {
            switch (axis)
            {
                case HalfAxis.HAxis.AXIS_X_N: return 2;
                case HalfAxis.HAxis.AXIS_X_P: return 1;
                case HalfAxis.HAxis.AXIS_Y_N: return 2;
                case HalfAxis.HAxis.AXIS_Y_P: return 0;
                case HalfAxis.HAxis.AXIS_Z_N: return 0;
                case HalfAxis.HAxis.AXIS_Z_P: return 1;
                default: return 1;
            }
        }
        #endregion
        #region Static
        public static void GetOuterDimensions(
            BoxProperties boxProperties
            , HalfAxis.HAxis boxOrientation
            , PackArrangement arrangement
            , ref double length, ref double width, ref double height)
        {
            if (null == boxProperties) return;
            length = arrangement._iLength * boxProperties.Dim(PackProperties.DimIndex0(boxOrientation));
            width = arrangement._iWidth * boxProperties.Dim(PackProperties.DimIndex1(boxOrientation));
            height = arrangement._iHeight * boxProperties.Dim(3 - PackProperties.DimIndex0(boxOrientation) - PackProperties.DimIndex1(boxOrientation));
        }
        #endregion
    }
    #endregion
}