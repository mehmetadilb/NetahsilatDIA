using CommonLib.Model;
using NetahsilatWebServiceLib.ErpWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetahsilatWebServiceLib.Models
{
    public class CustomerVirmanFicheParameters
    {
        public PaymentServiceModel Payment { get; set; }
        public CurrentAccountModel Customer { get; set; }
        public CurrentAccountModel OpponentCustomer { get; set; }
        public string VoucherNumber { get; set; }
        public DynamicFieldsModel DynamicFields { get; set; }
        public DescriptionModel DescriptionModel { get; set; }
        public DiaFirmInfoModel FirmInfoModel { get; set; }
        public CurrencyModel CurrencyModel { get; set; }
        public string RePaymentPlanCode { get; set; }
        public bool ForceReloadIfMissing { get; set; }
    }
}
