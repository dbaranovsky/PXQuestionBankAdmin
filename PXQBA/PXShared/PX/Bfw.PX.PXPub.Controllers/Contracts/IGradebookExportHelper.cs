using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Contracts
{
    public interface IGradebookExportHelper
    {
        /// <summary>
        /// Generates CVS string for export
        /// </summary>
        /// <param name="enrollments">Enrollments to be output into CSV string</param>
        /// <returns></returns>
        string GetCsvString(IEnumerable<Enrollment> enrollments);
    }
}
