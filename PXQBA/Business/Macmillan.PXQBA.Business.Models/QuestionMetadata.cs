using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    public class QuestionMetadata
    {
        public QuestionMetadata()
        {
            Data = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Data { get; set; } 
    }
}
