using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.Biz.Direct.Services
{
	public enum MetaDataAssociationType
	{
		none = 0,
		ebookfilter = 1,
		syllabusfilter = 2
	}

	public class AdminMetaDataActions : IAdminMetaDataActions
	{
		/// <summary>
		/// The IBusinessContext implementation to use.
		/// </summary>
		protected IBusinessContext Context { get; set; }


		protected IContentActions ContentActions { get; set; }

		/// <summary>
		/// Gets or sets the session manager.
		/// </summary>
		/// <value>
		/// The session manager.
		/// </value>
		protected ISessionManager SessionManager { get; set; }

		public AdminMetaDataActions(IBusinessContext context, ISessionManager sessionManager, IContentActions contentActions)
		{
			Context = context;
			SessionManager = sessionManager;
			ContentActions = contentActions;
		}

		/// <summary>
		/// Executes the status message against DLAP and returns the resulting
		/// status document.
		/// </summary>
		/// <returns>DLAP status document</returns>
		public XDocument GetStatus()
		{
			var cmd = new GetStatus();
			XDocument status = null;

			try
			{
				SessionManager.CurrentSession.Execute(cmd);
				status = cmd.Status;
			}
			catch (DlapException ex)
			{
				Context.Logger.Log(ex);
				status = null;
			}

			return status;
		}

		public void UpdateResourceSubType(string newResourceSubType, List<ContentItem> items)
		{
			var batch = new Batch();

			//foreach (var doc in searchResultSet.docs)
			foreach (var itm in items)
			{
				//update resourceSubType
				string itemXml = "<data><meta-content-type dlaptype='exact'>{0}</meta-content-type></data>";
				itemXml = String.Format(itemXml, newResourceSubType);

				Bfw.Agilix.DataContracts.Item contentItem = new Item
				{
					Id = itm.Id,
					EntityId = itm.CourseId,
					Data = XElement.Parse(itemXml)
				};

				var cmdPutItem = new PutItems();
				cmdPutItem.Items.Add(contentItem);

				batch.Add(cmdPutItem);
			}

			using (Context.Tracer.DoTrace("Update Resource SubType"))
			{
				if (!batch.Commands.IsNullOrEmpty())
				{
					SessionManager.CurrentSession.Execute(batch);
				}
			}
		}


		public void DeleteResourceSubType(List<ContentItem> items)
		{
			var batch = new Batch();

			foreach (var itm in items)
			{
				//delete resourceSubType
				const string itemXml = "<data><meta-content-type dlaptype='exact'/></data>";
				Bfw.Agilix.DataContracts.Item contentItem = new Item
				{
					Id = itm.Id,
					EntityId = itm.CourseId,
					Data = XElement.Parse(itemXml)
				};

				var cmdPutItem = new PutItems();
				cmdPutItem.Items.Add(contentItem);

				batch.Add(cmdPutItem);
			}

			using (Context.Tracer.DoTrace("Update Resource SubType"))
			{
				if (!batch.Commands.IsNullOrEmpty())
				{
					SessionManager.CurrentSession.Execute(batch);
				}
			}
		}
	}
}

