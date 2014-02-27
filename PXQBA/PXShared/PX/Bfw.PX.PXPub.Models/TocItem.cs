using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Model for an item of the table of contents
    /// </summary>
    public class TocItem
    {
        /// <summary>
        /// The item's title
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// The entity ID of the item
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>
        /// <value>
        /// The parent id.
        /// </value>
        public string ParentId { get; set; }

        /// <summary>
        /// The item's description
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Text to be display as tool tip in toc
        /// </summary>
        public string ToolTip { get; set; }
        /// <summary>
        /// The type of the item
        /// </summary>
        /// <value>
        /// The type of the item.
        /// </value>
        public string ItemType { get; set; }

        /// <summary>
        /// What level the item is at in the heirarchy
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        public int Level { get; set; }

        /// <summary>
        /// Relative order of this item to items with the same parent
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        public string Sequence { get; set; }

        /// <summary>
        /// True if this TocItem represents a link to an actual piece of content, i.e. shouldn't contain
        /// subnavigation
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is content; otherwise, <c>false</c>.
        /// </value>
        public bool IsContent { get; set; }

        /// <summary>
        /// True if this item is the "active" item
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// True if the item is hidden from students, false otherwise
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is hidden from students; otherwise, <c>false</c>.
        /// </value>
        public bool IsHiddenFromStudents { get; set; }

        /// <summary>
        /// True if the view and delete controls should be shown
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show controls]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowControls { get; set; }

        /// <summary>
        /// If this is a quiz item, how many questions does the quiz have.
        /// </summary>
        /// <value>
        /// The question count.
        /// </value>
        public int QuestionCount { get; set; }

        /// <summary>
        /// True if the TocItem represents and item that has been assigned
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is assigned; otherwise, <c>false</c>.
        /// </value>
        public bool IsAssigned { get; set; }

        /// <summary>
        /// The date the item is due if it has been assigned. DateTime.Min otherwise
        /// </summary>
        /// <value>
        /// The due date.
        /// </value>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The possible points of the item if it has been assigned. DateTime.Min otherwise
        /// </summary>
        /// <value>
        /// The max points.
        /// </value>
        public Double MaxPoints { get; set; }

        /// <summary>
        /// A list of this item's children
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public IEnumerable<TocItem> Children { get; set; }

        /// <summary>
        /// Gets or sets the parent lesson.
        /// </summary>
        /// <value>
        /// The parent lesson.
        /// </value>
        public string ParentLesson { get; set; }

        /// <summary>
        /// Gets or sets the parent syllbabus.
        /// </summary>
        /// <value>
        /// The parent syllbabus.
        /// </value>
        public string ParentSyllbabus { get; set; }


        /// <summary>
        /// Indicates if this item is part of the Assignment Center;
        /// </summary>
        public bool IsPartOfAssignmentCenter { get; set; }

        /// <summary>
        /// No of notes for that item
        /// </summary>
        public string NotesCount { get; set; }

        /// <summary>
        /// Indicates if the content is created by student
        /// </summary>
        public bool IsStudentCreated { get; set; }

        /// <summary>
        /// Indicates whether the item is locked from editing for the current course
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Indicates whether items like reflection assignments are not exportable
        /// </summary>
        public bool IsNotExportable { get; set; }

        /// <summary>
        /// Contructor requiring certain title, id, and children
        /// </summary>
        /// <param name="title">The title for the TocItem to create</param>
        /// <param name="id">The ID for the TocItem to create</param>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="description">The description.</param>
        /// <param name="children">A list of children for this TocItem to show beneath it</param>
        public TocItem(string title, string id, string itemType, string description, IEnumerable<TocItem> children)
        {
            Title = title;
            Id = id;
            ItemType = itemType;
            Description = description;
            Children = children;
        }
    }
}
