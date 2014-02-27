//-----------------------------------------------------------------------
// <copyright file="JqGridData.cs" company="KPMG">
//     Copyright (c) KPMG LLP.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Bfw.Common.JqGridHelper {
    /// <summary>
    /// This type is designed to conform to the structure required by the JqGrid JavaScript component. 
    /// It has all of the properties required by the grid. When this type is serialized to JSON, the resulting 
    /// JSON will be in the structure expected by the grid when it fetches pages of data via AJAX calls.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Jq",
        Justification = "JqGrid is the correct name of the JavaScript component this type is designed to support.")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Jq",
        Justification = "JqGrid is the correct name of the JavaScript component this type is designed to support.")]
    public class JqGridModel
    {
        /// <summary>
        /// Arbitrary data to be returned to the grid along with the row data. Leave null if not using. Must be serializable to JSON!
        /// </summary>
        public JqGridData Data { get; set; }

        /// <summary>
        /// Column Model.
        /// </summary>
        public object ColModel { get; set; }

        /// <summary>
        /// Column Names.
        /// </summary>
        public object ColNames { get; set; }
    }
}