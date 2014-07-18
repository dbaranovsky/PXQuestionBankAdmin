using System.Xml.Linq;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    public class XmlConsts
    {
        public static XName ItemName = "item";
        public static XName IdAttrName = "ident";
        public static XName GeneralId = "general";
        public static XName ConditionVarName = "conditionvar";
        public static XName DisplayFeedbackElementName = "displayfeedback";
        public static XName PresentationName = "presentation";
        public static XName MattextName = "mattext";
        public static XName ChoiceElementName = "response_lid";
        public static XName FlowName = "flow";
        public static XName FeedBackElementName = "itemfeedback";
        public static XName AnswerElementName = "response_str";
        public static XName SetVarElementName = "setvar";
        public static XName MetadataElementName = "qtimetadatafield";
        public static XName MetaFieldIdName = "fieldlabel";
        public static XName MetaFieldValueName = "fieldentry";
        public static XName RepsonseVariableName = "respcondition";
        public static XName ResponseLabelName = "response_label";


        public static XName ChoiceTypeAttribute = "rcardinality";
        public static XName ActionAttribute = "action";
        public static XName TitleAttribute = "title";
        public static XName LinkRefIdAttribute = "linkrefid";
   
               
        public static string VarequalXPath = "conditionvar/varequal";
        public static string MattextXPath = "material/mattext";
        public static XName VarequalElementName = "varequal";
        public static XName RespIdAttribute = "respident";
    }
}
