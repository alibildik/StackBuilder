#region Using directives
using System.Runtime.Serialization;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{

    [DataContract]
    public enum DCSBStatusEnu
    {
        [EnumMember]
        Success,
        [EnumMember]
        FailureLengthOrWidthExceeded,
        [EnumMember]
        FailureHeightExceeded,
        [EnumMember]
        FailureWeightExceeded
    }
    [DataContract]
    public class DCSBStatus
    {
        [DataMember]
        public DCSBStatusEnu Status { get; set; }
        [DataMember]
        public string Error { get; set; }
    }

    [DataContract]
    public class DCSBLoadResult
    {
        [DataMember]
        public DCSBConfigId ConfigId { get; set; }
        [DataMember]
        public DCSBStatus Status { get; set; }
        [DataMember]
        public int NumberPerLayer { get; set; } // string for non homogeneous layers
        [DataMember]
        public int NumberOfLayers { get; set; }
        [DataMember]
        public int UpalItem { get; set; }
        [DataMember]
        public int UpalCase { get; set; }
        [DataMember]
        public double IsoBasePercentage { get; set; }
        [DataMember]
        public double IsoVolPercentage { get; set; }
        [DataMember]
        public double LoadWeight { get; set; }
        [DataMember]
        public bool MaxLoadValidity { get; set; }
    }

    [DataContract]
    public class DCSBLoadResultContainer : DCSBLoadResult
    {
        [DataMember]
        public DCSBContainer Container { get; set; }
    }
    [DataContract]
    public class DCSBLoadResultPallet : DCSBLoadResult
    {
        [DataMember]
        public DCSBPalletWHeight Pallet { get; set; }
        [DataMember]
        public string PalletMapPhrase { get; set; }
    }
}