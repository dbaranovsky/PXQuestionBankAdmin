using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    public class RespondusQuestionParser : QuestionParserBase
    {
        private ParsedQuestion Question { set; get; }
        private QuestionParserHelper.ElementType LastElement { set; get; }
        private int LastLine { set; get; }

        public override bool Recognize()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ParsedQuestion> Parse(string data)
        {
            var lines = data.Split(new string[] { QuestionParserHelper.ParserExpressions.lineStartFull, QuestionParserHelper.ParserExpressions.lineStartPartial }, StringSplitOptions.None);

            // parse the string
            foreach (var line in lines)
            {
                LastLine++;

                if (line.Trim().Length == 0)
                {
                    continue;
                }

                if (line.Trim() == QuestionParserHelper.ParserExpressions.typeFillInBlank)
                {
                    Type = ParsedQuestionType.Answer;
                    continue;
                }

                Question = GetLastQuestion();

                // question from title
                if (CreateQuestionFromTitle(line))
                {
                    LastElement = QuestionParserHelper.ElementType.QuestionTitle;
                    continue;
                }

                // question from question text
                if (CreateQuestionFromText(line))
                {
                    LastElement = QuestionParserHelper.ElementType.QuestionText;
                    continue;
                }

                // general feedback
                if (ParseGeneralFeedback(line))
                {
                    LastElement = QuestionParserHelper.ElementType.Feedback;
                    continue;
                }

                // points
                if (ParsePoints(line))
                {
                    LastElement = QuestionParserHelper.ElementType.Points;
                    continue;
                }

                // generic answers
                if (ParseChoice(line))
                {
                    LastElement = QuestionParserHelper.ElementType.Answer;
                    continue;
                }

                // answers block
                if (line.Trim() == QuestionParserHelper.ParserExpressions.answerBlock)
                {
                    ParseChoiceBlock(lines.SkipWhile(o => !o.Equals(line)).Skip(1).ToArray());
                    break;
                }
            }

            // additional validation 
            if (QuestionList.Count(o => !string.IsNullOrEmpty(o.Text)) == 0)
            {
                string errorLine = (from l in lines where l.Length > 0 select l).LastOrDefault();

                var exception = new ParsedQuestion()
                {
                    Id = "-1",
                    Text = errorLine ?? string.Empty,
                    ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.noQuestions)
                };

                QuestionList.Add(exception);
            }
            else
            {
                var additionalExceptions = new List<ParsedQuestion>();

                foreach (var question in GetNotNullQuestions().Where(o => !o.Id.Equals("-1")))
                {
                    if (Type == ParsedQuestionType.MultipleChoice && question.Choices.Count < 2)
                    {
                        question.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.questionWithoutAnswers);
                    }

                    question.IsParsed = true;
                    question.Type = Type;

                    if (question.Type == ParsedQuestionType.Answer && question.Choices.Count == 1)
                    {
                        question.Choices.First().IsCorrect = true;
                    }

                    var correctAnswerCount = question.Choices.Count(o => o.IsCorrect);
                    if (correctAnswerCount == 0)
                    {
                        var errorLine = question.Text;

                        var exception = new ParsedQuestion()
                        {
                            Id = "-1",
                            Text = errorLine,
                            ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.missingCorrectAnswer),
                        };

                        additionalExceptions.Add(exception);
                    }
                }

                if (additionalExceptions.Count > 0)
                {
                    QuestionList.AddRange(additionalExceptions);
                }
            }

            return QuestionList;
        }

        /// <summary>
        /// gets last (current) question in the list
        /// </summary>
        /// <returns></returns>
        private ParsedQuestion GetLastQuestion()
        {
            return QuestionList.LastOrDefault(o => !o.IsParsed);
        }

        /// <summary>
        /// validates, parses out and creates question stem out of question title
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool CreateQuestionFromTitle(string line)
        {
            bool result = false;
            bool createNew = true;

            if (line.StartsWith(QuestionParserHelper.ParserExpressions.titleStart))
            {
                if (Question != null)
                {
                    if (!string.IsNullOrEmpty(Question.Text))
                    {
                        Question.IsParsed = true;
                    }
                    else if (!string.IsNullOrEmpty(Question.Title))
                    {
                        var exception = new ParsedQuestion()
                        {
                            Id = "-1",
                            Text = line,
                            ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.multipleTitle),
                        };

                        QuestionList.Add(exception);

                        createNew = false;
                    }
                }

                if (createNew)
                {
                    Question = new ParsedQuestion()
                    {
                        Title = line.Replace(QuestionParserHelper.ParserExpressions.titleStart, string.Empty).Trim()
                    };

                    QuestionList.Add(Question);
                }

                result = true;
            }

            return result;
        }

        /// <summary>
        /// validates, parses out and creates question stem out of question text
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool CreateQuestionFromText(string line)
        {
            bool result = false;

            var questionMatch = Regex.Match(line, QuestionParserHelper.ParserExpressions.questionStart);
            if (questionMatch.Success)
            {
                var id = Regex.Match(questionMatch.Value, QuestionParserHelper.ParserExpressions.questionId).Value.Replace(")", string.Empty).Replace(".", string.Empty).Trim();
                var text = questionMatch.Value.Replace(Regex.Match(questionMatch.Value, QuestionParserHelper.ParserExpressions.questionId).Value, string.Empty).Trim();

                if (Question != null)
                {
                    if (Question.Id != null)
                    {
                        if (Question != null)
                        {
                            Question.IsParsed = true;
                        }

                        Question = new ParsedQuestion()
                        {
                            Id = id,
                            Text = text
                        };

                        CheckQuestionId();

                        QuestionList.Add(Question);
                    }
                    else
                    {
                        // question was created previously from title
                        Question.Id = id;
                        Question.Text = text;
                    }
                }
                else
                {
                    Question = new ParsedQuestion()
                    {
                        Id = id,
                        Text = text
                    };

                    CheckQuestionId();

                    QuestionList.Add(Question);
                }

                result = true;
            }

            return result;
        }

        /// <summary>
        /// validates and parses out General Feedback
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool ParseGeneralFeedback(string line)
        {
            bool result = false;

            if (line.StartsWith(QuestionParserHelper.ParserExpressions.feedbackStart))
            {
                if (Question == null)
                {
                    var exception = new ParsedQuestion()
                    {
                        Id = "-1",
                        Text = line,
                        ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.feedbackWithoutQuestion)
                    };

                    QuestionList.Add(exception);
                }
                else
                {
                    if (Question.Feedback != null)
                    {
                        Question.Text = line;
                        Question.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.multipleFeedbackBlocks);
                    }
                    else
                    {
                        Question.Feedback = line.Replace(QuestionParserHelper.ParserExpressions.feedbackStart, string.Empty).Trim();
                    }
                }

                result = true;
            }

            return result;
        }

        /// <summary>
        /// validates and parses out points
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool ParsePoints(string line)
        {
            bool result = false;

            if (line.StartsWith(QuestionParserHelper.ParserExpressions.pointsStart))
            {
                double points = 0;

                if (!Double.TryParse(line.Replace(QuestionParserHelper.ParserExpressions.pointsStart, string.Empty).Trim(), out points))
                {
                    Question.Text = line;
                    Question.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.pointsDataTypeException);
                }

                if (Question == null)
                {
                    // create new question 
                    Question = new ParsedQuestion();

                    QuestionList.Add(Question);
                }
                else
                {
                    if (Question.Points != null)
                    {
                        if (LastElement == QuestionParserHelper.ElementType.Answer)
                        {
                            // create new question
                            if (Question != null)
                            {
                                Question.IsParsed = true;
                            }

                            Question = new ParsedQuestion();

                            QuestionList.Add(Question);
                        }
                        else
                        {
                            Question.Text = line;
                            Question.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.multiplePointsBlocks);
                        }
                    }
                }

                Question.Points = points;

                result = true;
            }

            return result;
        }

        /// <summary>
        /// validates and parses our answer stem
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool ParseChoice(string line)
        {
            bool result = false;

            var answerMatch = Regex.Match(line, QuestionParserHelper.ParserExpressions.answerStart);
            if (answerMatch.Success)
            {
                var id = Regex.Match(answerMatch.Value, QuestionParserHelper.ParserExpressions.answerId).Value.Replace(")", string.Empty).Replace(".", string.Empty).Trim();
                var text = answerMatch.Value.Replace(Regex.Match(answerMatch.Value, QuestionParserHelper.ParserExpressions.answerId).Value, string.Empty).Trim();

                if (Question == null)
                {
                    var exception = new ParsedQuestion()
                    {
                        Id = "-1",
                        Text = text,
                        ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.answersWithoutQuestion),
                    };

                    QuestionList.Add(exception);
                }
                else
                {
                    ParsedQuestionChoice choice = new ParsedQuestionChoice()
                    {
                        Id = id,
                        Text = text
                    };

                    choice = ParseChoiceId(Question.Choices, choice);

                    Question.Choices.Add(choice);
                }

                result = true;
            }

            return result;
        }

        /// <summary>
        /// parses out the answer block 
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool ParseChoiceBlock(string[] lines)
        {
            bool result = false;

            foreach (var line in lines)
            {
                LastLine++;

                if (line.Trim().Length == 0)
                {
                    continue;
                }

                var data = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                if (data.Length != 2)
                {
                    var exception = new ParsedQuestion()
                    {
                        Id = "-1",
                        Text = line,
                        ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.incorrectAnswerBlock)
                    };

                    QuestionList.Add(exception);
                }
                else
                {
                    var questionId = data[0].Trim().Replace(".", string.Empty).Replace(")", string.Empty);
                    var answerId = data[1].Trim();

                    var question = GetNotNullQuestions().FirstOrDefault(o => o.Id.Equals(questionId));

                    if (question == null)
                    {
                        var exception = new ParsedQuestion()
                        {
                            Id = "-1",
                            Text = line,
                            ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.answersWithoutQuestion)
                        };

                        QuestionList.Add(exception);
                    }
                    else
                    {
                        var choice = question.Choices.SingleOrDefault(o => o.Id.Equals(answerId));

                        if (choice == null)
                        {
                            var exception = new ParsedQuestion()
                            {
                                Id = "-1",
                                Text = line,
                                ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.incorrectAnswerBlock)
                            };

                            QuestionList.Add(exception);
                        }
                        else
                        {
                            if (Type == ParsedQuestionType.MultipleChoice && question.Choices.Count(o => o.IsCorrect) > 0)
                            {
                                choice.Text = line;
                                choice.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.multipleCorrectAnswers);
                            }
                            else
                            {
                                choice.IsCorrect = true;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private IEnumerable<ParsedQuestion> GetNotNullQuestions()
        {
            return (from q in QuestionList
                    where q.Id != null
                    select q).ToList();
        }

        private string GetFormattedError(string validationError)
        {
            return string.Format("{0}|{1}", LastLine, validationError);
        }

        /// <summary>
        /// validates question id
        /// </summary>
        private void CheckQuestionId()
        {
            if (GetNotNullQuestions().Count(o => o.Id.Equals(Question.Id)) > 0)
            {
                Question.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.ambiguousQuestionId);
            }
        }

        /// <summary>
        /// validates and parses out answer id
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="choice"></param>
        /// <returns></returns>
        private ParsedQuestionChoice ParseChoiceId(IList<ParsedQuestionChoice> choices, ParsedQuestionChoice choice)
        {
            if (choices.Count(o => o.Id.Equals(choice.Id)) > 0)
            {
                choice.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.ambiguousAnswerId);
            }

            if (Type == ParsedQuestionType.Answer)
            {
                //answer.IsCorrect = true;
            }
            else if (choice.Id.StartsWith(QuestionParserHelper.ParserExpressions.correctAnswer))
            {
                choice.IsCorrect = true;
                choice.Id = choice.Id.Replace(QuestionParserHelper.ParserExpressions.correctAnswer, string.Empty);

                if (choices.Count(o => o.IsCorrect) > 0)
                {
                    choice.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.multipleCorrectAnswers);
                }
            }

            return choice;
        }

    }
}
