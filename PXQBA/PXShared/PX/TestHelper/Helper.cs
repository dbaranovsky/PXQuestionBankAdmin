using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TestHelper
{
    public static class Helper
    {
        const string XML_ROOT_ITEM = "rootItem";
        const string XML_KEY = "type";

        public static XDocument GetResponse(Entity entity, string key)
        {
            XDocument result = new XDocument();

            string content = GetContent(entity);
            XDocument doc = XDocument.Parse(content);
            var target = doc.Root.Elements(XML_ROOT_ITEM).SingleOrDefault(e => e.Attribute(XML_KEY).Value.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (target != null)
            {
                result.Add(target.Elements().First());
            }

            return result;
        }

        public static string GetContent(Entity entity)
        {
            Assembly assembly;
            StreamReader textStreamReader;
            string result = String.Empty;
            string resourceName = String.Format("TestHelper.XML.{0}.xml", entity.ToString());

            try
            {
                assembly = Assembly.GetExecutingAssembly();
                using (StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(resourceName)))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
                result = String.Empty;
            }

            return result;
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static XDocument GetXDocument(string documentName)
        {
            documentName = "TestHelper.XML." + documentName + ".xml";
            XDocument doc;
            var assembly = Assembly.GetExecutingAssembly();
            using (StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(documentName)))
            {
                doc = XDocument.Load(sr);
            }
            return doc;
        }
    }

    public enum Entity
    {
        Item,
        Item_WithNoDlapExactInContainers,
        Enrollment,
        GroupList,
        Course,
        Messages,
        Questions,
        QuestionAnalysis,
        RunTask,
        GetDueSoonList,
        ListCourses,
        QuestionWithoutQuestionVersion,
        QuestionWithoutResourceEntityId,
        GetEnrollment3,
        Signal
    }
}
