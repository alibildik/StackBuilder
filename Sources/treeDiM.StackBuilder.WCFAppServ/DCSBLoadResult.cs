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
        public DCSBOrientation Orientation { get; set; }
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
        public double TotalWeight { get; set; }
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
    [DataContract]
    public class DCSBSuggest
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public int CaseCountFrom { get; set; }
        [DataMember]
        public int CaseCountTo { get; set; }
    }
    [DataContract]
    public class DCSBSuggestIncreasePalletXY : DCSBSuggest
    {
        [DataMember]
        public int Dim { get; set; }
        [DataMember]
        public DCSBDim2D PalletDimFrom { get; set; }
        [DataMember]
        public DCSBDim2D PalletDimTo { get; set; }
        [DataMember]
        public int PerLayerCountFrom { get; set; }
        [DataMember]
        public int PerLayerCountTo { get; set; }
    }
    [DataContract]
    public class DCSBSuggestIncreasePalletZ : DCSBSuggest
    {
        [DataMember]
        public double HeightFrom { get; set; }
        [DataMember]
        public double HeightTo { get; set; }
        [DataMember]
        public int LayerCountFrom {get; set;}
        [DataMember]
        public int LayerCountTo { get; set; }
    }
    [DataContract]
    public class DCSBSuggestDecreaseCaseXY : DCSBSuggest
    {
        [DataMember]
        public int Dim { get; set; }
        [DataMember]
        public DCSBDim3D CaseDimFrom { get; set; }
        [DataMember]
        public DCSBDim3D CaseDimTo { get; set; }
        [DataMember]
        public int PerLayerCountFrom { get; set; }
        [DataMember]
        public int PerLayerCountTo { get; set; }
    }
    [DataContract]
    public class DCSBSuggestDecreaseLayerHeight : DCSBSuggest
    {
        [DataMember]
        public int Dim { get; set; }
        public double HeightFrom { get; set; }
        [DataMember]
        public int LayerCountFrom { get; set; }
        [DataMember]
        public int LayerCountTo { get; set; }
    }

}