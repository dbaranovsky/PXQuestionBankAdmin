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
    public static class AssignTabSettingsMapper
    {
        public static AssignTabSettings ToAssignTabSettings(this BizDC.AssignTabSettings biz)
        {
            var model = new AssignTabSettings();

            if(biz != null)
            {
                model.ShowAllowLateSubmissions = biz.ShowAllowLateSubmissions;
                model.ShowAssignedSameDay = biz.ShowAssignedSameDay;
                model.ShowGradebookCategory = biz.ShowGradebookCategory;
                model.ShowMakeGradeable = biz.ShowMakeGradeable;
                model.ShowPointsPossible = biz.ShowPointsPossible;
                model.ShowScheduleReminder = biz.ShowScheduleReminder;
                model.ShowSendReminder = biz.ShowSendReminder;
                model.ShowSubContentCreation = biz.ShowSubContentCreation;
            	model.ShowIncludeScore = biz.ShowIncludeScore;
            	model.ShowCalculationType = biz.ShowCalculationType;
            	model.ShowMarkAsComplete = biz.ShowMarkAsComplete;
                model.ShowCompletionCategory = biz.ShowCompletionCategory;
            }

            return model;
        }

        public static BizDC.AssignTabSettings ToAssignTabSettings(this AssignTabSettings model)
        {
            var biz = new BizDC.AssignTabSettings();

            if (model != null)
            {
                biz.ShowAllowLateSubmissions = model.ShowAllowLateSubmissions;
                biz.ShowAssignedSameDay = model.ShowAssignedSameDay;
                biz.ShowGradebookCategory = model.ShowGradebookCategory;
                biz.ShowMakeGradeable = model.ShowMakeGradeable;
                biz.ShowPointsPossible = model.ShowPointsPossible;
                biz.ShowScheduleReminder = model.ShowScheduleReminder;
                biz.ShowSendReminder = model.ShowSendReminder;
                biz.ShowSubContentCreation = model.ShowSubContentCreation;
            	biz.ShowIncludeScore = model.ShowIncludeScore;
            	biz.ShowCalculationType = model.ShowCalculationType;
            }

            return biz;

        }
    }
}
