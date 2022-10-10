#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;

using treeDiM.PLMPack.DBClient;
using treeDiM.PLMPack.DBClient.PLMPackSR;
#endregion

namespace treeDiM.EdgeCrushTest
{
    internal abstract class PalletAccessor
    {
        public abstract List<DCSBPallet> GetAllPallets();
        public static PalletAccessor Instance
        {
            get
            {
                if (null == _instance)
                    _instance = new PalletAccessorWCF();
                return _instance;
            }
        }
        private static PalletAccessor _instance;
    }
   
    internal class PalletAccessorWCF : PalletAccessor
    {
        public override List<DCSBPallet> GetAllPallets()
        {
            try
            {
                List<DCSBPallet> pallets = new List<DCSBPallet>();
                using (var wcfClient = new WCFClient())
                {
                    int rangeIndex = 0, numberOfItems = 0;
                    do
                    {
                        var dcsbPallets = wcfClient.Client.GetAllPalletsSearch(string.Empty, false, rangeIndex++, ref numberOfItems, false);
                        pallets.AddRange(dcsbPallets);
                    }
                    while (rangeIndex * 20 < numberOfItems) ;
                    
                }
                return pallets;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return null;
            }
        }
        private static ILog _log = LogManager.GetLogger(typeof(PalletAccessorWCF));
    }

    internal class PalletItem
    {
        public PalletItem(DCSBPallet p) { PalletProp = p; }
        public override string ToString() => $"{PalletProp.Name} ({PalletProp.Dimensions.M0}x{PalletProp.Dimensions.M1}x{PalletProp.Dimensions.M2})";
        public DCSBPallet PalletProp { get; set; }
    }
}
