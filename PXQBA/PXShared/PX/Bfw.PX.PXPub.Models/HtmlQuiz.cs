using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Collections;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a page of ebook content that may have questions
    /// </summary>
    public class HtmlQuiz : Quiz
    {
        /// <summary>
        /// Defines the element in item's data that would make this htmlquiz render in the BH activity player
        /// </summary>
        public static string BHTemplateElement { get { return "examtemplate"; } }

        /// <summary>
        /// Logic for if we should display submission controls
        /// </summary>
        public bool DisplaySubmissionControls
        {
            get
            {
                return Questions != null && Questions.Count > 0 && (DateTime.Today <= DueDate || OverrideDueDateReq) && !IsInstructor; ;
            }
        }

        /// <summary>
        /// Log for if we should display previous attempts at the bottom of the html quiz
        /// </summary>
        public bool DisplayAttempts
        {
            get
            {
                return Submissions != null && Submissions.Count() > 0 && !IsInstructor;
            }
        }

        /// <summary>
        /// Get/Set if the html quiz should be rendered through the brain honey quiz player
        /// </summary>
        public bool BHHtmlQuiz { get; set; }

        public XBookAppParams XBookAppParams { get; set; }

        public HtmlQuiz()
            : base()
        {
            this.Type = "htmlquiz";
            XBookAppParams = new XBookAppParams();
            XBookAppParams.ComponentName = "MainContentArea";
        }
    }

    public class XBookAppParams
    {
        /// <summary>
        /// Name of the component that needs to be requested when 
        /// </summary>
        public string ComponentName { get; set; }
        public string CssOverride { get; set; }
        public string DlapCookie { get; set; }
        public string StudentOverride { get; set; }
        public string ItemID { get; set; }
        public string ProductCourseId { get; set; }
        public string EnrollmentId { get; set; }
        public string PXAppPath { get; set; }
        public string BrainHoneyUrl { get; set; }
        public string AgilixUrl { get; set; }

        //TODO: Add unit tests to test this
        public string GetComponentUrl()
        {
            var options =
                typeof(XBookAppParams).GetProperties().Where(p => p.Name != "ComponentName" && p.Name != "AgilixUrl");
            var properties = String.Join("&", options.Where(p => p.GetValue(this, null) != null).Map(k => String.Join("=",
                new string[]
                {
                    k.Name, 
                    k.GetValue(this, null).ToString()
                })).ToArray());
            var url = String.Format("{0}/Components/{1}?{2}", AgilixUrl, ComponentName, properties);

            return url;
        }
    }
}
