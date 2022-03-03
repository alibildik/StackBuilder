#region Using directives
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.ComponentModel;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.Basics;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    /// <summary>
    /// Classes that encapsulates data
    /// The application is MDI and might host several Document instance
    /// </summary>
    public class Document
    {
        #region Constructor
        public Document(string filePath, IDocumentListener listener)
        {
            // set name from file path
            Name = Path.GetFileNameWithoutExtension(filePath);
            if (null != listener)
            {
                // add listener
                AddListener(listener);
                // notify listener of document creation
                listener.OnNewDocument(this);
            }
            // load file
            Load(filePath);            
            // rechange name to match filePath
            Name = Path.GetFileNameWithoutExtension(filePath);
        }

        public Document(string name, string description, string author, DateTime dateCreated, IDocumentListener listener)
        {
            Name = name;
            Description = description;
            Author = author;
            DateOfCreation = dateCreated;
            if (null != listener)
            {
                // add listener
                AddListener(listener);
                // notify listener of document creation
                listener.OnNewDocument(this);
            }
        }
        #endregion

        #region Name checking / Getting new name
        public bool IsValidNewTypeName(string name, ItemBase itemToName)
        {
            // make sure is not empty
            if (name.Trim() == string.Empty)
                return false;
            // make sure it is not already used
            return null == _typeList.Find(
                delegate(ItemBase item)
                {   return (item != itemToName)
                    && string.Equals(item.ID.Name.Trim(), name.Trim(), StringComparison.CurrentCultureIgnoreCase); }
                );
        }
        public string GetValidNewTypeName(string prefix)
        {
            int index = 0;
            string name = string.Empty;
            while (!IsValidNewTypeName(name = string.Format("{0}{1}", prefix, index), null))
                ++index;
            while (!IsValidNewAnalysisName(name = string.Format("{0}{1}", prefix, index), null))
                ++index;
            return name;
        }
        public bool IsValidNewAnalysisName(string name, ItemBase analysisToRename)
        {
            string trimmedName = name.Trim();
            return (null == Analyses.Find(
                delegate (Analysis analysis)
                {
                    return analysis != analysisToRename
                        && string.Equals(analysis.ID.Name, trimmedName, StringComparison.InvariantCultureIgnoreCase);
                }
                ));
        }
        public string GetValidNewAnalysisName(string prefix)
        {
            int index = 0;
            string name = string.Empty;
            while (!IsValidNewAnalysisName(name = string.Format("{0}{1}", prefix, index), null))
                ++index;
            return name;
        }
        #endregion

        #region Public instantiation methods
        public Packable CreateNewPackable(Packable packable)
        {
            if (packable is BoxProperties boxProperties)
                return CreateNewCase(boxProperties);
            if (packable is BundleProperties bundleProperties)
                return CreateNewBundle(bundleProperties);
            if (packable is PackProperties packProperties)
                return CreateNewPack(packProperties);
            return null;
        }
        /// <summary>
        /// Create a new box
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="length">Length</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="weight">Weight</param>
        /// <param name="colors">Name</param>
        /// <returns>created BoxProperties instance</returns>
        public BoxProperties CreateNewBox(
            string name, string description
            , double length, double width, double height
            , double weight
            , Color[] colors)
        {
            // instantiate and initialize
            var boxProperties = new BoxProperties(this, length, width, height) { CAType = BoxProperties.CreatedAsType.Box };
            boxProperties.SetWeight( weight );
            boxProperties.ID.SetNameDesc( name, description);
            boxProperties.SetAllColors(colors);
            // insert in list
            _typeList.Add(boxProperties);
            // notify listeners
            NotifyOnNewTypeCreated(boxProperties);
            Modify();
            return boxProperties;
        }
        public BoxProperties CreateNewBox(BoxProperties boxProp)
        {
            // instantiate and initialize
            var boxPropClone = new BoxProperties(this, boxProp.Length, boxProp.Width, boxProp.Height) { CAType = boxProp.CAType };
            boxPropClone.SetWeight( boxProp.Weight );
            boxPropClone.SetNetWeight( boxProp.NetWeight );
            boxPropClone.ID.SetNameDesc( boxProp.ID.Name, boxProp.ID.Description );
            boxPropClone.SetAllColors(boxProp.Colors);
            // insert in list
            _typeList.Add(boxPropClone);
            // notify listeners
            NotifyOnNewTypeCreated(boxPropClone);
            Modify();

            return boxPropClone;
        }
        /// <summary>
        /// Create a new case
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="insideLength"></param>
        /// <param name="insideWidth"></param>
        /// <param name="insideHeight"></param>
        /// <param name="weight"></param>
        /// <param name="colors"></param>
        /// <returns></returns>
        public BoxProperties CreateNewCase(
            string name, string description
            , double length, double width, double height
            , double insideLength, double insideWidth, double insideHeight, bool hasInsideDimensions
            , double weight
            , Color[] colors)
        {
            // instantiate and initialize
            BoxProperties boxProperties = new BoxProperties(this, length, width, height, insideLength, insideWidth, insideHeight);
            boxProperties.HasInsideDimensions = hasInsideDimensions;
            boxProperties.SetWeight( weight );
            boxProperties.ID.SetNameDesc( name, description);
            boxProperties.SetAllColors(colors);
            // insert in list
            _typeList.Add(boxProperties);
            // notify listeners
            NotifyOnNewTypeCreated(boxProperties);
            Modify();
            return boxProperties;
        }
        public BoxProperties CreateNewCase(
            string name, string description
            , double length, double width, double height
            , double weight
            , Color[] colors)
        {
            // instantiate and initialize
            BoxProperties boxProperties = new BoxProperties(this, length, width, height);
            boxProperties.SetWeight(weight);
            boxProperties.ID.SetNameDesc(name, description);
            boxProperties.SetAllColors(colors);
            // insert in list
            _typeList.Add(boxProperties);
            // notify listeners
            NotifyOnNewTypeCreated(boxProperties);
            Modify();
            return boxProperties;
        }
        public BoxProperties CreateNewCase(BoxProperties boxProp)
        {
            // instantiate and initialize
            var boxPropClone = new BoxProperties(this
                , boxProp.Length, boxProp.Width, boxProp.Height
                , boxProp.InsideLength, boxProp.InsideWidth, boxProp.InsideHeight)  { CAType = BoxProperties.CreatedAsType.Case };
            boxPropClone.SetWeight( boxProp.Weight );
            boxPropClone.SetNetWeight( boxProp.NetWeight );
            boxPropClone.ID.SetNameDesc( boxProp.ID.Name, boxProp.ID.Description );
            boxPropClone.SetAllColors(boxProp.Colors);
            boxPropClone.TapeWidth = boxProp.TapeWidth;
            boxPropClone.TapeColor = boxProp.TapeColor;
            // insert in list
            _typeList.Add(boxPropClone);
            // notify listeners
            NotifyOnNewTypeCreated(boxPropClone);
            Modify();
            return boxPropClone;
        }
        public BagProperties CreateNewBag(
            string name, string description
            , Vector3D outerDimensions, double roundingRadius
            , double weight, OptDouble netWeight
            , Color color
            )
        {
            var bagProperties = new BagProperties(this
                , name, description
                , outerDimensions, roundingRadius);
            bagProperties.SetWeight(weight);
            bagProperties.SetNetWeight(netWeight);
            bagProperties.SetColor(color);
            // insert in list
            _typeList.Add(bagProperties);
            // Notify listeners
            NotifyOnNewTypeCreated(bagProperties);
            Modify();
            return bagProperties;
        }
        public BagProperties CreateNewBag(BagProperties bagProperties)
        {
            var bagClone = new BagProperties(this, bagProperties.Name, bagProperties.Description
                , bagProperties.OuterDimensions, bagProperties.Radius);
            bagClone.SetWeight(bagProperties.Weight);
            bagClone.SetNetWeight(bagProperties.NetWeight);
            bagClone.SetColor(bagProperties.ColorFill);
            _typeList.Add(bagClone);
            // Notify listeners
            NotifyOnNewTypeCreated(bagClone);
            Modify();
            return bagClone;
        }
        /// <summary>
        /// Create a new pack
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="packable">Inner content (either box or revolution solid)</param>
        /// <param name="arrangement">Arrangement</param>
        /// <param name="axis">Axis</param>
        /// <param name="wrapper">Wrapper</param>
        /// <returns></returns>
        public PackProperties CreateNewPack(
            string name, string description
            , Packable packable
            , PackArrangement arrangement
            , HalfAxis.HAxis axis, PackProperties.EnuRevSolidLayout revSolidLayout
            , PackWrapper wrapper
            , PackTray tray)
        { 
            // instantiate and initialize
            PackProperties packProperties = new PackProperties(this
                , packable
                , arrangement
                , axis, revSolidLayout
                , wrapper
                , tray);
            packProperties.ID.SetNameDesc( name, description );
            // insert in list
            _typeList.Add(packProperties);
            // notify listeners
            NotifyOnNewTypeCreated(packProperties);
            Modify();
            return packProperties;
        }
        public PackProperties CreateNewPack(PackProperties pack)
        {
            PackProperties packProperties = new PackProperties(this, pack.Content, pack.Arrangement, pack.BoxOrientation, pack.RevSolidLayout, pack.Wrap, pack.Tray);
            packProperties.ID.SetNameDesc(pack.ID.Name, pack.ID.Description);
            // insert in list
            _typeList.Add(packProperties);
            // notify listeners
            NotifyOnNewTypeCreated(packProperties);
            return packProperties;
        }
        public BundleProperties CreateNewBundle(
            string name, string description
            , double length, double width, double thickness
            , double weight
            , Color color
            , int noFlats)
        {
            // instantiate and initialize
            BundleProperties bundle = new BundleProperties(this, name, description, length, width, thickness, weight, noFlats, color);
            // insert in list
            _typeList.Add(bundle);
            // notify listeners
            NotifyOnNewTypeCreated(bundle);
            Modify();
            return bundle;
        }
        public BundleProperties CreateNewBundle(BundleProperties bundleProp)
        { 
            // instantiate
            BundleProperties bundlePropClone = new BundleProperties(this,
                bundleProp.Name, bundleProp.Description,
                bundleProp.Length, bundleProp.Width, bundleProp.UnitThickness, bundleProp.UnitWeight,
                bundleProp.NoFlats, bundleProp.Color);
            _typeList.Add(bundlePropClone);
            NotifyOnNewTypeCreated(bundlePropClone);
            Modify();
            return bundlePropClone;
        }
        public CylinderProperties CreateNewCylinder(CylinderProperties cyl)
        {
            // cylinder
            CylinderProperties cylinder = new CylinderProperties(this
                , cyl.ID.Name, cyl.ID.Description
                , cyl.RadiusOuter, cyl.RadiusInner, cyl.Height
                , cyl.Weight
                , cyl.ColorTop, cyl.ColorWallOuter, cyl.ColorWallInner);
            // insert in list
            _typeList.Add(cylinder);
            // notify listeners
            NotifyOnNewTypeCreated(cylinder);
            Modify();
            return cylinder;
        }
        public CylinderProperties CreateNewCylinder(
            string name, string description
            , double radiusOuter, double radiusInner, double height
            , double weight, OptDouble netWeight
            , Color colorTop, Color colorWallOuter, Color colorWallInner)
        {
            var cylinder = new CylinderProperties(this, name, description
                , radiusOuter, radiusInner, height, weight
                , colorTop, colorWallOuter, colorWallInner);
            cylinder.SetNetWeight(netWeight);
            // insert in list
            _typeList.Add(cylinder);
            // notify listeners
            NotifyOnNewTypeCreated(cylinder);
            Modify();
            return cylinder;        
        }

        public BottleProperties CreateNewBottle(
            string name, string description
            , List<Vector2D> profile
            , double weight, OptDouble netWeight
            , Color color)
        {
            var bottle = new BottleProperties(this, name, description
                , profile
                , weight
                , color);
            bottle.SetNetWeight(netWeight);
            // insert in list
            _typeList.Add(bottle);
            // notify listeners
            NotifyOnNewTypeCreated(bottle);
            Modify();
            return bottle;
        }

        public void AddType(ItemBase item)
        {
            // insert in list
            _typeList.Add(item);
            // notify listeners
            NotifyOnNewTypeCreated(item);
            Modify();
        }
        public InterlayerProperties CreateNewInterlayer(
            string name, string description
            , double length, double width, double thickness
            , double weight
            , Color color)
        { 
            // instantiate and intialize
            InterlayerProperties interlayer = new InterlayerProperties(
                this, name, description
                , length, width, thickness
                , weight, color);
            // insert in list
            _typeList.Add(interlayer);
            // notify listeners
            NotifyOnNewTypeCreated(interlayer);
            Modify();
            return interlayer;
        }
        public PalletCornerProperties CreateNewPalletCorners(string name, string description,
            double length, double width, double thickness,
            double weight,
            Color color)
        {
            // instantiate and initialize
            PalletCornerProperties palletCorners = new PalletCornerProperties(
                this,
                name, description,
                length, width, thickness,
                weight,
                color);
            // insert in list
            _typeList.Add(palletCorners);
            // notify listeners
            NotifyOnNewTypeCreated(palletCorners);
            Modify();
            return palletCorners;
        }
        public PalletCapProperties CreateNewPalletCap(PalletCapProperties palletCap)
        {
            // instantiate and initialize
            PalletCapProperties palletCapClone = new PalletCapProperties(
                this,
                palletCap.ID.Name, palletCap.ID.Description,
                palletCap.Length, palletCap.Width, palletCap.Height,
                palletCap.InsideLength, palletCap.InsideWidth, palletCap.InsideHeight,
                palletCap.Weight, palletCap.Color);
            // insert in list
            _typeList.Add(palletCapClone);
            // notify listeners
            NotifyOnNewTypeCreated(palletCapClone);
            Modify();
            return palletCapClone;
        }

        public PalletCapProperties CreateNewPalletCap(
            string name, string description,
            double length, double width, double height,
            double innerLength, double innerWidth, double innerHeight,
            double weight,
            Color color)
        {
            // instantiate and initialize
            PalletCapProperties palletCap = new PalletCapProperties(
                this,
                name, description,
                length, width, height,
                innerLength, innerWidth, innerHeight,
                weight, color);
            // insert in list
            _typeList.Add(palletCap);
            // notify listeners
            NotifyOnNewTypeCreated(palletCap);
            Modify();
            return palletCap;
        }

        public PalletFilmProperties CreateNewPalletFilm(PalletFilmProperties palletFilm)
        {
            // instantiate and initialize
            PalletFilmProperties palletFilmClone = new PalletFilmProperties(
                this,
                palletFilm.ID.Name, palletFilm.ID.Description,
                palletFilm.UseTransparency, palletFilm.UseHatching,
                palletFilm.HatchSpacing, palletFilm.HatchAngle,
                palletFilm.Weight,
                palletFilm.Color);
            // insert in list
            _typeList.Add(palletFilmClone);
            // notify listeners
            NotifyOnNewTypeCreated(palletFilmClone);
            Modify();
            return palletFilmClone; 
        }

        public PalletFilmProperties CreateNewPalletFilm(
            string name, string description,
            bool useTransparency,
            bool useHatching, double hatchSpacing, double hatchAngle,
            double weight,
            Color color)
        {
            // instantiate and initialize
            var palletFilm = new PalletFilmProperties(
                this,
                name, description,
                useTransparency,
                useHatching, hatchSpacing, hatchAngle,
                weight,
                color);
            // insert in list
            _typeList.Add(palletFilm);
            // notify listeners
            NotifyOnNewTypeCreated(palletFilm);
            Modify();
            return palletFilm;
        }
        public PalletLabelProperties CreateNewPalletLabel(
            string name, string description,
            Vector2D dimensions, 
            double weight,
            Color color,
            Bitmap bitmap)
        {
            // instantiate and initialize
            var palletLabel = new PalletLabelProperties(
                this,
                name, description,
                dimensions,
                weight,
                color, bitmap);
            // insert in list
            _typeList.Add(palletLabel);
            // notify
            NotifyOnNewTypeCreated(palletLabel);
            Modify();
            return palletLabel;
        }

        public InterlayerProperties CreateNewInterlayer(InterlayerProperties interlayerProp)
        {
            // instantiate and intialize
            InterlayerProperties interlayerClone = new InterlayerProperties(
                this, interlayerProp.ID.Name, interlayerProp.ID.Description
                , interlayerProp.Length, interlayerProp.Width, interlayerProp.Thickness
                , interlayerProp.Weight
                , interlayerProp.Color);
            // insert in list
            _typeList.Add(interlayerClone);
            // notify listeners
            NotifyOnNewTypeCreated(interlayerClone);
            Modify();
            return interlayerClone;       
        }
        public PalletProperties CreateNewPallet(
            string name, string description,
            string typeName,
            double length, double width, double height,
            double weight,
            Color palletColor)
        {
            PalletProperties palletProperties = new PalletProperties(this, typeName, length, width, height);
            palletProperties.ID.SetNameDesc( name, description );
            palletProperties.Weight = weight;
            palletProperties.Color = palletColor;
            // insert in list
            _typeList.Add(palletProperties);
            // notify listeners
            NotifyOnNewTypeCreated(palletProperties);
            Modify();
            return palletProperties;
        }

        public PalletProperties CreateNewPallet(PalletProperties palletProp)
        {
            PalletProperties palletPropClone = new PalletProperties(this, palletProp.TypeName, palletProp.Length, palletProp.Width, palletProp.Height);
            palletPropClone.ID.SetNameDesc( palletProp.ID.Name, palletProp.ID.Description);
            palletPropClone.Weight = palletProp.Weight;
            palletPropClone.Color = palletProp.Color;
            palletPropClone.AdmissibleLoadWeight = palletProp.AdmissibleLoadWeight;
            // insert in list
            _typeList.Add(palletPropClone);
            // notify listeners
            NotifyOnNewTypeCreated(palletPropClone);
            Modify();
            return palletPropClone;           
        }

        /// <summary>
        /// Creates a new truck in this document
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="length">Length</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="admissibleLoadWeight">AdmissibleLoadWeight</param>
        /// <param name="color">Color</param>
        /// <returns>TruckProperties</returns>
        public TruckProperties CreateNewTruck(
            string name, string description
            , double length
            , double width
            , double height
            , double admissibleLoadWeight
            , Color color)
        {
            TruckProperties truckProperties = new TruckProperties(this, length, width, height);
            truckProperties.ID.SetNameDesc( name, description);
            truckProperties.AdmissibleLoadWeight = admissibleLoadWeight;
            truckProperties.Color = color;
            // insert in list
            _typeList.Add(truckProperties);
            // notify listeners
            NotifyOnNewTypeCreated(truckProperties);
            Modify();
            return truckProperties;
        }
        #endregion

        #region Analyses instantiation method
        public Analysis CreateNewAnalysisCasePallet(
            string name, string description
            , Packable packable, PalletProperties pallet
            , List<InterlayerProperties> interlayers
            , PalletCornerProperties palletCorners, PalletCapProperties palletCap, PalletFilmProperties palletFilm
            , ConstraintSetCasePallet constraintSet
            , List<LayerEncap> layerEncaps
            , List<ConveyorSetting> conveyorSettings
            )
        {
            AnalysisCasePallet analysis = new AnalysisCasePallet(packable, pallet, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            if (null != interlayers)
            {
                foreach (InterlayerProperties interlayer in interlayers)
                    analysis.AddInterlayer(interlayer);
            }
            analysis.PalletCornerProperties     = palletCorners;
            analysis.PalletCapProperties        = palletCap;
            analysis.PalletFilmProperties       = palletFilm;
            analysis.ConveyorSettings           = conveyorSettings;

            analysis.AddSolution(layerEncaps);

            return InsertAnalysis(analysis);
        }

        public Analysis CreateNewAnalysisCasePallet(
            string name, string description
            , Packable packable, PalletProperties pallet
            , List<InterlayerProperties> interlayers
            , PalletCornerProperties palletCorners, PalletCapProperties palletCap, PalletFilmProperties palletFilm
            , ConstraintSetCasePallet constraintSet
            , List<KeyValuePair<LayerEncap, int>> listLayers
            , List<ConveyorSetting> conveyorSettings
            )
        {
            AnalysisCasePallet analysis = new AnalysisCasePallet(packable, pallet, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            if (null != interlayers)
            {
                foreach (InterlayerProperties interlayer in interlayers)
                    analysis.AddInterlayer(interlayer);
            }
            analysis.PalletCornerProperties = palletCorners;
            analysis.PalletCapProperties = palletCap;
            analysis.PalletFilmProperties = palletFilm;
            analysis.ConveyorSettings = conveyorSettings;
            analysis.AddSolution(listLayers);

            return InsertAnalysis(analysis);
        }

        public Analysis CreateNewAnalysisBoxCase(
            string name, string description
            , Packable packable, BoxProperties caseProperties
            , List<InterlayerProperties> interlayers
            , ConstraintSetBoxCase constraintSet
            , List<LayerEncap> layerDescs
            )
        {
            AnalysisBoxCase analysis = new AnalysisBoxCase(
                this, packable, caseProperties, constraintSet);
            analysis.ID.SetNameDesc( name, description );
            if (null != interlayers)
            {
                foreach (InterlayerProperties interlayer in interlayers)
                    analysis.AddInterlayer(interlayer);
            }
            analysis.AddSolution(layerDescs);
            return InsertAnalysis(analysis);
        }

        public Analysis CreateNewAnalysisBoxCase(
            string name, string description
            , Packable packable, BoxProperties caseProperties
            , List<InterlayerProperties> interlayers
            , ConstraintSetBoxCase constraintSet
            , List<KeyValuePair<LayerEncap, int>> listLayers
            )
        {
            AnalysisBoxCase analysis = new AnalysisBoxCase(
                this, packable, caseProperties, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            if (null != interlayers)
            {
                foreach (InterlayerProperties interlayer in interlayers)
                    analysis.AddInterlayer(interlayer);
            }
            analysis.AddSolution(listLayers);
            return InsertAnalysis(analysis);
        }

        public Analysis CreateNewAnalysisCaseTruck(
            string name, string description
            , Packable packable, TruckProperties truckProperties
            , List<InterlayerProperties> interlayers
            , ConstraintSetCaseTruck constraintSet
            , List<LayerEncap> layerEncaps)
        {
            AnalysisCaseTruck analysis = new AnalysisCaseTruck(
                this, packable, truckProperties, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            if (null != interlayers)
            {
                foreach (InterlayerProperties interlayer in interlayers)
                    analysis.AddInterlayer(interlayer);
            }
            analysis.AddSolution(layerEncaps);
            return InsertAnalysis(analysis);
        }
        public Analysis CreateNewAnalysisPalletTruck(
            string name, string description
            , Packable loadedPallet, TruckProperties truckProperties
            , ConstraintSetPalletTruck constraintSet
            , List<LayerEncap> layerEncaps)
        {
            AnalysisPalletTruck analysis = new AnalysisPalletTruck(
                loadedPallet, truckProperties, constraintSet);
            analysis.ID.SetNameDesc( name, description );
            analysis.AddSolution(layerEncaps);

            return InsertAnalysis(analysis);
        }
        public Analysis CreateNewAnalysisPalletsOnPallet(
            string name, string description,
            AnalysisPalletsOnPallet.EMasterPalletSplit masterPalletSplit, AnalysisPalletsOnPallet.ELoadedPalletOrientation loadedPalletOrientation,
            PalletProperties masterPallet,
            LoadedPallet loadedPallet0, LoadedPallet loadedPallet1,
            LoadedPallet loadedPallet2, LoadedPallet loadedPallet3)
        {
            AnalysisPalletsOnPallet analysis = new AnalysisPalletsOnPallet(this,
                masterPalletSplit, loadedPalletOrientation,
                masterPallet,
                loadedPallet0,
                loadedPallet1,
                loadedPallet2,
                loadedPallet3);
            analysis.ID.SetNameDesc(name, description);

            return InsertAnalysis(analysis);
        }

        public Analysis CreateNewAnalysisCylinderPallet(
            string name, string description
            , RevSolidProperties cylinder, PalletProperties palletProperties
            , List<InterlayerProperties> interlayers
            , ConstraintSetPackablePallet constraintSet
            , List<LayerEncap> layerDescs
            )
        { 
            // analysis
            AnalysisCylinderPallet analysis = new AnalysisCylinderPallet(
                cylinder, palletProperties, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            if (null != interlayers)
            {
                foreach (InterlayerProperties interlayer in interlayers)
                    analysis.AddInterlayer(interlayer);
            }
            analysis.AddSolution(layerDescs);
            return InsertAnalysis(analysis);
        }
        public Analysis CreateNewAnalysisHCylPallet(
            string name, string description
            , CylinderProperties cylinder, PalletProperties palletProperties
            , ConstraintSetPackablePallet constraintSet
            , HCylLayout layout
            )
        {
            // analysis
            var analysis = new AnalysisHCylPallet(
                this, cylinder, palletProperties, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            analysis.SetSolution(layout);
            return InsertAnalysis(analysis);
        }
        public Analysis CreateNewAnalysisCylinderCase(
            string name, string description
            , RevSolidProperties cylinder, BoxProperties caseProperties
            , List<InterlayerProperties> interlayers
            , ConstraintSetCylinderContainer constraintSet
            , List<LayerEncap> layerEncaps)
        {
            AnalysisCylinderCase analysis = new AnalysisCylinderCase(
                this, cylinder, caseProperties, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            if (null != interlayers)
            {
                foreach (InterlayerProperties interlayer in interlayers)
                    analysis.AddInterlayer(interlayer);
            }
            analysis.AddSolution(layerEncaps);
            return InsertAnalysis(analysis);
        }

        public Analysis CreateNewAnalysisCylinderTruck(
            string name, string description
            , CylinderProperties cylinder, TruckProperties truckProperties
            , ConstraintSetCylinderTruck constraintSet
            , List<LayerEncap> layerEncaps)
        {
            var analysis = new AnalysisCylinderTruck(
                this, cylinder, truckProperties, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            analysis.AddSolution(layerEncaps);
            return InsertAnalysis(analysis);
        }

        public Analysis CreateNewAnalysisHCylTruck(
            string name, string description
            , CylinderProperties cylinder, TruckProperties truckProperties
            , ConstraintSetCylinderTruck constraintSet
            , HCylLayout layout
            )
        {
            var analysis = new AnalysisHCylTruck(
                this, cylinder, truckProperties, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            analysis.SetSolution(layout);
            return InsertAnalysis(analysis);
        }

        public AnalysisHetero CreateNewHAnalysisCasePallet(
            string name, string description,
            List<ContentItem> contentItems,
            PalletProperties palletProperties,
            HConstraintSetPallet constraintSet,
            HSolution solution
            )
        {
            HAnalysisPallet analysis = new HAnalysisPallet(this)
            {
                Content = contentItems,
                Pallet = palletProperties,
                ConstraintSet = constraintSet,
                Solution = solution
            };
            analysis.ID.SetNameDesc(name, description);
            solution.Analysis = analysis;
            return InsertAnalysis(analysis);
        }

        public AnalysisHetero CreateNewHAnalysisCaseTruck(
            string name, string description,
            List<ContentItem> contentItems,
            TruckProperties truckProperties,
            HConstraintSetTruck constraintSet,
            HSolution solution)
        {
            HAnalysisTruck analysis = new HAnalysisTruck(this)
            {
                Content = contentItems,
                Truck = truckProperties,
                ConstraintSet = constraintSet,
                Solution = solution
            };
            analysis.ID.SetNameDesc(name, description);
            solution.Analysis = analysis;
            return InsertAnalysis(analysis);
        }

        private Analysis InsertAnalysis(Analysis analysis)
        {
            Analyses.Add(analysis);
            // notify listeners
            NotifyOnNewAnalysisCreated(analysis);
            // set document dirty
            Modify();
            return analysis;
        }
        private AnalysisHetero InsertAnalysis(AnalysisHetero analysis)
        {
            HAnalyses.Add(analysis);
            // notify listeners
            NotifyOnNewAnalysisCreated(analysis);
            // set document dirty
            Modify();
            return analysis;
        }
        public void UpdateAnalysis(Analysis analysis)
        {
            // notify listeners
            NotifyAnalysisUpdated(analysis);
            // set document dirty
            Modify();
        }
        public void UpdateAnalysis(AnalysisHetero analysis)
        {
            // notify listeners
            NotifyAnalysisUpdated(analysis);
            // set document dirty
            Modify();
        }
        #endregion

        #region Legacy analyses instantiation method
        public Analysis CreateNewPackPalletAnalysis(
            string name, string description
            , PackProperties pack, PalletProperties pallet
            , InterlayerProperties interlayer
            , PackPalletConstraintSet constraintSet
            , ILayerSolver solver)
        {
            ConstraintSetCasePallet constraintSetNew = new ConstraintSetCasePallet();
            constraintSetNew.SetMaxHeight(constraintSet.MaximumPalletHeight);
            constraintSetNew.OptMaxWeight = constraintSet.MaximumPalletWeight;
            constraintSetNew.Overhang = new Vector2D(constraintSet.OverhangX, constraintSet.OverhangY);
            constraintSetNew.SetAllowedOrientations(new bool[3] { false, false, true });

            List<InterlayerProperties> listInterlayers = new List<InterlayerProperties>();
            if (null != interlayer)
                listInterlayers.Add(interlayer);

            var layerEncaps = new List<LayerEncap>();
            if (null != solver)
            {
                List<Layer2DBrickImp> layers = solver.BuildLayers(
                    pack.OuterDimensions
                    , pack.Bulge
                    , new Vector2D(pallet.Length + constraintSetNew.Overhang.X, pallet.Width + constraintSetNew.Overhang.Y), pallet.Height
                    , constraintSetNew
                    , true);
                if (layers.Count > 0)
                    layerEncaps.Add(new LayerEncap(layers[0].LayerDescriptor));
            }

            Analysis analysis = CreateNewAnalysisCasePallet(
                name, description
                , pack, pallet
                , listInterlayers, null, null, null
                , constraintSetNew, layerEncaps
                , new List<ConveyorSetting>());

            Modify();
            return analysis;
        }
 
        public void RemoveItem(ItemBase item)
        {
            // sanity check
            if (null == item)
            {
                Debug.Assert(false);
                return;
            }
            // dispose item first as it may remove dependancies itself
            _log.Debug(string.Format("Disposing {0}...", item.ID.Name));
            item.Dispose();

            // notify listeners / remove
            if (item.GetType() == typeof(BoxProperties)
                || item.GetType() == typeof(BagProperties)
                || item.GetType() == typeof(BundleProperties)
                || item.GetType() == typeof(PackProperties)
                || item.GetType() == typeof(PalletProperties)
                || item.GetType() == typeof(InterlayerProperties)
                || item.GetType() == typeof(PalletCornerProperties)
                || item.GetType() == typeof(PalletCapProperties)
                || item.GetType() == typeof(PalletFilmProperties)
                || item.GetType() == typeof(PalletLabelProperties)
                || item.GetType() == typeof(TruckProperties)
                || item.GetType() == typeof(CylinderProperties)
                || item.GetType() == typeof(BottleProperties))
            {
                NotifyOnTypeRemoved(item);
                if (!_typeList.Remove(item))
                    _log.Warn($"Failed to properly remove item {item.ID.Name}");
            }
            else if (item is AnalysisHomo)
            {
                NotifyOnAnalysisRemoved(item as AnalysisHomo);
                if (!Analyses.Remove(item as Analysis))
                    _log.Warn($"Failed to properly remove analysis {item.ID.Name}");
            }
            else if (item is AnalysisHetero)
            {
                NotifyOnAnalysisRemoved(item);
                if (!HAnalyses.Remove(item as AnalysisHetero))
                    _log.Warn($"Failed to properly remove analysis {item.ID.Name}");
            }
            else if (item is AnalysisPalletsOnPallet)
            {
                NotifyOnAnalysisRemoved(item as Analysis);
                if (!Analyses.Remove(item as Analysis))
                    _log.Warn($"Failed to properly remove analysis {item.ID.Name}");
            }
            else
                _log.Error($"Removing document {item.ID.Name} of unknown type {item.GetType()}...");
            Modify();
        }
        #endregion

        #region Public properties
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime DateOfCreation { get; set; }

        public ReadOnlyCollection<ItemBase> TypeList =>new ReadOnlyCollection<ItemBase>(_typeList); 
        public IEnumerable<PackableBrickNamed> Bricks => _typeList.OfType<PackableBrickNamed>();
        public IEnumerable<BoxProperties> Boxes => _typeList.OfType<BoxProperties>();
        public IEnumerable<BagProperties> Bags => _typeList.OfType<BagProperties>();
        public IEnumerable<BoxProperties> CasesWInsideDims => _typeList.OfType<BoxProperties>().Where(b => b.HasInsideDimensions);
        public IEnumerable<BundleProperties> Bundles => _typeList.OfType<BundleProperties>();
        public IEnumerable<RevSolidProperties> Cylinders => _typeList.OfType<RevSolidProperties>();
        public IEnumerable<PalletProperties> Pallets => _typeList.OfType<PalletProperties>();
        public IEnumerable<InterlayerProperties> Interlayers => _typeList.OfType<InterlayerProperties>();
        public IEnumerable<TruckProperties> Trucks => _typeList.OfType<TruckProperties>();
        public IEnumerable<Analysis> LoadedPallets
        {
            get
            {
                var listAnalyses = new List<Analysis>();
                foreach (var analysis in Analyses)
                    if (analysis is AnalysisCasePallet || analysis is AnalysisCylinderPallet)
                        listAnalyses.Add(analysis);
                return listAnalyses;
            }
        }
        public IEnumerable<ItemBase> GetByType(Type t) => _typeList.Where(item => item.GetType() == t);
        public List<Analysis> Analyses { get; } = new List<Analysis>();
        public List<AnalysisHetero> HAnalyses { get; } = new List<AnalysisHetero>();

        #endregion

        #region Allowing analysis/opti
        public bool CanCreatePack => Boxes.Any() || Cylinders.Any();
        public bool CanCreateAnalysisCasePallet => Bricks.Any() && Pallets.Any();
        public bool CanCreateAnalysisBundlePallet => Bundles.Any() && Pallets.Any();
        public bool CanCreateAnalysisBoxCase => Boxes.Any() && CasesWInsideDims.Any();
        public bool CanCreateAnalysisBundleCase => Bundles.Any() && CasesWInsideDims.Any();
        public bool CanCreateAnalysisCylinderPallet => Cylinders.Any() && Pallets.Any();
        public bool CanCreateAnalysisCylinderCase => Cylinders.Any() && CasesWInsideDims.Any();
        public bool CanCreatePalletsOnPallet => Pallets.Any() && LoadedPallets.Any();
        public bool CanCreateAnalysisPalletTruck => Trucks.Any() && LoadedPallets.Any();
        public bool CanCreateAnalysisCaseTruck => Boxes.Any() && Trucks.Any();
        public bool CanCreateAnalysisCylinderTruck => Trucks.Any() && Cylinders.Any();
        public bool CanCreateOptiCasePallet => Boxes.Any() && Pallets.Any();
        public bool CanCreateOptiPack => (Boxes.Any() || Bags.Any()) && Pallets.Any();
        public bool CanCreateOptiMulticase => Bundles.Any() || Boxes.Any() && CasesWInsideDims.Any();
        #endregion

        #region Load methods
        public void Load(string filePath)
        {
            try
            {
                // instantiate XmlDocument
                XmlDocument xmlDoc = new XmlDocument();
                // load xml file in document and parse document
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    xmlDoc.Load(fileStream);
                    XmlElement xmlRootElement = xmlDoc.DocumentElement;
                    LoadDocumentElement(xmlRootElement);
                }
            }
            catch (FileNotFoundException ex)
            {
                _log.Error("Caught FileNotFoundException in Document.Load() -> rethrowing...");
                throw ex;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        void LoadDocumentElement(XmlElement docElement)
        {
            if (docElement.HasAttribute("Name"))
                Name = docElement.Attributes["Name"].Value;
            if (docElement.HasAttribute("Description"))
                Description = docElement.Attributes["Description"].Value;
            if (docElement.HasAttribute("Description"))
                Author = docElement.Attributes["Author"].Value;
            if (docElement.HasAttribute("DateCreated"))
            {
                try
                {
                    DateOfCreation = Convert.ToDateTime(docElement.Attributes["DateCreated"].Value, new CultureInfo("en-US"));
                }
                catch (Exception /*ex*/)
                {
                    DateOfCreation = DateTime.Now;
                    _log.Debug("Failed to load date of creation correctly: Loading file generated with former version?");
                }
            }
            if (docElement.HasAttribute("UnitSystem"))
                UnitSystem = (UnitsManager.UnitSystem)int.Parse(docElement.Attributes["UnitSystem"].Value);

            foreach (XmlNode docChildNode in docElement.ChildNodes)
            {
                // load item properties
                if (string.Equals(docChildNode.Name, "ItemProperties", StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (XmlNode itemPropertiesNode in docChildNode.ChildNodes)
                    {
                        try
                        {
                            if (string.Equals(itemPropertiesNode.Name, "BoxProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadBoxProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "BagProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadBagProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "CylinderProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadCylinderProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "BottleProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadBottleProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "PalletProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadPalletProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "InterlayerProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadInterlayerProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "PalletCornerProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadPalletCornerProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "PalletCapProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadPalletCapProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "PalletFilmProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadPalletFilmProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "PalletLabelProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadPalletLabelProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "BundleProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadBundleProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "TruckProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadTruckProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "PackProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadPackProperties(itemPropertiesNode as XmlElement);
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"Failed to load type: {ex.Message}");
                        }
                    }
                }
                // load analyses
                else if (string.Equals(docChildNode.Name, "Analyses", StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (XmlNode analysisNode in docChildNode.ChildNodes)
                    {
                        XmlElement analysisElt = analysisNode as XmlElement;
                        // StackBuilder 2.0 analyses
                        if (string.Equals(analysisNode.Name, "AnalysisPallet") && !analysisElt.HasAttribute("ContentId"))
                        {
                            try
                            {
                                LoadAnalysisPallet(analysisNode as XmlElement);
                            }
                            catch (Exception ex)
                            {
                                _log.Error($"Failed to load AnalysisPallet (StackBuilder 2): {ex.Message}");
                            }
                        }
                        else if (string.Equals(analysisNode.Name, "AnalysisBoxCase") && !analysisElt.HasAttribute("ContentId"))
                        {
                            try
                            {
                                LoadAnalysisBoxCase(analysisNode as XmlElement);
                            }
                            catch (Exception ex)
                            {
                                _log.Error($"Failed to load AnalysisBoxCase (StackBuilder 2): {ex.Message}");
                            }
                        }
                        else
                        { 
                            try
                            {
                                LoadAnalysis(analysisNode as XmlElement);
                            }
                            catch (Exception ex)
                            {
                                string analysisName = "Unknown";
                                if (null != analysisElt && analysisElt.HasAttribute("Name"))
                                    analysisName = analysisElt.GetAttribute("Name");
                                _log.Error($"Failed to load {analysisNode.Name} ({analysisName}) with error: {ex.Message}");
                            }                        
                        }

                    }
                }
                // load heterogeneous analyses
                else if (string.Equals(docChildNode.Name, "HAnalyses", StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (XmlNode analysisNode in docChildNode.ChildNodes)
                    {
                        try
                        {
                            LoadHAnalysis(analysisNode as XmlElement);
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"Failed to load analysis: {ex.Message}");
                        }
                    }
                }
            }
        }

        #region Load containers / basics element
        private void LoadBoxProperties(XmlElement eltBoxProperties)
        {
            string sid = eltBoxProperties.Attributes["Id"].Value;
            string sname = eltBoxProperties.Attributes["Name"].Value;
            string sdescription = eltBoxProperties.Attributes["Description"].Value;
            double length = UnitsManager.ConvertLengthFrom(Convert.ToDouble(eltBoxProperties.Attributes["Length"].Value, CultureInfo.InvariantCulture), UnitSystem);
            double width = UnitsManager.ConvertLengthFrom(Convert.ToDouble(eltBoxProperties.Attributes["Width"].Value, CultureInfo.InvariantCulture), UnitSystem);
            double height = UnitsManager.ConvertLengthFrom(Convert.ToDouble(eltBoxProperties.Attributes["Height"].Value, CultureInfo.InvariantCulture), UnitSystem);
            Vector3D cog = eltBoxProperties.HasAttribute("COG") ? UnitsManagerEx.ConvertLengthFrom(Vector3D.Parse(eltBoxProperties.Attributes["COG"].Value), UnitSystem) : new Vector3D(0.5 * length, 0.5 * width, 0.5 * height);
            string sInsideLength = string.Empty, sInsideWidth = string.Empty, sInsideHeight = string.Empty;
            bool hasInsideDimensions = false;
            if (eltBoxProperties.HasAttribute("InsideLength"))
            {
                hasInsideDimensions = true;
                sInsideLength = eltBoxProperties.Attributes["InsideLength"].Value;
                sInsideWidth = eltBoxProperties.Attributes["InsideWidth"].Value;
                sInsideHeight = eltBoxProperties.Attributes["InsideHeight"].Value;
            }
            Vector3D bulge = Vector3D.Zero;
            if (eltBoxProperties.HasAttribute("Bulge"))
                bulge = UnitsManagerEx.ConvertLengthFrom(Vector3D.Parse(eltBoxProperties.Attributes["Bulge"].Value), UnitSystem);
            string sweight = eltBoxProperties.Attributes["Weight"].Value;
            OptDouble optNetWeight = LoadOptDouble(eltBoxProperties, "NetWeight", UnitsManager.UnitType.UT_MASS);
            string sCAType = eltBoxProperties.HasAttribute("CAType") ? eltBoxProperties.Attributes["CAType"].Value : string.Empty;

            Color[] colors = new Color[6];
            List<Pair<HalfAxis.HAxis, Texture>> listTexture = new List<Pair<HalfAxis.HAxis,Texture>>();
            bool hasTape = false;
            double tapeWidth = 0.0;
            Color tapeColor = Color.Black;
            StrapperSet strapperSet = new StrapperSet();
            strapperSet.SetDimension(length, width, height);
            foreach (XmlNode node in eltBoxProperties.ChildNodes)
            {
                XmlElement childElt = node as XmlElement;
                if (string.Equals(node.Name, "FaceColors", StringComparison.CurrentCultureIgnoreCase))
                    LoadFaceColors(childElt, ref colors);
                else if (string.Equals(node.Name, "Textures", StringComparison.CurrentCultureIgnoreCase))
                    LoadTextureList(childElt, ref listTexture);
                else if (string.Equals(node.Name, "Tape", StringComparison.CurrentCultureIgnoreCase))
                    hasTape = LoadTape(childElt, out tapeWidth, out tapeColor);
                else if (string.Equals(node.Name, "StrapperSet", StringComparison.CurrentCultureIgnoreCase))
                    LoadStrapperSet(childElt, ref strapperSet);
            }
            int facing = 0;
            if (eltBoxProperties.HasAttribute("Facing"))
            {
                facing = int.Parse(eltBoxProperties.Attributes["Facing"].Value);
                if (facing < 0) facing = 0;
            }
            bool isCase = hasInsideDimensions || hasTape;
            if (!string.IsNullOrEmpty(sCAType))
                isCase = string.Equals(sCAType, "Case", StringComparison.CurrentCultureIgnoreCase);

            // create new BoxProperties instance
            BoxProperties boxProperties;
            if (isCase)
            {
                double insideLength = 0.0, insideWidth = 0.0, insideHeight = 0.0;
                if (!string.IsNullOrEmpty(sInsideLength)) insideLength = Convert.ToDouble(sInsideLength, CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(sInsideWidth)) insideWidth = Convert.ToDouble(sInsideWidth, CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(sInsideHeight)) insideHeight = Convert.ToDouble(sInsideHeight, CultureInfo.InvariantCulture);
                boxProperties = CreateNewCase(
                    sname
                    , sdescription
                    , length, width, height
                    , UnitsManager.ConvertLengthFrom(insideLength, UnitSystem)
                    , UnitsManager.ConvertLengthFrom(insideWidth, UnitSystem)
                    , UnitsManager.ConvertLengthFrom(insideHeight, UnitSystem)
                    , true
                    , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), UnitSystem)
                    , colors);
            }
            else
                boxProperties = CreateNewBox(
                    sname
                    , sdescription
                    , length, width, height
                    , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), UnitSystem)
                    , colors);
            // guid
            boxProperties.ID.IGuid = new Guid(sid);
            // textures
            boxProperties.TextureList = listTexture;
            // tape
            boxProperties.TapeColor = tapeColor;
            boxProperties.TapeWidth = new OptDouble(hasTape, UnitsManager.ConvertLengthFrom(tapeWidth, UnitSystem));
            // net weight
            boxProperties.SetNetWeight( optNetWeight );
            boxProperties.StrapperSet = strapperSet;
            // inside dimensions
            boxProperties.HasInsideDimensions = hasInsideDimensions;
            // bulge
            boxProperties.Bulge = bulge;
            // facing
            boxProperties.Facing = facing;
            // cog
            boxProperties.SetCOG(cog);
        }

        private void LoadBagProperties(XmlElement eltBagProperties)
        {
            string sid = eltBagProperties.Attributes["Id"].Value;
            string sname = eltBagProperties.Attributes["Name"].Value;
            string sdescription = eltBagProperties.Attributes["Description"].Value;
            double length = UnitsManager.ConvertLengthFrom(Convert.ToDouble(eltBagProperties.Attributes["Length"].Value, CultureInfo.InvariantCulture), UnitSystem);
            double width = UnitsManager.ConvertLengthFrom(Convert.ToDouble(eltBagProperties.Attributes["Width"].Value, CultureInfo.InvariantCulture), UnitSystem);
            double height = UnitsManager.ConvertLengthFrom(Convert.ToDouble(eltBagProperties.Attributes["Height"].Value, CultureInfo.InvariantCulture), UnitSystem);
            double radius = UnitsManager.ConvertLengthFrom(Convert.ToDouble(eltBagProperties.Attributes["Radius"].Value, CultureInfo.InvariantCulture), UnitSystem);
            double weight = UnitsManager.ConvertMassFrom(Convert.ToDouble(eltBagProperties.Attributes["Weight"].Value, CultureInfo.InvariantCulture), UnitSystem);
            OptDouble optNetWeight = LoadOptDouble(eltBagProperties, "NetWeight", UnitsManager.UnitType.UT_MASS);
            string sColor = eltBagProperties.Attributes["Color"].Value;
            Color color = Color.FromArgb(Convert.ToInt32(sColor));
            Vector3D bulge = Vector3D.Zero;
            if (eltBagProperties.HasAttribute("Bulge"))
                bulge = UnitsManagerEx.ConvertLengthFrom(Vector3D.Parse(eltBagProperties.Attributes["Bulge"].Value), UnitSystem);

            var bagProperties = CreateNewBag(sname, sdescription, new Vector3D(length, width, height), radius, weight, optNetWeight, color);
            bagProperties.Bulge = bulge;
            bagProperties.ID.IGuid = new Guid(sid);
        }

        private void LoadPackProperties(XmlElement eltPackProperties)
        {
            string sid = eltPackProperties.Attributes["Id"].Value;
            string sname = eltPackProperties.Attributes["Name"].Value;
            string sdescription = eltPackProperties.Attributes["Description"].Value;
            string sBoxId = eltPackProperties.Attributes["BoxProperties"].Value;
            string sOrientation = eltPackProperties.Attributes["Orientation"].Value;
            string sArrangement = eltPackProperties.Attributes["Arrangement"].Value;
            PackProperties.EnuRevSolidLayout revSolidLayout = PackProperties.EnuRevSolidLayout.ALIGNED;
            if (eltPackProperties.HasAttribute("RevSolidLayout"))
                revSolidLayout = (PackProperties.EnuRevSolidLayout)int.Parse(eltPackProperties.Attributes["RevSolidLayout"].Value);
            PackWrapper wrapper = null;
            PackTray tray = null;
            StrapperSet strapperSet = new StrapperSet();
            foreach (XmlElement node in eltPackProperties.ChildNodes)
            {
                if (string.Equals(node.Name, "Wrapper", StringComparison.CurrentCultureIgnoreCase))
                    wrapper = LoadWrapper(node);
                else if (string.Equals(node.Name, "Tray", StringComparison.CurrentCultureIgnoreCase))
                    tray = LoadTray(node);
                else if (string.Equals(node.Name, "StrapperSet", StringComparison.CurrentCultureIgnoreCase))
                    LoadStrapperSet(node, ref strapperSet);
            }
            Vector3D bulge = Vector3D.Zero;
            if (eltPackProperties.HasAttribute("Bulge"))
                bulge = UnitsManagerEx.ConvertLengthFrom(Vector3D.Parse(eltPackProperties.Attributes["Bulge"].Value), UnitSystem);
            PackProperties packProperties = CreateNewPack(
                sname
                , sdescription
                , GetTypeByGuid(new Guid(sBoxId)) as Packable
                , PackArrangement.TryParse(sArrangement)
                , HalfAxis.Parse(sOrientation), revSolidLayout
                , wrapper
                , tray);
            packProperties.Bulge = bulge;
            packProperties.ID.IGuid = new Guid(sid);
            packProperties.StrapperSet = strapperSet;
 
            if (eltPackProperties.HasAttribute("OuterDimensions"))
            {
                Vector3D outerDimensions = Vector3D.Parse(eltPackProperties.Attributes["OuterDimensions"].Value);
                packProperties.ForceOuterDimensions(outerDimensions);
            }
        }

        private PackWrapper LoadWrapper(XmlElement xmlWrapperElt)
        {
            if (null == xmlWrapperElt) return null;
            if (!xmlWrapperElt.HasAttribute("Type"))
                return null;

            string sType = xmlWrapperElt.Attributes["Type"].Value;
            string sColor = xmlWrapperElt.Attributes["Color"].Value;
            string sWeight = xmlWrapperElt.Attributes["Weight"].Value;
            string sUnitThickness = xmlWrapperElt.Attributes["UnitThickness"].Value;

            double thickness = UnitsManager.ConvertLengthFrom(Convert.ToDouble(sUnitThickness, CultureInfo.InvariantCulture), UnitSystem);
            Color wrapperColor = Color.FromArgb(Convert.ToInt32(sColor));
            double weight = UnitsManager.ConvertMassFrom(Convert.ToDouble(sWeight, CultureInfo.InvariantCulture), UnitSystem);

            if (sType == "WT_POLYETHILENE")
            {
                bool transparent = bool.Parse(xmlWrapperElt.Attributes["Transparent"].Value);
                return new WrapperPolyethilene(thickness, weight, wrapperColor);
            }
            else if (sType == "WT_PAPER")
            {
                return new WrapperPaper(thickness, weight, wrapperColor);
            }
            else if (sType == "WT_CARDBOARD")
            {
                string sWalls = "1 1 1";
                if (xmlWrapperElt.HasAttribute("NumberOfWalls"))
                    sWalls = xmlWrapperElt.Attributes["NumberOfWalls"].Value;
                int[] walls = sWalls.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();
                WrapperCardboard wrapper = new WrapperCardboard(thickness, weight, wrapperColor);
                wrapper.SetNoWalls(walls[0], walls[1], walls[2]);
                return wrapper;
            }
            else
                return null;
        }
        private PackTray LoadTray(XmlElement xmlTrayElt)
        {
            if (null == xmlTrayElt) return null;
            string sColor = xmlTrayElt.Attributes["Color"].Value;
            string sWeight = xmlTrayElt.Attributes["Weight"].Value;
            string sHeight = xmlTrayElt.Attributes["Height"].Value;
            string sUnitThickness = xmlTrayElt.Attributes["UnitThickness"].Value;
            string sWalls = "1 1 1";
            if (xmlTrayElt.HasAttribute("NumberOfWalls"))
                sWalls = xmlTrayElt.Attributes["NumberOfWalls"].Value;

            double height = UnitsManager.ConvertLengthFrom(Convert.ToDouble(sHeight, CultureInfo.InvariantCulture), UnitSystem);
            double weight = UnitsManager.ConvertMassFrom(Convert.ToDouble(sWeight, CultureInfo.InvariantCulture), UnitSystem);
            double unitThickness = UnitsManager.ConvertLengthFrom(Convert.ToDouble(sUnitThickness, CultureInfo.InvariantCulture), UnitSystem);
            Color color = Color.FromArgb(Convert.ToInt32(sColor));
            int[] walls = sWalls.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();

            PackTray tray = new PackTray(height, weight, color);
            tray.SetNoWalls(walls);
            return tray;
        }

        private PackTray LoadTrayLegacy(XmlElement xmlWrapperElt)
        {
            if (null == xmlWrapperElt) return null;

            string sType = xmlWrapperElt.Attributes["Type"].Value;
            string sColor = xmlWrapperElt.Attributes["Color"].Value;
            string sWeight = xmlWrapperElt.Attributes["Weight"].Value;
            string sUnitThickness = xmlWrapperElt.Attributes["UnitThickness"].Value;

            double unitThickness = UnitsManager.ConvertLengthFrom(Convert.ToDouble(sUnitThickness, CultureInfo.InvariantCulture), UnitSystem);
            Color trayColor = Color.FromArgb(Convert.ToInt32(sColor));
            double weight = UnitsManager.ConvertMassFrom(Convert.ToDouble(sWeight, CultureInfo.InvariantCulture), UnitSystem);

            if (sType == "WT_TRAY")
            {
                string sWalls = "1 1 1";
                if (xmlWrapperElt.HasAttribute("NumberOfWalls"))
                    sWalls = xmlWrapperElt.Attributes["NumberOfWalls"].Value;
                int[] walls = sWalls.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();

                string sHeight = xmlWrapperElt.Attributes["Height"].Value;
                double height = UnitsManager.ConvertLengthFrom(Convert.ToDouble(sHeight, CultureInfo.InvariantCulture), UnitSystem);

                PackTray tray = new PackTray(height, weight, trayColor);
                tray.UnitThickness = unitThickness;
                tray.SetNoWalls(walls);
                tray.Height = height;
                return tray;
            }
            else
                return null;
        }

        private void LoadCylinderProperties(XmlElement eltCylinderProperties)
        {
            string sid = eltCylinderProperties.Attributes["Id"].Value;
            string sname = eltCylinderProperties.Attributes["Name"].Value;
            string sdescription = eltCylinderProperties.Attributes["Description"].Value;
            string sRadiusOuter, sRadiusInner;
            if (eltCylinderProperties.HasAttribute("RadiusOuter"))
            {
                sRadiusOuter = eltCylinderProperties.Attributes["RadiusOuter"].Value;
                sRadiusInner = eltCylinderProperties.Attributes["RadiusInner"].Value;
            }
            else
            {
                sRadiusOuter = eltCylinderProperties.Attributes["Radius"].Value;
                sRadiusInner = "0.0";
            }
            string sheight = eltCylinderProperties.Attributes["Height"].Value;
            string sweight = eltCylinderProperties.Attributes["Weight"].Value;
            string sColorTop = eltCylinderProperties.Attributes["ColorTop"].Value;
            string sColorWallOuter, sColorWallInner;
            if (eltCylinderProperties.HasAttribute("ColorWall"))
            {
                sColorWallOuter = eltCylinderProperties.Attributes["ColorWall"].Value;
                sColorWallInner = eltCylinderProperties.Attributes["ColorWall"].Value;
            }
            else
            { 
                sColorWallOuter = eltCylinderProperties.Attributes["ColorWallOuter"].Value;
                sColorWallInner = eltCylinderProperties.Attributes["ColorWallInner"].Value;
            }

            CylinderProperties cylinderProperties = CreateNewCylinder(
                sname,
                sdescription,
                UnitsManager.ConvertLengthFrom(Convert.ToDouble(sRadiusOuter, CultureInfo.InvariantCulture), UnitSystem),
                UnitsManager.ConvertLengthFrom(Convert.ToDouble(sRadiusInner, CultureInfo.InvariantCulture), UnitSystem),
                UnitsManager.ConvertLengthFrom(Convert.ToDouble(sheight, CultureInfo.InvariantCulture), UnitSystem),
                UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), UnitSystem),
                OptDouble.Zero,                
                Color.FromArgb(Convert.ToInt32(sColorTop)),
                Color.FromArgb(Convert.ToInt32(sColorWallOuter)),
                Color.FromArgb(Convert.ToInt32(sColorWallInner))
                );
            cylinderProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadBottleProperties(XmlElement eltBottleProperties)
        {
            string sid = eltBottleProperties.Attributes["Id"].Value;
            string sname = eltBottleProperties.Attributes["Name"].Value;
            string sdescription = eltBottleProperties.Attributes["Description"].Value;
            string sweight = eltBottleProperties.Attributes["Weight"].Value;
            string scolor = eltBottleProperties.Attributes["Color"].Value;

            List<Vector2D> profile = new List<Vector2D>();
            foreach (var xmlNode in eltBottleProperties.SelectNodes("Profile"))
            {
                XmlElement xmlEltProfile = xmlNode as XmlElement;
                foreach (var hdNode in xmlEltProfile.SelectNodes("HeightDiameter"))
                {
                    try
                    {
                        XmlElement hdElt = hdNode as XmlElement;
                        profile.Add(LoadVectorLength(hdElt, "value"));
                    }
                    catch (Exception ex)
                    {
                        _log.Warn(ex.ToString());
                    }
                }
            }
            var bottleProperties = CreateNewBottle(
                sname,
                sdescription,
                profile,
                UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), UnitSystem),
                LoadOptDouble(eltBottleProperties, "NetWeight", UnitsManager.UnitType.UT_MASS),
                Color.FromArgb(Convert.ToInt32(scolor))
                );
            bottleProperties.ID.IGuid = Guid.Parse(sid);

        }
        private void LoadFaceColors(XmlElement eltColors, ref Color[] colors)
        {
            foreach (XmlNode faceColorNode in eltColors.ChildNodes)
            {
                XmlElement faceColorElt = faceColorNode as XmlElement;
                string sFaceIndex = faceColorElt.Attributes["FaceIndex"].Value;
                string sColorArgb = faceColorElt.Attributes["Color"].Value;
                int iFaceIndex = Convert.ToInt32(sFaceIndex);
                Color faceColor = Color.FromArgb(Convert.ToInt32(sColorArgb));
                colors[iFaceIndex] = faceColor;
            }
        }
        private void LoadTextureList(XmlElement eltTextureList, ref List<Pair<HalfAxis.HAxis, Texture>> listTexture)
        {
            foreach (XmlNode faceTextureNode in eltTextureList.ChildNodes)
            {
                try
                {
                    XmlElement xmlFaceTexture = faceTextureNode as XmlElement;
                    // face normal
                    HalfAxis.HAxis faceNormal = HalfAxis.Parse(xmlFaceTexture.Attributes["FaceNormal"].Value);
                    // position
                    Vector2D position = Vector2D.Parse(xmlFaceTexture.Attributes["Position"].Value);
                    // size
                    Vector2D size = Vector2D.Parse(xmlFaceTexture.Attributes["Size"].Value);
                    // angle
                    double angle = Convert.ToDouble(xmlFaceTexture.Attributes["Angle"].Value, CultureInfo.InvariantCulture);
                    // bitmap
                    Bitmap bmp = StringToBitmap(xmlFaceTexture.Attributes["Bitmap"].Value);
                    // add texture pair
                    listTexture.Add(new Pair<HalfAxis.HAxis, Texture>(faceNormal
                        , new Texture(
                            bmp
                            , UnitsManagerEx.ConvertLengthFrom(position, UnitSystem)
                            , UnitsManagerEx.ConvertLengthFrom(size, UnitSystem)
                            , angle)));
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                }
            }
        }
        private bool LoadTape(XmlElement eltTape, out double tapeWidth, out Color tapeColor)
        {
            tapeWidth = 0.0;
            tapeColor = Color.Black;
            try
            {
                tapeWidth = Convert.ToDouble(eltTape.Attributes["TapeWidth"].Value, CultureInfo.InvariantCulture);
                string sColorArgb = eltTape.Attributes["TapeColor"].Value;
                tapeColor = Color.FromArgb(Convert.ToInt32(sColorArgb));
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return false;
            }
            return true;
        }
        private bool LoadStrapperSet(XmlElement eltStrapperSet, ref StrapperSet strapperSet)
        {
            try
            {
                strapperSet.Width = Convert.ToDouble(eltStrapperSet.Attributes["Width"].Value, CultureInfo.InvariantCulture);
                strapperSet.Color = Color.FromArgb(Convert.ToInt32(eltStrapperSet.Attributes["Color"].Value));

                foreach (XmlNode node in eltStrapperSet.ChildNodes)
                {
                    if (string.Equals(node.Name, "EvenlySpacedStrappers", StringComparison.CurrentCultureIgnoreCase))
                    {
                        XmlElement eltDir = node as XmlElement;
                        int iDir = Convert.ToInt32(eltDir.Attributes["Dir"].Value);
                        int iNumber = Convert.ToInt32(eltDir.Attributes["Number"].Value);
                        double space = UnitsManager.ConvertLengthFrom( Convert.ToDouble(eltDir.Attributes["Spacing"].Value, CultureInfo.InvariantCulture), UnitSystem);
                        strapperSet.SetEvenlySpaced(iDir, iNumber, space, false);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return false;
            }
            return true;
        }

        private bool LoadPalletLabelInstances(XmlElement eltPalletLabels, ref List<PalletLabelInst> palletLabelInstances)
        {
            try
            {
                foreach (XmlNode node in eltPalletLabels)
                {
                    if (string.Equals(node.Name, "PalletLabelInst", StringComparison.CurrentCultureIgnoreCase))
                    {
                        XmlElement eltPalletLabelInst = node as XmlElement;
                        string sId = eltPalletLabelInst.Attributes["Id"].Value;
                        string sPosition = eltPalletLabelInst.Attributes["Position"].Value;
                        string sAxis = eltPalletLabelInst.Attributes["Side"].Value;
                        PalletLabelProperties palletLabelProperties = GetTypeByGuid(sId) as PalletLabelProperties;
                        Vector2D position = UnitsManagerEx.ConvertLengthFrom(Vector2D.Parse(sPosition), UnitSystem);
                        HalfAxis.HAxis axis = HalfAxis.Parse(sAxis);
                        palletLabelInstances.Add(new PalletLabelInst(palletLabelProperties, position, axis));
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return false;
            }
            return true;
        }

        private bool LoadConveyorSettings(XmlElement eltConveyorSettings, ref List<ConveyorSetting> conveyorSettings)
        {
            try
            {
                foreach (XmlNode node in eltConveyorSettings)
                {
                    if (string.Equals(node.Name, "ConveyorSetting", StringComparison.CurrentCultureIgnoreCase))
                    {
                        XmlElement eltConveyorSetting = node as XmlElement;
                        int id = 0;
                        if (eltConveyorSetting.HasAttribute("Id"))
                            id = int.Parse(eltConveyorSetting.Attributes["Id"].Value);
                        string sNumber = eltConveyorSetting.Attributes["Number"].Value;
                        string sAngle = eltConveyorSetting.Attributes["Angle"].Value;
                        string sGripperAngle = eltConveyorSetting.Attributes["GripperAngle"].Value;
                        conveyorSettings.Add(new ConveyorSetting(int.Parse(sAngle), int.Parse(sNumber), int.Parse(sGripperAngle)));
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return false;
            }
            return true;
        }
        private bool LoadRobotPreparation(XmlElement eltRobotPreparation, ref RobotPreparation robotPreparation)
        {
            try
            {
                foreach (XmlNode node in eltRobotPreparation)
                {
                    if (string.Equals(node.Name, "LayerTypes", StringComparison.CurrentCultureIgnoreCase))
                    {
                        XmlNode eltLayerTypes = node as XmlElement;
                        foreach (var nodeLayer in eltLayerTypes)
                        {
                            XmlElement eltLayerType = nodeLayer as XmlElement;
                            int layerID = int.Parse(eltLayerType.Attributes["LayerId"].Value);

                            var robotLayer = new RobotLayer(robotPreparation, layerID);
                            var robotDrops = new List<RobotDrop>();
                            foreach (var nodeRobotDrop in eltLayerType.ChildNodes)
                            {
                                ConveyorSetting conveyorSetting = new ConveyorSetting(0, 1, 0);
                                robotDrops.Add(new RobotDrop(robotLayer, conveyorSetting));
                            }
                            robotPreparation.LayerTypes.Add(new RobotLayer(robotPreparation, layerID) { Drops = robotDrops });
                        }
                    }
                    else if (string.Equals(node.Name, "LayerIndexes", StringComparison.CurrentCultureIgnoreCase))
                    {
                    }
                    else if (string.Equals(node.Name, "InterlayerIndexes", StringComparison.CurrentCultureIgnoreCase))
                    { 
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return false;
            }
            return true;
        }
        private void LoadPalletProperties(XmlElement eltPalletProperties)
        {
            string sid = eltPalletProperties.Attributes["Id"].Value;
            string sname = eltPalletProperties.Attributes["Name"].Value;
            string sdescription = eltPalletProperties.Attributes["Description"].Value;
            string slength = eltPalletProperties.Attributes["Length"].Value;
            string swidth = eltPalletProperties.Attributes["Width"].Value;
            string sheight = eltPalletProperties.Attributes["Height"].Value;
            string sweight = eltPalletProperties.Attributes["Weight"].Value;
            string sadmissibleloadweight = eltPalletProperties.Attributes["AdmissibleLoadWeight"].Value;
            string sadmissibleloadheight = eltPalletProperties.Attributes["AdmissibleLoadHeight"].Value;
            string stype = eltPalletProperties.Attributes["Type"].Value;
            string sColor = eltPalletProperties.Attributes["Color"].Value;

            if ("0" == stype)
                stype = "Block";
            else if ("1" == stype)
                stype = "UK Standard";

            // create new PalletProperties instance
            PalletProperties palletProperties = CreateNewPallet(
                sname
                , sdescription
                , stype
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(slength, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(swidth, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sheight, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), UnitSystem)
                , Color.FromArgb(Convert.ToInt32(sColor)));
            palletProperties.AdmissibleLoadWeight = UnitsManager.ConvertMassFrom(Convert.ToDouble(sadmissibleloadweight, CultureInfo.InvariantCulture), UnitSystem);
            palletProperties.AdmissibleLoadHeight = UnitsManager.ConvertLengthFrom(Convert.ToDouble(sadmissibleloadheight, CultureInfo.InvariantCulture), UnitSystem);
            palletProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadInterlayerProperties(XmlElement eltInterlayerProperties)
        {
            string sid = eltInterlayerProperties.Attributes["Id"].Value;
            string sname = eltInterlayerProperties.Attributes["Name"].Value;
            string sdescription = eltInterlayerProperties.Attributes["Description"].Value;
            string slength = eltInterlayerProperties.Attributes["Length"].Value;
            string swidth = eltInterlayerProperties.Attributes["Width"].Value;
            string sthickness = eltInterlayerProperties.Attributes["Thickness"].Value;
            string sweight = eltInterlayerProperties.Attributes["Weight"].Value;
            string sColor = eltInterlayerProperties.Attributes["Color"].Value;

            // create new InterlayerProperties instance
            InterlayerProperties interlayerProperties = CreateNewInterlayer(
                sname
                , sdescription
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(slength, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(swidth, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sthickness, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), UnitSystem)
                , Color.FromArgb(Convert.ToInt32(sColor)));
            interlayerProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadPalletCornerProperties(XmlElement eltPalletCornerProperties)
        {
            string sid = eltPalletCornerProperties.Attributes["Id"].Value;
            string sname = eltPalletCornerProperties.Attributes["Name"].Value;
            string sdescription = eltPalletCornerProperties.Attributes["Description"].Value;
            string slength = eltPalletCornerProperties.Attributes["Length"].Value;
            string swidth = eltPalletCornerProperties.Attributes["Width"].Value;
            string sthickness = eltPalletCornerProperties.Attributes["Thickness"].Value;
            string sweight = eltPalletCornerProperties.Attributes["Weight"].Value;
            string sColor = eltPalletCornerProperties.Attributes["Color"].Value;

            // create new PalletCornerProperties instance
            PalletCornerProperties palletCornerProperties = CreateNewPalletCorners(
                sname
                , sdescription
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(slength, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(swidth, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sthickness, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), UnitSystem)
                , Color.FromArgb(Convert.ToInt32(sColor))
                );
            palletCornerProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadPalletCapProperties(XmlElement eltPalletCapProperties)
        {
            string sid = eltPalletCapProperties.Attributes["Id"].Value;
            string sname = eltPalletCapProperties.Attributes["Name"].Value;
            string sdescription = eltPalletCapProperties.Attributes["Description"].Value;
            string slength = eltPalletCapProperties.Attributes["Length"].Value;
            string swidth = eltPalletCapProperties.Attributes["Width"].Value;
            string sheight = eltPalletCapProperties.Attributes["Height"].Value;
            string sinnerlength = eltPalletCapProperties.Attributes["InsideLength"].Value;
            string sinnerwidth = eltPalletCapProperties.Attributes["InsideWidth"].Value;
            string sinnerheight = eltPalletCapProperties.Attributes["InsideHeight"].Value;
            string sweight = eltPalletCapProperties.Attributes["Weight"].Value;
            string sColor = eltPalletCapProperties.Attributes["Color"].Value;

            PalletCapProperties palletCapProperties = CreateNewPalletCap(
                sname
                , sdescription
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(slength, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(swidth, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sheight, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sinnerlength, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sinnerwidth, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sinnerheight, CultureInfo.InvariantCulture), UnitSystem)
                , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), UnitSystem)
                , Color.FromArgb(Convert.ToInt32(sColor))
                );
            palletCapProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadPalletFilmProperties(XmlElement eltPalletFilmProperties)
        {
            string sid = eltPalletFilmProperties.Attributes["Id"].Value;
            string sname = eltPalletFilmProperties.Attributes["Name"].Value;
            string sdescription = eltPalletFilmProperties.Attributes["Description"].Value;
            bool useTransparency = bool.Parse(eltPalletFilmProperties.Attributes["Transparency"].Value);
            bool useHatching = bool.Parse(eltPalletFilmProperties.Attributes["Hatching"].Value);
            string sHatchSpacing = eltPalletFilmProperties.Attributes["HatchSpacing"].Value;
            string sHatchAngle = eltPalletFilmProperties.Attributes["HatchAngle"].Value;
            string sColor = eltPalletFilmProperties.Attributes["Color"].Value;
            string sWeight = string.Empty;
            if (eltPalletFilmProperties.HasAttribute("Weight"))
                sWeight = eltPalletFilmProperties.Attributes["Weight"].Value;

            PalletFilmProperties palletFilmProperties = CreateNewPalletFilm(
                sname,
                sdescription,
                useTransparency,
                useHatching,
                UnitsManager.ConvertLengthFrom(Convert.ToDouble(sHatchSpacing, CultureInfo.InvariantCulture), UnitSystem),
                Convert.ToDouble(sHatchAngle, CultureInfo.InvariantCulture),
                string.IsNullOrEmpty(sWeight) ? 0.0 : UnitsManager.ConvertMassFrom(Convert.ToDouble(sWeight, CultureInfo.InvariantCulture), UnitSystem),
                Color.FromArgb(Convert.ToInt32(sColor))
                );
            palletFilmProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadPalletLabelProperties(XmlElement eltPalletLabelProperties)
        {
            string sid = eltPalletLabelProperties.Attributes["Id"].Value;
            string sname = eltPalletLabelProperties.Attributes["Name"].Value;
            string sdescription = eltPalletLabelProperties.Attributes["Description"].Value;
            string sdimensions = eltPalletLabelProperties.Attributes["Dimensions"].Value;
            string sweight = eltPalletLabelProperties.Attributes["Weight"].Value;
            Vector2D dimensions = UnitsManagerEx.ConvertLengthFrom(Vector2D.Parse(sdimensions), UnitSystem);
            double weight = double.Parse(sweight, CultureInfo.InvariantCulture);
            Color color = Color.FromArgb(Int32.Parse(eltPalletLabelProperties.Attributes["Color"].Value));
            Bitmap bitmap =StringToBitmap(eltPalletLabelProperties.Attributes["Bitmap"].Value);

            PalletLabelProperties palletLabelProperties = CreateNewPalletLabel(
                sname
                , sdescription
                , dimensions
                , UnitsManager.ConvertMassFrom(weight, UnitSystem)
                , color
                , bitmap);
            palletLabelProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadBundleProperties(XmlElement eltBundleProperties)
        {
            string sid = eltBundleProperties.Attributes["Id"].Value;
            string sname = eltBundleProperties.Attributes["Name"].Value;
            string sdescription = eltBundleProperties.Attributes["Description"].Value;
            double length = double.Parse(eltBundleProperties.Attributes["Length"].Value, CultureInfo.InvariantCulture);
            double width = double.Parse(eltBundleProperties.Attributes["Width"].Value, CultureInfo.InvariantCulture);
            double unitThickness = double.Parse(eltBundleProperties.Attributes["UnitThickness"].Value, CultureInfo.InvariantCulture);
            double unitWeight = double.Parse(eltBundleProperties.Attributes["UnitWeight"].Value, CultureInfo.InvariantCulture);
            Color color = Color.FromArgb(Int32.Parse(eltBundleProperties.Attributes["Color"].Value));
            int noFlats = int.Parse(eltBundleProperties.Attributes["NumberFlats"].Value);
            BundleProperties bundleProperties = CreateNewBundle(
                sname
                , sdescription
                , UnitsManager.ConvertLengthFrom(length, UnitSystem)
                , UnitsManager.ConvertLengthFrom(width, UnitSystem)
                , UnitsManager.ConvertLengthFrom(unitThickness, UnitSystem)
                , UnitsManager.ConvertMassFrom(unitWeight, UnitSystem)
                , color
                , noFlats);
            bundleProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadTruckProperties(XmlElement eltTruckProperties)
        {
            string sid = eltTruckProperties.Attributes["Id"].Value;
            string sName = eltTruckProperties.Attributes["Name"].Value;
            string sDescription = eltTruckProperties.Attributes["Description"].Value;
            double length = double.Parse(eltTruckProperties.Attributes["Length"].Value, CultureInfo.InvariantCulture);
            double width = double.Parse(eltTruckProperties.Attributes["Width"].Value, CultureInfo.InvariantCulture);
            double height = double.Parse(eltTruckProperties.Attributes["Height"].Value, CultureInfo.InvariantCulture);
            double admissibleLoadWeight = double.Parse(eltTruckProperties.Attributes["AdmissibleLoadWeight"].Value, CultureInfo.InvariantCulture);
            string sColor = eltTruckProperties.Attributes["Color"].Value;

            // create new truck
            TruckProperties truckProperties = CreateNewTruck(
                sName
                , sDescription
                , UnitsManager.ConvertLengthFrom(length, UnitSystem)
                , UnitsManager.ConvertLengthFrom(width, UnitSystem)
                , UnitsManager.ConvertLengthFrom(height, UnitSystem)
                , UnitsManager.ConvertMassFrom(admissibleLoadWeight, UnitSystem)
                , Color.FromArgb(Convert.ToInt32(sColor)));
            truckProperties.ID.IGuid = new Guid(sid);
        }
        #endregion

        #region Load analysis
        private void LoadAnalysis(XmlElement eltAnalysis)
        {
            string sId = string.Empty;
            if (eltAnalysis.HasAttribute("Id"))
                sId = eltAnalysis.Attributes["Id"].Value;
            string sName = eltAnalysis.Attributes["Name"].Value;
            string sDescription = eltAnalysis.Attributes["Description"].Value;
            string sPalletCornerId = string.Empty;
            if (eltAnalysis.HasAttribute("PalletCornerId"))
                sPalletCornerId = eltAnalysis.Attributes["PalletCornerId"].Value;
            string sPalletCornerTopId = string.Empty;
            if (eltAnalysis.HasAttribute("PalletCornerTopId"))
                sPalletCornerTopId = eltAnalysis.Attributes["PalletCornerTopId"].Value;
            string sPalletCapId = string.Empty;
            if (eltAnalysis.HasAttribute("PalletCapId"))
                sPalletCapId = eltAnalysis.Attributes["PalletCapId"].Value;
            string sTopInterlayerId = string.Empty;
            if (eltAnalysis.HasAttribute("TopInterlayerId"))
                sTopInterlayerId = eltAnalysis.Attributes["TopInterlayerId"].Value;
            string sPalletFilmId = string.Empty;
            if (eltAnalysis.HasAttribute("PalletFilmId"))
                sPalletFilmId = eltAnalysis.Attributes["PalletFilmId"].Value;
            // content
            string sContentId = string.Empty;
            if (eltAnalysis.HasAttribute("ContentId"))
                sContentId = eltAnalysis.Attributes["ContentId"].Value;
            // container
            string sContainerId = string.Empty;
            if (eltAnalysis.HasAttribute("ContainerId"))
                sContainerId = eltAnalysis.Attributes["ContainerId"].Value;
            // interlayers
            List<InterlayerProperties> interlayers = new List<InterlayerProperties>();
            List<LayerEncap> listLayerEncaps = null;
            List<SolutionItem> listSolItems = null;

            if (string.Equals(eltAnalysis.Name, "AnalysisCasePallet", StringComparison.CurrentCultureIgnoreCase))
            {
                Packable packable = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
                PalletProperties palletProperties = GetTypeByGuid(sContainerId) as PalletProperties;
                PalletCornerProperties palletCorners = GetTypeByGuid(sPalletCornerId) as PalletCornerProperties;
                PalletCornerProperties palletCornersTop = GetTypeByGuid(sPalletCornerTopId) as PalletCornerProperties;
                PalletCapProperties palletCap = GetTypeByGuid(sPalletCapId) as PalletCapProperties;
                InterlayerProperties topInterlayer = GetTypeByGuid(sTopInterlayerId) as InterlayerProperties;
                PalletFilmProperties palletFilm = GetTypeByGuid(sPalletFilmId) as PalletFilmProperties;
                double palletFilmTopCovering = 0.0;
                if (null != palletFilm)
                {
                    if (eltAnalysis.HasAttribute("PalletFilmTopCovering"))
                        palletFilmTopCovering = LoadDouble(eltAnalysis, "PalletFilmTopCovering", UnitsManager.UnitType.UT_LENGTH);
                }
                bool hasPalletSleeve = false;
                Color palletSleeveColor = Color.Beige;
                if (eltAnalysis.HasAttribute("PalletSleeveColor"))
                {
                    hasPalletSleeve = true;
                    palletSleeveColor = Color.FromArgb(Convert.ToInt32(eltAnalysis.Attributes["PalletSleeveColor"].Value));
                }

                StrapperSet strapperSet = new StrapperSet();
                List<PalletLabelInst> palletLabelInstances = new List<PalletLabelInst>();
                List<ConveyorSetting> conveyorSettings = new List<ConveyorSetting>();
                RobotPreparation robotPreparation = null; 

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetCasePallet(node as XmlElement);
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerEncaps, out listSolItems);
                    else if (string.Equals(node.Name, "Interlayers", StringComparison.CurrentCultureIgnoreCase))
                        interlayers = LoadInterlayers(node as XmlElement);
                    else if (string.Equals(node.Name, "StrapperSet", StringComparison.CurrentCultureIgnoreCase))
                        LoadStrapperSet(node as XmlElement, ref strapperSet);
                    else if (string.Equals(node.Name, "PalletLabelInstances", StringComparison.CurrentCultureIgnoreCase))
                        LoadPalletLabelInstances(node as XmlElement, ref palletLabelInstances);
                    else if (string.Equals(node.Name, "ConveyorSettings", StringComparison.CurrentCultureIgnoreCase))
                        LoadConveyorSettings(node as XmlElement, ref conveyorSettings);
                    else if (string.Equals(node.Name, "RobotPreparation", StringComparison.CurrentCultureIgnoreCase))
                        LoadRobotPreparation(node as XmlElement, ref robotPreparation);
                }

                var analysis = CreateNewAnalysisCasePallet(
                    sName, sDescription
                    , packable, palletProperties
                    , interlayers
                    , palletCorners, palletCap, palletFilm
                    , constraintSet as ConstraintSetCasePallet
                    , listLayerEncaps
                    , conveyorSettings) as AnalysisLayered;
                AnalysisCasePallet analysisCasePallet = analysis as AnalysisCasePallet;
                analysisCasePallet.PalletCornerTopProperties = palletCornersTop;
                analysisCasePallet.StrapperSet = strapperSet;
                analysisCasePallet.PalletLabels = palletLabelInstances;
                analysisCasePallet.PalletFilmTopCovering = palletFilmTopCovering;
                analysisCasePallet.HasPalletSleeve = hasPalletSleeve;
                analysisCasePallet.PalletSleeveColor = palletSleeveColor;
                analysisCasePallet.TopInterlayerProperties = topInterlayer;
                analysisCasePallet.RobotPreparation = robotPreparation;

                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
                analysis.SolutionLay.SolutionItems = listSolItems;

            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisBoxCase", StringComparison.CurrentCultureIgnoreCase))
            {
                Packable packable = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
                BoxProperties caseProperties = GetTypeByGuid(sContainerId) as BoxProperties;

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetBoxCase(node as XmlElement, caseProperties);
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerEncaps, out listSolItems);
                    else if (string.Equals(node.Name, "Interlayers", StringComparison.CurrentCultureIgnoreCase))
                        interlayers = LoadInterlayers(node as XmlElement);
                }

                var analysis = CreateNewAnalysisBoxCase(
                    sName, sDescription
                    , packable, caseProperties
                    , interlayers
                    , constraintSet as ConstraintSetBoxCase, listLayerEncaps) as AnalysisLayered;
                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
                analysis.SolutionLay.SolutionItems = listSolItems;
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisCylinderPallet", StringComparison.CurrentCultureIgnoreCase))
            {
                RevSolidProperties revSolidProperties = GetContentByGuid(Guid.Parse(sContentId)) as RevSolidProperties;
                PalletProperties palletProperties = GetTypeByGuid(sContainerId) as PalletProperties;

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetCasePallet(node as XmlElement);
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerEncaps, out listSolItems);
                    else if (string.Equals(node.Name, "Interlayers", StringComparison.CurrentCultureIgnoreCase))
                        interlayers = LoadInterlayers(node as XmlElement);
                }

                var analysis = CreateNewAnalysisCylinderPallet(
                    sName, sDescription
                    , revSolidProperties, palletProperties
                    , interlayers
                    , constraintSet as ConstraintSetPackablePallet, listLayerEncaps)
                    as AnalysisLayered;
                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
                analysis.SolutionLay.SolutionItems = listSolItems;
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisCylinderCase", StringComparison.CurrentCultureIgnoreCase))
            {
                Packable revSolidProperties = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
                BoxProperties caseProperties = GetTypeByGuid(sContainerId) as BoxProperties;

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetCylinderCase(node as XmlElement, caseProperties);
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerEncaps, out listSolItems);
                    else if (string.Equals(node.Name, "Interlayers", StringComparison.CurrentCultureIgnoreCase))
                        interlayers = LoadInterlayers(node as XmlElement);
                }

                var analysis = CreateNewAnalysisCylinderCase(
                    sName, sDescription
                    , revSolidProperties as RevSolidProperties, caseProperties
                    , interlayers
                    , constraintSet as ConstraintSetCylinderContainer
                    , listLayerEncaps)
                    as AnalysisLayered;
                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
                analysis.SolutionLay.SolutionItems = listSolItems;
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisPalletTruck", StringComparison.CurrentCultureIgnoreCase))
            {
                Packable loadedPallet = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
                TruckProperties truckProperties = GetTypeByGuid(sContainerId) as TruckProperties;

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetPalletTruck(node as XmlElement, truckProperties);
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerEncaps, out listSolItems);
                }

                var analysis = CreateNewAnalysisPalletTruck(sName, sDescription
                    , loadedPallet, truckProperties
                    , constraintSet as ConstraintSetPalletTruck
                    , listLayerEncaps)
                    as AnalysisLayered;
                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
                analysis.SolutionLay.SolutionItems = listSolItems;
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisCaseTruck", StringComparison.CurrentCultureIgnoreCase))
            {
                Packable packable = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
                TruckProperties truckProperties = GetTypeByGuid(sContainerId) as TruckProperties;

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetCaseTruck(node as XmlElement, truckProperties);
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerEncaps, out listSolItems);
                    else if (string.Equals(node.Name, "Interlayers", StringComparison.CurrentCultureIgnoreCase))
                        interlayers = LoadInterlayers(node as XmlElement);
                }
                CreateNewAnalysisCaseTruck(sName, sDescription
                    , packable, truckProperties, interlayers
                    , constraintSet as ConstraintSetCaseTruck, listLayerEncaps);
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisCylinderTruck", StringComparison.CurrentCultureIgnoreCase))
            {
                TruckProperties truckProperties = GetTypeByGuid(sContainerId) as TruckProperties;
                ConstraintSetCylinderTruck constraintSet = null;

                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    // load constraint set
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetCylinderTruck(node as XmlElement, truckProperties);
                    // load solutions
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerEncaps, out listSolItems);
                }

                var analysis = CreateNewAnalysisCylinderTruck(
                    sName, sDescription,
                    GetTypeByGuid(sContentId) as CylinderProperties,
                    GetTypeByGuid(sContainerId) as TruckProperties,
                    constraintSet,
                    listLayerEncaps) as AnalysisLayered;

                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
                analysis.SolutionLay.SolutionItems = listSolItems;
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisPalletsOnPallet", StringComparison.CurrentCultureIgnoreCase))
            {
                var palletProperties = GetTypeByGuid(sContainerId) as PalletProperties;
                LoadedPallet pallet0 = null, pallet1 = null, pallet2 = null, pallet3 = null;
                if (eltAnalysis.HasAttribute("Pallet0"))
                    pallet0 = GetContentByGuid(Guid.Parse(eltAnalysis.Attributes["Pallet0"].Value)) as LoadedPallet;
                if (eltAnalysis.HasAttribute("Pallet1"))
                    pallet1 = GetContentByGuid(Guid.Parse(eltAnalysis.Attributes["Pallet1"].Value)) as LoadedPallet;
                if (eltAnalysis.HasAttribute("Pallet2"))
                    pallet2 = GetContentByGuid(Guid.Parse(eltAnalysis.Attributes["Pallet2"].Value)) as LoadedPallet;
                if (eltAnalysis.HasAttribute("Pallet3"))
                    pallet3 = GetContentByGuid(Guid.Parse(eltAnalysis.Attributes["Pallet3"].Value)) as LoadedPallet;

                AnalysisPalletsOnPallet.EMasterPalletSplit masterPalletSplit = AnalysisPalletsOnPallet.EMasterPalletSplit.HORIZONTAL;
                if (eltAnalysis.HasAttribute("MasterPalletSplit"))
                    masterPalletSplit = (AnalysisPalletsOnPallet.EMasterPalletSplit)int.Parse(eltAnalysis.Attributes["MasterPalletSplit"].Value);
                AnalysisPalletsOnPallet.ELoadedPalletOrientation loadedPalletOrientation = AnalysisPalletsOnPallet.ELoadedPalletOrientation.DEFAULT;
                if (eltAnalysis.HasAttribute("LoadedPalletOrientation"))
                    masterPalletSplit = (AnalysisPalletsOnPallet.EMasterPalletSplit)int.Parse(eltAnalysis.Attributes["LoadedPalletOrientation"].Value);

                var analysis = CreateNewAnalysisPalletsOnPallet(
                    sName, sDescription,
                    masterPalletSplit, loadedPalletOrientation,
                    palletProperties, pallet0, pallet1, pallet2, pallet3) as AnalysisPalletsOnPallet;
                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisHCylPallet", StringComparison.CurrentCultureIgnoreCase))
            {
                var cylProperties = GetTypeByGuid(sContentId) as CylinderProperties;
                var palletProperties = GetTypeByGuid(sContainerId) as PalletProperties;

                ConstraintSetPackablePallet constraintSet = null;
                string sDescriptor = string.Empty;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetPackablePallet(node as XmlElement) as ConstraintSetPackablePallet;
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out sDescriptor);
                }
                var analysis = CreateNewAnalysisHCylPallet(
                    sName, sDescription,
                    cylProperties,
                    palletProperties,
                    constraintSet,
                    new HCylLayout(
                        cylProperties.Diameter,
                        cylProperties.Height,
                        palletProperties.GetStackingDimensions(constraintSet),
                        sDescriptor)
                    );
                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisHCylTruck", StringComparison.CurrentCultureIgnoreCase))
            {
                var cylProperties = GetTypeByGuid(sContentId) as CylinderProperties;
                var truckProperties = GetTypeByGuid(sContainerId) as TruckProperties;

                ConstraintSetCylinderTruck constraintSet = null;
                string sDescriptor = string.Empty;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetCylinderTruck(node as XmlElement, truckProperties) as ConstraintSetCylinderTruck;
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out sDescriptor);
                }
                var analysis = CreateNewAnalysisHCylTruck(
                    sName, sDescription,
                    cylProperties,
                    truckProperties,
                    constraintSet,
                    new HCylLayout(
                        cylProperties.Diameter,
                        cylProperties.Height,
                        truckProperties.GetStackingDimensions(constraintSet),
                        sDescriptor)
                    );
            }
        }
        #region StackBuilder 2.0
        private void LoadAnalysisPallet(XmlElement eltAnalysis)
        {
            string sId = string.Empty;
            if (eltAnalysis.HasAttribute("Id"))
                sId = eltAnalysis.Attributes["Id"].Value;
            string sName = eltAnalysis.Attributes["Name"].Value;
            string sDescription = eltAnalysis.Attributes["Description"].Value;
            string sContentId = eltAnalysis.Attributes["BoxId"].Value;
            string sContainerId = eltAnalysis.Attributes["PalletId"].Value;
            string sPalletCornerId = string.Empty, sPalletCapId = string.Empty, sPalletFilmId = string.Empty;
            string sInterlayerId = string.Empty;


            Packable packable = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
            PalletProperties palletProperties = GetTypeByGuid(sContainerId) as PalletProperties;
            PalletCornerProperties palletCorners = GetTypeByGuid(sPalletCornerId) as PalletCornerProperties;
            PalletCapProperties palletCap = GetTypeByGuid(sPalletCapId) as PalletCapProperties;
            PalletFilmProperties palletFilm = GetTypeByGuid(sPalletFilmId) as PalletFilmProperties;
            StrapperSet strapperSet = new StrapperSet();            

            var interlayers = new List<InterlayerProperties>();
            if (GetTypeByGuid(sInterlayerId) is InterlayerProperties interlayer)
                interlayers.Add(interlayer);
            bool allowAlignedLayers = true, allowAlternateLayers = true;
            string sAllowedBoxPositions = string.Empty;
            string[] sAllowedPatterns;
            OptDouble maxPalletHeight = new OptDouble(true, 2000);
            OptDouble maxPalletWeight = OptDouble.Zero;
            OptInt maxNumberOfItems = OptInt.Zero;
            Vector2D overhang = Vector2D.Zero;

            foreach (var constraintSetNode in eltAnalysis.SelectNodes("ConstraintSetBox"))
            {
                if (constraintSetNode is XmlElement constraintSetElt)
                {
                    if (constraintSetElt.HasAttribute("AlignedLayersAllowed"))
                        allowAlignedLayers = bool.Parse(constraintSetElt.Attributes["AlignedLayersAllowed"].Value);
                    if (constraintSetElt.HasAttribute("AlternateLayersAllowed"))
                        allowAlternateLayers = bool.Parse(constraintSetElt.Attributes["AlternateLayersAllowed"].Value);
                    if (constraintSetElt.HasAttribute("AllowedBoxPositions"))
                        sAllowedBoxPositions = constraintSetElt.Attributes["AllowedBoxPositions"].Value;
                    if (constraintSetElt.HasAttribute("AllowedPatterns"))
                        sAllowedPatterns = constraintSetElt.Attributes["AllowedPatterns"].Value.Split(',');
                    if (constraintSetElt.HasAttribute("MaximumHeight"))
                        maxPalletHeight = new OptDouble(true, double.Parse(constraintSetElt.Attributes["MaximumHeight"].Value, NumberFormatInfo.InvariantInfo));
                    if (constraintSetElt.HasAttribute("MaximumPalletWeight"))
                        maxPalletWeight = new OptDouble(true, double.Parse(constraintSetElt.Attributes["MaximumPalletWeight"].Value, NumberFormatInfo.InvariantInfo));
                    if (constraintSetElt.HasAttribute("ManimumNumberOfItems"))
                        maxNumberOfItems = new OptInt(true, int.Parse(constraintSetElt.Attributes["ManimumNumberOfItems"].Value));
                }
            }

            bool[] allowedOrientations = { sAllowedBoxPositions.Contains("XP"), sAllowedBoxPositions.Contains("YP"), sAllowedBoxPositions.Contains("ZP") };

            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet()
            {
                Overhang = overhang,
                OptMaxWeight = maxPalletWeight,
                OptMaxNumber = maxNumberOfItems
            };
            constraintSet.SetAllowedOrientations(allowedOrientations);
            constraintSet.SetMaxHeight(maxPalletHeight);

            LayerDesc layerDesc = new LayerDescBox("Column", HalfAxis.HAxis.AXIS_Z_P, false);
            if (null != SolutionLayered.Solver)
                layerDesc = SolutionLayered.Solver.BestLayerDesc(
                    packable.OuterDimensions, Vector3D.Zero,
                    new Vector2D(palletProperties.Length + 2.0 * overhang.X, palletProperties.Width + 2.0 * overhang.Y),
                    palletProperties.Height,
                    constraintSet);

            var analysis = CreateNewAnalysisCasePallet(
                sName, sDescription
                , packable, palletProperties
                , interlayers
                , palletCorners, palletCap, palletFilm
                , constraintSet
                , new List<LayerEncap>() { new LayerEncap(layerDesc) }
                , new List<ConveyorSetting>()
                ) as AnalysisLayered;
            if (!string.IsNullOrEmpty(sId))
                analysis.ID.IGuid = Guid.Parse(sId);
            AnalysisCasePallet analysisCasePallet = analysis as AnalysisCasePallet;
            analysisCasePallet.StrapperSet = strapperSet;

        }
        private void LoadAnalysisBoxCase(XmlElement eltAnalysis)
        {
            string sId = string.Empty;
            if (eltAnalysis.HasAttribute("Id"))
                sId = eltAnalysis.Attributes["Id"].Value;
            string sName = eltAnalysis.Attributes["Name"].Value;
            string sDescription = eltAnalysis.Attributes["Description"].Value;
            string sContentId = eltAnalysis.Attributes["BoxId"].Value;
            string sContainerId = eltAnalysis.Attributes["CaseId"].Value;

            Packable packable = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
            BoxProperties caseProperties = GetTypeByGuid(sContainerId) as BoxProperties;
            List<InterlayerProperties> interlayers = new List<InterlayerProperties>();


            string sAllowedBoxPositions = string.Empty;
            foreach (var constraintSetNode in eltAnalysis.SelectNodes("ConstraintSetCase"))
            {

                if (constraintSetNode is XmlElement constraintSetElt)
                {
                if (constraintSetElt.HasAttribute("AllowedBoxPositions"))
                    sAllowedBoxPositions = constraintSetElt.Attributes["AllowedBoxPositions"].Value;
                }
            }
            bool[] allowedOrientations = { sAllowedBoxPositions.Contains("XP"), sAllowedBoxPositions.Contains("YP"), sAllowedBoxPositions.Contains("ZP") };
            var constraintSet = new ConstraintSetBoxCase(caseProperties);
            constraintSet.SetAllowedOrientations(allowedOrientations);

            var layerDesc = new LayerDescBox("Column", HalfAxis.HAxis.AXIS_Z_P, false);
            if (null != SolutionLayered.Solver)
                SolutionLayered.Solver.BestLayerDesc(
                    packable.OuterDimensions, Vector3D.Zero,
                    new Vector2D(caseProperties.InsideDimensions.X, caseProperties.InsideDimensions.Y),
                    0.0,
                    constraintSet
                    );

            var analysis = CreateNewAnalysisBoxCase(
                sName, sDescription,
                packable, caseProperties,
                interlayers, constraintSet,
                new List<LayerEncap>() { new LayerEncap(layerDesc) }
                );
            if (!string.IsNullOrEmpty(sId))
                analysis.ID.IGuid = Guid.Parse(sId);
        }
        #endregion

        private void LoadHAnalysis(XmlElement eltAnalysis)
        {
            string sId = string.Empty;
            if (eltAnalysis.HasAttribute("Id"))
                sId = eltAnalysis.Attributes["Id"].Value;
            string sName = eltAnalysis.Attributes["Name"].Value;
            string sDescription = eltAnalysis.Attributes["Description"].Value;

            // analysis members
            var containers = new List<ItemBase>(); 
            var contentItems = new List<ContentItem>();
            HSolution solution = null;
            HConstraintSet constraintSet = null;

            foreach (XmlNode node in eltAnalysis.ChildNodes)
            {
                if (string.Equals(node.Name, "Containers", StringComparison.CurrentCultureIgnoreCase))
                {
                    var eltContainers = node as XmlElement;
                    foreach (var nodeContainer in eltContainers.ChildNodes)
                    {
                        if (nodeContainer is XmlElement eltContainer)
                        {
                            containers.Add(GetTypeByGuid(Guid.Parse(eltContainer.Attributes["Id"].Value)));
                        }
                    }
                }
                else if (string.Equals(node.Name, "ContentItems", StringComparison.CurrentCultureIgnoreCase))
                {
                    XmlElement eltContentItems = node as XmlElement;
                    foreach (XmlNode nodeContentItem in eltContentItems)
                    {
                        if (nodeContentItem is XmlElement eltContentItem)
                        {
                            string sPackableId = eltContentItem.Attributes["PackableId"].Value;
                            uint number = Convert.ToUInt32(eltContentItem.Attributes["Number"].Value);
                            string sOrientations = eltContentItem.Attributes["Orientations"].Value;
                            bool[] orientations = sOrientations.Split(',').Select(s => bool.Parse(s)).ToArray();

                            contentItems.Add(new ContentItem(GetTypeByGuid(sPackableId) as Packable, number, orientations));
                        }
                    }
                }
                else if (string.Equals(node.Name, "HConstraintSetPallet", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (node is XmlElement eltHConstraintSet)
                    {
                        string sMaximumHeight = eltHConstraintSet.Attributes["MaximumHeight"].Value;
                        Vector2D vOverhang = Vector2D.Zero;
                        if (eltHConstraintSet.HasAttribute("Overhang"))
                            vOverhang = LoadVectorLength(eltHConstraintSet, "Overhang");

                        constraintSet = new HConstraintSetPallet()
                        {
                            MaximumHeight = UnitsManager.ConvertLengthFrom(Convert.ToDouble(sMaximumHeight), UnitSystem),
                            Overhang = vOverhang
                        };
                    }
                }
                else if (string.Equals(node.Name, "HConstraintSetTruck", StringComparison.CurrentCultureIgnoreCase))
                {
                }
                else if (string.Equals(node.Name, "HSolution", StringComparison.CurrentCultureIgnoreCase))
                {
                    XmlElement eltSolution = node as XmlElement;
                    solution = new HSolution(eltSolution.Attributes["Algo"].Value);

                    foreach (var nodeSolItem in eltSolution.ChildNodes)
                    {
                        if (nodeSolItem is XmlElement eltSolItem)
                        {
                            var hSolItem = solution.CreateSolItem();
                            hSolItem.ContainerType = 0;
                            foreach (var nodeContained in eltSolItem.ChildNodes)
                            {
                                if (nodeContained is XmlElement eltContained)
                                {
                                    hSolItem.InsertContainedElt(
                                        Convert.ToInt32(eltContained.Attributes["ContentTypeIndex"].Value),
                                        UnitsManagerEx.ConvertLengthFrom(BoxPosition.Parse(eltContained.Attributes["Position"].Value), UnitSystem));
                                }
                            }
                        }
                    }
                }
            }
            // sanity check
            if (containers.Count < 1)
            {
                _log.Error("No container found!");
                return;
            }
            // instantiate analysis
            if (string.Equals(eltAnalysis.Name, "HAnalysisPallet", StringComparison.CurrentCultureIgnoreCase))
            {
                AnalysisHetero analysis = CreateNewHAnalysisCasePallet(
                    sName, sDescription,
                    contentItems, containers[0] as PalletProperties,
                    constraintSet as HConstraintSetPallet,
                    solution
                    );
            }
            else if (string.Equals(eltAnalysis.Name, "HAnalysisCase"))
            {

            }
            else if (string.Equals(eltAnalysis.Name, "HAnalysisTruck"))
            {
                AnalysisHetero analysis = CreateNewHAnalysisCaseTruck(
                    sName, sDescription,
                    contentItems, containers[0] as TruckProperties,
                    constraintSet as HConstraintSetTruck,
                    solution
                    );
            }
            else if (string.Equals(eltAnalysis.Name, "HAnalysisPalletTruck"))
            {
            }
            else
                throw new Exception($"unsupported analysis name {eltAnalysis.Name}");
        }

        #region ConstraintSet loading
        private ConstraintSetAbstract LoadConstraintSetCasePallet(XmlElement eltConstraintSet)
        {
            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet
            {
                OptMaxWeight = LoadOptDouble(eltConstraintSet, "MaximumWeight", UnitsManager.UnitType.UT_MASS),
                OptMaxNumber = LoadOptInt(eltConstraintSet, "MaximumNumberOfItems"),
                Overhang = LoadVectorLength(eltConstraintSet, "Overhang"),
                MinimumSpace = LoadOptDouble(eltConstraintSet, "MinSpace", UnitsManager.UnitType.UT_LENGTH),
                AllowedOrientationsString = eltConstraintSet.HasAttribute("Orientations") ? eltConstraintSet.Attributes["Orientations"].Value : "0,0,1"
            };

            constraintSet.SetMaxHeight(LoadOptDouble(eltConstraintSet, "MaximumPalletHeight", UnitsManager.UnitType.UT_LENGTH));
            constraintSet.PalletFilmTurns = LoadInt(eltConstraintSet, "PalletFilmTurns");
            return constraintSet;
        }
        private ConstraintSetAbstract LoadConstraintSetPackablePallet(XmlElement eltConstraintSet)
        {
            var constraintSet = new ConstraintSetPackablePallet()
            {
                OptMaxWeight = LoadOptDouble(eltConstraintSet, "MaximumWeight", UnitsManager.UnitType.UT_MASS),
                OptMaxNumber = LoadOptInt(eltConstraintSet, "MaximumNumberOfItems"),
                Overhang = LoadVectorLength(eltConstraintSet, "Overhang"),
            };
            constraintSet.SetMaxHeight(LoadOptDouble(eltConstraintSet, "MaximumPalletHeight", UnitsManager.UnitType.UT_LENGTH));
            return constraintSet;
        }
        private ConstraintSetAbstract LoadConstraintSetBoxCase(XmlElement eltConstraintSet, IContainer container)
        {
            return new ConstraintSetBoxCase(container)
            {
                OptMaxNumber = LoadOptInt(eltConstraintSet, "MaximumNumberOfItems"),
                OptMaxWeight = LoadOptDouble(eltConstraintSet, "MaximumWeight", UnitsManager.UnitType.UT_MASS)
            };
        }
        private ConstraintSetAbstract LoadConstraintSetCylinderCase(XmlElement eltConstraintSet, IContainer container)
        {
            return new ConstraintSetCylinderContainer(container)
            {
                OptMaxNumber = LoadOptInt(eltConstraintSet, "MaximumNumberOfItems"),
                OptMaxWeight = LoadOptDouble(eltConstraintSet, "MaximumWeight", UnitsManager.UnitType.UT_MASS)
            };
        }
        private ConstraintSetAbstract LoadConstraintSetPalletTruck(XmlElement eltConstraintSet, IContainer container)
        {
            return new ConstraintSetPalletTruck(container)
            {
                MinDistanceLoadWall = LoadVectorLength(eltConstraintSet, "MinDistanceLoadWall"),
                MinDistanceLoadRoof = LoadDouble(eltConstraintSet, "MinDistanceLoadRoof", UnitsManager.UnitType.UT_LENGTH),
                OptMaxLayerNumber = new OptInt((1 != LoadInt(eltConstraintSet, "AllowMultipleLayers")), 1)
            };
        }
        private ConstraintSetAbstract LoadConstraintSetCaseTruck(XmlElement eltConstraintSet, IContainer container)
        {
            return new ConstraintSetCaseTruck(container)
            {
                MinDistanceLoadWall = LoadVectorLength(eltConstraintSet, "MinDistanceLoadWall"),
                MinDistanceLoadRoof = LoadDouble(eltConstraintSet, "MinDistanceLoadRoof", UnitsManager.UnitType.UT_LENGTH),
                OptMaxNumber = LoadOptInt(eltConstraintSet, "MaximumNumberOfItems"),
                OptMaxWeight = LoadOptDouble(eltConstraintSet, "MaximumWeight", UnitsManager.UnitType.UT_MASS)
            };
        }
        #endregion

        #region Load helpers
        private List<InterlayerProperties> LoadInterlayers(XmlElement eltInterlayers)
        {
            List<InterlayerProperties> interlayers = new List<InterlayerProperties>();
            foreach (XmlNode nodeInterlayer in eltInterlayers.ChildNodes)
            {
                XmlElement eltInterlayer = nodeInterlayer as XmlElement;
                interlayers.Add(GetTypeByGuid(eltInterlayer.InnerText) as InterlayerProperties);
            }
            return interlayers;
        }
        private OptInt LoadOptInt(XmlElement xmlElement, string attribute)
        {
            if (!xmlElement.HasAttribute(attribute))
                return new OptInt(false, 0);
            return OptInt.Parse(xmlElement.Attributes[attribute].Value);
        }
        private double LoadDouble(XmlElement xmlElement, string attribute, UnitsManager.UnitType unitType)
        {
            if (xmlElement.HasAttribute(attribute))
            {
                switch (unitType)
                {
                case UnitsManager.UnitType.UT_LENGTH:
                    return UnitsManager.ConvertLengthFrom(double.Parse(xmlElement.Attributes[attribute].Value), UnitSystem);
                case UnitsManager.UnitType.UT_MASS:
                    return UnitsManager.ConvertMassFrom(double.Parse(xmlElement.Attributes[attribute].Value), UnitSystem);
                default:
                    Debug.Assert(false);
                    break;
                }
            }
            _log.Warn(string.Format("Double type attribute {0} was not found!", attribute));
            return 0.0;
        }
        private Vector2D LoadVectorLength(XmlElement xmlElement, string attribute)
        {
            if (xmlElement.HasAttribute(attribute))
            {
                Regex r = new Regex(@"\((?<x>.*),(?<y>.*)\)", RegexOptions.Singleline);
                Match m = r.Match(xmlElement.Attributes[attribute].Value);
                Vector2D v0;
                if (m.Success)
                {
                    NumberStyles style = NumberStyles.Number;
                    CultureInfo culture = CultureInfo.InvariantCulture;
                    v0 = new Vector2D(
                        double.Parse(m.Result("${x}"), style, culture),
                        double.Parse(m.Result("${y}"), style, culture)
                        );
                }
                else
                {
                    throw new ParseException("Unsuccessful Match.");
                }
                return new Vector2D(UnitsManager.ConvertLengthFrom(v0.X, UnitSystem), UnitsManager.ConvertLengthFrom(v0.Y, UnitSystem));
            }
            return Vector2D.Zero;
        }
        private int LoadInt(XmlElement xmlElement, string attribute)
        {
            if (xmlElement.HasAttribute(attribute))
                return int.Parse( xmlElement.Attributes[attribute].Value );
            return 0;
        }
        private OptDouble LoadOptDouble(XmlElement xmlElement, string attribute, UnitsManager.UnitType unitType)
        {
            if (!xmlElement.HasAttribute(attribute))
                return new OptDouble(false, 0.0);
            else
            {
                OptDouble optD = OptDouble.Parse(xmlElement.Attributes[attribute].Value);
                switch (unitType)
                {
                    case UnitsManager.UnitType.UT_LENGTH:
                        optD.Value = UnitsManager.ConvertLengthFrom(optD.Value, UnitSystem);
                        break;
                    case UnitsManager.UnitType.UT_MASS:
                        optD.Value = UnitsManager.ConvertMassFrom(optD.Value, UnitSystem);
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
                return optD;
            }
        }
        #endregion

        #region Legacy loading
        private ConstraintSetCylinderTruck LoadConstraintSetCylinderTruck(XmlElement eltConstraintSet, TruckProperties truckProperties)
        {
            return new ConstraintSetCylinderTruck(truckProperties)
            {
                MinDistanceLoadWall = LoadVectorLength(eltConstraintSet, "MinDistanceLoadWall"),
                MinDistanceLoadRoof = LoadDouble(eltConstraintSet, "MinDistanceLoadRoof", UnitsManager.UnitType.UT_LENGTH),
                OptMaxNumber = LoadOptInt(eltConstraintSet, "MaximumNumberOfItems"),
                OptMaxWeight = LoadOptDouble(eltConstraintSet, "MaximumWeight", UnitsManager.UnitType.UT_MASS)
            };
        }
        #endregion

        private void LoadSolution(
            XmlElement eltSolution
            , out List<LayerEncap> listDesc
            , out List<SolutionItem> listSolItems)
        {
            listDesc = new List<LayerEncap>();
            listSolItems = new List<SolutionItem>();

            foreach (XmlNode node in eltSolution.ChildNodes)
            {
                if (string.Equals(node.Name, "LayerDescriptors", StringComparison.CurrentCultureIgnoreCase))
                {
                    XmlElement eltLayerDescriptors = node as XmlElement;
                    foreach (XmlNode nodeLayerDesc in eltLayerDescriptors.ChildNodes)
                    {
                        XmlElement eltLayerDesc = nodeLayerDesc as XmlElement;
                        if (string.Equals(eltLayerDesc.Name, "LayerDescBox", StringComparison.CurrentCultureIgnoreCase))
                            listDesc.Add(new LayerEncap(LayerDescBox.Parse(eltLayerDesc.InnerText)));
                        else if (string.Equals(eltLayerDesc.Name, "LayerDescCyl", StringComparison.CurrentCultureIgnoreCase))
                            listDesc.Add(new LayerEncap(LayerDescCyl.Parse(eltLayerDesc.InnerText)));
                        else if (string.Equals(eltLayerDesc.Name, "LayerExpBoxes", StringComparison.CurrentCultureIgnoreCase))
                        {
                            string name = eltLayerDesc.Attributes["Name"].Value;
                            Vector3D dimBox = UnitsManagerEx.ConvertLengthFrom( Vector3D.Parse(eltLayerDesc.Attributes["DimBox"].Value), UnitSystem);
                            Vector2D dimContainer = UnitsManagerEx.ConvertLengthFrom(Vector2D.Parse(eltLayerDesc.Attributes["DimContainer"].Value), UnitSystem);
                            HalfAxis.HAxis axisOrtho = HalfAxis.Parse(eltLayerDesc.Attributes["AxisOrtho"].Value);
                            var layer2D = new Layer2DBrickExp(dimBox, Vector3D.Zero, dimContainer, name, axisOrtho);
                            foreach (XmlNode nodeBP in eltLayerDesc.ChildNodes)
                                layer2D.AddPosition(LoadBoxPosition(nodeBP as XmlElement));
                            listDesc.Add(new LayerEncap(layer2D));
                        }
                        else if (string.Equals(eltLayerDesc.Name, "LayerExpCyl", StringComparison.CurrentCultureIgnoreCase))
                        {
                            double radius = 0.0;
                            double height = 0.0;
                            Vector2D dimContainer = Vector2D.Zero;
                            var layer2D = new Layer2DCylExp(radius, height, dimContainer);
                            listDesc.Add(new LayerEncap(layer2D));
                        }
                    }
                }
                else if (string.Equals(node.Name, "SolutionItems", StringComparison.CurrentCultureIgnoreCase))
                {
                    XmlElement eltSolutionItems = node as XmlElement;
                    foreach (XmlNode nodeSolutionItem in eltSolutionItems.ChildNodes)
                    {
                        if (nodeSolutionItem is XmlElement eltSolutionItem)
                            listSolItems.Add(SolutionItem.Parse(eltSolutionItem.InnerText));
                    }
                }
            }
        }

        private void LoadSolution(XmlElement eltSolution, out string sDescriptor)
        {
            var listCylLayout = eltSolution.GetElementsByTagName("CylLayout");
            XmlElement eltCylLayout = listCylLayout[0] as XmlElement;
            var listLayoutDesc = eltCylLayout.GetElementsByTagName("LayoutDesc");
            XmlElement eltLayoutDesc = listLayoutDesc[0] as XmlElement;
            sDescriptor = eltLayoutDesc.InnerText;
        }
        private BoxPosition LoadBoxPosition(XmlElement eltBoxPosition)
        {
            string sPosition = eltBoxPosition.Attributes["Position"].Value;
            string sAxisLength = eltBoxPosition.Attributes["AxisLength"].Value;
            string sAxisWidth = eltBoxPosition.Attributes["AxisWidth"].Value;
            return new BoxPosition(UnitsManagerEx.ConvertLengthFrom(Vector3D.Parse(sPosition), UnitSystem), HalfAxis.Parse(sAxisLength), HalfAxis.Parse(sAxisWidth));
        }
        #endregion

        #endregion // load methods

        #region Save methods
        #region Main methods
        public void Write(string filePath)
        {
            try
            {
                // instantiate XmlDocument
                XmlDocument xmlDoc = new XmlDocument();
                // let's add the XML declaration section
                XmlNode xmlnode = xmlDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                xmlDoc.AppendChild(xmlnode);
                // create Document (root) element
                XmlElement xmlRootElement = xmlDoc.CreateElement("Document");
                xmlDoc.AppendChild(xmlRootElement);
                // name
                XmlAttribute xmlDocNameAttribute = xmlDoc.CreateAttribute("Name");
                xmlDocNameAttribute.Value = Name;
                xmlRootElement.Attributes.Append(xmlDocNameAttribute);
                // description
                XmlAttribute xmlDocDescAttribute = xmlDoc.CreateAttribute("Description");
                xmlDocDescAttribute.Value = Description;
                xmlRootElement.Attributes.Append(xmlDocDescAttribute);
                // author
                XmlAttribute xmlDocAuthorAttribute = xmlDoc.CreateAttribute("Author");
                xmlDocAuthorAttribute.Value = Author;
                xmlRootElement.Attributes.Append(xmlDocAuthorAttribute);
                // dateCreated
                XmlAttribute xmlDateCreatedAttribute = xmlDoc.CreateAttribute("DateCreated");
                xmlDateCreatedAttribute.Value = Convert.ToString(DateOfCreation, new CultureInfo("en-US"));
                xmlRootElement.Attributes.Append(xmlDateCreatedAttribute);
                // unit system
                XmlAttribute xmlUnitSystem = xmlDoc.CreateAttribute("UnitSystem");
                xmlUnitSystem.Value = string.Format("{0}", (int)UnitsManager.CurrentUnitSystem);
                xmlRootElement.Attributes.Append(xmlUnitSystem);
                // create ItemProperties element
                XmlElement xmlItemPropertiesElt = xmlDoc.CreateElement("ItemProperties");
                xmlRootElement.AppendChild(xmlItemPropertiesElt);
                foreach (ItemBase itemProperties in _typeList)
                {
                    if (itemProperties is CaseOfBoxesProperties caseOfBoxesProperties)
                        Save(caseOfBoxesProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is BoxProperties boxProperties && !(itemProperties is CaseOfBoxesProperties))
                        Save(boxProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is BagProperties bagProperties)
                        Save(bagProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is BundleProperties bundleProperties)
                        Save(bundleProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is CylinderProperties cylinderProperties)
                        Save(cylinderProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is BottleProperties bottleProperties)
                        Save(bottleProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is PalletProperties palletProperties)
                        Save(palletProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is InterlayerProperties interlayerProperties)
                        Save(interlayerProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is PalletCornerProperties cornerProperties)
                        Save(cornerProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is PalletCapProperties capProperties)
                        Save(capProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is PalletFilmProperties filmProperties)
                        Save(filmProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is PalletLabelProperties labelProperties)
                        Save(labelProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is TruckProperties truckProperties)
                        Save(truckProperties, xmlItemPropertiesElt, xmlDoc);
                    else if (itemProperties is PackProperties packProperties)
                    { }
                    else
                        _log.Warn($"Type {itemProperties.Name}({itemProperties.GetType()}) not saved!");
                }
                foreach (ItemBase itemProperties in _typeList)
                {
                    if (itemProperties is PackProperties packProperties)
                        Save(packProperties, xmlItemPropertiesElt, xmlDoc);
                }
                // create Analyses element
                XmlElement xmlAnalysesElt = xmlDoc.CreateElement("Analyses");
                xmlRootElement.AppendChild(xmlAnalysesElt);
                foreach (var analysis in Analyses)
                {
                    if (analysis is AnalysisHomo analysisHomo)
                        SaveAnalysis(analysisHomo, xmlAnalysesElt, xmlDoc);
                    else if (analysis is AnalysisPalletsOnPallet analysisPalletsOnPallet)
                        SaveAnalysis(analysisPalletsOnPallet, xmlAnalysesElt, xmlDoc);
                }
                XmlElement xmlHAnalysesElt = xmlDoc.CreateElement("HAnalyses");
                xmlRootElement.AppendChild(xmlHAnalysesElt);
                foreach (AnalysisHetero analysis in HAnalyses)
                {
                    try
                    {
                        SaveHAnalysis(analysis, xmlHAnalysesElt, xmlDoc);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message);
                    }
                }
                // finally save XmlDocument
                xmlDoc.Save(filePath);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Saving types
        public void Save(BoxProperties boxProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlBoxProperties element
            XmlElement eltBoxProperties = xmlDoc.CreateElement("BoxProperties");
            parentElement.AppendChild(eltBoxProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = boxProperties.ID.IGuid.ToString();
            eltBoxProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = boxProperties.ID.Name;
            eltBoxProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = boxProperties.ID.Description;
            eltBoxProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.Length);
            eltBoxProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.Width);
            eltBoxProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Height");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.Height);
            eltBoxProperties.Attributes.Append(heightAttribute);
            // inside dimensions
            if (boxProperties.HasInsideDimensions)
            {
                // length
                XmlAttribute insideLengthAttribute = xmlDoc.CreateAttribute("InsideLength");
                insideLengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.InsideLength);
                eltBoxProperties.Attributes.Append(insideLengthAttribute);
                // width
                XmlAttribute insideWidthAttribute = xmlDoc.CreateAttribute("InsideWidth");
                insideWidthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.InsideWidth);
                eltBoxProperties.Attributes.Append(insideWidthAttribute);
                // height
                XmlAttribute insideHeightAttribute = xmlDoc.CreateAttribute("InsideHeight");
                insideHeightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.InsideHeight);
                eltBoxProperties.Attributes.Append(insideHeightAttribute);
            }
            // bulge
            if (boxProperties.HasBulge)
            {
                // bulge
                XmlAttribute bulgeAttribute = xmlDoc.CreateAttribute("Bulge");
                bulgeAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.Bulge.ToString());
                eltBoxProperties.Attributes.Append(bulgeAttribute);
            }
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.Weight);
            eltBoxProperties.Attributes.Append(weightAttribute);
            // net weight
            XmlAttribute netWeightAttribute = xmlDoc.CreateAttribute("NetWeight");
            netWeightAttribute.Value = boxProperties.NetWeight.ToString();
            eltBoxProperties.Attributes.Append(netWeightAttribute);
            // CAType
            XmlAttribute caTypeAttribute = xmlDoc.CreateAttribute("CAType");
            caTypeAttribute.Value = boxProperties.CAType.ToString();
            eltBoxProperties.Attributes.Append(caTypeAttribute);
            // colors
            SaveColors(boxProperties.Colors, eltBoxProperties, xmlDoc);
            // texture
            SaveTextures(boxProperties.TextureList, eltBoxProperties, xmlDoc);
            // tape
            XmlAttribute tapeAttribute = xmlDoc.CreateAttribute("ShowTape");
            tapeAttribute.Value = string.Format("{0}", boxProperties.TapeWidth.Activated);
            eltBoxProperties.Attributes.Append(tapeAttribute);
            if (boxProperties.TapeWidth.Activated)
            {
                XmlElement tapeElt = xmlDoc.CreateElement("Tape");
                eltBoxProperties.AppendChild(tapeElt);

                XmlAttribute tapeWidthAttribute = xmlDoc.CreateAttribute("TapeWidth");
                tapeWidthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.TapeWidth.Value);
                tapeElt.Attributes.Append(tapeWidthAttribute);

                XmlAttribute tapeColorAttribute = xmlDoc.CreateAttribute("TapeColor");
                tapeColorAttribute.Value = $"{boxProperties.TapeColor.ToArgb()}";
                tapeElt.Attributes.Append(tapeColorAttribute);
            }
            // strappers
            SaveStrappers(boxProperties.StrapperSet, eltBoxProperties, xmlDoc);
            // Facing
            XmlAttribute facingAttribute = xmlDoc.CreateAttribute("Facing");
            facingAttribute.Value = $"{boxProperties.Facing}";
            eltBoxProperties.Attributes.Append(facingAttribute);
            // COG
            XmlAttribute cogAttribute = xmlDoc.CreateAttribute("COG");
            cogAttribute.Value = boxProperties.COG.ToString();
            eltBoxProperties.Attributes.Append(cogAttribute);
        }
        public void Save(BagProperties bagProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlBagProperties element
            XmlElement eltBagProperties = xmlDoc.CreateElement("BagProperties");
            parentElement.AppendChild(eltBagProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = bagProperties.ID.IGuid.ToString();
            eltBagProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = bagProperties.ID.Name;
            eltBagProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = bagProperties.ID.Description;
            eltBagProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bagProperties.Length);
            eltBagProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bagProperties.Width);
            eltBagProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Height");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bagProperties.Height);
            eltBagProperties.Attributes.Append(heightAttribute);
            // radius
            XmlAttribute radiusAttribute = xmlDoc.CreateAttribute("Radius");
            radiusAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bagProperties.Radius);
            eltBagProperties.Attributes.Append(radiusAttribute);
            // bulge
            if (bagProperties.HasBulge)
            {
                XmlAttribute bulgeAttrib = xmlDoc.CreateAttribute("Bulge");
                bulgeAttrib.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bagProperties.Bulge);
                eltBagProperties.Attributes.Append(bulgeAttrib);
            }
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bagProperties.Weight);
            eltBagProperties.Attributes.Append(weightAttribute);
            // net weight
            XmlAttribute netWeightAttribute = xmlDoc.CreateAttribute("NetWeight");
            netWeightAttribute.Value = bagProperties.NetWeight.ToString();
            eltBagProperties.Attributes.Append(netWeightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = $"{bagProperties.ColorFill.ToArgb()}";
            eltBagProperties.Attributes.Append(colorAttribute);
        }
        public void Save(PackProperties packProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlPackProperties element
            XmlElement eltPackProperties = xmlDoc.CreateElement("PackProperties");
            parentElement.AppendChild(eltPackProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = packProperties.ID.IGuid.ToString();
            eltPackProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = packProperties.ID.Name;
            eltPackProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = packProperties.ID.Description;
            eltPackProperties.Attributes.Append(descAttribute);
            // boxProperties
            XmlAttribute boxPropAttribute = xmlDoc.CreateAttribute("BoxProperties");
            boxPropAttribute.Value = packProperties.Content.ID.IGuid.ToString();
            eltPackProperties.Attributes.Append(boxPropAttribute);
            // box orientation
            XmlAttribute orientationAttribute = xmlDoc.CreateAttribute("Orientation");
            orientationAttribute.Value = HalfAxis.ToString( packProperties.BoxOrientation );
            eltPackProperties.Attributes.Append(orientationAttribute);
            // arrangement
            XmlAttribute arrAttribute = xmlDoc.CreateAttribute("Arrangement");
            arrAttribute.Value = packProperties.Arrangement.ToString();
            eltPackProperties.Attributes.Append(arrAttribute);
            // revSolidLayout
            if (packProperties.Content is RevSolidProperties)
            {
                XmlAttribute revSolidLayoutAttrib = xmlDoc.CreateAttribute("CylLayout");
                revSolidLayoutAttrib.Value = packProperties.RevSolidLayout.ToString();
                eltPackProperties.Attributes.Append(revSolidLayoutAttrib);
            }
            // bulge
            if (packProperties.HasBulge)
            {
                XmlAttribute bulgeAttrib = xmlDoc.CreateAttribute("Bulge");
                bulgeAttrib.Value = packProperties.Bulge.ToString();
                eltPackProperties.Attributes.Append(bulgeAttrib);
            }
            // wrapper
            PackWrapper packWrapper = packProperties.Wrap;
            if (null != packWrapper)
            {
                XmlElement wrapperElt = xmlDoc.CreateElement("Wrapper");
                eltPackProperties.AppendChild(wrapperElt);
                if (packWrapper is WrapperPolyethilene wrapperPoly)
                    SaveWrapper(wrapperPoly, wrapperElt, xmlDoc);
                else if (packWrapper is WrapperPaper wrapperPaper)
                    SaveWrapper(wrapperPaper, wrapperElt, xmlDoc);
                else if (packWrapper is WrapperCardboard wrapperCardboat)
                   SaveWrapper(wrapperCardboat, wrapperElt, xmlDoc);
            }
            // tray
            PackTray packTray = packProperties.Tray;
            if (null != packTray)
            {
                XmlElement trayElt = xmlDoc.CreateElement("Tray");
                eltPackProperties.AppendChild(trayElt);
                SaveTray(packTray, trayElt, xmlDoc);
            }
            // strapper set
            SaveStrappers(packProperties.StrapperSet, eltPackProperties, xmlDoc);
            // outer dimensions
            if (packProperties.HasForcedOuterDimensions)
            {
                XmlAttribute outerDimAttribute = xmlDoc.CreateAttribute("OuterDimensions");
                outerDimAttribute.Value = packProperties.OuterDimensions.ToString();
                eltPackProperties.Attributes.Append(outerDimAttribute);
            }
        }
        public void Save(CylinderProperties cylinderProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create "CylinderProperties" element
            XmlElement eltCylProperties = xmlDoc.CreateElement("CylinderProperties");
            parentElement.AppendChild(eltCylProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = cylinderProperties.ID.IGuid.ToString();
            eltCylProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = cylinderProperties.ID.Name;
            eltCylProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = cylinderProperties.ID.Description;
            eltCylProperties.Attributes.Append(descAttribute);
            // radius outer
            XmlAttribute radiusOuterAttribute = xmlDoc.CreateAttribute("RadiusOuter");
            radiusOuterAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylinderProperties.RadiusOuter);
            eltCylProperties.Attributes.Append(radiusOuterAttribute);
            // radius inner
            XmlAttribute radiusInnerAttribute = xmlDoc.CreateAttribute("RadiusInner");
            radiusInnerAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylinderProperties.RadiusInner);
            eltCylProperties.Attributes.Append(radiusInnerAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Height");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylinderProperties.Height);
            eltCylProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylinderProperties.Weight);
            eltCylProperties.Attributes.Append(weightAttribute);
            // net weight
            XmlAttribute netWeightAttribute = xmlDoc.CreateAttribute("NetWeight");
            netWeightAttribute.Value = cylinderProperties.NetWeight.ToString();
            eltCylProperties.Attributes.Append(netWeightAttribute);
            // colorTop
            XmlAttribute topAttribute = xmlDoc.CreateAttribute("ColorTop");
            topAttribute.Value = $"{cylinderProperties.ColorTop.ToArgb()}";
            eltCylProperties.Attributes.Append(topAttribute);
            // colorWall
            XmlAttribute outerWallAttribute = xmlDoc.CreateAttribute("ColorWallOuter");
            outerWallAttribute.Value = $"{cylinderProperties.ColorWallOuter.ToArgb()}";
            eltCylProperties.Attributes.Append(outerWallAttribute);
            // color inner wall
            XmlAttribute innerWallAttribute = xmlDoc.CreateAttribute("ColorWallInner");
            innerWallAttribute.Value =$"{cylinderProperties.ColorWallInner.ToArgb()}";
            eltCylProperties.Attributes.Append(innerWallAttribute);
        }
        public void Save(BottleProperties bottleProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create element "BottleProperties"
            XmlElement eltBottleProperties = xmlDoc.CreateElement("BottleProperties");
            parentElement.AppendChild(eltBottleProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = bottleProperties.ID.IGuid.ToString();
            eltBottleProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = bottleProperties.ID.Name;
            eltBottleProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = bottleProperties.ID.Description;
            eltBottleProperties.Attributes.Append(descAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = $"{bottleProperties.Color.ToArgb()}";
            eltBottleProperties.Attributes.Append(colorAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bottleProperties.Weight);
            eltBottleProperties.Attributes.Append(weightAttribute);
            // netWeight
            XmlAttribute netWeightAttribute = xmlDoc.CreateAttribute("NetWeight");
            netWeightAttribute.Value = bottleProperties.NetWeight.ToString();
            eltBottleProperties.Attributes.Append(netWeightAttribute);
            // create element "Profile"
            var eltProfile = xmlDoc.CreateElement("Profile");
            eltBottleProperties.AppendChild(eltProfile);
            // create element "HeightDiameter"
            foreach (var hd in bottleProperties.Profile)
            {
                var eltHd = xmlDoc.CreateElement("HeightDiameter");
                eltProfile.AppendChild(eltHd);

                XmlAttribute heightDiamAttribute = xmlDoc.CreateAttribute("value");
                heightDiamAttribute.Value = hd.ToString();
                eltHd.Attributes.Append(heightDiamAttribute);
            }
        }
        public void Save(PalletProperties palletProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlPalletProperties element
            XmlElement eltPalletProperties = xmlDoc.CreateElement("PalletProperties");
            parentElement.AppendChild(eltPalletProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = palletProperties.ID.IGuid.ToString();
            eltPalletProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = palletProperties.ID.Name;
            eltPalletProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = palletProperties.ID.Description;
            eltPalletProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.Length);
            eltPalletProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.Width);
            eltPalletProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Height");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.Height);
            eltPalletProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.Weight);
            eltPalletProperties.Attributes.Append(weightAttribute);
            // admissible load weight
            XmlAttribute admLoadWeightAttribute = xmlDoc.CreateAttribute("AdmissibleLoadWeight");
            admLoadWeightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.AdmissibleLoadWeight);
            eltPalletProperties.Attributes.Append(admLoadWeightAttribute);
            // admissible load height
            XmlAttribute admLoadHeightAttribute = xmlDoc.CreateAttribute("AdmissibleLoadHeight");
            admLoadHeightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.AdmissibleLoadHeight);
            eltPalletProperties.Attributes.Append(admLoadHeightAttribute);
            // type
            XmlAttribute typeAttribute = xmlDoc.CreateAttribute("Type");
            typeAttribute.Value = $"{palletProperties.TypeName}";
            eltPalletProperties.Attributes.Append(typeAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = $"{palletProperties.Color.ToArgb()}";
            eltPalletProperties.Attributes.Append(colorAttribute);
        }
        public void Save(InterlayerProperties interlayerProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlPalletProperties element
            XmlElement eltInterlayerProperties = xmlDoc.CreateElement("InterlayerProperties");
            parentElement.AppendChild(eltInterlayerProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = interlayerProperties.ID.IGuid.ToString();
            eltInterlayerProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = interlayerProperties.ID.Name;
            eltInterlayerProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = interlayerProperties.ID.Description;
            eltInterlayerProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", interlayerProperties.Length);
            eltInterlayerProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", interlayerProperties.Width);
            eltInterlayerProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Thickness");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", interlayerProperties.Thickness);
            eltInterlayerProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", interlayerProperties.Weight);
            eltInterlayerProperties.Attributes.Append(weightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = $"{interlayerProperties.Color.ToArgb()}";
            eltInterlayerProperties.Attributes.Append(colorAttribute);
        }

        public void Save(PalletCornerProperties cornerProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create PalletCornerProperties element
            XmlElement eltCornerProperties = xmlDoc.CreateElement("PalletCornerProperties");
            parentElement.AppendChild(eltCornerProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = cornerProperties.ID.IGuid.ToString();
            eltCornerProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = cornerProperties.ID.Name;
            eltCornerProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = cornerProperties.ID.Description;
            eltCornerProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cornerProperties.Length);
            eltCornerProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cornerProperties.Width);
            eltCornerProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Thickness");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cornerProperties.Thickness);
            eltCornerProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cornerProperties.Weight);
            eltCornerProperties.Attributes.Append(weightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value =$"{cornerProperties.Color.ToArgb()}";
            eltCornerProperties.Attributes.Append(colorAttribute);
        }

        public void Save(PalletCapProperties capProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create PalletCornerProperties element
            XmlElement eltCapProperties = xmlDoc.CreateElement("PalletCapProperties");
            parentElement.AppendChild(eltCapProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = capProperties.ID.IGuid.ToString();
            eltCapProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = capProperties.ID.Name;
            eltCapProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = capProperties.ID.Description;
            eltCapProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Length);
            eltCapProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Width);
            eltCapProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Height");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Height);
            eltCapProperties.Attributes.Append(heightAttribute);
            // inside length
            XmlAttribute insideLengthAttribute = xmlDoc.CreateAttribute("InsideLength");
            insideLengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Length);
            eltCapProperties.Attributes.Append(insideLengthAttribute);
            // inside width
            XmlAttribute insideWidthAttribute = xmlDoc.CreateAttribute("InsideWidth");
            insideWidthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Width);
            eltCapProperties.Attributes.Append(insideWidthAttribute);
            // inside height
            XmlAttribute insideHeightAttribute = xmlDoc.CreateAttribute("InsideHeight");
            insideHeightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Height);
            eltCapProperties.Attributes.Append(insideHeightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Weight);
            eltCapProperties.Attributes.Append(weightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = $"{capProperties.Color.ToArgb()}";
            eltCapProperties.Attributes.Append(colorAttribute); 
        }

        public void Save(PalletFilmProperties filmProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create PalletFilmProperties element
            XmlElement eltFilmProperties = xmlDoc.CreateElement("PalletFilmProperties");
            parentElement.AppendChild(eltFilmProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = filmProperties.ID.IGuid.ToString();
            eltFilmProperties.Attributes.Append(guidAttribute);
            // Name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = filmProperties.ID.Name;
            eltFilmProperties.Attributes.Append(nameAttribute);
            // Description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = filmProperties.ID.Description;
            eltFilmProperties.Attributes.Append(descAttribute);
            // Transparency
            XmlAttribute transparencyAttribute = xmlDoc.CreateAttribute("Transparency");
            transparencyAttribute.Value = filmProperties.UseTransparency.ToString();
            eltFilmProperties.Attributes.Append(transparencyAttribute);
            // Hatching
            XmlAttribute hatchingAttribute = xmlDoc.CreateAttribute("Hatching");
            hatchingAttribute.Value = filmProperties.UseHatching.ToString();
            eltFilmProperties.Attributes.Append(hatchingAttribute);
            // HatchSpacing
            XmlAttribute hatchSpacingAttribute = xmlDoc.CreateAttribute("HatchSpacing");
            hatchSpacingAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", filmProperties.HatchSpacing);
            eltFilmProperties.Attributes.Append(hatchSpacingAttribute);
            // HatchAngle
            XmlAttribute hatchAngleAttribute = xmlDoc.CreateAttribute("HatchAngle");
            hatchAngleAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", filmProperties.HatchAngle);
            eltFilmProperties.Attributes.Append(hatchAngleAttribute);
            // Color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = $"{filmProperties.Color.ToArgb()}";
            eltFilmProperties.Attributes.Append(colorAttribute);
            // Weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", filmProperties.Weight);
            eltFilmProperties.Attributes.Append(weightAttribute);
        }

        public void Save(PalletLabelProperties palletLabelProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create element
            XmlElement eltPalletLabelProperties = xmlDoc.CreateElement("PalletLabelProperties");
            parentElement.AppendChild(eltPalletLabelProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = palletLabelProperties.ID.IGuid.ToString();
            eltPalletLabelProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = palletLabelProperties.ID.Name;
            eltPalletLabelProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = palletLabelProperties.ID.Description;
            eltPalletLabelProperties.Attributes.Append(descAttribute);
            // dimensions
            XmlAttribute dimAttribute = xmlDoc.CreateAttribute("Dimensions");
            dimAttribute.Value = palletLabelProperties.Dimensions.ToString();
            eltPalletLabelProperties.Attributes.Append(dimAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletLabelProperties.Weight);
            eltPalletLabelProperties.Attributes.Append(weightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = $"{palletLabelProperties.Color.ToArgb()}";
            eltPalletLabelProperties.Attributes.Append(colorAttribute);
            // bitmap
            XmlAttribute bitmapAttribute = xmlDoc.CreateAttribute("Bitmap");
            bitmapAttribute.Value = BitmapToString(palletLabelProperties.Bitmap);
            eltPalletLabelProperties.Attributes.Append(bitmapAttribute);
        }
        public void Save(BundleProperties bundleProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlPalletProperties element
            XmlElement eltBundleProperties = xmlDoc.CreateElement("BundleProperties");
            parentElement.AppendChild(eltBundleProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = bundleProperties.ID.IGuid.ToString();
            eltBundleProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = bundleProperties.ID.Name;
            eltBundleProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = bundleProperties.ID.Description;
            eltBundleProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bundleProperties.Length);
            eltBundleProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bundleProperties.Width);
            eltBundleProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("UnitThickness");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bundleProperties.UnitThickness);
            eltBundleProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("UnitWeight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bundleProperties.UnitWeight);
            eltBundleProperties.Attributes.Append(weightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = $"{bundleProperties.Color.ToArgb()}";
            eltBundleProperties.Attributes.Append(colorAttribute);
            // numberFlats
            XmlAttribute numberFlatsAttribute = xmlDoc.CreateAttribute("NumberFlats");
            numberFlatsAttribute.Value = string.Format("{0}", bundleProperties.NoFlats);
            eltBundleProperties.Attributes.Append(numberFlatsAttribute);
        }

        public void Save(TruckProperties truckProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlPalletProperties element
            XmlElement eltTruckProperties = xmlDoc.CreateElement("TruckProperties");
            parentElement.AppendChild(eltTruckProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = truckProperties.ID.IGuid.ToString();
            eltTruckProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = truckProperties.ID.Name;
            eltTruckProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = truckProperties.ID.Description;
            eltTruckProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", truckProperties.Length);
            eltTruckProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", truckProperties.Width);
            eltTruckProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Height");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", truckProperties.Height);
            eltTruckProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("AdmissibleLoadWeight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", truckProperties.AdmissibleLoadWeight);
            eltTruckProperties.Attributes.Append(weightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = string.Format("{0}", truckProperties.Color.ToArgb());
            eltTruckProperties.Attributes.Append(colorAttribute);
        }
        #endregion

        #region Save helpers
        private void SaveInt(int i, XmlDocument xmlDoc, XmlElement xmlElement, string attributeName)
        {
            XmlAttribute att = xmlDoc.CreateAttribute(attributeName);
            att.Value = i.ToString();
            xmlElement.Attributes.Append(att);
        }
        private void SaveDouble(double d, XmlDocument xmlDoc, XmlElement xmlElement, string attributeName)
        {
            XmlAttribute att = xmlDoc.CreateAttribute(attributeName);
            att.Value = string.Format(CultureInfo.InvariantCulture, "{0}", d);
            xmlElement.Attributes.Append(att);
        }
        private void SaveOptDouble(OptDouble optD, XmlDocument xmlDoc, XmlElement xmlElement, string attributeName)
        {
            XmlAttribute att = xmlDoc.CreateAttribute(attributeName);
            att.Value = optD.ToString();
            xmlElement.Attributes.Append(att);
        }
        private void SaveTextures(List<Pair<HalfAxis.HAxis, Texture>> textureList, XmlElement xmlBoxProperties, XmlDocument xmlDoc)
        { 
            XmlElement xmlTexturesElement = xmlDoc.CreateElement("Textures");
            xmlBoxProperties.AppendChild(xmlTexturesElement);
            foreach (Pair<HalfAxis.HAxis, Texture> texPair in textureList)
            {
                XmlElement xmlFaceTexture = xmlDoc.CreateElement("FaceTexture");
                xmlTexturesElement.AppendChild(xmlFaceTexture);
                // face index
                XmlAttribute xmlFaceNormal = xmlDoc.CreateAttribute("FaceNormal");
                xmlFaceNormal.Value = HalfAxis.ToString(texPair.first);
                xmlFaceTexture.Attributes.Append(xmlFaceNormal);
                // texture position
                XmlAttribute xmlPosition = xmlDoc.CreateAttribute("Position");
                xmlPosition.Value = texPair.second.Position.ToString();
                xmlFaceTexture.Attributes.Append(xmlPosition);
                // texture size
                XmlAttribute xmlSize = xmlDoc.CreateAttribute("Size");
                xmlSize.Value = texPair.second.Size.ToString();
                xmlFaceTexture.Attributes.Append(xmlSize);
                // angle
                XmlAttribute xmlAngle = xmlDoc.CreateAttribute("Angle");
                xmlAngle.Value = string.Format(CultureInfo.InvariantCulture, "{0}", texPair.second.Angle);
                xmlFaceTexture.Attributes.Append(xmlAngle);
                // bitmap
                XmlAttribute xmlBitmap = xmlDoc.CreateAttribute("Bitmap");
                xmlBitmap.Value = Document.BitmapToString(texPair.second.Bitmap);
                xmlFaceTexture.Attributes.Append(xmlBitmap);
            }
        }

        private void SaveColors(Color[] colors, XmlElement eltBoxProperties, XmlDocument xmlDoc)
        { 
            // face colors
            XmlElement xmlFaceColors = xmlDoc.CreateElement("FaceColors");
            eltBoxProperties.AppendChild(xmlFaceColors);
            short i = 0;
            foreach (Color color in colors)
            {
                XmlElement xmlFaceColor = xmlDoc.CreateElement("FaceColor");
                xmlFaceColors.AppendChild(xmlFaceColor);
                // face index
                XmlAttribute xmlFaceIndex = xmlDoc.CreateAttribute("FaceIndex");
                xmlFaceIndex.Value = string.Format("{0}", i);
                xmlFaceColor.Attributes.Append(xmlFaceIndex);
                // color
                XmlAttribute xmlColor = xmlDoc.CreateAttribute("Color");
                xmlColor.Value = $"{color.ToArgb()}";
                xmlFaceColor.Attributes.Append(xmlColor);
                ++i;
            }
        }
        #endregion

        #region SaveStrappers
        private void SaveStrappers(StrapperSet strapperSet, XmlElement eltParent, XmlDocument xmlDoc)
        {
            // strapperSet
            XmlElement eltStrapperSet = xmlDoc.CreateElement("StrapperSet");
            eltParent.AppendChild(eltStrapperSet);
            // strapperWidth
            XmlAttribute xmlWidth = xmlDoc.CreateAttribute("Width");
            xmlWidth.Value = string.Format(CultureInfo.InvariantCulture, "{0}", strapperSet.Width);
            eltStrapperSet.Attributes.Append(xmlWidth);
            // strapperColor
            XmlAttribute xmlColor = xmlDoc.CreateAttribute("Color");
            xmlColor.Value = $"{strapperSet.Color.ToArgb()}";
            eltStrapperSet.Attributes.Append(xmlColor);
            // evenly spaced strappers in directions 0/1/2
            for (int i = 0; i < 3; ++i)
            {
                if (strapperSet.GetNumber(i) > 0)
                {
                    XmlElement eltStrapper = xmlDoc.CreateElement("EvenlySpacedStrappers");
                    eltStrapperSet.AppendChild(eltStrapper);
                    // dim
                    XmlAttribute attDir = xmlDoc.CreateAttribute("Dir");
                    attDir.Value = $"{i}";
                    eltStrapper.Attributes.Append(attDir);
                    // number
                    XmlAttribute attNumber = xmlDoc.CreateAttribute("Number");
                    attNumber.Value = $"{strapperSet.GetNumber(i)}";
                    eltStrapper.Attributes.Append(attNumber);
                    // spacing
                    if (strapperSet.GetSpacing(i).Activated)
                    {
                        XmlAttribute attSpacing = xmlDoc.CreateAttribute("Spacing");
                        attSpacing.Value = string.Format(CultureInfo.InvariantCulture, "{0}", strapperSet.GetSpacing(i).Value);
                        eltStrapper.Attributes.Append(attSpacing);
                    }
                }
            }
        }
        private void SavePalletLabelInstances(List<PalletLabelInst> palletLabelInstances, XmlElement eltParent, XmlDocument xmlDoc)
        {
            // PalletLabelInst
            XmlElement eltPalletLabelInstances = xmlDoc.CreateElement("PalletLabelInstances");
            eltParent.AppendChild(eltPalletLabelInstances);
            foreach (var pli in palletLabelInstances)
            {
                XmlElement eltPli = xmlDoc.CreateElement("PalletLabelInst");
                eltPalletLabelInstances.AppendChild(eltPli);
                // Id
                XmlAttribute attId = xmlDoc.CreateAttribute("Id");
                attId.Value = pli.PalletLabelProperties.ID.IGuid.ToString();
                eltPli.Attributes.Append(attId);
                // position
                XmlAttribute attPosition = xmlDoc.CreateAttribute("Position");
                attPosition.Value = pli.Position.ToString();
                eltPli.Attributes.Append(attPosition);
                // side
                XmlAttribute attAxis = xmlDoc.CreateAttribute("Side");
                attAxis.Value = HalfAxis.ToString(pli.Side);
                eltPli.Attributes.Append(attAxis);
            }
        }
        private void SaveConveyorSettings(List<ConveyorSetting> conveyorSettings, XmlElement xmlAnalysisElt, XmlDocument xmlDoc)
        {
            try
            {
                // create conveyor settings elt
                XmlElement eltConveyorSettings = xmlDoc.CreateElement("ConveyorSettings");
                xmlAnalysisElt.AppendChild(eltConveyorSettings);

                foreach (var cs in conveyorSettings)
                {
                    XmlElement eltConveyorSetting = xmlDoc.CreateElement("ConveyorSetting");
                    eltConveyorSettings.AppendChild(eltConveyorSetting);
                    // angle
                    XmlAttribute attAngle = xmlDoc.CreateAttribute("Angle");
                    attAngle.Value = $"{cs.Angle}";
                    eltConveyorSetting.Attributes.Append(attAngle);
                    // number
                    XmlAttribute attNumber = xmlDoc.CreateAttribute("Number");
                    attNumber.Value = $"{cs.Number}";
                    eltConveyorSetting.Attributes.Append(attNumber);
                    // angle gripper
                    XmlAttribute attGripperAngle = xmlDoc.CreateAttribute("GripperAngle");
                    attGripperAngle.Value = $"{cs.GripperAngle}";
                    eltConveyorSetting.Attributes.Append(attGripperAngle);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private void SaveRobotPreparation(RobotPreparation robotPreparation, XmlElement xmlAnalysisElt, XmlDocument xmlDoc)
        {
            try
            {
                // create robot preparation elt
                XmlElement eltRobotPreparation = xmlDoc.CreateElement("RobotPreparation");
                xmlAnalysisElt.AppendChild(eltRobotPreparation);

                // layer types
                XmlElement eltLayerTypes = xmlDoc.CreateElement("LayerTypes");
                foreach (var robotLayer in robotPreparation.LayerTypes)
                {
                    
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region Save Wrappers
        private void SaveWrapperBase(PackWrapper wrapper, XmlElement wrapperElt, XmlDocument xmlDoc)
        {
            if (null == wrapper) return;
            // type
            XmlAttribute typeAttrib = xmlDoc.CreateAttribute("Type");
            typeAttrib.Value = wrapper.Type.ToString();
            wrapperElt.Attributes.Append(typeAttrib);
            // color
            XmlAttribute colorAttrib = xmlDoc.CreateAttribute("Color");
            colorAttrib.Value =$"{wrapper.Color.ToArgb()}";
            wrapperElt.Attributes.Append(colorAttrib);
            // weight
            XmlAttribute weightAttrib = xmlDoc.CreateAttribute("Weight");
            weightAttrib.Value = string.Format(CultureInfo.InvariantCulture, "{0}", wrapper.Weight);
            wrapperElt.Attributes.Append(weightAttrib);
            // thickness
            XmlAttribute thicknessAttrib = xmlDoc.CreateAttribute("UnitThickness");
            thicknessAttrib.Value = string.Format(CultureInfo.InvariantCulture, "{0}", wrapper.UnitThickness);
            wrapperElt.Attributes.Append(thicknessAttrib);
        }
        private void SaveWrapper(WrapperPolyethilene wrapper, XmlElement wrapperElt, XmlDocument xmlDoc)
        {
            if (null == wrapper) return;
            SaveWrapperBase(wrapper, wrapperElt, xmlDoc);
            // transparency
            XmlAttribute transparentAttrib = xmlDoc.CreateAttribute("Transparent");
            transparentAttrib.Value = wrapper.Transparent.ToString();
            wrapperElt.Attributes.Append(transparentAttrib);
        }
        private void SaveWrapper(WrapperPaper wrapper, XmlElement wrapperElt, XmlDocument xmlDoc)
        {
            if (null == wrapper) return;
            SaveWrapperBase(wrapper, wrapperElt, xmlDoc);
        }
        private void SaveWrapper(WrapperCardboard wrapper, XmlElement wrapperElt, XmlDocument xmlDoc)
        {
            if (null == wrapper) return;
            SaveWrapperBase(wrapper, wrapperElt, xmlDoc);
            // wall distribution
            XmlAttribute wallDistribAttrib = xmlDoc.CreateAttribute("NumberOfWalls");
            wallDistribAttrib.Value = string.Format("{0} {1} {2}", wrapper.Wall(0), wrapper.Wall(1), wrapper.Wall(2));
            wrapperElt.Attributes.Append(wallDistribAttrib);
        }
        private void SaveTray(PackTray tray, XmlElement trayElt, XmlDocument xmlDoc)
        {
            if (null == tray) return;
            // color
            XmlAttribute colorAttrib = xmlDoc.CreateAttribute("Color");
            colorAttrib.Value = $"{tray.Color.ToArgb()}";
            trayElt.Attributes.Append(colorAttrib);
            // height
            XmlAttribute heightAttrib = xmlDoc.CreateAttribute("Height");
            heightAttrib.Value = string.Format(CultureInfo.InvariantCulture, "{0}", tray.Height);
            trayElt.Attributes.Append(heightAttrib);
            // weight
            XmlAttribute weightAttrib = xmlDoc.CreateAttribute("Weight");
            weightAttrib.Value = string.Format(CultureInfo.InvariantCulture, "{0}", tray.Weight);
            trayElt.Attributes.Append(weightAttrib);
            // wall distribution
            XmlAttribute wallDistribAttrib = xmlDoc.CreateAttribute("NumberOfWalls");
            wallDistribAttrib.Value = string.Format("{0} {1} {2}", tray.Wall(0), tray.Wall(1), tray.Wall(2));
            trayElt.Attributes.Append(wallDistribAttrib);
            // unit thickness
            XmlAttribute unitThicknessAttrib = xmlDoc.CreateAttribute("UnitThickness");
            unitThicknessAttrib.Value = string.Format(CultureInfo.InvariantCulture, "{0}", tray.UnitThickness);
            trayElt.Attributes.Append(unitThicknessAttrib);
        }
        #endregion

        #region Save analysis
        private string AnalysisTypeName(Analysis analysis)
        {
            if (analysis is AnalysisBoxCase) return "AnalysisBoxCase";
            else if (analysis is AnalysisCylinderCase) return "AnalysisCylinderCase";
            else if (analysis is AnalysisCylinderPallet) return "AnalysisCylinderPallet";
            else if (analysis is AnalysisHCylCase) return "AnalysisHCylCase";
            else if (analysis is AnalysisCasePallet) return "AnalysisCasePallet";
            else if (analysis is AnalysisHCylPallet) return "AnalysisHCylPallet";
            else if (analysis is AnalysisCaseTruck) return "AnalysisCaseTruck";
            else if (analysis is AnalysisCylinderTruck) return "AnalysisCylinderTruck";
            else if (analysis is AnalysisHCylTruck) return "AnalysisHCylTruck";
            else if (analysis is AnalysisPalletTruck) return "AnalysisPalletTruck";
            else if (analysis is AnalysisPalletsOnPallet) return "AnalysisPalletsOnPallet";
            else return TypeDescriptor.GetClassName(analysis.GetType());
        }
        private void SaveAnalysis(AnalysisHomo analysis, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create analysis element
            XmlElement xmlAnalysisElt = xmlDoc.CreateElement(AnalysisTypeName(analysis));
            parentElement.AppendChild(xmlAnalysisElt);
            // guid
            XmlAttribute analysisGuidAttribute = xmlDoc.CreateAttribute("Id");
            analysisGuidAttribute.Value = analysis.ID.IGuid.ToString();
            xmlAnalysisElt.Attributes.Append(analysisGuidAttribute);
            // name
            XmlAttribute analysisNameAttribute = xmlDoc.CreateAttribute("Name");
            analysisNameAttribute.Value = analysis.ID.Name;
            xmlAnalysisElt.Attributes.Append(analysisNameAttribute);
            // description
            XmlAttribute analysisDescriptionAttribute = xmlDoc.CreateAttribute("Description");
            analysisDescriptionAttribute.Value = analysis.ID.Description;
            xmlAnalysisElt.Attributes.Append(analysisDescriptionAttribute);
            // contentId
            XmlAttribute analysisContentId = xmlDoc.CreateAttribute("ContentId");
            analysisContentId.Value = analysis.Content.ID.IGuid.ToString();
            xmlAnalysisElt.Attributes.Append(analysisContentId);
            // containerId
            XmlAttribute analysisContainerId = xmlDoc.CreateAttribute("ContainerId");
            analysisContainerId.Value = analysis.Container.ID.IGuid.ToString();
            xmlAnalysisElt.Attributes.Append(analysisContainerId);
            if (analysis is AnalysisLayered analysisLay)
            {
                // interlayers
                XmlElement eltInterlayers = xmlDoc.CreateElement("Interlayers");
                xmlAnalysisElt.AppendChild(eltInterlayers);
                foreach (InterlayerProperties interlayer in analysisLay.Interlayers)
                {
                    XmlElement eltInterlayer = xmlDoc.CreateElement("Interlayer");
                    eltInterlayer.InnerText = interlayer.ID.IGuid.ToString();
                    eltInterlayers.AppendChild(eltInterlayer);
                }
            }
            if (analysis is AnalysisCasePallet analysisCasePallet1)
            {
                // PalletCornerId
                if (null != analysisCasePallet1.PalletCornerProperties)
                {
                    XmlAttribute palletCornerAttribute = xmlDoc.CreateAttribute("PalletCornerId");
                    palletCornerAttribute.Value = string.Format("{0}", analysisCasePallet1.PalletCornerProperties.ID.IGuid);
                    xmlAnalysisElt.Attributes.Append(palletCornerAttribute);
                }
                // PalletCornerTopId
                if (null != analysisCasePallet1.PalletCornerTopProperties)
                {
                    XmlAttribute palletCornerAttribute = xmlDoc.CreateAttribute("PalletCornerTopId");
                    palletCornerAttribute.Value = string.Format("{0}", analysisCasePallet1.PalletCornerTopProperties.ID.IGuid);
                    xmlAnalysisElt.Attributes.Append(palletCornerAttribute);
                }
                // PalletCapId
                if (null != analysisCasePallet1.PalletCapProperties)
                {
                    XmlAttribute palletCapIdAttribute = xmlDoc.CreateAttribute("PalletCapId");
                    palletCapIdAttribute.Value = string.Format("{0}", analysisCasePallet1.PalletCapProperties.ID.IGuid);
                    xmlAnalysisElt.Attributes.Append(palletCapIdAttribute);
                }
                if (analysisCasePallet1.HasTopInterlayer)
                {
                    var topInterlayerIdAttribute = xmlDoc.CreateAttribute("TopInterlayerId");
                    topInterlayerIdAttribute.Value = string.Format("{0}", analysisCasePallet1.TopInterlayerProperties.ID.IGuid);
                    xmlAnalysisElt.Attributes.Append(topInterlayerIdAttribute);
                }
                // PalletFilmId
                if (null != analysisCasePallet1.PalletFilmProperties)
                {
                    XmlAttribute palletFilmIdAttribute = xmlDoc.CreateAttribute("PalletFilmId");
                    palletFilmIdAttribute.Value = string.Format("{0}", analysisCasePallet1.PalletFilmProperties.ID.IGuid);
                    xmlAnalysisElt.Attributes.Append(palletFilmIdAttribute);

                    XmlAttribute attPalletFilmTopCovering = xmlDoc.CreateAttribute("PalletFilmTopCovering");
                    attPalletFilmTopCovering.Value = analysisCasePallet1.PalletFilmTopCovering.ToString();
                    xmlAnalysisElt.Attributes.Append(attPalletFilmTopCovering);
                }
                // Pallet sleeve
                if (analysisCasePallet1.HasPalletSleeve)
                {
                    XmlAttribute attPalletSleeveColor = xmlDoc.CreateAttribute("PalletSleeveColor");
                    attPalletSleeveColor.Value = $"{analysisCasePallet1.PalletSleeveColor.ToArgb()}";
                    xmlAnalysisElt.Attributes.Append(attPalletSleeveColor);
                }
                // StrapperSet
                SaveStrappers(analysisCasePallet1.StrapperSet, xmlAnalysisElt, xmlDoc);
                // labels
                SavePalletLabelInstances(analysisCasePallet1.PalletLabels, xmlAnalysisElt, xmlDoc);
                // conveyor settings
                SaveConveyorSettings(analysisCasePallet1.ConveyorSettings, xmlAnalysisElt, xmlDoc);
                // robot preparation
                SaveRobotPreparation(analysisCasePallet1.RobotPreparation, xmlAnalysisElt, xmlDoc);
            }

            // constraint set
            ConstraintSetAbstract constraintSet = analysis.ConstraintSet; 
            XmlElement eltContraintSet = xmlDoc.CreateElement("ConstraintSet");
            xmlAnalysisElt.AppendChild(eltContraintSet);
            // allowed orientation
            XmlAttribute attOrientations = xmlDoc.CreateAttribute("Orientations");
            attOrientations.Value = constraintSet.AllowedOrientationsString;
            eltContraintSet.Attributes.Append(attOrientations);
            // maximum weight
            XmlAttribute attMaximumWeight = xmlDoc.CreateAttribute("MaximumWeight");
            attMaximumWeight.Value = constraintSet.OptMaxWeight.ToString();
            eltContraintSet.Attributes.Append(attMaximumWeight);
            // maximum number of items
            XmlAttribute attMaximumNumber = xmlDoc.CreateAttribute("MaximumNumberOfItems");
            attMaximumNumber.Value = constraintSet.OptMaxNumber.ToString();
            eltContraintSet.Attributes.Append(attMaximumNumber);

            if (analysis.ConstraintSet is ConstraintSetPackablePallet constraintSetPackablePallet)
            {
                XmlAttribute attOverhang = xmlDoc.CreateAttribute("Overhang");
                attOverhang.Value = constraintSetPackablePallet.Overhang.ToString();
                eltContraintSet.Attributes.Append(attOverhang);

                XmlAttribute attMaximumHeight = xmlDoc.CreateAttribute("MaximumPalletHeight");
                attMaximumHeight.Value = constraintSetPackablePallet.OptMaxHeight.ToString();
                eltContraintSet.Attributes.Append(attMaximumHeight);

                if (analysis.ConstraintSet is ConstraintSetCasePallet constraintSetCasePallet)
                {
                    XmlAttribute attMinSpace = xmlDoc.CreateAttribute("MinSpace");
                    attMinSpace.Value = constraintSetCasePallet.MinimumSpace.ToString();
                    eltContraintSet.Attributes.Append(attMinSpace);

                    XmlAttribute attPalletFilmTurns = xmlDoc.CreateAttribute("PalletFilmTurns");
                    attPalletFilmTurns.Value = constraintSetCasePallet.PalletFilmTurns.ToString();
                    eltContraintSet.Attributes.Append(attPalletFilmTurns);

                }
            }
            else if (analysis is AnalysisPalletTruck analysisPalletTruck)
            {
                ConstraintSetPalletTruck constraintSetPalletTruck = analysisPalletTruck.ConstraintSet as ConstraintSetPalletTruck;

                XmlAttribute attMinDistanceLoadWall = xmlDoc.CreateAttribute("MinDistanceLoadWall");
                attMinDistanceLoadWall.Value = constraintSetPalletTruck.MinDistanceLoadWall.ToString();
                eltContraintSet.Attributes.Append(attMinDistanceLoadWall);

                XmlAttribute attMinDiastanceLoadRoof = xmlDoc.CreateAttribute("MinDistanceLoadRoof");
                attMinDiastanceLoadRoof.Value = constraintSetPalletTruck.MinDistanceLoadRoof.ToString();
                eltContraintSet.Attributes.Append(attMinDiastanceLoadRoof);

                XmlAttribute attAllowMultipleLayers = xmlDoc.CreateAttribute("AllowMultipleLayers");
                attAllowMultipleLayers.Value = !constraintSetPalletTruck.OptMaxLayerNumber.Activated ? "1" : "0";
                eltContraintSet.Attributes.Append(attAllowMultipleLayers);
            }
            else if (analysis is AnalysisCylinderTruck analysisCylinderTruck)
            {
                ConstraintSetCylinderTruck constraintSetCylinderTruck = analysisCylinderTruck.ConstraintSet as ConstraintSetCylinderTruck;

                XmlAttribute attMinDistanceLoadWall = xmlDoc.CreateAttribute("MinDistanceLoadWall");
                attMinDistanceLoadWall.Value = constraintSetCylinderTruck.MinDistanceLoadWall.ToString();
                eltContraintSet.Attributes.Append(attMinDistanceLoadWall);

                XmlAttribute attMinDistanceLoadRoof = xmlDoc.CreateAttribute("MinDistanceLoadRoof");
                attMinDistanceLoadRoof.Value = constraintSetCylinderTruck.MinDistanceLoadRoof.ToString();
                eltContraintSet.Attributes.Append(attMinDistanceLoadRoof);
            }
            else if (analysis is AnalysisHCylPallet analysisHCylPallet)
            {
                var constraintSetPackableTruck = analysisHCylPallet.ConstraintSet as ConstraintSetPackablePallet;

                XmlAttribute attOverhang = xmlDoc.CreateAttribute("Overhang");
                attOverhang.Value = constraintSetPackableTruck.Overhang.ToString();
                eltContraintSet.Attributes.Append(attOverhang);

                XmlAttribute attMaximumHeight = xmlDoc.CreateAttribute("MaximumPalletHeight");
                attMaximumHeight.Value = constraintSetPackableTruck.OptMaxHeight.ToString();
                eltContraintSet.Attributes.Append(attMaximumHeight);
            }
            else if (analysis is AnalysisHCylTruck analysisHCylTruck)
            {
                var constraintSetPackableTruck = analysisHCylTruck.ConstraintSet as ConstraintSetPackableTruck;

                XmlAttribute attMinDistanceLoadWall = xmlDoc.CreateAttribute("MinDistanceLoadWall");
                attMinDistanceLoadWall.Value = constraintSetPackableTruck.MinDistanceLoadWall.ToString();
                eltContraintSet.Attributes.Append(attMinDistanceLoadWall);

                XmlAttribute attMinDistanceLoadRoof = xmlDoc.CreateAttribute("MinDistanceLoadRoof");
                attMinDistanceLoadRoof.Value = constraintSetPackableTruck.MinDistanceLoadRoof.ToString();
                eltContraintSet.Attributes.Append(attMinDistanceLoadRoof);
            }
            // solution
            if (analysis is AnalysisLayered analysisLay2)
                SaveSolution(analysisLay2.SolutionLay, xmlAnalysisElt, xmlDoc);
            else if (analysis is AnalysisHCyl analysisHCyl)
                SaveSolution(analysisHCyl.Solution as SolutionHCyl, xmlAnalysisElt, xmlDoc);
        }
        private void SaveAnalysis(AnalysisPalletsOnPallet analysis, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create analysis element
            XmlElement xmlAnalysisElt = xmlDoc.CreateElement(AnalysisTypeName(analysis));
            parentElement.AppendChild(xmlAnalysisElt);
            // guid
            XmlAttribute analysisGuidAttribute = xmlDoc.CreateAttribute("Id");
            analysisGuidAttribute.Value = analysis.ID.IGuid.ToString();
            xmlAnalysisElt.Attributes.Append(analysisGuidAttribute);
            // name
            XmlAttribute analysisNameAttribute = xmlDoc.CreateAttribute("Name");
            analysisNameAttribute.Value = analysis.ID.Name;
            xmlAnalysisElt.Attributes.Append(analysisNameAttribute);
            // description
            XmlAttribute analysisDescriptionAttribute = xmlDoc.CreateAttribute("Description");
            analysisDescriptionAttribute.Value = analysis.ID.Description;
            xmlAnalysisElt.Attributes.Append(analysisDescriptionAttribute);
            // containerId
            XmlAttribute analysisContainerId = xmlDoc.CreateAttribute("ContainerId");
            analysisContainerId.Value = analysis.Container.ID.IGuid.ToString();
            xmlAnalysisElt.Attributes.Append(analysisContainerId);
            // masterPalletSplit
            XmlAttribute masterPalletSplit = xmlDoc.CreateAttribute("MasterPalletSplit");
            masterPalletSplit.Value = $"{(int)analysis.MasterPalletSplit}";
            xmlAnalysisElt.Attributes.Append(masterPalletSplit);
            // loadedPalletOrientation
            XmlAttribute loadedPalletOrientation = xmlDoc.CreateAttribute("LoadedPalletOrientation");
            loadedPalletOrientation.Value = $"{(int)analysis.LoadedPalletOrientation}";
            xmlAnalysisElt.Attributes.Append(loadedPalletOrientation);

            // loaded pallets
            for (int i = 0; i < 4; ++i)
            {
                if (null != analysis.PalletAnalyses[i])
                {
                    XmlAttribute palletAttribute = xmlDoc.CreateAttribute($"Pallet{i}");
                    palletAttribute.Value = analysis.PalletAnalyses[i].ParentAnalysis.ID.IGuid.ToString();
                    xmlAnalysisElt.Attributes.Append(palletAttribute);
                }
            }
        }

        private void SaveSolution(SolutionLayered sol, XmlElement parentElement, XmlDocument xmlDoc)
        {
            XmlElement eltSolution = xmlDoc.CreateElement("Solution");
            parentElement.AppendChild(eltSolution);
            // layer encaps
            XmlElement eltLayerEncaps = xmlDoc.CreateElement("LayerDescriptors");
            eltSolution.AppendChild(eltLayerEncaps);
            foreach (var layerEncap in sol.LayerEncaps)
            {
                if (null != layerEncap.LayerDesc)
                {
                    string eltLayerDescName = string.Empty;
                    if (layerEncap.LayerDesc is LayerDescBox)
                        eltLayerDescName = "LayerDescBox";
                    else if (layerEncap.LayerDesc is LayerDescCyl)
                        eltLayerDescName = "LayerDescCyl";
                    else
                        throw new Exception("Unexpected LayerDesc type!");
                    XmlElement eltLayerDesc = xmlDoc.CreateElement(eltLayerDescName);
                    eltLayerDesc.InnerText = layerEncap.ToString();
                    eltLayerEncaps.AppendChild(eltLayerDesc);
                }
                else if (null != layerEncap.Layer2D)
                {
                    if (layerEncap.Layer2D is Layer2DBrickExp layerBrick)
                    {
                        XmlElement eltLayerExpBoxes = xmlDoc.CreateElement("LayerExpBoxes");
                        eltLayerEncaps.AppendChild(eltLayerExpBoxes);
                        // name
                        XmlAttribute attName = xmlDoc.CreateAttribute("Name");
                        attName.Value = layerBrick.Name;
                        eltLayerExpBoxes.Attributes.Append(attName);
                        // dimension box
                        XmlAttribute attDimBox = xmlDoc.CreateAttribute("DimBox");
                        attDimBox.Value = layerBrick.DimBox.ToString();
                        eltLayerExpBoxes.Attributes.Append(attDimBox);
                        // dimensions container
                        XmlAttribute attDimContainer = xmlDoc.CreateAttribute("DimContainer");
                        attDimContainer.Value = layerBrick.DimContainer.ToString();
                        eltLayerExpBoxes.Attributes.Append(attDimContainer);
                        // orientation
                        XmlAttribute attAxisOrtho = xmlDoc.CreateAttribute("AxisOrtho");
                        attAxisOrtho.Value = HalfAxis.ToString(layerBrick.AxisOrtho);
                        eltLayerExpBoxes.Attributes.Append(attAxisOrtho);
                        // positions
                        foreach (var p in layerBrick.Positions)
                            SaveBoxPosition(p, eltLayerExpBoxes, xmlDoc);
                    }
                    else if (layerEncap.Layer2D is Layer2DCylImp layerCyl)
                    {
                        XmlElement eltLayerExpCylinders = xmlDoc.CreateElement("LayerExpCyl");
                        eltLayerEncaps.AppendChild(eltLayerExpCylinders);
                        foreach (var p in layerCyl)
                            SaveCylPosition(p, eltLayerExpCylinders, xmlDoc);
                    }
                    else
                        throw new Exception("Unexpected Cylinder type!");
                }
            }
            // solution items
            XmlElement eltSolutionItems = xmlDoc.CreateElement("SolutionItems");
            eltSolution.AppendChild(eltSolutionItems);
            foreach (SolutionItem solItem in sol.SolutionItems)
            {
                XmlElement eltSolutionItem = xmlDoc.CreateElement("SolutionItem");
                eltSolutionItem.InnerText = solItem.ToString();
                eltSolutionItems.AppendChild(eltSolutionItem);
            }
        }

        private void SaveSolution(SolutionHCyl sol, XmlElement parentElement, XmlDocument xmlDoc)
        {
            XmlElement eltSolution = xmlDoc.CreateElement("Solution");
            parentElement.AppendChild(eltSolution);
            // cylinder layout
            XmlElement eltCylLayout = xmlDoc.CreateElement("CylLayout");
            eltSolution.AppendChild(eltCylLayout);
            // name
            XmlElement eltLayoutDesc = xmlDoc.CreateElement("LayoutDesc");
            eltLayoutDesc.InnerText = sol.Layout.Descriptor;
            eltCylLayout.AppendChild(eltLayoutDesc);
        }

        private void SaveHAnalysis(AnalysisHetero analysis, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // analysis name
            string analysisName = string.Empty;
            if (analysis is HAnalysisPallet) { analysisName = "HAnalysisPallet"; }
            else if (analysis is HAnalysisCase) { analysisName = "HAnalysisCase"; }
            else if (analysis is HAnalysisTruck) { analysisName = "HAnalysisTruck"; }
            else
                throw new Exception($"Unsupported analysis type {analysis.GetType()}");

            // create analysis element
            XmlElement eltAnalysis = xmlDoc.CreateElement(analysisName);
            parentElement.AppendChild(eltAnalysis);
            // guid
            XmlAttribute analysisGuidAttribute = xmlDoc.CreateAttribute("Id");
            analysisGuidAttribute.Value = analysis.ID.IGuid.ToString();
            eltAnalysis.Attributes.Append(analysisGuidAttribute);
            // name
            XmlAttribute analysisNameAttribute = xmlDoc.CreateAttribute("Name");
            analysisNameAttribute.Value = analysis.ID.Name;
            eltAnalysis.Attributes.Append(analysisNameAttribute);
            // description
            XmlAttribute analysisDescriptionAttribute = xmlDoc.CreateAttribute("Description");
            analysisDescriptionAttribute.Value = analysis.ID.Description;
            eltAnalysis.Attributes.Append(analysisDescriptionAttribute);
            // containers
            var eltContainers = xmlDoc.CreateElement("Containers");
            eltAnalysis.AppendChild(eltContainers);
            foreach (var container in analysis.Containers)
            {
                var eltContainer = xmlDoc.CreateElement("Container");
                eltContainers.AppendChild(eltContainer);
                // Id
                var attId = xmlDoc.CreateAttribute("Id");
                attId.Value = container.ID.IGuid.ToString();
                eltContainer.Attributes.Append(attId);
            }
            // contentItems
            XmlElement eltContentItems = xmlDoc.CreateElement("ContentItems");
            eltAnalysis.AppendChild(eltContentItems);
            foreach (ContentItem ci in analysis.Content)
            {
                var eltCI = xmlDoc.CreateElement("ContentItem");
                eltContentItems.AppendChild(eltCI);
                // set PackableId
                var attPackId = xmlDoc.CreateAttribute("PackableId");
                attPackId.Value = ci.Pack.ID.IGuid.ToString();
                eltCI.Attributes.Append(attPackId);
                // set number
                var attNumber = xmlDoc.CreateAttribute("Number");
                attNumber.Value = ci.Number.ToString();
                eltCI.Attributes.Append(attNumber);
                // set content items
                var attOrientations = xmlDoc.CreateAttribute("Orientations");
                attOrientations.Value = string.Join(",", ci.AllowedOrientations);
                eltCI.Attributes.Append(attOrientations);
            }
            // solution
            var eltHSolution = xmlDoc.CreateElement("HSolution");
            eltAnalysis.AppendChild(eltHSolution);
            // set algorithm
            var attAlgo = xmlDoc.CreateAttribute("Algo");
            attAlgo.Value = analysis.Solution.Algorithm;
            eltHSolution.Attributes.Append(attAlgo);
            foreach (var solItem in analysis.Solution.SolItems)
            {
                var eltSolItem = xmlDoc.CreateElement("SolItem");
                eltHSolution.AppendChild(eltSolItem);

                foreach (var contElt in solItem.ContainedElements)
                {
                    var eltContained = xmlDoc.CreateElement("ContentElt");
                    eltSolItem.AppendChild(eltContained);

                    // ContentTypeIndex
                    var attContentType = xmlDoc.CreateAttribute("ContentTypeIndex");
                    attContentType.Value = contElt.ContentType.ToString();
                    eltContained.Attributes.Append(attContentType);
                    // BoxPosition
                    var attPosition = xmlDoc.CreateAttribute("Position");
                    attPosition.Value = contElt.Position.ToString();
                    eltContained.Attributes.Append(attPosition);
                }
            }
            if (analysis is HAnalysisPallet analysisPallet)
            {
                HConstraintSetPallet constraintSet = analysisPallet.ConstraintSet as HConstraintSetPallet;
                // element containers
                var eltContainer = xmlDoc.CreateElement("Container");
                eltContainers.AppendChild(eltContainer);
                var attId = xmlDoc.CreateAttribute("Id");
                attId.Value = analysisPallet.Pallet.ID.IGuid.ToString();
                eltContainer.Attributes.Append(attId);
                // element HConstraintSetPallet
                var eltConstraintSet = xmlDoc.CreateElement("HConstraintSetPallet");
                eltAnalysis.AppendChild(eltConstraintSet);
                // attribute MaximumHeight
                var attMaximumHeight = xmlDoc.CreateAttribute("MaximumHeight");
                attMaximumHeight.Value = string.Format(CultureInfo.InvariantCulture, "{0}",  constraintSet.MaximumHeight.ToString());
                eltConstraintSet.Attributes.Append(attMaximumHeight);
                // attribute Overhang
                var attOverhang = xmlDoc.CreateAttribute("Overhang");
                attOverhang.Value = constraintSet.Overhang.ToString();
                eltConstraintSet.Attributes.Append(attOverhang);
            }
            else if (analysis is HAnalysisCase analysisCase)
            {
                HConstraintSetCase constraintSet = analysisCase.ConstraintSet as HConstraintSetCase;
                // element HConstraintSetCase
                var eltConstraintSet = xmlDoc.CreateElement("HConstraintSetCase");
                eltAnalysis.AppendChild(eltConstraintSet);
            }
            else if (analysis is HAnalysisTruck analysisTruck)
            {
                HConstraintSetTruck constraintSet = analysisTruck.ConstraintSet as HConstraintSetTruck;
                // element HConstraintSetTruck
                var eltConstraintSet = xmlDoc.CreateElement("HConstraintSetTruck");
                eltAnalysis.AppendChild(eltConstraintSet);
            }
        }
        #endregion

        #region Save legacy analysis
        public void Save(CaseOfBoxesProperties caseOfBoxesProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlBoxProperties element
            XmlElement xmlBoxProperties = xmlDoc.CreateElement("CaseOfBoxesProperties");
            parentElement.AppendChild(xmlBoxProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = caseOfBoxesProperties.ID.IGuid.ToString();
            xmlBoxProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = caseOfBoxesProperties.ID.Name;
            xmlBoxProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = caseOfBoxesProperties.ID.Description;
            xmlBoxProperties.Attributes.Append(descAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", caseOfBoxesProperties.Weight);
            xmlBoxProperties.Attributes.Append(weightAttribute);
            // save inside ref to box properties
            XmlAttribute insideBoxId = xmlDoc.CreateAttribute("InsideBoxId");
            insideBoxId.Value = caseOfBoxesProperties.InsideBoxProperties.ID.IGuid.ToString();
            xmlBoxProperties.Attributes.Append(insideBoxId);
            // save case definition
            SaveCaseDefinition(caseOfBoxesProperties.CaseDefinition, xmlBoxProperties, xmlDoc);
            // save optim constraintset
            SaveCaseOptimConstraintSet(caseOfBoxesProperties.CaseOptimConstraintSet, xmlBoxProperties, xmlDoc);
            // colors
            SaveColors(caseOfBoxesProperties.Colors, xmlBoxProperties, xmlDoc);
            // texture
            SaveTextures(caseOfBoxesProperties.TextureList, xmlBoxProperties, xmlDoc);
        }
        private void SaveCaseDefinition(CaseDefinition caseDefinition, XmlElement xmlBoxProperties, XmlDocument xmlDoc)
        {
            XmlElement xmlCaseDefElement = xmlDoc.CreateElement("CaseDefinition");
            xmlBoxProperties.AppendChild(xmlCaseDefElement);
            // case arrangement
            XmlAttribute xmlArrangement = xmlDoc.CreateAttribute("Arrangement");
            xmlArrangement.Value = caseDefinition.Arrangement.ToString();
            xmlCaseDefElement.Attributes.Append(xmlArrangement);
            // box orientation
            XmlAttribute xmlOrientation = xmlDoc.CreateAttribute("Orientation");
            xmlOrientation.Value = string.Format("{0} {1}", caseDefinition.Dim0, caseDefinition.Dim1);
            xmlCaseDefElement.Attributes.Append(xmlOrientation);
        }
        private void SaveCaseOptimConstraintSet(ParamSetPackOptim caseOptimConstraintSet, XmlElement xmlBoxProperties, XmlDocument xmlDoc)
        {
            XmlElement xmlCaseOptimConstraintSet = xmlDoc.CreateElement("OptimConstraintSet");
            xmlBoxProperties.AppendChild(xmlCaseOptimConstraintSet);
            // wall thickness
            XmlAttribute xmlWallThickness = xmlDoc.CreateAttribute("WrapperThickness");
            xmlWallThickness.Value = string.Format(CultureInfo.InvariantCulture, "{0}", caseOptimConstraintSet.WrapperThickness);
            xmlCaseOptimConstraintSet.Attributes.Append(xmlWallThickness);
            // wall surface mass
            XmlAttribute xmlWallSurfaceMass = xmlDoc.CreateAttribute("WrapperSurfMass");
            xmlWallSurfaceMass.Value = string.Format(CultureInfo.InvariantCulture, "{0}", caseOptimConstraintSet.WrapperSurfMass);
            xmlCaseOptimConstraintSet.Attributes.Append(xmlWallSurfaceMass);
            // no walls
            XmlAttribute xmlNumberOfWalls = xmlDoc.CreateAttribute("NumberOfWalls");
            xmlNumberOfWalls.Value = string.Format("{0} {1} {2}"
                , caseOptimConstraintSet.NoWrapperWalls[0]
                , caseOptimConstraintSet.NoWrapperWalls[1]
                , caseOptimConstraintSet.NoWrapperWalls[2]);
            xmlCaseOptimConstraintSet.Attributes.Append(xmlNumberOfWalls);
        }
        public void Save(Layer3DBox boxLayer, XmlElement layersElt, XmlDocument xmlDoc)
        {
            // BoxLayer
            XmlElement boxlayerElt = xmlDoc.CreateElement("BoxLayer");
            layersElt.AppendChild(boxlayerElt);
            // ZLow
            XmlAttribute zlowAttribute = xmlDoc.CreateAttribute("ZLow");
            zlowAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxLayer.ZLow);
            boxlayerElt.Attributes.Append(zlowAttribute);
            // maximum space
            XmlAttribute attributeMaxSpace = xmlDoc.CreateAttribute("MaximumSpace");
            attributeMaxSpace.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxLayer.MaximumSpace);
            boxlayerElt.Attributes.Append(attributeMaxSpace);

            foreach (BoxPosition boxPosition in boxLayer)
                SaveBoxPosition(boxPosition, boxlayerElt, xmlDoc);            
        }
        private void SaveBoxPosition(BoxPosition bPos, XmlElement parentElt, XmlDocument xmlDoc)
        {
            // BoxPosition
            XmlElement boxPositionElt = xmlDoc.CreateElement("BoxPosition");
            parentElt.AppendChild(boxPositionElt);
            // Position
            XmlAttribute positionAttribute = xmlDoc.CreateAttribute("Position");
            positionAttribute.Value = bPos.Position.ToString();
            boxPositionElt.Attributes.Append(positionAttribute);
            // AxisLength
            XmlAttribute axisLengthAttribute = xmlDoc.CreateAttribute("AxisLength");
            axisLengthAttribute.Value = HalfAxis.ToString(bPos.DirectionLength);
            boxPositionElt.Attributes.Append(axisLengthAttribute);
            // AxisWidth
            XmlAttribute axisWidthAttribute = xmlDoc.CreateAttribute("AxisWidth");
            axisWidthAttribute.Value = HalfAxis.ToString(bPos.DirectionWidth);
            boxPositionElt.Attributes.Append(axisWidthAttribute);
        }
        public void Save(Layer3DCyl cylLayer, XmlElement layersElt, XmlDocument xmlDoc)
        {
            // BoxLayer
            XmlElement cylLayerElt = xmlDoc.CreateElement("CylLayer");
            layersElt.AppendChild(cylLayerElt);
            // ZLow
            XmlAttribute zlowAttribute = xmlDoc.CreateAttribute("ZLow");
            zlowAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylLayer.ZLow);
            cylLayerElt.Attributes.Append(zlowAttribute);
            foreach (Vector3D cylPosition in cylLayer)
                SaveCylPosition(cylPosition, cylLayerElt, xmlDoc);                    
        }
        private void SaveCylPosition(Vector3D vPos, XmlElement parentElt, XmlDocument xmlDoc)
        {
            // BoxPosition
            XmlElement cylPositionElt = xmlDoc.CreateElement("CylPosition");
            parentElt.AppendChild(cylPositionElt);
            // Position
            XmlAttribute positionAttribute = xmlDoc.CreateAttribute("Position");
            positionAttribute.Value = vPos.ToString();
            cylPositionElt.Attributes.Append(positionAttribute);
        }
        private void SaveCylPosition(Vector2D vPos, XmlElement parentElt, XmlDocument xmlDoc)
        {
            // BoxPosition
            XmlElement cylPositionElt = xmlDoc.CreateElement("CylPosition");
            parentElt.AppendChild(cylPositionElt);
            // Position
            XmlAttribute positionAttribute = xmlDoc.CreateAttribute("Position2D");
            positionAttribute.Value = vPos.ToString();
            cylPositionElt.Attributes.Append(positionAttribute);
        }
        #endregion
        #endregion

        #region Close
        public virtual void Close()
        {
            // remove all analysis and items
            // -> this should close any listening forms
            while (Analyses.Count > 0)
                RemoveItem(Analyses[0]);
            while (_typeList.Count > 0)
                RemoveItem(_typeList[0]);
            DocumentClosed?.Invoke(this);
        }
        #endregion

        #region Helpers
        private ItemBase GetTypeByGuid(Guid guid)
        {
            return _typeList.FirstOrDefault(x => x.ID.IGuid == guid)
                ?? throw new ArgumentException($"No type with Guid = {guid}", nameof(guid));
        }
        private ItemBase GetTypeByGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;
            return GetTypeByGuid(Guid.Parse(guid));
        }
        private Packable GetContentByGuid(Guid guid)
        {
            foreach (ItemBase type in _typeList)
            {
                if (type.ID.IGuid == guid)
                {
                    if (type is Packable packable)
                        return packable;
                    else
                        throw new ArgumentException($"Guid {guid} found but not a packable", nameof(guid));
                }
            }
            foreach (var analysis in Analyses)
            {
                if (analysis.ID.IGuid == guid)
                {
                    if (analysis is AnalysisHomo analysisHomo)
                        return analysisHomo.EquivalentPackable;
                    else
                        throw new ArgumentException($"Guid {guid} found but not an hmomogeneous analysis", nameof(guid));
                }
            }
            throw new ArgumentException($"GetContentByGuid() -> No type with Guid = {guid.ToString()}", nameof(guid));
        }
        private static string BitmapToString(Bitmap bmp)
        {
            byte[] bmpBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmpBytes = ms.GetBuffer();
                ms.Close();
            }
            return Convert.ToBase64String(bmpBytes);
        }
        private static Bitmap StringToBitmap(string bmpData)
        {
            byte[] bytes = Convert.FromBase64String(bmpData);
            return new Bitmap(new MemoryStream(bytes));
        }
        private static int[] ParseInt2(string value)
        {
            string regularExp = "(?<i1>.*) (?<i2>.*)";
            Regex r = new Regex(regularExp, RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                int[] iArray = new int[2];
                iArray[0] = int.Parse(m.Result("${i1}"));
                iArray[1] = int.Parse(m.Result("${i2}"));
                return iArray;
            }
            else
                throw new Exception("Failed parsing int[2] from " + value);
        }
        private static int[] ParseInt3(string value)
        {
            string regularExp = "(?<i1>.*) (?<i2>.*) (?<i3>.*)";
            Regex r = new Regex(regularExp, RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                int[] iArray = new int[3];
                iArray[0] = int.Parse(m.Result("${i1}"));
                iArray[1] = int.Parse(m.Result("${i2}"));
                iArray[2] = int.Parse(m.Result("${i3}"));
                return iArray;
            }
            else
                throw new Exception("Failed parsing int[3] from " + value);
        }
        #endregion

        #region Methods to be overriden
        public virtual void Modify()
        {
        }
        #endregion

        #region Listener notification methods
        public void AddListener(IDocumentListener listener)
        {
            _listeners.Add(listener);
        }
        private void NotifyOnNewTypeCreated(ItemBase item)
        {
            TypeCreated?.Invoke(item);
            foreach (IDocumentListener listener in _listeners)
                listener.OnNewTypeCreated(this, item);
        }
        private void NotifyOnNewAnalysisCreated(Analysis analysis)
        {
            AnalysisCreated?.Invoke(analysis);
            foreach (IDocumentListener listener in _listeners)
                listener.OnNewAnalysisCreated(this, analysis);
        }
        private void NotifyAnalysisUpdated(Analysis analysis)
        {
            AnalysisUpdated?.Invoke(analysis);
            foreach (IDocumentListener listener in _listeners)
                listener.OnAnalysisUpdated(this, analysis);
        }
        private void NotifyOnNewAnalysisCreated(AnalysisHetero analysis)
        {
            AnalysisCreated?.Invoke(analysis);
            foreach (IDocumentListener listener in _listeners)
                listener.OnNewAnalysisCreated(this, analysis);
        }
        private void NotifyAnalysisUpdated(AnalysisHetero analysis)
        {
            AnalysisUpdated?.Invoke(analysis);
            foreach (IDocumentListener listener in _listeners)
                listener.OnAnalysisUpdated(this, analysis);
        }
        private void NotifyOnTypeRemoved(ItemBase item)
        {
            TypeRemoved?.Invoke(item);
            foreach (IDocumentListener listener in _listeners)
                listener.OnTypeRemoved(this, item);
        }
        private void NotifyOnAnalysisRemoved(ItemBase analysis)
        {
            AnalysisRemoved?.Invoke(analysis as Analysis);
            foreach (IDocumentListener listener in _listeners)
                listener.OnAnalysisRemoved(this, analysis);
        }
        #endregion

        #region Events
        public delegate void DocumentClosing(Document document);
        public event DocumentClosing DocumentClosed;

        public delegate void AnalysisCreating(Analysis analysis);
        public event AnalysisCreating AnalysisCreated;
        public delegate void AnalysisRemoving(Analysis analysis);
        public event AnalysisRemoving AnalysisRemoved;
        public delegate void AnalysisUpdating(Analysis analysis);
        public event AnalysisUpdating AnalysisUpdated;
        public delegate void TypeCreating(ItemBase item);
        public event TypeCreating TypeCreated;
        public delegate void TypeRemoving(ItemBase item);
        public event TypeRemoving TypeRemoved;
        #endregion

        #region Data members
        private UnitsManager.UnitSystem UnitSystem = UnitsManager.UnitSystem.UNIT_METRIC1;
        private List<ItemBase> _typeList = new List<ItemBase>();
        private List<IDocumentListener> _listeners = new List<IDocumentListener>();
        private Dictionary<int, ConveyorSetting> DictConveyorSettings { get; set; } = new Dictionary<int, ConveyorSetting>();

        protected static readonly ILog _log = LogManager.GetLogger(typeof(Document));
        #endregion
    }
}
