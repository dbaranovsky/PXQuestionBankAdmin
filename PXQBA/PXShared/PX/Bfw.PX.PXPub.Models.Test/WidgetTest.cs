using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.PX.PXPub.Models.Test
{
    [TestClass]
    public class WidgetTest
    {
        [TestMethod]
        public void Widget_Sets_UserProductCourse_Property()
        {
            var widget = new Widget();

            widget.UseProductCourse = true;

            Assert.AreEqual(true, widget.UseProductCourse);
        }

        [TestMethod]
        public void Widget_Gets_UserProductCourse_Property()
        {
            var widget = new TreeWidgetSettings();

            Assert.AreEqual(false, widget.UseProductCourse);
        }
    }
}
