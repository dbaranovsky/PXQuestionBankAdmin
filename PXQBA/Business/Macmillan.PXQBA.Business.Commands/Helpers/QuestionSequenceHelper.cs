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
        public static QuestionSearchResult UpdateSequence(IList<QuestionSearchResult> questions, string questionId, int newSequenceValue)
        {
            questions.Insert(0, new QuestionSearchResult
            {
                QuestionId = "Dummy",
                SortingField = "0"
            });
            var oldSequenceValue = questions.ToList().FindIndex(q => q.QuestionId == questionId);
            var currentPosition = questions[oldSequenceValue];
            if (oldSequenceValue != newSequenceValue)
            {
                if (oldSequenceValue < newSequenceValue)
                {
                    newSequenceValue++;
                }
                if (newSequenceValue >= questions.Count())
                {
                    var last = questions.LastOrDefault();
                    if (last != null)
                    {
                        var lastValue = double.Parse(last.SortingField);
                        currentPosition.SortingField = (Math.Floor(lastValue) + 1).ToString();
                        return currentPosition;
                    }
                    currentPosition.SortingField = "1";
                }
                else
                {
                    var nextValue = double.Parse(questions[newSequenceValue].SortingField);
                    var previousValue = double.Parse(questions[newSequenceValue - 1].SortingField);
                    var decimalDigitsToRound = Math.Max(GetDecimalDigitsCount(questions[newSequenceValue].SortingField),
                        GetDecimalDigitsCount(questions[newSequenceValue - 1].SortingField));
                    var averageValueWithTolerance = Math.Abs(nextValue + previousValue) / 2 + 1 / Math.Pow(10, GetDecimalDigitsCount((Math.Abs(nextValue + previousValue) / 2).ToString()));
                    var averageValue = Math.Round(averageValueWithTolerance, decimalDigitsToRound, MidpointRounding.AwayFromZero);

                    if (averageValue == nextValue)
                    {
                        currentPosition.SortingField = (Math.Abs(nextValue + previousValue) / 2).ToString();
                    }
                    else
                    {
                        currentPosition.SortingField = averageValue.ToString();
                    }
                }
            }
            return currentPosition;
        }

        private static int GetDecimalDigitsCount(string number)
        {
            return number.Length -
                               (number.Contains(DecimalSeparator) ? number.IndexOf(DecimalSeparator) + 1 : number.Length);
        }
    }
}
