using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationToolV2.Model
{
    public class Page<T>
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
