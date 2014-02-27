
namespace Bfw.PX.PXPub.Models
{
    public class ExternalContent : ContentItem
    {
        /// <summary>
        /// Whether this is an arga item with Url transformed
        /// </summary>
        public bool IsTransformedArgaItem { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalContent"/> class.
        /// </summary>
        public ExternalContent()
        {
            Type = "ExternalContent";
            AllowComments = true;
        }
    }
}
