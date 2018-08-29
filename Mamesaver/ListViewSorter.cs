/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System.Collections;
using System.Windows.Forms;

namespace Mamesaver
{
    public class ListViewSorter : IComparer
    {
        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private readonly CaseInsensitiveComparer _objectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewSorter()
        {
            SortColumn = 0;
            Order = SortOrder.Ascending;
            _objectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            var listviewX = (ListViewItem)x;
            var listviewY = (ListViewItem)y;

            var compareResult = _objectCompare.Compare(listviewX?.SubItems[SortColumn].Text, listviewY?.SubItems[SortColumn].Text);

            if (Order == SortOrder.None) return 0;

            return Order == SortOrder.Ascending ? compareResult : -compareResult;
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn { set; get; }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order { set; get; }
    }
}
