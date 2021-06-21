using System.Collections.Generic;

namespace Database.Utils
{
    public class PagedSearch<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalResults { get; set; }       
        public List<T> Items { get; set; }

        //public PagedSearch()
        //{
        //}

        public PagedSearch(int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
        }

        public PagedSearch(int currentPage) 
            : this(currentPage, 10) {}

        public int GetCurrentPage()
        {
            return CurrentPage == 0 ? 2 : CurrentPage;
        }

        public int GetPageSize()
        {
            return PageSize == 0 ? 10 : PageSize;
        }
    }
}
