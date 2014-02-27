using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bfw.PX.PXPub.Models
{
    public class GradebookPreferences
    {
        [Range(0, 100)]
        [Display(Name="Passing Score (%):")]
        public double PassingScore { set; get; }

        [Display(Name = "Use weighted categories")]
        public bool UseWeightedCategories { set; get; }

        public GradeBookWeights GradeBookWeights { set; get; }
    }
}
