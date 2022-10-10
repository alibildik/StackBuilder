#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;

using log4net;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.PLMPack.DBClient;
using treeDiM.PLMPack.DBClient.PLMPackSR;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    internal class DatabaseHelpers
    {
        static public void InsertDefaultItems(DocumentSB docSB)
        {
            if (null == docSB) return;
            try
            {
                using (WCFClient wcfClient = new WCFClient())
                {
                    // pallets
                    int rangeIndex = 0, numberOfItems = 0;
                    do
                    {
                        DCSBPallet[] pallets = wcfClient.Client.GetAllPalletsAuto(rangeIndex++, ref numberOfItems, true);
                        foreach (var dcsbPallet in pallets)
                        {
                            UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)dcsbPallet.UnitSystem;

                            PalletProperties palletProperties = docSB.CreateNewPallet(
                                dcsbPallet.Name, dcsbPallet.Description,
                                dcsbPallet.PalletType,
                                UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M0, us),
                                UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M1, us),
                                UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M2, us),
                                UnitsManager.ConvertMassFrom(dcsbPallet.Weight, us),
                                UnitsManager.ConvertMassFrom(null != dcsbPallet.AdmissibleLoad? dcsbPallet.AdmissibleLoad.Value : 0.0, us),
                                Color.FromArgb(dcsbPallet.Color));
                        }
                    }
                    while ((rangeIndex + 1) * 20 < numberOfItems);

                    // cases
                    rangeIndex = 0; numberOfItems = 0;
                    do
                    {
                        DCSBCase[] cases = wcfClient.Client.GetAllCasesAuto(rangeIndex++, ref numberOfItems, true);
                        foreach (var dcsbCase in cases)
                        {
                            UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)dcsbCase.UnitSystem;

                            var colors = new Color[6];
                            for (int i = 0; i < 6; ++i)
                                colors[i] = Color.FromArgb(dcsbCase.Colors[i]);

                            BoxProperties bProperties = null;
                            if (dcsbCase.IsCase)
                                bProperties = docSB.CreateNewCase(dcsbCase.Name, dcsbCase.Description,
                                UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M0, us),
                                UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M1, us),
                                UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M2, us),
                                UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsInner.M0, us),
                                UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsInner.M1, us),
                                UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsInner.M2, us),
                                dcsbCase.DimensionsInner.M0 > 0.0 && dcsbCase.DimensionsInner.M1 > 0.0 && dcsbCase.DimensionsInner.M2 > 0.0,
                                UnitsManager.ConvertMassFrom(dcsbCase.Weight, us),
                                colors);
                            else
                                bProperties = docSB.CreateNewBox(
                                    dcsbCase.Name, dcsbCase.Description,
                                    UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M0, us),
                                    UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M1, us),
                                    UnitsManager.ConvertLengthFrom(dcsbCase.DimensionsOuter.M2, us),
                                    UnitsManager.ConvertMassFrom(dcsbCase.Weight, us),
                                    colors);
                            bProperties.TapeWidth = new OptDouble(dcsbCase.ShowTape, UnitsManager.ConvertLengthFrom(dcsbCase.TapeWidth, us));
                            bProperties.TapeColor = Color.FromArgb(dcsbCase.TapeColor);
                            bProperties.SetNetWeight(
                                new OptDouble(!dcsbCase.HasInnerDims && dcsbCase.NetWeight.HasValue
                                    , dcsbCase.NetWeight ?? 0.0));
                            List<Pair<HalfAxis.HAxis, Texture>> textures = new List<Pair<HalfAxis.HAxis, Texture>>();
                            bProperties.TextureList = textures;
                        }
                    }
                    while ((rangeIndex + 1) * 20 < numberOfItems);

                    // bundles
                    rangeIndex = 0; numberOfItems = 0;
                    do
                    {
                        DCSBBundle[] bundles = wcfClient.Client.GetAllBundlesAuto(rangeIndex++, ref numberOfItems, true);
                        foreach (var dcsbBundle in bundles)
                        {
                            UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)dcsbBundle.UnitSystem;

                            docSB.CreateNewBundle(dcsbBundle.Name, dcsbBundle.Description,
                                UnitsManager.ConvertLengthFrom(dcsbBundle.DimensionsUnit.M0, us),
                                UnitsManager.ConvertLengthFrom(dcsbBundle.DimensionsUnit.M1, us),
                                UnitsManager.ConvertLengthFrom(dcsbBundle.DimensionsUnit.M2, us),
                                UnitsManager.ConvertMassFrom(dcsbBundle.UnitWeight, us),
                                Color.FromArgb(dcsbBundle.Color),
                                dcsbBundle.Number);
                        }
                    }
                    while ((rangeIndex + 1) * 20 < numberOfItems);

                    // interlayers
                    rangeIndex = 0; numberOfItems = 0;
                    do
                    {
                        DCSBInterlayer[] interlayers = wcfClient.Client.GetAllInterlayersAuto(rangeIndex++, ref numberOfItems, true);
                        foreach (var dcsbInterlayer in interlayers)
                        {
                            UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)dcsbInterlayer.UnitSystem;

                            docSB.CreateNewInterlayer(dcsbInterlayer.Name, dcsbInterlayer.Description,
                                UnitsManager.ConvertLengthFrom(dcsbInterlayer.Dimensions.M0, us),
                                UnitsManager.ConvertLengthFrom(dcsbInterlayer.Dimensions.M1, us),
                                UnitsManager.ConvertLengthFrom(dcsbInterlayer.Dimensions.M2, us),
                                UnitsManager.ConvertMassFrom(dcsbInterlayer.Weight, us),
                                Color.FromArgb(dcsbInterlayer.Color)
                                );
                        }
                    }
                    while ((rangeIndex + 1) * 20 < numberOfItems);

                    // trucks
                    rangeIndex = 0; numberOfItems = 0;
                    do
                    {
                        DCSBTruck[] trucks = wcfClient.Client.GetAllTrucksAuto(rangeIndex++, ref numberOfItems, true);
                        foreach (var dcsbTruck in trucks)
                        {
                            UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)dcsbTruck.UnitSystem;

                            docSB.CreateNewTruck(dcsbTruck.Name, dcsbTruck.Description,
                                UnitsManager.ConvertLengthFrom(dcsbTruck.DimensionsInner.M0, us),
                                UnitsManager.ConvertLengthFrom(dcsbTruck.DimensionsInner.M1, us),
                                UnitsManager.ConvertLengthFrom(dcsbTruck.DimensionsInner.M2, us),
                                UnitsManager.ConvertMassFrom(dcsbTruck.AdmissibleLoad, us),
                                Color.FromArgb(dcsbTruck.Color)
                                );
                        }
                    }
                    while ((rangeIndex + 1) * 20 < numberOfItems);

                    // cylinders
                    rangeIndex = 0; numberOfItems = 0;
                    do
                    {
                        DCSBCylinder[] cylinders = wcfClient.Client.GetAllCylindersAuto(rangeIndex++, ref numberOfItems, true);
                        foreach (var dcsbCylinder in cylinders)
                        {
                            UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)dcsbCylinder.UnitSystem;
                            docSB.CreateNewCylinder(
                               dcsbCylinder.Name, dcsbCylinder.Description,
                               UnitsManager.ConvertLengthFrom(dcsbCylinder.RadiusOuter, us),
                               UnitsManager.ConvertLengthFrom(dcsbCylinder.RadiusInner, us),
                               UnitsManager.ConvertLengthFrom(dcsbCylinder.Height, us),
                               UnitsManager.ConvertMassFrom(dcsbCylinder.Weight, us),
                               OptDouble.Zero,
                               Color.FromArgb(dcsbCylinder.ColorTop), Color.FromArgb(dcsbCylinder.ColorOuter), Color.FromArgb(dcsbCylinder.ColorInner)
                               );
                        }
                    }
                    while ((rangeIndex + 1) * 20 < numberOfItems);

                    // pallet corners
                    rangeIndex = 0; numberOfItems = 0;
                    do
                    {
                        DCSBPalletCorner[] palletCorners = wcfClient.Client.GetAllPalletCornersAuto(rangeIndex++, ref numberOfItems, true);
                        foreach (var dcsbPalletCorner in palletCorners)
                        {
                            UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)dcsbPalletCorner.UnitSystem;
                            docSB.CreateNewPalletCorners(
                                dcsbPalletCorner.Name, dcsbPalletCorner.Description,
                                UnitsManager.ConvertLengthFrom(dcsbPalletCorner.Length, us),
                                UnitsManager.ConvertLengthFrom(dcsbPalletCorner.Width, us),
                                UnitsManager.ConvertLengthFrom(dcsbPalletCorner.Thickness, us),
                                UnitsManager.ConvertMassFrom(dcsbPalletCorner.Weight, us),
                                Color.FromArgb(dcsbPalletCorner.Color)
                                );
                        }
                    }
                    while ((rangeIndex + 1) * 20 < numberOfItems);

                    // pallet caps
                    rangeIndex = 0; numberOfItems = 0;
                    do
                    {
                        DCSBPalletCap[] palletCaps = wcfClient.Client.GetAllPalletCapsAuto(rangeIndex++, ref numberOfItems, true);

                        foreach (var dcsbCap in palletCaps)
                        {
                            UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)dcsbCap.UnitSystem;
                            docSB.CreateNewPalletCap(
                                dcsbCap.Name, dcsbCap.Description,
                                UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsOuter.M0, us),
                                UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsOuter.M1, us),
                                UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsOuter.M2, us),
                                UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsInner.M0, us),
                                UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsInner.M1, us),
                                UnitsManager.ConvertLengthFrom(dcsbCap.DimensionsInner.M2, us),
                                UnitsManager.ConvertMassFrom(dcsbCap.Weight, us),
                                Color.FromArgb(dcsbCap.Color)
                                );
                        }
                    }
                    while ((rangeIndex + 1) * 20 < numberOfItems);

                    // pallet films
                    rangeIndex = 0; numberOfItems = 0;
                    do
                    {
                        DCSBPalletFilm[] palletFilms = wcfClient.Client.GetAllPalletFilmsAuto(rangeIndex++, ref numberOfItems, true);
                        foreach (var dcsbPalletFilm in palletFilms)
                        {
                            UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)dcsbPalletFilm.UnitSystem;
                            docSB.CreateNewPalletFilm(dcsbPalletFilm.Name, dcsbPalletFilm.Description,
                                dcsbPalletFilm.UseTransparency, dcsbPalletFilm.UseHatching,
                                UnitsManager.ConvertLengthFrom(dcsbPalletFilm.HatchingSpace, us),
                                dcsbPalletFilm.HatchingAngle,
                                UnitsManager.ConvertLinearMassFrom(dcsbPalletFilm.LinearMass, us),
                                Color.FromArgb(dcsbPalletFilm.Color));
                        }
                    }
                    while ((rangeIndex + 1) * 20 < numberOfItems);

                } // using
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private static ILog _log = LogManager.GetLogger(typeof(DatabaseHelpers));
    }
}
