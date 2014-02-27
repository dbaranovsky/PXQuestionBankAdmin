using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    public class Container
    {
        public string Toc { get; set; }
        public string Value { get; set; }
		public string DlapType { get; set; }

		public Container(string t, string v)
		{
			Toc = t;
			Value = v;
		}

		public Container(string t, string v, string d)
		{
			Toc = t;
			Value = v;
			DlapType = d;
		}
    }
}
