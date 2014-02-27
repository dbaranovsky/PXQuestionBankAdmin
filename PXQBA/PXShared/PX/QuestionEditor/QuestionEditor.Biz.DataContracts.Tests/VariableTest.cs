using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.PX.QuestionEditor.Biz.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuestionEditor.Biz.DataContracts.Tests
{
    [TestClass]
    public class VariableTest
    {
        [TestMethod]
        public void Verify_ArrayValue_Returns_Correctly_For_BinomPdf()
        {
            var variable = new Variable()
            {
                Constraints = new List<Constraint>()
                {
                    new Constraint()
                    {
                        Inclusions = "BinomPDF{$a, $b, $c}"
                    }
                }
            };
            
            Assert.AreEqual(variable.ArrayValues.Count(),1);
        }
    }
}
