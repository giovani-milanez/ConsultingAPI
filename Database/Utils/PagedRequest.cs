using System.Collections.Generic;

namespace Database.Utils
{
    public class PagedRequest
    {
        public PagedRequest(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
            SortFields = new List<SortField>();
            Filters = new List<Filter>();
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public IList<SortField> SortFields { get; set; }
        public IList<Filter> Filters { get; set; }
    }

    public class SortField
    {
        public string FieldName { get; set; }
        public string SortOrder { get; set; }
    }

    public class Filter
    {
        public string FieldName { get; set; }
        public string Value { get; set; }
    }

}
