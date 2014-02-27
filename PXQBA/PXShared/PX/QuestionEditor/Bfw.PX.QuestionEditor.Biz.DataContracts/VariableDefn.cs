using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.QuestionEditor.Biz.DataContracts
{
    [DataContract]
    public enum VarType
    {
        Numeric = 1,
        Math,
        Text,
        NumericArray,
        TextArray,
        MathArray
    }

    [DataContract]
    public enum ConditionType
    {
        Equal = 1,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }

    [DataContract]
    public class VariableDefn
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public VarType Type {get; set;}

        [DataMember]
        public string Format { get; set; }

        [DataMember]
        public List<Constraint> Constraints { get; set; }

        [DataMember]
        public bool ValidFlag { get; set; }

        public string GetValue()    
        {
            switch (Type)
            {
                case VarType.Numeric:
                    return GetNumericValue();
                case VarType.Text:
                    return GetTextValue();
                case VarType.Math:
                    return GetMathValue();
                case VarType.NumericArray:
                    return GetNumericArray();
                case VarType.MathArray:
                    return GetMathArray();
                case VarType.TextArray:
                    return GetTextArray();
                default:
                    return GetTextValue();

            }
        }
        private string GetNumericValue()
        {
            //bool isValid = false;
            string sNumeric = "0";
            List<Range> colRange = null;
            foreach (Constraint cnst in Constraints)
            {
                if (cnst.Ranges != null)
                {
                    if (colRange == null) colRange = new List<Range>();
                    foreach (Range rng in cnst.Ranges)
                    {
                        string sExpr = rng.Expression;

                        //TODO:formatting and rounding pending
                        sExpr = FormatNumber(sExpr);
                        colRange.Add(new Range() { Expression = sExpr, Type = rng.Type });
                    }
                }

                if (cnst.Inclusions != null && cnst.Condition == null)
                {
                }
                else if (cnst.Exclusions != null && cnst.Condition == null)
                {
                }
                else if (cnst.Inclusions != null && cnst.Condition != null)
                {
                }

            }

            return sNumeric;
        }

        public string GetTextValue()
        {
            return "";
        }

        public string GetMathValue()
        {
            return "";
        }

        public string GetNumericArray()
        {
            return "";
        }

        public string GetTextArray()
        {
            return "";
        }

        public string GetMathArray()
        {
            return "";
        }
        private string FormatNumber(string sNumber) { return sNumber.IndexOf("-") > 0 ? "(" + sNumber + ")" : sNumber; }
    }
}
