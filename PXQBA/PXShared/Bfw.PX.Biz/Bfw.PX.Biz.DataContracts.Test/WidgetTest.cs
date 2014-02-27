using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.PX.Biz.DataContracts.Test
{
    [TestClass]
    public class WidgetTest
    {
        [TestMethod]
        public void Widget_Returns_UserProductCourse_Property()
        {
            Widget widget = new Widget()
            {
                Properties = new Dictionary<string, PropertyValue>()
            };
            widget.Properties.Add("bfw_use_product_course", new PropertyValue() { Type = PropertyType.Boolean, Value = true });

            Assert.AreEqual(true, widget.UseProductCourse);
        }
    }
}
