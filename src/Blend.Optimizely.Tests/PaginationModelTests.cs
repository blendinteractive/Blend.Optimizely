using System.Linq;
using Blend.Optimizely.Search;
using Xunit;

namespace Blend.Optimizely.Tests
{
    public class PaginationModelTests
    {
        #region TotalPages
        [Fact]
        public void TotalPages_IsExact_WhenResultsDivisibleByPageSize()
        {
            var p = new PaginationModel(10, 2, 0, 100);
            Assert.Equal(10, p.TotalPages);
        }

        [Fact]
        public void TotalPages_RoundsUp_WhenResultsNotDivisibleByPageSize()
        {
            var p = new PaginationModel(10, 2, 0, 95);
            Assert.Equal(10, p.TotalPages);
        }
        #endregion

        #region MaxPageIndex
        [Fact]
        public void MaxPageIndex_IsTotalPagesMinusOne()
        {
            var p = new PaginationModel(10, 2, 0, 100);
            Assert.Equal(9, p.MaxPageIndex);
        }

        [Fact]
        public void MaxPageIndex_IsZero_WhenNoResults()
        {
            var p = new PaginationModel(10, 2, 0, 0);
            Assert.Equal(0, p.MaxPageIndex);
        }
        #endregion

        #region PageIndex clamping
        [Fact]
        public void PageIndex_IsPreserved_WhenInRange()
        {
            var p = new PaginationModel(10, 2, 5, 100);
            Assert.Equal(5, p.PageIndex);
        }

        [Fact]
        public void PageIndex_IsClamped_WhenAboveMaxPageIndex()
        {
            var p = new PaginationModel(10, 2, 99, 100);
            Assert.Equal(9, p.PageIndex);
        }

        [Fact]
        public void PageIndex_IsZero_WhenNegative()
        {
            var p = new PaginationModel(10, 2, -5, 100);
            Assert.Equal(0, p.PageIndex);
        }
        #endregion

        #region ResultsStartIndex / ResultsEndIndex
        [Fact]
        public void ResultsStartIndex_IsPageIndexTimesPageSize()
        {
            var p = new PaginationModel(10, 2, 3, 100);
            Assert.Equal(30, p.ResultsStartIndex);
        }

        [Fact]
        public void ResultsEndIndex_IsStartPlusPageSize_OnFullPage()
        {
            var p = new PaginationModel(10, 2, 3, 100);
            Assert.Equal(40, p.ResultsEndIndex);
        }

        [Fact]
        public void ResultsEndIndex_IsTotalResultsMinusOne_OnLastPartialPage()
        {
            var p = new PaginationModel(10, 2, 9, 95);
            Assert.Equal(94, p.ResultsEndIndex);
        }
        #endregion

        #region RangeStart / RangeEnd
        [Fact]
        public void RangeStart_IsPageIndexMinusDistance_WhenMiddle()
        {
            var p = new PaginationModel(10, 2, 5, 100);
            Assert.Equal(3, p.RangeStart);
        }

        [Fact]
        public void RangeEnd_IsPageIndexPlusDistance_WhenMiddle()
        {
            var p = new PaginationModel(10, 2, 5, 100);
            Assert.Equal(7, p.RangeEnd);
        }

        [Fact]
        public void RangeStart_IsClampedToZero_WhenNearStart()
        {
            var p = new PaginationModel(10, 3, 1, 100);
            Assert.Equal(0, p.RangeStart);
        }

        [Fact]
        public void RangeEnd_IsClampedToMaxPageIndex_WhenNearEnd()
        {
            var p = new PaginationModel(10, 3, 8, 100);
            Assert.Equal(9, p.RangeEnd);
        }
        #endregion

        #region GetUrl
        [Fact]
        public void GetUrl_DefaultReturnsQueryStringPage()
        {
            var p = new PaginationModel(10, 2, 0, 100);
            Assert.Equal("?p=3", p.GetUrl(3));
        }

        [Fact]
        public void GetUrl_CanBeReplaced()
        {
            var p = new PaginationModel(10, 2, 0, 100);
            p.GetUrl = page => $"/results?page={page}";
            Assert.Equal("/results?page=3", p.GetUrl(3));
        }
        #endregion

        #region Empty
        [Fact]
        public void Empty_IsNotNull()
        {
            Assert.NotNull(PaginationModel.Empty);
        }

        [Fact]
        public void Empty_HasZeroTotalResults()
        {
            Assert.Equal(0, PaginationModel.Empty.TotalResults);
        }
        #endregion

        #region Apply extension
        [Fact]
        public void Apply_SkipsAndTakesCorrectElements()
        {
            var list = Enumerable.Range(1, 15).ToList();
            var p = new PaginationModel(5, 0, 1, 15); // 0-based page 1 → items 6–10
            var result = p.Apply(list).ToList();
            Assert.Equal(new[] { 6, 7, 8, 9, 10 }, result);
        }

        [Fact]
        public void Apply_ReturnsRemainingItems_OnLastPartialPage()
        {
            var list = Enumerable.Range(1, 13).ToList();
            var p = new PaginationModel(5, 0, 2, 13); // 0-based page 2 → items 11–13
            var result = p.Apply(list).ToList();
            Assert.Equal(new[] { 11, 12, 13 }, result);
        }
        #endregion
    }
}
