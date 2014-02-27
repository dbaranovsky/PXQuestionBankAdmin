using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.QuestionEditor.Biz.DataContracts
{
    [DataContract]
    public class Choice
    {
        [DataMember]
        public string ChoiceId { get; set; }

        [DataMember]
        public string Fixed { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string Text { get; set; }
    }
}