﻿using System;
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
                                 new FieldEditorDescriptor(metaField.TypeDescriptor)
                         };

              // \todo Move custom field-level settings to configuration
             if (metaField.Name == MetadataFieldNames.DlapTitle)
             {
                 model.Width = "30%";
                 model.CanNotDelete = true;
                 model.LeftIcon = "glyphicon glyphicon-chevron-right titles-expander";
             }

             if (metaField.Name == MetadataFieldNames.DlapStatus)
             {
                 model.IsInlineEditingAllowed = true;
                 model.EditorDescriptor.AvailableChoice = GetAvailibleChoicesFromEnum(typeof(QuestionStatus));
             }

             if (metaField.Name == MetadataFieldNames.Sequence)
             {
                 model.IsInlineEditingAllowed = true;
                 model.EditorDescriptor.EditorType = EditorType.Number.ToString().ToLower();
             }

             return model;
         }

         private static Dictionary<string, string> GetAvailibleChoicesFromEnum(Type enumType)
         {
             return EnumHelper.GetEnumValues(enumType).ToDictionary(x => x.Key, x => x.Value);
         }


    }
}
