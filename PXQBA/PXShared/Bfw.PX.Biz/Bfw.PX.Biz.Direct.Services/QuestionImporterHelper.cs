using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.Direct.Services
{
    internal static class QuestionImporterHelper
    {
        internal class ParserExpressions
        {
            internal const string typeFillInBlank = "Type F:";
            internal const string answerBlock = "Answers:";
            internal const string lineStartFull = "\r\n";
            internal const string lineStartPartial = "\n";
            internal const string titleStart = "Title:";
            internal const string questionStart = @"^\d*[.|)][ ].*";
            internal const string questionId = @"^\d*[.|)][ ]";
            internal const string feedbackStart = @"@";
            internal const string pointsStart = "Points:";
            internal const string answerStart = @"^[*]?[a-z|A-Z]*[.|)][ ].*";
            internal const string answerId = @"^[*]?[a-z|A-Z]*[.|)][ ]";
            internal const string correctAnswer = "*";
        }

        internal class ValidationErrors
        {
            internal const string noQuestions = "No valid questions found";
            internal const string multipleTitle = "Multiple question titles found";
            internal const string questionWithoutAnswers = "Question without answers found";
            internal const string feedbackWithoutQuestion = "General feedback without question found";
            internal const string multipleFeedbackBlocks = "Multiple general feedbacks found";
            internal const string pointsDataTypeException = "Points data type exception encountered";
            internal const string multiplePointsBlocks = "Multiple points found";
            internal const string answersWithoutQuestion = "Answers without question found";
            internal const string ambiguousQuestionId = "Ambiguous question id encountered";
            internal const string ambiguousAnswerId = "Ambiguous answer id encountered";
            internal const string multipleCorrectAnswers = "Multiple correct answers found";
            internal const string incorrectAnswerBlock = "Incorrect answers encountered";
            internal const string missingCorrectAnswer = "Missing correct answer exception encountered";
            internal const string incorrectFormat = "Incorrect question format";
        }

        internal enum ElementType
        {
            QuestionTitle,
            Points,
            QuestionText,
            Feedback,
            Answer
        }
    }
}
