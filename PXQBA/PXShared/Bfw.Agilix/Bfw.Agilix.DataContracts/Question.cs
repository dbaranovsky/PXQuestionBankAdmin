using System;
using System.Collections.Generic;
using System.Windows.Annotations;
using System.Xml.Linq;
using System.Linq;
using System.Xml.XPath;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// A question that is part of an Assessment.
    /// </summary>
    public class Question : IDlapEntityParser, IDlapEntityTransformer
    {
        #region Constants

        /// <summary>
        /// Agilix has a 'default score' that appears to be 1.  I am not sure how it is set or if it can be changed.
        /// </summary>
        private const int AGX_DEFAULT_SCORE = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Id of the question.
        /// </summary>
        public string Id;

        /// <summary>
        /// The question's title
        /// </summary>
        public string Title;

        /// <summary>
        /// the Question's Feedback
        /// </summary>
        public string GeneralFeedback;

        /// <summary>
        /// Id of the entity the question exists in.
        /// </summary>
        public string EntityId;

        /// <summary>
        /// Body of the question. Format depends on question type.
        /// </summary>
        public string Body;

        /// <summary>
        /// URL o service that handles this type of question.
        /// </summary>
        public string CustomUrl;

        /// <summary>
        /// Type of interaction this question requires.
        /// </summary>
        public string InteractionType
        {
            get { return Interaction.Type; }
            set { Interaction.Type = value;  }
        }
        
        /// <summary>
        /// Contains question type, properties, and choices 
        /// </summary>
        public QuestionInteraction Interaction;

        /// <summary>
        /// Possible points user can earn from this question.
        /// </summary>
        public double Score;

        /// <summary>
        /// Choice list if this is a multiple choice question.
        /// </summary>
        public IList<QuestionChoice> Choices;

        /// <summary>
        /// Correct answer.
        /// </summary>
        public string Answer
        {
            get
            {
                string result = string.Empty;

                if (AnswerList != null && AnswerList.Count > 0)
                {
                    result = AnswerList.First();
                }

                return result;
            }
        }
 
        /// <summary>
        /// Answer List
        /// </summary>
        public IList<string> AnswerList; 

        /// <summary>
        /// XML representing the question.
        /// </summary>
        public string QuestionXml;

        /// <summary>
        /// Data required by the interaction.
        /// </summary>
        public string InteractionData;

        /// <summary>
        /// All question metadata used to determine question policies.
        /// </summary>
        public List<string> QuestionMetaData;

        /// <summary>
        /// Gets or sets the searchable meta data.
        /// </summary>
        /// <value>
        /// The searchable meta data.
        /// </value>
        public Dictionary<string, string> SearchableMetaData { get; set; }


        /// <summary>
        /// Gets or sets the learning curve question settings.
        /// </summary>
        /// <value>
        /// The learning curve question settings.
        /// </value>
        public List<LearningCurveQuestionSettings> LearningCurveQuestionSettings { get; set; }

        /// <summary>
        /// Groups that the question belongs to.
        /// </summary>
        public List<string> AssessmentGroups;

        /// <summary>
        /// Excercise Number for the  question.
        /// </summary>
        public string ExcerciseNo { get; set; }

        /// <summary>
        /// Question Bank for the  question.
        /// </summary>
        public string QuestionBank { get; set; }

        /// <summary>
        /// eBook Chapter for the  question.
        /// </summary>
        public string eBookChapter { get; set; }

        /// <summary>
        /// Difficulty for the  question.
        /// </summary>
        public string Difficulty { get; set; }

        /// <summary>
        /// Congnitive Level for the  question.
        /// </summary>
        public string CognitiveLevel { get; set; }

        /// <summary>
        /// Blooms Domain for the  question.
        /// </summary>
        public string BloomsDomain { get; set; }

        /// <summary>
        /// Guidance for the  question.
        /// </summary>
        public string Guidance { get; set; }

        /// <summary>
        /// Free Response Question for the  question.
        /// </summary>
        public string FreeResponseQuestion { get; set; }

        /// <summary>
        /// The Id for the question this question is used in.
        /// </summary>
        public string UsedIn { get; set; }
        /// <summary>
        /// Question particular version
        /// </summary>
        public string QuestionVersion { get; set; }

        /// <summary>
        /// Dictionary of learning objectives for question
        /// </summary>
        public Dictionary<string, string> LearningObjectives { get; set; }

        /// <summary>
        /// Dictionarty of Suggested Uses for a question
        /// </summary>
        public Dictionary<string, string> SuggestedUse { get; set; }


        /// <summary>
        /// Dictionary of Selected learning objectives from question
        /// </summary>
        public Dictionary<string, string> SelectedLearningObjectives { get; set; }

        /// <summary>
        /// Dictionarty of Selected Suggested Uses from question
        /// </summary>
        public Dictionary<string, string> SelectedSuggestedUse { get; set; }


        /// <summary>
        /// The Id for the question this question is used in.
        /// </summary>
        public string QuestionBankText { get; set; }

        /// <summary>
        /// Chapter to which quiz belongs
        /// </summary>
        public string EbookSectionText { get; set; }

        /// <summary>
        /// Question status could be "deleted", "In progress", "Available to Instructor"
        /// </summary>
        public string QuestionStatus { get; set; }

        /// <summary>
        /// All the first level elements inside meta tag
        /// </summary>
        public Dictionary<string, XElement> MetadataElements { get; set; } 
       
        /// <summary>
        /// Date the question was modified
        /// </summary>
        public DateTime ModifiedDate { get; set; }


        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(XElement element)
        {
            QuestionXml = element.ToString();
            if (element.Attribute(ElStrings.ResourceEntityId) != null)
            {
                EntityId = element.Attribute(ElStrings.ResourceEntityId).Value;    
            }
            else if (element.Attribute(ElStrings.Entityid) != null)
            {
                EntityId = element.Attribute(ElStrings.Entityid).Value;
            }
            Id = element.Attribute(ElStrings.QuestionId).Value;
            Body = element.Element(ElStrings.Body).Value;

            var questionVersion = element.Attribute(ElStrings.QuestionVersion);
            if (questionVersion != null)
            {
                QuestionVersion = questionVersion.Value;
            }

            var modifiedDate = element.Attribute(ElStrings.ModifiedDate);
            DateTime modifiedDateParsed = DateTime.MinValue;
            if (modifiedDate != null && DateTime.TryParse(modifiedDate.Value, out modifiedDateParsed))
            {
                ModifiedDate = modifiedDateParsed;
            }

            var feedback = element.Element(ElStrings.feedback);
            if (feedback != null)
            {
                GeneralFeedback = feedback.Value;
            }

            Choices = new List<QuestionChoice>();
           
            ParseInteraction(element);

            var answer = element.Element(ElStrings.answer);
            if (null != answer)
            {
                AnswerList = new List<string>();

                foreach (var node in answer.Nodes())
                {
                    AnswerList.Add((node as XElement).Value);
                }
            }

            var customurl = element.Element(ElStrings.CustomURL);
            if (null != customurl)
            {
                CustomUrl = customurl.Value;
            }

            
            var score = element.Attribute(ElStrings.Score);
            if (score != null)
            {
                Score = Convert.ToDouble(score.Value);
            }
            else
            {
                Score = AGX_DEFAULT_SCORE;
            }

            var groups = element.Element(ElStrings.Groups);

            AssessmentGroups = new List<string>();

            if (null != groups)
            {
                foreach (XElement group in groups.Elements(ElStrings.Group))
                {
                    AssessmentGroups.Add(group.Value);
                }
            }

            var searchablemetaData = element.Element(ElStrings.Meta);
            if (null != searchablemetaData)
            {
                SearchableMetaData = new Dictionary<string, string>();
                MetadataElements = new Dictionary<string, XElement>();
                foreach (XElement xElement in searchablemetaData.Elements())
                {
                    if (!MetadataElements.ContainsKey(xElement.Name.ToString()))
                    {
                        MetadataElements.Add(xElement.Name.ToString(), xElement);
                    }
                }
                foreach (XElement meta in searchablemetaData.Elements().Where(m => m.Name != ElStrings.ProductCourseDefaults && !m.Name.ToString().Contains(ElStrings.ProductCourseSection.ToString())))
                {
                    if (meta.Name == ElStrings.title)
                    {
                        Title = meta.Value;
                    }

                    if (meta.Attribute(ElStrings.searchterm) != null)
                    {
                        meta.Value = String.Format("[[{0}{1}]]", meta.Attribute(ElStrings.searchterm).Value, meta.Value);
                    }

                    if (meta.Name != "meta-lc-question-settings")
                    {
                        if (!SearchableMetaData.ContainsKey(meta.Name.ToString()))
                        {
                            SearchableMetaData.Add(meta.Name.ToString(), meta.Value);    
                        }
                        else
                        {
                            SearchableMetaData[meta.Name.ToString()] = meta.Value;
                        }                        
                    }
                }

                var learningCurveQuestionSettings = searchablemetaData.Element(ElStrings.meta_lc_question_settings);
                if (null != learningCurveQuestionSettings)
                {
                    var bizlcQuestionSettings = new List<LearningCurveQuestionSettings>();
                    foreach (var content in learningCurveQuestionSettings.Elements(ElStrings.meta_lc_question_setting))
                    {
                        var lcQuestionSetting = new LearningCurveQuestionSettings();
                        lcQuestionSetting.ParseEntity(content);

                        bizlcQuestionSettings.Add(lcQuestionSetting);
                    }

                    LearningCurveQuestionSettings = bizlcQuestionSettings;
                }
                
                ExcerciseNo = searchablemetaData.Element(ElStrings.ExcerciseNo) == null ? "" : searchablemetaData.Element(ElStrings.ExcerciseNo).Value;
                var questionbankXML = searchablemetaData.Element(ElStrings.QuestionBank_UsedIn);

                if (questionbankXML != null)
                {
                    var values = questionbankXML.Element("value");

                    if (values != null)
                    {

                        QuestionBank = values.Attribute("id") == null ? "" : values.Attribute("id").Value;
                        QuestionBankText = values.Attribute("name") == null ? "" : values.Attribute("name").Value;
                    }
                    
                }

                var ebookSection = searchablemetaData.Element(ElStrings.eBookChapter);

                if (ebookSection != null)
                {
                    var values = ebookSection.Element("value");

                    if (values != null)
                    {

                        eBookChapter = values.Attribute("id") == null ? "" : values.Attribute("id").Value;
                        EbookSectionText = values.Attribute("name") == null ? "" : values.Attribute("name").Value;
                    }

                }

                //PX-3448 adding Status property to Question //default value = "available to instructor" : 1
                QuestionStatus = searchablemetaData.Element(ElStrings.QuestionStatus) == null ? "1" : searchablemetaData.Element(ElStrings.QuestionStatus).Value;
                                

                var suggesteduse = searchablemetaData.Element("suggesteduse");
                SelectedSuggestedUse = new Dictionary<string, string>();
                if (suggesteduse != null)
                {
                    
                    var values = suggesteduse.Elements("value");
                    foreach (XElement el in values)
                    {

                        if (el != null)
                        {
                            SelectedSuggestedUse.Add(el.Attribute("id") == null ? "" : el.Attribute("id").Value, el.Attribute("name") == null ? "" : el.Attribute("name").Value);
                        }

                    }
                }

                var learningobjectives = searchablemetaData.Element("learningobjectives");
                SelectedLearningObjectives = new Dictionary<string, string>();
                if (learningobjectives != null)
                {
                    
                    var values = learningobjectives.Elements("value");
                    foreach (XElement el in values)
                    {
                        if (el != null)
                        {
                            SelectedLearningObjectives.Add(el.Attribute("id") == null ? "" : el.Attribute("id").Value, el.Attribute("name") == null ? "" : el.Attribute("name").Value);
                        }

                    }
                }


                if (searchablemetaData.Element("suggesteduse-searchable") != null)
                {
                    SearchableMetaData["suggesteduse"] = searchablemetaData.Element("suggesteduse-searchable").Value;
                    SearchableMetaData.Remove("suggesteduse-searchable");
                }


                if (searchablemetaData.Element("learningobjectives-searchable") != null)
                {
                    SearchableMetaData["learningobjectives"] = searchablemetaData.Element("learningobjectives-searchable").Value;
                    SearchableMetaData.Remove("learningobjectives-searchable");
                }
                  

                Difficulty = searchablemetaData.Element(ElStrings.Difficulty) == null ? "" : searchablemetaData.Element(ElStrings.Difficulty).Value;
                CognitiveLevel = searchablemetaData.Element(ElStrings.CognitiveLevel) == null ? "" : searchablemetaData.Element(ElStrings.CognitiveLevel).Value;
                BloomsDomain = searchablemetaData.Element(ElStrings.BloomsDomain) == null ? "": searchablemetaData.Element(ElStrings.BloomsDomain).Value;
                Guidance = searchablemetaData.Element(ElStrings.Guidance) == null ? "" : searchablemetaData.Element(ElStrings.Guidance).Value;
                FreeResponseQuestion = searchablemetaData.Element(ElStrings.FreeResponseQuestion) == null ? "": searchablemetaData.Element(ElStrings.FreeResponseQuestion).Value;
            }
            
            GetQuestionMetaData(QuestionXml);
        }

        private XElement ParseInteraction(XElement element)
        {
            var interaction = element.Element(ElStrings.Interaction);
            if (interaction != null)
            {
                Interaction.Type = interaction.Attribute(ElStrings.type).Value;
                var flags = interaction.Attribute(ElStrings.flags);

                if (flags != null)
                {
                    var flagsEnum = QuestionInteractionFlags.None;
                    Enum.TryParse(flags.Value, true, out flagsEnum);
                    Interaction.Flags = flagsEnum;
                }


                var width = interaction.AttributeValueAsInt(ElStrings.InteractionWidth, 0);
                if (width > 0)
                {
                    Interaction.Width = width;
                }

                var height = interaction.AttributeValueAsInt(ElStrings.InteractionHeight, 0);
                if (width > 0)
                {
                    Interaction.Height = height;
                }

                var significantFigures = interaction.AttributeValueAsInt(ElStrings.InteractionSignificantFigures, -1);
                if (width > -1)
                {
                    Interaction.SignificantFigures = significantFigures;
                }

                Interaction.TextType = interaction.AttributeValue(ElStrings.TextType.ToString(), "");

                var interactionData = interaction.Element(ElStrings.Data);
                if (interactionData != null)
                {
                    InteractionData = interactionData.Value;
                }

                var choices = interaction.Elements(ElStrings.Choice);
                if (!choices.IsNullOrEmpty())
                {
                    foreach (var choice in choices)
                    {
                        QuestionChoice qc = new QuestionChoice();
                        var choiceAnswer = choice.Element(ElStrings.answer);
                        if (choiceAnswer != null)
                        {
                            qc.Answer = choiceAnswer.Value;
                        }
                        var body = choice.Element(ElStrings.Body);
                        if (body != null)
                        {
                            var feeback = choice.Element(ElStrings.feedback) == null ? "" : choice.Element(ElStrings.feedback).Value;
                            qc.Text = body.Value;
                            qc.Id = choice.Attribute(ElStrings.Id).Value;
                            qc.Feedback = feeback;
                            Choices.Add(qc);
                        }


                    }
                }

            }
            return interaction;
        }


        private void GetQuestionMetaData(string xmlDoc)
        {
            QuestionMetaData = new List<string>();
            XDocument xDoc = XDocument.Parse(xmlDoc);
            var xpathList = System.Configuration.ConfigurationManager.AppSettings["QuestionMetadata"];

            if (!string.IsNullOrEmpty(xpathList))
            {
                string[] xpaths = xpathList.Split('|');
                foreach (var path in xpaths)
                {
                    var xpath = path.Split(',');
                    XElement xElement = xDoc.XPathSelectElement("//" + xpath[0]);
                    if (xElement != null)
                    {
                        string val = xElement.Value;
                        QuestionMetaData.Add(String.Format(xpath[1], val));
                    }
                }
            }
        }

        private XElement CreateAnswerBlock()
        {
            XElement answer = new XElement(ElStrings.answer);

            if (AnswerList != null && AnswerList.Count() > 1)
            {
                foreach (var choice in AnswerList)
                {
                    answer.Add(new XElement(ElStrings.Value, choice));
                }
            }
            else 
            {
                if (InteractionType.ToLower() == "text")
                {
                    answer.Add(new XElement(ElStrings.Value, Answer));
                }
                else
                {
                    foreach (var choice in Answer)
                    {
                        answer.Add(new XElement(ElStrings.Value, choice));
                    }
                }
            }

            return answer;
        }

        #endregion

        #region IDlapEntityTransformer Members

        /// <summary>
        /// Transforms internal object state into an XElement representation of a DLAP entity
        /// </summary>
        /// <returns>XElement containing the transformed object state</returns>
        /// <remarks></remarks>
        public XElement ToEntity()
        {
            var element = new XElement(
                ElStrings.question,
                new XAttribute(ElStrings.Schema, 2),
                new XAttribute(ElStrings.QuestionId, Id),
                new XAttribute(ElStrings.Entityid, EntityId),
                new XAttribute(ElStrings.Partial, InteractionType.ToLower() == "match" ? "true" : "false"),
                new XElement(ElStrings.Body, Body),
                new XElement(ElStrings.Groups, ""),
                new XElement(
                    ElStrings.Interaction,
                    new XAttribute(ElStrings.type, InteractionType.ToLower()),
                    new XElement(ElStrings.Data, InteractionData)
                ),
                //Answer is required for choice, answer, and text questions. It is ignored for match and essay questions. Specify multiple answer nodes for answer questions. 
                (!Answer.IsNullOrEmpty() || InteractionType.ToLower() == "answer" || InteractionType.ToLower() == "choice") ?
                    CreateAnswerBlock() :
                    new XElement(ElStrings.answer, Answer),
                new XElement(ElStrings.Meta, "")              
            );

          
            if (!String.IsNullOrEmpty(GeneralFeedback))
            {
                element.Add(new XElement(ElStrings.feedback, GeneralFeedback));
            }

            if (!String.IsNullOrEmpty(CustomUrl))
            {
                element.Add(new XElement(ElStrings.CustomURL, CustomUrl));
            }

            //for short answer questions we want to ignore case by default
            if (InteractionType.ToLower() == "text" && Interaction.TextType.IsNullOrEmpty()) 
            {
                Interaction.TextType = "IgnoreCase";
            }

            WriteInteraction(element);

            if (Score != AGX_DEFAULT_SCORE)
            {
                element.Add(new XAttribute(ElStrings.Score, Score));
            }

            if (!AssessmentGroups.IsNullOrEmpty())
            {
                var groups = element.Element(ElStrings.Groups);
                foreach (var grp in AssessmentGroups)
                {
                    groups.Add(
                        new XElement(ElStrings.Group, grp)); 
                }
            }

            if (!Title.IsNullOrEmpty())
            {
                var metas = element.Element(ElStrings.Meta);

                if (metas == null)
                {
                    element.Add(new XElement(metas));
                }

                metas.Add(new XElement(ElStrings.title, Title));
            }

            if (element.Element("meta") != null)
            {
                var meta = element.Element("meta");

                // writes the First Set of MetaData fields first then additional Properties must be writen below this For Loop //
                if (!SearchableMetaData.IsNullOrEmpty())
                {
                    var metas = element.Element(ElStrings.Meta);
                    foreach (var data in SearchableMetaData)
                    {
                        var meta_element = meta.Element(data.Key);
                        if (meta_element == null)
                        {
                            metas.Add(new XElement(data.Key, data.Value));
                        }
                        else
                        {
                            meta_element.Value = data.Value;
                        }
                    }

                    if (LearningCurveQuestionSettings != null)
                    {
                        var learningCurveQuestoinSettings = new XElement(ElStrings.meta_lc_question_settings);
                        foreach (var content in LearningCurveQuestionSettings)
                        {
                            if (content.Id != null)
                            {
                                learningCurveQuestoinSettings.Add(new XElement(ElStrings.meta_lc_question_setting, new XAttribute(ElStrings.Id, content.Id)));
                                var meta_setting = learningCurveQuestoinSettings.Element(ElStrings.meta_lc_question_setting);
                                meta_setting.Add(new XAttribute(ElStrings.never_scramble_answers, content.NeverScrambleAnswers));
                                meta_setting.Add(new XAttribute(ElStrings.primary_question, content.PrimaryQuestion));
                                meta_setting.Add(new XAttribute(ElStrings.difficulty_level, content.DifficultyLevel));
                            }
                        }
                        metas.Add(learningCurveQuestoinSettings);
                    }
                }

                if (!MetadataElements.IsNullOrEmpty())
                {
                    foreach (var data in MetadataElements)
                    {
                        var meta_element = meta.Element(data.Key);
                        if (meta_element == null)
                        {
                            meta.Add(data.Value);
                        }
                    }
                }

                if (Title != null)
                {
                    var title = meta.Element("title");
                    if (title == null)
                    {
                        title = new XElement("title");
                        title.Value = Title;
                        meta.Add(new XElement(title));
                    }
                    else
                    {
                        title.Value = Title;
                    }
                }

                if (ExcerciseNo != null)
                {
                    var exercisenumber = meta.Element(ElStrings.ExcerciseNo);
                    if (exercisenumber == null)
                    {
                        exercisenumber = new XElement(ElStrings.ExcerciseNo);
                        exercisenumber.Value = ExcerciseNo;
                        meta.Add(new XElement(exercisenumber));
                    }
                    else
                    {
                        exercisenumber.Value = ExcerciseNo;
                    }
                }

                if (QuestionBank != null)
                {

                    XElement value = new XElement("value");
                    value.SetAttributeValue("id", QuestionBank);
                    value.SetAttributeValue("name", QuestionBankText);

                    var questionBank = meta.Element(ElStrings.QuestionBank_UsedIn);

                    if (questionBank == null)
                    {
                        questionBank = new XElement(ElStrings.QuestionBank_UsedIn);
                        questionBank.Add(value);
                        meta.Add(new XElement(questionBank));
                    }
                    else
                    {
                        questionBank.RemoveAll();
                        questionBank.Add(value);
                    }

                }

                if (eBookChapter != null)
                {
                    var ebooksection = meta.Element(ElStrings.eBookChapter);

                    XElement value = new XElement("value");
                    value.SetAttributeValue("id", eBookChapter);
                    value.SetAttributeValue("name", EbookSectionText);

                    if (ebooksection == null)
                    {
                        ebooksection = new XElement(ElStrings.eBookChapter);
                        ebooksection.Add(value);
                        meta.Add(new XElement(ebooksection));
                    }
                    else
                    {
                        ebooksection.RemoveAll();
                        ebooksection.Add(value);
                    }
  
                }


                //PX-3448 adding Status property to Question to push to DLAP
                if (QuestionStatus != null)
                {
                    var QstatusNode = meta.Element(ElStrings.QuestionStatus);
                    if (QstatusNode == null)
                    {
                        QstatusNode = new XElement(ElStrings.QuestionStatus);
                        QstatusNode.Value = QuestionStatus;
                        meta.Add(new XElement(QstatusNode));
                    }
                    else
                    {
                        QstatusNode.Value = QuestionStatus;

                    }
                }


                if (Difficulty != null)
                {
                    var difficultyNode = meta.Element(ElStrings.Difficulty);
                    if (difficultyNode == null)
                    {
                        difficultyNode = new XElement(ElStrings.Difficulty);
                        difficultyNode.Value = Difficulty;
                        meta.Add(new XElement(difficultyNode));
                    }
                    else
                    {
                        difficultyNode.Value = Difficulty;

                    }
                }

                if (CognitiveLevel != null)
                {
                    var cognitivelevel = meta.Element(ElStrings.CognitiveLevel);
                    if (cognitivelevel == null)
                    {
                        cognitivelevel = new XElement(ElStrings.CognitiveLevel);
                        cognitivelevel.Value = CognitiveLevel;
                        meta.Add(new XElement(cognitivelevel));
                    }
                    else
                    {
                        cognitivelevel.Value = CognitiveLevel;

                    }
                }

                if (BloomsDomain != null)
                {

                    var bloomDomain = meta.Element(ElStrings.BloomsDomain);
                    if (bloomDomain == null)
                    {
                        bloomDomain = new XElement(ElStrings.BloomsDomain);
                        bloomDomain.Value = BloomsDomain;
                        meta.Add(new XElement(bloomDomain));
                    }
                    else
                    {
                        bloomDomain.Value = BloomsDomain;

                    }
                }
                if (Guidance != null)
                {
                    var guidance = meta.Element(ElStrings.Guidance);
                    if (guidance == null)
                    {
                        guidance = new XElement(ElStrings.Guidance);
                        guidance.Value = Guidance;
                        meta.Add(new XElement(guidance));
                    }
                    else
                    {
                        guidance.Value = Guidance;

                    }
                }

                if (FreeResponseQuestion != null)
                {
                    var freeresponsequestion = meta.Element(ElStrings.FreeResponseQuestion);





                    if (freeresponsequestion == null)
                    {
                        freeresponsequestion = new XElement(ElStrings.FreeResponseQuestion);
                        freeresponsequestion.Value = FreeResponseQuestion;
                        meta.Add(new XElement(freeresponsequestion));
                    }
                    else
                    {
                        freeresponsequestion.Value = FreeResponseQuestion;

                    }
                }

                if(SuggestedUse != null){

                    var suggesteduse = meta.Element("suggesteduse");
                    if (suggesteduse == null)
                    {
                        suggesteduse = new XElement("suggesteduse");
                        suggesteduse = AddValuesFromDictionary(SuggestedUse, suggesteduse);
                        meta.Add(new XElement(suggesteduse));
                    }
                    else
                    {
                        suggesteduse.RemoveAll();
                        suggesteduse = AddValuesFromDictionary(SuggestedUse, suggesteduse);
                        //meta.Add(new XElement(suggesteduse));
                    }


                    // creating a pipe-delimted string for Question data filtering requirement
                    var suggesteduse_values = meta.Element("suggesteduse-searchable");
                    if (suggesteduse_values == null)
                    {
                        suggesteduse_values = new XElement("suggesteduse-searchable");
                        suggesteduse_values.Value = string.Join("|", SuggestedUse.Select(x => x.Value));
                        meta.Add(new XElement(suggesteduse_values));
                    }
                    else
                    {
                        suggesteduse_values.RemoveAll();
                        suggesteduse_values.Value = string.Join("|", SuggestedUse.Select(x => x.Value));
                    }

                }
                if (LearningObjectives != null)
                {

                    var learningobjectives = meta.Element("learningobjectives");
                    if (learningobjectives == null)
                    {
                        learningobjectives = new XElement("learningobjectives");
                        learningobjectives = AddValuesFromDictionary(LearningObjectives, learningobjectives);
                        meta.Add(new XElement(learningobjectives));
                    }
                    else
                    {
                        learningobjectives.RemoveAll();
                        learningobjectives = AddValuesFromDictionary(LearningObjectives, learningobjectives);
                    }

                    // creating a pipe-delimted string for Question data filtering requirement
                    var learningobjectives_values = meta.Element("learningobjectives-searchable");
                    if (learningobjectives_values == null)
                    {
                        learningobjectives_values = new XElement("learningobjectives-searchable");
                        learningobjectives_values.Value = string.Join("|", LearningObjectives.Select(x => x.Value));
                        meta.Add(new XElement(learningobjectives_values));
                    }
                    else
                    {
                        learningobjectives_values.RemoveAll();
                        learningobjectives_values.Value = string.Join("|", LearningObjectives.Select(x => x.Value));
                    }

                }



            }

            return element;
        }

        private void WriteInteraction(XElement element)
        {
            var interaction = element.Element(ElStrings.Interaction);

            if (Interaction.Flags != QuestionInteractionFlags.None)
                interaction.SetAttributeValue(ElStrings.flags, (int)Interaction.Flags);
            if (Interaction.Height > 0)
                interaction.SetAttributeValue(ElStrings.InteractionHeight, Interaction.Height);
            if (Interaction.Width > 0)
                interaction.SetAttributeValue(ElStrings.InteractionWidth, Interaction.Width);
            if (Interaction.SignificantFigures > 0)
                interaction.SetAttributeValue(ElStrings.InteractionSignificantFigures, Interaction.SignificantFigures);
            if (!Interaction.TextType.IsNullOrEmpty())
                interaction.SetAttributeValue(ElStrings.TextType, Interaction.TextType);

            if (!Choices.IsNullOrEmpty())
            {
                foreach (var choice in Choices)
                {
                    var choiceElem = new XElement(ElStrings.Choice,
                        new XAttribute(ElStrings.Id, choice.Id),
                        new XElement(ElStrings.feedback, choice.Feedback),
                        new XElement(ElStrings.Body, choice.Text)
                        );
                    if (!choice.Answer.IsNullOrEmpty())
                    {
                        choiceElem.Add(new XElement(ElStrings.answer, choice.Answer));
                    }
                    interaction.Add(choiceElem);
                }
            }
        }

        /// <summary>
        /// Created meta value for question metadata
        /// </summary>
        /// <param name="property"></param>
        /// <param name="whereToAddIt"></param>
        /// <returns></returns>
        public XElement AddValuesFromDictionary(Dictionary<string, string> property, XElement whereToAddIt)
        {

            if (property != null && whereToAddIt != null)
            {

                foreach (KeyValuePair<String, String> s in property)
                {
                    XElement value = new XElement("value");
                    value.SetAttributeValue("id", s.Key);
                    value.SetAttributeValue("name", s.Value);
                    whereToAddIt.Add(value);
                }
           } 

            return whereToAddIt;

        }
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Question()
        {
            Interaction = new QuestionInteraction();
        }
    }
}
