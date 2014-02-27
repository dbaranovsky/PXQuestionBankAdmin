//-----------------------------------------------------------------------
// <copyright file="PagedList.cs" company="KPMG">
//     Copyright (c) KPMG LLP.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bfw.Common.JqGridHelper {
    /// <summary>
    /// Implementation for Page List 
    /// </summary>

    public class PagedList<T> : List<T>, IPagedList<T>
    {

        /// <summary>
        /// Contructor for PagedList
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// </summary>
        public PagedList(IEnumerable<T> source, int index, int pageSize)
            : this(source, index, pageSize, null)
        {
        }

        /// <summary>
        /// Contructor for PagedList
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// </summary>
        public PagedList(IEnumerable<T> source, int index, int pageSize, int? totalCount)
        {
            Initialize(source.AsQueryable(), index, pageSize, totalCount);
        }

        /// <summary>
        /// Contructor for PagedList
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// </summary>
        public PagedList(IQueryable<T> source, int index, int pageSize)
            : this(source, index, pageSize, null)
        {
        }

        /// <summary>
        /// Contructor for PagedList
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// </summary>
        public PagedList(IQueryable<T> source, int index, int pageSize, int? totalCount)
        {
            Initialize(source, index, pageSize, totalCount);
        }

        #region IPagedList Members

        /// <summary>
        /// Page Count
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// Total Item count
        /// </summary>
        public int TotalItemCount { get; private set; }

        /// <summary>
        /// Page Index
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Page Number
        /// </summary>
        public int PageNumber { get { return PageIndex + 1; } }

        /// <summary>
        /// Page Size
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Has any Previous Page
        /// </summary>
        public bool HasPreviousPage { get; private set; }

        /// <summary>
        /// Has any Next Page
        /// </summary>
        public bool HasNextPage { get; private set; }

        /// <summary>
        /// Is this the First Page
        /// </summary>
        public bool IsFirstPage { get; private set; }

        /// <summary>
        /// Is this the Last Page
        /// </summary>
        public bool IsLastPage { get; private set; }

        #endregion

        /// <summary>
        /// To Initialize
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// </summary>
        protected void Initialize(IQueryable<T> source, int index, int pageSize, int? totalCount)
        {
            //### argument checking
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("PageIndex cannot be below 0.");
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("PageSize cannot be less than 1.");
            }

            //### set source to blank list if source is null to prevent exceptions
            if (source == null)
            {
                source = new List<T>().AsQueryable();
            }

            //### set properties
            if (!totalCount.HasValue)
            {
                TotalItemCount = source.Count();
            }
            PageSize = pageSize;
            PageIndex = index;
            if (TotalItemCount > 0)
            {
                PageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);
            }
            else
            {
                PageCount = 0;
            }
            HasPreviousPage = (PageIndex > 0);
            HasNextPage = (PageIndex < (PageCount - 1));
            IsFirstPage = (PageIndex <= 0);
            IsLastPage = (PageIndex >= (PageCount - 1));

            //### add items to internal list
            if (TotalItemCount > 0)
            {
                AddRange(source.Skip((index) * pageSize).Take(pageSize).ToList());
            }
        }
    }
}