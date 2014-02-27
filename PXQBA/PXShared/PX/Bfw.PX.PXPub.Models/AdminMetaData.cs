using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Bfw.Common.DynamicExtention;

namespace Bfw.PX.PXPub.Models
{
	/// <summary>
	/// AdminMetaData contains information about ContentItem Metadata Elements
	/// </summary>
	[Serializable()]
	[XmlType(AnonymousType = true)]
	[XmlRoot("bfw_metadata_admin")]
	public class AdminMetaData
	{

		/// <summary>
		/// MetaDataElementsCollection contains list of Metadata Elements to display in MetaDataEditor
		/// </summary>
		[XmlElementAttribute("metaDataElements", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public MetaDataElements MetaData { get; set; }

	}

	/// <summary>
	/// MetaDataElements Collection contains list of Metadata Elements to display in MetaDataEditor
	/// </summary>
	[Serializable()]
	[XmlType(AnonymousType = true)]
	public class MetaDataElements //: IEnumerable<MetaDataElement>
	{
		private List<MetaDataElement> metaDataElements;

		/// <summary>
		/// MetaDataElements Collection contains list of Metadata Elements to display in MetaDataEditor
		/// </summary>
		[XmlElementAttribute("metaDataElement", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public List<MetaDataElement> Elements
		{
			get
			{
				return metaDataElements;
			}
			set
			{
				metaDataElements = value;
			}
		}
		//public IEnumerator<MetaDataElement> GetEnumerator()
		//{
		//    return null;
		//}


	}

	/// <summary>
	/// MetaDataElement contains instructions how to display MetaData in MetaDataEditor
	/// </summary>
	[Serializable()]
	[XmlType(AnonymousType = true)]
	public class MetaDataElement
	{
		/// <summary>
		/// MetaDataElement Name
		/// </summary>
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// MetaDataElement DisplayOrder
		/// </summary>
		[XmlAttribute("displayOrder")]
		public string DisplayOrder { get; set; }

		/// <summary>
		/// MetaDataElement Action
		/// </summary>
		[XmlAttribute("action")]
		public string Action { get; set; }

		/// <summary>
		/// MetaDataElement Controller
		/// </summary>
		[XmlAttribute("controller")]
		public string Controller { get; set; }

		/// <summary>
		/// MetaDataElement XPath
		/// </summary>
		[XmlAttribute("xPath")]
		public string XPath { get; set; }

		/// <summary>
		/// MetaDataElement Description
		/// </summary>
		[XmlAttribute("description")]
		public string Description { get; set; }

		/// <summary>
		/// MetaDataElement DefaultValue
		/// </summary>
		[XmlAttribute("defaultValue")]
		public string DefaultValue { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ElasticObject ElasticControl { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public XElement XElasticControl { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public XElement XElasticMetaNodeToSave { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public XElement XElasticData { get; set; }


		private ElasticObject _elasticData;
		public ElasticObject ElasticData
		{
			get { return _elasticData ?? ( _elasticData = new ElasticObject("ElasticData") ); }

			set
			{
				_elasticData = value;
			}
		}
	}
}
