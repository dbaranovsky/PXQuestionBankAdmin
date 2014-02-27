using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PX.XBkPlayer.Biz.DataContracts
{
    [DataContract]
    public class XBkQuestion
    {
        [DataMember]
        public string EntityId { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string ResponseText { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public int Points { get; set; }

        [DataMember]
        public string ItemId { get; set; }

        [DataMember]
        public string Answer { get; set; }

        [DataMember]
        public string Feedback { get; set; }

        [DataMember]
        public List<string> MetaData { get; set; }

        [DataMember]
        public string InteractionData { get; set; }

        [DataMember]
        public string Height { get; set; }

        [DataMember]
        public string QuestionXML { get; set; }

        [DataMember]
        public string Hint { get; set; }

        [DataMember]
        public string Score { get; set; }

        [DataMember]
        public string QuizId { get; set; }
    }
}
