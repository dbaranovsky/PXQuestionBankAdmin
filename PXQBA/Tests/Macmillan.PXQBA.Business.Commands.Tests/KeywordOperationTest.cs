using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.SQLOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class KeywordOperationTest
    {
        private IKeywordOperation keywordOperation;
        private IDatabaseManager databaseManager;

        [TestInitialize]
        public void TestInitialize()
        {
            databaseManager = Substitute.For<IDatabaseManager>();
            keywordOperation = new KeywordOperation(databaseManager);
        }

        [TestMethod]
        public void GetKeywordList_AnyCorrectParams_ListWithTwoKeywords()
        {
            var databaseRecord = new DatabaseRecord();
            databaseRecord["Keyword"] = "test";

            databaseManager.Query(Arg.Do<DbCommand>(x => Assert.IsTrue(x.CommandText == "dbo.GetKeywordList")))
                           .Returns(new List<DatabaseRecord>() { databaseRecord, databaseRecord });

            var keywords = keywordOperation.GetKeywordList("12", "keyword");
            Assert.AreEqual(keywords.Count(), 2);
            Assert.AreEqual(keywords.Count(x=> x == "test"), 2);
          
        }


        [TestMethod]
        public void GetKeywordList_AnyCorrectParams_NoKeywords()
        {
            var databaseRecord = new DatabaseRecord();
            databaseRecord["Keyword"] = "test";

            databaseManager.Query(Arg.Do<DbCommand>(x => Assert.IsTrue(x.CommandText == "dbo.GetKeywordList")))
                           .Returns(new List<DatabaseRecord>() );

            var keywords = keywordOperation.GetKeywordList("12", "keyword");
            Assert.IsFalse(keywords.Any());

        }

        [TestMethod]
        public void GetKeywordList_AnyCorrectParams_ListWithOneKeyword()
        {
            var databaseRecord = new DatabaseRecord();
            databaseRecord["Keyword"] = "test";

            databaseManager.Query(Arg.Do<DbCommand>(x => Assert.IsTrue(x.CommandText == "dbo.GetKeywordList")))
                           .Returns(new List<DatabaseRecord>() { databaseRecord });

            var keywords = keywordOperation.GetKeywordList("12", "keyword");
            Assert.AreEqual(keywords.Count(), 1);
            Assert.AreEqual(keywords.Count(x => x == "test"), 1);

        }

        [TestMethod]
        public void AddKeyWords_KeyWordsToAdd_CorrectCommand()
        {
            var counter = 0;
            databaseManager.ExecuteNonQuery(Arg.Do<DbCommand>(x =>
                                                              {
                                                                  Assert.IsTrue(x.CommandText == "dbo.AddKeyword");
                                                                  Assert.IsTrue((x.Parameters["@keyword"].Value.ToString() == "1" && counter == 0) || (x.Parameters["@keyword"].Value.ToString() == "2" && counter == 1));
                                                                  counter++;
                                                              }));
            keywordOperation.AddKeywords("12", "keyword", new List<string> {"1", "2"});
        }
    }
}
