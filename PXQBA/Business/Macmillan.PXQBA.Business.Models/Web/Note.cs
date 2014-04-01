using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.Models.Web
{
    public class Note
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public bool IsFlagged { get; set; }

        public int QuestionId { get; set; }
    }
}
