using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.QuestionEditor.Biz.DataContracts
{
    [DataContract]
    public class Answer
    {
        [DataMember]
        public string Correct { get; set; }

        [DataMember]
        public string Points { get; set; }

        [DataMember]
        public string AnswerRule { get; set; }

        [DataMember]
        public string SigFigs { get; set; }

        [DataMember]
        public string Tolerance { get; set; }

        [DataMember]
        public string ToleranceType { get; set; }
    }
}