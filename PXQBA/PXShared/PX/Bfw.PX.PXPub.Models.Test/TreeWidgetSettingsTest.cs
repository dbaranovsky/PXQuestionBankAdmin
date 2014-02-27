using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.PX.PXPub.Models.Test
{
    [TestClass]
    public class TreeWidgetSettingsTest
    {
        [TestMethod]
        public void TreeWidgetSettings_Sets_UserProductCourse_Property()
        {
            var settings = new TreeWidgetSettings();

            settings.UseProductCourse = true;

            Assert.AreEqual(true, settings.UseProductCourse);
        }

        [TestMethod]
        public void TreeWidgetSettings_Gets_UserProductCourse_Property()
        {
            var settings = new TreeWidgetSettings();

            Assert.AreEqual(false, settings.UseProductCourse);
        }
    }
}
