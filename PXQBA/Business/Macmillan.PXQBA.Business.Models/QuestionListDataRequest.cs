namespace Macmillan.PXQBA.Business.Models
{
    public class QuestionListDataRequest
    {
        public string Query { get; set; }

        public int PageNumber { get; set; }

        public OrderType OrderType { get; set; }

        public string OrderField { get; set; }
    }
}
