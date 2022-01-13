namespace Blend.Optimizely.Search
{
    public class PaginationModel
    {
        public int PageIndex { get; }

        public int TotalPages { get; }

        public int MaxPageIndex { get; }

        public int PageSize { get; }

        public int TotalResults { get; }

        public int ResultsStartIndex { get; }

        public int ResultsEndIndex { get; }

        public int RangeStart { get; }

        public int RangeEnd { get; }

        public Func<int, string> GetUrl { get; set; }

        public PaginationModel(int pageSize, int pageDistance, int pageIndex, int totalResults)
        {
            PageSize = pageSize;

            TotalResults = totalResults;
            TotalPages = (int)Math.Ceiling(totalResults / (double)pageSize);
            MaxPageIndex = Math.Max(0, TotalPages - 1);

            PageIndex = Math.Max(0, Math.Min(pageIndex, TotalPages - 1));
            ResultsStartIndex = PageIndex * pageSize;
            ResultsEndIndex = Math.Min((ResultsStartIndex + pageSize), (totalResults - 1));

            RangeStart = Math.Max(0, PageIndex - pageDistance);
            RangeEnd = Math.Min(MaxPageIndex, PageIndex + pageDistance);

            GetUrl = (int page) => "?p=" + page;
        }

        public static readonly PaginationModel Empty = new PaginationModel(0, 0, 0, 0);
    }

    public static class PaginationModelExtensions
    {
        public static IEnumerable<T> Apply<T>(this PaginationModel pagination, IEnumerable<T> fullList)
            => fullList.Skip(pagination.ResultsStartIndex).Take(pagination.PageSize);
    }
}