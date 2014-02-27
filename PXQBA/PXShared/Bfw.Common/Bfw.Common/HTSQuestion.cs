using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace Bfw.HtsConversion
{

    /// <summary>
    /// Used to represent and HTS question.
    /// </summary>
    public class HTSQuestion
    {
        /// <summary>
        /// A navigation type.
        /// </summary>
        private enum NavType : int
        {
            next,
            correct,
            incorrect
        }

        /// <summary>
        /// A feedback type.
        /// </summary>
        private enum FeebbackType : int
        {
            correct,
            incorrect
        }

        /// <summary>
        /// Constructor for the class.
        /// </summary>
        /// <param name="sectionHeading">Section heading is the text that applies to all parts of the HTS question. If this is blank, a generic text "Answer the following questions" will be added.</param>
        public HTSQuestion(string sectionHeading)
        {
            if (!string.IsNullOrEmpty(sectionHeading))
            {
                this.sectionHeading = sectionHeading;
            }

            if (this.sectionHeading.Trim() == string.Empty)
            {
                this.sectionHeading = "Answer following questions";
            }

            this.xDoc.Add(new XElement("iproblem"));
            this.xDoc.Root.Add(new XAttribute("maxpoints", this.maxPoints));
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HTSQuestion"/> class.
        /// </summary>
        /// <param name="sectionHeading">The section heading.</param>
        /// <param name="questionXml">The question XML.</param>
        public HTSQuestion(string sectionHeading, string questionXml)
        {
            this.sectionHeading = "Section Heading";
            this.xDoc.Add(new XElement("iproblem"));
            this.xDoc.Root.Add(new XAttribute("maxpoints", this.maxPoints));
            this.AddSectionHeading();

            // Parse out the needed values
            XmlDocument doc = new XmlDocument();
            doc.InnerXml = questionXml;
            XmlElement root = doc.DocumentElement;
            string questionType = root.SelectSingleNode("/questions/question/interaction/@type").InnerText;
            string questionText = root.SelectSingleNode("/questions/question/body").InnerXml;
            List<ChoiceTerm> choiceAnswersMap = new List<ChoiceTerm> { };
            List<string> choices = new List<string> { };
            foreach (XmlElement v1 in root.SelectNodes("/questions/question/interaction/choice/body")) choices.Add(v1.InnerText);
            List<string> choiceAnswers = new List<string> { };
            try
            {
                foreach (XmlElement v1 in root.SelectNodes("/questions/question/interaction/choice/answer")) choiceAnswers.Add(v1.InnerText);
            }
            catch
            {
            }
            try
            {
                for (int i = 0; i < choices.Count; i++) choiceAnswersMap.Add(new ChoiceTerm(choices[i], choiceAnswers[i]));
            }
            catch
            {

            }
            int questionAnswer = 0;
            try
            {
                questionAnswer = Int32.Parse(root.SelectSingleNode("/questions/question/answer").InnerText);
            }
            catch
            {
            }
            string answer = root.SelectSingleNode("/questions/question/answer").InnerText;
            int questionScore = 1;
            try
            {
                questionScore = Int32.Parse(root.SelectSingleNode("/questions/question/@score").InnerText);
            }
            catch
            {
            }
            string answerFeedback = "", answerFeedbackWrong = "";
            try
            {
                answerFeedback = root.SelectNodes("/questions/question/interaction/choice/feedback")[questionAnswer - 1].InnerText;
            }
            catch
            {
            }
            try
            {
                answerFeedbackWrong = "The correct answer is :" + root.SelectNodes("/questions/question/interaction/choice/body")[questionAnswer - 1].InnerText;
            }
            catch
            {
            }
            // process depending on question type
            switch (questionType)
            {
                case "choice": // Multiple Choice Question
                    AddQuestionMultipleChoice(questionText, questionScore, choices, questionAnswer, answerFeedback, answerFeedbackWrong, true);
                    break;
                case "text":
                    List<string> answers = new List<string> { };
                    answers.Add(answer);
                    AddQuestionFillInTheBlanks(" ", questionScore, answers, answerFeedback, answerFeedbackWrong, true);
                    break;
                case "essay":
                    AddQuestionEssay(questionText, questionScore, answer, answerFeedback, answerFeedbackWrong, true);
                    break;
                case "match":
                    AddQuestionMatching(questionText, questionScore, choices, choiceAnswers, choiceAnswersMap, answerFeedback, answerFeedbackWrong, true);
                    break;
                case "answer":
                    List<int> choiceAnswersInt = new List<int> { };
                    try { foreach (XmlElement v1 in root.SelectNodes("/questions/question/answer/value")) choiceAnswersInt.Add(Int32.Parse(v1.InnerText)); }
                    catch { }
                    AddQuestionMultiSelect(questionText, questionScore, choices, choiceAnswersInt, answerFeedback, answerFeedbackWrong, true);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Used to store the seciton heading internally.
        /// </summary>
        private string sectionHeading = string.Empty;

        /// <summary>
        /// Used to store the mac points internally.
        /// </summary>
        private int maxPoints = 0;

        /// <summary>
        /// Used to store the document internally.
        /// </summary>
        private XDocument xDoc = new XDocument();

        /// <summary>
        /// Used to store the question count internally.
        /// </summary>
        private int questionCount = 0;

        /// <summary>
        /// Used to store whether the last question was added internally.
        /// </summary>
        private bool isLastQuestionAdded = false;

        /// <summary>
        /// Used to store the HTS version internally.
        /// </summary>
        private int htsVersion = 2;

        /// <summary>
        /// Used to get the HTS version.
        /// </summary>
        public int HtsVersion
        {
            get
            {
                return this.htsVersion = 2;
            }
            set
            {
                this.htsVersion = value;
            }
        }

        /// <summary>
        /// Returns XDocument representing Hts question.
        /// </summary>
        /// <returns></returns>
        public XDocument GetHtsQuestion()
        {

            if (!this.isLastQuestionAdded)
            {
                throw new HtsException("none of the questions was marked as last question");
            }

            this.xDoc.Root.Attribute("maxpoints").Value = this.maxPoints.ToString();
            return this.xDoc;
        }

        /// <summary>
        /// Add step0 for section heading, and a pointer to next question (step1).
        /// </summary>
        private void AddSectionHeading()
        {
            XElement stepSectionHeading = this.GetNodeForStep("step0", 0, this.sectionHeading);

            XElement inputNode = this.GetNodeForInput();
            XElement navNextNode = this.GetNodeNav(NavType.next, "step1");

            stepSectionHeading.Add(inputNode);
            stepSectionHeading.Add(navNextNode);

            this.xDoc.Root.Add(stepSectionHeading);
        }

        /// <summary>
        /// Update section heading for the question.
        /// </summary>
        /// <param name="sectionHeading"></param>
        public void UpdateSectionHeading(string sectionHeading)
        {
            this.sectionHeading = sectionHeading;

            XElement elHeading = (from c in this.xDoc.Descendants("iprostep")
                                  where c.Attribute("id").Value == "step0"
                                  select c).First();

            elHeading.Value = sectionHeading;
        }

        /// <summary>
        /// Gets XElement iprostep.
        /// </summary>
        /// <param name="stepId">ID for step.</param>
        /// <param name="points">Points for step.</param>
        /// <param name="textForStep">Text for step.</param>
        /// <returns></returns>
        private XElement GetNodeForStep(string stepId, int points, string textForStep)
        {
            XElement step = new XElement("iprostep", textForStep);
            step.Add(new XAttribute("id", stepId));
            if (points >= 0)
            {
                step.Add(new XAttribute("points", points));
            }
            return step;
        }

        /// <summary>
        /// Gets XElement input, which is need in all steps except last step or feedback steps.
        /// </summary>
        /// <returns></returns>
        private XElement GetNodeForInput()
        {
            XElement input = new XElement("input");
            input.Add(new XAttribute("class", "next"));
            input.Add(new XAttribute("id", "@step.next"));
            input.Add(new XAttribute("onclick", "javascript:next('next', '@step')"));
            input.Add(new XAttribute("type", "button"));
            input.Add(new XAttribute("value", "Continue"));
            input.Add(new XAttribute("hide", "no"));

            return input;
        }

        /// <summary>
        /// Gets the node nav.
        /// </summary>
        /// <param name="navType">Type of the nav.</param>
        /// <param name="nextStepId">The next step id.</param>
        /// <returns></returns>
        private XElement GetNodeNav(NavType navType, string nextStepId)
        {
            XElement nav = new XElement("ipronav");
            nav.Add(new XAttribute("navtype", navType.ToString()));
            nav.Add(new XAttribute("next", nextStepId));
            return nav;
        }

        /// <summary>
        /// Creates an exception of type HtsException, using error text and appends question number to exception message.
        /// </summary>
        /// <param name="errorText">The error text.</param>
        /// <returns></returns>
        private HtsException GetException(string errorText)
        {
            errorText = errorText + " -- question number : " + (this.questionCount + 1);
            HtsException ex = new HtsException(errorText);
            return ex;
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns></returns>
        private HtsException GetException(Exception ex)
        {
            string errorText = ex.Message + " -- question number : " + (this.questionCount + 1);
            HtsException htsEx = new HtsException(errorText, ex);
            return htsEx;
        }

        /// <summary>
        /// This validation applies to all question, so it is encapsulated in this function. It validates 3 thinngs:
        /// 1. Question text is not missing.
        /// 2. Points are greater than zero.
        /// 3. There is already a question which is marked as last question.
        /// If any of the validation fails, an exception of tye HtsException will be thrown.
        /// </summary>
        /// <param name="questionText">The question text.</param>
        /// <param name="points">The points.</param>
        private void GenericValidationForAllQuestions(string questionText, int points)
        {
            if (this.isLastQuestionAdded)
            {
                HtsException ex = this.GetException("cannot add anymore questions, because a question marked as last question has been added ");
                throw ex;
            }

            if (string.IsNullOrEmpty(questionText))
            {
                HtsException ex = this.GetException("questionText is missing ");
                throw ex;
            }

            if (points <= 0)
            {
                HtsException ex = this.GetException("points must be greater than zero ");
                throw ex;
            }
        }

        /// <summary>
        /// Return true, if <paramref name="text"/> can be parsed as int or double.
        /// </summary>
        /// <param name="text">Text that has to be tested.</param>
        /// <returns></returns>
        private bool IsNumeric(string text)
        {

            int i = 0;
            if (int.TryParse(text, out i))
            {
                return true;
            }

            double d = 0;
            if (double.TryParse(text, out d))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get node iproelement_short.
        /// </summary>
        /// <param name="answer">Value for attribute "answer". If this is numeric, value of attribute "type" will be "numeric" else it will be "text".</param>
        /// <param name="elId">Value of attribute elId. If there are more than 1 short elements in a step, then elid should be unique for each.</param>
        /// <returns></returns>
        private XElement GetElementShort(string answer, int elId)
        {
            // <iproelement_short id="@step.s@id" version="2" correct="1.234" type="numeric" checksyntax="off" elid="1"/><br/>
            XElement elShort = new XElement("iproelement_short");
            elShort.Add(new XAttribute("id", "@step.s@id"));
            elShort.Add(new XAttribute("version", this.htsVersion));
            elShort.Add(new XAttribute("correct", answer));

            if (this.IsNumeric(answer))
            {
                elShort.Add(new XAttribute("type", "numeric"));
            }
            else
            {
                elShort.Add(new XAttribute("type", "text"));
            }

            elShort.Add(new XAttribute("checksyntax", "off"));
            elShort.Add(new XAttribute("elid", elId));

            return elShort;
        }

        /// <summary>
        /// For each answer, creates an element of type iproelement_short and adds it to current step.
        /// </summary>
        /// <param name="answers">List of answers.</param>
        /// <param name="currentStep">Current step.</param>
        private void AddAnswers(List<string> answers, XElement currentStep)
        {
            if (currentStep == null || answers == null || answers.Count == 0)
            {
                return;
            }

            for (int i = 0; i < answers.Count; i++)
            {
                currentStep.Add("Answer " + (i + 1));
                XElement elShort = this.GetElementShort(answers[i], i + 1);
                currentStep.Add(elShort);
                currentStep.Add("<br />");
            }
        }

        /// <summary>
        /// Adds the answer.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <param name="currentStep">The current step.</param>
        private void AddAnswer(string answer, XElement currentStep)
        {
            if (currentStep == null || answer == null)
            {
                return;
            }
            XElement elShort = this.GetElementShort(answer, 1);
            currentStep.Add(elShort);
        }

        /// <summary>
        /// This code applied to all question types, so it has been encapsulated here. It does 3 things:
        /// 1. If question is not marked as last, added pointer to next step.
        /// 2. Adds the current step to xDoc.
        /// 3. If feedbacks are present adds them to question.
        /// </summary>
        /// <param name="isLastQuestion">if set to <c>true</c> [is last question].</param>
        /// <param name="currentStep">The current step.</param>
        /// <param name="feedbackCorrectAnswer">The feedback correct answer.</param>
        /// <param name="feedbackWrongAnswer">The feedback wrong answer.</param>
        private void AddNextStepAndFeedback(bool isLastQuestion, XElement currentStep, string feedbackCorrectAnswer, string feedbackWrongAnswer)
        {
            if (isLastQuestion)
            {
                this.isLastQuestionAdded = true;
            }
            else
            {
                XElement input = this.GetNodeForInput();
                currentStep.Add(input);

                string nextId = "step" + (this.questionCount + 1);
                XElement next = this.GetNodeNav(NavType.next, nextId);
                currentStep.Add(next);
            }

            this.xDoc.Root.Add(currentStep);

            this.AddFeedback(FeebbackType.correct, feedbackCorrectAnswer, currentStep);
            this.AddFeedback(FeebbackType.incorrect, feedbackWrongAnswer, currentStep);
        }

        /// <summary>
        /// This code applied to all question types, so it has been encapsulated here. It does 3 things:
        /// 1. Increments question count.
        /// 2. Adds a step for the question.
        /// 3. Updates max points by adding points of this question to it.
        /// </summary>
        /// <param name="questionText">The question text.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        private XElement AddQuestionStepAndUpdateMaxPoints(string questionText, int points)
        {
            this.questionCount++;
            string stepId = "step" + this.questionCount;
            XElement step = this.GetNodeForStep(stepId, points, questionText);
            this.maxPoints += points;

            return step;
        }

        #region Add Question Fill in the blanks

        /// <summary>
        /// Add Fill in the blank(s) question.
        /// </summary>
        /// <param name="questionText">Text for the question.</param>
        /// <param name="points">Points for the question.</param>
        /// <param name="answers">List of answers. Order of answers should match blanks, i.e. first ans should be for first blank.</param>
        /// <param name="feedbackCorrectAnswer">Feedback if the answer is correct.</param>
        /// <param name="feedbackWrongAnswer">Feedback if the answer is wrong.</param>
        /// <param name="isLastQuestion">Pass true if this is the last question. For last question, pointer to next step is not added.</param>
        public void AddQuestionFillInTheBlanks(string questionText, int points, List<string> answers, string feedbackCorrectAnswer, string feedbackWrongAnswer, bool isLastQuestion)
        {
            this.GenericValidationForAllQuestions(questionText, points);

            if (answers == null || answers.Count == 0)
            {
                HtsException ex = this.GetException("answers are missing for fill in the blanks question ");
                throw ex;
            }

            try
            {
                XElement step = this.AddQuestionStepAndUpdateMaxPoints(questionText, points);

                this.AddAnswers(answers, step);

                this.AddNextStepAndFeedback(isLastQuestion, step, feedbackCorrectAnswer, feedbackWrongAnswer);
            }
            catch (Exception ex)
            {
                HtsException htsEx = this.GetException(ex);
                throw htsEx;
            }
        }

        #endregion

        #region Add Question Essay

        /// <summary>
        /// Add Essay question.
        /// </summary>
        /// <param name="questionText">Text for the question.</param>
        /// <param name="points">Points for the question.</param>
        /// <param name="answer">The answer.</param>
        /// <param name="feedbackCorrectAnswer">Feedback if the answer is correct.</param>
        /// <param name="feedbackWrongAnswer">Feedback if the answer is wrong.</param>
        /// <param name="isLastQuestion">Pass true if this is the last question. For last question, pointer to next step is not added.</param>
        public void AddQuestionEssay(string questionText, int points, string answer, string feedbackCorrectAnswer, string feedbackWrongAnswer, bool isLastQuestion)
        {
            this.GenericValidationForAllQuestions(questionText, points);

            if (answer == null)
            {
                HtsException ex = this.GetException("answers are missing for fill in the blanks question ");
                throw ex;
            }

            try
            {
                XElement step = this.AddQuestionStepAndUpdateMaxPoints(questionText, points);
                this.AddAnswer(answer, step);
                this.AddNextStepAndFeedback(isLastQuestion, step, feedbackCorrectAnswer, feedbackWrongAnswer);
            }
            catch (Exception ex)
            {
                HtsException htsEx = this.GetException(ex);
                throw htsEx;
            }
        }

        #endregion

        #region Add Question True or False

        /// <summary>
        /// Add question True or false.
        /// </summary>
        /// <param name="questionText">Text for question.</param>
        /// <param name="points">Points for question.</param>
        /// <param name="isCorrectAnswerTrue">Pass true if correct asnwer is "true", else pass false.</param>
        /// <param name="feedbackCorrectAnswer">Feedback if the answer is correct.</param>
        /// <param name="feedbackWrongAnswer">Feedback if the answer is wrong.</param>
        /// <param name="isLastQuestion">Pass true if this is the last question. For last question, pointer to next step is not added.</param>
        public void AddQuestionTrueFalse(string questionText, int points, bool isCorrectAnswerTrue, string feedbackCorrectAnswer, string feedbackWrongAnswer, bool isLastQuestion)
        {
            this.GenericValidationForAllQuestions(questionText, points);

            XElement step = this.AddQuestionStepAndUpdateMaxPoints(questionText, points);

            List<string> choices = new List<string>() { "True", "False" };
            int indexOfCorrectChoice = 2; // assume false is correct answer
            if (isCorrectAnswerTrue)
            {
                indexOfCorrectChoice = 1;
            }

            try
            {
                this.AddMC_ChoicesUngrouped(choices, indexOfCorrectChoice, step);
                AddNextStepAndFeedback(isLastQuestion, step, feedbackCorrectAnswer, feedbackWrongAnswer);
            }
            catch (Exception ex)
            {
                HtsException htsEx = this.GetException(ex);
                throw htsEx;
            }
        }

        #endregion

        #region  Add Question Multiple Choice

        /// <summary>
        /// Add multiple choice question.
        /// </summary>
        /// <param name="questionText">Text for the question.</param>
        /// <param name="points">Points for question.</param>
        /// <param name="choices">Choices for questoin.</param>
        /// <param name="indexOfCorrectChoice">Index for correct choice, index starts from 0.</param>
        /// <param name="feedbackCorrectAnswer">Feedback if answer is correct.</param>
        /// <param name="feedbackWrongAnswer">Feedback if answer is wrong.</param>
        /// <param name="isLastQuestion">Pass true if this is the last question. For last question, pointer to next step is not added.</param>
        public void AddQuestionMultipleChoice(string questionText, int points, List<string> choices, int indexOfCorrectChoice, string feedbackCorrectAnswer, string feedbackWrongAnswer, bool isLastQuestion)
        {
            this.GenericValidationForAllQuestions(questionText, points);

            if (choices == null || choices.Count == 0)
            {
                HtsException ex = this.GetException("choices are missing for multiple choice question ");
                throw ex;
            }

            if (indexOfCorrectChoice < 1)
            {
                HtsException ex = this.GetException("index Of Correct Choice is less than 1 ");
                throw ex;
            }

            try
            {
                XElement step = this.AddQuestionStepAndUpdateMaxPoints(questionText, points);

                XElement mcChoices = this.GetMC_ChoicesGrouped(choices, indexOfCorrectChoice);
                step.Add(mcChoices);

                this.AddNextStepAndFeedback(isLastQuestion, step, feedbackCorrectAnswer, feedbackWrongAnswer);
            }
            catch (Exception ex)
            {
                HtsException htsEx = this.GetException(ex);
                throw htsEx;
            }
        }

        #endregion

        #region Add Question Multi Select

        /// <summary>
        /// Add a multi select question. In this question user is given choices and its possible to select more than 1 choice.
        /// </summary>
        /// <param name="questionText">Text for the question.</param>
        /// <param name="points">Points for the question.</param>
        /// <param name="choices">List of choices.</param>
        /// <param name="correctChoices">Index for correct choices, this should be in accordance with "choices".</param>
        /// <param name="feedbackCorrectAnswer">Feedback for correct answer.</param>
        /// <param name="feedbackWrongAnswer">Feedback for wrong answer.</param>
        /// <param name="isLastQuestion">Pass true if this is the last question. For last question, pointer to next step is not added.</param>
        public void AddQuestionMultiSelect(string questionText, int points, List<string> choices, List<int> correctChoices, string feedbackCorrectAnswer, string feedbackWrongAnswer, bool isLastQuestion)
        {
            this.GenericValidationForAllQuestions(questionText, points);

            if (choices == null || choices.Count == 0)
            {
                HtsException ex = this.GetException("choice are missing ");
                throw ex;
            }

            if (correctChoices == null || correctChoices.Count == 0)
            {
                HtsException ex = this.GetException("correct Choices are missing ");
                throw ex;
            }
            try
            {
                int numberOfChoices = choices.Count;

                // loop through all correct choices and make sure they are with the indexes of available choices
                for (int i = 0; i < correctChoices.Count; i++)
                {
                    int correctIndex = correctChoices[i];
                    if (correctIndex < 0 || correctIndex > numberOfChoices)
                    {
                        HtsException ex = this.GetException("correct Choices are not within the range of available choices ");
                        throw ex;
                    }
                }

                XElement step = this.AddQuestionStepAndUpdateMaxPoints(questionText, points);

                XElement htmlListofChoices = new XElement("ol");
                foreach (string c in choices)
                {
                    XElement li = new XElement("li", c);
                    htmlListofChoices.Add(li);
                }

                step.Add("<br />");
                step.Add(htmlListofChoices);
                step.Add("<p />");
                step.Add("Enter the <em>number(s)</em> of the <em>choice(s)</em> you wish to select. Separate multiple selections with commas or semicolons. Your selections:");

                string listOfCorrectChoices = string.Empty;
                for (int j = 0; j < correctChoices.Count; j++)
                {
                    int index = correctChoices[j];
                    listOfCorrectChoices += index.ToString() + ",";
                }

                listOfCorrectChoices = listOfCorrectChoices.Trim(',');

                XElement elShort = this.GetElementShort("1", 1);
                elShort.SetAttributeValue("correct", listOfCorrectChoices);

                step.Add(elShort);

                this.AddNextStepAndFeedback(isLastQuestion, step, feedbackCorrectAnswer, feedbackWrongAnswer);
            }
            catch (Exception ex)
            {
                HtsException htsEx = this.GetException(ex);
                throw htsEx;
            }
        }
        #endregion

        #region Add question Matching
        /// <summary>
        /// Add Matching question. In matching question user has to match choices to terms.
        /// </summary>
        /// <param name="questionText">Text for question.</param>
        /// <param name="points">Points for question.</param>
        /// <param name="choices">Choices for question.</param>
        /// <param name="terms">Terms for question.</param>
        /// <param name="choiceAndTerms">Mapping of terms with choices.</param>
        /// <param name="feedbackCorrectAnswer">Feedback when the answer is correct.</param>
        /// <param name="feedbackWrongAnswer">Feedback when the answer is wrong.</param>
        /// <param name="isLastQuestion">Pass true if this is the last question. For last question, pointer to next step is not added.</param>
        public void AddQuestionMatching(string questionText, int points, List<string> choices, List<string> terms, List<ChoiceTerm> choiceAndTerms, string feedbackCorrectAnswer, string feedbackWrongAnswer, bool isLastQuestion)
        {
            this.GenericValidationForAllQuestions(questionText, points);

            if (choices == null || choices.Count == 0)
            {
                HtsException ex = this.GetException("choice are missing ");
                throw ex;
            }

            if (terms == null || terms.Count == 0)
            {
                HtsException ex = this.GetException("terms are missing ");
                throw ex;
            }

            if (choiceAndTerms == null || choiceAndTerms.Count == 0)
            {
                HtsException ex = this.GetException("choiceAndTerms are missing ");
                throw ex;
            }

            if (choices.Count != terms.Count)
            {
                HtsException ex = this.GetException("number of choices is not equal to number of terms ");
                throw ex;
            }

            if (choices.Count != choiceAndTerms.Count)
            {
                HtsException ex = this.GetException("number of choices is not equal choiceAndTerms ");
                throw ex;
            }

            try
            {
                XElement step = this.AddQuestionStepAndUpdateMaxPoints(questionText, points);

                step.Add("<br />");
                XElement htmlListForTerms = this.GetHtmlListForTerms(terms);
                step.Add(htmlListForTerms);

                step.Add("Enter the <em>number</em> of the term that corresponds to each <em>choice</em>:");
                XElement elChoices = this.GetChoicesForMultiSelect(choices, terms, choiceAndTerms);
                step.Add(elChoices);

                AddNextStepAndFeedback(isLastQuestion, step, feedbackCorrectAnswer, feedbackWrongAnswer);
            }
            catch (Exception ex)
            {
                HtsException htsEx = this.GetException(ex);
                throw htsEx;
            }
        }
        #endregion

        /// <summary>
        /// In case of multi select question, each choice has to be represented as "iproelement_short" and all of them
        /// have to be added to an html list.
        /// </summary>
        /// <param name="choices">The choices.</param>
        /// <param name="terms">The terms.</param>
        /// <param name="choiceAndTerms">The choice and terms.</param>
        /// <returns></returns>
        private XElement GetChoicesForMultiSelect(List<string> choices, List<string> terms, List<ChoiceTerm> choiceAndTerms)
        {
            //<ol type="A">
            XElement elChoices = new XElement("ol");
            elChoices.Add(new XAttribute("type", "A"));

            for (int i = 0; i < choices.Count; i++)
            {
                string c = choices[i];
                XElement li = new XElement("li");

                int termIndex = ChoiceTerm.GetTermIndex(choiceAndTerms, c, terms);
                XElement elShort = this.GetElementShort(termIndex.ToString(), (i + 1));

                li.Add(elShort);
                li.Add(c);

                elChoices.Add(li);
            }

            return elChoices;
        }

        /// <summary>
        /// Takes a list of terms and makes html list for them.
        /// </summary>
        /// <param name="terms">List of terms.</param>
        /// <returns></returns>
        private XElement GetHtmlListForTerms(List<string> terms)
        {
            XElement elTerms = new XElement("ol");

            foreach (string t in terms)
            {
                XElement li = new XElement("li", t);
                elTerms.Add(li);
            }

            return elTerms;
        }

        /// <summary>
        /// Adds feedback to question.
        /// </summary>
        /// <param name="feedbackType">Type of feedback, correct or incorrect.</param>
        /// <param name="feedbackText">Text for feedback.</param>
        /// <param name="currentStep">Step in which feedback has to be added.</param>
        private void AddFeedback(FeebbackType feedbackType, string feedbackText, XElement currentStep)
        {
            if (string.IsNullOrEmpty(feedbackText))
            {
                return;
            }

            string feedbackId = string.Empty;
            XElement next = null;
            if (feedbackType == FeebbackType.correct)
            {
                feedbackId = "step" + this.questionCount + "corr";
                next = this.GetNodeNav(NavType.correct, feedbackId);
            }
            else
            {
                feedbackId = "step" + this.questionCount + "inc";
                next = this.GetNodeNav(NavType.incorrect, feedbackId);
            }

            currentStep.Add(next);

            XElement stepCorrect = this.GetNodeForStep(feedbackId, -1, feedbackText);
            this.xDoc.Root.Add(stepCorrect);

        }

        /// <summary>
        /// Creates and element iproelement_mc.
        /// </summary>
        /// <param name="indexOfCorrectChoice">This value is added on the attribute correct.</param>
        /// <returns></returns>
        private XElement GetElementMC(int indexOfCorrectChoice)
        {
            XElement elMC = new XElement("iproelement_mc");
            // <iproelement_mc correct="3" scramble="yes" elid="1" version="2" id="@step.s@id">
            elMC.Add(new XAttribute("correct", indexOfCorrectChoice.ToString()));
            elMC.Add(new XAttribute("scramble", "yes"));
            elMC.Add(new XAttribute("elid", "1"));
            elMC.Add(new XAttribute("version", this.htsVersion.ToString()));
            elMC.Add(new XAttribute("id", "@step.s@id"));

            return elMC;
        }

        /// <summary>
        /// Gets element iproelement_mc which has children of type ipro_mcchoice for each choice.
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="indexOfCorrectChoice"></param>
        /// <returns></returns>
        private XElement GetMC_ChoicesGrouped(List<string> choices, int indexOfCorrectChoice)
        {
            XElement choicesRoot = this.GetElementMC(indexOfCorrectChoice);

            for (int i = 0; i < choices.Count; i++)
            {
                XElement c = this.GetChoiceElementGrouped((i + 1).ToString(), choices[i]);
                choicesRoot.Add(c);
            }

            return choicesRoot;
        }

        /// <summary>
        /// Gets an element of type ipro_mcchoice to be for the case when choice are GROUPED.
        /// </summary>
        /// <param name="choiceId">ID for the choice, each choice should have a unique ID.</param>
        /// <param name="choiceText">Text for the choice.</param>
        /// <returns></returns>
        private XElement GetChoiceElementGrouped(string choiceId, string choiceText)
        {
            XElement choice = new XElement("ipro_mcchoice", choiceText);
            // <ipro_mcchoice fixed="no" choiceid="1" id="@step.mc@id" version="2">
            choice.Add(new XAttribute("fixed", "no"));
            choice.Add(new XAttribute("choiceid", choiceId));
            choice.Add(new XAttribute("id", "@step.mc@id"));
            choice.Add(new XAttribute("version", this.htsVersion.ToString()));

            return choice;
        }

        /// <summary>
        /// Gets an element of type ipro_mcchoice for the case when choice are UNgrouped.
        /// </summary>
        /// <param name="choiceId">ID for the choice, each choice should have a unique ID.</param>
        /// <param name="choiceText">Text for the choice.</param>
        /// <returns></returns>
        private XElement GetChoiceElementUngrouped(string choiceId, string choiceText)
        {
            XElement choice = new XElement("ipro_mcchoice", choiceText);
            //<ipro_mcchoice fixed="yes" mcid="1" choiceid="1" id="@step.mc@id" version="2">True</ipro_mcchoice>

            choice.Add(new XAttribute("fixed", "yes"));
            choice.Add(new XAttribute("mcid", "1"));
            choice.Add(new XAttribute("choiceid", choiceId));
            choice.Add(new XAttribute("id", "@step.mc@id"));
            choice.Add(new XAttribute("version", this.htsVersion.ToString()));

            return choice;
        }

        /// <summary>
        /// Adds ungrouped choices to current step.
        /// </summary>
        /// <param name="choices">List of choices.</param>
        /// <param name="indexOfCorrectChoice">Index of correct choice.</param>
        /// <param name="currentStep">Step to which choices should be added.</param>
        private void AddMC_ChoicesUngrouped(List<string> choices, int indexOfCorrectChoice, XElement currentStep)
        {
            XElement elMC = this.GetElementMC(indexOfCorrectChoice);

            currentStep.Add(elMC);

            for (int i = 0; i < choices.Count; i++)
            {
                XElement c = GetChoiceElementUngrouped((i + 1).ToString(), choices[i]);
                currentStep.Add(c);
            }
        }
    }

}