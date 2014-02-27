using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class CourseAcademicTermMapper
    {
        public static CourseAcademicTerm ToAcademicTerm(this BizDC.CourseAcademicTerm biz)
        {
            return new CourseAcademicTerm
            {
                Id = biz.Id,
                Name = biz.Name,
                StartDate = biz.StartDate,
                EndDate = biz.EndDate
            };
        }
    }
}
