
using System.Collections.Generic;
namespace Bfw.PX.Biz.DataContracts
{
[System.Serializable]
    public class StudentRubricReport
    {
        public string EnrollmentId { get; set; }

        public string StudentName { get; set; }

        public bool IsStudentDeleted { get; set; }

        public int AssignmentsCompleted { get; set; }

        public int ReviewedSubmissions { get; set; }

        public int AverageAllCriteria { get; set; }

        /// <summary>
        /// per rubric rule id.
        /// </summary>
        /// <value>
        /// The rubric results.
        /// </value>
        public Dictionary<string, RubricGrade> RubricResults { get; set; }
    }
}
