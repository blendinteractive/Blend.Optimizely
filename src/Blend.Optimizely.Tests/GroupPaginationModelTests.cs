using System.Linq;
using Blend.Optimizely.Search;
using Xunit;

namespace Blend.Optimizely.Tests
{
    public class GroupPaginationModelTests
    {
        #region TotalPages
        [Fact]
        public void TotalPages_IsExact_WhenItemsDivisibleByPageSize()
        {
            var p = new GroupPaginationModel(10, 5, 1, 100);
            Assert.Equal(10, p.TotalPages);
        }

        [Fact]
        public void TotalPages_RoundsUp_WhenItemsNotDivisibleByPageSize()
        {
            var p = new GroupPaginationModel(10, 5, 1, 95);
            Assert.Equal(10, p.TotalPages);
        }
        #endregion

        #region PageIndex clamping
        [Fact]
        public void PageIndex_IsPreserved_WhenInRange()
        {
            var p = new GroupPaginationModel(10, 5, 5, 100);
            Assert.Equal(5, p.PageIndex);
        }

        [Fact]
        public void PageIndex_IsClamped_WhenBelowOne()
        {
            var p = new GroupPaginationModel(10, 5, 0, 100);
            Assert.Equal(1, p.PageIndex);
        }

        [Fact]
        public void PageIndex_IsClamped_WhenAboveTotalPages()
        {
            var p = new GroupPaginationModel(10, 5, 99, 100);
            Assert.Equal(10, p.PageIndex);
        }
        #endregion

        #region Window: all pages shown
        [Fact]
        public void StartPage_IsOne_WhenTotalPagesLessThanOrEqualToMaxPages()
        {
            var p = new GroupPaginationModel(10, 5, 2, 30); // 3 total pages ≤ maxPages 5
            Assert.Equal(1, p.StartPage);
        }

        [Fact]
        public void EndPage_IsTotalPages_WhenTotalPagesLessThanOrEqualToMaxPages()
        {
            var p = new GroupPaginationModel(10, 5, 2, 30); // 3 total pages ≤ maxPages 5
            Assert.Equal(3, p.EndPage);
        }

        [Fact]
        public void Pages_ContainsAllPages_WhenTotalPagesLessThanOrEqualToMaxPages()
        {
            var p = new GroupPaginationModel(10, 5, 2, 30); // 3 total pages ≤ maxPages 5
            Assert.Equal(new[] { 1, 2, 3 }, p.Pages.ToArray());
        }
        #endregion

        #region Window: near start
        // maxPages=5 → floor(5/2)=2. pageIndex ≤ 2 triggers the near-start branch.
        [Fact]
        public void StartPage_IsOne_WhenNearStart()
        {
            var p = new GroupPaginationModel(10, 5, 2, 100);
            Assert.Equal(1, p.StartPage);
        }

        [Fact]
        public void EndPage_IsMaxPages_WhenNearStart()
        {
            var p = new GroupPaginationModel(10, 5, 2, 100);
            Assert.Equal(5, p.EndPage);
        }

        [Fact]
        public void Pages_StartsAtOne_WhenNearStart()
        {
            var p = new GroupPaginationModel(10, 5, 2, 100);
            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, p.Pages.ToArray());
        }
        #endregion

        #region Window: near end
        // maxPages=5 → ceil(5/2)-1=2. pageIndex+2 ≥ totalPages triggers the near-end branch.
        [Fact]
        public void EndPage_IsTotalPages_WhenNearEnd()
        {
            var p = new GroupPaginationModel(10, 5, 9, 100);
            Assert.Equal(10, p.EndPage);
        }

        [Fact]
        public void StartPage_IsOffsetFromEnd_WhenNearEnd()
        {
            var p = new GroupPaginationModel(10, 5, 9, 100); // totalPages-maxPages+1 = 6
            Assert.Equal(6, p.StartPage);
        }

        [Fact]
        public void Pages_EndsAtTotalPages_WhenNearEnd()
        {
            var p = new GroupPaginationModel(10, 5, 9, 100);
            Assert.Equal(new[] { 6, 7, 8, 9, 10 }, p.Pages.ToArray());
        }
        #endregion

        #region Window: middle
        // pageIndex=6, maxPages=5: before=2, after=2 → window [4..8]
        [Fact]
        public void StartPage_IsPageIndexMinusFloorHalf_WhenMiddle()
        {
            var p = new GroupPaginationModel(10, 5, 6, 100);
            Assert.Equal(4, p.StartPage);
        }

        [Fact]
        public void EndPage_IsPageIndexPlusCeilHalfMinusOne_WhenMiddle()
        {
            var p = new GroupPaginationModel(10, 5, 6, 100);
            Assert.Equal(8, p.EndPage);
        }

        [Fact]
        public void Pages_IsWindowAroundCurrentPage_WhenMiddle()
        {
            var p = new GroupPaginationModel(10, 5, 6, 100);
            Assert.Equal(new[] { 4, 5, 6, 7, 8 }, p.Pages.ToArray());
        }
        #endregion

        #region StartIndex / EndIndex
        [Fact]
        public void StartIndex_IsZero_OnFirstPage()
        {
            var p = new GroupPaginationModel(10, 5, 1, 100);
            Assert.Equal(0, p.StartIndex);
        }

        [Fact]
        public void StartIndex_IsPageIndexMinusOneTimesPageSize()
        {
            var p = new GroupPaginationModel(10, 5, 4, 100);
            Assert.Equal(30, p.StartIndex);
        }

        [Fact]
        public void EndIndex_IsStartPlusPageSizeMinusOne_OnFullPage()
        {
            var p = new GroupPaginationModel(10, 5, 4, 100);
            Assert.Equal(39, p.EndIndex);
        }

        [Fact]
        public void EndIndex_IsTotalItemsMinusOne_OnLastPartialPage()
        {
            var p = new GroupPaginationModel(10, 5, 10, 95);
            Assert.Equal(94, p.EndIndex);
        }
        #endregion

        #region GetUrl
        [Fact]
        public void GetUrl_DefaultReturnsQueryStringPage()
        {
            var p = new GroupPaginationModel(10, 5, 1, 100);
            Assert.Equal("?p=5", p.GetUrl(5));
        }

        [Fact]
        public void GetUrl_CanBeReplaced()
        {
            var p = new GroupPaginationModel(10, 5, 1, 100);
            p.GetUrl = page => $"/results?page={page}";
            Assert.Equal("/results?page=5", p.GetUrl(5));
        }
        #endregion

        #region Empty
        [Fact]
        public void Empty_IsNotNull()
        {
            Assert.NotNull(GroupPaginationModel.Empty);
        }

        [Fact]
        public void Empty_HasPageIndexOne()
        {
            Assert.Equal(1, GroupPaginationModel.Empty.PageIndex);
        }
        #endregion
    }
}
