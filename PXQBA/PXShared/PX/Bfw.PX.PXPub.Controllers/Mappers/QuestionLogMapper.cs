using System.Linq;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using System.Xml.Linq;
namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class QuestionLogMapper
    {

        /// <summary>
        /// Map a QuestionNote business object to a QuestionNote model.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static QuestionLog ToQuestionNote(this BizDC.QuestionLog biz)
        {
            var model = new QuestionLog
                            {
                                Id = biz.Id,
                                QuestionId = biz.QuestionId,
                                CourseId = biz.CourseId,
                                Type = biz.EventType.ToString(),
                                Created = biz.Created,
                                UserId = biz.UserId,
                                FirstName = biz.FirstName,
                                LastName = biz.LastName,
                                Version = string.IsNullOrEmpty(biz.Version) == true ? "" : biz.Version
                            };

            // parse the xml into another field for easy display on the view
            if (!string.IsNullOrEmpty(biz.ChangesMadeXML))
            {
                if (model.ParsedFields == null)
                    model.ParsedFields = new System.Collections.Generic.List<QuestionFieldLog>() { };
                string s = biz.ChangesMadeXML.ToString();
                XDocument doc = XDocument.Parse(s);
                if (doc.Root.Elements().Count() > 0)
                {
                    QuestionFieldLog parsedItem = null;
                    foreach ( var node in doc.Root.Elements())
                    {
                        parsedItem = new QuestionFieldLog()
                        {
                            Field = node.Element("field").Value,
                            OriginalValue = node.Element("orig").Value,
                            NewValue = node.Element("new").Value
                        };
                        parsedItem.OriginalValue = parsedItem.OriginalValue.Replace("||", " </br> ");
                        parsedItem.NewValue = parsedItem.NewValue.Replace("||", " </br> ");
                        model.ParsedFields.Add(parsedItem);
                    }
                }
            }
            return model;
        }
    }
}
