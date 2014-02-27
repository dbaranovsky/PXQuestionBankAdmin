using System;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Store information to represent an assignment
    /// </summary>
    public class DocumentToView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentToView"/> class.
        /// </summary>
        public DocumentToView()
        {
            AllowRelativeUrl = false;
            isAssignmentView = false;
            CommenterId = "";
            HighlightType = 0;
            IsReadOnly = false;
            IsCurrentUserContext = false;
            AllowComments = true;
            IsBinary = false;
            IsExernalContent = false;
            ShowRubrics = false;
            RubricsList = string.Empty;
            RubricsGuide = new string[] { };
        }

        /// <summary>
        /// Document ID
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
        /// Gets or sets the secondary id.
        /// </summary>
        /// <value>
        /// The secondary id.
        /// </value>
        public string SecondaryId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the peer review id.
        /// </summary>
        /// <value>
        /// The peer review id.
        /// </value>
        public string PeerReviewId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the discipline id.
        /// </summary>
        /// <value>
        /// The discipline id.
        /// </value>
        public string DisciplineId
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
        /// Gets or sets the type of the highlight.
        /// </summary>
        /// <value>
        /// The type of the highlight.
        /// </value>
        public int HighlightType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the highlight description.
        /// </summary>
        /// <value>
        /// The highlight description.
        /// </value>
        public string HighlightDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the highlight text.
        /// </summary>
        /// <value>
        /// The highlight text.
        /// </value>
        public string HighlightText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the comment text.
        /// </summary>
        /// <value>
        /// The comment text.
        /// </value>
        public string CommentText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the comment link.
        /// </summary>
        /// <value>
        /// The comment link.
        /// </value>
        public string CommentLink
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow comments].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow comments]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowComments
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the color of the highlight.
        /// </summary>
        /// <value>
        /// The color of the highlight.
        /// </value>
        public string HighlightColor
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DocumentToView"/> is locked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if locked; otherwise, <c>false</c>.
        /// </value>
        public bool Locked
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DocumentToView"/> is shared.
        /// </summary>
        /// <value>
        ///   <c>true</c> if shared; otherwise, <c>false</c>.
        /// </value>
        public bool Shared
        {
            get;
            set;
        }
        /// <summary>
        /// Url
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url
        {
            get;
            set;
        }


        /// <summary>
        /// External Url
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string ExternalUrl
        {
            get;
            set;
        }


        /// <summary>
        /// Highlight ID to showcase in document
        /// </summary>
        /// <value>
        /// The highlight id.
        /// </value>
        public Guid HighlightId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the note id.
        /// </summary>
        /// <value>
        /// The note id.
        /// </value>
        public string NoteId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is assignment view.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is assignment view; otherwise, <c>false</c>.
        /// </value>
        public Boolean isAssignmentView
        {
            get;
            set;
        }

        /// <summary>
        /// Some document types need to have relative URLs remain unmodified (e.g., HtmlDocument). This 
        /// value set to true indicates that the document should not be changed before being displayed.
        /// </summary>
        public bool AllowRelativeUrl { get; set; }

        /// <summary>
        /// Gets or sets the commenter id.
        /// </summary>
        /// <value>
        /// The commenter id.
        /// </value>
        public string CommenterId
        {
            get;
            set;
        }

        /// <summary>
        /// flag to indicate if the document is a binary file for download
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsBinary
        {
            get;
            set;
        }

        /// <summary>
        /// read only flag, if true document can not be commented
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// read only flag, if true document can not be commented
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is current user context; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsCurrentUserContext
        {
            get;
            set;
        }

        /// <summary>
        /// Whether current course is product course or not
        /// </summary>
        public bool IsProductCourse
        {
            get;
            set;
        }

        /// <summary>
        /// Should be false for all internal content like Peer Review, Assignemt etc. Only for external content it should be true. 
        /// Currenly set to true in ExternalContent.ascs
        /// </summary>
        public bool IsExernalContent
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
        /// Gets or sets the rubrics list.
        /// </summary>
        /// <value>
        /// The rubrics list.
        /// </value>
        public string RubricsList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is reflection assignment.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is reflection assignment; otherwise, <c>false</c>.
        /// </value>
        public bool IsReflectionAssignment
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the start xpath of highlighted text.
        /// </summary>
        /// <value>
        /// xpath info
        /// </value>
        public string start
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end xpath of highlighted text.
        /// </summary>
        /// <value>
        /// xpath info
        /// </value>
        public string end
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the start character offset of highlighted text.
        /// </summary>
        /// <value>
        /// offset
        /// </value>
        public int startOffset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end character offset of highlighted text.
        /// </summary>
        /// <value>
        /// offset
        /// </value>
        public int endOffset
        {
            get;
            set;
        }
    }
}
