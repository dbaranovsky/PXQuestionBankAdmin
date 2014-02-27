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

namespace Bfw.PX.QuestionEditor.Biz.DataContracts
{
    using VariableDictionary = Dictionary<string, Variable>;
    using System.Text.RegularExpressions;

    [DataContract]
    public class HTSData
    {

        #region "Entity Name Variable Block"
        /* Reference from svn://svn.bfwpub.com/HTS/trunk/px/server/HTS/CHTSProblem.cs  ## unsSymbols
         And characters rendered from FormulaEditor.swf file
         Including from http://www.elizabethcastro.com/html/extras/entities.html
         */
        private string iProblemDocumentType = string.Concat("<!DOCTYPE iproblem [<!ENTITY times 'times'><!ENTITY div 'div'>",
            "<!ENTITY Alpha 'Alpha'><!ENTITY alpha 'alpha'><!ENTITY Beta 'Beta'><!ENTITY beta 'beta'><!ENTITY Chi 'Chi'><!ENTITY chi 'chi'><!ENTITY Delta 'Delta'><!ENTITY delta 'delta'>",
            "<!ENTITY Epsilon 'Epsilon'><!ENTITY epsilon 'epsilon'><!ENTITY epsi 'epsi'><!ENTITY Eta 'Eta'><!ENTITY eta 'eta'><!ENTITY Gamma 'Gamma'><!ENTITY gamma 'gamma'><!ENTITY Iota 'Iota'><!ENTITY iota 'iota'>",
            "<!ENTITY Kappa 'Kappa'><!ENTITY kappa 'kappa'><!ENTITY Lambda 'Lambda'><!ENTITY lambda 'lambda'><!ENTITY Mu 'Mu'><!ENTITY mu 'mu'><!ENTITY Nu 'Nu'><!ENTITY nu 'nu'><!ENTITY Omega 'Omega'>",
            "<!ENTITY omega 'omega'><!ENTITY Omicron 'Omicron'><!ENTITY omicron 'omicron'><!ENTITY Phi 'Phi'><!ENTITY phi 'phi'><!ENTITY Pi 'Pi'><!ENTITY pi 'pi'><!ENTITY piv 'piv'><!ENTITY Psi 'Psi'>",
            "<!ENTITY psi 'psi'><!ENTITY Rho 'Rho'><!ENTITY rho 'rho'><!ENTITY Sigma 'Sigma'><!ENTITY sigma 'sigma'><!ENTITY sigmaf 'sigmaf'><!ENTITY Tau 'Tau'><!ENTITY tau 'tau'><!ENTITY Theta 'Theta'>",
            "<!ENTITY theta 'theta'><!ENTITY thetasym 'thetasym'><!ENTITY upsih 'upsih'><!ENTITY Upsilon 'Upsilon'><!ENTITY upsilon 'upsilon'><!ENTITY upsi 'upsi'><!ENTITY Xi 'Xi'><!ENTITY xi 'xi'><!ENTITY Zeta 'Zeta'>",
            "<!ENTITY zeta 'zeta'>",
            "<!ENTITY acute 'acute'><!ENTITY cedil 'cedil'><!ENTITY circ 'circ'><!ENTITY macr 'macr'><!ENTITY middot 'middot'><!ENTITY tilde 'tilde'><!ENTITY uml 'uml'><!ENTITY Aacute 'Aacute'>",
            "<!ENTITY aacute 'aacute'><!ENTITY Acirc 'Acirc'><!ENTITY acirc 'acirc'><!ENTITY AElig 'AElig'><!ENTITY aelig 'aelig'><!ENTITY Agrave 'Agrave'><!ENTITY agrave 'agrave'><!ENTITY Aring 'Aring'>",
            "<!ENTITY aring 'aring'><!ENTITY Atilde 'Atilde'><!ENTITY atilde 'atilde'><!ENTITY Auml 'Auml'><!ENTITY auml 'auml'><!ENTITY Ccedil 'Ccedil'><!ENTITY ccedil 'ccedil'><!ENTITY Eacute 'Eacute'>",
            "<!ENTITY eacute 'eacute'><!ENTITY Ecirc 'Ecirc'><!ENTITY ecirc 'ecirc'><!ENTITY Egrave 'Egrave'><!ENTITY egrave 'egrave'><!ENTITY ETH 'ETH'><!ENTITY eth 'eth'><!ENTITY Euml 'Euml'>",
            "<!ENTITY euml 'euml'><!ENTITY Iacute 'Iacute'><!ENTITY iacute 'iacute'><!ENTITY Icirc 'Icirc'><!ENTITY icirc 'icirc'><!ENTITY Igrave 'Igrave'><!ENTITY igrave 'igrave'><!ENTITY Iuml 'Iuml'>",
            "<!ENTITY iuml 'iuml'><!ENTITY Ntilde 'Ntilde'><!ENTITY ntilde 'ntilde'><!ENTITY Oacute 'Oacute'><!ENTITY oacute 'oacute'><!ENTITY Ocirc 'Ocirc'><!ENTITY ocirc 'ocirc'><!ENTITY OElig 'OElig'>",
            "<!ENTITY oelig 'oelig'><!ENTITY Ograve 'Ograve'><!ENTITY ograve 'ograve'><!ENTITY Oslash 'Oslash'><!ENTITY oslash 'oslash'><!ENTITY Otilde 'Otilde'><!ENTITY otilde 'otilde'><!ENTITY Ouml 'Ouml'>",
            "<!ENTITY ouml 'ouml'><!ENTITY Scaron 'Scaron'><!ENTITY scaron 'scaron'><!ENTITY szlig 'szlig'><!ENTITY THORN 'THORN'><!ENTITY thorn 'thorn'><!ENTITY Uacute 'Uacute'><!ENTITY uacute 'uacute'>",
            "<!ENTITY Ucirc 'Ucirc'><!ENTITY ucirc 'ucirc'><!ENTITY Ugrave 'Ugrave'><!ENTITY ugrave 'ugrave'><!ENTITY Uuml 'Uuml'><!ENTITY uuml 'uuml'><!ENTITY Yacute 'Yacute'><!ENTITY yacute 'yacute'>",
            "<!ENTITY yuml 'yuml'><!ENTITY Yuml 'Yuml'><!ENTITY cent 'cent'><!ENTITY curren 'curren'><!ENTITY euro 'euro'><!ENTITY pound 'pound'><!ENTITY yen 'yen'><!ENTITY brvbar 'brvbar'><!ENTITY bull 'bull'>",
            "<!ENTITY copy 'copy'><!ENTITY dagger 'dagger'><!ENTITY Dagger 'Dagger'><!ENTITY frasl 'frasl'><!ENTITY hellip 'hellip'><!ENTITY iexcl 'iexcl'>",
            "<!ENTITY image 'image'><!ENTITY iquest 'iquest'><!ENTITY lrm 'lrm'><!ENTITY mdash 'mdash'><!ENTITY ndash 'ndash'><!ENTITY not 'not'><!ENTITY oline 'oline'><!ENTITY ordf 'ordf'><!ENTITY ordm 'ordm'>",
            "<!ENTITY para 'para'><!ENTITY permil 'permil'><!ENTITY prime 'prime'><!ENTITY Prime 'Prime'><!ENTITY real 'real'><!ENTITY reg 'reg'><!ENTITY rlm 'rlm'><!ENTITY sect 'sect'><!ENTITY shy 'shy'>",
            "<!ENTITY sup1 'sup1'><!ENTITY trade 'trade'><!ENTITY weierp 'weierp'><!ENTITY bdquo 'bdquo'><!ENTITY laquo 'laquo'><!ENTITY ldquo 'ldquo'><!ENTITY lsaquo 'lsaquo'><!ENTITY lsquo 'lsquo'>",
            "<!ENTITY raquo 'raquo'><!ENTITY rdquo 'rdquo'><!ENTITY rsaquo 'rsaquo'><!ENTITY rsquo 'rsquo'><!ENTITY sbquo 'sbquo'><!ENTITY emsp 'emsp'><!ENTITY ensp 'ensp'><!ENTITY nbsp 'nbsp'>",
            "<!ENTITY thinsp 'thinsp'><!ENTITY zwj 'zwj'><!ENTITY zwnj 'zwnj'><!ENTITY deg 'deg'><!ENTITY divide 'divide'><!ENTITY frac12 'frac12'><!ENTITY frac14 'frac14'><!ENTITY frac34 'frac34'>",
            "<!ENTITY ge 'ge'><!ENTITY le 'le'><!ENTITY minus 'minus'><!ENTITY sup2 'sup2'><!ENTITY sup3 'sup3'><!ENTITY times 'times'><!ENTITY alefsym 'alefsym'><!ENTITY and 'and'><!ENTITY ang 'ang'>",
            "<!ENTITY asymp 'asymp'><!ENTITY cap 'cap'><!ENTITY cong 'cong'><!ENTITY cup 'cup'><!ENTITY empty 'empty'><!ENTITY equiv 'equiv'><!ENTITY exist 'exist'><!ENTITY fnof 'fnof'><!ENTITY forall 'forall'>",
            "<!ENTITY infin 'infin'><!ENTITY int 'int'><!ENTITY isin 'isin'><!ENTITY lang 'lang'><!ENTITY lceil 'lceil'><!ENTITY lfloor 'lfloor'><!ENTITY lowast 'lowast'><!ENTITY micro 'micro'>",
            "<!ENTITY nabla 'nabla'><!ENTITY ne 'ne'><!ENTITY ni 'ni'><!ENTITY notin 'notin'><!ENTITY nsub 'nsub'><!ENTITY oplus 'oplus'><!ENTITY or 'or'><!ENTITY otimes 'otimes'><!ENTITY part 'part'>",
            "<!ENTITY perp 'perp'><!ENTITY plusmn 'plusmn'><!ENTITY prod 'prod'><!ENTITY prop 'prop'><!ENTITY radic 'radic'><!ENTITY rang 'rang'><!ENTITY rceil 'rceil'><!ENTITY rfloor 'rfloor'>",
            "<!ENTITY sdot 'sdot'><!ENTITY sim 'sim'><!ENTITY sub 'sub'><!ENTITY sube 'sube'><!ENTITY sum 'sum'><!ENTITY sup 'sup'><!ENTITY supe 'supe'><!ENTITY there4 'there4'>",
            "<!ENTITY crarr 'crarr'><!ENTITY darr 'darr'><!ENTITY dArr 'dArr'><!ENTITY harr 'harr'><!ENTITY hArr 'hArr'><!ENTITY larr 'larr'><!ENTITY lArr 'lArr'><!ENTITY rarr 'rarr'><!ENTITY rArr 'rArr'>",
            "<!ENTITY uarr 'uarr'><!ENTITY uArr 'uArr'><!ENTITY clubs 'clubs'><!ENTITY diams 'diams'><!ENTITY hearts 'hearts'><!ENTITY spades 'spades'><!ENTITY loz 'loz'>",
                                                            "<!ENTITY inf 'inf'><!ENTITY cap 'cap'><!ENTITY cup 'cup'><!ENTITY and 'and'><!ENTITY or 'or'><!ENTITY notin 'notin'><!ENTITY in 'in'>",
                                                            "<!ENTITY sube 'sube'><!ENTITY deg 'deg'><!ENTITY cent 'cent'><!ENTITY rangle 'rangle'><!ENTITY empty 'empty'><!ENTITY emptyop 'emptyop'>",
                                                            "<!ENTITY uparrow 'uparrow'><!ENTITY downarrow 'downarrow'><!ENTITY leftarrow 'leftarrow'><!ENTITY rightarrow 'rightarrow'>",
                                                            "<!ENTITY leftrightarrow 'leftrightarrow'><!ENTITY dleftrightarrow 'dleftrightarrow'><!ENTITY langle 'langle'><!ENTITY angle 'angle'>",
                                                            "<!ENTITY sqrt 'sqrt'><!ENTITY minusplus 'minusplus'><!ENTITY plusminus 'plusminus'><!ENTITY cdelta 'cdelta'><!ENTITY lognot 'lognot'>",
                                                            "<!ENTITY subset 'subset'><!ENTITY notsubset 'notsubset'><!ENTITY propsubset 'propsubset'><!ENTITY notpropsubset 'notpropsubset'>",
                                                            "<!ENTITY cgamma 'cgamma'><!ENTITY clambda 'clambda'><!ENTITY comega 'comega'><!ENTITY cphi 'cphi'><!ENTITY phiv 'phiv'><!ENTITY cpi 'cpi'>",
                                                            "<!ENTITY piv 'piv'><!ENTITY cpsi 'cpsi'><!ENTITY csigma 'csigma'><!ENTITY sigmav 'sigmav'><!ENTITY ctheta 'ctheta'><!ENTITY thetav 'thetav'>",
                                                            "<!ENTITY cupsi 'cupsi'><!ENTITY cxi 'cxi'><!ENTITY prime 'prime'><!ENTITY prime2 'prime2'><!ENTITY prime3 'prime3'><!ENTITY tilde 'tilde'>",
                                                            "<!ENTITY otimes 'otimes'><!ENTITY oplus 'oplus'><!ENTITY forall 'forall'><!ENTITY exists 'exists'><!ENTITY propsupset 'propsupset'>",
                                                            "<!ENTITY supset 'supset'><!ENTITY notpropsupset 'notpropsupset'><!ENTITY notsupset 'notsupset'><!ENTITY compfn 'compfn'>",
                                                            "<!ENTITY hyphen 'hyphen'><!ENTITY pound 'pound'><!ENTITY yen 'yen'><!ENTITY euro 'euro'><!ENTITY mdash 'mdash'>",
                                                            "<!ENTITY grad 'grad'><!ENTITY partiald 'partiald'><!ENTITY equiv 'equiv'><!ENTITY approx 'approx'><!ENTITY approxequal 'approxequal'>",
                                                            "<!ENTITY amp 'amp'><!ENTITY gt 'gt'><!ENTITY lt 'lt'><!ENTITY quot 'quot'>",
                                                            "]>");
        #endregion

