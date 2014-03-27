using System;
using System.Collections.Generic;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class QbaAPiTests
    {
        private IQBA_API_interface qbaApi;

        [TestInitialize]
        public void TestInitialize()
        {
            qbaApi = new QbaApiStub();
        }


        [TestMethod]
        public void CopyQuestionTest()
        {
            qbaApi.CopyQuestion(String.Empty);
        }


        [TestMethod]
        public void CreateQuestionTest()
        {
            qbaApi.CreateQuestion();
        }


        [TestMethod]
        public void DeleteNoteTest()
        {
             qbaApi.DeleteNote(String.Empty, String.Empty);
        }


        [TestMethod]
        public void GetBankListTest()
        {
            qbaApi.GetBankList(String.Empty);
        }


        [TestMethod]
        public void GetChapterListTest()
        {
            qbaApi.GetChapterList(String.Empty);
        }


        [TestMethod]
        public void GetFilterableMetadataFieldsTest()
        {
            qbaApi.GetFilterableMetadataFields(String.Empty);
        }


        [TestMethod]
        public void GetOrderedQuestionListTest()
        {
            qbaApi.GetOrderedQuestionList(String.Empty, String.Empty);
        }


        [TestMethod]
        public void GetPossibleMetadataFieldValuesTest()
        {
            qbaApi.GetPossibleMetadataFieldValues(String.Empty, String.Empty);
        }


        [TestMethod]
        public void GetQuestionMetadataTest()
        {
            qbaApi.GetQuestionMetadata(String.Empty);
        }


        [TestMethod]
        public void GetQuestionNotesTest()
        {
            qbaApi.GetQuestionNotes(String.Empty);
        }


        [TestMethod]
        public void GetQuestionPreviewTest()
        {
            qbaApi.GetQuestionPreview(String.Empty);
        }


        [TestMethod]
        public void GetQuestionStatusTest()
        {
            qbaApi.GetQuestionStatus(String.Empty);
        }


        [TestMethod]
        public void GetQuestionStatusListTest()
        {
            qbaApi.GetQuestionStatusList();
        }


        [TestMethod]
        public void GetQuestionsTest()
        {
            qbaApi.GetQuestions(String.Empty);
        }


        [TestMethod]
        public void GetQuestionsWithDictionaryParamTest()
        {
            qbaApi.GetQuestions(new Dictionary<string, string>());
        }


        [TestMethod]
        public void GetTitleListTest()
        {
            qbaApi.GetTitleList(String.Empty);
        }


        [TestMethod]
        public void IsMetadataFieldEditableTest()
        {
            qbaApi.IsMetadataFieldEditable(String.Empty, String.Empty);
        }


        [TestMethod]
        public void IsNoteFlaggedTest()
        {
            qbaApi.IsNoteFlagged(String.Empty, String.Empty);
        }


        [TestMethod]
        public void IsQuestionFlaggedTest()
        {
            qbaApi.IsQuestionFlagged(String.Empty);
        }


        [TestMethod]
        public void IsQuestionLiveTest()
        {
            qbaApi.IsQuestionLive(String.Empty);
        }


        [TestMethod]
        public void QuestionBulkUpdateTest()
        {
            qbaApi.QuestionBulkUpdate(new List<Question>());
        }


        [TestMethod]
        public void SaveNoteTest()
        {
            qbaApi.SaveNote(String.Empty);
        }


        [TestMethod]
        public void SaveQuestionTest()
        {
            qbaApi.SaveQuestion(new Question());
        }


        [TestMethod]
        public void SetFlagTest()
        {
            qbaApi.SetFlag(String.Empty, false);
        }
    }
}
