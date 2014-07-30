using System.ComponentModel;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Metadata field type
    /// </summary>
    public enum MetadataFieldType
    {
        [Description("Single Line")]
        Text = 0,

        [Description("Single value")]
        SingleSelect = 1,

        [Description("Multiple values")]
        MultiSelect = 2,

        [Description("Multiple Lines")]
        MultilineText = 3,

        [Description("Keywords")]
        Keywords = 4,

        [Description("Pairs of ItemID/ItemTitle")]
        ItemLink = 5
    }
}


 