        [DataMember]
        public List<Step> Steps { get; set; }

        [DataMember]
        public List<Response> Responses { get; set; }

        [DataMember]
        public List<Variable> Variables { get; set; }

        [DataMember]
        public VariableDictionary VariableLookup { get; set; }

        [DataMember]
        public string QuestionId { get; set; }

        [DataMember]
        public string QuestionTitle { get; set; }

        [DataMember]
        public string QuizId { get; set; }

        [DataMember]
        public string EntityId { get; set; }

        [DataMember]
        public string PlayerUrl { get; set; }

        [DataMember]
        public string FormulaEditorUrl { get; set; }

        [DataMember]
        public string EnrollmentId { get; set; }

        [DataMember]
        public string Solution { get; set; }

        [DataMember]
        public string MaxPoints { get; set; }

        [DataMember]
        public string RawXML { get; set; }

        [DataMember]
        public string PendingXML { get; set; }

        [DataMember]
        public string ContentWithInvalidXml { get; set; }

        /// <summary>
        /// Gets or sets flag from javascript that we are in Xml mode or UI mode when checking for question is saved. 
        /// </summary>
        [DataMember]
        public string ViewAsXml { get; set; }

        public HTSData()
        {
            Steps = new List<Step>();
            Responses = new List<Response>();
            VariableLookup = new VariableDictionary();
            this.ContentWithInvalidXml = "false";
        }

