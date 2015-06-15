using System;
using System.Collections.Generic;

namespace GenericIndexing.IndexService.BAL
{
    /// <summary>
    /// Description of ContentTypeField.
    /// </summary>
    public class ContentTypeField
    {
        static readonly Dictionary<string, FieldDataType> contentTypes = new Dictionary<string, FieldDataType>
        {
            {"int",    FieldDataType.IntegerField },
            {"string", FieldDataType.StringField },
            {"date",   FieldDataType.DateField },
            {"text",   FieldDataType.TextField },
            {"binary", FieldDataType.BinaryField },
            {"link",   FieldDataType.LinkField },
            {"complexlink", FieldDataType.ComplexLinkField },
            {"binaryserialized", FieldDataType.BinarySerializedField },
            {"currentnodeindex", FieldDataType.CurrentNodePosition },
            {"parentnodeindex", FieldDataType.ParentNodePosition },
            {"multimedia", FieldDataType.Multimedia }
        };
        
        public ContentTypeField(string field_name, 
                                string data_type,
                                bool multi_valued,
                                string default_value,
                                bool is_constant,
                                string xpath,
                                string field_delimiter,
                                string transform_field,
                                bool is_key,
                                string target_field
                               )
        {
            this.fieldName = field_name;
            
            if(contentTypes.ContainsKey(data_type))
            {
                this.fieldDataType = contentTypes[data_type];
            }
            
            this.multiValued  = multi_valued;
            this.defaultValue = default_value;
            this.isConstant   = is_constant;
            this.componentPresentationXPath = xpath;
            this.complexFieldDelimiter = field_delimiter;
            this.targetFieldName = target_field;
            this.isKey = is_key;
            this.transformFieldName = transform_field;
        }
        
        string fieldName;
        
        public string field_name {
            get {
                return this.fieldName;
            }
        }
        
        FieldDataType fieldDataType;
        
        public FieldDataType field_data_type {
            get {
                return this.fieldDataType;
            }
        }
        
        bool multiValued;
        
        public bool is_multi_valued {
            get {
                return this.multiValued;
            }
        }
        
        string defaultValue;
        
        public string default_value {
            get {
                return this.defaultValue;
            }
        }
        
        bool isConstant;
        
        public bool is_constant {
            get {
                return this.isConstant;
            }
        }
        
        string componentPresentationXPath;
        
        
        public string cpxpath {
            get {
                return this.componentPresentationXPath;
            }
        }
        
        string complexFieldDelimiter;
        
        public string complex_field_delimiter {
            get {
                return this.complexFieldDelimiter;
            }
        }

        string transformFieldName;

        public string transform_field_name
        {
            get
            {
                return this.transformFieldName;
            }
        }
        
        bool isKey;
        
        public bool is_key {
            get {
                return this.isKey;
            }
        }
        
        string targetFieldName;
        
        public string target_field_name {
            get {
                return this.targetFieldName;
            }
        }
        
        public bool IsAutoDateField(){
            return (string.Equals(this.fieldDataType, FieldDataType.DateField) && 
                    string.IsNullOrEmpty(this.componentPresentationXPath) &&
                    string.Equals(this.defaultValue, "auto"));
        }

        public bool IsDateField()
        {
            return string.Equals(this.fieldDataType, 
                                 FieldDataType.DateField);
        }
        
        public bool IsTransformationRequired()
        {
            return !string.IsNullOrEmpty(this.transformFieldName);
        }
        
        public bool IsRTF(){
            return this.fieldDataType.Equals(FieldDataType.TextField);
        }
        
        public bool IsLink(){
            return string.Equals(this.fieldDataType, 
                                 FieldDataType.LinkField);
        }
        
        public bool IsComplexLink(){
            return string.Equals(this.fieldDataType, 
                                 FieldDataType.ComplexLinkField);
        }
        
        public bool IsBinarySerializedField(){
            return string.Equals(this.fieldDataType, 
                                 FieldDataType.BinarySerializedField);
        }

        public bool IsCurrentPositionNumber(){
            return string.Equals(this.fieldDataType,
                                 FieldDataType.CurrentNodePosition);
        }

        public bool IsParentPositionNumber()
        {
            return string.Equals(this.fieldDataType,
                                 FieldDataType.ParentNodePosition);
        }
        public bool IsMultimediaField()
        {
            return string.Equals(this.fieldDataType,
                                 FieldDataType.Multimedia);
        }
    }
}