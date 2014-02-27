using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace HTS
{
    public enum eVar_Type
    {
        eVar_Numeric = 1,
        eVar_Math = 2,
        eVar_Text = 3,
        eVar_NumericArray = 4,
        eVar_MathArray = 5,
        eVar_TextArray = 6,

    }
    public class CVardef
    {
        private string _name = "";
        private string _format = "#";
        private eVar_Type _type = eVar_Type.eVar_Numeric;
        private ArrayList _constraint = new ArrayList();

        internal CHTSProblem _problem = null;

        private object _value = "";

        private bool validFlag = false;

        public CVardef(string name, CHTSProblem problem)
        {
            _name = name;
            _problem = problem;
        }
        public CVardef(XElement xEl, CHTSProblem problem)
        {
            _problem = problem;
            _name = CUtils.getXElementAttribureValue(xEl, "name", string.Empty);
            _format = CUtils.getXElementAttribureValue(xEl, "format", "#");
            string t = CUtils.getXElementAttribureValue(xEl, "type", "numeric");
            _type = CUtils.varTypeFromString(t);
            initValue();
            getConstraints(xEl);
        }
        
        public CVardef(XElement xEl, CHTSResponse response)
        {
            _name = CUtils.getXElementAttribureValue(xEl, "name", string.Empty);
            string t = CUtils.getXElementAttribureValue(xEl, "type", "numeric");
            _type = CUtils.varTypeFromString(t);
            initValue();
            switch (_type)
            {
                case eVar_Type.eVar_NumericArray:
                case eVar_Type.eVar_MathArray:
                case eVar_Type.eVar_TextArray:
                    IEnumerable<XElement> values = xEl.XPathSelectElements("./value");
                    foreach (XElement el in values)
                    {
                        (_value as StringCollection).Add(el.Value);
                    }
                    break;
                case eVar_Type.eVar_Numeric:
                case eVar_Type.eVar_Math:
                case eVar_Type.eVar_Text:
                default:
                    _value = xEl.XPathSelectElement("value").Value;
                    break;
            }



        }

        private void initValue()
        {
            switch (_type)
            {
                case eVar_Type.eVar_NumericArray:
                case eVar_Type.eVar_MathArray:
                case eVar_Type.eVar_TextArray:
                    _value = new StringCollection();
                    break;
                case eVar_Type.eVar_Numeric:
                    _value = "0";
                    break;
                case eVar_Type.eVar_Math:
                case eVar_Type.eVar_Text:
                default:
                    _value = "";
                    break;
            }
        }

        private string typeToString()
        {
            return CUtils.varTypeNameToString(this._type);
        }

        private void getConstraints(XElement xEl)
        {
            IEnumerable<XElement> constraints = xEl.XPathSelectElements("./constraint");
            foreach (XElement el in constraints)
            {
                CConstraint constraint = new CConstraint(el, this);
                _constraint.Add(constraint);
            }
        }

        //properties
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public eVar_Type varType
        {
            get { return _type; }
            set { _type = value; }
        }
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        internal bool doVariable()
        {
            Dictionary<string, CVardef> vars = _problem.vardefs;

            switch (_type)
            {
                case eVar_Type.eVar_Numeric:
                    _value = this.getNumericVardefValue();
                    break;
                case eVar_Type.eVar_NumericArray:
                    _value = this.getNumericArrayVardefValue();
                    break;
                case eVar_Type.eVar_Math:
                    _value = this.getMathVardefValue();
                    break;
                case eVar_Type.eVar_MathArray:
                    _value = this.getMathArrayVardefValue();
                    break;
                case eVar_Type.eVar_Text:
                    _value = this.getTextVardefValue();
                    break;
                case eVar_Type.eVar_TextArray:
                    _value = this.getTextArrayVardefValue();
                    break;

                    
            }
            Console.WriteLine("var " + _name + " = " + _value.ToString());
            return validFlag;
        }

        public string toXMLString()
        {
            if (this._value == null) return "<vardef></vardef>";

            string str = "<vardef name=\"" + this._name + "\" type=\"" + this.typeToString() + "\" >";

            StringCollection val = null;
            switch (_type)
            {
                case eVar_Type.eVar_NumericArray:
                case eVar_Type.eVar_MathArray:
                case eVar_Type.eVar_TextArray:
                    val = _value as StringCollection;
                    if (val != null)
                    {
                        for (int i = 0; i < val.Count; i++)
                        {
                            str += "<value id=\"" + i.ToString() + "\">" + val[i] + "</value>";
                        }
                    }
                    break;
                case eVar_Type.eVar_Numeric:
                case eVar_Type.eVar_Math:
                case eVar_Type.eVar_Text:
                default:
                    str += "<value id=\"1\">" + _value.ToString() + "</value>";
                    break;
            }
            str += "</vardef>";

            return str;

        }
        
        public string valueToString()
        {
            if (_value == null ) return "null";

            string str = string.Empty;

            StringCollection val = null;
            switch (_type)
            {
                case eVar_Type.eVar_NumericArray:
                case eVar_Type.eVar_MathArray:
                case eVar_Type.eVar_TextArray:
                    val = _value as StringCollection;
                    if (val == null) return "null";
                    for (int i = 0; i < val.Count; i++)
                    {
                        if (i != 0) str += ",";
                        str += val[i];
                    }
                    return str;
                case eVar_Type.eVar_Numeric:
                case eVar_Type.eVar_Math:
                case eVar_Type.eVar_Text:
                default:
                    return _value.ToString();


            }

        }

        internal string getNumericVardefValue()
        {
            validFlag = false;
            string numeric = "0";
            ArrayList ranges = new ArrayList();
            for (int i = 0; i < _constraint.Count; i++)
            {
                CConstraint cnstr = (CConstraint)_constraint[i];

                // a)	all range types present in the current set are added to the global set of ranges
                if (cnstr.isRangesPresent())
                {
                    for (int k = 0; k < cnstr._ranges.Count; k++)
                    {
                        CRange range = (CRange)cnstr._ranges[k];

                        string expr = range.expression;
                        expr = this.minusCheck(expr);
                        expr = this._problem.exprReplace(expr);
                        numeric = this.doCalculate(expr);
                        CRange newRange = new CRange(range.conditionType, numeric, this);
                        ranges.Add(newRange);
                    }
                }

                if (cnstr.isInclusionsPresent() && (!cnstr.isConditionPresent()))
                {
                    string expr = cnstr.getRandomValueFromInclusion();
                    expr = this._problem.exprReplace(expr);
                    expr = this.minusCheck(expr);
                    numeric = this.doCalculate(expr);
                    if (this.checkRanges(Convert.ToDouble(numeric), ranges)) //check for ranges
                    {
                        validFlag = checkExclusion(numeric, cnstr._exclusions, _format);
                    }

                }
                else if (cnstr.isRangesPresent() && (!cnstr.isConditionPresent()))
                {
                    int flagExclusionCount = 0;
                    while (!validFlag && flagExclusionCount < 50)
                    {
                        try
                        {
                            numeric = cnstr.getRandomValueFromRange(_format, ranges);
                        }
                        catch (Exception e)
                        {
                            //trace(e);
                            Console.WriteLine(e.Message);
                            //throw new NaNVariableException("VariableCondition::getNumericVardefValue : Get variable \"" + name + "\",the range have error.\n" + e.message);
                        }
                        validFlag = this.checkExclusion(numeric, cnstr._exclusions, _format);
                        if (!validFlag)
                        {
                            flagExclusionCount++;
                        }
                        else
                        {
                            Console.WriteLine("~" + _name + "\\" + " : " + numeric);
                        }

                    }

                }
                else if (cnstr.isInclusionsPresent() && (cnstr.isConditionPresent()))
                {
                    string expr = cnstr.getRandomValueFromInclusion();
                    expr = this._problem.exprReplace(expr);
                    expr = this.minusCheck(expr);
                    if (cnstr._condition.doCheck())
                    {
                        numeric = this.doCalculate(expr);
                    }

                }
            }
            return numeric;
        }
        private StringCollection getNumericArrayVardefValue()
        {
            object numeric = "0";
            StringCollection numArray = new StringCollection();
            for (int j = 0; j < this._constraint.Count; j++)
            {
                CConstraint cnstr = (CConstraint)this._constraint[j];
                for (int k = 0; k < cnstr._inclusions.Count; k++)
                {
                    string expr = (string)cnstr._inclusions[k];
                    expr = this._problem.exprReplace(expr);
                    expr = this.minusCheck(expr);
                    numeric = this.doCalculate(expr);

                    //numeric = this._problem.doCalculate(expr);
                    //if (numeric == null) numeric = "0";
                    //numeric = CUtils.formatNumber(numeric.ToString(), _format);
                    numArray.Add(numeric.ToString());
                }
            }
            return numArray;
        }

        private object getMathVardefValue()
        {
            string math = valueToString();
            string expr = "";

            for (int i = 0; i < _constraint.Count; i++)
            {
                CConstraint cnstr = (CConstraint)_constraint[i];
                if (cnstr.isInclusionsPresent() )
                {
                    expr = cnstr.getRandomValueFromInclusion();
                    expr = this._problem.exprReplace(expr);
                }
                if (!cnstr.isConditionPresent())
                {
                    math = expr;
                }
                else
                {
                    if (cnstr._condition.doCheck())
                    {
                        math = expr;
                    }
                }
            }
            //math = CUtils.reduceMath(math);

            return math;
        }

        private StringCollection getMathArrayVardefValue()
        {
            StringCollection mathArray = new StringCollection();
            for (int j = 0; j < this._constraint.Count; j++)
            {
                CConstraint cnstr = (CConstraint)this._constraint[j];
                for (int k = 0; k < cnstr._inclusions.Count; k++)
                {
                    string expr = (string)cnstr._inclusions[k];
                    expr = this._problem.exprReplace(expr);
                    //expr = CUtils.reduceMath(expr);
                    mathArray.Add(expr);
                }
            }
            return mathArray;

        }

        private string getTextVardefValue()
        {
            string text = valueToString();
            string expr = "";

            for (int i = 0; i < _constraint.Count; i++)
            {
                CConstraint cnstr = (CConstraint)_constraint[i];
                if (cnstr.isInclusionsPresent())
                {
                    expr = cnstr.getRandomValueFromInclusion();
                    expr = this._problem.exprReplace(expr);
                }
                if (!cnstr.isConditionPresent())
                {
                    text = expr;
                }
                else
                {
                    if (cnstr._condition.doCheck())
                    {
                        text = expr;
                    }
                }
            }
            return text;
        }
        private StringCollection getTextArrayVardefValue()
        {
            string text = "";
            StringCollection textArray = new StringCollection();
            for (int j = 0; j < this._constraint.Count; j++)
            {
                CConstraint cnstr = (CConstraint)this._constraint[j];
                for (int k = 0; k < cnstr._inclusions.Count; k++)
                {
                    text = (string)cnstr._inclusions[k];
                    text = this._problem.exprReplace(text);
                    textArray.Add(text);
                }
            }
            return textArray;
        }

        private bool checkExclusion(string numeric, StringCollection exclusions, string _format)
        {
			bool flag = true;
			if(exclusions != null && exclusions.Count > 0)
			{
				for(int k  = 0; k < exclusions.Count; k++)
				{
					string exclusion = (string)exclusions[k];
                    exclusion = this._problem.exprReplace(exclusion);
                    exclusion = this.minusCheck(exclusion);
                    exclusion = this.doCalculate(exclusion);
                    try
                    {
                        if (Convert.ToDouble(numeric) == Convert.ToDouble(exclusion))
                        {
                            flag = false;
                            break;
                        }
                    }
                    catch { }
				 }
			}
			return flag;
        }

        internal string doCalculate(string expr)
        {
            object numeric = "0";
            numeric = this._problem.doCalculate(expr);
            if (numeric == null) numeric = "0";
            numeric = CUtils.formatNumber(numeric.ToString(), _format);
            return numeric.ToString();

        }

        internal bool checkRanges(double num, ArrayList ranges)
		{
			if(ranges != null)
			{
                for (int i = 0; i < ranges.Count; i++)
				{
                    CRange range = (CRange)ranges[i];
                    if (! range.doCheck(num))
                    {
                        return false;
                    }
				}
			} 
			
            return true;
		}


        private StringCollection getArrayValue()
        {
            return _value as StringCollection;
        }

        private string minusCheck(string expr)
        {
            if (expr != string.Empty)
            {
                if (expr[0] == '-')
                {
                    expr = "(" + expr + ")";
                }
            }
            return expr;
        }


        public XElement getVardef()
        {
            XElement constraint = new XElement("constraint");
            if ((this._type == eVar_Type.eVar_Math) || (this._type == eVar_Type.eVar_Text) || (this._type == eVar_Type.eVar_Numeric))
            {
                constraint.Add(new XElement ("inclusion",
                                    new XElement("expr", this.valueToString())));
            }
            else
            {
                StringCollection elems = this.getArrayValue();
                for (int i = 0; i < elems.Count; i++)
                {
                    constraint.Add(new XElement ("inclusion",
                                    new XElement("expr", elems[i])));
                }
            }

            XElement vdef = new XElement("vardef",
                            new XAttribute("name", this._name),
                            new XAttribute("type", this.typeToString()),
                            new XAttribute("format", this._format),
                            constraint
                                );
            return vdef;
        }
    }
}
