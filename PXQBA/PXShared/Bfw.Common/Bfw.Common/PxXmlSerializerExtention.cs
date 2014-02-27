using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Bfw.Common
{
	/// <summary>
	/// 
	/// </summary>
	public static class PxXmlSerializerExtention
	{
		
		/// <summary>
		/// IsSerializable
		/// </summary>
		/// <param name="obj"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private static bool IsSerializable<T>(this T obj)
		{
			if (obj is ISerializable) return true;
			return Attribute.IsDefined(obj.GetType(), typeof(SerializableAttribute));
		}

		

		/// <summary>
		/// Serialize to XDocument.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">Object to be serialized.</param>
		/// <param name="rootName">RootName for the XML, takes the class Root name if it's empty.</param>
		/// <returns></returns>
		public static XDocument Serialize<T>(this T obj, string rootName)
		{
			return !IsSerializable(obj) ? null : Serialize(obj, rootName, false);
		}

		/// <summary>
		/// Serialize to XDocument.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">Object to be serialized</param>
		/// <param name="rootName">RootName for the XML, takes the class Root name if its empty</param>
		/// <param name="allowNamespace">Set to true if namespace is needed</param>
		/// <returns></returns>
		public static XDocument Serialize<T>(this T obj, string rootName, bool allowNamespace)
		{
		    if ( !IsSerializable(obj)) return null;

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
		/// De-serializes the XML string to object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data">Xml string.</param>
		/// <returns></returns>
		public static T Deserialize<T>(this string data)
		{
			return !IsSerializable(data) ? default(T) : Deserialize<T>(data, null);
		}

		/// <summary>
		/// De-serializes the XML string to object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data">XML string.</param>
		/// <param name="type">Object type to de-serialize.</param>
		/// <returns></returns>
		public static T Deserialize<T>(this string data, Type type)
		{
			if (!IsSerializable(data)) return default(T);

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
