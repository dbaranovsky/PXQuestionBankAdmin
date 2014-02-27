using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Bfw.Common.Collections;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents the state of the AssignmentCenter's navigation and potentially
    /// some operation that is changing it.
    /// </summary>
    /// 
    [DataContract]
    public class AssignmentCenterNavigationState
    {
        /// <summary>
        /// The category whose state is changing.
        /// </summary>
        /// 
        [DataMember(Name = "category")]
        public AssignmentCenterCategory Category { get; set; }

        /// <summary>
        /// The item that triggered the state change.
        /// </summary>
        /// 
        [DataMember(Name = "changed")]
        public AssignmentCenterItem Changed { get; set; }

        /// <summary>
        /// The item that triggered the state change.
        /// </summary>
        /// 
        [DataMember(Name = "changes")]
        public List<AssignmentCenterItem> Changes { get; set; }

        /// <summary>
        /// The sibling above the item that has changed, or null.
        /// </summary>
        /// 
        [DataMember(Name = "above")]
        public AssignmentCenterItem Above { get; set; }

        /// <summary>
        /// The sibling below the item that has changed, or null.
        /// </summary>
        /// 
        [DataMember(Name = "below")]
        public AssignmentCenterItem Below { get; set; }

        /// <summary>
        /// The entity id that the item belongs to.
        /// </summary>
        /// 
        [DataMember(Name = "entityId")]
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the unit container.
        /// </summary>
        /// <value>
        /// The unit container.
        /// </value>
        [DataMember(Name = "container")]
        public AssignmentUnitContainer UnitContainer { get; set; }

        /// <summary>
        /// The operation that has changed the state.
        /// </summary>
        /// 
        [DataMember(Name = "operation")]
        public AssignmentCenterOperation Operation { get; set; }

        [DataMember(Name = "toc")]
        public string Toc { get; set; }
        
        /// <summary>
        /// A string of toc filter separated by commas where the item is to be removed from.
        /// </summary>
        /// <value>
        /// </value>
        [DataMember(Name = "removeFrom")]
        public string RemoveFrom { get; set; }

        public bool IsIgnoreChildren { get; set; }

        /// <summary>
        /// Find the node with the given id.
        /// </summary>
        /// <param name="id">id of the node to find.</param>
        /// <returns>Node with matching id, or null if node not found.</returns>
        public AssignmentCenterItem FindItem(string id)
        {
            AssignmentCenterItem found = null;
            if (Category != null && !Category.Items.IsNullOrEmpty())
            {
                foreach (var node in Category.Items)
                {
                    found = FindItem(node, node.Children, id);

                    if (found != null)
                    {
                        break;
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// Depth first search through the tree searching for the node with the given id.
        /// </summary>
        /// <param name="root">root node in the tree.</param>
        /// <param name="children">child nodes of root.</param>
        /// <param name="id">id of the node being searched for.</param>
        /// <returns>Node that matches id, or null if no node was found.</returns>
        protected AssignmentCenterItem FindItem(AssignmentCenterItem root, List<AssignmentCenterItem> children, string id)
        {
            AssignmentCenterItem found = null;

            if (root.Id == id)
            {
                return root;
            }
            else if(!children.IsNullOrEmpty())
            {
                foreach (var child in children)
                {
                    child.Parent = root;
                    found = FindItem(child, child.Children, id);

                    if (found != null)
                    {
                        break;
                    }
                }
            }

            return found;
        }
    }

    [DataContract]
    public class AssignmentUnitContainer
    {
        /// <summary>
        /// Gets or sets the toc.
        /// </summary>
        /// <value>
        /// The toc.
        /// </value>
        [DataMember(Name = "toc")]
        public string Toc { get; set; }

        /// <summary>
        /// Gets or sets the unit (Subcontainer Id) unique identifier.
        /// </summary>
        /// <value>
        /// The unit unique identifier.
        /// </value>
        [DataMember(Name = "unitId")]
        public string UnitId { get; set; }


        /// <summary>
        /// Gets or sets the category unique identifier.
        /// </summary>
        /// <value>
        /// The category unique identifier.
        /// </value>
        [DataMember(Name = "categoryId")]
        public string CategoryId { get; set; }
    }
}
