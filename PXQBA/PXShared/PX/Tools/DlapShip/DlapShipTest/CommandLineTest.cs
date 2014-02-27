using System;
using System.IO;
using DlapShip;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DlapShipTest
{
    [TestClass]
    public class CommandLineTest
    {
        private const string DeleteDlapItemFile1 = "../../../TestScripts/DeleteDlapItem1.xml";

        [TestMethod]
        public void CommandLineAction_IfValidDeleteCmdArgs_ArgsParsedProperly()
        {
            string deleteFile = "C:\\Blah\\Blah.xml";
            string book = "theguide10e";
            string app = "xbook";
            string env = "dev";
            PrivateType pt = new PrivateType(typeof(Program));
      
            CommandOptions options = (CommandOptions)pt.InvokeStatic("GetCommandOptions", new object[]{
                new string[] { 
                    string.Format("/d:{0}", deleteFile), 
                    string.Format("/ti:{0}", book), 
                    string.Format("/app:{0}", app), 
                    string.Format("/e:{0}", env)
                }
            });

            Assert.IsTrue(options.DeleteItemsFilePath.Equals(deleteFile) && options.Title.Equals(book) && 
                            options.App.Equals(app) && options.Environment.Equals(env));
        }

        [TestMethod]
        public void CommandLineAction_IfValidDeleteCmdFile_FileExists()
        {
            PrivateType pt = new PrivateType(typeof(Program));
            Program.DeleteItemsFilePath = DeleteDlapItemFile1;

            Assert.IsTrue((bool)pt.InvokeStatic("AreFilesValid", new object[]{}));
        }

        [TestMethod]
        public void CommandLineAction_IfInvalidDeleteCmdFile_FileDoesntExist()
        {
            PrivateType pt = new PrivateType(typeof(Program));
            Program.DeleteItemsFilePath = "../../../TestScripts/DoesntExistFile.xml";

            Assert.IsFalse((bool)pt.InvokeStatic("AreFilesValid", new object[] { }));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CommandLineAction_IfCmdInvalidArg_ArgumentExceptionIsThrown()
        {
            string deleteFile = "C:\\Blah\\Blah.xml";
            string book = "theguide10e";
            string app = "xbook";
            PrivateType pt = new PrivateType(typeof(Program));

            CommandOptions options = (CommandOptions)pt.InvokeStatic("GetCommandOptions", new object[]{
                new string[] { 
                    string.Format("/d:{0}", deleteFile), 
                    string.Format("/ti:{0}", book), 
                    string.Format("/app:{0}", app),
                    "/invalid:shouldfail"
                }
            });
        }
    }
}
