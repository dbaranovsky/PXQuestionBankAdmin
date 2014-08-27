using System;
using System.Collections.Generic;
using System.Data.Common;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.SQLOperations;
using Macmillan.PXQBA.Business.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class ParsedFileOperationTest
    {
        private IParsedFileOperation parsedFileOperation;
        private IDatabaseManager databaseManager;

        [TestInitialize]
        public void TestInitialize()
        {
            databaseManager = Substitute.For<IDatabaseManager>();
            parsedFileOperation = new ParsedFileOperation(databaseManager);
        }

        [TestMethod]
        public void AddParsedFile_AnyCorrectParams_SuccessRun()
        {

            var counter = 0;
            databaseManager.ExecuteScalar(Arg.Do<DbCommand>(x =>
            {
                Assert.IsTrue(x.CommandText == "dbo.AddQBAParsedFile");
                Assert.IsTrue(x.Parameters["@fileName"].Value.ToString() == "file.zip");
                counter++;
            })).Returns(5);

            var result = parsedFileOperation.AddParsedFile("file.zip", "data", new byte[4]);
            Assert.AreEqual(result, 5);
            Assert.AreEqual(counter, 1);

        }


        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void AddParsedFile_AnyIncorrectParams_FailedRun()
        {

            var counter = 0;
            databaseManager.ExecuteScalar(Arg.Do<DbCommand>(x =>
            {
                Assert.IsTrue(x.CommandText == "dbo.AddQBAParsedFile");
                Assert.IsTrue(x.Parameters["@fileName"].Value.ToString() == "file.zip");
                counter++;
            })).Returns("a");

            parsedFileOperation.AddParsedFile("file.zip", "data", new byte[4]);


        }

        [TestMethod]
        public void SetParsedFileStatus_AnyCorrectParams_SuccessRun()
        {

            var counter = 0;
            databaseManager.ExecuteScalar(Arg.Do<DbCommand>(x =>
            {
                Assert.IsTrue(x.CommandText == "dbo.SetQBAParsedFile");
                Assert.IsTrue(x.Parameters["@id"].Value.ToString() == "4");
                Assert.IsTrue(x.Parameters["@status"].Value.ToString() == "1");
                counter++;
            })).Returns(5);

            var result = parsedFileOperation.SetParsedFileStatus(4, ParsedFileStatus.Imported);
            Assert.AreEqual(result, 5);
            Assert.AreEqual(counter, 1);

        }


        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void SetParsedFileStatus_AnyIncorrectParams_FailedRun()
        {

            var counter = 0;
            databaseManager.ExecuteScalar(Arg.Do<DbCommand>(x =>
            {
                Assert.IsTrue(x.CommandText == "dbo.SetQBAParsedFile");
                Assert.IsTrue(x.Parameters["@id"].Value.ToString() == "4");
                Assert.IsTrue(x.Parameters["@status"].Value.ToString() == "1");
                counter++;
            })).Returns("a");

           parsedFileOperation.SetParsedFileStatus(4, ParsedFileStatus.Imported);


        }


        [TestMethod]
        public void GetParsedFile_ParsedFileId_ParsedFileWithResourses()
        {
            var databaseRecord = new DatabaseRecord();
            databaseRecord["Id"] = (long)5;
            databaseRecord["FileName"] = "test.zip";
            databaseRecord["QuestionsData"] = "test";
            databaseRecord["ResourcesData"] = new byte[5];

            databaseManager.Query(Arg.Do<DbCommand>(x =>
                                                    {
                                                        Assert.IsTrue(x.CommandText == "dbo.GetQBAParsedFile");
                                                        Assert.IsTrue(x.Parameters["@id"].Value.ToString() == "5");
                                                    }))
                           .Returns(new List<DatabaseRecord>() { databaseRecord });

            var parsedFile = parsedFileOperation.GetParsedFile(5);
            Assert.IsTrue(parsedFile != null);
            Assert.IsTrue(parsedFile.FileName == "test.zip");
            Assert.IsTrue(parsedFile.ResourcesData != null);
            Assert.IsTrue(parsedFile.QuestionsData == "test");
            Assert.IsTrue(parsedFile.Id == 5);


        }

        [TestMethod]
        public void GetParsedFile_ParsedFileId_ParsedFileWithoutResourses()
        {
            var databaseRecord = new DatabaseRecord();
            databaseRecord["Id"] = (long)5;
            databaseRecord["FileName"] = "test.zip";
            databaseRecord["QuestionsData"] = "test";
            databaseRecord["ResourcesData"] = null;

            databaseManager.Query(Arg.Do<DbCommand>(x =>
            {
                Assert.IsTrue(x.CommandText == "dbo.GetQBAParsedFile");
                Assert.IsTrue(x.Parameters["@id"].Value.ToString() == "5");
            }))
                           .Returns(new List<DatabaseRecord>() { databaseRecord });

            var parsedFile = parsedFileOperation.GetParsedFile(5);
            Assert.IsTrue(parsedFile != null);
            Assert.IsTrue(parsedFile.FileName == "test.zip");
            Assert.IsTrue(parsedFile.ResourcesData == null);
            Assert.IsTrue(parsedFile.QuestionsData == "test");
            Assert.IsTrue(parsedFile.Id == 5);


        }
    }
}
