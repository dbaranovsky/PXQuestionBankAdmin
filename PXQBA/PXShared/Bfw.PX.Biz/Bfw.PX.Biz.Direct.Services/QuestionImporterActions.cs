using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.DataContracts;
using System.Text.RegularExpressions;
using Bfw.PX.Biz.Services.Mappers;

namespace Bfw.PX.Biz.Direct.Services
{
    public class QuestionImporterActions : IQuestionImporterActions
    {
        private List<RespondusQuestion> QuestionList { get; set; }
        private RespondusQuestion Question { set; get; }
        private QuestionImporterHelper.ElementType LastElement { set; get; }
        private int LastLine { set; get; }
        private RespondusType Type { get; set; }

        public QuestionImporterActions()
        {
            // default it to multiple choice questions
            Type = RespondusType.MultipleChoice;

            QuestionList = new List<RespondusQuestion>();
        }

        /// <summary>
        /// Adds questions to the course and to the current quiz
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="data"></param>
        /// <param name="quizId"></param>
        /// <param name="contentActions"></param>
        public List<RespondusQuestion> Import(string entityId, List<RespondusQuestion> questions, string quizId, IQuestionActions questionActions)
        {
            var bizQuestions = GetBizQuestions(questions, entityId);

            // add questions the course
            try
            {
                questionActions.StoreQuestions(bizQuestions);
            }
            catch (Exception ex)
            {
                // create new question with dlap exception
                questions.Clear();
                questions.Add(new RespondusQuestion()
                    {
                         Id = "-1",
                         ValidationError = GetFormattedError(ex.Message)
                    });

                return questions;
            }

            // add questions to the quiz
            questionActions.AppendQuestionList(entityId, quizId, bizQuestions);

            return questions;
        }

        /// <summary>
        /// Validates parsed questions against DLAP for format 
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="questions"></param>
        /// <param name="contentActions"></param>
        /// <returns></returns>
        public List<RespondusQuestion> ValidateThruDLAP(string entityId, List<RespondusQuestion> questions, IQuestionActions questionActions)
        {
            List<RespondusQuestion> result = new List<RespondusQuestion>();

            var bizQuestions = GetBizQuestions(questions, entityId);
            var questionIds = from q in bizQuestions select q.Id;

            // add questions
            try
            {
                questionActions.StoreQuestions(bizQuestions);
            }
            catch (Exception ex)
            {
                result.Add(new RespondusQuestion()
                    {
                        Id = "-1",
                        ValidationError = GetFormattedError(ex.Message)
                    });

                return result;
            }

            // check if all questions been added
            var dlapQuestions = questionActions.GetQuestions(entityId, questionIds);

            foreach (var bizQuestion in bizQuestions)
            {
                var match = dlapQuestions.SingleOrDefault(q => q.Id.Equals(bizQuestion.Id));

                if (match == null)
                {
                    var errorQuestion = questions.Find(q => q.Equals(bizQuestion.Id));
                    errorQuestion.ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.incorrectFormat);

                    result.Add(errorQuestion);
                }
            }

            // delete questions
            questionActions.DeleteQuestions(entityId, questionIds);

            return result;
        }

