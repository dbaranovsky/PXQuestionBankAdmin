using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Xml.XPath;
namespace HTS
{
    public class CConstraint
    {
        internal const double constStartNumber = -2147483648.0;
        internal const double constEndNumber = 2147483647.0;
        internal static Random random = new Random();
        internal ArrayList _ranges = new ArrayList();
        internal StringCollection _inclusions = new StringCollection();
        internal StringCollection _exclusions = new StringCollection();
        internal CCondition _condition;
        internal CVardef _vardef;
        public CConstraint(XElement xEl, CVardef vardef)
        {
            this._vardef = vardef;
            this.getRanges(xEl);
            this.getInclusions(xEl);
            this.getExclusions(xEl);
            this.getConditions(xEl);
        }
        private void getRanges(XElement xEl)
        {
            IEnumerable<XElement> enumerable = xEl.XPathSelectElements("./range");
            foreach (XElement current in enumerable)
            {
                CRange value = new CRange(current, this._vardef);
                this._ranges.Add(value);
            }
        }
        private void getInclusions(XElement xEl)
        {
            IEnumerable<XElement> enumerable = xEl.XPathSelectElements("./inclusion/expr");
            if (enumerable != null)
            {
                foreach (XElement current in enumerable)
                {
                    string value = current.Value;
                    if (value != string.Empty)
                    {
                        this._inclusions.Add(value);
                    }
                }
            }
        }
        private void getExclusions(XElement xEl)
        {
            IEnumerable<XElement> enumerable = xEl.XPathSelectElements("./exclusion/expr");
            if (enumerable != null)
            {
                foreach (XElement current in enumerable)
                {
                    string value = current.Value;
                    if (value != string.Empty)
                    {
                        this._exclusions.Add(value);
                    }
                }
            }
        }
        private void getConditions(XElement xEl)
        {
            XElement xElement = xEl.XPathSelectElement("./condition");
            if (xElement != null)
            {
                this._condition = new CCondition(xElement, this._vardef);
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
            return this._inclusions[index];
        }
        internal string getRandomValueFromRange(string _format, ArrayList allranges)
        {
            double start = 0.0;
            double num = 0.0;
            ArrayList arrayList = new ArrayList();
            string text = "";
            for (int i = 0; i < this._ranges.Count + 1; i++)
            {
                if (i < this._ranges.Count)
                {
                    CRange cRange = (CRange)this._ranges[i];
                    text = this._vardef._problem.exprReplace(cRange.expression);
                    text = this._vardef.doCalculate(text);
                }
                try
                {
                    double num2 = Convert.ToDouble(text);
                    if (i == 0)
                    {
                        start = num2;
                        bool flag = false;
                        int num3 = 0;
                        while (!flag)
                        {
                            num = CUtils.getNumberByCondition(-2147483648.0, num2, _format);
                            num3++;
                            flag = this._vardef.checkRanges(num, allranges);
                            if (num3 > 20)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (i == this._ranges.Count)
                        {
                            bool flag = false;
                            int num3 = 0;
                            while (!flag)
                            {
                                num = CUtils.getNumberByCondition(start, 2147483647.0, _format);
                                num3++;
                                flag = this._vardef.checkRanges(num, allranges);
                                if (num3 > 20)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            bool flag = false;
                            int num3 = 0;
                            while (!flag)
                            {
                                num = CUtils.getNumberByCondition(start, num2, _format);
                                num3++;
                                flag = this._vardef.checkRanges(num, allranges);
                                if (num3 > 20)
                                {
                                    break;
                                }
                            }
                            start = num2;
                        }
                    }
                    if (this._vardef.checkRanges(num, allranges))
                    {
                        arrayList.Add(num);
                    }
                }
                catch
                {
                }
            }
            if (arrayList.Count > 0)
            {
                int index = CConstraint.random.Next(0, arrayList.Count);
                num = Convert.ToDouble(arrayList[index]);
                return num.ToString();
            }
            throw new Exception(this._vardef.Name + " - VariableCondition::getRandomValueFromRange : Get random Number in the range have something wrong.");
        }
    }
}
