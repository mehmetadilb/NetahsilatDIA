using CommonLib.Model;
using NetahsilatWebServiceLib.ErpWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetahsilatWebServiceLib.Models
{
    public class BaseCreditCardFicheParameters
    {
        public CurrentAccountModel Customer { get; set; }
        public long BankAccount { get; set; }
        public BankPaymentBackPlan BankPaymentBackPlan { get; set; }
        public string VoucherNumber { get; set; }
        public CurrencyModel CurrencyModel { get; set; }
        public DescriptionModel DescriptionModel { get; set; }
        public int PaymentCommissionType { get; set; }
        public DiaFirmInfoModel FirmInfoModel { get; set; }
    }

    public class CreditCardFicheParameters : BaseCreditCardFicheParameters
    {
        public PaymentServiceModel Payment { get; set; }
        public string RePaymentPlanCode { get; set; }
        public int AccounttingAccountId { get; set; }
        public DynamicFieldsModel DynamicFields { get; set; }
    }

    public class CreditCardReversalFicheParameters : BaseCreditCardFicheParameters
    {
        public ReversalServiceModel Reversal { get; set; }
    }
}
