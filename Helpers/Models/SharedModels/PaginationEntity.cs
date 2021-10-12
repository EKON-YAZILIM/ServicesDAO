using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.SharedModels
{
    /// <summary>
    ///  Extension class for .NET Core PagedList package
    /// </summary>
    public class PaginationEntity<TEntity> where TEntity : class
    {
        public PaginationEntity()
        {
            Items = new List<TEntity>();
        }
        public IEnumerable<TEntity> Items { get; set; }
        public PaginationMetaData MetaData { get; set; }
    }

    /// <summary>
    ///  Paged list meta data
    /// </summary>
    public class PaginationMetaData
    {
        public int Count { get; set; }
        public int FirstItemOnPage { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool IsFirstPage { get; set; }
        public bool IsLastPage { get; set; }
        public int LastItemOnPage { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
    }
}
