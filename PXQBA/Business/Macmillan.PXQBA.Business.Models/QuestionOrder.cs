using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Macmillan.PXQBA.Business.Models
{
    public class QuestionOrder 
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderType OrderType { get; set; }

        public string OrderField { get; set; }
    }
}
