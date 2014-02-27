using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Runtime.Serialization;
using HtmlAgilityPack;
using System.IO;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace Bfw.PX.QuestionEditor.Biz.DataContracts
{
    using VariableDictionary = Dictionary<string, Variable>;
    /// <summary>
    /// Helper class to build HTS xml.
    /// </summary>
    public static class HTSHelper
    {
        #region "XML generation methods"

        /// <summary>
        /// Helper method to create an new xml node representing an HTS variable.
        /// </summary>
        /// <param name="xDoc">The xml document to create the node for.</param>
        /// <param name="stepId">The id of the new node.</param>
        /// <param name="stepText">Inner text of the new step node.</param>
        /// <returns></returns>
        public static XmlNode CreateVariableNode(this XmlDocument xDoc, Variable v)
        {
            XmlNode varNode = xDoc.CreateElement("vardef");
            XmlAttribute attr = null;

            attr = xDoc.CreateAttribute("name");
            attr.Value = v.Name;
            varNode.Attributes.Append(attr);

            attr = xDoc.CreateAttribute("type");
            attr.Value = v.Type;
            varNode.Attributes.Append(attr);

            if (v.Format != null)
            {
                attr = xDoc.CreateAttribute("format");
                string format = "#";
                int decimalPlaces;
                if (int.TryParse(v.Format, out decimalPlaces))
                {
                    if (decimalPlaces > 0)
                    {
                        format += ".";
                        for (int i = 0; i < decimalPlaces; i++)
                        {
                            format += "#";
                        }
                    }
                }

                attr.Value = format;
                varNode.Attributes.Append(attr);
            }

            if (v.Constraints != null)
            {
                foreach (Constraint c in v.Constraints)
                {
                    var cNode = xDoc.CreateConstraintNode(c);
                    varNode.AppendChild(cNode);
                }
            }

            return varNode;
        }

        public static XmlNode CreateConstraintNode(this XmlDocument xDoc, Constraint c)
        {
            XmlNode cNode = xDoc.CreateElement("constraint");
            XmlAttribute attr = null;

            if (c.Ranges != null)
            {
                foreach (Range r in c.Ranges)
                {
                    XmlNode rangeNode = xDoc.CreateElement("range");
                    attr = xDoc.CreateAttribute("type");
                    attr.Value = r.Type;
                    rangeNode.Attributes.Append(attr);

                    XmlNode exprNode = xDoc.CreateElement("expr");
                    exprNode.InnerText = r.Expression;

                    rangeNode.AppendChild(exprNode);
                    cNode.AppendChild(rangeNode);
                }
            }

            if (c.Inclusions != null) 
            {
                XmlNode inclusionNode = xDoc.CreateElement("inclusion"); 
                var inclusionValues = c.Inclusions.ToLower().Contains("binompdf")? new string[]{c.Inclusions}: c.Inclusions.Split(',');
                foreach (string inclusion in inclusionValues)
                {
                    XmlNode exprNode = xDoc.CreateElement("expr");
                    exprNode.InnerText = inclusion;
                    inclusionNode.AppendChild(exprNode);
                }

                cNode.AppendChild(inclusionNode);
            }

            if (c.Exclusions != null)
            {
                XmlNode exclusionNode = xDoc.CreateElement("exclusion");
                var exclusionValues = c.Exclusions.Split(',');
                foreach (string exclusion in exclusionValues)
                {

                    XmlNode exprNode = xDoc.CreateElement("expr");
                    exprNode.InnerText = exclusion;
                    exclusionNode.AppendChild(exprNode);
                }

                cNode.AppendChild(exclusionNode);
            }

            if (c.Condition != null)
            {
                XmlNode conditionNode = xDoc.CreateElement("condition");
                attr = xDoc.CreateAttribute("type");
                attr.Value = c.Condition.Type;
                conditionNode.Attributes.Append(attr);

                XmlNode exprNode1 = xDoc.CreateElement("expr");
                exprNode1.InnerText = c.Condition.Expression1;

                conditionNode.AppendChild(exprNode1);

                XmlNode exprNode2 = xDoc.CreateElement("expr");
                exprNode2.InnerText = c.Condition.Expression2;

                conditionNode.AppendChild(exprNode2);
                cNode.AppendChild(conditionNode);
            }

            return cNode;
        }



        /// <summary>
        /// Helper method to create an new xml node representing an HTS step.
        /// </summary>
        /// <param name="xDoc">The xml document to create the node for.</param>
        /// <param name="stepId">The id of the new node.</param>
        /// <param name="stepText">Inner text of the new step node.</param>
        /// <returns></returns>
        public static XmlNode CreateStepNode(this XmlDocument xDoc, HTSData htsData, string stepId, string stepText)
        {
            XmlNode stepNode = xDoc.CreateElement("iprostep");
            XmlAttribute attr = xDoc.CreateAttribute("id");
            attr.Value = stepId;
            stepNode.Attributes.Append(attr);

            if (string.IsNullOrEmpty(stepText))
            {
                stepText = string.Empty;
            }

            stepNode.InnerXml = stepText;// HTSHelper.ReplaceHTMLSymbols(stepText); //HTSHelper.ReplaceHTMLSymbols(stepText); /* Regex.Replace(HTSHelper.ReplaceHTMLSymbols(stepText), "&(?!amp;)", "&amp;");  */ /* Regex.Replace(stepText, "&(?!amp;)", "&amp;"); */ //stepText.Replace("&", "&amp;");

            var stepHtml = stepNode.ToValidXml(htsData);
            stepNode.InnerXml = stepHtml.ToHtmlString(); // HTSHelper.ReplaceHTMLSymbols(stepHtml.ToHtmlString());

            return stepNode;
        }

        public static HtmlDocument ToHtmlDocument(this XmlNode stepNode)
        {
            //load and change markup                            
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            HtmlNode.ElementsFlags["img"] = HtmlElementFlag.Empty;
            HtmlNode.ElementsFlags["input"] = HtmlElementFlag.Empty;
            HtmlNode.ElementsFlags["br"] = HtmlElementFlag.Empty;

            htmlDoc.OptionWriteEmptyNodes = true;            
            string stepContent = stepNode.InnerXml;

            if (!string.IsNullOrEmpty(stepContent))
            {
                htmlDoc.LoadHtml(stepContent);
            }
            else
            {
                htmlDoc.LoadHtml("");
            }

            return htmlDoc;
        }

        public static HtmlDocument ConvertFormulasToImages(this HtmlDocument doc, HTSData htsData)
        {
            List<Property> colAnswer = new List<Property>();

            var nodes = doc.DocumentNode.SelectNodes("//iproformula");

            if (nodes == null) return doc;

            foreach (HtmlNode node in nodes)
            {
                var strFormulaPath = node.GetAttributeValue("src", "").Replace("_", "-");
                //strFormulaPath = HttpUtility.HtmlDecode(strFormulaPath);

                var strFormulaText = strFormulaPath.Replace("geteq.ashx?eqtext=", "").Replace("geteq.ashx?exprtext=", "");
                strFormulaPath = strFormulaPath.Replace(strFormulaText, HttpUtility.UrlEncode(strFormulaText));
                
                string strFormulaUrlBase = htsData.FormulaEditorUrl.Replace("geteq.ashx", "");               
                
                var strSrc = strFormulaUrlBase + strFormulaPath;

                var imgNode = HtmlNode.CreateNode("<img />");
                foreach (HtmlAttribute attr in node.Attributes)
                {
                    colAnswer.Add(new Property() { Name = attr.Name, Value = attr.Value });
                    imgNode.Attributes.Add(attr.Name, attr.Value);
                }

                imgNode.Attributes.Add("iprof", "formula");
                imgNode.Attributes.Add("hts-data-type", "formula");
                imgNode.SetAttributeValue("src", strSrc);
                imgNode.SetAttributeValue("alt", strSrc);
                imgNode.SetAttributeValue("hts-data-equation", HttpUtility.UrlEncode(strFormulaText));

                node.ParentNode.ReplaceChild(imgNode, node);
            }

            return doc;
        }
        //private static string ReplaceSpecialChars(string s, bool bAdditional)
        //{
        //    string sLcl = s;

        //    if (bAdditional)
        //    {
        //        if (sLcl.IndexOf("+") > 0) sLcl = sLcl.Replace("+", "%2b");

        //        // if (sLcl.IndexOf("&")>0) sLcl = sLcl.Replace("&", "%26");

        //        // if (sLcl.IndexOf(",")>0) sLcl = sLcl.Replace(",", "%2C");

        //        //// if (sLcl.IndexOf("//")>0) sLcl = sLcl.Replace("//", "%2F");

        //        // if (sLcl.IndexOf(":")>0) sLcl = sLcl.Replace(":", "%3A");

        //        // if (sLcl.IndexOf(";")>0) sLcl = sLcl.Replace(";", "%3B");

        //        // if (sLcl.IndexOf("=")>0) sLcl = sLcl.Replace("=", "%3D");

        //        // if (sLcl.IndexOf("@")>0) sLcl = sLcl.Replace("+", "%40");
        //    }
        //    return sLcl;
        //}

        public static List<Response> ConvertMCToImages(this HtmlDocument doc, HTSData htsData)
        {
            List<Response> responses = new List<Response>();

            var nodes = doc.DocumentNode.SelectNodes("//iproelement_mc");

            if (nodes == null) return responses;

            foreach (HtmlNode node in nodes)
            {
                Response response = node.ToMultiResponse();
                responses.Add(response);

                HtmlNode imgNode = node.ToHtsImageNode(htsData);

                node.ParentNode.ReplaceChild(imgNode, node);
            }

            return responses;
        }

        public static List<Response> ConvertShortToImages(this HtmlDocument doc, HTSData htsData)
        {
            List<Response> responses = new List<Response>();

            var nodes = doc.DocumentNode.SelectNodes("//iproelement_short");

            if (nodes == null) return responses;

            foreach (HtmlNode node in nodes)
            {
                //add response to collection
                Response response = node.ToResponse();
                responses.Add(response);

                //convert response(iproshortelement) to image node
                HtmlNode imgNode = node.ToHtsImageNode(htsData);

                node.ParentNode.ReplaceChild(imgNode, node);
            }

            return responses;
        }

        public static HtmlDocument ConvertGraphsToImages(this HtmlDocument doc, HTSData htsData)
        {
            List<Response> responses = new List<Response>();

            var nodes = doc.DocumentNode.SelectNodes("//iproelement");

            if (nodes == null) return doc;

            foreach (HtmlNode node in nodes)
            {
                //add response to collection
                //Response response = node.ToResponse();
                //responses.Add(response);

                //convert response(iproshortelement) to image node
                HtmlNode imgNode = node.ToHtsImageNode(htsData);

                node.ParentNode.ReplaceChild(imgNode, node);
            }

            return doc;
        }

        public static Step ToStep(this XmlNode stepNode, HTSData htsData)
        {
            Step oStep = new Step();

            string strQuestion = stepNode.ToValidHtml(htsData);

            string stepId = stepNode.Attributes["id"].Value;
            string sequence = stepId.Replace("step", "");

            oStep.Id = stepId;
            oStep.Sequence = Convert.ToInt32(sequence);
            oStep.Question = strQuestion;

            return oStep;
        }

        public static Constraint ToConstraint(this XmlNode constraintNode)
        {

            Constraint constraint = new Constraint();
            try
            {
                var rangeNodes = constraintNode.SelectNodes("range");
                if (rangeNodes.Count > 0)
                {
                    List<Range> ranges = new List<Range>();
                    foreach (XmlNode rangeNode in rangeNodes)
                    {
                        Range range = new Range();
                        range.Type = rangeNode.Attributes["type"].Value;
                        var exprNode = rangeNode.SelectSingleNode("expr");
                        range.Expression = exprNode.InnerXml;
                        ranges.Add(range);
                    }
                    constraint.Ranges = ranges;
                }

                var exclusionNodes = constraintNode.SelectNodes("exclusion");
                if (exclusionNodes != null && exclusionNodes.Count > 0)
                {
                    StringBuilder allExclusions = new StringBuilder();
                    allExclusions = IncludeExcludeNode(exclusionNodes, allExclusions);

                    if (allExclusions.Length > 0)
                    {
                        constraint.Exclusions = allExclusions.ToString();// HttpUtility.UrlDecode(allExclusions.ToString());
                    }
                }

                var inclusionNodes = constraintNode.SelectNodes("inclusion");
                if (inclusionNodes != null && inclusionNodes.Count > 0)
                {
                    StringBuilder allInclusions = new StringBuilder();
                    allInclusions = IncludeExcludeNode(inclusionNodes, allInclusions);

                    if (allInclusions.Length > 0)
                    {
                        constraint.Inclusions = allInclusions.ToString();// HttpUtility.UrlDecode(allInclusions.ToString());
                    }
                }

                var conditionNode = constraintNode.SelectSingleNode("condition");
                if (conditionNode != null)
                {
                    Condition condition = new Condition();
                    condition.Type = conditionNode.Attributes["type"].Value;
                    var exprNodes = conditionNode.SelectNodes("expr");
                    if (exprNodes.Count > 0)
                    {
                        condition.Expression1 = exprNodes[0].InnerText;
                        condition.Expression2 = exprNodes[1].InnerText;
                    }
                    constraint.Condition = condition;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Question XML Constraint is invalid!");
            }

            return constraint;
        }

        private static StringBuilder IncludeExcludeNode(XmlNodeList inclusionExclusionNodes, StringBuilder allInclusionExclusions)
        {
            foreach (XmlNode inclusionNode in inclusionExclusionNodes)
            {
                var exprNodes = inclusionNode.SelectNodes("expr");
                if (exprNodes.Count > 0)
                {
                    List<string> inclusionList = new List<string>();
                    foreach (XmlNode exprNode in exprNodes)
                    {
                        inclusionList.Add(exprNode.InnerXml);
                    }

                    allInclusionExclusions = (allInclusionExclusions.Length > 0)
                        ? allInclusionExclusions.Append(",").Append(string.Join<string>(",", inclusionList))
                        : allInclusionExclusions.Append(string.Join<string>(",", inclusionList));
                }
            }
            return allInclusionExclusions;
        }

        public static Variable ToVariable(this XmlNode varNode, HTSData htsData)
        {
            Variable variable = new Variable();
            variable.Name = varNode.Attributes["name"].Value;

            if (varNode.Attributes["format"] != null)
            {
                string format = varNode.Attributes["format"].Value;
                int decimalCount = format.Count(x => x == '#');
                decimalCount = decimalCount - 1;
                variable.Format = decimalCount.ToString();
            }
            else
            {
                variable.Format = string.Empty;
            }

            variable.Type = varNode.Attributes["type"].Value;

            var constraintNodes = varNode.SelectNodes("constraint");
            if (constraintNodes != null)
            {
                foreach (XmlNode constraintNode in constraintNodes)
                {
                    Constraint c = constraintNode.ToConstraint();
                    variable.Constraints.Add(c);
                }
            }

            return variable;
        }

        /// <summary>
        /// Helper method to create an new xml node representing an HTS step.
        /// </summary>
        /// <param name="xDoc">The xml document to create the node for.</param>
        /// <param name="stepId">The id of the new node.</param>
        /// <param name="stepText">Inner text of the new step node.</param>
        /// <returns></returns>
        public static VariableDictionary GetVariables(this XmlDocument xDoc, HTSData htsData)
        {
            VariableDictionary variables = new VariableDictionary();
            var varNodes = xDoc.DocumentElement.SelectNodes("//vardef");
            if (varNodes == null) return variables;

            foreach (XmlNode varNode in varNodes)
            {
                Variable v = varNode.ToVariable(htsData);

                //remove old definition if present
                if (variables.Keys.Contains(v.Name)) 
                    variables[v.Name] = v;
                else
                    variables.Add(v.Name, v);
            }
            return variables;
        }

        /// <summary>
        /// This method updates the variable information in HTS Data
        /// </summary>
        /// <param name="xDocument">Current document derived from Pending XML</param>
        /// <param name="htsData">HTS Data for which variable need to be updated</param>
        public static void UpdateCurrentVariable(this XmlDocument xDocument, HTSData htsData)
        {
            htsData.VariableLookup = xDocument.GetVariables(htsData);
            if (htsData.VariableLookup != null)
            {
                if (htsData.Variables == null)
                {
                    htsData.Variables = new List<Variable>();
                }
                else
                {
                    htsData.Variables.Clear();
                }

                htsData.Variables.AddRange(htsData.VariableLookup.Values);
            }
        }

        public static void ConvertVariablesToImages(this HtmlDocument doc, HTSData htsData)
        {
            doc.ConvertVariableNodesToImages(htsData);
            doc.ConvertVariableTextToImages(htsData);
        }

        public static void ConvertVariableNodesToImages(this HtmlDocument doc, HTSData htsData)
        {
            var varNodes = doc.DocumentNode.SelectNodes("//iprovar");
            if (varNodes == null) return;

            foreach (HtmlNode varNode in varNodes)
            {
                string varName = varNode.GetAttributeValue("name", "");
                string varIndex = "";

                Match match = Regex.Match(varName, @"\[([0-9]+)\]");
                if (match.Success)
                {
                    varIndex = match.Groups[1].Value;
                    varName = varName.Replace("[" + varIndex + "]", "");
                    varName = varName.Replace("[]", "");
                }

                if (htsData.VariableLookup.ContainsKey(varName))
                {
                    var variable = htsData.VariableLookup[varName];

                    string variableType = variable.Type;
                    string variableImageFile = variableType + "_var_long.gif";

                    //convert variable node (iprovar) to image node
                    HtmlNode imgNode = HtmlNode.CreateNode("<img />");

                    var strSrc = string.Format("~/content/images/{0}", variableImageFile);
                    strSrc = VirtualPathUtility.ToAbsolute(strSrc);
                    imgNode.Attributes.Add("src", strSrc);
                    imgNode.Attributes.Add("alt", strSrc);
                    imgNode.Attributes.Add("hts-data-type", "variable");
                    imgNode.Attributes.Add("hts-data-variable-type", variable.Type);

                    if (!string.IsNullOrEmpty(varIndex))
                    {
                        imgNode.Attributes.Add("hts-data-variable-index", varIndex);
                    }

                    imgNode.Attributes.Add("hts-data-id", varName);

                    varNode.ParentNode.ReplaceChild(imgNode, varNode);
                }
            }
        }

        public static void ConvertVariableTextToImages(this HtmlDocument doc, HTSData htsData)
        {
            var textNodes = doc.DocumentNode.SelectNodes("//text()[normalize-space(.) != '']");

            if (textNodes == null) return;

            VariableDictionary sortedVariableLookup = new VariableDictionary();
            foreach (var entry in htsData.VariableLookup)
            {
                entry.Value.Sequence = entry.Value.Name.Length;
            }

            //sort lookup array variables first
            if (htsData.VariableLookup != null)
            {
                sortedVariableLookup = (from entry in htsData.VariableLookup orderby entry.Value.SortType ascending, entry.Value.Sequence descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            }             

            foreach (HtmlNode textNode in textNodes)
            {
                foreach (var variableEntry in sortedVariableLookup)
                {
                    Variable v = variableEntry.Value;
                    string variableType = v.Type;

                    //convert response(iproshortelement) to image node
                    HtmlNode imgNode = HtmlNode.CreateNode("<img />");

                    // add the image source based on node type
                    string strUrlBase = htsData.FormulaEditorUrl.Replace("geteq.ashx", "geticon.ashx");
                    //strUrlBase = "http://localhost:49676/geticon.ashx";

                    string caption = v.Name;
                    caption = HttpUtility.UrlEncode(caption);

                    string nodeType = v.Type + "_var";

                    var strQuery = string.Format("?caption={0}&type={1} ", caption, nodeType);
                    var strSrc = strUrlBase + strQuery;
                    strSrc = HttpUtility.HtmlEncode(strSrc);
                    
                    imgNode.Attributes.Add("src", strSrc);
                    imgNode.Attributes.Add("alt", strSrc);
                    imgNode.Attributes.Add("hts-data-type", "variable");
                    imgNode.Attributes.Add("hts-data-variable-type", v.Type);
                    imgNode.Attributes.Add("hts-data-id", v.Name);

                    bool bMatch = false;
                    string innerText = textNode.InnerText.Trim();

                    var parentNode = textNode.ParentNode.Name;
                    if (parentNode.Equals("p", StringComparison.InvariantCultureIgnoreCase) ||
                        parentNode.Equals("span", StringComparison.InvariantCultureIgnoreCase) ||
                        parentNode.Equals("strong", StringComparison.InvariantCultureIgnoreCase) ||
                        parentNode.Equals("em", StringComparison.InvariantCultureIgnoreCase))
                    {
                        innerText = textNode.InnerText;
                    }

                    if (!string.IsNullOrEmpty(innerText))
                    {
                        string arrayPattern = string.Format(@"(\${0}\[[^]]*\])|\${1}", v.Name, v.Name);
                                                            
                        // Get a collection of matches.
                        innerText = Regex.Replace(innerText, arrayPattern,
                            delegate(Match m)
                            {
                                //check for nested array variable
                                if ((m.Index > 0) && (innerText[m.Index - 1] == '['))
                                    return m.Value;

                                int iBrktPos = innerText.IndexOf(m.Value);
                                //check charcater previous to square brackets to see if it is array index or within quote to see if it is assigned value
                                if (iBrktPos != 0 && (innerText[iBrktPos - 1] == '[' || innerText[iBrktPos-1] == '"'))
                                   return m.Value;

                                if (m.Value.Contains('['))
                                {
                                    var arrIndex = m.Value.Replace("$" + v.Name + "[", "").Replace("]", "");
                                        
                                    string arrayName = m.Value.Replace("\\", "");
                                    arrayName = arrayName.Substring(1);
                                    //displayName = arrayName.Replace("_", "-");
                                    strSrc = string.Format("{0}?caption={1}&type={2} ", strUrlBase, arrayName, nodeType);

                                    //??
                                    strSrc = HttpUtility.HtmlEncode(strSrc);
                                    imgNode.Attributes.Remove("src");
                                    imgNode.Attributes.Add("src", strSrc);
                                    if (imgNode.Attributes.Contains("hts-data-variable-index"))
                                    {
                                        imgNode.Attributes["hts-data-variable-index"].Value = arrIndex;
                                    }
                                    else
                                    {
                                        imgNode.Attributes.Add("hts-data-variable-index", arrIndex);
                                    }

                                    bMatch = true;
                                }

                                return imgNode.OuterHtml;
                            });

                        //if (v.Type.Contains("array"))
                        //{
                        //    nodeText = doc.ConvertVariableArrayToImages(nodeText, v);
                        //}
                        //else
                        //{
                        //    //nodeText = nodeText.Replace("~" + v.Name.Trim() + "\\", imgNode.OuterHtml);
                        //    nodeText = doc.ConvertVariableArrayToImages(nodeText, v);
                        //}

                        textNode.InnerHtml = innerText;
                    }
                }
            }
        }

        public static void DeleteOrphanVariables(this HtmlDocument doc, HTSData htsData)
        {
            var varNodes = doc.DocumentNode.SelectNodes("//img");
            if (varNodes == null) return;

            foreach (HtmlNode varNode in varNodes)
            {
                string varName = varNode.GetAttributeValue("name", "");
                string varType = varNode.GetAttributeValue("hts-data-type", "");
                string varIndex = "";

                if (varType != "variable") continue;

                Match match = Regex.Match(varName, @"\[([0-9]+)\]");
                if (match.Success)
                {
                    varIndex = match.Groups[1].Value;
                    varName = varName.Replace("[" + varIndex + "]", "");
                    varName = varName.Replace("[]", "");
                }

                if (!htsData.VariableLookup.ContainsKey(varName))
                {
                    varNode.ParentNode.RemoveChild(varNode);
                }
            }
        }

        private static string HtsResponseShortType(string responseType)
        {
            string shortType = "";

            if (!string.IsNullOrEmpty(responseType))
            {
                switch (responseType.ToLowerInvariant())
                {
                    case "text":
                        shortType = "T";
                        break;
                    case "numeric":
                        shortType = "N";
                        break;
                    case "multi":
                        shortType = "MC";
                        break;
                    case "math":
                        shortType = "E";
                        break;
                    case "graph":
                        shortType = "Graph";
                        break;
                }
            }

            return shortType;
        }

        private static string HtsVariableShortType(string variableType)
        {
            string shortType = "";

            if (!string.IsNullOrEmpty(variableType))
            {
                switch (variableType.ToLowerInvariant())
                {
                    case "text":
                        shortType = "Tv";
                        break;
                    case "numeric":
                        shortType = "Nv";
                        break;
                    case "math":
                        shortType = "Ev";
                        break;
                    case "textarray":
                        shortType = "TAv";
                        break;
                    case "numarray":
                        shortType = "NAv";
                        break;
                    case "matharray":
                        shortType = "MAv";
                        break;                    
                }
            }

            return shortType;
        }

        /// <summary>
        /// Convert an iproelement, iproelement_short, or iproelement_mc to an AQE hotspot icon.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="htsData"></param>
        /// <returns></returns>
        public static HtmlNode ToHtsImageNode(this HtmlNode node, HTSData htsData)
        {
            //convert response to image node
            HtmlNode imgNode = HtmlNode.CreateNode("<img />");
            string caption = "new response";

            foreach (HtmlAttribute attr in node.Attributes)
            {
                imgNode.Attributes.Add(attr.Name, attr.Value);

                if (attr.Name == "correct")
                {
                    caption = attr.Value;

                    try
                    {
                        if (node.Attributes["type"] != null 
                            && node.Attributes["type"].Value == "numeric" 
                            && node.Attributes["format"] != null 
                            && node.Attributes["format"].Value != "auto"
                            && (node.Attributes["sigfigs"] == null || node.Attributes["sigfigs"].Value == "")
                            && caption.IndexOf('e') == -1)
                        {
                            /*int num = 0;
                            int.TryParse(node.Attributes["format"].Value, out num);
                            string mantissa = new String('#', num);
                            string format = string.Concat("{0:#0.", mantissa, ";-#0.", mantissa, "}");
                            caption = String.Format(format, double.Parse(caption));*/
                            caption = format_number(caption, int.Parse(node.Attributes["format"].Value));
                        }
                    }
                    catch { }                    
                }
            }

            string nodeType = node.GetAttributeValue("type", "text");
            if (node.Name == "iproelement_mc")
            {
                string elementId = node.GetAttributeValue("elid", "");
                HtmlNodeCollection choiceNodes;
                nodeType = "multi";
                
                if (string.IsNullOrEmpty(elementId))
                {
                    choiceNodes = node.SelectNodes("//ipro_mcchoice");
                }
                else
                {
                    choiceNodes = node.SelectNodes(string.Format("//iproelement_mc[@elid={0}]//ipro_mcchoice", elementId));
                }
                 
                if (choiceNodes != null)
                {
                    caption = "";
                    List<String> choiceTextList = new List<String>();
                    foreach (var ch in choiceNodes)
                    {
                        choiceTextList.Add(ch.InnerText);
                    }
                    caption += String.Join("|", choiceTextList);
                }
            }

            if (node.Name == "iproelement")
            {
                nodeType = "graph";
            }

            // add the image source based on node type
            string strUrlBase = htsData.FormulaEditorUrl.Replace("geteq.ashx", "geticon.ashx");
            //strUrlBase = "http://localhost:49676/geticon.ashx";

            //caption = HttpUtility.HtmlDecode(caption);
            caption = HttpUtility.UrlEncode(caption);
            
            var strQuery = string.Format("?caption={0}&type={1} ", caption, nodeType + "Response");
            var strSrc = strUrlBase + strQuery;
            
            strSrc = HttpUtility.HtmlEncode(strSrc);

            imgNode.Attributes.Add("src", strSrc);
            imgNode.Attributes.Add("alt", strSrc);
            
            imgNode.Attributes.Add("hts-data-type", nodeType);

            string nodeElementId = node.GetAttributeValue("elid", "");
            imgNode.Attributes.Add("hts-data-id", nodeElementId);

            return imgNode;
        }

        private static string format_number(string pnumber, int decimals)
        {
            double number, dec;
            Int32 whole;
            if (double.TryParse(pnumber, out number) == false) return "0";
            var negative = false;
            var sec = pnumber.Split('.');
            Int32.TryParse(sec[0], out whole);

            if (number < 0) {
                negative = true;
                whole = -(whole);
            }
            
            var result = string.Empty;
            if (sec.Length > 1) {
                double.TryParse(sec[1], out dec);
                dec = dec / Math.Pow(10, (sec[1].Length - decimals));
                var decStr = (whole + Math.Round(dec) / Math.Pow(10, decimals)).ToString();
                var dot = decStr.IndexOf('.');
                if (dot == -1 && decimals > 0)
                {
                    decStr += '.';
                    dot = decStr.IndexOf('.');
                }
                while (decStr.Length <= dot + decimals) { decStr += '0'; }
                result = decStr;
            } else {
                var decStr = whole.ToString();
                decStr += '.';
                var dot = decStr.IndexOf('.');
                while (decStr.Length <= dot + decimals) { decStr += '0'; }
                result = decStr;
            }
            if (negative == true) {
                result = '-' + result;
            }
            return result;
        }

        public static Response ToResponse(this HtmlNode node)
        {
            string answerRule = node.GetAttributeValue("answerrule", "");
            string tolerance = node.GetAttributeValue("tolerance", "");
            string format = node.GetAttributeValue("format", "");
            string allowedWords = node.GetAttributeValue("allowedwords", "");
            string type = node.GetAttributeValue("type", "text");
            string elementId = node.GetAttributeValue("elid", "");
            string correct = node.GetAttributeValue("correct", "");
            string points = node.GetAttributeValue("points", "");
            string size = node.GetAttributeValue("size", "");
            string id = node.GetAttributeValue("id", "");
            string version = node.GetAttributeValue("version", "");
            string checkSyntax = node.GetAttributeValue("checksyntax", "");
            string toleranceType = node.GetAttributeValue("tolerancetype", "");
            string sigfigs = node.GetAttributeValue("sigfigs", "");

            Response response = new Response();
            response.Type = type;
            response.ElementId = elementId;
            response.Correct = correct; // type == "math" ? HttpUtility.UrlEncode(HttpUtility.HtmlDecode(correct)) : correct;
            response.AnswerRule = answerRule;
            response.Tolerance = tolerance;
            response.Format = format;
            response.Points = points;
            response.Size = size;
            response.Id = id;
            response.Version = version;
            response.AllowedWords = allowedWords;
            response.CheckSyntax = checkSyntax;
            response.ToleranceType = toleranceType;
            response.SigFigs = sigfigs;

            var answerNodes = node.SelectNodes("//ipro_alt_correct");

            if (!string.IsNullOrEmpty(elementId))            
            {
                answerNodes = node.SelectNodes(string.Format("//iproelement_short[@elid={0}]//ipro_alt_correct", elementId));
            }

            if (answerNodes != null)
            {
                foreach (HtmlNode answerNode in answerNodes)
                {
                    Answer answer = answerNode.ToAnswer();
                    response.Answers.Add(answer);
                }
            }

            return response;
        }

        public static Response ToMultiResponse(this HtmlNode node)
        {
            string elementId = node.GetAttributeValue("elid", "");
            string correct = node.GetAttributeValue("correct", "");
            string scramble = node.GetAttributeValue("scramble", "");
            string points = node.GetAttributeValue("points", "");
            string columns = node.GetAttributeValue("columns", "");
            string text = node.InnerHtml;

            HtmlNodeCollection choiceNodes;
            if (string.IsNullOrEmpty(elementId))
            {
                choiceNodes = node.SelectNodes("//ipro_mcchoice");
            }
            else
            {
                choiceNodes = node.SelectNodes(string.Format("//iproelement_mc[@elid={0}]//ipro_mcchoice",elementId));
            }


            Response response = new Response();
            response.ElementId = elementId;
            response.Correct = correct;
            response.Scramble = scramble;
            response.Points = points;
            response.Text = text;
            response.Columns = columns;

            if (choiceNodes != null)
            {
                foreach (HtmlNode choiceNode in choiceNodes)
                {
                    Choice choice = choiceNode.ToChoice();
                    response.Choices.Add(choice);
                }
            }
            return response;
        }

        public static Choice ToChoice(this HtmlNode node)
        {
            string choiceId = node.GetAttributeValue("choiceid", "");
            string isFixed = node.GetAttributeValue("fixed", "");
            string id = node.GetAttributeValue("id", "");
            string version = node.GetAttributeValue("version", "");
            string text = node.InnerHtml;

            Choice choice = new Choice();
            choice.ChoiceId = choiceId;
            choice.Fixed = isFixed;
            choice.Id = id;
            choice.Version = version;
            choice.Text = text;

            return choice;
        }

        public static Answer ToAnswer(this HtmlNode node)
        {
            string answerRule = node.GetAttributeValue("answerrule", "");
            string sigfigs = node.GetAttributeValue("sigfigs", "");
            string tolerance = node.GetAttributeValue("tolerance", "");
            string points = node.GetAttributeValue("points", "");
            string correct = node.GetAttributeValue("correct", "");
            string toleranceType = node.GetAttributeValue("tolerancetype", "");

            Answer answer = new Answer();
            answer.Correct = correct;
            answer.AnswerRule = answerRule;
            answer.SigFigs = sigfigs;
            answer.Tolerance = tolerance;
            answer.Points = points;
            answer.ToleranceType = toleranceType;
            return answer;
        }


        //public static HtmlDocument ConvertImagesToFormulas(this HtmlDocument doc, HTSData htsData)
        //{
        //    var nodes = doc.DocumentNode.SelectNodes("//img[@hts-data-type='formula']");

        //    if (nodes == null) return doc;

        //    foreach (HtmlNode node in nodes)
        //    {
        //        string strFormulaSrc = node.GetAttributeValue("src", "");
        //        string strDataEq = node.GetAttributeValue("hts-data-equation", "");
        //        if(strFormulaSrc.IndexOf('=')>0) strFormulaSrc = strFormulaSrc.Split('=')[0] + "=" + strDataEq;
        //        string strFormulaUrlBase = htsData.FormulaEditorUrl.Replace("get.ashx", "");
        //        strFormulaSrc = strFormulaSrc.Replace(strFormulaUrlBase, "");
        //        //strFormulaSrc = HttpUtility.UrlDecode(strFormulaSrc);
        //        //strFormulaSrc = HttpUtility.HtmlEncode(strFormulaSrc);
        //        var strFormulaStyle = node.GetAttributeValue("style", "");

        //        HtmlNode formulaNode = HtmlNode.CreateNode("<iproformula />");
        //        formulaNode.Attributes.Add("alt", "");
        //        formulaNode.Attributes.Add("style", strFormulaStyle);
        //        formulaNode.Attributes.Add("type", "text");
        //        formulaNode.Attributes.Add("src", strFormulaSrc);
        //        formulaNode.Attributes.Add("hts-data-equation", strDataEq);

        //        node.ParentNode.ReplaceChild(formulaNode, node);
        //    }

        //    return doc;
        //}

        public static HtmlDocument ConvertImagesToFormulas(this HtmlDocument doc, HTSData htsData)
        {
            var nodes = doc.DocumentNode.SelectNodes("//img[@hts-data-type='formula']");

            if (nodes == null) return doc;

            foreach (HtmlNode node in nodes)
            {
                string strFormulaSrc = node.GetAttributeValue("src", "");
                string strFormulaUrlBase = htsData.FormulaEditorUrl.Replace("geteq.ashx", "");
                strFormulaSrc = strFormulaSrc.Replace(strFormulaUrlBase, "");
                strFormulaSrc = HttpUtility.UrlDecode(strFormulaSrc);
                strFormulaSrc = HttpUtility.HtmlEncode(strFormulaSrc);
                strFormulaSrc = Regex.Replace(strFormulaSrc, "&(?!amp;|#)", "&amp;");
                //strFormulaSrc = HttpUtility.HtmlEncode(strFormulaSrc);
                var strFormulaStyle = node.GetAttributeValue("style", "");
                var strDataEq = node.GetAttributeValue("hts-data-equation", "");

                HtmlNode formulaNode = HtmlNode.CreateNode("<iproformula />");
                formulaNode.Attributes.Add("alt", "");
                formulaNode.Attributes.Add("style", strFormulaStyle);
                formulaNode.Attributes.Add("type", "text");
                formulaNode.Attributes.Add("src", strFormulaSrc);
                formulaNode.Attributes.Add("hts-data-equation", strDataEq);

                node.ParentNode.ReplaceChild(formulaNode, node);
            }

            return doc;
        }

        public static HtmlDocument ConvertImagesToGraphs(this HtmlDocument doc, HTSData htsData)
        {
            var nodes = doc.DocumentNode.SelectNodes("//img[@hts-data-type='graph']");

            if (nodes == null) return doc;

            foreach (HtmlNode node in nodes)
            {
                string strFunction = node.GetAttributeValue("function", "");
                string strHeight = node.GetAttributeValue("height", "");
                string strWidth = node.GetAttributeValue("width", "");
                string strDatafile = node.GetAttributeValue("datafile", "");
                
                HtmlNode graphNode = HtmlNode.CreateNode("<iproelement />");
                graphNode.Attributes.Add("class", "graph");
                graphNode.Attributes.Add("function", strFunction);
                graphNode.Attributes.Add("height", strHeight);
                graphNode.Attributes.Add("width", strWidth);
                graphNode.Attributes.Add("datafile", strDatafile);

                node.ParentNode.ReplaceChild(graphNode, node);
            }

            return doc;
        }

        public static HtmlDocument ConvertImagesToVariables(this HtmlDocument doc, HTSData htsData)
        {
            var nodes = doc.DocumentNode.SelectNodes("//img[@hts-data-type='variable']");
            if (nodes == null) return doc;

            VariableDictionary sortedVariableLookup = new VariableDictionary();
            foreach (var entry in htsData.VariableLookup)
            {
                entry.Value.Sequence = entry.Value.Name.Length;
            }

            if (htsData.VariableLookup != null)
            {
                sortedVariableLookup = (from entry in htsData.VariableLookup orderby entry.Value.SortType ascending, entry.Value.Sequence descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            } 
            
            foreach (var entry in sortedVariableLookup)
            {
                foreach (HtmlNode node in nodes)
                {
                    var varName = node.GetAttributeValue("hts-data-id", "");

                    if (entry.Value.Name == varName)
                    {
                        var variable = sortedVariableLookup[varName];

                        if (variable.Type.Contains("array"))
                        {
                            var varIndex = node.GetAttributeValue("hts-data-variable-index", "");
                            if (varIndex.IndexOf("$") != -1)
                            {
                                varIndex = varIndex.Replace("$", "~") + "\\";
                            }

                            varName = string.Format("{0}[{1}]", varName, varIndex);
                        }

                        HtmlNode varNode = doc.CreateTextNode();
                        varNode.InnerHtml = "~" + varName + "\\ ";
                        node.ParentNode.ReplaceChild(varNode, node);
                    }
                }
            }

            return doc;
        }

        /// <summary>
        /// Replaces all shorthand($) variable references to player tilda\backslash format.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="htsData"></param>
        /// <returns></returns>
        public static XmlDocument ConvertVariableReferences(this XmlDocument doc, HTSData htsData)
        {
            string innerText = doc.InnerXml;// HTSHelper.ReplaceHTMLSymbols(doc.InnerXml);
            string arrayName = "";
            VariableDictionary sortedVariableLookup = new VariableDictionary();
            foreach (var entry in htsData.VariableLookup)
            {
                entry.Value.Sequence = entry.Value.Name.Length;
            }

            if (htsData.VariableLookup != null)
            {
                sortedVariableLookup = (from entry in htsData.VariableLookup orderby entry.Value.SortType ascending, entry.Value.Sequence descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            } 

            foreach (var variableEntry in sortedVariableLookup)
            {
                bool bMatch = false;

                Variable v = variableEntry.Value;
                string arrayPattern = string.Format(@"(\${0}\[[^]]*\])|\${1}", v.Name, v.Name);

                innerText = Regex.Replace(innerText, arrayPattern,
                delegate(Match m)
                {
                    if (m.Value.Contains('['))
                    {
                        arrayName = m.Value.Replace("$"+v.Name, "~" + v.Name);
                        arrayName = arrayName + "\\";
                        bMatch = true;
                    }
                    else
                        arrayName = m.Value;

                    return arrayName;
                });

                if (bMatch == false)
                {
                    string sName = v.Name.Replace("_", "-");
                    string shortFormat = "$" + sName;
                    string tildaFormat = "~" + v.Name + "\\";
                    innerText =innerText.Replace(shortFormat, tildaFormat);

                    //replace variable reference enclosed within brackets ("(" and ")")
                    shortFormat = "$(" + sName + ")";
                    tildaFormat = "~" + v.Name + "\\";
                    innerText = innerText.Replace(shortFormat, tildaFormat);
                }
            }
            doc.PreserveWhitespace = true;
            doc.InnerXml = innerText;

            return doc;
        }

        /// <summary>
        /// Revert all tilda\backslash player reference to shorthand($) variable reference.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="htsData"></param>
        /// <returns></returns>
        public static XmlDocument RevertVariableReferences(this XmlDocument doc, VariableDictionary htsVariables)
        {
            string innerText = doc.InnerXml;
            string arrayName = "";
            
            VariableDictionary sortedVariableLookup = new VariableDictionary();

            if (htsVariables != null)
            {
                foreach (var entry in htsVariables)
                {
                    entry.Value.Sequence = entry.Value.Name.Length;
                }

                sortedVariableLookup = (from entry in htsVariables orderby entry.Value.SortType ascending, entry.Value.Sequence descending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            foreach (var variableEntry in sortedVariableLookup)
            {
                bool bMatch = false;

                Variable v = variableEntry.Value;
                string arrayPattern = string.Format(@"(~{0}\[[^]]*\]\\)|(~{1}\\)", v.Name, v.Name);

                innerText = Regex.Replace(innerText, arrayPattern,
                delegate(Match m)
                {
                    ////check for nested array variable
                    //if ((m.Index > 0) && (innerText[m.Index - 1] == '['))
                    //    return m.Value;

                    if (m.Value.Contains('['))
                    {
                        arrayName = m.Value.Replace("~", "$").Replace("\\", "");
                        bMatch = true;
                    }
                    else
                    {
                        arrayName = m.Value;
                        bMatch = false;
                    }
                        

                    return arrayName;
                });

                if (bMatch == false)
                {
                    string shortFormat = "$" + v.Name;
                    string tildaFormat = "~" + v.Name + "\\";
                    innerText = innerText.Replace(tildaFormat, shortFormat);
                }
            }

            doc.InnerXml = innerText;

            return doc;
        }

        public static HtmlNode ToShortNode(this Response response)
        {
            HtmlNode shortNode = null;
            if (response != null)
            {
                shortNode = HtmlNode.CreateNode("<iproelement_short />");
                shortNode.Attributes.Add("elid", response.ElementId);
                shortNode.Attributes.Add("id", "@step.s@id");
                shortNode.Attributes.Add("version", "2");
                shortNode.Attributes.Add("correct", HttpUtility.HtmlEncode(response.Correct));
                shortNode.Attributes.Add("type", response.Type);


                if (response.Type == "numeric")
                {
                    if (!string.IsNullOrEmpty(response.Tolerance))
                    {
                        shortNode.Attributes.Add("tolerance", response.Tolerance);
                    }

                    if (!string.IsNullOrEmpty(response.Format))
                    {
                        shortNode.Attributes.Add("format", response.Format);
                    }

                    if (!string.IsNullOrEmpty(response.AllowedWords))
                    {
                        shortNode.Attributes.Add("allowedwords", HttpUtility.HtmlEncode(response.AllowedWords));
                    }

                    if (!string.IsNullOrEmpty(response.CheckSyntax))
                    {
                        shortNode.Attributes.Add("checksyntax", response.CheckSyntax);
                    }
                    if (!string.IsNullOrEmpty(response.ToleranceType))
                    {
                        shortNode.Attributes.Add("tolerancetype", response.ToleranceType);
                    }
                    if (!string.IsNullOrEmpty(response.SigFigs))
                    {
                        shortNode.Attributes.Add("sigfigs", response.SigFigs);
                    }
                }

                if (response.Type == "math")
                {
                    if (!string.IsNullOrEmpty(response.AnswerRule))
                    {
                        shortNode.Attributes.Add("answerrule", response.AnswerRule);
                    }
                }

                if (!string.IsNullOrEmpty(response.Points))
                {
                    shortNode.Attributes.Add("points", response.Points);
                }

                if (!string.IsNullOrEmpty(response.Size))
                {
                    shortNode.Attributes.Add("size", response.Size);
                }
                else
                {
                    shortNode.Attributes.Add("size", "2");
                }

                if (!string.IsNullOrEmpty(response.Style))
                {
                    shortNode.Attributes.Add("style", response.Style);
                }

                foreach (Answer answer in response.Answers)
                {
                    HtmlNode answerNode = HtmlNode.CreateNode("<ipro_alt_correct />");
                    answerNode.Attributes.Add("correct", HttpUtility.HtmlEncode(answer.Correct));
                    answerNode.Attributes.Add("answerrule", answer.AnswerRule);
                    answerNode.Attributes.Add("sigfigs", answer.SigFigs);
                    answerNode.Attributes.Add("tolerance", answer.Tolerance);
                    answerNode.Attributes.Add("points", answer.Points);
                    answerNode.Attributes.Add("tolerancetype", answer.ToleranceType);
                    shortNode.AppendChild(answerNode);
                }

                //shortNode.Attributes.Add("correct", "~m\\");
                //shortNode.Attributes.Add("tolerance", "0.1");
                //shortNode.Attributes.Add("type", "numeric");
                //shortNode.Attributes.Add("format", "");
                //shortNode.Attributes.Add("answerrule", "");
                //shortNode.Attributes.Add("points", "");
                //shortNode.Attributes.Add("size", "2");
                //shortNode.Attributes.Add("checksyntax", "on");
                //shortNode.Attributes.Add("allowedwords", "");
                //shortNode.Attributes.Add("elid", "1");
            }
            return shortNode;
        }

        public static HtmlDocument ConvertImagesToShort(this HtmlDocument doc, HTSData htsData)
        {
            var nodes = doc.DocumentNode.SelectNodes("//img[@hts-data-type='text'] | //img[@hts-data-type='numeric'] | //img[@hts-data-type='math']");

            if (nodes == null) return doc;

            foreach (HtmlNode node in nodes)
            {
                var responseId = node.GetAttributeValue("hts-data-id", "");
                var response = htsData.Responses.Find(r => r.ElementId == responseId);
                var shortNode = response.ToShortNode();
                if (shortNode != null)
                {
                    node.ParentNode.ReplaceChild(shortNode, node);
                }
            }

            return doc;
        }

        public static HtmlNode ToMultipleChoiceNode(this Response response, HtmlNode parentNodeDiv)
        {
            var columnsPerRow = 1;
            HtmlNode nodeDiv = null, node = null;

            int.TryParse(response.Columns, out columnsPerRow);            
            columnsPerRow = columnsPerRow == 0 ? 1 : columnsPerRow;

            if (response != null)
            {
                node = HtmlNode.CreateNode("<iproelement_mc />");
                node.Attributes.Add("elid", response.ElementId);

                if (!string.IsNullOrEmpty(response.Scramble))
                {
                    node.Attributes.Add("scramble", response.Scramble);
                    node.Attributes.Add("points", string.IsNullOrEmpty(response.Points) ? "0" : response.Points);
                }

                node.Attributes.Add("columns", columnsPerRow.ToString());
                node.Attributes.Add("correct", response.Correct);

                foreach (Choice choice in response.Choices)
                {
                    HtmlNode choiceNode = HtmlNode.CreateNode("<ipro_mcchoice />");
                    choiceNode.Attributes.Add("id", "@step.mc@id");
                    choiceNode.Attributes.Add("version", "2");
                    choiceNode.Attributes.Add("fixed", choice.Fixed);
                    choiceNode.Attributes.Add("choiceid", choice.ChoiceId);
                    choiceNode.Attributes.Add("mcid", response.ElementId);
                    choiceNode.InnerHtml = choice.Text ?? "";
                    node.AppendChild(choiceNode);
                }
             }

            if (parentNodeDiv == null || !(parentNodeDiv.Name.Equals("div", StringComparison.InvariantCultureIgnoreCase)))
            {
                nodeDiv = HtmlNode.CreateNode("<div>");
                nodeDiv.AppendChild(node);
                return nodeDiv;
            }

            return node;
        }

        public static HtmlDocument ConvertImagesToMC(this HtmlDocument doc, HTSData htsData)
        {
            var nodes = doc.DocumentNode.SelectNodes("//img[@hts-data-type='multi']");

            if (nodes == null) return doc;

            foreach (HtmlNode node in nodes)
            {
                var responseId = node.GetAttributeValue("hts-data-id", "");
                var response = htsData.Responses.Find(r => r.ElementId == responseId);

                var xmlNode = response.ToMultipleChoiceNode(node.ParentNode);
                if (xmlNode != null)
                {
                    node.ParentNode.ReplaceChild(xmlNode, node);
                }
            }

            return doc;
        }

        public static HtmlDocument AddHtsHintButton(this HtmlDocument doc, string hintStepId)
        {
            var nextNode = doc.DocumentNode.SelectSingleNode("//input[@id='@step.hint']");

            if (nextNode == null)
            {
                HtmlNode newInputNode = HtmlNode.CreateNode("<input />");
                newInputNode.Attributes.Add("class", "hint");
                newInputNode.Attributes.Add("id", "@step.hint");
                newInputNode.Attributes.Add("onclick", "javascript:next('hint', '@step')");
                newInputNode.Attributes.Add("type", "button");
                newInputNode.Attributes.Add("value", "Hint");
                newInputNode.Attributes.Add("hide", "yes");

                var existingInputNode = doc.DocumentNode.SelectSingleNode("//input");
                if (existingInputNode != null)
                {
                    var parentNode = existingInputNode.ParentNode;
                    parentNode.AppendChild(newInputNode);
                }
                else
                {
                    HtmlNode newDivNode = HtmlNode.CreateNode("<div />");
                    newDivNode.AppendChild(newInputNode);
                    doc.DocumentNode.AppendChild(newDivNode);
                }

                doc.AddHtsNavNode("hint", hintStepId);
            }

            return doc;
        }

        public static HtmlDocument AddHtsNextButton(this HtmlDocument doc, bool isSingleStep, string nextStepId)
        {
            var nextNode = doc.DocumentNode.SelectSingleNode("//input[@id='@step.next']");

            if (nextNode == null)
            {
                HtmlNode newInputNode = HtmlNode.CreateNode("<input />");
                newInputNode.Attributes.Add("class", "next");
                newInputNode.Attributes.Add("id", "@step.next");
                newInputNode.Attributes.Add("onclick", "javascript:next('next', '@step')");
                newInputNode.Attributes.Add("type", "button");
                newInputNode.Attributes.Add("hide", "no");

                if (isSingleStep)
                {
                    newInputNode.Attributes.Add("value", "Check");
                }
                else
                {
                    newInputNode.Attributes.Add("value", "Continue");
                }

                var existingInputNode = doc.DocumentNode.SelectSingleNode("//input");
                if (existingInputNode != null)
                {
                    var parentNode = existingInputNode.ParentNode;
                    parentNode.AppendChild(newInputNode);
                }
                else
                {
                    HtmlNode newDivNode = HtmlNode.CreateNode("<div />");
                    newDivNode.AppendChild(newInputNode);
                    doc.DocumentNode.AppendChild(newDivNode);
                }

               if(!isSingleStep) doc.AddHtsNavNode("next", nextStepId);
            }

            return doc;
        }

        public static HtmlDocument AddHtsNavNode(this HtmlDocument doc, string navType, string navStepId)
        {
            var navNode = doc.DocumentNode.SelectSingleNode("//ipronav[@navtype='" + navType + "']");

            if (navNode == null)
            {
                HtmlNode newNavNode = HtmlNode.CreateNode("<ipronav />");
                newNavNode.Attributes.Add("navtype", navType);
                newNavNode.Attributes.Add("next", navStepId);

                var existingNode = doc.DocumentNode.SelectSingleNode("//ipronav");
                if (existingNode != null)
                {
                    var parentNode = existingNode.ParentNode;
                    parentNode.AppendChild(newNavNode);
                }
                else
                {
                    HtmlNode newDivNode = HtmlNode.CreateNode("<div />");
                    newDivNode.AppendChild(newNavNode);
                    doc.DocumentNode.AppendChild(newDivNode);
                }
            }

            return doc;
        }

        /// <summary>
        /// Removes all ipronav elements from HTMLDocument object.
        /// </summary>
        /// <param name="doc">Document to remove from.</param>
        /// <returns></returns>
        public static HtmlDocument RemoveNavElements(this HtmlDocument doc)
        {
            var navNodes = doc.DocumentNode.SelectNodes("//ipronav");
            HtmlNode parentNode = null;
            if (navNodes != null)
            {
                foreach (HtmlNode node in navNodes)
                {
                    if (parentNode != node.ParentNode)
                    {
                        if (parentNode != null && parentNode.InnerHtml.Trim() != null && parentNode.InnerHtml.Equals("DIV", StringComparison.OrdinalIgnoreCase)) parentNode.Remove();
                        parentNode = node.ParentNode;
                    }

                    if (node != null && parentNode != null) parentNode.RemoveChild(node);
                }

                if (parentNode.InnerHtml.Trim() == "") parentNode.Remove();
            }
    
            return doc;
        }

        /// <summary>
        /// Removes all button elements from HTMLDocument object.
        /// </summary>
        /// <param name="doc">Document to remove from.</param>
        /// <returns></returns>
        public static HtmlDocument RemoveButtonElements(this HtmlDocument doc)
        {
            //var buttonNodes = doc.DocumentNode.SelectNodes("//input[@type='button']");
            //if (buttonNodes == null) return doc;
            //foreach (HtmlNode buttonNode in buttonNodes) buttonNode.Remove();

            var hintNodes = doc.DocumentNode.SelectNodes("//input[@class='hint']");
            HtmlNode parentNode = null;
            if (hintNodes != null)
            {
                foreach (HtmlNode node in hintNodes)
                {
                    if (parentNode != node.ParentNode)
                    {
                        parentNode = node.ParentNode;
                        if (parentNode != null && parentNode.InnerHtml.Trim() != null && parentNode.Name.Equals("DIV", StringComparison.OrdinalIgnoreCase)) parentNode.Remove();
                    }

                    //if (node != null) node.ParentNode.RemoveChild(node);
                    if (node != null && parentNode != null) parentNode.RemoveChild(node);
                }

                if (parentNode.InnerHtml.Trim() == "") parentNode.Remove();
            }
            var nextNodes = doc.DocumentNode.SelectNodes("//input[@class='next']");
            parentNode = null;
            if (nextNodes != null)
            {
                foreach (HtmlNode node in nextNodes)
                {
                    if (parentNode != node.ParentNode)
                    {
                        parentNode = node.ParentNode;
                        if (parentNode != null && parentNode.InnerHtml.Trim() != null && parentNode.Name.Equals("DIV", StringComparison.OrdinalIgnoreCase)) parentNode.Remove();
                    }

                    if (node != null && parentNode != null) parentNode.RemoveChild(node);
                }

                if (parentNode != null && parentNode.InnerHtml.Trim() == "") parentNode.Remove();
            }
            return doc;
        }

        public static string ToValidHtml(this XmlNode stepNode, HTSData htsData)
        {
            var doc = stepNode.ToHtmlDocument();
         
            doc.ConvertFormulasToImages(htsData);
            doc.ConvertGraphsToImages(htsData);

            //convert the multiple choice responses and add to collection
            var multiResponses = doc.ConvertMCToImages(htsData);
            htsData.Responses.AddRange(multiResponses);

            //convert the short responses and add to collection
            var shortResponses = doc.ConvertShortToImages(htsData);
            htsData.Responses.AddRange(shortResponses);

            doc.DeleteOrphanVariables(htsData);            
            doc.ConvertVariablesToImages(htsData);

            return doc.ToHtmlString();
        }

        public static HtmlDocument ToValidXml(this XmlNode stepNode, HTSData htsData)
        {
            var doc = stepNode.ToHtmlDocument();

            doc.ConvertImagesToFormulas(htsData);
            doc.ConvertImagesToGraphs(htsData);
            doc.ConvertImagesToMC(htsData);
            doc.ConvertImagesToShort(htsData);
            doc.ConvertImagesToVariables(htsData);            
            return doc;
        }

        public static string ToHtmlString(this HtmlDocument doc)
        {
            string result = "";
            doc.OptionWriteEmptyNodes = true;
            using (var sw = new StringWriter())
            {
                doc.Save(sw);
                result = sw.ToString();
            }            

            result = Regex.Replace(result, "<input(.*?)>", "<input $1/>");
            result = Regex.Replace(result, "<input(.*?)//>", "<input $1/>");
            result = Regex.Replace(result, "<img(.*?)>", "<img $1/>");
            result = Regex.Replace(result, "<img(.*?)//>", "<img $1/>");
            result = Regex.Replace(result, "<hr(.*?)>", "<hr $1/>");
            result = Regex.Replace(result, "<hr(.*?)//>", "<hr $1/>");
            result = Regex.Replace(result, "<br(.*?)>", "<br $1/>");
            result = Regex.Replace(result, "<br(.*?)//>", "<br $1/>");
            
            result = result.Replace("&nbsp;", " ");
            return result;
        }

        /// <summary>
        /// This is extension to default InnerText() method of XmlNode
        /// </summary>
        /// <param name="current">Take the current xml node to give inner text and formula text result</param>
        /// <returns>Returns inner string and formula tet</returns>
        public static string InnerTextWithFormula(this XmlNode current)
        {
            XmlNode firstChild = current.FirstChild;
            if (firstChild == null)
            {
                return string.Empty;
            }
            if (firstChild.NextSibling == null)
            {
                switch (firstChild.NodeType)
                {
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        return firstChild.Value;
                }
            }
            StringBuilder builder = new StringBuilder();
            current.AppendChildText(builder);
            return builder.ToString();
        }

        private static void AppendChildText(this XmlNode current, StringBuilder builder)
        {
            for (XmlNode node = current.FirstChild; node != null; node = node.NextSibling)
            {
                if (node.FirstChild == null)
                {
                    if (((node.NodeType == XmlNodeType.Text) || (node.NodeType == XmlNodeType.CDATA)) || ((node.NodeType == XmlNodeType.Whitespace) || (node.NodeType == XmlNodeType.SignificantWhitespace)))
                    {
                        builder.Append(node.InnerText);
                    }
                    else if (node.NodeType == XmlNodeType.Element && node.Attributes["hts-data-equation"] != null && !string.IsNullOrEmpty(node.Attributes["hts-data-equation"].Value))
                    {
                        builder.Append(HttpUtility.UrlDecode(node.Attributes["hts-data-equation"].Value));
                    }
                }
                else
                {
                    node.AppendChildText(builder);
                }
            }
        }

        /// <summary>
        /// This method is used to replace symbol characters so that we could avoid "&" related error. Check the list of symbols from
        /// http://www.w3schools.com/tags/ref_entities.asp OR  http://www.w3.org/TR/html4/sgml/entities.html
        /// </summary>
        /// <param name="htmlString">String value to check and replace</param>
        /// <returns>Replaced string value</returns>
        public static string ReplaceHTMLSymbols(string htmlString)
        {
            
            StringBuilder tempHtml = new StringBuilder(htmlString);
            tempHtml = tempHtml.Replace("&nbsp;", "&#160;")/*.Replace("&emptyop;", "&#160;")*/.Replace("&iexcl;", "&#161;").Replace("&cent;", "&#162;") /* .Replace("&emptyop;", "&#160;") It is from Formula Editor */
                .Replace("&pound;", "&#163;").Replace("&curren;", "&#164;").Replace("&yen;", "&#165;").Replace("&brvbar;", "&#166;")
                .Replace("&sect;", "&#167;").Replace("&uml;", "&#168;").Replace("&copy;", "&#169;").Replace("&ordf;", "&#170;")
                .Replace("&laquo;", "&#171;").Replace("&not;", "&#172;").Replace("&shy;", "&#173;").Replace("&reg;", "&#174;")
                .Replace("&macr;", "&#175;").Replace("&deg;", "&#176;").Replace("&plusmn;", "&#177;")/*.Replace("&plusminus;", "&#177;")*/.Replace("&sup2;", "&#178;") /* Replace("&plusminus;", "&#177;") It is from Formula Editor */
                .Replace("&sup3;", "&#179;").Replace("&acute;", "&#180;").Replace("&micro;", "&#181;").Replace("&para;", "&#182;")
                .Replace("&middot;", "&#183;").Replace("&cedil;", "&#184;").Replace("&sup1;", "&#185;").Replace("&ordm;", "&#186;")
                .Replace("&raquo;", "&#187;").Replace("&frac14;", "&#188;").Replace("&frac12;", "&#189;").Replace("&frac34;", "&#190;")
                .Replace("&iquest;", "&#191;").Replace("&times;", "&#215;").Replace("&divide;", "&#247;").Replace("&Agrave;", "&#192;")
                .Replace("&Aacute;", "&#193;").Replace("&Acirc;", "&#194;").Replace("&Atilde;", "&#195;").Replace("&Auml;", "&#196;")
                .Replace("&Aring;", "&#197;").Replace("&AElig;", "&#198;").Replace("&Ccedil;", "&#199;").Replace("&Egrave;", "&#200;")
                .Replace("&Eacute;", "&#201;").Replace("&Ecirc;", "&#202;").Replace("&Euml;", "&#203;").Replace("&Igrave;", "&#204;")
                .Replace("&Iacute;", "&#205;").Replace("&Icirc;", "&#206;").Replace("&Iuml;", "&#207;").Replace("&ETH;", "&#208;")
                .Replace("&Ntilde;", "&#209;").Replace("&Ograve;", "&#210;").Replace("&Oacute;", "&#211;").Replace("&Ocirc;", "&#212;")
                .Replace("&Otilde;", "&#213;").Replace("&Ouml;", "&#214;").Replace("&Oslash;", "&#216;").Replace("&Ugrave;", "&#217;")
                .Replace("&Uacute;", "&#218;").Replace("&Ucirc;", "&#219;").Replace("&Uuml;", "&#220;").Replace("&Yacute;", "&#221;")
                .Replace("&THORN;", "&#222;").Replace("&szlig;", "&#223;").Replace("&agrave;", "&#224;").Replace("&aacute;", "&#225;")
                .Replace("&acirc;", "&#226;").Replace("&atilde;", "&#227;").Replace("&auml;", "&#228;").Replace("&aring;", "&#229;")
                .Replace("&aelig;", "&#230;").Replace("&ccedil;", "&#231;").Replace("&egrave;", "&#232;").Replace("&eacute;", "&#233;")
                .Replace("&ecirc;", "&#234;").Replace("&euml;", "&#235;").Replace("&igrave;", "&#236;").Replace("&iacute;", "&#237;")
                .Replace("&icirc;", "&#238;").Replace("&iuml;", "&#239;").Replace("&eth;", "&#240;").Replace("&ntilde;", "&#241;")
                .Replace("&ograve;", "&#242;").Replace("&oacute;", "&#243;").Replace("&ocirc;", "&#244;").Replace("&otilde;", "&#245;")
                .Replace("&ouml;", "&#246;").Replace("&oslash;", "&#248;").Replace("&ugrave;", "&#249;").Replace("&uacute;", "&#250;")
                .Replace("&ucirc;", "&#251;").Replace("&uuml;", "&#252;").Replace("&yacute;", "&#253;").Replace("&thorn;", "&#254;")
                .Replace("&yuml;", "&#255;"); /*Replace("&quot;", "&#34;").Replace("&apos;", "&#39;").Replace("&amp;", "&#38;").Replace("&lt;", "&#60;").Replace("&gt;", "&#62;").*/


            tempHtml = tempHtml.Replace("&Alpha;", "&#913;").Replace("&Beta;", "&#914;").Replace("&Gamma;", "&#915;").Replace("&Delta;", "&#916;")
                .Replace("&Epsilon;", "&#917;")/*.Replace("&Epsi;", "&#917;")*/.Replace("&Zeta;", "&#918;").Replace("&Eta;", "&#919;").Replace("&Theta;", "&#920;") /*.Replace("&Epsi;", "&#917;")  it is from Formula Editor*/
                .Replace("&Iota;", "&#921;").Replace("&Kappa;", "&#922;").Replace("&Lambda;", "&#923;").Replace("&Mu;", "&#924;").Replace("&Nu;", "&#925;")
                .Replace("&Xi;", "&#926;").Replace("&Omicron;", "&#927;").Replace("&Pi;", "&#928;").Replace("&Rho;", "&#929;").Replace("&Sigma;", "&#931;")
                .Replace("&Tau;", "&#932;").Replace("&Upsilon;", "&#933;")/*.Replace("&Upsi;", "&#933;")*/.Replace("&Phi;", "&#934;").Replace("&Chi;", "&#935;").Replace("&Psi;", "&#936;") /*.Replace("&Upsi;", "&#933;")  it is from Formula Editor*/
                .Replace("&Omega;", "&#937;").Replace("&alpha;", "&#945;").Replace("&beta;", "&#946;")/*.Replace("&gamma;", "&#947;").Replace("&delta;", "&#948;")*/
                .Replace("&epsilon;", "&#949;")/*.Replace("&epsi;", "&#949;")*/.Replace("&zeta;", "&#950;").Replace("&eta;", "&#951;").Replace("&theta;", "&#952;").Replace("&iota;", "&#953;")  /*.Replace("&epsi;", "&#949;")  it is from Formula Editor*/
                .Replace("&kappa;", "&#954;").Replace("&lambda;", "&#955;").Replace("&mu;", "&#956;").Replace("&nu;", "&#957;").Replace("&xi;", "&#958;")
                .Replace("&omicron;", "&#959;").Replace("&pi;", "&#960;").Replace("&rho;", "&#961;").Replace("&sigmaf;", "&#962;").Replace("&sigma;", "&#963;")
                .Replace("&tau;", "&#964;").Replace("&upsilon;", "&#965;")/*.Replace("&upsi;", "&#965;")*/.Replace("&phi;", "&#966;").Replace("&chi;", "&#967;").Replace("&psi;", "&#968;") /*.Replace("&upsi;", "&#965;")  it is from Formula Editor*/
                .Replace("&omega;", "&#969;").Replace("&thetasy;", "&#977;").Replace("&upsih;", "&#978;").Replace("&piv;", "&#982;");

            tempHtml = tempHtml.Replace("&bull;", "&#8226;").Replace("&hellip;", "&#8230;").Replace("&prime;", "&#8242;").Replace("&Prime;", "&#8243;")
                .Replace("&oline;", "&#8254;").Replace("&frasl;", "&#8260;").Replace("&weierp;", "&#8472;").Replace("&image;", "&#8465;").Replace("&real;", "&#8476;")
                .Replace("&trade;", "&#8482;").Replace("&alefsym;", "&#8501;").Replace("&larr;", "&#8592;").Replace("&uarr;", "&#8593;").Replace("&rarr;", "&#8594;")
                .Replace("&darr;", "&#8595;").Replace("&harr;", "&#8596;").Replace("&crarr;", "&#8629;").Replace("&lArr;", "&#8656;").Replace("&uArr;", "&#8657;")
                .Replace("&rArr;", "&#8658;").Replace("&dArr;", "&#8659;").Replace("&hArr;", "&#8660;").Replace("&forall;", "&#8704;").Replace("&part;", "&#8706;");

            tempHtml = tempHtml.Replace("&exist;", "&#8707;").Replace("&empty;", "&#8709;").Replace("&nabla;", "&#8711;").Replace("&isin;", "&#8712;")
                .Replace("&notin;", "&#8713;").Replace("&ni;", "&#8715;").Replace("&prod;", "&#8719;").Replace("&sum;", "&#8721;").Replace("&minus;", "&#8722;")
                .Replace("&lowast;", "&#8727;").Replace("&radic;", "&#8730;").Replace("&prop;", "&#8733;").Replace("&infin;", "&#8734;").Replace("&ang;", "&#8736;")
                .Replace("&and;", "&#8743;").Replace("&or;", "&#8744;").Replace("&cap;", "&#8745;").Replace("&cup;", "&#8746;").Replace("&int;", "&#8747;")
                .Replace("&there4;", "&#8756;").Replace("&sim;", "&#8764;").Replace("&cong;", "&#8773;").Replace("&asymp;", "&#8776;").Replace("&ne;", "&#8800;")
                .Replace("&equiv;", "&#8801;").Replace("&le;", "&#8804;").Replace("&ge;", "&#8805;").Replace("&sub;", "&#8834;").Replace("&sup;", "&#8835;")
                .Replace("&nsub;", "&#8836;").Replace("&sube;", "&#8838;").Replace("&supe;", "&#8839;").Replace("&oplus;", "&#8853;").Replace("&otimes;", "&#8855;")
                .Replace("&perp;", "&#8869;").Replace("&sdot;", "&#8901;").Replace("&lceil;", "&#8968;").Replace("&rceil;", "&#8969;").Replace("&lfloor;", "&#8970;")
                .Replace("&rfloor;", "&#8971;").Replace("&lang;", "&#9001;").Replace("&rang;", "&#9002;").Replace("&loz;", "&#9674;").Replace("&spades;", "&#9824;")
                .Replace("&clubs;", "&#9827;").Replace("&hearts;", "&#9829;").Replace("&diams;", "&#9830;");

            tempHtml = tempHtml.Replace("&OElig;","&#338;").Replace("&oelig;","&#339;").Replace("&Scaron;","&#352;").Replace("&scaron;","&#353;").Replace("&Yuml;","&#376;")
                .Replace("&circ;","&#710;").Replace("&tilde;","&#732;").Replace("&ensp;","&#8194;").Replace("&emsp;","&#8195;").Replace("&thinsp;","&#8201;")
                .Replace("&zwnj;","&#8204;").Replace("&zwj;","&#8205;").Replace("&lrm;","&#8206;").Replace("&rlm;","&#8207;").Replace("&ndash;","&#8211;")
                .Replace("&mdash;","&#8212;").Replace("&lsquo;","&#8216;").Replace("&rsquo;","&#8217;").Replace("&sbquo;","&#8218;").Replace("&ldquo;","&#8220;")
                .Replace("&rdquo;","&#8221;").Replace("&bdquo;","&#8222;").Replace("&dagger;","&#8224;").Replace("&Dagger;","&#8225;").Replace("&permil;","&#8240;")
                .Replace("&lsaquo;","&#8249;").Replace("&rsaquo;","&#8250;").Replace("&euro;","&#8364;").Replace("&fnof;","&#402;");


            /* Even after all these replacements "&" in <a href="http://ww......&..." > and <img src="... & .." >    doesn't get replaced so replace it at the end. */
            return Regex.Replace(tempHtml.ToString(), "&(?![#|amp;])", "&#38;");
        }

        #endregion
    }
}