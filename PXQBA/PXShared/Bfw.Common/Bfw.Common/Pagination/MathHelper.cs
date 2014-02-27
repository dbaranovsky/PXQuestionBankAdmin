using System;

namespace Bfw.Common.Pagination
{
    internal static class MathHelper
    {
        internal static int CalcLastItem(int currentPage, int pageSize, int totalItems)
        {
        	int tmp = currentPage * pageSize;

        	return tmp > totalItems ? totalItems : tmp;
        }

    	internal static int CalcFirstItem(int currentPage, int pageSize, int totalItems)
        {
            if (totalItems == 0) return 0;
            
            return (currentPage * pageSize) - (pageSize - 1);
        }

        internal static int CalcNextPage(int currentPage, int totalPages)
        {
            if (currentPage == totalPages) return totalPages;
      
            return currentPage + 1;
            
        }

        internal static int CalcPreviousPage(int currentPage)
        {
            if (currentPage == 1) return 1;
 
            return currentPage - 1;
            
        }

        internal static int CalcTotalPages(int totalItems, int pageSize)
        {
            double tmp = Math.Ceiling(totalItems / (double)pageSize);
            
			return Convert.ToInt32(tmp);
        }

        internal static int CalcUpperBound(int currentPage, int pagesInPager, int totalPages)
        {
            int lowerBound = CalcLowerBound(currentPage, pagesInPager, totalPages);
            int upperBound = lowerBound + (pagesInPager - 1);

            return upperBound > totalPages ? totalPages : upperBound;
        }

        internal static int CalcLowerBound(int currentPage, int pagesInPager, int totalPages)
        {
            double rawIntervalsNumber = totalPages / (double)pagesInPager;
            double tmp = totalPages / (double)currentPage;
            double rawCurrentInvertal = rawIntervalsNumber / tmp;

            if (double.IsNaN(rawCurrentInvertal)) return 0;          

            if (rawCurrentInvertal <= 1) return 1;
            
            int lowerBound = Convert.ToInt32((Math.Floor(rawCurrentInvertal) * pagesInPager) + 1);

            if (lowerBound > currentPage) return currentPage - pagesInPager + 1;
            
            return lowerBound;
        }
    }
}
