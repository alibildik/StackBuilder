#region Using directives
using System.Runtime.Serialization;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{
    [DataContract]
    public class DCSBContainer
    {
        [DataMember]
        DCSBDim3D Dimensions { get; set; }
        [DataMember]
        double? MaxLoadWeight { get; set; }
        [DataMember]
        string ContainerType { get; set; }
        [DataMember]
        public int Color { get; set; }
    }
}