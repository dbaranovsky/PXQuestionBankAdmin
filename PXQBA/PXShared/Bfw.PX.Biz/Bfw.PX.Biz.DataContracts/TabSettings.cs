using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
	/// <summary>
	/// Course tab settings
	/// </summary>
	public class TabSettings
	{
		public ViewTab view_tab { get; set; }

		public TabSettings()
		{
			this.view_tab = new ViewTab();
		}
	}

    [Serializable]
	//subclass to match an XML structure
	public class ViewTab
	{
		public bool show_policies { get; set; }

		public bool show_assignment_details { get; set; }

		public bool show_rubrics { get; set; }

		public bool show_learning_objectives { get; set; }

		public ViewTab()
		{
			//this.show_policies = false;
			//this.show_assignment_details = true;
			//this.show_rubrics = true;
			//this.show_learning_objectives = true;
		}


	}


}