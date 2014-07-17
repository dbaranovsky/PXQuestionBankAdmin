using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;

namespace Macmillan.PXQBA.Business.QuestionParserModule.Respondus
{
    public class RespondusQuestionParser : QuestionParserBase
    {
        private ParsedQuestion Question { get; set; }

        private ElementType LastElement { get; set; }
        public override bool Recognize(string fileName)
        {
            if (fileName.ToUpper().EndsWith(".TXT"))
            {
                return true;
            }
            return false;
        }

        public override IEnumerable<ParsedQuestion> Parse(byte[] file)
        {
            var data = System.Text.Encoding.UTF8.GetString(file);

            var questionBlocks =
                Regex.Split(data, ElementPattern.QuestionBlock, RegexOptions.Multiline)
                    .Where(s => !string.IsNullOrEmpty(s));
            var questionList = new List<ParsedQuestion>();
            foreach (var questionBlock in questionBlocks)
            {
                Question = new ParsedQuestion
                {
                    Type = ParsedQuestionType.MultipleChoice
                };
                var lines = questionBlock.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
                LastElement = ElementType.None;
                foreach (var line in lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
                {
                    if (Regex.IsMatch(line, ElementPattern.Title))
                    {
                        ParseTitle(line);
                        LastElement = ElementType.QuestionTitle;
                        continue;
                    }
                    if (Regex.IsMatch(line, ElementPattern.GeneralFeedback))
                    {
                        LastElement = ElementType.GeneralFeedback;
                        continue;
                    }
                    if (Regex.IsMatch(line, ElementPattern.Feedback))
                    {
                        LastElement = ElementType.Feedback;
                        continue;
                    }
                    if (Regex.IsMatch(line, ElementPattern.CorrectAnswer))
                    {
                        LastElement = ElementType.CorrectAnswer;
                        continue;
                    }
                    if (Regex.IsMatch(line, ElementPattern.CorrectAnswers))
                    {
                        LastElement = ElementType.CorrectAnswers;
                        continue;
                    }
                    if (LastElement == ElementType.QuestionTitle && Regex.IsMatch(line, ElementPattern.Choice))
                    {
                        ParseChoices(line);
                        continue;
                    }
                    if (LastElement == ElementType.QuestionTitle && Regex.IsMatch(line, ElementPattern.MatchingChoice))
                    {
                        ParseMatchingChoice(line);
                        continue;
                    }

                    if (LastElement == ElementType.GeneralFeedback)
                    {
                        ParseGeneralFeedback(line);
                        continue;
                    }
                    if (LastElement == ElementType.CorrectAnswer)
                    {
                        ParseEssayFeedback(line);
                        continue;
                    }
                    if (LastElement == ElementType.CorrectAnswers)
                    {
                        ParseShortAnswerAnswer(line);
                    }
                    if (LastElement == ElementType.Feedback)
                    {
                        ParseChoiceFeedback(line);
                        continue;
                    }
                    if (LastElement == ElementType.MatchingChoice)
                    {
                        ParseMatchingAnswer(line);
                        continue;
                    }
                    if (LastElement == ElementType.QuestionTitle)
                    {
                        ParseBody(line);
                        continue;
                    }
                }
                if (Question.Choices.Any() && Question.Choices.Count(c => c.IsCorrect) > 1)
                {
                    Question.Type=ParsedQuestionType.MultipleAnswer;
                }
                questionList.Add(Question);
            }
            return questionList;
        }

        private void ParseBody(string line)
        {
            if (!string.IsNullOrEmpty(Question.Text))
            {
                Question.Text += Environment.NewLine;
            }
            Question.Text += line;
        }

        private void ParseMatchingAnswer(string line)
        {
            if(Regex.IsMatch(line, ElementPattern.Choice))
            {
                var answerMatch = Regex.Match(line, ElementPattern.Choice);
                var id = answerMatch.Groups[2].Captures[0].Value;
                var matching = Question.Choices.FirstOrDefault(c => c.Answer.ToUpper() == id.ToUpper());
                if (matching != null)
                {
                    matching.Answer = answerMatch.Groups[3].Captures[0].Value;
                }
            }
        }

        private void ParseMatchingChoice(string line)
        {
            if (Regex.IsMatch(line, ElementPattern.MatchingChoice))
            {
                var choiceMatch = Regex.Match(line, ElementPattern.MatchingChoice);
                Question.Choices.Add(new ParsedQuestionChoice
                {
                    Id = choiceMatch.Groups[2].Captures[0].Value,
                    Text = choiceMatch.Groups[3].Captures[0].Value,
                    Answer = choiceMatch.Groups[1].Captures[0].Value
                });
                Question.Type = ParsedQuestionType.Matching;
                LastElement = ElementType.MatchingChoice;

            }
        }

        private void ParseChoiceFeedback(string line)
        {
            if(Regex.IsMatch(line, ElementPattern.Choice))
            {
                var feedbackChoiceMatch = Regex.Match(line, ElementPattern.Choice);
                var id = feedbackChoiceMatch.Groups[2].Captures[0].Value;
                var choice = Question.Choices.FirstOrDefault(c => c.Id.ToUpper() == id.ToUpper());
                if (choice != null)
                {
                    choice.Feedback = feedbackChoiceMatch.Groups[3].Captures[0].Value;
                }
            }
        }

        private void ParseShortAnswerAnswer(string line)
        {
            var correctAnswers = Regex.Match(line, ElementPattern.Choice);
            var answer = Question.Choices.FirstOrDefault();
            if (answer == null)
            {
                Question.Choices.Add(new ParsedQuestionChoice
                {
                    Id = correctAnswers.Groups[3].Captures[0].Value,
                    Text = correctAnswers.Groups[3].Captures[0].Value,
                    IsCorrect = true
                });
            }
            Question.Type = ParsedQuestionType.ShortAnswer;
        }

        private void ParseEssayFeedback(string line)
        {
            if (!string.IsNullOrEmpty(Question.Feedback))
            {
                Question.Feedback += Environment.NewLine;
            }
            Question.Feedback += line;
            Question.Type = ParsedQuestionType.Essay;
        }

        private void ParseGeneralFeedback(string line)
        {
            if (!string.IsNullOrEmpty(Question.Feedback))
            {
                Question.Feedback += Environment.NewLine;
            }
            Question.Feedback += line;
        }

        private void ParseChoices(string line)
        {
            if (Regex.IsMatch(line, ElementPattern.Choice))
            {
                var answerMatch = Regex.Match(line, ElementPattern.Choice);
                var choice = new ParsedQuestionChoice
                {
                    Id = answerMatch.Groups[2].Captures[0].Value,
                    Text = answerMatch.Groups[3].Captures[0].Value,
                    IsCorrect = answerMatch.Groups[1].Length > 0
                };
                Question.Choices.Add(choice);
            }
        }

        private void ParseTitle(string line)
        {
            var groups = Regex.Split(line, ElementPattern.Points);
            Question.Title = groups[0];
            Question.Points = double.Parse(groups[1]);
        }
    }
}
