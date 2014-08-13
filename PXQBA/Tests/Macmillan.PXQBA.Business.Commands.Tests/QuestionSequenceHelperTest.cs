using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Commands.DataContracts;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class QuestionSequenceHelperTest
    {
       
        private readonly IList<QuestionSearchResult> list = new List<QuestionSearchResult>()
        {
            new QuestionSearchResult(){QuestionId = "1", SortingField = "1"},
                new QuestionSearchResult(){QuestionId = "2", SortingField = "1.1"},
                new QuestionSearchResult(){QuestionId = "3", SortingField = "1.2"},
                new QuestionSearchResult(){QuestionId = "4", SortingField = "1.7"},
                new QuestionSearchResult(){QuestionId = "5", SortingField = "1.7"},
                new QuestionSearchResult(){QuestionId = "6", SortingField = "1.7"},
                new QuestionSearchResult(){QuestionId = "7", SortingField = "1.7"},
                new QuestionSearchResult(){QuestionId = "8", SortingField = "1.7"},
                new QuestionSearchResult(){QuestionId = "9", SortingField = "1.7"},
                new QuestionSearchResult(){QuestionId = "10", SortingField = "1.8"},
                new QuestionSearchResult(){QuestionId = "11", SortingField = "1.9"},
                new QuestionSearchResult(){QuestionId = "12", SortingField = "2.7"},
                new QuestionSearchResult(){QuestionId = "13", SortingField = "4.7"},
        };
        
            [TestMethod]
        public void GetNewLastValue_NoQuestionsWithDecimalSequence_Returns1()
        {
            var list = new List<QuestionSearchResult>()
            {
                new QuestionSearchResult(){SortingField = "aaa"},
                new QuestionSearchResult(){SortingField = "bb"},
                new QuestionSearchResult(){SortingField = ""},
                new QuestionSearchResult(){SortingField = ""}
            };
            var result = QuestionSequenceHelper.GetNewLastValue(list);
            Assert.IsTrue(result == "1");
        }

        [TestMethod]
        public void GetNewLastValue_QuestionsWithDecimalAndNonDecimalSequence_ReturnsNextAvailableValue()
        {
            var list = new List<QuestionSearchResult>()
            {
                new QuestionSearchResult(){SortingField = "aaa"},
                new QuestionSearchResult(){SortingField = "1.1"},
                new QuestionSearchResult(){SortingField = "2.1"},
                new QuestionSearchResult(){SortingField = "cc"}
            };
            var result = QuestionSequenceHelper.GetNewLastValue(list);
            Assert.IsTrue(result == "3");
        }


        [TestMethod]
        public void GetNewLastValue_QuestionsWithDecimalSequenceOnly_ReturnsNextAvailableValue()
        {
            var list = new List<QuestionSearchResult>()
            {
                new QuestionSearchResult(){SortingField = "1"},
                new QuestionSearchResult(){SortingField = "1.1"},
                new QuestionSearchResult(){SortingField = "1.2"},
                new QuestionSearchResult(){SortingField = "1.7"}
            };
            var result = QuestionSequenceHelper.GetNewLastValue(list);
            Assert.IsTrue(result == "2");
        }

        [TestMethod]
        public void UpdateSequence_NewSequenceIsMoreThanQuestionsCount_ReturnsNextAvailableValue()
        {
            const string questionId = "7";
            const int newSequence = 100;
            var result = QuestionSequenceHelper.UpdateSequence(list, questionId, newSequence);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].QuestionId == questionId);
        }

        [TestMethod]
        public void UpdateSequence_NewSequenceIsLessThanQuestionsCount_QuestionsWithIdenticalSequenceUpdated()
        {
            const string questionId = "2";
            const int newSequence = 5;
            var result = QuestionSequenceHelper.UpdateSequence(list, questionId, newSequence);
            Assert.IsTrue(result.Count == 5);
        }
       
    }
}
