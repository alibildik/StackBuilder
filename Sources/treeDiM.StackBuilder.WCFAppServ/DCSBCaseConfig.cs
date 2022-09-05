#region Using directives
using System.Runtime.Serialization;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{
    [DataContract]
    public enum DCSBConfigId
    {
        [EnumMember]
        Config1,
        [EnumMember]
        Config2,
        [EnumMember]
        Config3
    }
    [DataContract]
    public enum DCSBStabilityEnum
    {
        [EnumMember]
        Stable,
        [EnumMember]
        Unstable
    }

    [DataContract]
    public enum DCSBConveyability
    {
        [EnumMember]
        Conveyable,
        [EnumMember]
        NonConveyableDimensions,
        [EnumMember]
        NonConveyableWeight
    }
    [DataContract]
    public enum DCSBConveyMode
    {
        [EnumMember]
        Manual,
        [EnumMember]
        Automatisable
    }
    [DataContract]
    public enum DCSBOrientationName
    {
        [EnumMember]
        BottomTop,
        [EnumMember]
        FrontBack,
        [EnumMember]
        LeftRight
    }
    [DataContract]
    public enum DCSBPrep
    {
        [EnumMember]
        Pac,
        [EnumMember]
        HorsPac
    }

    [DataContract]
    public class DCSBCaseConfig
    {
        [DataMember]
        public DCSBConfigId ConfigId { get; set; }
        [DataMember]
        public DCSBDim3D Dim3D { get; set; }
        [DataMember]
        public DCSBDim2D[] FaceDimensions { get; set; }
        [DataMember]
        DCSBConveyability Conveyability { get; set; }
        [DataMember]
        DCSBOrientationName ConveyFace { get; set; }
        [DataMember]
        DCSBConveyMode ConveyMode { get; set; }
        [DataMember]
        DCSBPrep PrepMode { get; set; }
        [DataMember]
        DCCompFileOutput Image { get; set; }
    }