using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Bfw.Common
{
    /// <summary>
    /// Helps easily serializing and DeSerializing XML.
    /// </summary>
    public static class PxXmlSerializer
    {
        /// <summary>
        /// Serialize to XDocument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to be serialized.</param>
        /// <param name="rootName">RootName for the XML, takes the class Root name if it's empty.</param>
        /// <returns></returns>
        public static XDocument Serialize<T>(T obj, string rootName)
        {
            return Serialize(obj, rootName, false);
        }

        /// <summary>
        /// Serialize to XDocument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="rootName">RootName for the XML, takes the class Root name if its empty</param>
        /// <param name="allowNamespace">Set to true if namespace is needed</param>
        /// <returns></returns>
        public static XDocument Serialize<T>(T obj, string rootName, bool allowNamespace)
        {
            XmlRootAttribute xRoot = null;
            if (!string.IsNullOrEmpty(rootName))
            {
                xRoot = new XmlRootAttribute
                {
                    ElementName = rootName
                };
            }
            var result = new XDocument();
            var serializer = new XmlSerializer(obj.GetType(), xRoot);
            using (var writer = result.CreateWriter())
            {
                var namespaces = new XmlSerializerNamespaces();
                if (!allowNamespace)
                {
                    namespaces.Add(string.Empty, string.Empty);
                }
                serializer.Serialize(writer, obj, namespaces);
            }
            return result;
        }

        /// <summary>
        /// Deserializes the XML string to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Xml string.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string data)
        {
            return Deserialize<T>(data, null);
        }

        /// <summary>
        /// Deserializes the XML string to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">XML string.</param>
        /// <param name="type">Object type to deserialize.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string data, Type type)
        {
            if (type == null)
                type = typeof(T);

            T obj;
            var serializer = new XmlSerializer(type);
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                mem.Seek(0, SeekOrigin.Begin);
                obj = (T)serializer.Deserialize(mem);
            }
            return obj;
        }
    }
}
