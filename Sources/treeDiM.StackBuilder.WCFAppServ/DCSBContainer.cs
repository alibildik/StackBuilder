#region Using directives
using System.Runtime.Serialization;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{
    [DataContract]
    public class DCSBContainer : DCSBItem
    {
        [DataMember]
        public DCSBDim3D Dimensions { get; set; }
        [DataMember]
        public double? MaxLoadWeight { get; set; }
        [DataMember]
        public int Color { get; set; }
    }
}