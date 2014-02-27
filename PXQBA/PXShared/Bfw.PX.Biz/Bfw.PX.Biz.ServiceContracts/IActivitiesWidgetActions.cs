using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bdc = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contract for ActivitiesWidget actions.
    /// </summary>
    public interface IActivitiesWidgetActions
    {
        /// <summary>
        /// Retrieving a list of activities from DLAP by the specific contentTypeFilter. 
        /// If enrollmentId is specified, will check for student submission against each item.
        /// </summary>
        /// <param name="contentTypeFilter">The activity content type to search for.</param>
        /// <param name="enrollmentId">If specified, will check for student submission against each item.</param>
        Bdc.ActivitiesWidgetResults LoadActivitiesByType(string contentTypeFilter, string enrollmentId = null);
    }
}
