using System;
using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.PXPub.Controllers.Mappers;

namespace Bfw.PX.PXPub.Controllers.Tests.Mappers
{
    [TestClass]
    public class WidgetMapperTest
    {
        [TestMethod]
        public void ToWidgetItem_Maps_UseProductCourse_Property()
        {
            Bfw.PX.Biz.DataContracts.Widget biz = new Bfw.PX.Biz.DataContracts.Widget();
            biz.Properties.Add("bfw_use_product_course", new PropertyValue() { Type = PropertyType.Boolean, Value = true });

            var widget = biz.ToWidgetItem();

            Assert.AreEqual(biz.UseProductCourse, widget.UseProductCourse);
        }
    }
}
