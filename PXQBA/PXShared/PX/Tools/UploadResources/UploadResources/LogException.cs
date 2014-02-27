using System;
using System.Collections;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace UploadResources.Exception
{
    /// <summary>
    /// Publish the exception using LogException.Publish method into XML File.
    /// </summary>
    public static class LogException
    {
 
        public static string Publish(System.Exception userException)
        {
            string errorReferenceNumber = "";
            string logFileName;

            errorReferenceNumber = GenerateErrorNumber();
            logFileName = ConfigurationManager.AppSettings.Get("ErrorLogFolder") + errorReferenceNumber + ".xml";
            

            try
            {                
                var xmlWriter = new XmlTextWriter(logFileName, Encoding.Default);

                //Exception log start
                xmlWriter.WriteStartElement("UploadResourcesException");
                xmlWriter.WriteElementString("ErrorNumber", errorReferenceNumber);
                xmlWriter.WriteElementString("MachineName", Environment.MachineName);
                xmlWriter.WriteElementString("ExceptionDate", DateTime.Now.ToString());
                xmlWriter.WriteElementString("Message", CleanMessage(userException.Message));
                xmlWriter.WriteElementString("Source", userException.Source);

                //Base Exception
                if (userException.GetBaseException() != null)
                    WriteException(userException.GetBaseException(), xmlWriter, "BaseException");

                //Final exception
                WriteException(userException, xmlWriter, "FinalException");

                //Inner exceptions
                System.Exception innerException = userException;
                int index = 0;

                while (true)
                {
                    innerException = innerException.InnerException;
                    if (innerException == null)
                    {
                        if (index > 0)
                            xmlWriter.WriteEndElement();
                        break;
                    }
                    index += 1;
                    if (index == 1)
                        xmlWriter.WriteStartElement("InnerExceptions");

                    //WriteException(innerException, xmlWriter, "InnerException_" + index);

                    int lastIndexOfPeriod = innerException.GetType().ToString().LastIndexOf(".");
                    WriteException(innerException, xmlWriter,
                                    innerException.GetType().ToString().Remove(0, lastIndexOfPeriod + 1));
                }




                //Exception log end
                xmlWriter.WriteEndElement();

                //Close XMLWriter
                xmlWriter.Close();
            }
            catch (System.Exception ex)
            {
                errorReferenceNumber = "";
            }

            SendMessage(logFileName);

            return errorReferenceNumber;
        }


        private static void WriteException(System.Exception userException, XmlWriter xmlWriter, string exceptionName)
        {
            xmlWriter.WriteStartElement(exceptionName);
            xmlWriter.WriteElementString("Message", CleanMessage(userException.Message));
            xmlWriter.WriteElementString("Source", userException.Source);
            xmlWriter.WriteElementString("Type", userException.GetType().ToString());
            xmlWriter.WriteElementString("StackTrace", userException.StackTrace);
            xmlWriter.WriteElementString("TargetSite",
                                          userException.TargetSite != null ? userException.TargetSite.Name : "");
            xmlWriter.WriteStartElement("Data");
            foreach (DictionaryEntry item in userException.Data)
                xmlWriter.WriteElementString(item.Key.ToString(), item.Value.ToString());
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        private static string GenerateErrorNumber()
        {
            var randomObject = new Random();
            return DateTime.Now.ToString("yyyyMMddHHmmssf") + randomObject.Next(10000000, 99999999);
        }

        private static string CleanMessage(string message)
        {
            //Get rid of (replace with a space) any character outside of the ASCII range 32-126.
            const string pattern = @"[^\x20-\x7E]";
            var rex = new Regex(pattern);
            return rex.Replace(message, " ");
        }

        private static void SendMessage(string attachmentPath)
        {
            //var from = new MailAddress(ConfigurationManager.AppSettings["ErrorFromEmail"]);
            //var to = new MailAddress(ConfigurationManager.AppSettings["ErrorToEmail"]);
            string to = ConfigurationManager.AppSettings["ErrorFromEmail"];
            string from = ConfigurationManager.AppSettings["ErrorToEmail"];
            var attachFile = new Attachment(attachmentPath);
            var emailClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpHost"]);
            try
            {
                
                using (var MyMessage = new MailMessage(from, to))
                {
                    // setup message details here
                    MyMessage.Subject = "New Error Reported on Agilix Resource Uploader";
                    MyMessage.Body = "Error Occurred in Angel -> Agilix Resource Uploader";
                    MyMessage.Attachments.Add(attachFile);
                    emailClient.Send(MyMessage);
                }
            }
            catch (System.Exception)
            {
                //throw;
            }
        }
    }
}
