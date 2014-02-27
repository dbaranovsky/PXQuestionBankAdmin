using System;
namespace HTS
{
	public class CHTSIproNav
	{
		internal string navtype = "";
		internal string nextStepID = "";
		public CHTSIproNav(string nav, string next)
		{
			this.navtype = nav;
			this.nextStepID = next;
		}
	}
}
