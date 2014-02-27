using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.QuestionEditor.Biz.DataContracts
{
    [DataContract]
    public class Variable
    {
        private string sSortType = "R";

        [DataMember]
        public List<Constraint> Constraints { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Type {get; set;}

        [DataMember]
        public string SortType
        {
            get
            {
                switch (Type.IndexOf("array") > 0)
                {
                    case true:
                        sSortType = "A"; //array
                        break;
                    case false:
                        sSortType = "R";
                        break;
                }

                return sSortType;
            }

            set {
                sSortType = value;
            }
        }
        [DataMember]
        public int Sequence { get; set; }

        [DataMember]
        public string Format { get; set; }

        [DataMember]
        public string OldName { get; set; }      

        public Variable()
        {
            Constraints = new List<Constraint>();            
        }
        
        private string FormatNumber(string sNumber) { return sNumber.IndexOf("-") > 0 ? "(" + sNumber + ")" : sNumber; }

        public string[] ArrayValues {
            get{
                var inclusionStrings = new string[]{};

                if(this.Constraints != null && this.Constraints.Count > 0)
                {
                    var constraint = this.Constraints[0];
                    if(constraint.Inclusions != null)
                    {
                        inclusionStrings = !constraint.Inclusions.ToLower().Contains("binompdf") ? constraint.Inclusions.Split(',') : new string[]{constraint.Inclusions};
                    }
                }

                return inclusionStrings;
            }        
        }
    }
}
