using System.ComponentModel.DataAnnotations;

namespace Bfw.PX.PXPub.Models
{
    public class QuizQuestionSettings
    {
        public string Id { get; set; }
        public string QuizId { get; set; }
        public string EntityId { get; set; }

        [Display(Name = "Points possible")]
        [Required()]
        public double? Points { get; set; }

        public QuizQuestionSettings()
        {
            Points = 0;
        }
    }
}
