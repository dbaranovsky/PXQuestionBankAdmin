using System.Linq;
using System.Xml.Linq;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Macmillan.PXQBA.Business.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class QuestionHelperTests
    {
        public string questionBodyWithImages =
            @"<question questionid=""88b12b6e-01b7-4ada-b97e-148a49e6d0cb"" version=""2"" resourceentityid=""39768,62"" creationdate=""2014-07-08T09:49:02.167Z"" creationby=""7"" modifieddate=""2014-07-08T09:49:08.9233687Z"" modifiedby=""7"" flags=""4"" actualentityid=""39768"" schema=""2"" partial=""false"">
                  <meta>
                    
                  </meta>
                  <answer>
                    <value>1</value>
                  </answer>
                  <body>&lt;div&gt;text a lot of&amp;nbsp;[~] texttext a lot of text&lt;img src=""[~]/tumblr_m0igv3QYdQ1r28r9f.gif"" border=""0"" alt=""[~]/tumblr_m0igv3QYdQ1r28r9f.gif"" title=""[~]/tumblr_m0igv3QYdQ1r28r9f.gif"" width=""140"" height=""140"" /&gt;text a lot of text&amp;nbsp; text a lot&amp;nbsp;[~] of text text a lot of texttext a lot of text [~]&lt;/div&gt;&lt;div&gt;&amp;nbsp;text a lot of texttext&amp;nbsp; [~] /&amp;nbsp; sdfdf /dfs dfdfsfd/ [~] [~]a lot of text&lt;/div&gt;&lt;div&gt;&amp;nbsp;&lt;/div&gt;</body>
                  <interaction type=""choice"">
                    <choice id=""1"">
                      <body>&lt;div&gt;&lt;img src=""[~]/funpage24.jpg"" border=""0"" alt=""[~]/funpage24.jpg"" title=""[~]/funpage24.jpg"" width=""130"" height=""69"" /&gt;&lt;/div&gt;</body>
                    </choice>
                    <choice id=""2"">
                      <body>2</body>
                    src=""[~]/folder/image.jpg""
                    </choice>
                    <choice id=""3"">
                      <body>3</body>
                    </choice>
                  </interaction>
                </question>";

        [TestMethod]
        public void GetQuestionRelatedResources_QuestionXmlWithThreeImages_ListOfThreePathes()
        {
            var list = QuestionHelper.GetQuestionRelatedResources(questionBodyWithImages);

            Assert.AreEqual(list.Count, 3);
            Assert.IsFalse( list.First().Contains("[~]"));
        }
    }
}