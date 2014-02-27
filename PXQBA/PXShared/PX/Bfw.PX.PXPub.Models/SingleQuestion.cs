
namespace Bfw.PX.PXPub.Models
{
    public class SingleQuestion
    {
        public string QuizId    { get; set; }
        public string QuestionId { get; set; }
        public string EntityId { get; set; }
        public bool? AllowSelection{ get; set; }
        public bool? AllowDrag{ get; set; }
        public bool? ShowExpand{ get; set; }
        public bool? ShowAddLink{ get; set; }
        public bool? ShowPoints{ get; set; }
        public string ExtraClass{ get; set; }
        public bool? IsOdd{ get; set; }
        public bool? IsReused{ get; set; }
        public string QuestionEditedType{ get; set; }
        public bool? IsPrimary{ get; set; }
        public QuizBrowserMode? Mode{ get; set; }
        public bool? IsQuestionOverview{ get; set; }
        public string MainQuizId{ get; set; }
        public bool? IsPoolQuestion{ get; set; }
        public bool? ShowReused { get; set; }
        public string QuestionNumber { get; set; }
    }
}
