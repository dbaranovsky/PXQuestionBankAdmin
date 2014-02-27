using System;
using System.Collections.Generic;
using System.Configuration;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.PXPub.Controllers.Mappers;

namespace Bfw.PX.PXPub.Controllers.Tests.Mappers
{
    [TestClass]
    public class ExternalDomainMapperTest
    {

        [TestCategory("ExternalDomainMapper"), TestMethod]
        public void MapUrlToPxUrl_IsExternalContent_IsArgaItem_ExpectIsTransformedArgaItemIsTrue()
        {
            ExternalContent externalContent = new ExternalContent { Url = "http://courses.bfwpub.com" };
            ExternalDomainMapper.MapUrlToPxUrl(externalContent);
            Assert.IsTrue(externalContent.IsTransformedArgaItem);
        }

        [TestCategory("ExternalDomainMapper"), TestMethod]
        public void MapUrlToPxUrl_IsExternalContent_IsEbook_ExpectIsTransformedArgaItemIsFalse()
        {
            ExternalContent externalContent = new ExternalContent { Url = "assets/pdx_files/digfir/morris1e/hlw_ebook/hlw_ch02_4.html" };
            ExternalDomainMapper.MapUrlToPxUrl(externalContent);
            Assert.IsFalse(externalContent.IsTransformedArgaItem);
        }
    }
}
