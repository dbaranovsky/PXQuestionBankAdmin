using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.QuestionEditor.Biz.DataContracts
{
    [DataContract]
    public class Property
    {

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}
