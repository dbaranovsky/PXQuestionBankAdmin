using System.Runtime.Serialization;

namespace Bfw.PX.PXPub.Models
{
	[DataContract]
	public class Container
	{
		[DataMember]
		public string Toc { get; set; }

		[DataMember]
		public string Value { get; set; }

		[DataMember]
		public string DlapType { get; set; }

		public Container(string t, string v, string d)
		{
			Toc = t;
			Value = v;
			DlapType = d;
		}
		public Container(string t, string v)
		{
			Toc = t;
			Value = v;
		}
	}
}
