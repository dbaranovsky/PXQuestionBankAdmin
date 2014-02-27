using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class SharedCourseDefinitionMapper
    {
        /// <summary>
        /// Converts SharedCourseDefinition (Data Contract) to  SharedCourseDefinition (Model)
        /// </summary>
        /// <param name="Biz"></param>
        /// <returns></returns>
        public static SharedCourseDefinition ToSharedCourseDefinition(this BizDC.SharedCourseDefinition Biz)
        {
            var Model = new SharedCourseDefinition();

            if (null != Biz)
            {
                Model.AnonyousName = Biz.AnonyousName;
                Model.IsAnonymous = Biz.IsAnonymous;
                Model.Note = Biz.Note;
                Model.SharedCourseId = Biz.SharedCourseId;
                Model.OwnersUserId = Biz.OwnersUserId;
                Model.OwnerEnrollmentId = Biz.OwnerEnrollmentId;
                Biz.SharedUserIds.ToList().ForEach(userid => Model.SharedUserIds.Add(userid));
                Biz.SharedItemIds.ToList().ForEach(itemid => Model.SharedItemIds.Add(itemid));
            }

            return Model;
        }

        /// <summary>
        /// Converts SharedCourseDefinition (Model) to SharedCourseDefinition (Data Contract)
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public static BizDC.SharedCourseDefinition ToSharedCourseDefinition(this SharedCourseDefinition Model)
        {
            var Biz = new BizDC.SharedCourseDefinition();

            if (null != Model)
            {
                Biz.AnonyousName = Model.AnonyousName;
                Biz.IsAnonymous = Model.IsAnonymous;
                Biz.Note = Model.Note;
                Biz.SharedCourseId = Model.SharedCourseId;
                Biz.OwnersUserId = Model.OwnersUserId;
                Biz.OwnerEnrollmentId = Model.OwnerEnrollmentId;
                Model.SharedUserIds.ToList().ForEach(userid => Biz.SharedUserIds.Add(userid));
                Model.SharedItemIds.ToList().ForEach(itemid => Biz.SharedItemIds.Add(itemid));
            }

            return Biz;
        }
    }
}
