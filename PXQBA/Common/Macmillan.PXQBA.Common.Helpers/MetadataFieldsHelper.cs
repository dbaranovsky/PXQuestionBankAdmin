using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.Editor;
using Macmillan.PXQBA.Web.ViewModels.Filter;

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
                             IsMultiline = false,
                             FilterType = FilterType.MultiSelectWithAddition.ToString().ToLower(),
                             AllowDeselect = false,
                             ColumnAppendAllowed = true
                         };


            // \todo Move custom field-level settings to configuration
            
            if (metaField.Name == MetadataFieldNames.DlapTitle)
            {
                model.FilterType = FilterType.None.ToString().ToLower();
            }

             if (metaField.Name == MetadataFieldNames.DlapTitle)
             {
                 model.Width = "30%";
                 model.CanNotDelete = true;
             }

             if (metaField.Name == MetadataFieldNames.ProductCourse)
             {
                 model.FilterType = FilterType.SingleSelect.ToString().ToLower();
                 model.Width = "30%";
             }

             if (metaField.Name == MetadataFieldNames.QuestionStatus)
             {
                 model.IsInlineEditingAllowed = true;
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

            if (metaField.Name == MetadataFieldNames.Difficulty)
            {
                model.AllowDeselect = true;
            }

            if (metaField.Name == MetadataFieldNames.Flag)
            {
                model.ColumnAppendAllowed = false;
            }

            return model;
         }
    }
}
