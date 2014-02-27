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
    public enum eVar_ConditionType
    {
        eVarCT_EQ = 1,
        eVarCT_NE = 2,
        eVarCT_LT = 3,
        eVarCT_LE = 4,
        eVarCT_GT = 5,
        eVarCT_GE = 6,
    }

    public class CConstraint
    {
        internal const double constStartNumber =  -2147483648;
        internal const double constEndNumber = 2147483647;

        internal static Random random = new Random();

        internal ArrayList _ranges = new ArrayList();
        internal StringCollection _inclusions = new StringCollection();
        internal StringCollection _exclusions = new StringCollection();
        internal CСondition _condition = null;
        internal CVardef _vardef = null;

        public CConstraint(XElement xEl, CVardef vardef)
        {
            _vardef = vardef;
            getRanges(xEl);
            getInclusions(xEl);
            getExclusions(xEl);
            getConditions(xEl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xEl">"constraint" node</param>
        private void getRanges(XElement xEl)
        {
            IEnumerable<XElement> ranges = xEl.XPathSelectElements("./range");
            foreach (XElement el in ranges)
            {
                CRange range = new CRange(el, _vardef);
                _ranges.Add(range);
            }
        }

        private void getInclusions(XElement xEl)
        {
            IEnumerable<XElement> exprs = xEl.XPathSelectElements("./inclusion/expr");
            if (exprs != null)
            {
                foreach (XElement el in exprs)
                {
                    string _expr = el.Value;
                    if (_expr != string.Empty)
                        _inclusions.Add(_expr);

                }
            }
        }
        private void getExclusions(XElement xEl)
        {
            IEnumerable<XElement> exprs = xEl.XPathSelectElements("./exclusion/expr");
            if (exprs != null)
            {
                foreach (XElement el in exprs)
                {
                    string _expr = el.Value;
                    if (_expr != string.Empty)
                        _exclusions.Add(_expr);
                }
            }
        }

        private void getConditions(XElement xEl)
        {
            XElement condition = xEl.XPathSelectElement("./condition");
            if (condition != null)
            {
                _condition = new CСondition(condition, _vardef);
            }
        }

        internal bool isRangesPresent()
        {
            return this._ranges.Count > 0;
        }

        internal bool isInclusionsPresent()
        {
            return this._inclusions.Count > 0;
        }

        internal bool isExclusionsPresent()
        {
            return this._exclusions.Count > 0;
        }

        internal bool isConditionPresent()
        {
            return this._condition != null;
        }

        internal string getRandomValueFromInclusion()
        {
            int index = CConstraint.random.Next(0, this._inclusions.Count);
            return (string)this._inclusions[index];
        }

        /// <summary>
        /// getting Random Value From presents in Constraint Ranges
        /// </summary>
        /// <param name="_format"></param>
        /// <param name="allranges">list of others ranges for checking value</param>
        /// <returns></returns>
        internal string getRandomValueFromRange(string _format, ArrayList allranges)
        {
            double numPrev = 0;
            bool limitsFlag = false;
            int k = 0;
            double val = 0;
            ArrayList line = new ArrayList();
            CRange range = null;
            string expr = "";

            for (int i = 0; i < _ranges.Count+1; i++)
            {
                double num = 0;
                if (i < _ranges.Count)
                {
                    range = (CRange)_ranges[i];
                    expr = this._vardef._problem.exprReplace(range.expression);
                    expr = this._vardef.doCalculate(expr);
                }
                try
                {
                    num = Convert.ToDouble(expr);
                    if (i == 0)
                    {
                        numPrev = num;
                        limitsFlag = false;
                        k = 0;
                        while (!limitsFlag)
                        {
                            val = CUtils.getNumberByCondition(constStartNumber, num, _format);
                            k++;
                            limitsFlag = this._vardef.checkRanges(val, allranges);
                            if (k > 20) break;
                        }

                    }
                    else if (i == _ranges.Count )
                    {
                        limitsFlag = false;
                        k = 0;
                        while (!limitsFlag)
                        {
                            val = CUtils.getNumberByCondition(numPrev, constEndNumber, _format);
                            k++;
                            limitsFlag = this._vardef.checkRanges(val, allranges);
                            if (k > 20) break;
                        }
                    }
                    else 
                    {
                        limitsFlag = false;
                        k = 0;
                        while (!limitsFlag)
                        {
                            val = CUtils.getNumberByCondition(numPrev, num, _format);

                            k++;
                            limitsFlag = this._vardef.checkRanges(val, allranges);
                            if (k > 20) break;
                        }
                        numPrev = num;
                    }
                    if (this._vardef.checkRanges(val, allranges))
                    {
                        line.Add(val);
                    }
                }
                catch { }
                
            }

            if (line.Count > 0)
            {
                int jj = CConstraint.random.Next(0, line.Count);
                val = Convert.ToDouble(line[jj]);
                return val.ToString();
            }
            else
            {
                throw new Exception(this._vardef.Name + " - VariableCondition::getRandomValueFromRange : Get random Number in the range have something wrong.");
            }
            return "0"; // TO DO Exception
        }

    }

    public class CRange
    {
        private eVar_ConditionType _conditionType = eVar_ConditionType.eVarCT_GT;
        private string _expr = "";
        private CVardef _vardef = null;

        /// <summary>
        /// Constructor from xml node
        /// </summary>
        /// <param name="xEl">"range" node</param>
        public CRange(XElement xEl, CVardef vardef)
        {
            _vardef = vardef;
            string cType = CUtils.getXElementAttribureValue(xEl, "type", "gt");
            _conditionType = CUtils.getConditionType(cType);

            XElement expr = xEl.XPathSelectElement("./expr");
            _expr = expr != null ? expr.Value : "0";
        }

        /// <summary>
        /// Constructor from explicit parameters
        /// </summary>
        /// <param name="conditionType"></param>
        /// <param name="_conditionExpr"></param>
        /// <param name="vardef"></param>
        public CRange(eVar_ConditionType conditionType, string _conditionExpr, CVardef vardef)
        {
            _vardef = vardef;
            _expr = _conditionExpr;
            _conditionType = conditionType;
        }
        public string expression
        {
            get { return _expr; }
            set { _expr = value; }
        }
        public eVar_ConditionType conditionType
        {
            get { return _conditionType; }
            set { _conditionType = value; }
        }

        /// <summary>
        /// Check if val belong to range
        /// </summary>
        /// <param name="val">checked value</param>
        /// <returns></returns>
        public bool doCheck(double val)
        {
            double expr = 0;
            string sExpr = _expr;
            try
            {
                sExpr = this._vardef._problem.exprReplace(sExpr);
                sExpr = this._vardef.doCalculate(sExpr);
                expr = Convert.ToDouble(sExpr);
            }
            catch { }

            switch (_conditionType)
            {
                case eVar_ConditionType.eVarCT_LT:
                    return (val < expr);
                    break;
                case eVar_ConditionType.eVarCT_LE:
                    return (val <= expr);
                    break;

                case eVar_ConditionType.eVarCT_GT:
                    return (val > expr);
                    break;

                case eVar_ConditionType.eVarCT_GE:
                    return (val >= expr);
                    break;

                default:
                    break;
            }
            return false;
        }
    }

    /// <summary>
    /// describes the condition in the constraint
    /// </summary>
    public class CСondition
    {
        private eVar_ConditionType _conditionType = eVar_ConditionType.eVarCT_EQ;
        private string _expr1 = "";
        private string _expr2 = "";
        private CVardef _vardef = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xEl">"condition" node</param>
        public CСondition(XElement xEl,CVardef vardef)
        {
            _vardef = vardef;
            string cType = CUtils.getXElementAttribureValue(xEl, "type", "gt");
            _conditionType = CUtils.getConditionType(cType);

            IEnumerable<XElement> exprs = xEl.XPathSelectElements("./expr");
            if (exprs != null)
            {
                XElement[] elems = exprs.ToArray();
                _expr1 = elems[0].Value;
                _expr2 = elems[1].Value;

            }
        }


        /// <summary>
        /// Check condition of conditional constraint
        /// </summary>
        /// <returns></returns>
        public bool doCheck()
        {
            double expr1 = 0;
            double expr2 = 0;

            string sExpr = _expr1;
            sExpr = this._vardef._problem.exprReplace(sExpr);
            sExpr = this._vardef.doCalculate(sExpr);
            expr1 = Convert.ToDouble(sExpr);

            sExpr = _expr2;
            sExpr = this._vardef._problem.exprReplace(sExpr);
            sExpr = this._vardef.doCalculate(sExpr);
            expr2 = Convert.ToDouble(sExpr);

            switch (_conditionType)
            {
                case eVar_ConditionType.eVarCT_EQ:
                    return expr1 == expr2;
                case eVar_ConditionType.eVarCT_NE:
                    return expr1 != expr2;
                case eVar_ConditionType.eVarCT_GE:
                    return expr1 >= expr2;
                case eVar_ConditionType.eVarCT_GT:
                    return expr1 > expr2;
                case eVar_ConditionType.eVarCT_LE:
                    return expr1 <= expr2;
                case eVar_ConditionType.eVarCT_LT:
                    return expr1 < expr2;
                default:
                    return false;
            }
        }
    }

}