        public void LoadQuestionXml(string questionXml)
        {
            if (!string.IsNullOrEmpty(questionXml))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(questionXml);
                RawXML = xDoc.InnerXml;

                var questionNode = xDoc.SelectSingleNode("question");
                if (questionNode != null)
                {
                    Step oStep = new Step();
                    oStep.Id = "step0";
                    oStep.Sequence = 0;

                    var bodyNode = questionNode.SelectSingleNode("body");
                    if (bodyNode != null)
                    {
                        oStep.Question = "<p>" + bodyNode.InnerText + "</p>";
                    }

                    var interactionNode = questionNode.SelectSingleNode("interaction");
                    if (interactionNode != null)
                    {
                        InteractionNodeXml(questionNode, oStep, interactionNode);
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(oStep.Question);
                        doc.ConvertMCToImages(this);
                        doc.ConvertShortToImages(this);
                        doc.ConvertGraphsToImages(this);
                        oStep.Question = doc.ToHtmlString();
                        Steps.Add(oStep);
                    }
                }
            }
        }


        // XML before conversion to images
        private void InteractionNodeXml(XmlNode questionNode, Step oStep, XmlNode interactionNode)
        {
            Response response = new Response();
            response.ElementId = "1";
            response.Correct = "";
            response.Scramble = "";
            response.Points = "1";
            response.Text = "";

            Dictionary<string, string> choiceIds = new Dictionary<string, string>();
            bool haveAlphaChoiceIds = false;

            string questionType = "";
            if (interactionNode.Attributes["type"] != null)
            {
                questionType = interactionNode.Attributes["type"].Value;
            }

            switch (questionType)
            {
                case "text":
                    response.Text = questionNode.SelectSingleNode("body").InnerText;
                    response.Type = "text";
                    oStep.Question += response.ToShortNode().WriteTo();
                    Responses.Add(response);

                    break;
                case "choice":
                    var choiceNodes = interactionNode.SelectNodes("choice");
                    if (choiceNodes != null)
                    {
                        int idx = 0;

                        // choiceId must be numeric
                        // if at least one is not numeric then the new unique sequence will be generated
                        foreach (XmlNode choiceNode in choiceNodes)
                        {
                            var choiceId = choiceNode.Attributes["id"].Value;
                            int numChoiceId = 0;
                            if (!Int32.TryParse(choiceId, out numChoiceId))
                            {
                                haveAlphaChoiceIds = true;
                                break;
                            }
                        }

                        foreach (XmlNode choiceNode in choiceNodes)
                        {
                            idx++;
                            var choiceId = choiceNode.Attributes["id"].Value;

                            // will be using choiceIds dictionary to assign ids 
                            // and also to lookup the id for the correct answer
                            if (haveAlphaChoiceIds)
                                choiceIds.Add(choiceId, idx.ToString()); //convert to number
                            else
                                choiceIds.Add(choiceId, choiceId); // use original data

                            Choice choice = new Choice();
                            choice.ChoiceId = choiceIds[choiceId];
                            choice.Fixed = "false";
                            choice.Id = "@step.mc@id";
                            choice.Version = "2";
                            choice.Text = choiceNode.SelectSingleNode("body").InnerText;
                            response.Choices.Add(choice);
                        }
                    }

                    oStep.Question += response.ToMultipleChoiceNode(null).WriteTo();
                    Responses.Add(response);
                    break;

                default:
                    break;
            }

            var answerNode = questionNode.SelectSingleNode("answer/value");
            if (answerNode != null)
            {
                if (haveAlphaChoiceIds)
                {
                    // choice ids were converted to numbers
                    response.Correct = choiceIds[answerNode.InnerText];
                    Solution = choiceIds[answerNode.InnerText];
                }
                else
                {
                    response.Correct = answerNode.InnerText;
                    Solution = answerNode.InnerText;
                }
            }
        }

