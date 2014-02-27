//-----------------------------------------------------------------------
// <copyright file="IPagedList.cs" company="KPMG">
//     Copyright (c) KPMG LLP.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Bfw.Common.JqGridHelper {
    /// <summary>
    /// Interface for Page List. 
    /// </summary>
    public interface IPagedList<T> : IList<T>
    {
        /// <summary>
        /// Page Count 
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Total Item count 
        /// </summary>
        int TotalItemCount { get; }

        /// <summary>
        /// Page Index 
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// Page Number 
        /// </summary>
        int PageNumber { get; }

        /// <summary>
        /// Page Size 
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Previous Page 
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// Next Page 
        /// </summary>
        bool HasNextPage { get; }

        /// <summary>
        /// To check: Is this the First Page 
        /// </summary>
        bool IsFirstPage { get; }

        /// <summary>
        /// To check: Is this the Last Page 
        /// </summary>
        bool IsLastPage { get; }
    }
}