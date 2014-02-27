using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.QuestionEditor.Biz.DataContracts
{
    [DataContract]
    public class Response
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Size { get; set; }        

        [DataMember]
        public string ElementId { get; set; }

        [DataMember]
        public string Correct { get; set; }

        [DataMember]
        public string Points { get; set; }

        [DataMember]
        public string AnswerRule { get; set; }

        [DataMember]
        public string Tolerance { get; set; }

        [DataMember]
        public string Format { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string AllowedWords { get; set; }

        [DataMember]
        public string CheckSyntax { get; set; }

        [DataMember]
        public string Scramble { get; set; }

        [DataMember]
        public string IncludeVariable { get; set; }

        [DataMember]
        public string Columns { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Style { get; set; }

        [DataMember]
        public List<Answer> Answers { get; set; }

        [DataMember]
        public string SigFigs { get; set; }

        [DataMember]
        public List<Choice> Choices { get; set; }
        
        [DataMember]
        public string ToleranceType { get; set; }
        
        public Response()
        {
            Answers = new List<Answer>();
            Choices = new List<Choice>();            
        }
    }
}