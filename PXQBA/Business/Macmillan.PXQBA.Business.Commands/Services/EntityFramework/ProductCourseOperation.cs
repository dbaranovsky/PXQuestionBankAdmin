using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;

namespace Macmillan.PXQBA.Business.Commands.Services.EntityFramework
{
    public class ProductCourseOperation : IProductCourseOperation
    {
        public Course GetCurrentProductCourse()
        {
            return new Course
            {
                ProductCourseId = CacheProvider.GetCurrentTitleId(),
                Title = "Sample title",
                LearningObjectives = new List<LearningObjective>
                {
                    
                    new LearningObjective
                    {
                        Description = "Rhetorical Knowledge",
                        Guid = "9222c505-86f8-4a7c-9b6c-346e601d66b6"
                    },
                    new LearningObjective
                    {
                        Description = "Focus on a purpose",
                        Guid = "e8844d58-5c81-4eaa-897d-adcef44a68c1"
                    },
                    new LearningObjective
                    {
                        Description = "Respond to the needs of different audiences",
                        Guid = "87024fc0-36e3-4468-8162-f81f1590d55e"
                    },
                    new LearningObjective
                    {
                        Description = "Respond appropriately to different kinds of rhetorical situations and added more text to show how long text is wrapped in learning objective field on metadata tab of question editor",
                        Guid = "6344b2e5-1e8d-4585-a50f-22b442692c30"
                    },
                    new LearningObjective
                    {
                        Description = "Use conventions of format and structure appropriate to the rhetorical situation",
                        Guid = "7a5c30a8-78a1-4ddf-9e1d-c2ee6753aac9"
                    }
                }
            };
        }
    }
}
