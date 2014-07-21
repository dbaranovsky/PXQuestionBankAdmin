using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Web.ViewModels.Import
{
    public class ImportFromFileViewModel
    {
        public long FileId { get; set; }
        public bool IsImported { get; set; }

        public bool IsNothingToImport { get; set; }

        public string ParsingErrorMessage { get; set; }
        public int QuestionToImport { get; set; }
        public int QuestionSkipped { get; set; }
    }
}
