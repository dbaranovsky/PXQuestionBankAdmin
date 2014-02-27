using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Bfw.PX.PXPub.Models;

namespace Bfw.PX.PXPub.Controllers.Helpers
{
    /// <summary>
    /// Allows a LearningCurveActivity object to be transformed into a text format
    /// currently supported by the LearningCurve player.
    /// </summary>
    public class LearningCurveTransform
    {
        private LearningCurveActivity activity;
        private StringBuilder builder;
        private static string[] supportedQuestionFormats = { "MC", "TXT" };
        private const string DELIMITER = "-------------------------------------------------------------";

        public LearningCurveTransform(LearningCurveActivity activity)
        {
            this.activity = activity;
        }

        /// <summary>
        /// Transforms a Quiz object of subtype LearningCurveActivity into a customized text format
        /// currently supported by the LearningCurve player.
        /// </summary>
        /// <returns></returns>
        public string Execute()
        {
            if (activity == null)
            {
                throw new InvalidOperationException("Activity object is null. Transformation cannot be applied.");
            }
            builder = new StringBuilder();
            SetActivityLevel();
            HandleTopics();
            return builder.ToString();
        }

        private void SetActivityLevel()
        {
            builder.AppendFormat("ACTIVITY_TITLE: {0}\\n", activity.Title);
            PrintIfAvailable(activity.Description, s => builder.AppendFormat("ACTIVITY_DESCRIPTION: {0}\\n", s));
            PrintIfAvailable(activity.BookId, s => builder.AppendFormat("BOOK_ID: {0}\\n", s));
            PrintIfAvailable(activity.TargetScore, s => builder.AppendFormat("TOOL_INFO: {{score_target:{0}}}\\n", s));
            if (!String.IsNullOrWhiteSpace(activity.WhoopsRight) && !String.IsNullOrWhiteSpace(activity.WhoopsWrong))
            {
                string whoopsString = String.Concat(activity.WhoopsRight, "|", activity.WhoopsWrong);
                builder.AppendFormat("WHOOPS_STRINGS: {0}\\n", whoopsString);
            }
            PrintIfAvailable(activity.EbookReferenceDescription, s => builder.AppendFormat("EBOOK_REF_DESCRIPTION: {0}\\n", s));
        }

        private void HandleTopics()
        {
            var topics = activity.Topics;
            if (topics != null)
            {
                foreach (var topic in topics)
                {
                    builder.AppendFormat("\\n{0}\\n", DELIMITER);
                    builder.AppendFormat("TOPIC: {0}\\n", topic.Title);
                    // PW: specify that this is related content for a topic
                    SetRelatedContent(topic.RelatedContent, "topic");
                    builder.Append(DELIMITER);
                    HandleQuestions(topic);
                }
            }
        }

