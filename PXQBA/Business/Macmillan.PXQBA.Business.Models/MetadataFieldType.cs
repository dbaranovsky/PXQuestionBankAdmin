using System.ComponentModel;

namespace Macmillan.PXQBA.Business.Models
{
    public enum MetadataFieldType
    {
        [Description("Text")]
        Text = 0,

        [Description("Single select")]
        SingleSelect = 1,

        [Description("Multi select")]
        MultiSelect = 2,

        [Description("Multiline text")]
        MultilineText = 3,

        [Description("Keywords")]
        Keywords = 4,

        [Description("Item link")]
        ItemLink = 5
    }
}


 