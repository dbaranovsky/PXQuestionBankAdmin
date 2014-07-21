using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Logging;

namespace Macmillan.PXQBA.Business.QuestionParserModule.Respondus
{
    public class RespondusQuestionParser : QuestionParserBase
    {
        private ElementType LastElement { get; set; }

   
        public override bool Recognize(string fileName)
        {
            return String.Equals(Path.GetExtension(fileName), EnumHelper.GetEnumDescription(QuestionFileType.Respondus), StringComparison.CurrentCultureIgnoreCase);
        }

        public override ValidationResult Parse(string fileName, byte[] file)
        {
            Result = new ValidationResult();
            Result.FileValidationResults.Add(new FileValidationResult
            {
                FileName = fileName
            });
            var data = System.Text.Encoding.UTF8.GetString(file);
            ParseFileData(data);
            return Result;
        }

        private void ParseFileData(string data)
        {
            var questionBlocks =
               Regex.Split(data, ElementPattern.QuestionBlock, RegexOptions.Multiline).Select(s => s.Trim('\n', '\r', '\t'))
                   .Where(s => !string.IsNullOrEmpty(s));
            foreach (var questionBlock in questionBlocks)
            {
                CurrentQuestion = new ParsedQuestion
                {
                    Type = ParsedQuestionType.Essay
                };
                var lines = questionBlock.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                ParseQuestionBlock(lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)));
            }
        }

        private void ParseQuestionBlock(IEnumerable<string> lines)
        {
            var isParsed = true;
            LastElement = ElementType.None;
            var lastLine = 0;
            foreach (var line in lines)
            {
                try
                {
                    lastLine++;
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
                    if ((LastElement == ElementType.QuestionTitle || LastElement == ElementType.MatchingChoice) &&
                        Regex.IsMatch(line, ElementPattern.MatchingChoice))
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
                catch (Exception ex)
                {
                    isParsed = false;
                    var fileResult = Result.FileValidationResults.LastOrDefault();
                    if (fileResult != null)
                    {
                        fileResult.ValidationErrors.Add(string.Format("File {0}, line {1} wasn't parse.",
                            fileResult.FileName, lastLine));
                    }
                    StaticLogger.LogError("RespondusQuestionParser.Parse ", ex);
                }
            }
            if (CurrentQuestion.Choices.Any())
            {
                CurrentQuestion.Type = CurrentQuestion.Choices.Count(c => c.IsCorrect) > 1 ? ParsedQuestionType.MultipleAnswer : ParsedQuestionType.MultipleChoice;
            }
            CurrentQuestion.IsParsed = isParsed;

            Result.FileValidationResults.Last().Questions.Add(CurrentQuestion);
        }

        private void ParseBody(string line)
        {
            if (!string.IsNullOrEmpty(CurrentQuestion.Text))
            {
                CurrentQuestion.Text += Environment.NewLine;
            }
            CurrentQuestion.Text += line;
        }

        private void ParseMatchingAnswer(string line)
        {
            if(Regex.IsMatch(line, ElementPattern.Choice))
            {
                var answerMatch = Regex.Match(line, ElementPattern.Choice);
                var id = answerMatch.Groups[2].Captures[0].Value;
                var matching = CurrentQuestion.Choices.FirstOrDefault(c => c.Answer.ToUpper() == id.ToUpper());
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
                CurrentQuestion.Choices.Add(new ParsedQuestionChoice
                {
                    Id = choiceMatch.Groups[2].Captures[0].Value,
                    Text = choiceMatch.Groups[3].Captures[0].Value,
                    Answer = choiceMatch.Groups[1].Captures[0].Value
                });
                CurrentQuestion.Type = ParsedQuestionType.Matching;
                LastElement = ElementType.MatchingChoice;

            }
        }

        private void ParseChoiceFeedback(string line)
        {
            if(Regex.IsMatch(line, ElementPattern.Choice))
            {
                var feedbackChoiceMatch = Regex.Match(line, ElementPattern.Choice);
                var id = feedbackChoiceMatch.Groups[2].Captures[0].Value;
                var choice = CurrentQuestion.Choices.FirstOrDefault(c => c.Id.ToUpper() == id.ToUpper());
                if (choice != null)
                {
                    choice.Feedback = feedbackChoiceMatch.Groups[3].Captures[0].Value;
                }
            }
        }

        private void ParseShortAnswerAnswer(string line)
        {
            var correctAnswers = Regex.Match(line, ElementPattern.Choice);
            var answer = CurrentQuestion.Choices.FirstOrDefault();
            if (answer == null)
            {
                CurrentQuestion.Choices.Add(new ParsedQuestionChoice
                {
                    Id = correctAnswers.Groups[3].Captures[0].Value,
                    Text = correctAnswers.Groups[3].Captures[0].Value,
                    IsCorrect = true
                });
            }
            CurrentQuestion.Type = ParsedQuestionType.ShortAnswer;
        }

        private void ParseEssayFeedback(string line)
        {
            if (!string.IsNullOrEmpty(CurrentQuestion.Feedback))
            {
                CurrentQuestion.Feedback += Environment.NewLine;
            }
            CurrentQuestion.Feedback += line;
            CurrentQuestion.Type = ParsedQuestionType.Essay;
        }

        private void ParseGeneralFeedback(string line)
        {
            if (!string.IsNullOrEmpty(CurrentQuestion.Feedback))
            {
                CurrentQuestion.Feedback += Environment.NewLine;
            }
            CurrentQuestion.Feedback += line;
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
                CurrentQuestion.Choices.Add(choice);
            }
        }

        private void ParseTitle(string line)
        {
            var groups = Regex.Split(line, ElementPattern.Points);
            CurrentQuestion.Title = groups[0];
            CurrentQuestion.Points = double.Parse(groups[1]);
        }
    }
}