        public void LoadXML(string problemXml)
        {
            RawXML = problemXml;

            if (!string.IsNullOrEmpty(problemXml))
            {
                var xDocument = new XmlDocument();
                xDocument.PreserveWhitespace = true;
                problemXml = this.RemoveDocElements(problemXml);
                problemXml = string.Concat(iProblemDocumentType, problemXml);   // HTSHelper.ReplaceHTMLSymbols(this.RemoveDocElements(problemXml)); /*Regex.Replace(this.RemoveDocElements(problemXml), "&(?!amp;)", "&amp;");*/ /* For <a tag, src & alt can contain "&" -- replaceing it with "&amp;" */

                xDocument.LoadXml(problemXml);
                ParseXMLDocument(xDocument);
                xDocument.UpdateCurrentVariable(this);
            }

            if (Steps.Count == 0)
                Steps.Add(new Step());

            if (Responses.Count == 0)
                Responses.Add(new Response());
        }

        public void LoadFile(string filePath)
        {
            XmlDocument xDocument = new XmlDocument();
            xDocument.Load(filePath);
            ParseXMLDocument(xDocument);
            xDocument.UpdateCurrentVariable(this);
        }

        /// <summary>
        /// Generate XML document from htsData properties and return inner xml. 
        /// </summary>
        /// <returns>An XmlDocument object representing the HTS Data.</returns>
        public string ToXML()
        {
            string actualString = this.ToXmlDocument().InnerXml;
            return actualString.Remove(0, iProblemDocumentType.Length - 1); //.Replace("&amp;amp;","&amp;");
        }

