using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Helpers
{
    /// <summary>
    /// Helper that is used to convert Course object to xml and vice versa
    /// </summary>
   public class CourseDataXmlHelper
   {
       /// <summary>
       /// Pattern for item link which is relative url to the item
       /// </summary>
       public const string ItemLinkPatterm = "#/launchpad/item/{0}?mode=preview";
       
       private const string itemLinksSectionName = "itemlinksfields";
       private  static readonly XName friendleNameAttribute = "friendlyname";
       private static readonly XName itemElementName = "item";
       private static readonly XName idNodeName = "id";
       private static readonly XName valueNodeName = "value";

       /// <summary>
       /// Loads the list of metadata field descriptors for item links field
       /// </summary>
       /// <param name="courseData"></param>
       /// <returns></returns>
       public static IEnumerable<CourseMetadataFieldDescriptor> GetItemLinksDescriptors(XElement courseData)
       {
            var allFields = new List<CourseMetadataFieldDescriptor>();
           if (courseData == null)
           {
               return allFields;
           }
          
           var itemLinkFields = courseData.Element(itemLinksSectionName);

           if (itemLinkFields == null || !itemLinkFields.Elements().Any())
           {
               return allFields;
           }

           foreach (var itemLinkField in itemLinkFields.Elements())
           {
                   allFields.Add(new CourseMetadataFieldDescriptor()
                                 {
                                     Type = MetadataFieldType.ItemLink,
                                     Name = itemLinkField.Name.ToString(),
                                     FriendlyName = itemLinkField.Attribute(friendleNameAttribute) == null ? string.Empty : itemLinkField.Attribute(friendleNameAttribute).Value,
                                     CourseMetadataFieldValues = GetItemLinksValues(itemLinkField)
                                 });
           }

           return allFields;
       }

       private static IEnumerable<CourseMetadataFieldValue> GetItemLinksValues(XElement itemLinkField)
       {
           var itemLinksValues = new List<CourseMetadataFieldValue>();
           var values = itemLinkField.Elements(itemElementName);
           if (!values.Any())
           {
               return itemLinksValues;
           }

           foreach (var value in values)
           {
               itemLinksValues.Add(new CourseMetadataFieldValue()
                                   {
                                       Id = value.Element(idNodeName) == null ? string.Empty : String.Format(ItemLinkPatterm, value.Element(idNodeName).Value),
                                       Text = value.Element(valueNodeName) == null ? string.Empty : value.Element(valueNodeName).Value,
                                   });
           }

           return itemLinksValues;
       }
   }
}
