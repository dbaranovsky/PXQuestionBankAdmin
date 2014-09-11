using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfw.Agilix.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Common.Helpers.Tests
{
    [TestClass]
    public class QuestionPreviewHelperTest
    {
        [TestMethod]
        public void GetQuestionHtmlPreview_EmptyCustomQuestion_BodyIsEmptyHtml()
        {
            const string expectedHtml = @"<div class=""question-preview""><div class=""question-body""></div>Body is empty, no preview available</div>";

            Question question = new Question()
                                {
                                    InteractionType = "custom"
                                };

            string html = QuestionPreviewHelper.GetQuestionHtmlPreview(question);

            Assert.IsTrue(expectedHtml == html);
        }

        [TestMethod]
        public void GetQuestionHtmlPreview_CustomGRAPHQuestion_HtmlPreview()
        {
            const string notExpectedHtml = @"<div class=""question-preview""><div class=""question-body""></div>Body is empty, no preview available</div>";

            const string interactionData = @"<question>
                                      <meta chapter="""" difficulty=""SELECT"" skill=""SELECT"" topic="""">
                                        <custom><![CDATA[]]></custom>
                                      </meta>
                                      <feedback>
                                        <correct><![CDATA[]]></correct>
                                        <incorrect><![CDATA[]]></incorrect>
                                      </feedback>
                                      <evaluate/>
                                      <graph>
                                        <tolerance pixelAccuracy=""10"" slopeAccuracy=""10"" unlabeledDeductions=""15""/>
                                        <snapping default=""true"" toggleable=""true""/>
                                        <mouse xIncrements="".25"" yIncrements="".25""/>
                                        <units rows=""100"" cols=""100"" drawGrid=""true"" drawTicks=""true"" rowTicks=""10"" colTicks=""10"" rowGrid=""5"" colGrid=""5""/>
                                        <labels x=""x-axis"" y=""y-axis""/>
                                      </graph>
                                      <defaultElements>
                                        <element type=""TwoPointLine"" legend=""None"" dotColor=""0"" lineColor=""0"" hideDots=""false"" evaluate=""false"">
                                          <editable duplicate=""true"" erase=""false"" legend=""true"" drag=""false""/>
                                          <point x=""35"" y=""60""/>
                                          <point x=""70"" y=""60""/>
                                        </element>
                                      </defaultElements>
                                      <answerElements>
                                        <element type=""SinglePoint"" legend=""None"" dotColor=""0"" lineColor=""0"" hideDots=""false"" evaluate=""true"">
                                          <editable duplicate=""true"" erase=""true"" legend=""true"" drag=""true""/>
                                          <evaluate by=""""/>
                                          <point x=""30"" y=""60""/>
                                        </element>
                                      </answerElements>
                                      <legend title=""Legend"">
                                        <radio label=""None"" color=""0x000000"" selectable=""true""/>
                                      </legend>
                                      <ui>
                                        <tool type=""Selection"" tip=""""/>
                                        <tool type=""SinglePoint"" tip=""""/>
                                        <tool type=""Duplicate"" tip=""""/>
                                        <tool type=""Eraser"" tip=""""/>
                                      </ui>
                                      <question><![CDATA[]]></question>
                                </question>";

            Question question = new Question()
            {
                Id = "Id",
                InteractionType = "custom",
                InteractionData = interactionData,
                CustomUrl = "FMA_GRAPH"
            };

            string html = QuestionPreviewHelper.GetQuestionHtmlPreview(question);

            Assert.IsTrue(notExpectedHtml.ToUpper() != html.ToUpper());
            Assert.IsTrue(!String.IsNullOrEmpty(html));
        }

        [TestMethod]
        public void GetQuestionHtmlPreview_CustomHTSQuestion_HtmlPreview()
        {
            const string notExpectedHtml = @"<div class=""question-preview""><div class=""question-body""></div>Body is empty, no preview available</div>";

            const string interactionData = @"<iproblem maxpoints=""1""><iprostep id=""step0""><p>gtdrfxgbxfg</p><div><input class=""hint"" id=""@step.hint""
                    onclick=""javascript:next('hint', '@step')"" type=""button"" value=""Hint"" hide=""yes"" /></div><div><ipronav navtype=""hint"" next=""step0hint"">
                    </ipronav><ipronav navtype=""correct"" next=""step0corr""></ipronav><ipronav navtype=""incorrect"" next=""step0inc""></ipronav></div></iprostep>
                    <iprostep id=""step0hint""><p>fgfdcgbv</p></iprostep><iprostep id=""step0corr""><p>fcgbfgbdcfg</p></iprostep><iprostep id=""step0inc"">
                    <p>dfgfcdgfcg</p></iprostep></iproblem>";

            Question question = new Question()
            {
                Id = "Id",
                InteractionType = "custom",
                InteractionData = interactionData,
                CustomUrl = "HTS"
            };

            string html = QuestionPreviewHelper.GetQuestionHtmlPreview(question);

            Assert.IsTrue(notExpectedHtml.ToUpper() != html.ToUpper());
            Assert.IsTrue(!String.IsNullOrEmpty(html));
        }

        [TestMethod]
        public void GetQuestionHtmlPreview_TextQuestion_HtmlPreview()
        {
            const string expectedHtml = @"<div class=""question-preview""><div class=""question-body""></div><b>Correct answer:</b><p>Correct</p></div>";

            Question question = new Question()
            {
                Id = "Id",
                InteractionType = "text",
                AnswerList = new List<string>()
                             {
                                 "Correct"
                             }
            };

            string html = QuestionPreviewHelper.GetQuestionHtmlPreview(question);

            Assert.IsTrue(expectedHtml.ToUpper() == html.ToUpper());
        }

        [TestMethod]
        public void GetQuestionHtmlPreview_MatchQuestion_HtmlPreview()
        {
            const string expectedHtml = @"<div class=""question-preview""><div class=""question-body""></div><ul><li><span class=""option-text"">Text1 = Answer1</span ></li><li><span class=""option-text"">Text2 = Answer2</span ></li></ul></div>";

            Question question = new Question()
            {
                Id = "Id",
                InteractionType = "match",
                Choices = new List<QuestionChoice>()
                          {
                              new QuestionChoice()
                              {
                                  Answer = "Answer1",
                                  Id="1",
                                  Text =  "Text1"
                              },
                              
                              new QuestionChoice()
                              {
                                  Answer = "Answer2",
                                  Id="2",
                                  Text =  "Text2"
                              }
                          }
            };

            string html = QuestionPreviewHelper.GetQuestionHtmlPreview(question);

            Assert.IsTrue(expectedHtml.ToUpper() == html.ToUpper());
        }

        [TestMethod]
        public void GetQuestionHtmlPreview_AnswerQuestion_HtmlPreview()
        {
            const string expectedHtml = @"<div class=""question-preview""><div class=""question-body""></div><ul><li><input disabled=""disabled"" type=""checkbox""  /><span class=""option-text"">Text1</span ></li><li><input disabled=""disabled"" type=""checkbox"" checked=checked /><span class=""option-text"">Text2</span ></li></ul></div>";

            Question question = new Question()
            {
                Id = "Id",
                InteractionType = "answer",
                Choices = new List<QuestionChoice>()
                          {
                              new QuestionChoice()
                              {
                                  Answer = "Answer1",
                                  Id="1",
                                  Text =  "Text1"
                              },
                              
                              new QuestionChoice()
                              {
                                  Answer = "Answer2",
                                  Id="2",
                                  Text =  "Text2"
                              }
                          },
                AnswerList = new List<string>()
                             {
                                 "2"
                             }
            };

            string html = QuestionPreviewHelper.GetQuestionHtmlPreview(question);

            Assert.IsTrue(expectedHtml.ToUpper() == html.ToUpper());
        }

        [TestMethod]
        public void GetGraphEditor_CorrectXml_Html()
        {
            const string customXML = @"<question>
                      <meta chapter="""" difficulty=""SELECT"" skill=""SELECT"" topic="""">
                        <custom><![CDATA[]]></custom>
                      </meta>
                      <feedback>
                        <correct><![CDATA[]]></correct>
                        <incorrect><![CDATA[]]></incorrect>
                      </feedback>
                      <evaluate/>
                      <graph>
                        <tolerance pixelAccuracy=""10"" slopeAccuracy=""10"" unlabeledDeductions=""15""/>
                        <snapping default=""true"" toggleable=""true""/>
                        <mouse xIncrements="".25"" yIncrements="".25""/>
                        <units rows=""100"" cols=""100"" drawGrid=""true"" drawTicks=""true"" rowTicks=""10"" colTicks=""10"" rowGrid=""5"" colGrid=""5""/>
                        <labels x=""x-axis"" y=""y-axis""/>
                      </graph>
                      <defaultElements>
                        <element type=""TwoPointLine"" legend=""None"" dotColor=""0"" lineColor=""0"" hideDots=""false"" evaluate=""false"">
                          <editable duplicate=""true"" erase=""false"" legend=""true"" drag=""false""/>
                          <point x=""35"" y=""65""/>
                          <point x=""90"" y=""65""/>
                        </element>
                      </defaultElements>
                      <answerElements>
                        <element type=""SinglePoint"" legend=""None"" dotColor=""0"" lineColor=""0"" hideDots=""false"" evaluate=""true"">
                          <editable duplicate=""true"" erase=""true"" legend=""true"" drag=""true""/>
                          <evaluate by=""""/>
                          <point x=""25"" y=""65""/>
                        </element>
                        <element type=""SinglePoint"" legend=""None"" dotColor=""0"" lineColor=""0"" hideDots=""false"" evaluate=""true"">
                          <editable duplicate=""true"" erase=""true"" legend=""true"" drag=""true""/>
                          <evaluate by=""""/>
                          <point x=""30"" y=""50""/>
                        </element>
                        <element type=""InfinityLine"" legend=""None"" dotColor=""0"" lineColor=""0"" hideDots=""false"" evaluate=""true"">
                          <editable duplicate=""true"" erase=""true"" legend=""true"" drag=""true""/>
                          <evaluate by=""""/>
                          <point x=""30"" y=""15""/>
                          <point x=""65"" y=""40""/>
                        </element>
                        <element type=""TwoPointLine"" legend=""None"" dotColor=""0"" lineColor=""0"" hideDots=""false"" evaluate=""true"">
                          <editable duplicate=""true"" erase=""true"" legend=""true"" drag=""true""/>
                          <point x=""5"" y=""85""/>
                          <point x=""60"" y=""40""/>
                          <evaluate by=""""/>
                        </element>
                      </answerElements>
                      <legend title=""Legend"">
                        <radio label=""None"" color=""0x000000"" selectable=""true""/>
                      </legend>
                      <ui>
                        <tool type=""Selection"" tip=""""/>
                        <tool type=""SinglePoint"" tip=""""/>
                        <tool type=""Duplicate"" tip=""""/>
                        <tool type=""Eraser"" tip=""""/>
                      </ui>
                      <question><![CDATA[]]></question>
                    </question>";



            string html = QuestionPreviewHelper.GetGraphEditor(customXML, "questionId", "FMA_GRAPH");

            Assert.IsTrue(!String.IsNullOrEmpty(html));
        }


        [TestMethod]
        public void GetEditorUrl_CorrectData_Url()
        {
          string url = QuestionPreviewHelper.GetEditorUrl("type", "questionId", "entId", "quizId");

          Assert.IsTrue(url == @"http://root.dev.brainhoney.bfwpub.com/BrainHoney/Component/QuestionEditor?enrollmentid=entId&itemid=quizId&questionid=questionId&showcancel=true");
        }
    }
}
