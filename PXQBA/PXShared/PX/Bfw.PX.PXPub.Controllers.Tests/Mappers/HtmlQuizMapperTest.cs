using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.PX.Biz.ServiceContracts;
using BizDC = Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bfw.PX.PXPub.Controllers.Tests.Mappers
{
    [TestClass]
    public class HtmlQuizMapperTest
    {
        private IContentActions _contentActions;

        [TestInitialize]
        public void Setup()
        {
            _contentActions = Substitute.For<IContentActions>();
        }

        [TestCategory("Mapping"), TestMethod]
        public void HtmlQuizMapperAction_ToHtmlQuiz_SetsBHHtmlQuizTrue_IfTemplateElementPresent()
        {
            var bizItem = new BizDC.ContentItem()
            {
                Type = "Assessment",
                Subtype = "htmlquiz",
                ExamTemplate = "examtemplate"
            };
            var contentItem = bizItem.ToHtmlQuiz(_contentActions, true);
            Assert.IsTrue(contentItem.BHHtmlQuiz);
        }

        [TestCategory("Mapping"), TestMethod]
        public void HtmlQuizMapperAction_ToHtmlQuiz_SetsBHHtmlQuizFalse_IfNoTemplateElement()
        {
            var bizItem = new BizDC.ContentItem()
            {
                Type = "Assessment",
                Subtype = "htmlquiz"
            };
            var contentItem = bizItem.ToHtmlQuiz(_contentActions, true);
            Assert.IsFalse(contentItem.BHHtmlQuiz);
        }
    }
}
