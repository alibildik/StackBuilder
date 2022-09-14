#region Using directives
using System.Runtime.Serialization;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{
    [DataContract]
    public class DCSBLoadResultSingle
    {
        [DataMember]
        public DCSBStatus Status { get; set; }
        [DataMember]
        public DCCompFileOutput OutFile { get; set; }
    }

    [DataContract]
    public class DCSBLoadResultSinglePallet : DCSBLoadResultSingle
    {
        [DataMember]
        public DCSBLoadResultPallet Result { get; set; }
    }
    [DataContract]
    public class DCSBLoadResultSingleContainer : DCSBLoadResultSingle
    {
        [DataMember]
        public DCSBLoadResultContainer Result { get; set; }
    }
}