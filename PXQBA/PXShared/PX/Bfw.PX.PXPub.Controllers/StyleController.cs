using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using Bfw.Agilix.DataContracts;
using Bfw.Common;
using Bfw.Common.HttpModules.Configuration.ResourceCompression;
using Bfw.Common.Web;
using Bfw.Common.Logging;
using Bfw.Common.HttpModules.ResourceCompression;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Components;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using BizDC = Bfw.PX.Biz.DataContracts;

using Yahoo.Yui.Compressor;
using dotless.Core;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class StyleController : Controller
    {
        /// <summary>
        /// A protected Variable for the Default Layout
        /// </summary>
        protected string DefaultLayout = "DefaultLayout";

        /// <summary>
        /// A protected Variable for the Course Layout.
        /// </summary>
        protected string CourseLayout = "course";

        /// <summary>
        /// A protected Variable for the Widget Directory.
        /// </summary>
        protected string WidgetDirectory = "widgets";

        /// <summary>
        /// A protected Variable for the Base Local Directory.
        /// </summary>
        protected string BaseLocalDirectory = "Content";

        /// <summary>
        /// A protected Variable for the Base Remote Directory.
        /// </summary>
        protected string BaseRemoteDirectory = "Assets";

        /// <summary>
        /// Brainhoney Location for images
        /// </summary>
        protected string BrainHoneyLocation = "";


        protected Bfw.Common.Caching.ICacheProvider Cache { get; set; }

        /// <summary>
        /// A protected Variable for the variable Use Local Files.
        /// </summary>
        protected bool UseLocalFiles = false;

        private string Theme { get; set; }

        private void Configure()
        {
            var bld = ConfigurationManager.AppSettings["BaseLocalDirectory"];
            var brd = ConfigurationManager.AppSettings["BaseRemoteDirectory"];
            var ulf = ConfigurationManager.AppSettings["UseLocalFiles"];
            var wd = ConfigurationManager.AppSettings["WidgetDirectory"];
            var dl = ConfigurationManager.AppSettings["DefaultLayoutName"];
            var cl = ConfigurationManager.AppSettings["CourseLayoutName"];
            var bh = ConfigurationManager.AppSettings["BHLoc"];

            if (!string.IsNullOrEmpty(bld))
            {
                BaseLocalDirectory = bld;
            }

            if (!string.IsNullOrEmpty(brd))
            {
                BaseRemoteDirectory = brd;
            }

            if (!string.IsNullOrEmpty(wd))
            {
                WidgetDirectory = wd;
            }

            if (!string.IsNullOrEmpty(dl))
            {
                DefaultLayout = dl;
            }

            if (!string.IsNullOrEmpty(cl))
            {
                CourseLayout = cl;
            }

            bool val = false;
            if (Boolean.TryParse(ulf, out val))
            {
                UseLocalFiles = val;
            }

            BrainHoneyLocation = string.IsNullOrEmpty(bh)?"":bh;
        }

        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Gets or sets the course actions.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected Bfw.Common.Logging.ITraceManager Tracer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleController"/> class.
        /// </summary>
        /// <param name="bizContext">The biz context.</param>
        /// <param name="contentActions">The content actions.</param>
        public StyleController(Bfw.Common.Caching.ICacheProvider cache, Bfw.Common.Logging.ITraceManager tracer, BizSC.ICourseActions courseActions)
        {
            Cache = cache;
            Tracer = tracer;
            CourseActions = courseActions;
        }

        /// <summary>
        /// Indexes the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// 
        [OutputCache(Duration = 15552000, VaryByCustom = "debug", VaryByParam = "*")]
        public ActionResult Index(string path, string theme = "", string type = "")
        {
            var result = new ContentResult();
            result.ContentType = "text/css";
            result.ContentEncoding = Encoding.UTF8;

            Configure();
            Theme = theme;

            if (string.IsNullOrEmpty(path))
            {
                var courseId = RouteData.Values["courseid"] != null ? RouteData.Values["courseid"].ToString() : "";
                //var key = string.Format("PX_CSS_{0}", Request.Url.PathAndQuery);


                var cssContent = Cache.FetchCssFile(Request.Url.PathAndQuery, courseId);

                if (cssContent == null)
                {
                    cssContent = LoadCSS(type);

                    if (!ResourceEngine.IsDebug())
                    {
                        Cache.StoreCssFile(Request.Url.PathAndQuery, courseId, cssContent);
                    }

                }
                result.Content = cssContent;
            }
            else
            {
                var fileType = Path.GetExtension(path);
                //for CSS file, parse them to replace image paths
                //CSS files should be loaded directly only from localhost (development)
                if (fileType == ".css" || fileType == ".less")
                {
                    result.Content = LocalCSSFile(path);

                }
                else
                {
                    return LoadFile(path);
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the compressed css file for this course
        /// </summary>
        /// <param name="theme"></param>
        ///  <param name="courseProductName"></param>
        ///  <param name="courseType"></param>
        /// <returns></returns>
        /// 
        [OutputCache(Duration = 15552000, VaryByCustom = "debug", VaryByParam = "*")]
        public ActionResult CourseCss(string theme, string courseProductName = "", string courseType = "", string component = "")
        {
            ContentResult result = new ContentResult();
            result.ContentType = "text/css";
            result.ContentEncoding = Encoding.UTF8;

            Configure();
            Theme = theme;

            var courseId = RouteData.Values["courseid"] != null ? RouteData.Values["courseid"].ToString() : "";
            
            var cachedCss = Cache.FetchCssFile(Request.Url.PathAndQuery, courseId);

            if (cachedCss == null)
            {
              
                var css = new StringBuilder();

                // update file lookup strings based off component being rendered
                if (!String.IsNullOrWhiteSpace(component))
                {
                    component = component.ToLowerInvariant();
                    courseType = String.Format("component/{0}/{1}", component, courseType);
                }

                //if product-specific layout css doesnt exist, load generic product CSS
                string courseTypeCss = LocalCSSFile(courseType);

                string courseProductCss = GetProductCss(courseProductName, courseType, component);


                css.AppendLine("/**         <type> **/"); css.AppendLine(courseTypeCss);

                using (Tracer.DoTrace("Compress CSS"))
                {
                    result.Content = CssCompressor.Compress(css.ToString(), 200, CssCompressionType.StockYuiCompressor, false);
                }
                if (!ResourceEngine.IsDebug())
                {
                    Cache.StoreCssFile(Request.Url.PathAndQuery, courseId, result.Content);
                }
            }
            else
            {
                result.Content = cachedCss;
            }
            

            return result;
        }
        /// <summary>
        /// Get product css for a specific product (ie: myers)
        /// </summary>
        /// <param name="courseProductName"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        private string GetProductCss(string courseProductName, string courseType = "", string component = "")
        {
            var css = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(component))
            {
                courseProductName = String.Format("component/{0}/{1}", component, courseProductName);
            }

            // load course-specific css (ie: _strayer2e.css)
            string courseProductCss = LocalCSSFile(courseProductName);

            // the same book might be in multiple applications, load type_productname css (ie: _xbook_strayer2e.css)
            string courseTypeAndProductCss = LocalCSSFile(String.Format("{0}_{1}", courseType, courseProductName));

            css.AppendLine("/**      <product> **/"); css.AppendLine(courseProductCss);
            css.AppendLine("/** <type_product> **/"); css.AppendLine(courseTypeAndProductCss);
            
            return css.ToString();

        }
        /// <summary>
        ///  Get product css for a specific product course based on dlap css parameters (ie: myers)
        /// </summary>
        /// <param name="course"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        private string GetProductCssWithParams(BizDC.Course course, string component = "")
        {
            string titleCss;
            string courseProduct = course.CourseType;
            if (!String.IsNullOrWhiteSpace(component))
            {
                courseProduct = String.Format("component/{0}/{1}", component, courseProduct);
            }
            titleCss = LocalCSSFile(courseProduct.ToLower() + "_title");
            string sParmValue ="";
            //replace titlecss parameters with values from dlap
            if (!string.IsNullOrWhiteSpace(titleCss))
            {
                foreach (var cssParam in course.TitleCSSParameters)
                {
                    sParmValue = cssParam.Value;
                    if(Uri.IsWellFormedUriString(cssParam.Value, UriKind.Relative)) {
                       sParmValue =  sParmValue.Replace("style/", BrainHoneyLocation);
                    }

                    titleCss = titleCss.Replace("{{" + cssParam.Name + "}}", sParmValue);
                }
            }

            titleCss = dotless.Core.Less.Parse(titleCss);
            return titleCss;
        }
        /// <summary>
        /// Loads the title css for a specific title
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 86400, VaryByCustom = "debug", VaryByParam = "*")]
        public ActionResult TitleCss(string courseProductName, string courseType, string productCourseId, string component = "" )
        {
            string titleCss = string.Empty;
            var result = new ContentResult();
            result.ContentType = "text/css";
            result.ContentEncoding = Encoding.UTF8;

            var courseId = productCourseId;
            if (courseId.IsNullOrEmpty())
            {
                courseId = RouteData.Values["courseid"].ToString();
            }
            if (courseId.IsNullOrEmpty())
            {
                return new EmptyResult();
            }
            //var key = string.Format("PX_CSS_{0}", Request.Url.PathAndQuery);
            titleCss = Cache.FetchCssFile(Request.Url.PathAndQuery, "");

            if (titleCss == null)
            {
                var course = CourseActions.GetCourseByCourseId(courseId);
               
                if (course.TitleCSSParameters.Count > 0)
                { //if course has css parameters, use those to generate title css
                    titleCss = GetProductCssWithParams(course);
                }
                else
                { //load our normal title css
                    titleCss = GetProductCss(courseProductName, courseType, component);
                }
                try
                {
                    titleCss = CssCompressor.Compress(titleCss, 200, CssCompressionType.StockYuiCompressor, true);
                }
                catch (Exception ex)
                {

                }

                if (!ResourceEngine.IsDebug())
                {
                    Cache.StoreCssFile(Request.Url.PathAndQuery, "", titleCss);
                }
            }
            result.Content = titleCss;
            
            return result;
        }

        

        /// <summary>
        /// Loads all necessary CSS
        /// </summary>
        /// <returns></returns>
        private string LoadCSS(string type = "")
        {
            var result = "";
            using (Tracer.DoTrace("LoadCSS"))
            {         
                var layout = DefaultLayout;
                var courseLayout = DetermineCourseLayout();
                
                string css = string.Empty;

                if (UseLocalFiles)
                    css = LocalCSS(layout, courseLayout, type);                 
                else
                    css = RemoteCSS(layout, courseLayout, type);

                if (!ResourceEngine.IsDebug())
                {
                    using (Tracer.DoTrace("Compress CSS"))
                        css = CssCompressor.Compress(css, 200, CssCompressionType.StockYuiCompressor, false);
                }

                result = css;
            }
            return result;
        }

        /// <summary>
        /// Loads a file by path
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private ActionResult LoadFile(string path)
        {
            ActionResult result = new EmptyResult();

            if (UseLocalFiles)
                result = LocalFile(path);
            else
                result = RemoteFile(path);

            return result;
        }

        #region Common Methods
        
        /// <summary>
        /// determines the correct course specific layout name.
        /// </summary>
        /// <returns>
        /// layout name specific to this course.
        /// </returns>
        private string DetermineCourseLayout()
        {
            var layout = CourseLayout;
            if (UseLocalFiles)
            {
                //in this case layout is based on the course name from the url.
                if (RouteData.Values.ContainsKey("course"))
                    layout = RouteData.Values["course"].ToString();
            }
            return layout;
        }

        #endregion

        #region Loading Local Files

        /// <summary>
        /// Loads all CSS content necessary to render the site.
        /// </summary>
        /// <param name="layout">name of the layout file to use.</param>
        /// <param name="course">name of the course file to use.</param>
        /// <returns></returns>
        private string LocalCSS(string layout, string course, string type)
        {
            var css = new StringBuilder();

            using (Tracer.DoTrace("LocalCSS"))
            {
                switch (type.ToLower())
                {
                    case "platform":
                        css.AppendLine(LocalCSSFile(layout));
                        break;
                    case "widget1":
                        css.AppendLine(LocalWidgetCSS(type));
                        break;
                    case "widget2":
                        css.AppendLine(LocalWidgetCSS(type));
                        break;
                    case "course":
                        css.AppendLine(LocalCourseCSS(course));
                        if (!string.IsNullOrEmpty(Theme))
                            css.AppendLine(LocalCSSFile(Theme));
                        break;
                    default:
                        css.AppendLine(LocalCSSFile(layout));
                        css.AppendLine(LocalWidgetCSS(string.Empty));
                        css.AppendLine(LocalCourseCSS(course));
                        if (!string.IsNullOrEmpty(Theme))
                            css.AppendLine(LocalCSSFile(Theme));
                        break;
                }                                
            }
            return css.ToString();
        }

        /// <summary>
        /// finds & loads the course css file from a local location.
        /// </summary>
        /// <param name="file">course name</param>
        /// <returns>
        /// contents of file
        /// </returns>
        private string LocalCourseCSS(string course)
        {
            string courseCss = LocalCSSFile(course + "_style");

            //if course-specific css doesn't exist, load product-style
            courseCss = string.IsNullOrWhiteSpace(courseCss) ? LocalCSSFile(course) : courseCss;

            // if generic course css doesn't exist, load default-style
            courseCss = string.IsNullOrWhiteSpace(courseCss) ? LocalCSSFile("_defaultcourse") : courseCss;

            return courseCss;
        }

        /// <summary>
        /// Loads a css file from a local location.
        /// </summary>
        /// <param name="file">name of the file, .css extension will be added by the method</param>
        /// <returns>
        /// contents of file
        /// </returns>
        private string LocalCSSFile(string file)
        {
            var css = string.Empty;
            bool isLess = false;
            file = file.Replace(".css", "").Replace(".less", "");
            using (Tracer.DoTrace("LocalCSSFile(file={0})", file))
            {
                if (string.IsNullOrEmpty(file))
                    return css;

                var path = Server.MapPath(string.Format("~/{0}/{1}.css", BaseLocalDirectory, file));
                if (!System.IO.File.Exists(path))
                {
                    path = Server.MapPath(string.Format("~/{0}/_{1}.css", BaseLocalDirectory, file));
                }
                if (!System.IO.File.Exists(path))//check for .less file
                {
                    path = Server.MapPath(string.Format("~/{0}/{1}.less", BaseLocalDirectory, file));
                    isLess = true;
                }
                if (!System.IO.File.Exists(path))
                {
                    path = Server.MapPath(string.Format("~/{0}/_{1}.less", BaseLocalDirectory, file));
                    isLess = true;
                }

                if (System.IO.File.Exists(path))
                {
                    using (var fs = new StreamReader(path))
                    {
                        css = fs.ReadToEnd();
                        if (isLess)
                        {
                            //parse less file and assign only if css 
                            var parsedcss = dotless.Core.Less.Parse(css);
                            if (!parsedcss.IsNullOrEmpty())
                            {
                                css = parsedcss;
                            }

                        }
                    }
                }
            }

            //replace images location from local to brainhoney location
            if (!string.IsNullOrEmpty(css))
            {
                if (string.IsNullOrEmpty(BrainHoneyLocation)) BrainHoneyLocation = ConfigurationManager.AppSettings["BHLoc"];
                css = css.Replace("style/images/", BrainHoneyLocation + "images/");
            }

            return css;
        }

        /// <summary>
        /// Renders the correct include HTML needed to load the widgets for this product type
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public IHtmlString IncludesForWidgetCSS(string productType)
        {
            var widgetLinks = String.Empty;
            var productWidgetFilepath = VirtualPathUtility.ToAbsolute("~/Content/" + productType + "_widgets.css");
            var resource = ResourceEngine.Resource(productWidgetFilepath);
            if (resource != null)
            {
                widgetLinks = ResourceEngine.IncludesFor(productWidgetFilepath, Url.RouteUrl("CourseSectionHome")).ToString();
            }
            else
            {

                widgetLinks =
                    ResourceEngine.LinkTag(Url.RouteUrl("CourseSectionDefault",
                        new { controller = "Style", action = "Index", type = "widget1" }));
                widgetLinks +=
                   ResourceEngine.LinkTag(Url.RouteUrl("CourseSectionDefault",
                       new { controller = "Style", action = "Index", type = "widget2" }));
            }
            return new HtmlString(widgetLinks);

        }


        /// <summary>
        /// Loads all CSS files in the local "widgets" directory.
        /// </summary>
        /// <returns>
        /// contents of all widget css files
        /// </returns>
        private string LocalWidgetCSS(string type)
        {
            var css = new StringBuilder();
            var result = string.Empty;
            var debug = IsDebug();


            using (Tracer.DoTrace("LocalWidgetCSS"))
            {

                if (string.IsNullOrEmpty(result))
                {
                    var dir = Server.MapPath(string.Format("~/{0}/{1}", BaseLocalDirectory, WidgetDirectory));

                    List<string> files;
                    using (Tracer.DoTrace("Get File List"))
                    {
                        files = Directory.GetFiles(dir).ToList<string>();
                        files.Sort();

                        if (files.Count != 0)
                        {
                            int count = files.Count / 2;
                            if(type.ToLower() == "widget1")
                                files = files.Take(count).ToList();
                            else if (type.ToLower() == "widget2")
                                files = files.Skip(count).ToList();                                                                     
                        }                        
                    }

                    using (Tracer.DoTrace("Concatenate Files"))
                    {
                        foreach (var file in files)
                        {
                            using (var fs = new StreamReader(file))
                            {
                                var content = fs.ReadToEnd();
                                if (Path.GetExtension(file) == ".less")
                                    content = dotless.Core.Less.Parse(content);
                                css.AppendLine(content);
                            }
                        }
                    }

                    using (Tracer.DoTrace("Render String"))
                        result = css.ToString();

                }
            }

            return result;
        }



        /// <summary>
        /// Loads a local file.
        /// </summary>
        /// <param name="file">path to the file we are loading.</param>
        /// <returns>
        /// contents of the file.
        /// </returns>
        private FileStreamResult LocalFile(string file)
        {
            FileStreamResult result = null;
            var path = Server.MapPath(string.Format("~/{0}/{1}", BaseLocalDirectory, file));

            if (System.IO.File.Exists(path))
                result = new FileStreamResult(new FileStream(path, FileMode.Open, FileAccess.Read), ContentTypeUtils.GetContentType(Path.GetFileName(path)));

            return result;
        }

        #endregion

        #region Loading Remote Files

        /// <summary>
        /// Loads all CSS content necessary to render the site, assuming the content is remote.
        /// </summary>
        /// <param name="layout">name of the layout file to use.</param>
        /// <param name="course">name of the course file to use.</param>
        /// <returns></returns>
        private string RemoteCSS(string layout, string course, string type)
        {
            var css = new StringBuilder();
            switch (type.ToLower())
            {
                case "platform":
                    css.AppendLine(RemoteCSSFile(layout));
                    break;
                case "widget1":
                    css.AppendLine(RemoteWidgetCSS(type));
                    break;
                case "widget2":
                    css.AppendLine(RemoteWidgetCSS(type));
                    break;
                case "course":
                    css.AppendLine(RemoteCSSFile(course));
                    css.AppendLine(RemoteCSSFile(Theme));                    
                    break;
                default:
                    css.AppendLine(RemoteCSSFile(layout));
                    css.AppendLine(RemoteWidgetCSS(string.Empty));
                    css.AppendLine(RemoteCSSFile(course));
                    css.AppendLine(RemoteCSSFile(Theme));
                    break;
            }                 
            return css.ToString();
        }

        /// <summary>
        /// Loads a remote css file.
        /// </summary>
        /// <param name="file">file to load, without the .css extension.</param>
        /// <returns>
        /// contents of remote css file.
        /// </returns>
        private string RemoteCSSFile(string file)
        {
            var css = string.Empty;
            var path = string.Format("{0}/{1}.css", BaseRemoteDirectory, file);

            var resource = ContentActions.GetResource(Context.EntityId, path);
            if (resource != null)
            {
                using (var sr = new StreamReader(resource.GetStream()))
                    css = sr.ReadToEnd();
            }

            return css;
        }

        /// <summary>
        /// Loads all CSS for all widget stored remotely.
        /// </summary>
        /// <returns>contents of all widget css files.</returns>
        private string RemoteWidgetCSS(string type)
        {
            var css = new StringBuilder();
            var files = ContentActions.ListResources(Context.EntityId, string.Format("{0}/{1}/*.css", BaseRemoteDirectory, WidgetDirectory), string.Empty);

            files.OrderBy(f => f.Name);
            if (files.Count() != 0)
            {
                int count = files.Count() / 2;
                if (type.ToLower() == "widget1")
                    files = files.Take(count).ToList();
                else if (type.ToLower() == "widget2")
                    files = files.Skip(count).ToList();
            }   

            foreach (var file in files)
            {
                using (var sr = new StreamReader(file.GetStream()))
                    css.AppendLine(sr.ReadToEnd());
            }

            return css.ToString();
        }

        /// <summary>
        /// Loads a remote file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        private FileStreamResult RemoteFile(string file)
        {
            FileStreamResult result = null;
            var path = string.Format("{0}/{1}", BaseRemoteDirectory, file);
            var resource = ContentActions.GetResource(Context.EntityId, path);
            if (resource != null)
                result = new FileStreamResult(resource.GetStream(), ContentTypeUtils.GetContentType(Path.GetFileName(path)));

            return result;
        }

        #endregion

        /// <summary>
        /// Checks the compilation section in the web.config and returns true if the debug 
        /// attribute is set.
        /// </summary>
        /// <returns></returns>
        private bool IsDebug()
        {
            bool debug = true;

            var compilation = System.Configuration.ConfigurationManager.GetSection("system.web/compilation") as System.Web.Configuration.CompilationSection;

            if (compilation != null)
                debug = compilation.Debug;

            return debug;
        }
    }
}
