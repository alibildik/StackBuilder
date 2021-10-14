#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;

using log4net;
using Sharp3D.Math.Core;

using treeDiM.PLMPack.DBClient;
using treeDiM.PLMPack.DBClient.PLMPackSR;
#endregion

namespace treeDiM.EdgeCrushTest
{
    internal abstract class CardboardQualityAccessor
    {
        #region Abstract methods
        public abstract List<QualityData> CardboardQualities { get; }
        public abstract void AddQuality(string name, string profile, double thickness, double ECT, double rigidityX, double rigidityY);
        public abstract void RemoveQuality(int index);
        #endregion
        #region public methods
        public bool NameExists(string name)
        {
            var qualities = CardboardQualities;
            return null != qualities.Find(q => q.Name.ToLower() == name.ToLower());
        }
        public QualityData GetQualityDataByName(string name)
        {
            var qualities = CardboardQualities;
            QualityData qd = qualities.Find(q => q.Name.ToLower() == name.ToLower());
            if (null == qd)
                throw new ECTException(ECTException.ErrorType.ERROR_INVALIDCARDBOARD, name);
            return qd;
        }
        public List<QualityData> GetFilteredListCardboardQuality(string profile)
        {
            var qualities = CardboardQualities;
            return qualities.Where(qd => (string.IsNullOrEmpty(profile) || string.Equals(qd.Profile, profile))).ToList();
        }
        public List<QualityData> GetSortedListCardboardQuality(Vector3D dim, string caseType, bool dblWall, McKeeFormula.FormulaType formulaType, string profile, bool filtered, double appliedLoad)
        {
            var qualities = CardboardQualities;
            return qualities.Where(qd => (string.IsNullOrEmpty(profile) || string.Equals(qd.Profile, profile)))
                          .Where(qd => (qd.ComputeStaticBCT(dim, caseType, dblWall, formulaType) >= appliedLoad * 9.81 / 10) || !filtered)
                          .OrderBy(qh => qh.ComputeStaticBCT(dim, caseType, dblWall, formulaType))
                          .ToList();
        }
        public List<string> GetProfileList()
        {
            var qualities = CardboardQualities;
            return qualities.Select(q => q.Profile).Distinct().ToList();
        }
        public string UserName { get; set; }
        public abstract void UploadAllDefaultCardboardQualities();
        #endregion
        #region Singleton
        public static CardboardQualityAccessor Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new CardboardQualityAccessorWCF();
                return _instance;
            }
        }
        private static CardboardQualityAccessor _instance;
        #endregion
        #region Data members
        protected List<QualityData> _listQualities;
        #endregion
    }

    internal class CardboardQualityAccessorWCF : CardboardQualityAccessor
    {
        /// <summary>
        /// Access dictionnary of cardboard qualities
        /// </summary>
        public override List<QualityData> CardboardQualities
        {
            get
            {
                if (null == _listQualities)
                {
                    // instantiate
                    var list = new List<QualityData>();

                    try
                    {
                        using (var wcfClient = new WCFClient())
                        {
                            UserName = wcfClient.User.Name;
                            var qualities = wcfClient.Client.GetAllCardboardQualities();
                            foreach (var q in qualities)
                            {
                                try
                                {
                                    list.Add(                                        
                                        new QualityData(q.ID, q.Name, q.Profile, q.Thickness, q.ECT, q.Rigidity[0], q.Rigidity[1])
                                        );
                                }
                                catch (Exception ex)
                                {
                                    _log.Error($"{q.Name} -> {ex.Message}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.ToString());
                    }
                    _listQualities = list.OrderBy(q => q.Profile).ThenBy(q => q.Name).ToList();
                }
                return _listQualities;
            }
        }
        /// <summary>
        /// Add new cardboard quality
        /// </summary>
        public override void AddQuality(string name, string profile, double thickness, double ECT, double rigidityX, double rigidityY)
        {
            try
            {
                using (WCFClient wcfClient = new WCFClient())
                {
                    wcfClient.Client.CreateNewCardboardQuality(name, profile, thickness, ECT, rigidityX, rigidityY);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            _listQualities = null;
        }
        /// <summary>
        /// Remove cardboard quality
        /// </summary>
        public override void RemoveQuality(int index)
        {
            try
            {
                var listQualities = CardboardQualities;
                if (index < listQualities.Count)
                {
                    var qualityData = listQualities[index];
                    using (WCFClient wcfClient = new WCFClient())
                    {
                        wcfClient.Client.RemoveCardboardQuality(
                            new DCCardboardQuality()
                            {
                                ID =  qualityData.ID,
                                Name = qualityData.Name,
                                Profile = qualityData.Profile,
                                Thickness = qualityData.Thickness,
                                ECT = qualityData.ECT,
                                Rigidity = new double[2]{ qualityData.RigidityDX, qualityData.RigidityDY},
                                YoungModulus = 0.0,
                                SurfacicMass = 0.0
                            }
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            _listQualities = null;
        }

        public override void UploadAllDefaultCardboardQualities()
        {
            if (CardboardQualityList.LoadFromFile(Properties.Settings.Default.CardboardQualityDBFile, out CardboardQualityList cardboardQualityList))
            {
                using (WCFClient wcfClient = new WCFClient())
                {
                    foreach (var cardboardQuality in cardboardQualityList.CardboardQuality)
                    {
                        try
                        {
                            wcfClient.Client.CreateNewCardboardQuality(cardboardQuality.Name, cardboardQuality.Profile, cardboardQuality.Thickness, cardboardQuality.ECT, cardboardQuality.RigidityDX, cardboardQuality.RigidityDY);
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex.ToString());
                        }
                    }
                }
            }
        }
        #region Data members
        private static ILog _log = LogManager.GetLogger(typeof(CardboardQualityAccessorWCF));
        #endregion
    }
}
