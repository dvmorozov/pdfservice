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
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 
namespace HtmlCleanup.Config {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:xmlns:HTMLCleanupConfig")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:xmlns:HTMLCleanupConfig", IsNullable=false)]
    public partial class HTMLCleanupConfig {
        
        private TextProcessorType textProcessorConfigField;
        
        private ParagraphExtractorType paragraphExtractorConfigField;
        
        private SpecialHTMLRemoverType specialHTMLRemoverConfigField;
        
        private InnerTagRemoverType innerTagRemoverConfigField;
        
        private TagWithTextRemoverType tagWithTextRemoverConfigField;
        
        private URLFormatterType uRLFormatterConfigField;
        
        private TextFormatterType textFormatterConfigField;
        
        /// <remarks/>
        public TextProcessorType TextProcessorConfig {
            get {
                return this.textProcessorConfigField;
            }
            set {
                this.textProcessorConfigField = value;
            }
        }
        
        /// <remarks/>
        public ParagraphExtractorType ParagraphExtractorConfig {
            get {
                return this.paragraphExtractorConfigField;
            }
            set {
                this.paragraphExtractorConfigField = value;
            }
        }
        
        /// <remarks/>
        public SpecialHTMLRemoverType SpecialHTMLRemoverConfig {
            get {
                return this.specialHTMLRemoverConfigField;
            }
            set {
                this.specialHTMLRemoverConfigField = value;
            }
        }
        
        /// <remarks/>
        public InnerTagRemoverType InnerTagRemoverConfig {
            get {
                return this.innerTagRemoverConfigField;
            }
            set {
                this.innerTagRemoverConfigField = value;
            }
        }
        
        /// <remarks/>
        public TagWithTextRemoverType TagWithTextRemoverConfig {
            get {
                return this.tagWithTextRemoverConfigField;
            }
            set {
                this.tagWithTextRemoverConfigField = value;
            }
        }
        
        /// <remarks/>
        public URLFormatterType URLFormatterConfig {
            get {
                return this.uRLFormatterConfigField;
            }
            set {
                this.uRLFormatterConfigField = value;
            }
        }
        
        /// <remarks/>
        public TextFormatterType TextFormatterConfig {
            get {
                return this.textFormatterConfigField;
            }
            set {
                this.textFormatterConfigField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TextFormatterType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(URLFormatterType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TagWithTextRemoverType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(InnerTagRemoverType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SpecialHTMLRemoverType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ParagraphExtractorType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:xmlns:HTMLCleanupConfig")]
    public partial class TextProcessorType {
        
        private bool skippedField;
        
        /// <remarks/>
        public bool Skipped {
            get {
                return this.skippedField;
            }
            set {
                this.skippedField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:xmlns:HTMLCleanupConfig")]
    public partial class DelimiterSymbolType {
        
        private byte symbolCodeField;
        
        /// <remarks/>
        public byte SymbolCode {
            get {
                return this.symbolCodeField;
            }
            set {
                this.symbolCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:xmlns:HTMLCleanupConfig")]
    public partial class TagToRemoveType {
        
        private string startTagWithoutBracketField;
        
        private string endTagField;
        
        /// <remarks/>
        public string StartTagWithoutBracket {
            get {
                return this.startTagWithoutBracketField;
            }
            set {
                this.startTagWithoutBracketField = value;
            }
        }
        
        /// <remarks/>
        public string EndTag {
            get {
                return this.endTagField;
            }
            set {
                this.endTagField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:xmlns:HTMLCleanupConfig")]
    public partial class SpecialHTMLSymbolType {
        
        private string specialHTMLField;
        
        private string replacementField;
        
        /// <remarks/>
        public string SpecialHTML {
            get {
                return this.specialHTMLField;
            }
            set {
                this.specialHTMLField = value;
            }
        }
        
        /// <remarks/>
        public string Replacement {
            get {
                return this.replacementField;
            }
            set {
                this.replacementField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:xmlns:HTMLCleanupConfig")]
    public partial class TextFormatterType : TextProcessorType {
        
        private DelimiterSymbolType[] delimitersField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("DelimiterSymbol", IsNullable=false)]
        public DelimiterSymbolType[] Delimiters {
            get {
                return this.delimitersField;
            }
            set {
                this.delimitersField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:xmlns:HTMLCleanupConfig")]
    public partial class URLFormatterType : TextProcessorType {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:xmlns:HTMLCleanupConfig")]
    public partial class TagWithTextRemoverType : TextProcessorType {
        
        private TagToRemoveType[] tagsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Tag", IsNullable=false)]
        public TagToRemoveType[] Tags {
            get {
                return this.tagsField;
            }
            set {
                this.tagsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:xmlns:HTMLCleanupConfig")]
    public partial class InnerTagRemoverType : TextProcessorType {
        
        private TagToRemoveType[] tagsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Tag", IsNullable=false)]
        public TagToRemoveType[] Tags {
            get {
                return this.tagsField;
            }
            set {
                this.tagsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:xmlns:HTMLCleanupConfig")]
    public partial class SpecialHTMLRemoverType : TextProcessorType {
        
        private SpecialHTMLSymbolType[] specialHTMLField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("SpecialHTMLSymbol", IsNullable=false)]
        public SpecialHTMLSymbolType[] SpecialHTML {
            get {
                return this.specialHTMLField;
            }
            set {
                this.specialHTMLField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:xmlns:HTMLCleanupConfig")]
    public partial class ParagraphExtractorType : TextProcessorType {
    }
}
