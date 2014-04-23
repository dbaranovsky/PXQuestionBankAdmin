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
                },

                QuestionCardLayout = @"<script id=""questioncard-template"" type=""text/x-handlebars-template"">
                                            <div class=""card"">
                                            <div class=""questioncardrow firstrow"">
                                            {{#if difficulty}}
                                            <div class=""difficulty"">
                                            <h1>Difficulty:</h1>
                                            <div class=""content"">{{difficulty}}</div>
                                            </div>
                                            {{/if}} {{#if cognitivelevel}}
                                            <div class=""cognitivelevel"">
                                            <h1>Cognitive level:</h1>
                                            <div class=""content"">{{cognitivelevel}}</div>
                                            </div>
                                            {{/if}} {{#if bloomdomain}}
                                            <div class=""bloomdomain"">
                                            <h1>Bloom's domain:</h1>
                                            <div class=""content"">{{bloomdomain}}</div>
                                            </div>
                                            {{/if}}
                                            </div>
                                            <div class=""questioncardrow"">
                                            {{#if coreconcept}}
                                            <div class=""coreconcept"">
                                            <h1>Core Concept:</h1>
                                            <div class=""content"">{{coreconcept}}</div>
                                            </div>
                                            {{/if}} {{#if relatedcontent}}
                                            <div class=""relatedcontent"">
                                            <h1>Related Content:</h1>
                                            <div class=""content"">{{relatedcontent}}</div>
                                            </div>
                                            {{/if}}
                                            </div>
                                            <div class=""questioncardrow"">
                                            {{#if guidance}}
                                            <div class=""guidance"">
                                            <h1>Guidance:</h1>
                                            <div class=""content"">{{guidance}}</div>
                                            </div>
                                            {{/if}}
                                            </div>
                                            <div class=""questioncardrow"">
                                            {{#if freeresponsequestion}}
                                            <div class=""freeresponsequestion"">
                                            <h1>Free response question:</h1>
                                            <div class=""content"">{{freeresponsequestion}}</div>
                                            </div>
                                            {{/if}}
                                            </div>
                                            <div class=""questioncardrow"">
                                            {{#if suggesteduse}}
                                            <div class=""suggesteduse"">
                                            <h1>Suggested use:</h1>
                                            <div class=""content"">{{suggesteduse}}</div>
                                            </div>
                                            {{/if}}
                                            </div>
                                            </div>
                                            </script>"
            };
        }
    }
}
