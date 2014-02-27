using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
    public class PageActions : IPageActions
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
        public PageActions(IBusinessContext context, ISessionManager sessionManager)
        {
            this.Context = context;
            this.SessionManager = sessionManager;
        }
        #endregion

        #region IPageActions Members

        /// <summary>
        /// Loads the Page item along with all of its Zones, and Widgets in those Zones.
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public Bdc.PageDefinition LoadPageDefinition(string pageName)
        {
            Bdc.PageDefinition pageResult = null;
            if (string.IsNullOrEmpty(pageName))
            {
                throw new Exception("pageName missing in [PageActions]LoadPageDefinition");
            }

            using (Context.Tracer.DoTrace("PageActions.LoadPageDefinition(pageName={0})", pageName))
            {
                pageResult = Context.CacheProvider.FetchPageDefinition(Context.CourseId, pageName);
                if (pageResult != null)
                {
                    Context.Logger.Log(string.Format("Page Definition {0} loaded from cache", pageName), LogSeverity.Debug);
                }
                else
                {
                    var cmdBatch = new Batch();

                    var searchParams = new Adc.ItemSearch()
                        {
                            EntityId = this.Context.CourseId,
                            ItemId = pageName
                            //,Query = string.Format("/parent = '{0}'", "PX_PAGES")
                        };

                    if (pageName.IsNullOrEmpty())
                    {
                        searchParams.Query = string.Format("/parent = '{0}'", "PX_PAGES");
                    }


                    //var zones = ListZones(pageName);
                    GetItems cmdGetPageDefinition = new GetItems()
                    {

                        SearchParameters = searchParams
                    };

                    cmdBatch.Add(pageName, cmdGetPageDefinition);

                    string zoneListName = GetZoneListName(pageName);
                    GetItems cmdGetZones = new GetItems()
                    {
                        SearchParameters = new Adc.ItemSearch()
                        {
                            EntityId = this.Context.CourseId,
                            Query = string.Format("/parent = '{0}'", zoneListName)
                        }
                    };

                    cmdBatch.Add(zoneListName, cmdGetZones);

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmdBatch);

                    var pageResults = cmdBatch.CommandAs<GetItems>(pageName).Items;
                    if (!pageResults.IsNullOrEmpty())
                    {
                        pageResult = pageResults.First().ToPageDefinition();
                        var zoneResults = cmdBatch.CommandAs<GetItems>(zoneListName).Items;

                        if (!zoneResults.IsNullOrEmpty())
                        {
                            var cmdWidgetBatch = new Batch();

                            foreach (var zone in zoneResults)
                            {
                                pageResult.Zones.Add(zone.ToZoneItem(this));

                                GetItems cmdGetWidget = new GetItems()
                                {
                                    SearchParameters = new Adc.ItemSearch()
                                    {
                                        EntityId = this.Context.CourseId,
                                        Query = string.Format("/parent = '{0}'", zone.Id)
                                    }
                                };
                                cmdWidgetBatch.Add(zone.Id, cmdGetWidget);
                            }

                            SessionManager.CurrentSession.ExecuteAsAdmin(cmdWidgetBatch);

                            foreach (var zone in pageResult.Zones)
                            {
                                var widgetResults = cmdWidgetBatch.CommandAs<GetItems>(zone.Id).Items;
                                if (!widgetResults.IsNullOrEmpty())
                                {
                                    foreach (var widget in widgetResults.OrderBy(SortFunction("sequence")))
                                    {
                                        zone.Widgets.Add(widget.ToWidgetItem());
                                    }
                                }
                            }


                        }

                    }

                    Context.CacheProvider.StorePageDefinition(Context.CourseId, pageResult);
                }
            }

            return pageResult;
        }

        /// <summary>
        /// Loads the Page item along with all of its Zones, and Widgets in those Zones.
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public Bdc.Menu LoadMenu(string menuId)
        {
            Bdc.Menu result = null;
            if (string.IsNullOrEmpty(menuId))
            {
                throw new Exception("menuId missing in [PageActions]LoadMenu");
            }

            using (Context.Tracer.DoTrace("PageActions.LoadmenuId(menuId={0})", menuId))
            {
                result = Context.CacheProvider.FetchMenu(Context.CourseId, menuId);
                if (result != null)
                {
                    Context.Logger.Log(string.Format("Menu {0} loaded from cache", menuId), LogSeverity.Debug);
                }
                else
                {
                    var cmdBatch = new Batch();

                    var cmdMenu = new GetItems()
                    {
                        SearchParameters = new Adc.ItemSearch()
                        {
                            EntityId = this.Context.CourseId,
                            ItemId = menuId
                        }
                    };

                    var cmdMenuItems = new GetItems()
                    {
                        SearchParameters = new Adc.ItemSearch()
                        {
                            EntityId = this.Context.CourseId,
                            Query = string.Format("/parent = '{0}'", menuId)
                        }
                    };

                    string templates = (menuId == "PX_PRIMARY") ? "PX_MENUITEM_TEMPLATES" : menuId + "_TEMPLATES";
                    var cmdTemplate = new GetItems()
                    {
                        SearchParameters = new Adc.ItemSearch()
                        {
                            EntityId = this.Context.CourseId,
                            Query = string.Format("/parent = '{0}'", templates)
                        }
                    };

                    cmdBatch.Add("PX_PRIMARY", cmdMenu);
                    cmdBatch.Add("PX_PRIMARY_MENUITEMS", cmdMenuItems);
                    cmdBatch.Add("PX_MENUITEM_TEMPLATES", cmdTemplate);
                    SessionManager.CurrentSession.ExecuteAsAdmin(cmdBatch);

                    result = new Bdc.Menu() { Id = menuId };
                    var pxPrimayItems = cmdBatch.CommandAs<GetItems>("PX_PRIMARY").Items;
                    if (!pxPrimayItems.IsNullOrEmpty())
                    {
                        foreach (var item in pxPrimayItems.OrderBy(SortFunction("sequence")).ToList())
                        {
                            result = item.ToMenu();
                        }
                    }


                    if (!cmdBatch.CommandAs<GetItems>("PX_PRIMARY_MENUITEMS").Items.IsNullOrEmpty())
                    {
                        
                        foreach (var item in cmdBatch.CommandAs<GetItems>("PX_PRIMARY_MENUITEMS").Items.OrderBy(SortFunction("sequence")).ToList())
                        {
                            result.MenuItems.Add(item.ToPageMenuItem());
                        }
                        
                    }

                    if (!cmdBatch.CommandAs<GetItems>("PX_MENUITEM_TEMPLATES").Items.IsNullOrEmpty())
                    {
                        foreach (var item in cmdBatch.CommandAs<GetItems>("PX_MENUITEM_TEMPLATES").Items.OrderBy(SortFunction("sequence")).ToList())
                        {
                            result.MenuItemTemplates.Add(item.ToPageMenuItem());
                        }
                    }

                    result.FlatCount = (result.MenuItems.IsNullOrEmpty()) ? 0 : result.MenuItems.Count;

                    Context.CacheProvider.StoreMenu(Context.CourseId, result);
                }
            }

            return result;
        }

        public List<Bdc.Zone> ListZones(string pageName)
        {
            List<Bdc.Zone> result = null;
            string zoneListName = GetZoneListName(pageName);

            using (Context.Tracer.DoTrace("PageActions.ListZones(pageName={0})", pageName))
            {
                var cmdZone = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = this.Context.CourseId,
                        Query = string.Format("/parent = '{0}'", zoneListName)
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmdZone);

                if (!cmdZone.Items.IsNullOrEmpty())
                {
                    result = cmdZone.Items.Map(i => i.ToZoneItem(this)).ToList();
                }
            }

            return result;
        }

        /// <summary>
        /// Returns an InstructorConsoleSettings object which is translated from the widget object.
        /// </summary>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        public Bdc.InstructorConsoleSettings GetInstructorConsoleSettings(string widgetId)
        {
            var w = GetWidget(widgetId);            

            return w.InstructorConsoleSettings;
        }

        /// <summary>
        /// Returns an InstructorConsoleSettings.LaunchpadSettings object which is translated from the widget object.
        /// </summary>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        public Bdc.LaunchPadSettings GetInstructorConsoleLaunchPadSettings()
        {
            Bdc.Widget widget = GetWidget(System.Configuration.ConfigurationManager.AppSettings["DefaultLaunchPadWidget"]);
            var model = new Bdc.LaunchPadSettings();
            model.Properties = widget.Properties;
            model.Id = widget.Id;
            model.Id = widget.Id;
            model.ParentId = widget.ParentId;
            model.CourseID = widget.CourseID;
            model.Title = widget.Title;
            model.Sequence = widget.Sequence;
            model.Type = widget.Type;
            model.CategoryName = widget.Properties.ContainsKey("bfw_launchpadtitle") ? widget.Properties["bfw_launchpadtitle"].Value.ToString() : "";
            model.GrayoutPastDueLater = widget.Properties.ContainsKey("bfw_grayoutpastduelater") && Convert.ToBoolean(widget.Properties["bfw_grayoutpastduelater"].Value.ToString());
            model.DueLaterDays = widget.Properties.ContainsKey("bfw_toggleduelaterdays") ? widget.Properties["bfw_toggleduelaterdays"].Value.ToString() : "";
            model.CollapseUnassigned = widget.Properties.ContainsKey("bfw_collapseunassigned") && Convert.ToBoolean(widget.Properties["bfw_collapseunassigned"].Value.ToString());
            model.DisableEditing = widget.Properties.ContainsKey("bfw_disableediting") && Convert.ToBoolean(widget.Properties["bfw_disableediting"].Value.ToString());
            model.DisableDragAndDrop = widget.Properties.ContainsKey("bfw_disabledraganddrop") && Convert.ToBoolean(widget.Properties["bfw_disabledraganddrop"].Value.ToString());
            model.ShowItemsOnly = widget.Properties.ContainsKey("bfw_showitemsonly") ? widget.Properties["bfw_showitemsonly"].Value.ToString() : "";
            if (model.ShowItemsOnly.ToLower().Equals("assigned"))
            {
                model.CollapseUnassigned = true;
            }
            model.CollapseDueLater = widget.Properties.ContainsKey("bfw_toggleduelater") && Convert.ToBoolean(widget.Properties["bfw_toggleduelater"].Value.ToString());
            model.CollapsePastDue = widget.Properties.ContainsKey("bfw_togglepastdue") && Convert.ToBoolean(widget.Properties["bfw_togglepastdue"].Value.ToString());
            model.ShowCollapseUnassigned = widget.Properties.ContainsKey("bfw_showcollapseunassigned") && Convert.ToBoolean(widget.Properties["bfw_showcollapseunassigned"].Value.ToString());
            model.SortByDueDate = widget.Properties.ContainsKey("bfw_sortbyduedate") && Convert.ToBoolean(widget.Properties["bfw_sortbyduedate"].Value.ToString());
            model.CategoryName = widget.Properties.ContainsKey("bfw_launchpadtitle") ? widget.Properties["bfw_launchpadtitle"].Value.ToString() : "";
            model.SplitAssigned = widget.Properties.ContainsKey("bfw_splitassigned") && Convert.ToBoolean(widget.Properties["bfw_splitassigned"].Value.ToString());

            return model;
        }

        public Bdc.Widget GetWidget(string widgetId)
        {
            Bdc.Widget result = Context.CacheProvider.FetchWidget(Context.CourseId, widgetId);

            if (result == null)
            {
                using (Context.Tracer.DoTrace("PageActions.GetWidget(widgetId={0}", widgetId))
                {
                    result = GetWidgetItem(widgetId).ToWidgetItem();
                    Context.CacheProvider.StoreWidget(Context.CourseId, result);
                }
            }
            return result;
        }

        private Adc.Item GetWidgetItem(string widgetId)
        {
            Adc.Item result = null;

            var cmdGetWidget = new GetItems()
            {
                SearchParameters = new Adc.ItemSearch()
                {
                    EntityId = this.Context.CourseId,
                    ItemId = widgetId
                }
            };

            this.SessionManager.CurrentSession.ExecuteAsAdmin(cmdGetWidget);

            if (!cmdGetWidget.Items.IsNullOrEmpty())
            {
                result = cmdGetWidget.Items.First();
            }
            return result;
        }

        public Bdc.NavigationItem GetMenuItem(string menuItemId)
        {
            using (Context.Tracer.DoTrace("PageActions.GetMenuItem(menuItemId={0}", menuItemId))
            {
                Bdc.NavigationItem result = null;

                var cmd = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = this.Context.CourseId,
                        ItemId = menuItemId
                    }
                };

                this.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (!cmd.Items.IsNullOrEmpty())
                {
                    result = cmd.Items.First().ToMenuItem();
                }

                return result;
            }
        }

        /// <summary>
        /// Sorts.
        /// </summary>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        private static Func<Adc.Item, object> SortFunction(string sort)
        {
            Func<Adc.Item, object> func = null;
            func = x => x.Sequence;
            return func;
        }


        /// <summary>
        /// Moves the Widget so that it is in the specified zone at the specified postion.
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="zoneName"></param>
        /// <param name="widgetId"></param>
        /// <param name="minSequence"></param>
        /// <param name="maxSequence"></param>
        /// <returns>the sequence which is assigned to the widget on move</returns>
        public string MoveWidget(string pageName, string zoneName, string widgetId, string minSequence, string maxSequence)
        {
            string newSequence = string.Empty;

            if (string.IsNullOrEmpty(pageName))
            {
                throw new Exception("pageName is missing [PageActions]MoveWidget()");
            }

            if (string.IsNullOrEmpty(zoneName))
            {
                throw new Exception("zoneName is missing [PageActions]MoveWidget()");
            }

            if (string.IsNullOrEmpty(widgetId))
            {
                throw new Exception("widgetId is missing [PageActions]MoveWidget()");
            }

            using (Context.Tracer.DoTrace("PageActions.MoveWidget(pageName={0}, zoneName={1}, widgetId={2})", pageName, zoneName, widgetId))
            {
                var ZoneName = GetZoneName(pageName, zoneName);

                Bdc.Widget widget = GetWidget(widgetId);

                if (widget != null)
                {
                    var cmdGetWidgetTemplate = new GetItems();
                    var isAdd = IsOkToAddWidget(ZoneName, widget.Template, out cmdGetWidgetTemplate);

                    if (isAdd)
                    {
                        //the widget needs to be at the specified zone at specified position
                        widget.ParentId = ZoneName;
                        newSequence = Context.Sequence(minSequence, maxSequence);
                        widget.Sequence = newSequence;

                        var cmdAddWidget = new PutItems();
                        cmdAddWidget.Add(widget.ToItem());

                        this.SessionManager.CurrentSession.ExecuteAsAdmin(cmdAddWidget);
                        Context.CacheProvider.InvalidateWidget(Context.CourseId, widget);
                        Context.CacheProvider.InvalidatePageDefinition(Context.CourseId, pageName);
                    }
                }
            }
            return newSequence;
        }



        /// <summary>
        /// Rename the Course.
        /// </summary>
        /// <param name="courseName"></param>
        public void RenameCourse(string courseName, string pageName)
        {
            if (string.IsNullOrEmpty(courseName))
            {
                throw new Exception("courseName is missing [PageActions]RenameCourse()");
            }

            using (Context.Tracer.DoTrace("PageActions.RenameCourse(courseName={0},pageName={1})", courseName, pageName))
            {
                var cmdGeCourse = new GetCourse()
                {
                    SearchParameters = new Adc.CourseSearch()
                    {
                        CourseId = this.Context.CourseId
                    }
                };
                this.SessionManager.CurrentSession.ExecuteAsAdmin(cmdGeCourse);

                Bdc.Course newCourse = cmdGeCourse.Courses.First().ToCourse();
                newCourse.Title = courseName;

                var cmdUpdateCourse = new UpdateCourses();
                cmdUpdateCourse.Add(newCourse.ToCourse());

                this.SessionManager.CurrentSession.ExecuteAsAdmin(cmdUpdateCourse);
                Context.CacheProvider.InvalidatePageDefinition(Context.CourseId, pageName);
            }
        }

        /// <summary>
        /// Moves the Widget so that it is in the specified zone at the specified postion.
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="zoneName"></param>
        /// <param name="widgetId"></param>
        /// <param name="minSequence"></param>
        /// <param name="maxSequence"></param>
        /// <param name="menuId"></param>
        /// <returns>the sequence which is assigned to the widget on move</returns>
        public string MoveMenuItem(string menuId, string menuItemId, string minSequence, string maxSequence)
        {
            string newSequence = string.Empty;

            if (string.IsNullOrEmpty(menuItemId))
            {
                throw new Exception("pageName is missing [PageActions]MoveWidget()");
            }

            using (Context.Tracer.DoTrace("PageActions.MoveMenuItem(menuId={0},menuItemId={1})", menuId, menuItemId))
            {
                var cmdGeMenuItem = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = this.Context.CourseId,
                        ItemId = menuItemId
                    }
                };

                this.SessionManager.CurrentSession.ExecuteAsAdmin(cmdGeMenuItem);
                if (!cmdGeMenuItem.Items.IsNullOrEmpty())
                {
                    var contentItem = cmdGeMenuItem.Items.First();
                    var menuItem = contentItem.ToMenuItem();
                    var cmdTemplate = new GetItems();

                    newSequence = Context.Sequence(minSequence, maxSequence);
                    contentItem.Sequence = newSequence;

                    var cmdAddWidget = new PutItems();
                    cmdAddWidget.Add(contentItem);

                    this.SessionManager.CurrentSession.ExecuteAsAdmin(cmdAddWidget);
                    Context.CacheProvider.InvalidateMenu(Context.CourseId, menuId);
                }
            }
            return newSequence;
        }

        /// <summary>
        /// Visibility of the widget for various users
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="widgetDisplayOptions"></param>
        public void SetWidgetDisplay(string widgetId, Bdc.WidgetDisplayOptions widgetDisplayOptions)
        {
            if (string.IsNullOrEmpty(widgetId))
            {
                throw new Exception("widgetID is missing in [PageActions]SetWidgetDisplay");
            }

            if (widgetDisplayOptions == null)
            {
                throw new Exception("widgetDisplayOptions is missing in [PageActions]SetWidgetDisplay");
            }

            using (this.Context.Tracer.DoTrace("PageActions.SetWidgetDisplay(widgetId={0}, WidgetDisplayOptions)", widgetId))
            {
                var cmdGetWidget = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = this.Context.CourseId,
                        ItemId = widgetId
                    }
                };

                this.SessionManager.CurrentSession.ExecuteAsAdmin(cmdGetWidget);

                if (cmdGetWidget.Items.IsNullOrEmpty())
                {
                    throw new Exception("Invalid widgetID in [PageActions]SetWidgetDisplay");
                }

                var itemWidget = cmdGetWidget.Items.First();

                var bfwDisplayFlags = itemWidget.Data.Element("bfw_display_flags");

                if (bfwDisplayFlags == null)
                {
                    bfwDisplayFlags = new XElement("bfw_display_flags");
                    itemWidget.Data.Add(bfwDisplayFlags);
                }
                else
                {
                    bfwDisplayFlags.RemoveNodes();
                }

                //remove duplicate display options
                var DisplayOptions = widgetDisplayOptions.DisplayOptions.Distinct();

                foreach (var displayOption in DisplayOptions)
                {
                    bfwDisplayFlags.Add(new XElement("display", displayOption));
                }

                //Update the widget info
                var cmdPutWidget = new PutItems();
                cmdPutWidget.Add(itemWidget);

                this.SessionManager.CurrentSession.ExecuteAsAdmin(cmdPutWidget);
                Context.CacheProvider.InvalidateWidget(Context.CourseId, new Bdc.Widget() { Id = widgetId });
            }
        }

        /// <summary>
        /// Removes the Widget from any Zone it is currently in.
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="pageName"></param>
        public void RemoveWidget(string widgetId, string pageName)
        {
            if (string.IsNullOrEmpty(widgetId))
            {
                throw new Exception("widgetID is missing [PageActions]RemoveWidget()");
            }

            using (this.Context.Tracer.DoTrace("PageActions.RemoveWidget(widgetID={0},pageName={1}", widgetId, pageName))
            {
                var cmdDelete = new DeleteItems()
                {
                    Items = new List<Adc.Item>()
                    {
                        new Adc.Item()
                        {
                            EntityId = this.Context.CourseId,
                            Id = widgetId                            
                        }
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmdDelete);
                Context.CacheProvider.InvalidatePageDefinition(Context.CourseId, pageName);
            }
        }

        /// <summary>
        /// Removes the menu Item from any Menu it is currently in.
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="menuId"></param>
        public void RemoveMenuItem(string menuId, string menuItemId)
        {
            if (string.IsNullOrEmpty(menuItemId))
            {
                throw new Exception("menuItemId is missing [PageActions]RemoveMenuItem()");
            }

            using (this.Context.Tracer.DoTrace("PageActions.RemoveMenuItem(menuId={0},menuItemId={1})", menuId, menuItemId))
            {
                var cmdDelete = new DeleteItems()
                {
                    Items = new List<Adc.Item>()
                    {
                        new Adc.Item()
                        {
                            EntityId = this.Context.CourseId,
                            Id = menuItemId                            
                        }
                    }
                };

                SessionManager.CurrentSession.Execute(cmdDelete);
                Context.CacheProvider.InvalidateMenu(Context.CourseId, menuId);
            }
        }

        /// <summary>
        /// This method will create a copy of the specified widget and puts it in the correct page zone.
        /// The newly created widget will be returned.
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="zoneId"></param>
        /// <param name="widgetTemplateId"></param>
        /// <param name="minSequence"></param>
        /// <param name="maxSequence"></param>
        /// <returns></returns>
        public DataContracts.Widget AddWidget(string pageName, string zoneId, string widgetTemplateId, string minSequence, string maxSequence, string newWidgetId)
        {
            Bdc.Widget widget = null;

            if (string.IsNullOrEmpty(pageName))
            {
                throw new Exception("pageName is missing [PageActions]AddWidget()");
            }

            if (string.IsNullOrEmpty(zoneId))
            {
                throw new Exception("zoneId is missing [PageActions]AddWidget()");
            }

            if (string.IsNullOrEmpty(widgetTemplateId))
            {
                throw new Exception("widgetTemplateId is missing [PageActions]AddWidget()");
            }

            using (this.Context.Tracer.DoTrace("PageActions.AddWidget(pageName={0}, zoneID={1}, widgetTemplateID={2}", pageName, zoneId, widgetTemplateId))
            {
                var ZoneName = GetZoneName(pageName, zoneId);
                var cmdGetWidgetTemplate = new GetItems();
                var isAdd = IsOkToAddWidget(ZoneName, widgetTemplateId, out cmdGetWidgetTemplate);

                Adc.Item itemWidget = null;

                if (isAdd)
                {
                    if (!cmdGetWidgetTemplate.Items.IsNullOrEmpty())
                    {

                        if (newWidgetId == "NotKnownYet")
                        {
                            var cmdAddWidget = new PutItems();
                            itemWidget = cmdGetWidgetTemplate.Items.First();
                            itemWidget.Id = Guid.NewGuid().ToString();
                            itemWidget.ParentId = ZoneName;
                            itemWidget.Sequence = Context.Sequence(minSequence, maxSequence);

                            cmdAddWidget.Add(itemWidget);
                            this.SessionManager.CurrentSession.ExecuteAsAdmin(cmdAddWidget);
                        }
                        else
                        {
                            itemWidget = GetCustomWidgetTemplate(newWidgetId).Items.First();
                        }
                        widget = itemWidget.ToWidgetItem();

                        Context.CacheProvider.InvalidatePageDefinition(Context.CourseId, pageName);
                        Context.CacheProvider.InvalidateWidget(Context.CourseId, new Bdc.Widget() { Id = newWidgetId });
                    }
                }
                else
                {
                    if (newWidgetId != "NotKnownYet")
                    {
                        itemWidget = GetCustomWidgetTemplate(newWidgetId).Items.First();
                        widget = itemWidget.ToWidgetItem();
                    }
                }
            }

            return widget;
        }

        public DataContracts.Widget AddWidget(string pageName, string zoneId, string templateId, string minSequence, string maxSequence, string title, IDictionary<string, DataContracts.PropertyValue> properties)
        {
            Bdc.Widget widget = null;

            if (string.IsNullOrEmpty(pageName))
            {
                throw new ArgumentException("pageName is missing [PageActions]AddWidget()");
            }

            if (string.IsNullOrEmpty(zoneId))
            {
                throw new ArgumentException("zoneId is missing [PageActions]AddWidget()");
            }

            if (string.IsNullOrEmpty(templateId))
            {
                throw new ArgumentException("templateId is missing [PageActions]AddWidget()");
            }

            using (this.Context.Tracer.DoTrace("PageActions.AddWidget(pageName={0}, zoneID={1}, templateId={2}", pageName, zoneId, templateId))
            {
                var ZoneName = GetZoneName(pageName, zoneId);
                var cmdGetWidgetTemplate = new GetItems();
                var isOkToAdd = IsOkToAddWidget(ZoneName, templateId, out cmdGetWidgetTemplate);

                if (isOkToAdd && !cmdGetWidgetTemplate.Items.IsNullOrEmpty())
                {
                    widget = cmdGetWidgetTemplate.Items.First().ToWidgetItem();
                    widget.Id = Context.NewItemId();
                    widget.Template = templateId;
                    widget.ParentId = ZoneName;
                    widget.Sequence = Context.Sequence(minSequence, maxSequence);

                    if (properties != null)
                    {
                        foreach (var prop in properties)
                        {
                            widget.Properties[prop.Key] = prop.Value;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        widget.Title = title;
                    }

                    var cmdAddWidget = new PutItems();
                    cmdAddWidget.Add(widget.ToItem());

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmdAddWidget);
                    Context.CacheProvider.InvalidatePageDefinition(Context.CourseId, pageName);
                    Context.CacheProvider.InvalidateWidget(Context.CourseId, widget);
                }
            }

            return widget;
        }


        /// <summary>
        /// Stores the updated new widget object
        /// </summary>
        /// <param name="w"></param>
        public void UpdateWidget(DataContracts.Widget w)
        {
            var cmdAddWidget = new PutItems();
            cmdAddWidget.Add(w.ToItem());
            SessionManager.CurrentSession.ExecuteAsAdmin(cmdAddWidget);
            Context.CacheProvider.InvalidateWidget(Context.CourseId, w);
        }

        public void EmptySettingsCache(string id)
        {
            var widget = GetWidget(id);
            if (widget == null || widget.ParentId.IsNullOrEmpty())
            {
                return;
            }

            string parentId = widget.ParentId;
            while (!parentId.IsNullOrEmpty())
            {
                if (parentId.Equals("PX_PAGES"))
                {
                    CacheHelper.InvalidatePageDefinition(Context.CacheProvider, Context.CourseId, widget.Id);
                    CacheHelper.InvalidatePageDefinition(Context.CacheProvider, Context.CourseId, Context.Course.CourseHomePage);
                    return;
                }
                else
                {
                    widget = GetWidget(parentId);
                    parentId = (widget == null) ? "" : widget.ParentId;
                }
            }
        }

        /// <summary>
        /// This method will update an exiting widget by using the given property values.
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public DataContracts.Widget UpdateWidget(string pageName, string widgetId, string title, IDictionary<string, DataContracts.PropertyValue> properties)
        {
            Bdc.Widget widget = null;

            if (string.IsNullOrEmpty(widgetId))
            {
                throw new ArgumentException("widgetId is missing [PageActions]UpdateWidget()");
            }

            using (this.Context.Tracer.DoTrace("PageActions.UpdateWidget(pageName={0},widgetId={1})", pageName, widgetId))
            {
                var getItem = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = Context.EntityId,
                        ItemId = widgetId
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(getItem);

                if (!getItem.Items.IsNullOrEmpty())
                {
                    widget = getItem.Items.FirstOrDefault().ToWidgetItem();

                    if (properties != null)
                    {
                        //widget.Properties.Clear();
                        foreach (var prop in properties)
                        {
                            widget.Properties[prop.Key] = prop.Value;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        widget.Title = title;
                    }

                    var cmdAddWidget = new PutItems();
                    cmdAddWidget.Add(widget.ToItem());

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmdAddWidget);
                    Context.CacheProvider.InvalidateWidget(Context.CourseId, widget);
                    Context.CacheProvider.InvalidatePageDefinition(Context.CourseId, pageName);
                }
            }

            return widget;
        }

        /// <summary>
        /// This method will create a copy of the specified widget and puts it in the correct page zone.
        /// The newly created widget will be returned.
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="zoneId"></param>
        /// <param name="widgetTemplateId"></param>
        /// <param name="minSequence"></param>
        /// <param name="maxSequence"></param>
        /// <returns></returns>
        //public DataContracts.MenuItem AddMenuItem(string menuId, string menuItemId, string templateId, string title, string url, string minSequence, string maxSequence, IDictionary parameters)
        public DataContracts.MenuItem AddMenuItem(string menuId, Bdc.MenuItem sourceMenuItem, IDictionary parameters)
        {
            Bdc.MenuItem retVal = null;

            if (string.IsNullOrEmpty(sourceMenuItem.BfwMenuCreatedby))
            {
                throw new Exception("templateId is missing [PageActions]AddMenuItem()");
            }

            using (this.Context.Tracer.DoTrace("PageActions.AddMenuItem(menuId={0},title={1})", menuId, sourceMenuItem.Title))
            {
                var cmdGetTemplate = new GetItems();
                var isAdd = IsAddMenuItem(sourceMenuItem.BfwMenuCreatedby, out cmdGetTemplate);

                if (isAdd)
                {
                    if (!cmdGetTemplate.Items.IsNullOrEmpty())
                    {
                        var templateItem = cmdGetTemplate.Items.First();
                        templateItem.Id = sourceMenuItem.Id.IsNullOrEmpty() ? Guid.NewGuid().ToString() : sourceMenuItem.Id;
                        templateItem.ParentId = menuId;
                        if (!string.IsNullOrEmpty(sourceMenuItem.Title))
                        {
                            templateItem.Title = sourceMenuItem.Title;
                        }

                        templateItem.Sequence = sourceMenuItem.Sequence;

                        var bfw_menu_created_by = templateItem.Data.Descendants("bfw_menu_created_by").FirstOrDefault();
                        if (null != bfw_menu_created_by)
                        {
                            bfw_menu_created_by.Value = sourceMenuItem.BfwMenuCreatedby;
                        }
                        else
                        {
                            bfw_menu_created_by = new XElement("bfw_menu_created_by");
                            bfw_menu_created_by.Value = sourceMenuItem.BfwMenuCreatedby;
                            templateItem.Data.Add(bfw_menu_created_by);
                        }

                        var CallbackMethods = templateItem.Data.Descendants("bfw_menu_callbacks").Descendants("method");
                        if (!sourceMenuItem.Callbacks.First().Value.Url.IsNullOrEmpty())
                        {
                            foreach (XElement callbackMethod in CallbackMethods)
                            {
                                callbackMethod.Attribute("name").Value = "";
                                callbackMethod.Attribute("controller").Value = "";
                                callbackMethod.Attribute("action").Value = "";
                                callbackMethod.Attribute("type").Value = "";
                                callbackMethod.Attribute("route").Value = "";
                                callbackMethod.Attribute("url").Value = sourceMenuItem.Callbacks.First().Value.Url;
                            }
                        }

                        if (parameters.Count > 0)
                        {
                            foreach (XElement callbackMethod in CallbackMethods)
                            {
                                var subParams = callbackMethod.Elements("parameter");

                                foreach (var subParam in subParams)
                                {
                                    subParam.Remove();
                                }

                                foreach (var param in parameters.Keys)
                                {
                                    var paramNode = new XElement("parameter");
                                    paramNode.Add(new XAttribute("name", param.ToString()));
                                    paramNode.Add(new XAttribute("value", parameters[param].ToString()));
                                    callbackMethod.Add(paramNode);
                                }
                            }
                        }

                        var bfwDisplayFlags = templateItem.Data.Element("bfw_display_flags");

                        if (bfwDisplayFlags == null)
                        {
                            bfwDisplayFlags = new XElement("bfw_display_flags");
                            templateItem.Data.Add(bfwDisplayFlags);
                        }
                        else
                        {
                            bfwDisplayFlags.RemoveNodes();
                        }

                        //remove duplicate display options
                        var DisplayOptions = sourceMenuItem.WidgetDisplayOptions.DisplayOptions.Distinct();

                        foreach (var displayOption in DisplayOptions)
                        {
                            var allow = new XElement("allow");
                            allow.Add(new XElement("role", displayOption));
                            allow.Add(new XElement("coursetype", "any"));
                            bfwDisplayFlags.Add(allow);
                        }

                        var cmdAdd = new PutItems();
                        cmdAdd.Add(templateItem);

                        this.SessionManager.CurrentSession.Execute(cmdAdd);
                        retVal = templateItem.ToPageMenuItem();

                        Context.CacheProvider.InvalidateMenu(Context.CourseId, menuId);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Returns the Dlap Page Folder name given the page name
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        protected string GetPageName(string pageName)
        {
            var PageName = string.Format("PX_{0}", pageName);
            return PageName;
        }

        /// <summary>
        /// Returns DLAP ZoneList Folder given the page name
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        protected string GetZoneListName(string pageName)
        {
            string localPageName = pageName.Replace("PX_", "");
            var zoneListName = string.Format("PX_{0}_Zones", localPageName);
            return zoneListName;
        }

        /// <summary>
        /// Get the DLAP Zone folder name given the page name and zone name
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        protected string GetZoneName(string pageName, string zoneName)
        {
            string ZoneName = "";

            if (zoneName.ToLowerInvariant().StartsWith(pageName.ToLowerInvariant()) || zoneName.ToLowerInvariant().StartsWith(string.Format("px_{0}", pageName.ToLowerInvariant())))
            {
                ZoneName = zoneName;
            }
            else
            {
                ZoneName = string.Format("PX_{0}_{1}", pageName, zoneName);
            }

            return ZoneName;
        }


        /// <summary>
        /// Get the Widget Template 
        /// </summary>
        /// <param name="widgetTemplateID"></param>
        /// <returns></returns>
        public DataContracts.Widget GetWidgetTemplate(string widgetTemplateId)
        {
            using (this.Context.Tracer.DoTrace("PageActions.GetWidgetTemplate(widgetTemplateId={0})", widgetTemplateId))
            {
                var cmdBatch = new Batch();
                var cmdGetWidgetTemplate = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = Context.CourseId,
                        ItemId = widgetTemplateId
                    }
                };
                cmdBatch.Add(cmdGetWidgetTemplate);
                SessionManager.CurrentSession.ExecuteAsAdmin(cmdBatch);
                return cmdGetWidgetTemplate.Items[0].ToWidgetItem();
            }
        }

        /// <summary>
        /// Get the Widget Template 
        /// </summary>
        /// <param name="widgetTemplateID"></param>
        /// <returns></returns>
        protected GetItems GetCustomWidgetTemplate(string widgetTemplateID)
        {
            using (this.Context.Tracer.DoTrace("PageActions.GetCustomWidgetTemplate(widgetTemplateID={0})", widgetTemplateID))
            {
                var cmdBatch = new Batch();
                var cmdGetWidgetTemplate = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = Context.CourseId,
                        ItemId = widgetTemplateID
                    }
                };
                cmdBatch.Add(cmdGetWidgetTemplate);
                SessionManager.CurrentSession.ExecuteAsAdmin(cmdBatch);
                return cmdGetWidgetTemplate;
            }
        }

        /// <summary>
        /// Determines whether a widget can be added.
        /// </summary>
        /// <param name="zoneName"></param>
        /// <param name="widgetTemplateID"></param>
        /// <returns></returns>
        protected bool IsOkToAddWidget(string zoneName, string widgetTemplateID, out GetItems cmdWidgetTemplate)
        {
            using (this.Context.Tracer.DoTrace("PageActions.IsOkToAddWidget(zoneName={0},widgetTemplateID={1})", zoneName, widgetTemplateID))
            {
                var isAdd = false;
                var IsWidgetAllowed = false;
                var IsMultipleAllowed = false;
                var IsTitleHidden = false;
                var IsWidgetExists = false;

                var cmdBatch = new Batch();

                var cmdGetZone = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = this.Context.CourseId,
                        ItemId = zoneName
                    }
                };
                cmdBatch.Add(cmdGetZone);

                var cmdGetWidget = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = Context.CourseId,
                        Query = string.Format("/parent='{0}' and /bfw_widget_template='{1}'", zoneName, widgetTemplateID)
                    }
                };
                cmdBatch.Add(cmdGetWidget);

                var cmdGetWidgetTemplate = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = Context.CourseId,
                        ItemId = widgetTemplateID
                    }
                };
                cmdBatch.Add(cmdGetWidgetTemplate);

                SessionManager.CurrentSession.ExecuteAsAdmin(cmdBatch);

                //check if the widget is allowed in the zone
                if (!cmdGetZone.Items.IsNullOrEmpty())
                {
                    var zone = cmdGetZone.Items.First().ToZoneItem(this);

                    var allowedWidget = (from widget in zone.AllowedWidgets
                                         where widget.widgetType.ToUpper() == widgetTemplateID.ToUpper()
                                         select widget.widgetType).FirstOrDefault();

                    if (!allowedWidget.IsNullOrEmpty())
                    {
                        IsWidgetAllowed = true;
                    }
                }

                //check if the widget already exists in the zone            
                if (!cmdGetWidget.Items.IsNullOrEmpty())
                {
                    IsWidgetExists = true;
                }

                //check if multiple widgets are allowed
                cmdWidgetTemplate = cmdGetWidgetTemplate;
                if (!cmdGetWidgetTemplate.Items.IsNullOrEmpty())
                {
                    var widgetTemplateItem = cmdGetWidgetTemplate.Items.First().ToWidgetItem();
                    if (widgetTemplateItem.IsMultipleAllowed)
                    {
                        IsMultipleAllowed = true;
                    }
                }

                if (IsWidgetExists && cmdGetWidget.Items[0].ParentId == zoneName) // this is to handle the case when widget is moved within the same zone, i.e reordered
                {
                    isAdd = true;
                }

                if (IsWidgetAllowed && (IsMultipleAllowed || !IsWidgetExists))
                {
                    isAdd = true;
                }

                return isAdd;
            }
        }

        /// <summary>
        /// Determine whether to add a menu item or not.
        /// </summary>
        /// <param name="zoneName"></param>
        /// <param name="widgetTemplateID"></param>
        /// <param name="cmdWidgetTemplate"></param>
        /// <returns></returns>
        protected bool IsAddMenuItem(string menuItemTemplateID, out GetItems cmdTemplate)
        {
            using (this.Context.Tracer.DoTrace("PageActions.IsAddMenuItem(menuItemTemplateID={0})", menuItemTemplateID))
            {
                var isAdd = false;

                var cmdGetMenuItem = new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = Context.CourseId,
                        ItemId = menuItemTemplateID
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmdGetMenuItem);

                cmdTemplate = cmdGetMenuItem;

                //check if the menu item is allowed in the zone
                if (!cmdGetMenuItem.Items.IsNullOrEmpty())
                {
                    isAdd = true;
                }

                return isAdd;
            }
        }
        #endregion
    }
}
