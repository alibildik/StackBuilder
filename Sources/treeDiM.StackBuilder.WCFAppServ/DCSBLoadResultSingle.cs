#region Using directives
using System.Runtime.Serialization;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{
    [DataContract]
    public class DCSBLoadResultSingle
    {
        [DataMember]
        DCSBStatus Status { get; set; }
        [DataMember]
        DCCompFileOutput OutFile { get; set; }
    }

    [DataContract]
    public class DCSBLoadResultSinglePallet : DCSBLoadResult
    {
        [DataMember]
        DCSBLoadResultPallet Result { get; set; }
    }

    public class DCSBLoadResultSingleContainer : DCSBLoadResult
    {
        [DataMember]
        DCSBLoadResultContainer Result { get; set; }
    }
}