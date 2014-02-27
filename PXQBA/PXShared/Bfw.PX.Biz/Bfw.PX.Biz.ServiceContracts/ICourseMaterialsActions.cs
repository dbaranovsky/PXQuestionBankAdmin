using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// provides functionality to manage course materials in eportfolio courses
    /// </summary>
    public interface ICourseMaterialsActions
    {

        /// <summary>
        /// Retrieves the course materials for the current course 
        /// </summary>
        /// <returns></returns>
        CourseMaterials GetCourseMaterials();

        /// <summary>
        /// Deletes a course material resource from an eportfolio course
        /// </summary>
        /// <param name="itemid"></param>
        /// <returns></returns>
        bool DeleteCourseMaterialResource(string itemID);
    }
}
