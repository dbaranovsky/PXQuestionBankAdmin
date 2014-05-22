using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.Commands.DataContracts;

namespace Macmillan.PXQBA.Business.Commands.Helpers
{
    public class QuestionSequenceHelper
    {
        private static readonly string DecimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        private static IList<QuestionSearchResult> updated = new List<QuestionSearchResult>();
        public static IList<QuestionSearchResult> UpdateSequence(IList<QuestionSearchResult> questions, string questionId, int newSequenceValue)
        {
            questions.Insert(0, new QuestionSearchResult
            {
                QuestionId = "Dummy",
                SortingField = "0"
            });
            var oldSequenceValue = questions.ToList().FindIndex(q => q.QuestionId == questionId);
            var oldPosition = questions[oldSequenceValue];
            if (oldSequenceValue != newSequenceValue)
            {
                if (oldSequenceValue < newSequenceValue)
                {
                    newSequenceValue++;
                }
                decimal seq;
                var questionsWithDecimalSequence = questions.Where(q => decimal.TryParse(q.SortingField, out seq)).ToList();
                if (newSequenceValue >= questionsWithDecimalSequence.Count())
                {
                    oldPosition.SortingField = GetNewLastValue(questionsWithDecimalSequence);
                }
                else
                {
                    var nextPosition = questionsWithDecimalSequence[newSequenceValue];
                    var previousPosition = questionsWithDecimalSequence[newSequenceValue - 1];
                    var previousValue = decimal.Parse(previousPosition.SortingField);
                    var nextValue = decimal.Parse(nextPosition.SortingField);
                    if (nextValue == previousValue)
                    {
                        nextPosition = UpdateEqualValues(previousPosition, nextPosition, questionsWithDecimalSequence, newSequenceValue, 0);
                        nextValue = decimal.Parse(nextPosition.SortingField);
                    }

                    oldPosition.SortingField = NewInsertedValue(previousValue, nextValue).ToString();
                }
                updated.Add(oldPosition);
            }
            return updated;
        }

        private static QuestionSearchResult UpdateEqualValues(QuestionSearchResult previousPosition, QuestionSearchResult nextPosition, List<QuestionSearchResult> questionsWithDecimalSequence, int newSequenceValue, int counter)
        {
            var recursionDepth = counter;
            var next = nextPosition;
            if (decimal.Parse(previousPosition.SortingField) == decimal.Parse(nextPosition.SortingField))
            {
                newSequenceValue++;
                if (newSequenceValue >= questionsWithDecimalSequence.Count())
                {
                    next.SortingField = GetNewLastValue(questionsWithDecimalSequence);
                }
                else
                {
                    next = UpdateEqualValues(nextPosition, questionsWithDecimalSequence[newSequenceValue], questionsWithDecimalSequence, newSequenceValue, recursionDepth + 1);
                }
            }
            if (recursionDepth != 0)
            {
                previousPosition.SortingField = NewInsertedValue(decimal.Parse(previousPosition.SortingField), decimal.Parse(next.SortingField)).ToString();
                updated.Add(previousPosition);
                return previousPosition;
            }
            return next;
        }

        public static string GetNewLastValue(IEnumerable<QuestionSearchResult> questionsWithDecimalSequence)
        {
            var last = questionsWithDecimalSequence.LastOrDefault();
            if (last != null)
            {
                var lastValue = decimal.Parse(last.SortingField);
                return (Math.Floor(lastValue) + 1).ToString();
            }
            return "1";
        }


        private static decimal NewInsertedValue(decimal previousValue, decimal nextValue)
        {
            var decimalDigitsToRound = Math.Max(GetDecimalDigitsCount(nextValue.ToString()),
                        GetDecimalDigitsCount(previousValue.ToString()));
            var averageValueWithTolerance = Math.Abs(nextValue + previousValue) / 2; //+ 1 / Math.Pow(10, GetDecimalDigitsCount((Math.Abs(nextValue + previousValue) / 2).ToString()));
            var averageValueRounded = Math.Round(averageValueWithTolerance, decimalDigitsToRound, MidpointRounding.AwayFromZero);

            if (averageValueRounded == nextValue)
            {
                return Math.Abs(nextValue + previousValue) / 2;
            }
            else
            {
                return averageValueRounded;
            }
        }

        private static int GetDecimalDigitsCount(string number)
        {
            return number.Length -
                               (number.Contains(DecimalSeparator) ? number.IndexOf(DecimalSeparator) + 1 : number.Length);
        }
    }
}
