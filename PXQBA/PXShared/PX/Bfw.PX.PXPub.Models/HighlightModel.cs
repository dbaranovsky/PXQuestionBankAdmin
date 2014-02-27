using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Contains all data required to display a resource
    /// </summary>
    public class HighlightModel
    {
        /// <summary>
        /// Gets or sets the highlight id.
        /// </summary>
        /// <value>
        /// The highlight id.
        /// </value>
        public string HighlightId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the highlight.
        /// </summary>
        /// <value>
        /// The type of the highlight.
        /// </value>
        public PxHighlightType HighlightType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="HighlightModel"/> is public.
        /// </summary>
        /// <value>
        ///   <c>true</c> if public; otherwise, <c>false</c>.
        /// </value>
        public bool Public
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the note.
        /// </summary>
        /// <value>
        /// The type of the note.
        /// </value>
        public NoteType NoteType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public HighlightStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>
        /// The item id.
        /// </value>
        public string ItemId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the enrollment id.
        /// </summary>
        /// <value>
        /// The enrollment id.
        /// </value>
        public string EnrollmentId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the review id.
        /// </summary>
        /// <value>
        /// The review id.
        /// </value>
        public string ReviewId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public string Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is user highlight.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is user highlight; otherwise, <c>false</c>.
        /// </value>
        public bool IsUserHighlight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is instructor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is instructor; otherwise, <c>false</c>.
        /// </value>
        public bool IsInstructor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        public IList<Note> Notes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show rubrics].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show rubrics]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowRubrics
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the rubrics guide.
        /// </summary>
        /// <value>
        /// The rubrics guide.
        /// </value>
        public string[] RubricsGuide
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the allow share comment value
        /// </summary>
        /// <value>
        /// whether sharing comments is allowed
        /// </value>
        public bool AllowShareComments
        {
            get;
            set;
        }
    }
}
