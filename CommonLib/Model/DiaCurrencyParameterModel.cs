using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class DiaCurrencyParameterModel
    {
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
        public string CurrencyType { get; set; }
    }
}
