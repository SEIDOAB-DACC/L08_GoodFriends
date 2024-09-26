using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class csRespPageDTO<T>
    {
        public List<T> PageItems { get; init; }
        public int DbItemsCount { get; init; }

        public int PageNr { get; init; }
        public int PageSize { get; init; }
        public int PageCount => (int)Math.Ceiling((double)DbItemsCount / PageSize);

    }
}
