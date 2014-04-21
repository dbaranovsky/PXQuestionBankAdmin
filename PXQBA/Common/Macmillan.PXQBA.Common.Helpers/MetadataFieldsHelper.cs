using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Models.Web;
using Macmillan.PXQBA.Business.Models.Web.Editor;

namespace Macmillan.PXQBA.Common.Helpers
{
     public static class MetadataFieldsHelper
    {
        public static  QuestionFieldViewModel Convert(QuestionMetaField metaField)
         {
             var model = new QuestionFieldViewModel
                         {
                             MetadataName = metaField.Name,
                             FriendlyName = metaField.FriendlyName,
                             Width = "10%",
                             CanNotDelete = false,
                             IsInlineEditingAllowed = false,
                             EditorDescriptor =
                                 new FieldEditorDescriptor(metaField.TypeDescriptor),
                             CanAddValues = false,
                             IsMultiline = false
                         };

              // \todo Move custom field-level settings to configuration
             if (metaField.Name == MetadataFieldNames.DlapTitle)
             {
                 model.Width = "30%";
                 model.CanNotDelete = true;
                 model.IsMultiline = true;
             }

             if (metaField.Name == MetadataFieldNames.DlapStatus)
             {
                 model.IsInlineEditingAllowed = true;
                 model.EditorDescriptor.AvailableChoice = GetAvailibleChoicesFromEnum(typeof(QuestionStatus));
             }
             if (metaField.Name == MetadataFieldNames.DlapType)
             {
                 model.EditorDescriptor.AvailableChoice = GetAvailibleChoicesFromEnum(typeof(QuestionType));
             }

             if (metaField.Name == MetadataFieldNames.Sequence)
             {
                 model.IsInlineEditingAllowed = true;
                 model.EditorDescriptor.EditorType = EditorType.Number.ToString().ToLower();
             
             }

             if (metaField.Name == MetadataFieldNames.Keywords)
             {
                 model.CanAddValues = true;
                 model.Width = "20%";
             }

             if (metaField.Name == MetadataFieldNames.Guidance)
             {
                 model.IsMultiline = true;
                 model.Width = "30%";
             }

            return model;
         }

         private static Dictionary<string, string> GetAvailibleChoicesFromEnum(Type enumType)
         {
             return EnumHelper.GetEnumValues(enumType).ToDictionary(x => x.Key, x => x.Value);
         }


    }
}
