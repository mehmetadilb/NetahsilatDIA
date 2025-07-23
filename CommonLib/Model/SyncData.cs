using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class SyncData<T>
    {
        public DateTime LastSync { get; set; }
        public List<T> Data { get; set; }
    }
}
