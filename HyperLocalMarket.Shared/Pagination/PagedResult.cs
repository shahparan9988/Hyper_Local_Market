using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperLocalMarket.Shared.Pagination
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyCollection<T> Items { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        private PagedResult(IReadOnlyCollection<T> value, int pageNumber, int pageSize, int totalCount)
        {
            Items = value;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
        public static PagedResult<T> Create(IReadOnlyCollection<T> value, int pageNumber, int pageSize, int totalCount)
        {
            return new PagedResult<T>(value, pageNumber, pageSize, totalCount);
        }
    
    }
}
