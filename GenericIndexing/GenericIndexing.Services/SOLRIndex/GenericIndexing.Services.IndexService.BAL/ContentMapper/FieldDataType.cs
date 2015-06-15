using System;

namespace GenericIndexing.IndexService.BAL
{
    /// <summary>
    /// Description of FieldDataType.
    /// </summary>
    public enum FieldDataType
    {
        StringField, 
        IntegerField, 
        BinaryField,
        DateField, 
        TextField,
        LinkField,
        ComplexLinkField,
        BinarySerializedField,
        CurrentNodePosition,
        ParentNodePosition,
        Multimedia,
        Multivalued
    }
}
