using System;
using System.Collections.Generic;
using System.Linq;

namespace Blend.Episerver.Search
{
    public class GroupPaginationModel
    {
        public GroupPaginationModel(int pageSize, int maxPages, int pageIndex, int totalItems)
        {
            // calculate total pages
            var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);

            // ensure current page isn't out of range
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            else if (pageIndex > totalPages)
            {
                pageIndex = totalPages;
            }

            int startPage;
            int endPage;

            if (totalPages <= maxPages)
            {
                // total pages less than max so show all pages
                startPage = 1;
                endPage = totalPages;
            }
            else
            {
                // total pages more than max so calculate start and end pages
                var maxPagesBeforeCurrentPage = (int)Math.Floor(maxPages / (decimal)2);
                var maxPagesAfterCurrentPage = (int)Math.Ceiling(maxPages / (decimal)2) - 1;

                if (pageIndex <= maxPagesBeforeCurrentPage)
                {
                    // current page near the start
                    startPage = 1;
                    endPage = maxPages;
                }
                else if (pageIndex + maxPagesAfterCurrentPage >= totalPages)
                {
                    // current page near the end
                    startPage = totalPages - maxPages + 1;
                    endPage = totalPages;
                }
                else
                {
                    // current page somewhere in the middle
                    startPage = pageIndex - maxPagesBeforeCurrentPage;
                    endPage = pageIndex + maxPagesAfterCurrentPage;
                }
            }

            // calculate start and end item indexes
            var startIndex = (pageIndex - 1) * pageSize;
            var endIndex = Math.Min(startIndex + pageSize - 1, totalItems - 1);

            // create an array of pages that can be looped over
            var pages = Enumerable.Range(startPage, (endPage + 1) - startPage);

            // update object instance with all pager properties required by the view
            this.TotalItems = totalItems;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.TotalPages = totalPages;
            this.StartPage = startPage;
            this.EndPage = endPage;
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
            this.Pages = pages;
            this.GetUrl = (int page) => "?p=" + page;
        }

        public int TotalItems { get; private set; }

        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public int TotalPages { get; private set; }

        public int StartPage { get; private set; }

        public int EndPage { get; private set; }

        public int StartIndex { get; private set; }

        public int EndIndex { get; private set; }

        public IEnumerable<int> Pages { get; private set; }

        public Func<int, string> GetUrl { get; set; }

        public static readonly GroupPaginationModel Empty = new GroupPaginationModel(10, 1, 1, 1);
    }
}