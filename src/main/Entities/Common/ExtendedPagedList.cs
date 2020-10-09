namespace Main.Entities.Common
{
    using System.Collections.Generic;
    using System.Linq;

    using Main.Entities.Models;
    using Main.Entities.ViewModels;

    using X.PagedList;

    /// <summary>
    ///     Extends a paged list by adding a total count of items
    /// </summary>
    /// <typeparam name="T">The type of object the collection should contain.</typeparam>
    internal class ExtendedPagedList<T>
    {
        /// <summary>
        ///     Gets the paged content
        /// </summary>
        public IEnumerable<T> Content { get; set; }

        /// <summary>
        ///     Returns true if this is the first page
        /// </summary>
        public bool First { get; set; }

        /// <summary>
        ///     Returns true if this is the last page
        /// </summary>
        public bool Last { get; set; }

        /// <summary>
        ///     Gets the page number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        ///     Gets the number of elements in this page
        /// </summary>
        public int NumberOfElements { get; set; }

        /// <summary>
        ///     Gets the pagination size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        ///     Gets the total count of elements without pagination
        /// </summary>
        public int TotalElements { get; set; }

        /// <summary>
        ///     Gets the total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        public ExtendedPagedList(PagedList<T> list)

        {
            if (list == null)
            {
                this.Content = new List<T>();
            }
            else
            {
                this.Content = list.AsEnumerable();
                this.First = list.IsFirstPage;
                this.Last = list.IsLastPage;
                this.Number = list.PageNumber;
                this.NumberOfElements = list.Count;
                this.Size = list.PageSize;
                this.TotalElements = list.TotalItemCount;
                this.TotalPages = list.PageCount;
            }
        }
    }
}