using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;

namespace Macmillan.PXQBA.Business.QuestionParserModule.Respondus
{
    public class RespondusQuestionParser : QuestionParserBase
    {
        public override bool Recognize(string fileName)
        {
            if (fileName.ToUpper().EndsWith(".txt"))
            {
                return true;
            }
            return false;
        }

        public override IEnumerable<ParsedQuestion> Parse(byte[] file)
        {
            var data = System.Text.Encoding.UTF8.GetString(file);

            data = @"1. Multiple choice title (3.0 points)
Multiple
choice body 

A. Choice 1 
*B. Choice 2 
C. Choice 3 
 
General Feedback:
	Multiple choice general feedback 
 
Feedback:
	a)  Choice 1 feedback 
	*b)  Choice 2 feedback 
	c)  Choice 3 feedback 


2. True and False title (1.0 point)
True and false body 

*A. True 
B. False 
 
General Feedback:
	True and false general feedback 
 
Feedback:
	*a)  True feedback 
	b)  False feedback 


3. Essay title (1.0 point)
Essay body 

Correct Answer:
Essay feedback 


4. Matching title (1.0 point)
Matching body 

	[A] 1. Left 1 
	[B] 2. Left 2 
	[C] 3. Left 3 

	A. Right 1 
	B. Right 2 
	C. Right 3 
 
General Feedback:
	Matching general feedback 


5. Fill blank title (1.0 point)
Fill blank body 

Correct Answer(s):
A. answer 1
B. answer 2
C. answer 3 
 
General Feedback:
	fill blank general feedback 


6. Multiple answer title (1.0 point)
Multiple answer body 

*A. Answer 1 
B. Answer 2 
*C. Answer 3 
D. Answer 4 
 
General Feedback:
	Multiple answer general feedback 
 
