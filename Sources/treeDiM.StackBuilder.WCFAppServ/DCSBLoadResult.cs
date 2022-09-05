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

    public class DCSBStatus
    {
        [DataMember]
        DCSBStatusEnu Status { get; set; }
        [DataMember]
        string Error { get; set; }
    }

    [DataContract]
    public class DCSBLoadResult
    {
        [DataMember]
        DCSBConfigId ConfigId { get; set; }
        [DataMember]
        DCSBStatus Status { get; set; }
        [DataMember]
        int NumberPerLayer { get; set; } // string for non homogeneous layers
        [DataMember]
        int NumberOfLayers { get; set; }
        [DataMember]
        int UpalItem { get; set; }
        [DataMember]
        int UpalCase { get; set; }
        [DataMember]
        double IsoBasePercentage { get; set; }
        [DataMember]
        double IsoVolPercentage { get; set; }
        [DataMember]
        double LoadWeight { get; set; }
        [DataMember]
        bool MaxLoadValidity { get; set; }
    }

    [DataContract]
    public class DCSBLoadResultContainer : DCSBLoadResult
    {
        [DataMember]
        DCSBContainer Container { get; set; }
    }

    public class DCSBLoadResultPallet : DCSBLoadResult
    {
        [DataMember]
        DCSBPallet Pallet { get; set; }
    }
}