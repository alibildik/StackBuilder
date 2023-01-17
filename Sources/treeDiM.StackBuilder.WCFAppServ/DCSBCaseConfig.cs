#region Using directives
using System.Runtime.Serialization;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{
    [DataContract]
    public enum DCSBOrientation
    {
        [EnumMember]
        FrontBack = 0,
        [EnumMember]
        LeftRight = 1,
        [EnumMember]
        BottomTop = 2
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
        LeftRight,
        [EnumMember]
        Unconveyable
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
        public double Length { get; set; }
        [DataMember]
        public double Width { get; set; }
        [DataMember]
        public double Height { get; set; }
        [DataMember]
        public double Weight { get; set; }
        [DataMember]
        public int Pcb { get; set; }
        [DataMember]
        public DCSBOrientation Orientation { get; set; }
        [DataMember]
        public DCSBDim3D Dim3D { get; set; }
        [DataMember]
        public double Volume { get; set; }
        [DataMember]
        public double AreaBottomTop { get; set; }
        [DataMember]
        public double AreaFrontBack { get; set; }
        [DataMember]
        public double AreaLeftRight { get; set; }
        [DataMember]
        public DCSBStabilityEnum Stable { get; set; }
        [DataMember]
        public DCSBConveyability Conveyability { get; set; }
        [DataMember]
        public DCSBOrientationName ConveyFace { get; set; }
        [DataMember]
        public DCSBConveyMode ConveyMode { get; set; }
        [DataMember]
        public DCSBPrep PrepMode { get; set; }
        [DataMember]
        public DCCompFileOutput Image { get; set; }
    }
}