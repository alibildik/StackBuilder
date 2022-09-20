#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{
    [DataContract]
    public class DCSBPallet : DCSBItem
    {
        [DataMember]
        public DCSBDim3D Dimensions { get; set; }
        [DataMember]
        public string PalletType { get; set; }
        [DataMember]
        public double Weight { get; set; }
        [DataMember]
        public int Color { get; set; }
    }
    [DataContract]
    public class DCSBPalletWHeight : DCSBPallet
    {
        [DataMember]
        public double MaxPalletHeight { get; set; }
        [DataMember]
        public double MaxPalletLoad { get; set; }
    }
    public class ComparerDCSBPallet_Name : IComparer<DCSBPallet>
    {
        public int Compare(DCSBPallet pallet1, DCSBPallet pallet2)
        {
            return string.Compare(pallet1.Name, pallet2.Name);
        }
    }
}