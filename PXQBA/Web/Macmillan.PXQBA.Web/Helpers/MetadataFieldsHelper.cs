using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.Editor;
using Macmillan.PXQBA.Web.ViewModels.Filter;

namespace Macmillan.PXQBA.Web.Helpers
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
                             IsMultiline = false,
                             FilterType = FilterType.MultiSelectWithAddition.ToString().ToLower(),
                             AllowDeselect = true,
                             ColumnAppendAllowed = true,
                             CanCloseOnFilter = true,
                             CanUpdateSharedValue = false
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
                 model.CanCloseOnFilter = false;
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

            if (metaField.Name == MetadataFieldNames.TargetProductCourse)
            {
                model.ColumnAppendAllowed = false;
                model.FilterType = FilterType.None.ToString().ToLower();
            }

            if (metaField.Name == MetadataFieldNames.ContainsText)
            {
                model.ColumnAppendAllowed = false;
                model.CanCloseOnFilter = false;
                model.FilterType = FilterType.Text.ToString().ToLower();
            }

            if (metaField.Name == MetadataFieldNames.Chapter||
                metaField.Name == MetadataFieldNames.Bank)
            {
                model.IsInlineEditingAllowed = true;
            }

            return model;
         }
    }
}
