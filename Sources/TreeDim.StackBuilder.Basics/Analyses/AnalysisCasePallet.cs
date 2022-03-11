#region Using directives
using System.Collections.Generic;
using System.Linq;
using Sharp3D.Math.Core;
using System.Drawing;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    public class AnalysisCasePallet : AnalysisPackablePallet
    {
        #region Constructor
        public AnalysisCasePallet(
            Packable packable,
            PalletProperties palletProperties,
            ConstraintSetCasePallet constraintSet,
            bool temporary = false)
            : base(packable, palletProperties, constraintSet, temporary)
        {
        }
        #endregion

        #region Specific case/pallet decoration 
        public StrapperSet StrapperSet
        {
            get => _strapperSet;
            set
            {
                _strapperSet = value;
                SolutionLay.ClearStrapperSets();
            }
        }
        public PalletCornerProperties PalletCornerProperties
        {
            get { return _palletCornerProperties; }
            set
            {
                if (_palletCornerProperties == value) return;
                _palletCornerProperties?.RemoveDependancy(this);
                _palletCornerProperties = value;
                if (!Temporary && null != ParentDocument)
                    _palletCornerProperties?.AddDependancy(this);
            }
        }
        public PalletCornerProperties PalletCornerTopProperties
        {
            get => _palletCornerTopProperties;
            set
            {
                if (_palletCornerTopProperties == value) return;
                _palletCornerTopProperties?.RemoveDependancy(this);
                _palletCornerTopProperties = value;
                if (!Temporary && null != ParentDocument)
                    _palletCornerTopProperties?.AddDependancy(this);
            }
        }
        public InterlayerProperties TopInterlayerProperties
        {
            get => _topInterlayerProperties;
            set
            {
                if (_topInterlayerProperties == value) return;
                _topInterlayerProperties?.RemoveDependancy(this);
                _topInterlayerProperties = value;
                if (!Temporary && null != ParentDocument)
                    _topInterlayerProperties?.AddDependancy(this);
            }
        }
        public PalletCapProperties PalletCapProperties
        {
            get { return _palletCapProperties; }
            set
            {
                if (_palletCapProperties == value) return;
                _palletCapProperties?.RemoveDependancy(this);
                _palletCapProperties = value;
                if (!Temporary && null != ParentDocument)
                    _palletCapProperties?.AddDependancy(this);
            }
        }
        public PalletFilmProperties PalletFilmProperties
        {
            get { return _palletFilmProperties; }
            set
            {
                if (_palletFilmProperties == value) return;
                _palletFilmProperties?.RemoveDependancy(this);
                _palletFilmProperties = value;
                if (!Temporary && null != ParentDocument)
                    _palletFilmProperties?.AddDependancy(this);
            }
        }
        public bool PalletCornersTopX { get; set; }
        public bool PalletCornersTopY { get; set; }
        public double PalletFilmTopCovering { get; set; }
        public bool HasPalletCorners => null != _palletCornerProperties;
        public bool HasPalletCornersTopX => null != _palletCornerTopProperties && PalletCornersTopX;
        public bool HasPalletCornersTopY => null != _palletCornerTopProperties && PalletCornersTopY;
        public bool HasTopInterlayer => null != _topInterlayerProperties;
        public bool HasPalletCap => null != _palletCapProperties;
        public bool HasPalletFilm => null != _palletFilmProperties;
        public bool HasStrappers => null != StrapperSet;
        public bool HasPalletSleeve { get; set; }
        public Color PalletSleeveColor { get; set; }
        #endregion

        #region Override AnalysisHomo
        public override BBox3D BBoxLoadWDeco(BBox3D loadBBox)
        {
            var bbox = new BBox3D(loadBBox);
            // --- extend for pallet corners: begin
            double thickness = System.Math.Max(
                (HasPalletCorners ? PalletCornerProperties.Thickness : 0.0),
                (HasPalletCornersTopX||HasPalletCornersTopY ? PalletCornerTopProperties.Thickness : 0.0)
                );
            if (HasPalletCorners || HasPalletCornersTopX || HasPalletCornersTopY)
            {
                Vector3D ptMin = bbox.PtMin;
                ptMin.X -= thickness;
                ptMin.Y -= thickness;
                Vector3D ptMax = bbox.PtMax;
                ptMax.X += thickness;
                ptMax.Y += thickness;
                bbox.Extend(ptMin);
                bbox.Extend(ptMax);
            }
            if (HasPalletCornersTopX || HasPalletCornersTopY)
            {
                double thicknessTop = PalletCornerTopProperties.Thickness;
                Vector3D ptMax = bbox.PtMax;
                ptMax.Z += thicknessTop;
                bbox.Extend(ptMax);
            }
            // --- extend for pallet corners: end
            // --- extend for pallet cap : begin
            if (HasPalletCap)
            {
                double zMax = bbox.PtMax.Z;
                Vector3D v0 = new Vector3D(
                        0.5 * (PalletProperties.Length - PalletCapProperties.Length),
                        0.5 * (PalletProperties.Width - PalletCapProperties.Width),
                        zMax + PalletCapProperties.Height - PalletCapProperties.InsideHeight);
                bbox.Extend(v0);
                Vector3D v1 = new Vector3D(
                    0.5 * (PalletProperties.Length + PalletCapProperties.Length),
                    0.5 * (PalletProperties.Width + PalletCapProperties.Width),
                    zMax + PalletCapProperties.Height - PalletCapProperties.InsideHeight);
                bbox.Extend(v1);
            }
            // --- extend for pallet cap : end 
            return bbox;
        }
        public override double DecorationWeight
        {
            get
            {
                double labelWeight = 0.0;
                foreach (var labelInst in PalletLabels)
                {
                    labelWeight += labelInst.PalletLabelProperties.Weight;
                }

                return (HasPalletCap ? PalletCapProperties.Weight : 0.0)
                    + (HasPalletCorners ? 4 * PalletCornerProperties.Weight : 0.0)
                    + (HasPalletCornersTopX ? 2 * PalletCornerTopProperties.Weight : 0.0)
                    + (HasPalletCornersTopY ? 2 * PalletCornerTopProperties.Weight : 0.0)
                    + (HasPalletFilm ? PalletFilmProperties.Weight : 0.0)
                    + labelWeight;
            }
        }
        #endregion

        #region Robot related members
        public List<ConveyorSetting> ConveyorSettings = new List<ConveyorSetting>()
        {
            new ConveyorSetting(0, 1, 0),
            new ConveyorSetting(0, 2, 0) 
        };
        public ConveyorSetting DefaultConveyorSetting
        {
            get
            {
                if (!ConveyorSettings.Where(cs => cs.Number == 1).Any())
                    ConveyorSettings.Add(new ConveyorSetting(0, 1, 0));
                return ConveyorSettings.Where(cs => cs.Number == 1).First();
            }
        }
        public RobotPreparation RobotPreparation
        {
            get
            {
                if (null == _robotPreparation)
                    _robotPreparation = new RobotPreparation(this);
                return _robotPreparation;
            }
            set
            {
                _robotPreparation = value; 
            }
        }
        #endregion

        #region Non-Public Members
        private PalletCornerProperties _palletCornerProperties;
        private PalletCornerProperties _palletCornerTopProperties;
        private InterlayerProperties _topInterlayerProperties;
        private PalletCapProperties _palletCapProperties;
        private PalletFilmProperties _palletFilmProperties;
        private StrapperSet _strapperSet = new StrapperSet();
        private RobotPreparation _robotPreparation;
        #endregion
    }
}
