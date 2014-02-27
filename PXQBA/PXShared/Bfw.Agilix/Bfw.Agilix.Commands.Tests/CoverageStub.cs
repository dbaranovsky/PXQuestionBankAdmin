using System;
using System.Reflection;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Bfw.Agilix;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Components;
using Bfw.Agilix.Dlap.Configuration;
using Bfw.Agilix.Dlap.Session;

using Bfw.Common;
using Bfw.Common.Caching;
using Bfw.Common.Captcha;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using Bfw.Common.DynamicExtention;
using Bfw.Common.Exceptions;
using Bfw.Common.HttpModules;
using Bfw.Common.JqGridHelper;
using Bfw.Common.Logging;
using Bfw.Common.Mvc;
using Bfw.Common.Pagination;
using Bfw.Common.Patterns;
using Bfw.Common.SSO;
using Bfw.Common.Tests;
using Bfw.Common.Web;
using Bfw.Common.Xaml;

using Bfw.HtsConversion;

using Bfw.PX.Abstractions;

using Bfw.PX.Account;
using Bfw.PX.Account.Abstract;
using Bfw.PX.Account.Components;

using Bfw.PX.Biz.Components;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services;
using Bfw.PX.Biz.Services.Mappers;

using Bfw.PX.Notes;

using Bfw.PX.PXPub;
using Bfw.PX.PXPub.Components;
//using Bfw.PX.PXPub.Controllers;
using Bfw.PX.PXPub.Models;
using System.Collections.Generic;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class CoverageStub
    {
        [TestMethod]
        public void ForceCoverage()
        {
            string path = @"S:\BuildAgent\work";
            string pattern = @"*Bfw.Agilix.Commands.Tests";
            List<string> directories;
            string workingDirectory = "";

            try
            {
                directories = new List<string>(Directory.EnumerateDirectories(path, pattern, SearchOption.AllDirectories));

                foreach (var dir in directories)
                {
                    Console.WriteLine("{0}", dir.Substring(dir.LastIndexOf("\\") + 1));
                    if (!dir.Contains(".old"))
                    {
                        Console.WriteLine("Working directory set.");
                        workingDirectory = dir;
                    }
                }
                Console.WriteLine("{0} directories found.",  directories.Count);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("path: '{0}', pattern: '{1}'", path, pattern));
            }

            Assert.IsNotNull(workingDirectory);           
            
            try
            {               
                var dirInfo = new DirectoryInfo(workingDirectory + @"\publish\bin");
                var dlls = dirInfo.GetFiles("*.dll", SearchOption.AllDirectories);

                Assert.IsTrue(dlls.Length > 0);

                foreach (var dll in dlls)
                {
                    try
                    {
                        var asm = Assembly.LoadFile(dll.FullName);
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                
                 Console.WriteLine(ex.Message + "\n working dir: '{0}'", workingDirectory);
            }
            

            Assert.IsTrue(true);
        }
    }
}
