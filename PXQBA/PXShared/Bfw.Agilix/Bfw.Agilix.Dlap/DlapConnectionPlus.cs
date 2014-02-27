using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Linq;
using Bfw.Common.Collections;
using Bfw.Common.Logging;


namespace Bfw.Agilix.Dlap
{
#if (PERFORMANCE)
    /// <summary>
    /// This class has a special logging feature to track DLAP commands
    /// </summary>
    public class DlapConnectionPlus : DlapConnection
    {

        protected string FilePath { get; set; }
        protected string actionName;
        protected string actionDescription;
        protected string url;
        /// <summary>
        /// In order to create a connection, you must specify the Url to the DLAP server. This value
        /// can not be changed after the object has been created
        /// </summary>
        /// <param name="dlapUrl"></param>
        public DlapConnectionPlus(string dlapUrl)
            : base(dlapUrl)
        {
            FilePath = HttpContext.Current.Request.MapPath("~/dlapCommandStackTrace.txt");
            var actionNameItem = HttpContext.Current.Items["EXECUTING_ACTION"];
            actionName = (actionNameItem ?? "").ToString();
            actionDescription = !String.IsNullOrEmpty(actionName) ? String.Format(", [{0}]", actionName) : "";
            url = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
        }

        /// <summary>
        /// Sends a request to the DLAP server and returns the response
        /// </summary>
        /// <param name="request">Request to send to DLAP</param>
        /// <returns>Response sent by DLAP</returns>
        public override DlapResponse Send(DlapRequest request)
        {
            var callMessage = String.Join("&", request.Parameters.Keys.Map(k => k + "=" + request.Parameters[k]));
            var msg = "";
            using (Tracer.DoTrace("DlapConnection.Send"))
            {
                System.Diagnostics.Stopwatch sw = null;

                var logDlap = Logger == null ? false : Logger.ShouldLog("DLAP");

                sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                HttpWebResponse webResponse = Transmit(request);
                DlapResponse response = ProcessResponse(webResponse);

                sw.Stop();

                if (String.IsNullOrEmpty(callMessage))
                {
                    var xmlRequestBody = request.GetXmlRequestBody();
                    msg = xmlRequestBody.ToString();
                    callMessage = xmlRequestBody != null
                        ? xmlRequestBody.ToString().Replace("\n", "")
                        : "Could not determine DLAP command specifics.";
                }
                else
                    msg = callMessage;

                msg = String.Format("{0:H:mm:ss}, DLAP call ({2:ss\\.ffffff}): {3}{1} [{4}]", DateTime.Now,
                    actionDescription, sw.Elapsed, msg, url);
                if (logDlap)
                {
                    Logger.Debug(msg, LogSeverity.Debug, new List<string>() { "DLAP" });
                }
                StackWriter.Write(msg, FilePath);


                return response;
            }
        }
    }
#endif

    /// <summary>
    /// Manages the creation of DlapConnection instances
    /// </summary>
    public static class ConnectionFactory
    {
        /// <summary>
        /// Factory method that decides whether to return an instance
        /// that logs dlap command call stacks
        /// </summary>
        /// <param name="dlapUrl"></param>
        /// <returns></returns>
        public static DlapConnection GetDlapConnection(string dlapUrl)
        {
#if (!PERFORMANCE)
            return new DlapConnection(dlapUrl);
#else
            return new DlapConnectionPlus(dlapUrl);
#endif
        }
    }

#if (PERFORMANCE)
    internal class StackWriter
    {
        private StackWriter() { }
        private static readonly object myLock = new object();
        private static string[] filtered = { "System.", "Bfw.Agilix", "Bfw.Common", "*_index_aspx.ProcessRequest" };

        public static void Write(string command, string FilePath)
        {
            StackTrace stack = new StackTrace();
            StringBuilder sb = new StringBuilder("\r\n" + command + "\r\n");
            for (int i = 1; i < stack.GetFrames().Length; i++)
            {
                var method = stack.GetFrame(i).GetMethod();
                var type = method.ReflectedType;
                if (type != null)
                {
                    var name = String.Format("{0}.{1}", type.FullName, method.Name);
                    if (!filtered.Any(f => { return !f.StartsWith("*") ? name.StartsWith(f) : name.Contains(f.Substring(1)); }))
                        sb.AppendLine(name);
                }
            }
            lock (myLock)
            {
                using (StreamWriter sw = new StreamWriter(FilePath, true))
                {
                    sw.Write(sb.ToString());
                }
            }
        }
    }
#endif
}