        /// <summary>
        /// Parses out string into multiple choice question list
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>        
        public List<RespondusQuestion> Parse(string data)
        {
            var lines = data.Split(new string[] { QuestionImporterHelper.ParserExpressions.lineStartFull, QuestionImporterHelper.ParserExpressions.lineStartPartial }, StringSplitOptions.None);

            // parse the string
            foreach (var line in lines)
            {
                LastLine++;

                if (line.Trim().Length == 0)
                {
                    continue;
                }

                if (line.Trim() == QuestionImporterHelper.ParserExpressions.typeFillInBlank)
                {
                    Type = RespondusType.FillInBlank;
                    continue;
                }

                Question = GetLastQuestion();

                // question from title
                if (CreateQuestionFromTitle(line))
                {
                    LastElement = QuestionImporterHelper.ElementType.QuestionTitle;
                    continue;
                }

                // question from question text
                if (CreateQuestionFromText(line))
                {
                    LastElement = QuestionImporterHelper.ElementType.QuestionText;
                    continue;
                }

                // general feedback
                if (ParseGeneralFeedback(line))
                {
                    LastElement = QuestionImporterHelper.ElementType.Feedback;
                    continue;
                }

                // points
                if (ParsePoints(line))
                {
                    LastElement = QuestionImporterHelper.ElementType.Points;
                    continue;
                }

                // generic answers
                if (ParseChoice(line))
                {
                    LastElement = QuestionImporterHelper.ElementType.Answer;
                    continue;
                }

                // answers block
                if (line.Trim() == QuestionImporterHelper.ParserExpressions.answerBlock)
                {
                    ParseChoiceBlock(lines.SkipWhile(o => !o.Equals(line)).Skip(1).ToArray());
                    break;
                }
            }

            // additional validation 
            if (QuestionList.Count(o => !string.IsNullOrEmpty(o.Text)) == 0)
            {
                string errorLine = (from l in lines where l.Length > 0 select l).LastOrDefault();

                var exception = new RespondusQuestion()
                {
                    Id = "-1",
                    Text = errorLine ?? string.Empty,
                    ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.noQuestions)
                };

                QuestionList.Add(exception);
            }
            else
            {
                var additionalExceptions = new List<RespondusQuestion>();

                foreach (var question in GetNotNullQuestions().Where(o => !o.Id.Equals("-1")))
                {
                    if (Type == RespondusType.MultipleChoice && question.Choices.Count < 2)
                    {
                        question.ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.questionWithoutAnswers);
                    }

                    question.IsParsed = true;
                    question.Type = Type;

                    if (question.Type == RespondusType.FillInBlank && question.Choices.Count == 1)
                    {
                        question.Choices.First().IsCorrect = true;
                    }

                    var correctAnswerCount = question.Choices.Count(o => o.IsCorrect);
                    if (correctAnswerCount == 0)
                    {
                        var errorLine = question.Text;

                        var exception = new RespondusQuestion()
                        {
                            Id = "-1",
                            Text = errorLine,
                            ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.missingCorrectAnswer),
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
        private RespondusQuestion GetLastQuestion()
        {
            return QuestionList.Where(o => !o.IsParsed).LastOrDefault();
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

            if (line.StartsWith(QuestionImporterHelper.ParserExpressions.titleStart))
            {
                if (Question != null)
                {
                    if (!string.IsNullOrEmpty(Question.Text))
                    {
                        Question.IsParsed = true;
                    }
                    else if (!string.IsNullOrEmpty(Question.Title))
                    {
                        var exception = new RespondusQuestion()
                        {
                            Id = "-1",
                            Text = line,
                            ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.multipleTitle),
                        };

                        QuestionList.Add(exception);

                        createNew = false;
                    }
                }

                if (createNew)
                {
                    Question = new RespondusQuestion()
                    {
                        Title = line.Replace(QuestionImporterHelper.ParserExpressions.titleStart, string.Empty).Trim()
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

            var questionMatch = Regex.Match(line, QuestionImporterHelper.ParserExpressions.questionStart);
            if (questionMatch.Success)
            {
                var id = Regex.Match(questionMatch.Value, QuestionImporterHelper.ParserExpressions.questionId).Value.Replace(")", string.Empty).Replace(".", string.Empty).Trim();
                var text = questionMatch.Value.Replace(Regex.Match(questionMatch.Value, QuestionImporterHelper.ParserExpressions.questionId).Value, string.Empty).Trim();

                if (Question != null)
                {
                    if (Question.Id != null)
                    {
                        if (Question != null)
                        {
                            Question.IsParsed = true;
                        }

                        Question = new RespondusQuestion()
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
                    Question = new RespondusQuestion()
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

            if (line.StartsWith(QuestionImporterHelper.ParserExpressions.feedbackStart))
            {
                if (Question == null)
                {
                    var exception = new RespondusQuestion()
                    {
                        Id = "-1",
                        Text = line,
                        ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.feedbackWithoutQuestion)
                    };

                    QuestionList.Add(exception);
                }
                else
                {
                    if (Question.Feedback != null)
                    {
                        Question.Text = line;
                        Question.ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.multipleFeedbackBlocks);
                    }
                    else
                    {
                        Question.Feedback = line.Replace(QuestionImporterHelper.ParserExpressions.feedbackStart, string.Empty).Trim();
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

            if (line.StartsWith(QuestionImporterHelper.ParserExpressions.pointsStart))
            {
                double points = 0;

                if (!Double.TryParse(line.Replace(QuestionImporterHelper.ParserExpressions.pointsStart, string.Empty).Trim(), out points))
                {
                    Question.Text = line;
                    Question.ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.pointsDataTypeException);
                }

                if (Question == null)
                {
                    // create new question 
                    Question = new RespondusQuestion();

                    QuestionList.Add(Question);
                }
                else
                {
                    if (Question.Points != null)
                    {
                        if (LastElement == QuestionImporterHelper.ElementType.Answer)
                        {
                            // create new question
                            if (Question != null)
                            {
                                Question.IsParsed = true;
                            }

                            Question = new RespondusQuestion();

                            QuestionList.Add(Question);
                        }
                        else
                        {
                            Question.Text = line;
                            Question.ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.multiplePointsBlocks);
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

            var answerMatch = Regex.Match(line, QuestionImporterHelper.ParserExpressions.answerStart);
            if (answerMatch.Success)
            {
                var id = Regex.Match(answerMatch.Value, QuestionImporterHelper.ParserExpressions.answerId).Value.Replace(")", string.Empty).Replace(".", string.Empty).Trim();
                var text = answerMatch.Value.Replace(Regex.Match(answerMatch.Value, QuestionImporterHelper.ParserExpressions.answerId).Value, string.Empty).Trim();

                if (Question == null)
                {
                    var exception = new RespondusQuestion()
                    {
                        Id = "-1",
                        Text = text,
                        ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.answersWithoutQuestion),
                    };

                    QuestionList.Add(exception);
                }
                else
                {
                    RespondusQuestionChoice choice = new RespondusQuestionChoice()
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
                    var exception = new RespondusQuestion()
                    {
                        Id = "-1",
                        Text = line,
                        ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.incorrectAnswerBlock)
                    };

                    QuestionList.Add(exception);
                }
                else
                {
                    var questionId = data[0].Trim().Replace(".", string.Empty).Replace(")", string.Empty);
                    var answerId = data[1].Trim();

                    var question = GetNotNullQuestions().Where(o => o.Id.Equals(questionId)).FirstOrDefault();

                    if (question == null)
                    {
                        var exception = new RespondusQuestion()
                        {
                            Id = "-1",
                            Text = line,
                            ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.answersWithoutQuestion)
                        };

                        QuestionList.Add(exception);
                    }
                    else
                    {
                        var choice = question.Choices.Where(o => o.Id.Equals(answerId)).SingleOrDefault();

                        if (choice == null)
                        {
                            var exception = new RespondusQuestion()
                            {
                                Id = "-1",
                                Text = line,
                                ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.incorrectAnswerBlock)
                            };

                            QuestionList.Add(exception);
                        }
                        else
                        {
                            if (Type == RespondusType.MultipleChoice && question.Choices.Count(o => o.IsCorrect) > 0)
                            {
                                choice.Text = line;
                                choice.ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.multipleCorrectAnswers);
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


        /// <summary>
        /// validates question id
        /// </summary>
        private void CheckQuestionId()
        {
            if (GetNotNullQuestions().Count(o => o.Id.Equals(Question.Id)) > 0)
            {
                Question.ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.ambiguousQuestionId);
            }
        }

        /// <summary>
        /// validates and parses out answer id
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="choice"></param>
        /// <returns></returns>
        private RespondusQuestionChoice ParseChoiceId(List<RespondusQuestionChoice> choices, RespondusQuestionChoice choice)
        {
            if (choices.Count(o => o.Id.Equals(choice.Id)) > 0)
            {
                choice.ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.ambiguousAnswerId);
            }

            if (Type == RespondusType.FillInBlank)
            {
                //answer.IsCorrect = true;
            }
            else if (choice.Id.StartsWith(QuestionImporterHelper.ParserExpressions.correctAnswer))
            {
                choice.IsCorrect = true;
                choice.Id = choice.Id.Replace(QuestionImporterHelper.ParserExpressions.correctAnswer, string.Empty);

                if (choices.Count(o => o.IsCorrect) > 0)
                {
                    choice.ValidationError = GetFormattedError(QuestionImporterHelper.ValidationErrors.multipleCorrectAnswers);
                }
            }

            return choice;
        }

        private List<RespondusQuestion> GetNotNullQuestions() 
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
        /// Converts Parsed questions into biz questions
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        private List<Question> GetBizQuestions(List<RespondusQuestion> questions, string entityId)
        {
            var bizQuestions = (from q in questions
                                select q.ToQuestion()).ToList();

            bizQuestions.ForEach(delegate(Question q)
            {
                q.EntityId = entityId;
                q.Id = Guid.NewGuid().ToString();
            });

            return bizQuestions;
        }
    }
}
