using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class CurrencyModel
    {
        public decimal PureAmount { get; set; }
        public decimal PureExchangeAmount { get; set; }
        public decimal PureReportAmount { get; set; }

        public decimal NetAmount { get; set; }
        public decimal NetExchangeAmount { get; set; }
        public decimal NetReportAmount { get; set; }


        public decimal ReportRate { get; set; }
        public decimal ExchangeRate { get; set; }
        public string CurrencyType { get; set; }
    }
}
