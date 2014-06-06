﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.Commands.DataContracts
{
    public class QuestionSearchResult
    {
        public string QuestionId { get; set; }

        public string SortingField { get; set; }

        public string Index { get; set; }

        public string DraftFrom { get; set; }
    }

    public class ParentQuestion
    {
        public string QuestionId { get; set; }

        public IEnumerable<ParentQuestion> Children { get; set; } 
    }
}
