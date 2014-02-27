using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class StudentGrade
    {
        public string UserId { get; set; }

        public string EnrollmentId {get; set;}

        public bool HasSubmitted { get; set; }

        public bool HasGraded {get; set;}

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Text { get; set; }

        public List<ArtifactScore> ArtifactData { get; set; }
        
        public List<string> HighestPerformance { get; set; }
        
        public List<string> LowestPerformance { get; set; }

        //based on rubric data
        public double TotalPossiblePoints { get; set; }
        public double TotalAchievedPoints { get; set; }
        public double GradePct { get; set; }
        public double PointScore { get; set; }

        //based on main grading
        public double TotalPossiblePoints2 { get; set; }
        public double TotalAchievedPoints2 { get; set; }
        public double GradePct2 { get; set; }
        public double PointScore2 { get; set; }

    }

    public class ArtifactScore {
        public string ArtifactName {get; set;} 

        public double PointsPossible {get; set;}

        public double PointsAchieved {get;set;}

        public string GradeText { get; set; }

        public double GradePct { get; set; }

        public double PointScore { get; set; }
    }
}
