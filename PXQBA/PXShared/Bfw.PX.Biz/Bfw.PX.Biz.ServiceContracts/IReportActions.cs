// -----------------------------------------------------------------------
// <copyright file="IRubricReportActions.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;
namespace Bfw.PX.Biz.ServiceContracts
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IReportActions
    {
        IEnumerable<ContentItem> GetStudentItems(string enrollmentId);
    }
}
