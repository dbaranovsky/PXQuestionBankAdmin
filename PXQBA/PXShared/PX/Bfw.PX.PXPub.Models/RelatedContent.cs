// -----------------------------------------------------------------------
// <copyright file="RelatedContent.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RelatedContent : ContentItem
    {
        public IList<ContentItem> RelatedContents { get; set; }
    }
}
