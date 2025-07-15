using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class BankPaymentBackPlan
    {
        public int VposId { get; set; }
        public string VposName { get; set; }
        public List<string> PayBackPlans { get; set; }
        public string PayBack1 { get; set; }
        public string PayBack2 { get; set; }
        public string PayBack3 { get; set; }
        public string PayBack4 { get; set; }
        public string PayBack5 { get; set; }
        public string PayBack6 { get; set; }
        public string PayBack7 { get; set; }
        public string PayBack8 { get; set; }
        public string PayBack9 { get; set; }
        public string PayBack10 { get; set; }
        public string PayBack11 { get; set; }
        public string PayBack12 { get; set; }
        public string ApplyPayBackPlan { get; set; }

        public BankPaymentBackPlan()
        {
            PayBack1 = "";
            PayBack2 = "";
            PayBack3 = "";
            PayBack4 = "";
            PayBack5 = "";
            PayBack6 = "";
            PayBack7 = "";
            PayBack8 = "";
            PayBack9 = "";
            PayBack10 = "";
            PayBack11 = "";
            PayBack12 = "";
        }
    }
}
