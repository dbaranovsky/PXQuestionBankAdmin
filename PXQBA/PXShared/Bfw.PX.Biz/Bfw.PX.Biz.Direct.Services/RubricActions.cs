using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Adc = Bfw.Agilix.DataContracts;
using Bdc = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implements ITaskActions using direct connection to DLAP.
    /// </summary>
    public class RubricActions : IRubricActions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskActions"/> class.
        /// </summary>
        /// <param name="ctx">The IBusinessContext implementation.</param>
        /// <param name="sessionManager">The session manager.</param>
        public RubricActions(IBusinessContext ctx, ISessionManager sessionManager)
        {
            Context = ctx;
            SessionManager = sessionManager;
        }

        #endregion

        #region IRubricActions Members

        /// <summary>
        /// Immediately runs an Agilix task. 
        /// If the task is already running, RunTask returns the running task's status and does not start a new task..
        /// </summary>
        /// <param name="Command">The DLAP task command to execute.</param>
        /// <returns>A DLAP task object.</returns>
        public IEnumerable<Bdc.ContentItem> ListAssociatedItems(String entityId, String rubricUri)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("RubricActions.ListAssociatedItems(entityId={0}, rubricUri={1})", entityId, rubricUri))
            {
                var query = string.Format("/Rubric='{0}'", rubricUri);
                var cmd = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = entityId,
                        Query = query
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (!cmd.Items.IsNullOrEmpty())
                {
                    result = cmd.Items.Map(c => c.ToContentItem(Context));
                }
            }
            return result;
        }

        public IEnumerable<Bdc.ContentItem> ListRubrics(String entityId)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("RubricActions.ListRubrics(entityId={0})", entityId))
            {
                var query = string.Format("/bfw_type='{0}' AND /parent<>'{1}' AND /parent<>'{2}'", "Rubric", "PX_TEMPLATES", "PX_TEMP");
                var cmd = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = entityId,
                        Query = query
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (!cmd.Items.IsNullOrEmpty())
                {
                    result = cmd.Items.Map(c => c.ToContentItem(Context));
                }
            }
            return result;
        }

        public IEnumerable<Bdc.ContentItem> ListContentWithRubrics(String entityId)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("RubricActions.ListContentWithRubrics(entityId={0})", entityId))
            {
                var query = string.Format("/bfw_type<>'{0}' AND /rubric<>''", "rubric");
                var cmd = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = entityId,
                        Query = query
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (!cmd.Items.IsNullOrEmpty())
                {
                    result = cmd.Items.Map(c => c.ToContentItem(Context));
                }
            }
            return result;
        }

        public IEnumerable<Bdc.ContentItem> ListAlignedContent(string entityId, List<string> selectedRubricList)
        {
            IEnumerable<Bdc.ContentItem> result = new List<Bdc.ContentItem>();

            using (Context.Tracer.DoTrace("RubricActions.ListAlignedContent(entityId={0})", entityId))
            {
                var query = GetRubricSearchQuery(selectedRubricList);

                var cmd = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = entityId,
                        Query = query
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (!cmd.Items.IsNullOrEmpty())
                {
                    result = cmd.Items.Map(c => c.ToContentItem(Context));
                }
            }
            return result;
        }

        /// <summary>
        /// Lists the content of the aligned per entityids.
        /// </summary>
        /// <param name="entityIds">The entity ids.</param>
        /// <param name="selectedRubricList">The selected rubric list.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> ListAlignedContent(List<string> entityIds, List<string> selectedRubricList)
        {
            var result = new List<Bdc.ContentItem>();
            var batch = new Batch();
            batch.RunAsync = true;
            foreach (var entityId in entityIds)
            {
                using (Context.Tracer.DoTrace("RubricActions.ListAlignedContent(entityId={0})", entityId))
                {
                    var query = GetRubricSearchQuery(selectedRubricList);
                    batch.Add(new GetItems()
                       {
                           SearchParameters = new Adc.ItemSearch()
                           {
                               EntityId = entityId,
                               Query = query
                           }
                       });
                }
            }
            if (!batch.Commands.IsNullOrEmpty())
            {
                SessionManager.CurrentSession.ExecuteAsAdmin(batch);
                for (int index = 0; index < batch.Commands.Count(); index++)
                {
                    if (!batch.CommandAs<GetItems>(index).Items.IsNullOrEmpty())
                    {
                        var items = batch.CommandAs<GetItems>(index).Items;
                        foreach (var item in items)
                        {
                            var contentItem = item.ToContentItem();
                            result.Add(contentItem);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the rubric search query.
        /// </summary>
        /// <param name="selectedRubricList">The selected rubric list.</param>
        /// <returns></returns>
        private string GetRubricSearchQuery(List<string> selectedRubricList)
        {
            string query = string.Format("/bfw_type<>'{0}'", "Rubric");
            int count = 0;

            foreach (string rubricId in selectedRubricList)
            {
                string rubricPath = "Templates/Data/" + rubricId + "/_rubric.xml";
                if (count == 0)
                {
                    query = query + string.Format(" AND (/rubric='{0}'", rubricPath);
                }
                else
                {
                    query = query + string.Format(" OR /rubric='{0}'", rubricPath);
                }
                count++;
            }

            query = query + ")";
            return query;
        }

        public Bdc.ContentItem GetRubric(String rubricPath)
        {
            Bdc.ContentItem rubric = null;
            using (Context.Tracer.DoTrace("RubricActions.ListAlignedContent(rubricPath={0})", rubricPath))
            {
                var items = ListRubrics(Context.EntityId).ToList();
                rubric = items.Find(r => r.AssignmentSettings.Rubric == rubricPath);
            }
            return rubric;
        }

        /// <summary>
        /// Retrieves list of rubrics and updates to add course activation.
        /// </summary>
        /// <param name="selectedRubricList">Ids of rubrics to add to course.</param>
        /// <returns>Collection of rubrics modified.</returns>
        public IEnumerable<Bdc.ContentItem> AddCourseRubrics(List<string> selectedRubricList)
        {
            List<Bdc.ContentItem> rubrics = null;

            using (Context.Tracer.DoTrace("RubricActions.AddCourseRubrics"))
            {
                if (!selectedRubricList.IsNullOrEmpty())
                {
                    var getBatch = new Batch();
                    var putItems = new PutItems();

                    using (Context.Tracer.DoTrace("Load Rubrics"))
                    {
                        foreach (var rubricId in selectedRubricList)
                        {
                            getBatch.Add(rubricId, new GetItems()
                            {
                                SearchParameters = new Adc.ItemSearch()
                                {
                                    EntityId = Context.EntityId,
                                    ItemId = rubricId
                                }
                            });
                        }

                        SessionManager.CurrentSession.ExecuteAsAdmin(getBatch);
                    }

                    using (Context.Tracer.DoTrace("Modify Rubric Items"))
                    {
                        Bdc.ContentItem current = null;
                        foreach (var rubricId in selectedRubricList)
                        {
                            current = getBatch.CommandAs<GetItems>(rubricId).Items.First().ToContentItem(Context);

                            current.isActiveRubric = true;

                            putItems.Add(current.ToItem());
                        }
                    }

                    using (Context.Tracer.DoTrace("Store Rubrics"))
                    {
                        SessionManager.CurrentSession.ExecuteAsAdmin(putItems);
                    }
                }
            }


            return rubrics;
        }

        /// <summary>
        /// Retrieves list of rubrics and updates to remove course activation.
        /// </summary>
        /// <param name="selectedRubricList">Ids of rubrics to remove from course.</param>
        /// <returns>Collection of rubrics modified.</returns>
        public IEnumerable<Bdc.ContentItem> RemoveCourseRubrics(List<string> selectedRubricList)
        {
            List<Bdc.ContentItem> rubrics = null;

            using (Context.Tracer.DoTrace("RubricActions.RemoveCourseRubrics"))
            {
                if (!selectedRubricList.IsNullOrEmpty())
                {
                    var getBatch = new Batch();
                    var putItems = new PutItems();

                    using (Context.Tracer.DoTrace("Load Rubrics"))
                    {
                        foreach (var rubricId in selectedRubricList)
                        {
                            getBatch.Add(rubricId, new GetItems()
                            {
                                SearchParameters = new Adc.ItemSearch()
                                {
                                    EntityId = Context.EntityId,
                                    ItemId = rubricId
                                }
                            });
                        }

                        SessionManager.CurrentSession.ExecuteAsAdmin(getBatch);
                    }

                    using (Context.Tracer.DoTrace("Modify Rubric Items"))
                    {
                        Bdc.ContentItem current = null;
                        foreach (var rubricId in selectedRubricList)
                        {
                            current = getBatch.CommandAs<GetItems>(rubricId).Items.First().ToContentItem(Context);

                            current.isActiveRubric = false;

                            putItems.Add(current.ToItem());
                        }
                    }

                    using (Context.Tracer.DoTrace("Store Rubrics"))
                    {
                        SessionManager.CurrentSession.ExecuteAsAdmin(putItems);
                    }
                }
            }

            return rubrics;
        }

        #endregion
    }
}