        // PW: add second parameter to say whether this is a topic or a question
        private void SetRelatedContent(RelatedContent related, string context)
        {
            if (related != null)
            {
                var items = related.RelatedContents;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        string label;
                        // PW: questions always get labeled with "_ebook" (even if the related item
                        // isn't actually an ebook section)
                        if (context == "question")
                        {
                            label = "_ebook";
                        }
                        // PW: topic items that are ebook sections get labeled "EBOOK"
                        else if (item.Description != null && item.Description.Equals("ebook", StringComparison.CurrentCultureIgnoreCase))
                        {
                            label = "EBOOK";
                        }
                        // PW: topic items that aren't ebook sections get labeled "T_ITEM"
                        else
                        {
                            label = "T_ITEM";
                        }
                        builder.AppendFormat("{0}: {1}[[{2}]]\\n", label, item.Title, item.Id);
                    }
                }
            }
        }

        private void HandleQuestions(Quiz topic)
        {
            var questions = topic.Questions.Where(q => supportedQuestionFormats.Contains(q.Type));
            if (questions != null)
            {
                foreach (var question in questions)
                {
                    string type = GetQuizType(question.Type);
                    string questionText = question.Text;
                    try
                    {

                        //questionText = HttpUtility.HtmlEncode(questionText);
                        //questionText = Uri.EscapeDataString(questionText);

                    }
                    catch
                    {

                    }

                    builder.AppendFormat("\\n{0}: {1}", type, questionText);
                    builder.Append("\\n");
                    switch (type)
                    {
                        case "MC":
                            MultipleChoiceBlock(question);
                            break;
                        case "FB":
                            FillTheBlankBlock(question);
                            break;
                    }
                    SetQuestionFeedback(question);
                    builder.Append("\\n");

                    HandleQuestionData(question, topic.Id);
                }
            }
        }

        private void SetQuestionFeedback(Question question)
        {
            var el = XElement.Parse(question.QuestionXml);
            var rawFeedback = el.Element("feedback");
            if (rawFeedback != null)
            {
                SetFeedback(rawFeedback.Value, FeedbackType.Question);
            }
        }

        private void MultipleChoiceBlock(Question question)
        {
            var choices = question.Choices;
            if (choices != null)
            {
                foreach (var choice in choices)
                {
                    if (choice.Id == question.Answer)
                    {
                        builder.Append("*");
                    }
                    builder.AppendFormat("{0}. {1}", choice.Id, choice.Text);
                    SetFeedback(choice.Feedback, FeedbackType.Choice);
                    builder.Append("\\n");
                }
            }
        }

        private void FillTheBlankBlock(Question question)
        {
            builder.AppendFormat("*{0}", question.Answer);
            SetFeedback("", FeedbackType.Other);
            builder.Append("\\n");
        }

        private void HandleQuestionData(Question question, string topicId)
        {
            var settings = question.LearningCurveQuestionSettings;
            if (settings != null)
            {
                var quizQSettings = settings.SingleOrDefault(s => s.QuizId == topicId);
                if (quizQSettings != null)
                {
                    PrintBooleanIfTrue(quizQSettings.NeverScrambleAnswers, "_neverScramble");
                    PrintIfAvailable(quizQSettings.DifficultyLevel, s => builder.AppendFormat("_level: {0}\\n", s));
                    PrintBooleanIfTrue(quizQSettings.PrimaryQuestion, "_isPrimary");
                }
            }
            PrintIfAvailable(question.Hints, s => builder.AppendFormat("_content_hint: {0}\\n", s));
            PrintIfAvailable(question.LearningCurve_Blooms, s => builder.AppendFormat("_blooms: {0}\\n", s));
            // PW: specify that this is related content for a question
            SetRelatedContent(question.RelatedContent, "question");
            builder.AppendFormat("_fq_uid: {0}\\n", question.Id);
        }

        private void SetFeedback(string feedback, FeedbackType feedbackType)
        {
            //filter out div tags
            if (!String.IsNullOrWhiteSpace(feedback))
            {
                var match = Regex.Match(feedback, @"^(?:<div>)(.*)(?:</div>)$", RegexOptions.Compiled);
                if (match.Success)
                {
                    feedback = match.Groups[1].Value;
                    if (!String.IsNullOrWhiteSpace(feedback))
                    {
                        feedback = HttpUtility.HtmlEncode(feedback);
                        if (feedbackType == FeedbackType.Question)
                        {
                            builder.AppendFormat("_content_hint:{0}\\n", feedback);
                        }
                        else
                        {
                            builder.AppendFormat(" [[{0}]]", feedback);
                        }

                    }
                }
            }
        }

        private void PrintIfAvailable(string value, Action<string> action)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                action(value);
            }
        }

        private void PrintBooleanIfTrue(bool value, string key)
        {
            if (value)
            {
                builder.AppendFormat("{0}: true\\n", key);
            }
        }

        private string GetQuizType(string fromModel)
        {
            string type = "";
            switch (fromModel)
            {
                case "MC":
                    type = "MC";
                    break;
                case "TXT":
                    type = "FB";
                    break;
            }
            return type;
        }
    }
}