Feedback:
	*a)  Feedback answer 1 
	b)  Feedback answer 2 
	*c)  Feedback answer 3 
	d)  Feedback answer 4 

 ";

            var questionBlocks =
                Regex.Split(data, ElementPattern.QuestionBlock, RegexOptions.Multiline)
                    .Where(s => !string.IsNullOrEmpty(s));
            var questionList = new List<ParsedQuestion>();
            foreach (var questionBlock in questionBlocks)
            {
                var question = new ParsedQuestion();
                var lines = questionBlock.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
                var LastElement = ElementType.None;
                var correctAnswer = string.Empty;
                foreach (var line in lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
                {
                    if (Regex.IsMatch(line, ElementPattern.Title))
                    {
                        var groups = Regex.Split(line, ElementPattern.Points);
                        question.Title = groups[0];
                        question.Points = double.Parse(groups[1]);
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
                        LastElement = ElementType.Choice;
                        var answerMatch = Regex.Match(line, ElementPattern.Choice);
                        var choice = new ParsedQuestionChoice
                        {
                            Id = answerMatch.Groups[2].Captures[0].Value,
                            Text = answerMatch.Groups[3].Captures[0].Value,
                            IsCorrect = answerMatch.Groups[1].Length > 0
                        };
                        question.Choices.Add(choice);
                        continue;
                    }

                    if (LastElement == ElementType.GeneralFeedback)
                    {
                        if (!string.IsNullOrEmpty(question.Feedback))
                        {
                            question.Feedback += Environment.NewLine;
                        }
                        question.Feedback += line;
                        continue;
                    }
                    if (LastElement == ElementType.CorrectAnswer)
                    {
                        if (!string.IsNullOrEmpty(correctAnswer))
                        {
                            correctAnswer += Environment.NewLine;
                        }
                        correctAnswer += line;
                        continue;
                    }
                    if (LastElement == ElementType.CorrectAnswers)
                    {
                        var correctAnswers = Regex.Match(line, ElementPattern.Choice);
                        var choice = new ParsedQuestionChoice
                        {
                            Id = correctAnswers.Groups[2].Captures[0].Value,
                            Text = correctAnswers.Groups[3].Captures[0].Value,
                            IsCorrect = correctAnswers.Groups[1].Length > 0
                        };
                        question.Choices.Add(choice);
                    }
                    if (LastElement == ElementType.Feedback && Regex.IsMatch(line, ElementPattern.Choice))
                    {
                        var feedbackChoiceMatch = Regex.Match(line, ElementPattern.Choice);
                        var id = feedbackChoiceMatch.Groups[2].Captures[0].Value;
                        var choice = question.Choices.FirstOrDefault(c => c.Id.ToUpper() == id.ToUpper());
                        if (choice != null)
                        {
                            choice.Feedback = feedbackChoiceMatch.Groups[3].Captures[0].Value;
                        }
                        else
                        {
                            question.Choices.Add(new ParsedQuestionChoice
                            {
                                Id = id,
                                Feedback = feedbackChoiceMatch.Groups[3].Captures[0].Value
                            });
                        }
                        continue;
                    }
                    if (Regex.IsMatch(line, ElementPattern.MatchingChoice))
                    {
                        var feedbackChoiceMatch = Regex.Match(line, ElementPattern.MatchingChoice);
                        var id = feedbackChoiceMatch.Groups[2].Captures[0].Value;
                        //Regex.Match(answerMatch.Value, choiceId).Value.Replace(")", string.Empty).Replace(".", string.Empty).Trim();
                        var text = feedbackChoiceMatch.Groups[3].Captures[0].Value;
                        //answerMatch.Value.Replace(Regex.Match(answerMatch.Value, choiceId).Value, string.Empty).Trim();
                        var match = feedbackChoiceMatch.Groups[1].Captures[0].Value;
                        continue;
                    }
                    if (LastElement == ElementType.QuestionTitle)
                    {
                        if (!string.IsNullOrEmpty(question.Text))
                        {
                            question.Text += Environment.NewLine;
                        }
                        question.Text += line;
                        continue;
                    }
                }
                questionList.Add(question);
            }
            return questionList;
        }

        //// parse the string
            //    foreach (var line in lines)
            //    {
            //        LastLine++;

            //        if (line.Trim().Length == 0)
            //        {
            //            continue;
            //        }

            //        if (line.Trim() == QuestionParserHelper.ParserExpressions.typeFillInBlank)
            //        {
            //            Type = ParsedQuestionType.Answer;
            //            continue;
            //        }

            //        Question = GetLastQuestion();

            //        // question from title
            //        if (CreateQuestionFromTitle(line))
            //        {
            //            LastElement = QuestionParserHelper.ElementType.QuestionTitle;
            //            continue;
            //        }

            //        // question from question text
            //        if (CreateQuestionFromText(line))
            //        {
            //            LastElement = QuestionParserHelper.ElementType.QuestionText;
            //            continue;
            //        }

            //        // general feedback
            //        if (ParseGeneralFeedback(line))
            //        {
            //            LastElement = QuestionParserHelper.ElementType.Feedback;
            //            continue;
            //        }

            //        // points
            //        if (ParsePoints(line))
            //        {
            //            LastElement = QuestionParserHelper.ElementType.Points;
            //            continue;
            //        }

            //        // generic answers
            //        if (ParseChoice(line))
            //        {
            //            LastElement = QuestionParserHelper.ElementType.Answer;
            //            continue;
            //        }

            //        // answers block
            //        if (line.Trim() == QuestionParserHelper.ParserExpressions.answerBlock)
            //        {
            //            ParseChoiceBlock(lines.SkipWhile(o => !o.Equals(line)).Skip(1).ToArray());
            //            break;
            //        }
            //    }

            //    // additional validation 
            //    if (QuestionList.Count(o => !string.IsNullOrEmpty(o.Text)) == 0)
            //    {
            //        string errorLine = (from l in lines where l.Length > 0 select l).LastOrDefault();

            //        var exception = new ParsedQuestion()
            //        {
            //            Id = "-1",
            //            Text = errorLine ?? string.Empty,
            //            ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.noQuestions)
            //        };

            //        QuestionList.Add(exception);
            //    }
            //    else
            //    {
            //        var additionalExceptions = new List<ParsedQuestion>();

            //        foreach (var question in GetNotNullQuestions().Where(o => !o.Id.Equals("-1")))
            //        {
            //            if (Type == ParsedQuestionType.MultipleChoice && question.Choices.Count < 2)
            //            {
            //                question.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.questionWithoutAnswers);
            //            }

            //            question.IsParsed = true;
            //            question.Type = Type;

            //            if (question.Type == ParsedQuestionType.Answer && question.Choices.Count == 1)
            //            {
            //                question.Choices.First().IsCorrect = true;
            //            }

            //            var correctAnswerCount = question.Choices.Count(o => o.IsCorrect);
            //            if (correctAnswerCount == 0)
            //            {
            //                var errorLine = question.Text;

            //                var exception = new ParsedQuestion()
            //                {
            //                    Id = "-1",
            //                    Text = errorLine,
            //                    ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.missingCorrectAnswer),
            //                };

            //                additionalExceptions.Add(exception);
            //            }
            //        }

            //        if (additionalExceptions.Count > 0)
            //        {
            //            QuestionList.AddRange(additionalExceptions);
            //        }
            //    }

            //    return QuestionList;
            //}

            ///// <summary>
            ///// gets last (current) question in the list
            ///// </summary>
            ///// <returns></returns>
            //private ParsedQuestion GetLastQuestion()
            //{
            //    return QuestionList.LastOrDefault(o => !o.IsParsed);
            //}

            ///// <summary>
            ///// validates, parses out and creates question stem out of question title
            ///// </summary>
            ///// <param name="line"></param>
            ///// <returns></returns>
            //private bool CreateQuestionFromTitle(string line)
            //{
            //    bool result = false;
            //    bool createNew = true;

            //    if (line.StartsWith(QuestionParserHelper.ParserExpressions.titleStart))
            //    {
            //        if (Question != null)
            //        {
            //            if (!string.IsNullOrEmpty(Question.Text))
            //            {
            //                Question.IsParsed = true;
            //            }
            //            else if (!string.IsNullOrEmpty(Question.Title))
            //            {
            //                var exception = new ParsedQuestion()
            //                {
            //                    Id = "-1",
            //                    Text = line,
            //                    ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.multipleTitle),
            //                };

            //                QuestionList.Add(exception);

            //                createNew = false;
            //            }
            //        }

            //        if (createNew)
            //        {
            //            Question = new ParsedQuestion()
            //            {
            //                Title = line.Replace(QuestionParserHelper.ParserExpressions.titleStart, string.Empty).Trim()
            //            };

            //            QuestionList.Add(Question);
            //        }

            //        result = true;
            //    }

            //    return result;
            //}

            ///// <summary>
            ///// validates, parses out and creates question stem out of question text
            ///// </summary>
            ///// <param name="line"></param>
            ///// <returns></returns>
            //private bool CreateQuestionFromText(string line)
            //{
            //    bool result = false;

            //    var questionMatch = Regex.Match(line, QuestionParserHelper.ParserExpressions.questionStart);
            //    if (questionMatch.Success)
            //    {
            //        var id = Regex.Match(questionMatch.Value, QuestionParserHelper.ParserExpressions.questionId).Value.Replace(")", string.Empty).Replace(".", string.Empty).Trim();
            //        var text = questionMatch.Value.Replace(Regex.Match(questionMatch.Value, QuestionParserHelper.ParserExpressions.questionId).Value, string.Empty).Trim();

            //        if (Question != null)
            //        {
            //            if (Question.Id != null)
            //            {
            //                if (Question != null)
            //                {
            //                    Question.IsParsed = true;
            //                }

            //                Question = new ParsedQuestion()
            //                {
            //                    Id = id,
            //                    Text = text
            //                };

            //                CheckQuestionId();

            //                QuestionList.Add(Question);
            //            }
            //            else
            //            {
            //                // question was created previously from title
            //                Question.Id = id;
            //                Question.Text = text;
            //            }
            //        }
            //        else
            //        {
            //            Question = new ParsedQuestion()
            //            {
            //                Id = id,
            //                Text = text
            //            };

            //            CheckQuestionId();

            //            QuestionList.Add(Question);
            //        }

            //        result = true;
            //    }

            //    return result;
            //}

            ///// <summary>
            ///// validates and parses out General Feedback
            ///// </summary>
            ///// <param name="line"></param>
            ///// <returns></returns>
            //private bool ParseGeneralFeedback(string line)
            //{
            //    bool result = false;

            //    if (line.StartsWith(QuestionParserHelper.ParserExpressions.feedbackStart))
            //    {
            //        if (Question == null)
            //        {
            //            var exception = new ParsedQuestion()
            //            {
            //                Id = "-1",
            //                Text = line,
            //                ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.feedbackWithoutQuestion)
            //            };

            //            QuestionList.Add(exception);
            //        }
            //        else
            //        {
            //            if (Question.Feedback != null)
            //            {
            //                Question.Text = line;
            //                Question.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.multipleFeedbackBlocks);
            //            }
            //            else
            //            {
            //                Question.Feedback = line.Replace(QuestionParserHelper.ParserExpressions.feedbackStart, string.Empty).Trim();
            //            }
            //        }

            //        result = true;
            //    }

            //    return result;
            //}

            ///// <summary>
            ///// validates and parses out points
            ///// </summary>
            ///// <param name="line"></param>
            ///// <returns></returns>
            //private bool ParsePoints(string line)
            //{
            //    bool result = false;

            //    if (line.StartsWith(QuestionParserHelper.ParserExpressions.pointsStart))
            //    {
            //        double points = 0;

            //        if (!Double.TryParse(line.Replace(QuestionParserHelper.ParserExpressions.pointsStart, string.Empty).Trim(), out points))
            //        {
            //            Question.Text = line;
            //            Question.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.pointsDataTypeException);
            //        }

            //        if (Question == null)
            //        {
            //            // create new question 
            //            Question = new ParsedQuestion();

            //            QuestionList.Add(Question);
            //        }
            //        else
            //        {
            //            if (Question.Points != null)
            //            {
            //                if (LastElement == QuestionParserHelper.ElementType.Answer)
            //                {
            //                    // create new question
            //                    if (Question != null)
            //                    {
            //                        Question.IsParsed = true;
            //                    }

            //                    Question = new ParsedQuestion();

            //                    QuestionList.Add(Question);
            //                }
            //                else
            //                {
            //                    Question.Text = line;
            //                    Question.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.multiplePointsBlocks);
            //                }
            //            }
            //        }

            //        Question.Points = points;

            //        result = true;
            //    }

            //    return result;
            //}

            ///// <summary>
            ///// validates and parses our answer stem
            ///// </summary>
            ///// <param name="line"></param>
            ///// <returns></returns>
            //private bool ParseChoice(string line)
            //{
            //    bool result = false;

            //    var answerMatch = Regex.Match(line, QuestionParserHelper.ParserExpressions.answerStart);
            //    if (answerMatch.Success)
            //    {
            //        var id = Regex.Match(answerMatch.Value, QuestionParserHelper.ParserExpressions.answerId).Value.Replace(")", string.Empty).Replace(".", string.Empty).Trim();
            //        var text = answerMatch.Value.Replace(Regex.Match(answerMatch.Value, QuestionParserHelper.ParserExpressions.answerId).Value, string.Empty).Trim();

            //        if (Question == null)
            //        {
            //            var exception = new ParsedQuestion()
            //            {
            //                Id = "-1",
            //                Text = text,
            //                ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.answersWithoutQuestion),
            //            };

            //            QuestionList.Add(exception);
            //        }
            //        else
            //        {
            //            ParsedQuestionChoice choice = new ParsedQuestionChoice()
            //            {
            //                Id = id,
            //                Text = text
            //            };

            //            choice = ParseChoiceId(Question.Choices, choice);

            //            Question.Choices.Add(choice);
            //        }

            //        result = true;
            //    }

            //    return result;
            //}

            ///// <summary>
            ///// parses out the answer block 
            ///// </summary>
            ///// <param name="lines"></param>
            ///// <returns></returns>
            //private bool ParseChoiceBlock(string[] lines)
            //{
            //    bool result = false;

            //    foreach (var line in lines)
            //    {
            //        LastLine++;

            //        if (line.Trim().Length == 0)
            //        {
            //            continue;
            //        }

            //        var data = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            //        if (data.Length != 2)
            //        {
            //            var exception = new ParsedQuestion()
            //            {
            //                Id = "-1",
            //                Text = line,
            //                ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.incorrectAnswerBlock)
            //            };

            //            QuestionList.Add(exception);
            //        }
            //        else
            //        {
            //            var questionId = data[0].Trim().Replace(".", string.Empty).Replace(")", string.Empty);
            //            var answerId = data[1].Trim();

            //            var question = GetNotNullQuestions().FirstOrDefault(o => o.Id.Equals(questionId));

            //            if (question == null)
            //            {
            //                var exception = new ParsedQuestion()
            //                {
            //                    Id = "-1",
            //                    Text = line,
            //                    ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.answersWithoutQuestion)
            //                };

            //                QuestionList.Add(exception);
            //            }
            //            else
            //            {
            //                var choice = question.Choices.SingleOrDefault(o => o.Id.Equals(answerId));

            //                if (choice == null)
            //                {
            //                    var exception = new ParsedQuestion()
            //                    {
            //                        Id = "-1",
            //                        Text = line,
            //                        ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.incorrectAnswerBlock)
            //                    };

            //                    QuestionList.Add(exception);
            //                }
            //                else
            //                {
            //                    if (Type == ParsedQuestionType.MultipleChoice && question.Choices.Count(o => o.IsCorrect) > 0)
            //                    {
            //                        choice.Text = line;
            //                        choice.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.multipleCorrectAnswers);
            //                    }
            //                    else
            //                    {
            //                        choice.IsCorrect = true;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    return result;
            //}

            //private IEnumerable<ParsedQuestion> GetNotNullQuestions()
            //{
            //    return (from q in QuestionList
            //            where q.Id != null
            //            select q).ToList();
            //}

            //private string GetFormattedError(string validationError)
            //{
            //    return string.Format("{0}|{1}", LastLine, validationError);
            //}

            ///// <summary>
            ///// validates question id
            ///// </summary>
            //private void CheckQuestionId()
            //{
            //    if (GetNotNullQuestions().Count(o => o.Id.Equals(Question.Id)) > 0)
            //    {
            //        Question.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.ambiguousQuestionId);
            //    }
            //}

            ///// <summary>
            ///// validates and parses out answer id
            ///// </summary>
            ///// <param name="choices"></param>
            ///// <param name="choice"></param>
            ///// <returns></returns>
            //private ParsedQuestionChoice ParseChoiceId(IList<ParsedQuestionChoice> choices, ParsedQuestionChoice choice)
            //{
            //    if (choices.Count(o => o.Id.Equals(choice.Id)) > 0)
            //    {
            //        choice.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.ambiguousAnswerId);
            //    }

            //    if (Type == ParsedQuestionType.Answer)
            //    {
            //        //answer.IsCorrect = true;
            //    }
            //    else if (choice.Id.StartsWith(QuestionParserHelper.ParserExpressions.correctAnswer))
            //    {
            //        choice.IsCorrect = true;
            //        choice.Id = choice.Id.Replace(QuestionParserHelper.ParserExpressions.correctAnswer, string.Empty);

            //        if (choices.Count(o => o.IsCorrect) > 0)
            //        {
            //            choice.ValidationError = GetFormattedError(QuestionParserHelper.ValidationErrors.multipleCorrectAnswers);
            //        }
            //    }

            //    return choice;
            //}
    }
}
