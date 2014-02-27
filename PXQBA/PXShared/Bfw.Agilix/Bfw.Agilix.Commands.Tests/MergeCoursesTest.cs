using Bfw.Agilix.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using TestHelper;

namespace Bfw.Agilix.Commands.Tests
{
	[TestClass]
	public class MergeCoursesTest
	{
		private MergeCourses mergeCourses;

		//as en example merge the derative course with ID 8874 into its master course. 
		[TestInitialize]
		public void TestInitialize()
		{
			mergeCourses = new MergeCourses {CourseId = "123", BatchOrder = 1, RunAsync = true};
			
		}

		[TestMethod]
		public void MergeCoursesTest_ToRequest()
		{
			var request = mergeCourses.ToRequest();
			Assert.AreEqual("mergecourses", request.Parameters["cmd"]);
		}

		[TestMethod]
		public void MergeCoursesTest_ParseResponse()
		{
			var dlapResponse = new DlapResponse(){Code=DlapResponseCode.OK};
			mergeCourses.ParseResponse(dlapResponse);			
		}

		[TestMethod]
		[ExpectedException(typeof(DlapException))]
		public void MergeCoursesTest_Raise_Exception_When_DLAP_Return_Error()
		{
			mergeCourses.ParseResponse(new DlapResponse() { Code = DlapResponseCode.Error });
		}

		[TestMethod]
		[ExpectedException(typeof(DlapException))]
		public void MergeCoursesTest_Raise_Exception_When_CourseId_Empty()
		{
			var merge = new MergeCourses();
			merge.ToRequest();
		}


		[TestMethod]
		public void MergeCoursesTest_Initialize_Request()
		{
			mergeCourses.ToRequest();
			var request = mergeCourses.ToRequest();
			Assert.AreEqual("mergecourses", request.Parameters["cmd"]);
		}
	}
}