        /// <summary>
        /// Generate XML and create XMLDocument object from HTSData. 
        /// </summary>
        /// <returns>An XmlDocument object representing the HTS Data.</returns>
        public XmlDocument ToXmlDocument()
        {
            XmlDocument doc = new XmlDocument();
            string startXml = string.Concat(iProblemDocumentType, "<iproblem></iproblem>");
            doc.LoadXml(startXml);
            XmlNode problemNode = doc.DocumentElement;

            var attr = doc.CreateAttribute("maxpoints");
            attr.Value = string.IsNullOrEmpty(this.MaxPoints) ? "1" : this.MaxPoints;
            problemNode.Attributes.Append(attr);

            var orderedStepList = Steps.OrderBy(s => s.Sequence).ToList();

            if (Variables != null)
            {
                VariableLookup = Variables.ToDictionary(k => k.Name, v => v);

                foreach (var variableEntry in VariableLookup)
                {
                    Variable v = variableEntry.Value;
                    XmlNode varNode = doc.CreateVariableNode(v);
                    problemNode.AppendChild(varNode);
                }
            }


            foreach (Step step in orderedStepList)
            {
                bool isLastStep = step.Sequence == orderedStepList.Count - 1;
                bool isSingleStep = orderedStepList.Count == 1;

                if (!string.IsNullOrEmpty(step.Question))
                {
                    var stepNode = doc.CreateStepNode(this, "step" + step.Sequence, step.Question);

                    var stepHtml = stepNode.ToValidXml(this);
                    //if (step.Sequence == 0)
                    //{
                    //    var title = string.IsNullOrEmpty(stepNode.InnerText.Trim()) ? "Advanced Question" : stepNode.InnerTextWithFormula().Trim().Split('\n')[0].ToString();
                    //    QuestionTitle = title.Length > 100 ? title.Substring(0, 99) : title;
                    //}

                    XmlNode hintNode = null;
                    XmlNode correctNode = null;
                    XmlNode incorrectNode = null;

                    if (!isLastStep)
                    {
                        int nextStepSequence = step.Sequence + 1;
                        string nextStepId = "step" + nextStepSequence;
                        stepHtml.AddHtsNextButton(isSingleStep, nextStepId);
                    }

                    if (!string.IsNullOrEmpty(step.Hint))
                    {
                        string stepId = "step" + step.Sequence + "hint";
                        hintNode = doc.CreateStepNode(this, stepId, step.Hint);
                        problemNode.AppendChild(hintNode);

                        stepHtml.AddHtsHintButton(stepId);
                    }

                    if (!string.IsNullOrEmpty(step.Correct))
                    {
                        string stepId = "step" + step.Sequence + "corr";
                        correctNode = doc.CreateStepNode(this, stepId, step.Correct);
                        problemNode.AppendChild(correctNode);

                        stepHtml.AddHtsNavNode("correct", stepId);
                    }

                    if (!string.IsNullOrEmpty(step.Incorrect))
                    {
                        string stepId = "step" + step.Sequence + "inc";
                        incorrectNode = doc.CreateStepNode(this, "step" + step.Sequence + "inc", step.Incorrect);
                        problemNode.AppendChild(incorrectNode);

                        stepHtml.AddHtsNavNode("incorrect", stepId);
                    }

                    //finish writing modified step xml to iproblem document.
                    stepNode.InnerXml = stepHtml.ToHtmlString();
                    problemNode.AppendChild(stepNode);

                    if (hintNode != null)
                    {
                        problemNode.AppendChild(hintNode);
                    }

                    if (correctNode != null)
                    {
                        problemNode.AppendChild(correctNode);
                    }

                    if (incorrectNode != null)
                    {
                        problemNode.AppendChild(incorrectNode);
                    }
                }
            }

            if (!string.IsNullOrEmpty(Solution))
            {
                var solutionNode = doc.CreateStepNode(this, "solution", Solution);
                problemNode.AppendChild(solutionNode);
            }

            doc.ConvertVariableReferences(this);

            return doc;
        }

