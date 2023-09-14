using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PagedApiResponse<T>
    {
        public List<T> Items { get; set; }
        public int TotalItems { get; set; }
    }

}
