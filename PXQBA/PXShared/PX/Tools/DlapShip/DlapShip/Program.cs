using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;

using Bfw.Common.Collections;
using Bfw.Common.Patterns.Unity;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;
using Bfw.Common.Database;

using Mono.Options;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using DlapShip.Properties;

namespace DlapShip
{
    public class Program
    {
        /// <summary>
        /// The db column_ product course id
        /// </summary>
        private const string DbColumnProductCourseId = "AgilixCourseID";

        /// <summary>
        /// The dlap root path
        /// </summary>
        private const string DlapItemsRootPath = "items/item/data";

        /// <summary>
        /// The items per batch
        /// </summary>
        private const int ItemsPerBatch = 20;

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        private static ISession Session { get; set; }

        /// <summary>
        /// Gets or sets the dlap database manager.
        /// </summary>
        /// <value>
        /// The dlap database manager.
        /// </value>
        private static DatabaseManager DlapDatabaseManager { get; set; }

        /// <summary>
        /// Gets or sets the list product course ids.
        /// </summary>
        /// <value>
        /// The list product course ids.
        /// </value>
        public static List<string> ListProductCourseIds { get; set; }

        /// <summary>
        /// Gets or sets the items count.
        /// </summary>
        /// <value>
        /// The items count.
        /// </value>
        public static int ItemsCount { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public static string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the update items file path.
        /// </summary>
        /// <value>
        /// The update items file path.
        /// </value>
        public static string UpdateItemsFilePath { get; set; }

        /// <summary>
        /// Gets or sets the delete items file path.
        /// </summary>
        /// <value>
        /// The delete items file path.
        /// </value>
        public static string DeleteItemsFilePath { get; set; }

        /// <summary>
        /// Gets or sets the run environment.
        /// </summary>
        /// <value>
        /// The run environment.
        /// </value>
        public static string RunEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the result data. -- for testing purpose only
        /// </summary>
        /// <value>
        /// The result data.
        /// </value>
        public static List<XDocument> ResultData { get; set; }

        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static void Main(string[] args)
        {
            var options = GetCommandOptions(args);

            if (ValidateArguments(options))
            {
                Initialize(options);
                try
                {
                    SetDatabaseManager();

                    if (AreFilesValid())
                    {
                        Execute(options.App, options.Title, options.EntityId);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(@"There was an error executing DlapShip.  See log file for details.");
                    Logger.Write(String.Format("DlapShip Failed: {0}", e));
                }

                Console.WriteLine(@"Not executed");
            }

#if Release
                    
            Console.WriteLine("press any key to exit");
            Console.ReadKey();
#endif

        }

        /// <summary>
        /// Gets a populated CommandOptions object based on <paramref name="args"/> passed in.
        /// </summary>
        /// <param name="args">Args passed into the command line to be parsed into the CommandOptions object</param>
        /// <returns>A populated CommandOptions object based of passed in args</returns>
        private static CommandOptions GetCommandOptions(IEnumerable<string> args)
        {
            var options = new CommandOptions();
            var os = RegisterOptions(options);
            os.Parse(args);
            return options;
        }

        /// <summary>
        /// Executes the specified filename.
        /// </summary>
        /// <param name="appName">Name of the app.</param>
        /// <param name="title">The title.</param>
        /// <param name="entityId">The entity id.</param>
        private static void Execute(string appName, string title, string entityId)
        {
            if (!String.IsNullOrWhiteSpace(entityId))
            {
                ListProductCourseIds = new List<string> { entityId };
            }
            else
            {
                // Retrieve Product Course's Ids from database based on application name or course title
                if (!String.IsNullOrWhiteSpace(appName) && !String.IsNullOrWhiteSpace(title))
                {
                    ListProductCourseIds = new List<string>(GetProductCourseIdsBy(title, appName));
                }
                else if (!String.IsNullOrWhiteSpace(appName))
                {
                    ListProductCourseIds = new List<string>(GetProductCourseIdsBy(appName));
                }
            }

            if (ListProductCourseIds != null && ListProductCourseIds.Count > 0)
            {
                foreach (string id in ListProductCourseIds)
                {
                    /*
                        * <batch> in provided xml file 
                        * update items defined in the file
                     */
                    ExecuteBatchUpload(id);

                    /*
                        * <UpdateItems> in provided xml file 
                        * get all items from DLAP based on product course id, massage the data and update it on DLAP 
                     */
                    UpdateItems(id);

                    /*
                        * <DeleteItems> in provided xml file 
                        * get all items from DLAP based on product course id, delete items from DLAP
                     */
                    DeleteItemsCommand(id);
                }
            }
            else
            {
                Console.WriteLine(@"No records for productcourse ids");
            }
        }

        /// <summary>
        /// Executes the xpath query for.
        /// </summary>
        /// <param name="courseId">The course id.</param>
        private static void ExecuteBatchUpload(string courseId)
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine(@"Upload file not exists");
                return;
            }

            Stream stream = null;

            try
            {
                stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var settings = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};

                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    stream = null;

                    XDocument xDoc = XDocument.Load(reader);

                    /* course id value changes */
                    ReplaceValues(ref xDoc, "-courseid-", courseId);
                    ReplaceValues(ref xDoc, "-entityid-", courseId);

                    /* batch upload based on provided xml file */
                    ExecuteDlapScript(xDoc.Root);
                }
            }
            catch (IOException ioEx)
            {
                Logger.Write(String.Format("Failed at ExecuteBatchUpload: {0} file being used by another process. Exception: {1}", FilePath, ioEx.Message));
            }
            catch (Exception ex)
            {
                Logger.Write(String.Format("Failed at ExecuteBatchUpload with message: {0}", ex.Message));
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// Executes the dlap script.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private static void ExecuteDlapScript(XElement doc)
        {
            var request = new DlapRequest
                                    {
                                        Type = DlapRequestType.Post,
                                        ContentType = "text/xml",
                                        SuppressWrapper = true,
                                        Mode = DlapRequestMode.Batch
                                    };

            var batchElem = doc.Element("batch");
            if (batchElem != null)
            {
                request.AppendData(batchElem);
            }
            else
            {
                var requestElements = doc.Elements("request");
                foreach (XElement elm in requestElements)
                {
                    request.AppendData(elm);
                }
                request.SuppressWrapper = false;
            }

            //execute script try
            string error = string.Empty;
            try
            {
                DlapResponse response = Session.Send(request, true, RunEnvironment);

                if (response.Code != DlapResponseCode.OK)
                {
                    error = response.Message;
                }
                else
                {
                    Console.WriteLine(response.ResponseXml.ToString());
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            Console.WriteLine(error);
        }

        /// <summary>
        /// Updates the items.
        /// </summary>
        private static void UpdateItems(string entityId)
        {
            if (!File.Exists(UpdateItemsFilePath))
            {
                Console.WriteLine(@"UpdateItems file not exists");
                return;
            }

#if DEBUG
            ResultData = new List<XDocument>(); // for TESTING ONLY!
#endif

            Stream stream = new FileStream(UpdateItemsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            try
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    stream = null;

                    var xmlDoc = new XmlDocument();

                    xmlDoc.Load(reader);

                    XElement elem = XElement.Parse(xmlDoc.OuterXml);

                    /* bulk update based on provided xml file */
                    
                    foreach (XElement item in elem.Elements("Item"))
                    {
                        XDocument xDoc =  GetXItemsFromDlap(item, entityId);

                        try
                        {
                            UpdateXItems(item, ref xDoc);

                            DeleteXItems(item, ref xDoc);

                            UpdateDlapItems(xDoc, entityId);
                        }
                        catch (Exception exx)
                        {
                            Logger.Write(String.Format("Executing the product course with id {0} failed with message: {1}", entityId, exx.Message));
                        }
                    }
                }
            }
            catch (IOException ioEx)
            {
                Logger.Write(String.Format("Failed at ExecuteBatchUpload: {0} file being used by another process. Exception: {1}", FilePath, ioEx.Message));
            }
            catch (Exception ex)
            {
                Logger.Write(String.Format("Failed at ExecuteBatchUpload with message: {0}", ex.Message));
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// Deletes the items
        /// </summary>
        /// <param name="entityId">Id of the course to delete the items from </param>
        private static void DeleteItemsCommand(string entityId)
        {
            if (!File.Exists(DeleteItemsFilePath))
            {
                return;
            }

            XElement elem;
            if (TryParseDeleteDlapItemsFile(out elem))
            {
                foreach (XElement query in elem.Elements("Item"))
                {
                    List<Item> items = GetItemsFromDlap(query, entityId);

                    try
                    {
                        Console.WriteLine(@"Delete Query: {1} | Items To Delete: {0}", items.Count(), query.Value.Replace(Environment.NewLine, string.Empty));
                        
                        DeleteDlapItems(items, entityId);
                    }
                    catch (Exception exx)
                    {
                        Logger.Write(String.Format("Executing the product course with id {0} failed with message: {1}", entityId, exx.Message));
                    }
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Error parsing file: {0} - Invalid XML or invalid deleteitem structure", DeleteItemsFilePath));
            }
        }

        #region Database Queries

        /// <summary>
        /// Gets the product course ids by application.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        private static IEnumerable<string> GetProductCourseIdsBy(string applicationName)
        {
            var query = String.Format(@"select distinct ac.AgilixCourseID
                                        from [dbo].[tblAgilixCourseID] ac
                                        inner join [dbo].[tblSiteKey] sk (nolock) on sk.SiteID = ac.SiteId
                                        left join [dbo].[tblSiteISBN] si (nolock) on si.SiteID = ac.SiteId
                                        where ac.AgilixType in ({0})", applicationName);

            return GetProductCourseIdsFor(query);
        }

        /// <summary>
        /// Gets the product course ids by title.
        /// </summary>
        /// <param name="courseTitle">The course title.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns></returns>
        private static IEnumerable<string> GetProductCourseIdsBy(string courseTitle, string applicationName)
        {
            var prefix = RunEnvironment;
            if (RunEnvironment.ToLower().Equals("prod") || RunEnvironment.ToLower().Equals("snap"))
            {
                prefix = "www";
            }

            var query = String.Format(@"select distinct ac.AgilixCourseID
                                            from tblAgilixCourseID ac
                                            inner join tblSiteKey sk (nolock) on sk.SiteID = ac.SiteId
                                            left join tblSiteISBN si (nolock) on si.SiteID = ac.SiteId
                                            where sk.BaseUrl like '{2}.%/{0}%' and ac.AgilixType in ({1})",
                                            courseTitle, applicationName, prefix);

            return GetProductCourseIdsFor(query);
        }

        /// <summary>
        /// Gets the product course ids for.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        private static IEnumerable<string> GetProductCourseIdsFor(string query)
        {
            var dbRecords = DlapDatabaseManager.Query(query);

            return dbRecords.Select(record => record.String(DbColumnProductCourseId)).ToList();
        }

        /// <summary>
        /// Sets the database manager for.
        /// </summary>
        private static void SetDatabaseManager()
        {
            var conn = "devRA";
            switch (RunEnvironment.ToLower())
            {
                case "qa":
                    conn = "qaRA";
                    break;
                case "prod":
                    conn = "prodRA";
                    break;
                case "snap":
                    conn = "prodSnapshotRA";
                    break;
                case "pr":
                    conn = "pristineRA";
                    break;
            }

            /* Database manager set up */
            DlapDatabaseManager = new DatabaseManager(conn);
        }

        #endregion

        #region XPath methods

        /// <summary>
        /// Deletes the value from.
        /// </summary>
        /// <param name="xDocument">The x document.</param>
        /// <param name="valueToDelete">The value to delete.</param>
        /// <returns></returns>
        private static XDocument DeleteValueFrom(XDocument xDocument, string valueToDelete)
        {
            var attr = GetAttributeName(valueToDelete);

            XmlDocument xmlDocument = xDocument.GetXmlDocument();
            XPathNavigator navigator = xmlDocument.CreateNavigator();
            XPathNodeIterator navIterator = navigator.Select(String.Format("{0}/{1}", DlapItemsRootPath, valueToDelete.Trim()));

            foreach (XPathNavigator nav in navIterator)
            {
                if (!String.IsNullOrWhiteSpace(attr))
                {
                    nav.MoveToAttribute(attr, nav.NamespaceURI);
                    nav.DeleteSelf();
                }
                else // remove the whole node -  clearing all attributes
                {
                    while (nav.MoveToFirstAttribute())
                    {
                        nav.DeleteSelf();
                    }

                    nav.SetValue(String.Empty);
                }
            }

            return XDocument.Parse(xmlDocument.OuterXml);
        }

        /// <summary>
        /// Deletes the X items.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="xDocument">The x document.</param>
        private static void DeleteXItems(XElement item, ref XDocument xDocument)
        {
            Logger.Write(String.Format(@"//////// DELETE //////////{0}", Environment.NewLine));

            IEnumerable<XElement> listDelete = item.Elements("Delete");
            foreach (XElement delete in listDelete)
            {
                if (!String.IsNullOrWhiteSpace(delete.Value))
                {
                    xDocument = DeleteValueFrom(xDocument, delete.Value);

                    Logger.Write(String.Format(@"{1}{0}", Environment.NewLine, delete.Value));
                }
            }

            Logger.Write(String.Format(@"//////// END OF DELETE //////////{0}", Environment.NewLine));
        }

        /// <summary>
        /// Deletes the dlap items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="entityId">The entity id.</param>
        private static void DeleteDlapItems(List<Item> items, string entityId)
        {
            var skip = 0;
            if (items.Count() > ItemsPerBatch)
            {
                List<Item> batch;

                do
                {
                    //putting group: " + skip
                    batch = items.Skip(skip).Take(ItemsPerBatch).ToList();

                    if (batch.Count > 0)
                    {
                        ItemsCount++;
                        Console.WriteLine(@"Deleting items {0} -> {1}", skip, skip + batch.Count());
                        ExecuteDlapDeleteCommand(batch, entityId);
                    }

                    skip += ItemsPerBatch;

                } while (batch.Count >= ItemsPerBatch);
            }
            else
            {
                ItemsCount++;
                ExecuteDlapDeleteCommand(items, entityId);
            }

            Logger.Write(String.Format(@"//////// END OF DELETE //////////{0}", Environment.NewLine));
        }

        /// <summary>
        /// Sets the value to existing X path.
        /// </summary>
        /// <param name="xDocument">The x document.</param>
        /// <param name="query">The query.</param>
        /// <param name="value">The value.</param>
        private static void FillValues(ref XDocument xDocument, string query, string value)
        {
            var xmlDocument = xDocument.GetXmlDocument();
            var navigator = xmlDocument.CreateNavigator();
            var navIterator = navigator.Select(String.Format("{0}{1}", DlapItemsRootPath, query));
            if (navIterator.Count <= 0 || String.IsNullOrWhiteSpace(value)) return;
            foreach (XPathNavigator nav in navIterator)
            {
                nav.SetValue(value);
            }

            xDocument = XDocument.Parse(xmlDocument.OuterXml);
        }

        /// <summary>
        /// Fills the elements.
        /// </summary>
        /// <param name="xDocument">The x document.</param>
        /// <param name="query">The query.</param>
        private static void FillElements(ref XDocument xDocument, string query)
        {
            var rootPath = DlapItemsRootPath;

            IEnumerable<XElement> nodes = xDocument.XPathSelectElements(rootPath);

            // Create a new Regex object
            var r = new Regex(@"/+([\w]+)(\[@([\w]+)='([^']*)'((\sand\s)?(\sor\s)?(@([\w]+)='([^']*)'))*?\])?|/@([\w]+)");

            // Find matches
            Match m = r.Match(query);

            while (m.Success)
            {
                string nodePath = m.Groups[0].Value;
                string nodeName = m.Groups[1].Value;
                Dictionary<string, string> mAttributes = GetAttributesFromRegex(m);

                if (String.IsNullOrWhiteSpace(nodeName))
                {
                    continue;
                }

                foreach (XElement node in nodes)
                {
                    XElement element = node.Element(nodeName);
                    if (element == null) 
                    {
                        element = new XElement(nodeName);

                        // <thisNode>NotEmpty</thisNode> -> <thisNode><addThisNode/></thisNode>
                        if (!node.HasElements && !node.IsEmpty)
                        {
                            node.SetValue(String.Empty);
                        }

                        node.Add(element);
                    }

                    foreach (KeyValuePair<string, string> kvp in mAttributes) 
                    {
                        element.SetAttributeValue(kvp.Key, kvp.Value);
                    }
                }

                rootPath = String.Format("{0}{1}", rootPath, nodePath);
                nodes = xDocument.XPathSelectElements(rootPath);

                m = m.NextMatch();
            }
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        private static string GetAttributeName(string query)
        {
            string[] arr = query.Split('/');

            if (arr.Length == 0)
                return String.Empty;

            var attributeName = String.Empty;

            var chunk = arr[arr.Length - 1];
            if (chunk.Contains("[@"))
            {
                var startIndex = chunk.IndexOf("[@", StringComparison.Ordinal) + 2;
                var endIndex = chunk.IndexOf("=", StringComparison.Ordinal);
                if (endIndex == -1)
                    endIndex = chunk.IndexOf("]", StringComparison.Ordinal);

                if (endIndex >= startIndex)
                {
                    attributeName = chunk.Substring(startIndex, endIndex - startIndex);
                }
            }

            return attributeName;
        }

        /// <summary>
        /// Gets the attributes from regex.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetAttributesFromRegex(Match m)
        {
            var mAttributes = new Dictionary<string, string>();

            if (m.Groups.Count > 5)
            {
                try
                {
                    for (int i = 3; i < m.Groups.Count; i = i + 6)
                    {
                        if (String.IsNullOrWhiteSpace(m.Groups[i].Value))
                            continue;

                        if (mAttributes.ContainsKey(m.Groups[i].Value))
                        {
                            mAttributes[m.Groups[i].Value] = m.Groups[i + 1].Value;
                        }
                        else
                        {
                            mAttributes.Add(m.Groups[i].Value, m.Groups[i + 1].Value);
                        }
                    }
                }
                catch { }
            }
            else
            {
                mAttributes.Add(m.Groups[3].Value, m.Groups[4].Value);
            }

            return mAttributes;
        }

        /// <summary>
        /// Replaces the values.
        /// </summary>
        /// <param name="xDocument">The x document.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private static void ReplaceValues(ref XDocument xDocument, string key, string value)
        {
            if (String.IsNullOrWhiteSpace(key))
                return;

            string document = xDocument.ToString().Replace(key, value);

            xDocument = XDocument.Parse(document);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="xDocument">The x document.</param>
        /// <param name="query">The query.</param>
        /// <param name="value">The value.</param>
        private static void UpdateValues(ref XDocument xDocument, string query, string value)
        {
            if (String.IsNullOrWhiteSpace(query))
                return;

            FillElements(ref xDocument, query);
            FillValues(ref xDocument, query, value);
        }

        /// <summary>
        /// Updates the X items.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="xDocument">The x document.</param>
        private static void UpdateXItems(XElement item, ref XDocument xDocument)
        {
            Logger.Write(String.Format(@"//////// UPDATE //////////{0}", Environment.NewLine));

            IEnumerable<XElement> listUpdate = item.Elements("Update");
            foreach (XElement update in listUpdate)
            {
                var key = update.Element("key");
                var value = update.Element("value");

                if (key != null && !String.IsNullOrWhiteSpace(key.Value))
                {
                    Logger.Write(String.Format(@"<key>{1}</key>{0}<value>{2}</value>{0}", Environment.NewLine, key, value));

                    string keyValue = (value != null && !String.IsNullOrWhiteSpace(value.Value)) ? GetXElementInnerNodes(value) : String.Empty;
                    UpdateValues(ref xDocument, key.Value.Trim(), keyValue);
                }
            }

            Logger.Write(String.Format(@"//////// END OF UPDATE //////////{0}", Environment.NewLine));
        }

        /// <summary>
        /// Gets the X element inner nodes.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private static string GetXElementInnerNodes(XElement element)
        {
            var reader = element.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }

        /// <summary>
        /// Parses an XML file to return the top level XML element
        /// </summary>
        /// <returns>Top level XML element</returns>
        private static XElement ParseCommandFile(string file)
        {
            XElement retval;
            Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    stream = null;

                    var xmlDoc = new XmlDocument();

                    xmlDoc.Load(reader);

                    retval = XElement.Parse(xmlDoc.OuterXml);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(String.Format("Error parsing command file: {0}", ex.Message));
                //Let the exception reach the app so that we can test proper exceptions in unit tests
                throw;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            return retval;
        }

        private static bool TryParseDeleteDlapItemsFile(out XElement rootElement)
        {
            rootElement = ParseCommandFile(DeleteItemsFilePath);
            return VerifyValidDeleteItemFile(rootElement);
        }
        #endregion

        #region DLAP calls

        /// <summary>
        /// Executes the dlap update command.
        /// </summary>
        /// <param name="xDocument">The x document.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns></returns>
        private static void ExecuteDlapUpdateCommand(XDocument xDocument, string entityId)
        {
            var batch = new Batch {RunAsync = true};

            var cmdPut = new PutRawItem
            {
                ItemDoc = xDocument
            };

            batch.Add(cmdPut);

            ExecuteSessionCommand(batch, entityId);

            Logger.Write(String.Format(@"Updating {2} items batch ""{0}"" with Product Course Id ""{1}""...", ItemsCount, entityId, ItemsPerBatch));

            if (cmdPut.Success)
            {
#if DEBUG
                ResultData.Add(xDocument); // FOR TESTING!
#endif

                Console.WriteLine(@"Successful update to DLAP");

                Logger.Write(String.Format(@"Successfully updated items batch ""{0}"" for Product Course Id ""{1}""", ItemsCount, entityId));
            }
            else
            {
                if (!cmdPut.Failures.IsNullOrEmpty())
                {
                    foreach (var failure in cmdPut.Failures)
                    {
                        var failedMessage = String.Format("Failed updating item {0} with reason {1}", failure.ItemId, failure.Reason);

                        Console.WriteLine(failedMessage);

                        Logger.Write(failedMessage);
                    }
                }
            }
        }

        /// <summary>
        /// Executes a DLAP DeleteItems command for all items in <paramref name="itemsToDelete"/> against a course
        /// </summary>
        /// <param name="itemsToDelete">List of dlap items to delete</param>
        /// <param name="entityId">ID of the course to delete the items from</param>
        /// <returns>True if the delete command executed successfully</returns>
        private static void ExecuteDlapDeleteCommand(List<Item> itemsToDelete, string entityId)
        {
            var cmd = new DeleteItems
            {
                Items = itemsToDelete
            };

            ExecuteSessionCommand(cmd, entityId);

            if (cmd.Failures.IsNullOrEmpty())
            {
                Console.WriteLine(@"Delete sucessfully run");

                Logger.Write(String.Format(@"Successfully deleted items batch ""{0}"" for Product Course Id ""{1}""", ItemsCount, entityId));
            }
            else
            {
                foreach (var failure in cmd.Failures)
                {
                    var failedMessage = String.Format("Failed deleting item {0}: {1}", failure.ItemId, failure.Reason);

                    Console.WriteLine(failedMessage);

                    Logger.Write(failedMessage);
                }
            }

            if (cmd.Failures != null) cmd.Failures.IsNullOrEmpty();
        }

        /// <summary>
        /// Executes the session command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="entityId">The entity id.</param>
        private static void ExecuteSessionCommand(DlapCommand command, string entityId)
        {
            try
            {
                Session.ExecuteAsAdmin(RunEnvironment, command);
            }
            catch (BadDlapResponseException bde)
            {
                var message = String.Format("Failed withh error message -----{1}----- for Product Course Id {0}", entityId, bde.Message);

                Console.WriteLine(message);

                Logger.Write(message);
            }
            catch (DlapException de)
            {
                var message = String.Format("Failed withh error message -----{1}----- for Product Course Id {0}", entityId, de.Message);

                Console.WriteLine(message);

                Logger.Write(message);
            }
            catch (Exception e)
            {
                var message = String.Format("Failed with error message -----{1}----- for Product Course Id {0}", entityId, e.Message);

                Console.WriteLine(message);

                Logger.Write(message);
            }
        }

        /// <summary>
        /// Gets the X items.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns></returns>
        private static XDocument GetXItemsFromDlap(XElement item, string entityId)
        {
            var id=item.Descendants("Id").Count() > 0?item.Descendants("Id").FirstOrDefault().Value:"";
          
            var cmdGet = new GetRawItemList {EntityId = entityId, ItemId = id};

            XElement queryFilter = item.Element("Where");
            if (queryFilter != null
                && !String.IsNullOrWhiteSpace(queryFilter.Value))
            {
                // retrieve the items by passing &query=/*
                cmdGet.Query = queryFilter.Value;
            }

            ExecuteSessionCommand(cmdGet, entityId);

            return cmdGet.ItemDocument;
        }

        /// <summary>
        /// Returns a list of agilix items that match the <paramref name="query"/>
        /// </summary>
        /// <param name="query">query string to use to search for the items</param>
        /// <param name="entityId">course you want to search for the items in</param>
        /// <returns>List of items within the course that match the query</returns>
        private static List<Item> GetItemsFromDlap(XElement query, string entityId)
        {
            var cmdGet = new GetItems {SearchParameters = {EntityId = entityId}};

            XElement queryFilter = query.Element("Where");
            if (queryFilter != null
                && !String.IsNullOrWhiteSpace(queryFilter.Value))
            {
                // retrieve the items by passing &query=/*
                cmdGet.SearchParameters.Query = queryFilter.Value;
            }

            ExecuteSessionCommand(cmdGet, entityId);

            return cmdGet.Items;
        }

        /// <summary>
        /// Updates the dlap items.
        /// </summary>
        /// <param name="xDocument">The x document.</param>
        /// <param name="entityId"></param>
        private static void UpdateDlapItems(XDocument xDocument, string entityId)
        {
            if (entityId == null) throw new ArgumentNullException("entityId");

            // send chunked items wrapped in XDocument to ExecuteDlapCommands
            XElement elemItems = xDocument.Element("items");
            if (elemItems != null)
            {
                List<XElement> items = elemItems.Elements("item").ToList();

                if (items.Count() > ItemsPerBatch)
                {
                    int skip = 0;

                    List<XElement> records;

                    do
                    {
                        //putting group: " + skip
                        records = items.Skip(skip).Take(ItemsPerBatch).ToList();

                        if (records.Count > 0)
                        {
                            ItemsCount++;

                            XDocument newDocument = WrapInNodeRequests(records, entityId);

                            ExecuteDlapUpdateCommand(newDocument, entityId);
                        }

                        skip += ItemsPerBatch;

                    } while (records.Count >= ItemsPerBatch);
                }
                else
                {
                    ItemsCount++;

                    xDocument = WrapInNodeRequests(items, entityId);

                    ExecuteDlapUpdateCommand(xDocument, entityId);
                }
            }
        }

        /// <summary>
        /// Converts to post.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="entityId">The entity id.</param>
        /// <returns></returns>
        private static XDocument WrapInNodeRequests(IEnumerable<XElement> items, string entityId)
        {
            foreach (XElement item in items)
            {
                if (item.Attributes("id").Any())
                {
                    var id = item.Attribute("id").Value;

                    item.RemoveAttributes();
                    item.SetAttributeValue("itemid", id);
                    item.SetAttributeValue("entityid", entityId);
                }
            }

            var requests = new XElement("requests");
            requests.Add(items);

            return new XDocument(requests);
        }
        #endregion

        #region Validation
        /// <summary>
        /// Checks through all of the possible file paths to see if one is valid
        /// </summary>
        /// <returns></returns>
        private static bool AreFilesValid()
        {
            if (!File.Exists(FilePath) && !File.Exists(UpdateItemsFilePath) && !File.Exists(DeleteItemsFilePath))
            {
                Console.WriteLine(@"Error loading " + (FilePath ?? UpdateItemsFilePath ?? DeleteItemsFilePath) + @": file does not exist");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests to make sure all of the arguments passed into the application are valid
        /// </summary>
        /// <param name="options">options that were passed into the application</param>
        /// <returns>True if all of the arguments are valid</returns>
        private static bool ValidateArguments(CommandOptions options)
        {
            bool retval = true;
            if (String.IsNullOrWhiteSpace(options.DataFilePath) && String.IsNullOrWhiteSpace(options.UpdateItemsFilePath)
                          && String.IsNullOrWhiteSpace(options.DeleteItemsFilePath))
            {
                Console.WriteLine(Resources.NoFileError);
                retval = false;
            }
            else if (String.IsNullOrWhiteSpace(options.App))
            {
                Console.WriteLine(Resources.NoAppError);
                retval = false;
            }
            else if (String.IsNullOrWhiteSpace(options.Environment))
            {
                Console.WriteLine(Resources.NoEnvError);
                retval = false;
            }

            return retval;
        }

        /// <summary>
        /// Verify's the structure of a DeleteDlapItem xml file 
        /// </summary>
        /// <param name="rootElem">Root element of the xml file after parsing</param>
        /// <returns>True if the XML structure is valid</returns>
        private static bool VerifyValidDeleteItemFile(XElement rootElem)
        {
            //Root element should be deleteItems
            bool retval = rootElem.Name.LocalName.ToLowerInvariant().Equals("deleteitems");
            //Check to make sure there is at least one Item elements
            retval = retval && rootElem.Elements("Item").Any();
            return retval;
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Configures the service locator.
        /// </summary>
        private static void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        /// <summary>
        /// Establishes the connection.
        /// </summary>
        /// <returns></returns>
        private static ISessionManager EstablishConnection()
        {
            var username = ConfigurationManager.AppSettings["user"];
            var password = ConfigurationManager.AppSettings["password"];
            var sm = ServiceLocator.Current.GetInstance<ISessionManager>();
            sm.CurrentSession = sm.StartNewSession(username, password, false, username);
            return sm;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private static void Initialize(CommandOptions options)
        {
            ConfigureServiceLocator();

            var tracer = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ITraceManager>();
            if (tracer != null)
                tracer.StartTracing();

            var sm = EstablishConnection();
            Session = sm.CurrentSession;

            try
            {
                FilePath = options.DataFilePath;
                UpdateItemsFilePath = options.UpdateItemsFilePath;
                DeleteItemsFilePath = options.DeleteItemsFilePath;
                RunEnvironment = options.Environment;
            }
            catch { }
        }

        /// <summary>
        /// Registers the options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        private static OptionSet RegisterOptions(CommandOptions options)
        {
            /* invocation entails passing params as follows:
             *      /f:'' /u:'' /app:launchpad ti:universe9e e:dev
             *      
             *      where: 
             *      /f = batch <requests> file path
             *      /u = xpath queries file path
             *      /app = application name {px, xbook, etc.,} included in single quotes and separated by comma, for example 'FACEPLATE','Launchpad'
             *      /ti = course title
             *      /e = environment {dev, qa, prod}
             *      /id = product course/ entity id
             * 
             */

            var os = new OptionSet
            {
                {"f:", f => options.DataFilePath = f},
                {"u:", u => options.UpdateItemsFilePath = u},
                {"d:", d => options.DeleteItemsFilePath = d},
                {"app:", app => options.App = app},
                {"ti:", ti => options.Title = ti},
                {"e:", e => options.Environment = e},
                {"id:", id => options.EntityId = id}
            };

            return os;
        }
        #endregion Initialization
    }

    public class CommandOptions
    {
        /// <summary>
        /// Gets or sets the attribute.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        public string Attribute { get; set; }

        /// <summary>
        /// Gets or sets the data file path.
        /// </summary>
        /// <value>
        /// The data file path.
        /// </value>
        public string DataFilePath { get; set; }

        /// <summary>
        /// Gets or sets the update items file path.
        /// </summary>
        /// <value>
        /// The update items file path.
        /// </value>
        public string UpdateItemsFilePath { get; set; }

        /// <summary>
        /// Gets or sets the delete items file path.
        /// </summary>
        /// <value>
        /// The delete items file path.
        /// </value>
        public string DeleteItemsFilePath { get; set; }

        /// <summary>
        /// Gets or sets the app.
        /// </summary>
        /// <value>
        /// The app.
        /// </value>
        public string App { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>
        /// The entity id.
        /// </value>
        public string EntityId { get; set; }
    }
}
