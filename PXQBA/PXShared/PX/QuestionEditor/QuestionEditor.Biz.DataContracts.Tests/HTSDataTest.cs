using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Bfw.PX.QuestionEditor.Biz.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace QuestionEditor.Biz.DataContracts.Tests
{
    [TestClass]
    public class HTSDataTest
    {
     /// <summary>
     /// Testing a portion of the question conversion from  "multiple choice" to "advanced" type.
     /// Specifically, if a multiple choice question has internal choice ids that are not numeric (i.e. publisher true/false questions),
     /// they have to be replaced by the new numeric sequence as required by the advanced question player
     /// </summary>
        
        // test conversion of non-numeric ids
        [TestMethod]
        public void Verify_InteractionNodeXml_ConvertedNumericChoiceIds()
        {
           // setup        
            XmlDocument xDoc = dataNotNumeric();
            var questionNode = xDoc.SelectSingleNode("question");
            var interactionNode = questionNode.SelectSingleNode("interaction");

            // Normally we don't test private methods, but this one is done on purpose
            // InteractionNodeXml method allows us to intercept question xml before its converted to an image
            
            var htsdata = new HTSData();
            Step oStep = new Step();
            PrivateObject o = new PrivateObject(htsdata);
  
            o.Invoke("InteractionNodeXml", new object[] {questionNode, oStep, interactionNode} );
            Assert.IsTrue(oStep.Question.Contains("choiceid=\"1\""));
            Assert.IsTrue(oStep.Question.Contains("choiceid=\"2\""));
            Assert.IsTrue(oStep.Question.Contains("choiceid=\"3\""));
            
            //make sure that correct answer id is also replaced
            Assert.AreEqual(htsdata.Solution, "2");
        }

        // test that numeric choice ids don't change
        [TestMethod]
        public void Verify_InteractionNodeXml_NumericChoiceIds()
        {
            // setup        
            XmlDocument xDoc = dataNumeric();
            var questionNode = xDoc.SelectSingleNode("question");
            var interactionNode = questionNode.SelectSingleNode("interaction");

            // test 
            var htsdata = new HTSData();
            Step oStep = new Step();
            PrivateObject o = new PrivateObject(htsdata);
            o.Invoke("InteractionNodeXml", new object[] { questionNode, oStep, interactionNode });

            Assert.IsTrue(oStep.Question.Contains("choiceid=\"11\""));
            Assert.IsTrue(oStep.Question.Contains("choiceid=\"22\""));
            Assert.IsTrue(oStep.Question.Contains("choiceid=\"33\""));
            Assert.AreEqual(htsdata.Solution, "33");
        }

        #region testData

        private static XmlDocument dataNotNumeric()
        {
            XmlDocument xDoc = new XmlDocument();
            string mcXml = @"<question>
                <answer>
                    <value>B</value>
                </answer>
                <body>a?</body>
                <interaction type='choice'>
                <choice id='A'>
                    <body>a</body>
                </choice>
                <choice id='B'>
                     <body>aa</body>
                </choice>
                <choice id='C'>
                    <body>aaa</body>
                </choice>
                </interaction>
            </question>";
            xDoc.LoadXml(mcXml);
            return xDoc;
        }

        private static XmlDocument dataNumeric()
        {
            XmlDocument xDoc = new XmlDocument();
            string mcXml = @"<question>
                <answer>
                    <value>33</value>
                </answer>
                <body>a?</body>
                <interaction type='choice'>
                <choice id='11'>
                    <body>a</body>
                </choice>
                <choice id='22'>
                     <body>aa</body>
                </choice>
                <choice id='33'>
                    <body>aaa</body>
                </choice>
                </interaction>
            </question>";
            xDoc.LoadXml(mcXml);
            return xDoc;
        }
        #endregion
    }
}

  