using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class ServiceStartResponse
    {
        public bool RunPayment { get; set; }
        public bool RunAccount { get; set; }
        public bool RunVendor { get; set; }
        public List<int> WorkingHours { get; set; }
        public int TimerInterval { get; set; }
    }
}
