﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 
namespace JJA.InputData {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://JJAnamespace")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://JJAnamespace", IsNullable=false)]
    public partial class input {
        
        private inputContainer[] containersField;
        
        private inputPallet[] palletsField;
        
        private inputCase[] casesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("container", IsNullable=false)]
        public inputContainer[] containers {
            get {
                return this.containersField;
            }
            set {
                this.containersField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("pallet", IsNullable=false)]
        public inputPallet[] pallets {
            get {
                return this.palletsField;
            }
            set {
                this.palletsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("case", IsNullable=false)]
        public inputCase[] cases {
            get {
                return this.casesField;
            }
            set {
                this.casesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://JJAnamespace")]
    public partial class inputContainer {
        
        private string nameField;
        
        private int colorField;
        
        private double[] dimensionsField;
        
        private double maxLoadWeightField;
        
        private bool maxLoadWeightFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int color {
            get {
                return this.colorField;
            }
            set {
                this.colorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double[] dimensions {
            get {
                return this.dimensionsField;
            }
            set {
                this.dimensionsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double maxLoadWeight {
            get {
                return this.maxLoadWeightField;
            }
            set {
                this.maxLoadWeightField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxLoadWeightSpecified {
            get {
                return this.maxLoadWeightFieldSpecified;
            }
            set {
                this.maxLoadWeightFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://JJAnamespace")]
    public partial class inputPallet {
        
        private string nameField;
        
        private int colorField;
        
        private string typeField;
        
        private double[] dimensionsField;
        
        private double weightField;
        
        private double maxPalletHeightField;
        
        private double maxLoadWeightField;
        
        private bool maxLoadWeightFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int color {
            get {
                return this.colorField;
            }
            set {
                this.colorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double[] dimensions {
            get {
                return this.dimensionsField;
            }
            set {
                this.dimensionsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double weight {
            get {
                return this.weightField;
            }
            set {
                this.weightField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double maxPalletHeight {
            get {
                return this.maxPalletHeightField;
            }
            set {
                this.maxPalletHeightField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double maxLoadWeight {
            get {
                return this.maxLoadWeightField;
            }
            set {
                this.maxLoadWeightField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxLoadWeightSpecified {
            get {
                return this.maxLoadWeightFieldSpecified;
            }
            set {
                this.maxLoadWeightFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://JJAnamespace")]
    public partial class inputCase {
        
        private string nameField;
        
        private int colorField;
        
        private double[] dimensionsField;
        
        private double weightField;
        
        private int pcbField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int color {
            get {
                return this.colorField;
            }
            set {
                this.colorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double[] dimensions {
            get {
                return this.dimensionsField;
            }
            set {
                this.dimensionsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double weight {
            get {
                return this.weightField;
            }
            set {
                this.weightField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int pcb {
            get {
                return this.pcbField;
            }
            set {
                this.pcbField = value;
            }
        }
    }
}
