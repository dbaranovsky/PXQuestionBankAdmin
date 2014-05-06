using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Question DTO
    /// </summary>
    public class QuestionViewModel : Question
    {

        public string ActionPlayerUrl { get; set; }
        public string EditorUrl { get; set; }

    }
}