        #region "Parsing methods"
        private Step FindStep(string stepId)
        {
            Step step = null;
            var stepsFound = Steps.Where(s => s.Id == stepId);
            if (stepsFound.Any())
            {
                step = stepsFound.First();
            }

            return step;
        }

        private void ParseXMLDocument(XmlDocument xDoc)
        {
            VariableLookup = xDoc.GetVariables(this);

            xDoc.RevertVariableReferences(VariableLookup);

            VariableLookup = xDoc.GetVariables(this);

            var problemNode = xDoc.SelectSingleNode("//iproblem");
            if (problemNode.Attributes["maxpoints"] != null)
            {
                this.MaxPoints = problemNode.Attributes["maxpoints"].Value;
            }
            else
            {
                this.MaxPoints = "1";
            }

            var nodes = xDoc.DocumentElement.SelectNodes("//iprostep");
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes["id"] != null)
                    {
                        string stepId = node.Attributes["id"].Value;
                        string stepType = GetStepType(stepId);
                        string parentStepId = string.Empty;

                        Step oStep = null;

                        switch (stepType)
                        {
                            case "corr":
                                parentStepId = stepId.Replace("corr", "");
                                oStep = FindStep(parentStepId);
                                oStep.Correct = node.ToValidHtml(this);
                                break;
                            case "inc":
                                parentStepId = stepId.Replace("inc", "");
                                oStep = FindStep(parentStepId);
                                oStep.Incorrect = node.ToValidHtml(this);
                                break;
                            case "hint":
                                parentStepId = stepId.Replace("hint", "");
                                oStep = FindStep(parentStepId);
                                oStep.Hint = node.ToValidHtml(this);
                                break;
                            case "solution":
                                Solution = node.ToValidHtml(this);
                                break;
                            case "step":
                                var step = node.ToStep(this);
                                Steps.Add(step);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        #endregion
        
        #region "String helper methods"
        private string GetStepType(string stepId)
        {
            string type = "step";

            if (stepId.ToLowerInvariant().Contains("solution"))
                type = "solution";

            else if (stepId.ToLowerInvariant().Contains("corr"))
                type = "corr";

            else if (stepId.ToLowerInvariant().Contains("inc"))
                type = "inc";

            else if (stepId.ToLowerInvariant().Contains("hint"))
                type = "hint";

            else
                type = "step";

            return type;
        }

        private VarType GetVarType(string sType)
        {
            switch (sType)
            {
                case "numeric":
                    return VarType.Numeric;
                case "math":
                    return VarType.Math;
                case "numericarray":
                    return VarType.NumericArray;
                case "textarray":
                    return VarType.TextArray;
                case "text":
                    return VarType.Text;
                default:
                    return VarType.Numeric;
            }
        }

        private ConditionType GetConditionType(string sType)
        {
            switch (sType)
            {
                case "eq":
                    return ConditionType.Equal;
                case "lt":
                    return ConditionType.LessThan;
                case "gt":
                    return ConditionType.GreaterThan;
                case "le":
                    return ConditionType.LessThanOrEqual;
                case "ge":
                    return ConditionType.GreaterThanOrEqual;
                default:
                    return ConditionType.Equal;
            }
        }

        private string RemoveDocElements(string problemXml)
        {
            string result = "";
            //load and change markup                            
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();            
            doc.LoadHtml(problemXml);
            doc.RemoveNavElements();
            doc.RemoveButtonElements();
            doc = EncodeDocElement(doc);
            result = doc.ToHtmlString();
            return result;
        }

        /// <summary>
        /// This method will be used to encode url link as per HTML. Else XML parsing will complain for this.
        /// </summary>
        /// <param name="doc">HTML AgilityPack Document</param>
        /// <returns>HTML AgilityPack Document as reference</returns>
        private HtmlAgilityPack.HtmlDocument EncodeDocElement(HtmlAgilityPack.HtmlDocument doc)
        {
            EncodeSingleDocElement(doc, htmlElement: "a", elementAttribute: "href");
            EncodeSingleDocElement(doc, htmlElement: "img", elementAttribute: "src");
            EncodeTextElement(doc);
            return doc;
        }

        /// <summary>
        /// This helper method will help to change one element attribute at a time.
        /// </summary>
        /// <param name="doc">HTML AgilityPack Document</param>
        /// <param name="htmlElement">HTML element tag</param>
        /// <param name="elementAttribute">Attribute within an element needed to be changed.</param>
        private static void EncodeSingleDocElement(HtmlAgilityPack.HtmlDocument doc, string htmlElement, string elementAttribute)
        {
            var anchorNodes = doc.DocumentNode.SelectNodes(string.Format("//{0}[@{1}]", htmlElement, elementAttribute));
            if (anchorNodes != null)
            {
                foreach (HtmlNode node in anchorNodes)
                {
                    node.Attributes[elementAttribute].Value = HttpUtility.HtmlEncode(node.Attributes[elementAttribute].Value);
                }
            }
        }

        private static void EncodeTextElement(HtmlAgilityPack.HtmlDocument doc)
        {
            var textNodes = doc.DocumentNode.SelectNodes("//text()[normalize-space(.) != '']");

            if (textNodes == null) return;

            foreach (HtmlNode textNode in textNodes)
            {
                var parentNode = textNode.ParentNode.Name;
                if (parentNode.Equals("p", StringComparison.InvariantCultureIgnoreCase) || 
                    parentNode.Equals("span", StringComparison.InvariantCultureIgnoreCase) ||
                    parentNode.Equals("strong", StringComparison.InvariantCultureIgnoreCase) ||
                    parentNode.Equals("em", StringComparison.InvariantCultureIgnoreCase))
                {
                    string innerText = textNode.InnerText;//.Trim();
                    innerText = HttpUtility.HtmlEncode(innerText);
                    innerText = Regex.Replace(innerText, "&amp;#", "&#");
                    textNode.InnerHtml = innerText;
                }
            }
        }

        #endregion
    }
}