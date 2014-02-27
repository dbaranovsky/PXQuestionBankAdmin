using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.Helpers
{
    public class GradebookExportHelper : IGradebookExportHelper
    {
        public string GetCsvString(IEnumerable<Enrollment> enrollments)
        {
            var sb = new StringBuilder(300000);
            var categories = new List<KeyValuePair<string, string>>();

            if (enrollments.Any(o => o.CategoryGrades.Count() > 0))
            {
                categories = (from c in enrollments.ToList().First().CategoryGrades
                             select new KeyValuePair<string, string>(c.Id, c.Name)).ToList();
            }

            WriteGradeHeaderLine(categories, sb);

            foreach (var enrollment in enrollments)
            {
                WriteGradeLine(enrollment, categories, sb);
            }

            sb.Remove(sb.Length - 2, 2); 

            return sb.ToString();            
        }

        private void WriteGradeHeaderLine(List<KeyValuePair<string, string>> categories, StringBuilder sb)
        {
            var headerRow = sb.AppendFormat("{0},", "FullName");

            sb.AppendFormat("{0},", "LMS ID");
            sb.AppendFormat("{0},", "Email");
            sb.AppendFormat("{0},", "Points Achieved");
            sb.AppendFormat("{0},", "Points Possible");
            sb.AppendFormat("{0},", "Score");

            if (categories != null)
            {
                foreach (var category in categories)
                {
                    sb.AppendFormat("{0} Points Achieved,", category.Value);
                    sb.AppendFormat("{0} Points Possible,", category.Value);
                }
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine();
        }

        private void WriteGradeLine(Enrollment enrollment, List<KeyValuePair<string, string>> categories, StringBuilder sb)
        {
            sb.AppendFormat("{0},", enrollment.User.FormattedName);
            sb.AppendFormat("{0},", enrollment.User.ReferenceId);
            sb.AppendFormat("{0},", enrollment.User.Email);
            sb.AppendFormat("{0},", Math.Round(enrollment.OverallAchieved, 2));
            sb.AppendFormat("{0},", enrollment.OverallPossible);
            sb.AppendFormat("{0},", enrollment.OverallGrade);

            foreach (var category in categories)
            {
                var categoryScore = enrollment.CategoryGrades.SingleOrDefault(o => o.Id.Equals(category.Key));

                if (categoryScore != null)
                {
                    sb.AppendFormat("{0},", Math.Round(categoryScore.Achieved, 2));
                    sb.AppendFormat("{0},", categoryScore.Possible);
                }
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine();
        }   
    }
}
