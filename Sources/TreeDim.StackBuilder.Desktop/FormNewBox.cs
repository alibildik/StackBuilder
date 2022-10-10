﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Exporters;
using treeDiM.StackBuilder.Desktop.Properties;

using treeDiM.PLMPack.DBClient;
using treeDiM.PLMPack.DBClient.PLMPackSR;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewBox : FormNewBase, IDrawingContainer
    {
        #region Mode enum
        public enum Mode { BOX, CASE }
        private Mode ModeInit { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// FormNewBox constructor used when defining a new BoxProperties item
        /// </summary>
        /// <param name="document">Document in which the BoxProperties item is to be created</param>
        /// <param name="mode">Mode is either Mode.MODE_CASE or Mode.MODE_BOX</param>
        public FormNewBox(Document document, Mode mode)
            : base(document, null)
        {
            InitializeComponent();
            if (!DesignMode)
            {
                // set unit labels
                UnitsManager.AdaptUnitLabels(this);
                // save document reference
                _document = document;

                switch (mode)
                {
                    case Mode.CASE:
                        tbName.Text = _document.GetValidNewTypeName(Resources.ID_CASE);
                        uCtrlDimensionsOuter.ValueX = UnitsManager.ConvertLengthFrom(400.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                        uCtrlDimensionsOuter.ValueY = UnitsManager.ConvertLengthFrom(300.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                        uCtrlDimensionsOuter.ValueZ = UnitsManager.ConvertLengthFrom(200.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                        uCtrlDimensionsInner.Checked = true;
                        uCtrlDimensionsInner.Value = new Vector3D(
                            uCtrlDimensionsOuter.ValueX - UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1),
                            uCtrlDimensionsOuter.ValueY - UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1),
                            uCtrlDimensionsOuter.ValueZ - UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1));
                        uCtrlCOG.Value = 0.5 * uCtrlDimensionsOuter.Value;
                        uCtrlTapeWidth.Value = new OptDouble(true, UnitsManager.ConvertLengthFrom(50, UnitsManager.UnitSystem.UNIT_METRIC1));
                        Facing = string.IsNullOrEmpty(ExporterRobot.DefaultName) ? 0 : 1;
                        break;
                    case Mode.BOX:
                        tbName.Text = _document.GetValidNewTypeName(Resources.ID_BOX);
                        uCtrlDimensionsOuter.ValueX = UnitsManager.ConvertLengthFrom(120.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                        uCtrlDimensionsOuter.ValueY = UnitsManager.ConvertLengthFrom(60.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                        uCtrlDimensionsOuter.ValueZ = UnitsManager.ConvertLengthFrom(30.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                        uCtrlDimensionsInner.Value = new Vector3D(
                            uCtrlDimensionsOuter.ValueX - UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1),
                            uCtrlDimensionsOuter.ValueY - UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1),
                            uCtrlDimensionsOuter.ValueZ - UnitsManager.ConvertLengthFrom(6.0, UnitsManager.UnitSystem.UNIT_METRIC1));
                        uCtrlCOG.Visible = false;
                        uCtrlDimensionsInner.Checked = false;
                        uCtrlCOG.Value = 0.5 * uCtrlDimensionsOuter.Value;
                        uCtrlTapeWidth.Value = new OptDouble(false, UnitsManager.ConvertLengthFrom(10, UnitsManager.UnitSystem.UNIT_METRIC1));
                        lbFacing.Visible = false;
                        cbFacing.Visible = false;
                        Facing = 0;
                        break;
                    default:
                        break;
                }
                cbTapeColor.Color = Color.Beige;
                ModeInit = mode;
                // description (same as name)
                tbDescription.Text = tbName.Text;
                // color : all faces set together / face by face
                chkAllFaces.Checked = false;
                OnAllFacesColorCheckedChanged(this, null);
                // set colors
                for (int i = 0; i < 6; ++i)
                    _faceColors[i] = mode == Mode.BOX ? Color.Turquoise : Color.Chocolate;
                // set textures
                _textures = new List<Pair<HalfAxis.HAxis, Texture>>();
                // set default face
                cbFace.SelectedIndex = 0;
                // weight
                Weight = UnitsManager.ConvertMassFrom(1.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                // net weight
                NetWeight = new OptDouble(false, UnitsManager.ConvertMassFrom(0.0, UnitsManager.UnitSystem.UNIT_METRIC1));

                StrapperSet = new StrapperSet();
                ctrlStrapperSet.StrapperSet = StrapperSet;

                // disable Ok button
                UpdateStatus(string.Empty);
            }
        }
        /// <summary>
        /// FormNewBox constructor used to edit existing boxes
        /// </summary>
        /// <param name="document">Document that contains the edited box</param>
        /// <param name="boxProperties">Edited box</param>
        public FormNewBox(Document document, BoxProperties boxProperties)
            : base(document, boxProperties)
        { 
            InitializeComponent();
            if (!DesignMode)
            {
                // set unit labels
                UnitsManager.AdaptUnitLabels(this);
                // set colors
                for (int i = 0; i < 6; ++i)
                    _faceColors[i] = boxProperties.Colors[i];
                // set textures
                _textures = boxProperties.TextureListCopy;
                // set caption text
                Text = string.Format(Resources.ID_EDIT, boxProperties.Name);
                // initialize value
                uCtrlDimensionsOuter.ValueX = boxProperties.Length;
                uCtrlDimensionsOuter.ValueY = boxProperties.Width;
                uCtrlDimensionsOuter.ValueZ = boxProperties.Height;
                uCtrlDimensionsInner.Value = new Vector3D(boxProperties.InsideLength, boxProperties.InsideWidth, boxProperties.InsideHeight);
                uCtrlDimensionsInner.Checked = boxProperties.HasInsideDimensions;
                // COG
                COG = boxProperties.COG;
                // weight
                vcWeight.Value = boxProperties.Weight;
                // net weight
                uCtrlNetWeight.Value = boxProperties.NetWeight;
                // max weight
                uCtrlMaxWeight.Value = boxProperties.MaxWeight;
                // color : all faces set together / face by face
                chkAllFaces.Checked = boxProperties.UniqueColor;
                OnAllFacesColorCheckedChanged(this, null);
                // tape
                uCtrlTapeWidth.Value = boxProperties.TapeWidth;
                cbTapeColor.Color = boxProperties.TapeColor;
                // set default face
                cbFace.SelectedIndex = 0;
                // set StrapperSet
                StrapperSet = boxProperties.StrapperSet.Clone();
                ctrlStrapperSet.StrapperSet = StrapperSet;
                // bulge
                Bulge = boxProperties.Bulge;
                // facing mark
                Facing = boxProperties.Facing;
                // disable Ok button
                UpdateStatus(string.Empty);
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Name
        /// </summary>
        public string BoxName
        {
            get => tbName.Text;
            set => tbName.Text = value; 
        }
        /// <summary>
        /// Description
        /// </summary>
        public string Description
        {
            get => tbDescription.Text; 
            set => tbDescription.Text = value;
        }
        /// <summary>
        /// Length
        /// </summary>
        public double BoxLength
        {
            get => uCtrlDimensionsOuter.ValueX; 
            set => uCtrlDimensionsOuter.ValueX = value; 
        }
        /// <summary>
        /// Width
        /// </summary>
        public double BoxWidth
        {
            get => uCtrlDimensionsOuter.ValueY; 
            set => uCtrlDimensionsOuter.ValueY = value; 
        }
        /// <summary>
        /// Height
        /// </summary>
        public double BoxHeight
        {
            get => uCtrlDimensionsOuter.ValueZ; 
            set => uCtrlDimensionsOuter.ValueZ = value; 
        }
        /// <summary>
        /// Has inside dimensions ?
        /// </summary>
        public bool HasInsideDimensions
        {
            get => uCtrlDimensionsInner.Checked; 
            set => uCtrlDimensionsInner.Checked = value;
        }
        /// <summary>
        /// Inside length
        /// </summary>
        public double InsideLength
        {
            get => uCtrlDimensionsInner.X;
            set => uCtrlDimensionsInner.X = value; 
        }

        /// <summary>
        /// Inside width
        /// </summary>
        public double InsideWidth
        {
            get => uCtrlDimensionsInner.Y; 
            set => uCtrlDimensionsInner.Y = value; 
        }
        /// <summary>
        /// Inside height
        /// </summary>
        public double InsideHeight
        {
            get => uCtrlDimensionsInner.Z; 
            set => uCtrlDimensionsInner.Z = value; 
        }
        /// <summary>
        /// Weight
        /// </summary>
        public double Weight
        {
            get => vcWeight.Value; 
            set => vcWeight.Value = value; 
        }
        public Vector3D COG
        {
            get => uCtrlCOG.Value;
            set => uCtrlCOG.Value = value;
        }
        /// <summary>
        /// Colors
        /// </summary>
        public Color[] Colors
        {
            get => _faceColors; 
            set { }
        }
        /// <summary>
        /// Textures
        /// </summary>
        public List<Pair<HalfAxis.HAxis, Texture>> TextureList
        {
            get => _textures; 
            set
            {
                _textures.Clear();
                _textures.AddRange(value);
            }
        }
        /// <summary>
        /// Tape width
        /// </summary>
        public OptDouble TapeWidth
        {
            get => uCtrlTapeWidth.Value;
            set => uCtrlTapeWidth.Value = value; 
        }
        /// <summary>
        /// Tape color
        /// </summary>
        public Color TapeColor
        {
            get => cbTapeColor.Color;
            set => cbTapeColor.Color = value; 
        }
        /// <summary>
        /// Facing Mark
        /// </summary>
        public int Facing
        {
            get => cbFacing.SelectedIndex;
            set => cbFacing.SelectedIndex = value;
        }
        /// <summary>
        /// Bulge
        /// </summary>
        public Vector3D Bulge
        {
            get => uCtrlOptBulge.Checked ? uCtrlOptBulge.Value : Vector3D.Zero;
            set { uCtrlOptBulge.Checked = value.GetLength() > 0; uCtrlOptBulge.Value = value; }
        }
        #endregion

        #region FormNewBase override
        public override string ItemDefaultName => ModeInit == Mode.CASE ? Resources.ID_CASE : Resources.ID_BOX;
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            graphCtrl.DrawingContainer = this;
            // caption
            Text = Resources.ID_ADDNEWBOX;
            // update thicknesses
            UpdateThicknesses();
            // update tape definition controls
            OnTapeWidthChecked(this, null);
            // update box drawing
            graphCtrl.Invalidate();
            // windows settings
            if (null != Settings.Default.FormNewBoxPosition)
                Settings.Default.FormNewBoxPosition.Restore(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // window position
            if (null == Settings.Default.FormNewBoxPosition)
                Settings.Default.FormNewBoxPosition = new WindowSettings();
            Settings.Default.FormNewBoxPosition.Record(this);
        }
        protected override void OnResize(EventArgs e)
        {
 	         base.OnResize(e);
        }
        #endregion

        #region Handlers
        private void OnBoxPropertyChanged(object sender, EventArgs e)
        {
            try
            {
                // maintain inside dimensions
                if (sender is UCtrlTriDouble uCtrlDimOut && uCtrlDimensionsOuter == uCtrlDimOut)
                {
                    InsideLength = BoxLength - _thicknessLength;
                    InsideWidth = BoxWidth - _thicknessWidth;
                    InsideHeight = BoxHeight - _thicknessHeight;

                    // update COG
                    COG = new Vector3D(0.5 * BoxLength, 0.5 * BoxWidth, 0.5 * BoxHeight);
                }
                if (sender is UCtrlOptTriDouble uCtrlDimIn && uCtrlDimensionsInner == uCtrlDimIn)
                {
                    if (BoxLength < InsideLength)
                        BoxLength = InsideLength + _thicknessLength;
                    if (BoxWidth < InsideWidth)
                        BoxWidth = InsideWidth + _thicknessWidth;
                    if (BoxHeight <= InsideHeight)
                        BoxHeight = InsideHeight + _thicknessHeight;

                    // update strapper set
                    StrapperSet.SetDimension(BoxLength, BoxWidth, BoxHeight);
                }
                uCtrlNetWeight.Enabled = !uCtrlDimensionsInner.Checked;
                uCtrlMaxWeight.Enabled = uCtrlDimensionsInner.Checked;

                // update thicknesses
                UpdateThicknesses();
                // update ok button status
                UpdateStatus(string.Empty);
                // update box drawing
                graphCtrl.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void OnSelectedFaceChanged(object sender, EventArgs e)
        {
            // get current index
            int iSel = cbFace.SelectedIndex;
            cbColor.Color = _faceColors[iSel];
            graphCtrl.Invalidate();
        }
        private void OnFaceColorChanged(object sender, EventArgs e)
        {
            if (!chkAllFaces.Checked)
            {
                int iSel = cbFace.SelectedIndex;
                if (iSel >=0 && iSel < 6)
                    _faceColors[iSel] = cbColor.Color;
            }
            else
            {
                for (int i = 0; i < 6; ++i)
                    _faceColors[i] = cbColor.Color;
            }
            graphCtrl.Invalidate();
        }

        public override void UpdateStatus(string message)
        {
            // case length consistency
            if (HasInsideDimensions && InsideLength > BoxLength)
                message = string.Format(Resources.ID_INVALIDINSIDELENGTH, InsideLength, BoxLength);
            // case width consistency
            else if (HasInsideDimensions && InsideWidth > BoxWidth)
                message = string.Format(Resources.ID_INVALIDINSIDEWIDTH, InsideWidth, BoxWidth);
            // case height consistency
            else if (HasInsideDimensions && InsideHeight > BoxHeight)
                message = string.Format(Resources.ID_INVALIDINSIDEHEIGHT, InsideHeight, BoxHeight);
            // box/case net weight consistency
            else if (NetWeight.Activated && NetWeight > Weight)
                message = string.Format(Resources.ID_INVALIDNETWEIGHT, NetWeight.Value, Weight);
            base.UpdateStatus(message);
        }

        private void OnAllFacesColorCheckedChanged(object sender, EventArgs e)
        {
            lbFace.Enabled = !chkAllFaces.Checked;
            cbFace.Enabled = !chkAllFaces.Checked;
            if (chkAllFaces.Checked)
                cbColor.Color = _faceColors[0];
        }
        private void OnEditTextures(object sender, EventArgs e)
        {
            try
            {
                FormEditBitmaps form = new FormEditBitmaps(BoxLength, BoxWidth, BoxHeight, _faceColors, _textures)
                {
                    TapeWidth = TapeWidth,
                    TapeColor = TapeColor,
                };
                if (DialogResult.OK == form.ShowDialog())
                    _textures = form.Textures;
                graphCtrl.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void OnTapeWidthChecked(object sender, EventArgs e)
        {
            bool isActivated = uCtrlTapeWidth.Value.Activated;
            lbTapeColor.Enabled = isActivated;
            cbTapeColor.Enabled = isActivated;
            graphCtrl.Invalidate();
        }
        private void OnFacingChanged(object sender, EventArgs e)
        {
            graphCtrl.Invalidate();
        }
        #endregion

        #region Helpers
        private void UpdateThicknesses()
        {
            _thicknessLength = BoxLength - InsideLength;
            _thicknessWidth = BoxWidth - InsideWidth;
            _thicknessHeight = BoxHeight - InsideHeight;
        }
        #endregion

        #region Net weight
        public OptDouble NetWeight
        {
            get
            {
                if (HasInsideDimensions)
                    return new OptDouble(false, 0.0);
                else
                    return uCtrlNetWeight.Value; 
            }
            set { uCtrlNetWeight.Value = value; }
        }
        #endregion

        #region Max weight
        public OptDouble MaxWeight
        {
            get
            {
                if (!HasInsideDimensions)
                    return new OptDouble(false, 0.0);
                else
                    return uCtrlMaxWeight.Value;
            }
            set { uCtrlMaxWeight.Value = value; }
        }
        #endregion

        #region Draw box
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            var boxProperties = new BoxProperties(
                null, uCtrlDimensionsOuter.ValueX, uCtrlDimensionsOuter.ValueY, uCtrlDimensionsOuter.ValueZ)
            {
                TextureList = _textures,
                TapeWidth = TapeWidth,
                TapeColor = TapeColor,
                StrapperSet = StrapperSet,
                Facing = Facing
            };
            boxProperties.SetAllColors(_faceColors);
            graphics.AddBox(new Box(0, boxProperties));
            graphics.AddDimensions(
                new DimensionCube(uCtrlDimensionsOuter.ValueX, uCtrlDimensionsOuter.ValueY, uCtrlDimensionsOuter.ValueZ)
                );
        }
        #endregion

        #region Save to database
        private void OnSaveToDatabase(object sender, EventArgs e)
        {
            try
            {
                FormSetItemName form = new FormSetItemName() { ItemName = BoxName };
                if (DialogResult.OK == form.ShowDialog())
                {
                    using (WCFClient wcfClient = new WCFClient())
                    {
                        int[] colors = (from fc in _faceColors select fc.ToArgb()).ToArray();

                        var client = wcfClient.Client;
                        client?.CreateNewCase(
                            new DCSBCase()
                            {
                                Name = form.ItemName,
                                Description = Description,
                                UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                                DimensionsOuter = new DCSBDim3D() { M0 = uCtrlDimensionsOuter.ValueX, M1 = uCtrlDimensionsOuter.ValueY, M2 = uCtrlDimensionsOuter.ValueZ },
                                IsCase = HasInsideDimensions,
                                HasInnerDims = HasInsideDimensions,
                                DimensionsInner = new DCSBDim3D() { M0 = uCtrlDimensionsInner.X, M1 = uCtrlDimensionsInner.Y, M2 = uCtrlDimensionsInner.Z },
                                ShowTape = TapeWidth.Activated,
                                TapeWidth = TapeWidth.Value,
                                TapeColor = TapeColor.ToArgb(),
                                Weight = Weight,
                                NetWeight = !HasInsideDimensions && NetWeight.Activated ? NetWeight.Value : new double?(),
                                MaxWeight = HasInsideDimensions && MaxWeight.Activated ? MaxWeight.Value : new double?(),
                                Colors = colors,
                                AutoInsert = false
                            }
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region Data members
        public Color[] _faceColors = new Color[6];
        public List<Pair<HalfAxis.HAxis, Texture>> _textures;
        public StrapperSet StrapperSet { get; } = new StrapperSet();
        private double _thicknessLength = 0.0, _thicknessWidth = 0.0, _thicknessHeight = 0.0;
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewBox));
        #endregion
    }
}