#region Using directives
using System.Runtime.Serialization;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{
    [DataContract(Name = "OutFormat")]
    public enum EOutFormat
    {
        [EnumMember]
        NONE,
        [EnumMember]
        IMAGE,
        [EnumMember]
        CFF2,
        [EnumMember]
        DXF,
        [EnumMember]
        AI,
        [EnumMember]
        PDF,
        [EnumMember]
        DES
    }

    [DataContract]
    public class DCCompSize
    {
        [DataMember]
        public int CX { get; set; }
        [DataMember]
        public int CY { get; set; }
    }

    [DataContract]
    public class DCCompFormat
    {
        [DataMember]
        public EOutFormat Format { get; set; }
        [DataMember]
        public DCCompSize Size{ get; set; }
        [DataMember]
        public bool ShowCotations { get; set; } = true;
    }

    [DataContract]
    public class DCCompFileOutput
    {
        [DataMember]
        public DCCompFormat Format;
        [DataMember]
        public byte[] Bytes;
    }
}