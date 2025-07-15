using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class AccountTransaction
    {
        public int TransId { get; set; }
        public DateTime RecordDate { get; set; }
        public int Firm { get; set; }
        public int Period { get; set; }
        public bool Status { get; set; }
        public double Paid { get; set; }
        public decimal Total { get; set; }
        public bool Deleted { get; set; }
    }
}
