using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;

namespace Bfw.PXWebAPI.Helpers
{
    public class ItemActions
    {
        protected ISessionManager SessionManager { get; set; }

        public ItemActions(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }


        public List<Item> GetItems(string courseId, string itemId, string query = "")
        {
            var cmd = new Agilix.Commands.GetItems
            {
                SearchParameters = new ItemSearch
                {
                    EntityId = courseId,
                    ItemId = itemId,
                    Query = query
                }
            };

            SessionManager.CurrentSession.Execute(cmd);

            if (!cmd.Items.IsNullOrEmpty())
            {
                return cmd.Items;
            }
            return null;
        }

    }
}
