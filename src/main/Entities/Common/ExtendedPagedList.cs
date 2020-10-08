namespace Main.Entities.Common
{
    using System.Collections.Generic;

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
    }
}