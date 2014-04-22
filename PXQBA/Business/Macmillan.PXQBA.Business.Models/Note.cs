namespace Macmillan.PXQBA.Business.Models
{
    public class Note
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public bool IsFlagged { get; set; }

        public string QuestionId { get; set; }
    }
}
