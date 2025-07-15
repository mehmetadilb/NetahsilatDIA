using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class CurrentAccountTransactionModel
    {
        [JsonProperty("_key")]
        public string Key { get; set; }

        [JsonProperty("_serial")]
        public string Serial { get; set; }

        [JsonProperty("_order")]
        public string Order { get; set; }

        [JsonProperty("_cdate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("_date")]
        public DateTime? Date { get; set; }

        [JsonProperty("aciklama")]
        public string Description { get; set; }

        [JsonProperty("carikodu")]
        public string AccountCode { get; set; }

        [JsonProperty("cariunvan")]
        public string AccountName { get; set; }

        [JsonProperty("borc")]
        public string Debit { get; set; }

        [JsonProperty("alacak")]
        public string Credit { get; set; }

        [JsonProperty("borc_cari")]
        public string DebtInCurrency { get; set; }

        [JsonProperty("alacak_cari")]
        public string CreditInCurrency { get; set; }

        [JsonProperty("yerelborc")]
        public string LocalDebt { get; set; }

        [JsonProperty("yerelalacak")]
        public string LocalCredit { get; set; }

        [JsonProperty("dovizturu")]
        public string CurrencyType { get; set; }

        [JsonProperty("dovizkuru")]
        public string ExchangeRate { get; set; }

        [JsonProperty("raporlamadovizkuru")]
        public string ReportingRate { get; set; }

        [JsonProperty("fisno")]
        public string VoucherNo { get; set; }

        [JsonProperty("tarih")]
        public DateTime? TransactionDate { get; set; }

        [JsonProperty("vade")]
        public DateTime? DueDate { get; set; }

        [JsonProperty("turu")]
        public string Type { get; set; }

        [JsonProperty("turuack")]
        public string TypeDescription { get; set; }

        [JsonProperty("sube")]
        public string Branch { get; set; }

        [JsonProperty("firmaadi")]
        public string CompanyName { get; set; }

        [JsonProperty("ekleyenkullaniciadi")]
        public string CreatedByUser { get; set; }

        [JsonProperty("kullaniciadi")]
        public string UserName { get; set; }

        [JsonProperty("masrafmerkezikodu")]
        public string CostCenterCode { get; set; }

        [JsonProperty("masrafmerkeziaciklama")]
        public string CostCenterDescription { get; set; }

        [JsonProperty("projekodu")]
        public string ProjectCode { get; set; }

        [JsonProperty("proje_kodu_aciklama")]
        public string ProjectDescription { get; set; }

        [JsonProperty("carisatiselemani")]
        public string SalesPersonCode { get; set; }

        [JsonProperty("satiselemaniaciklama")]
        public string SalesPersonDescription { get; set; }

        [JsonProperty("bankaodemeplanikodu")]
        public string BankPaymentPlanCode { get; set; }

        [JsonProperty("bankaodemeplaniaciklama")]
        public string BankPaymentPlanDescription { get; set; }

        [JsonProperty("hizmetkartkodu")]
        public string ServiceCardCode { get; set; }

        [JsonProperty("hizmetkartaciklama")]
        public string ServiceCardDescription { get; set; }

        [JsonProperty("fisyetkikodu")]
        public string VoucherAuthCode { get; set; }

        [JsonProperty("fisaciklama")]
        public string VoucherDescription { get; set; }

        [JsonProperty("bilgi")]
        public string Info { get; set; }

        [JsonProperty("fisozelkod")]
        public string VoucherSpecialCode { get; set; }

        [JsonProperty("ozelkod")]
        public string SpecialCode { get; set; }

        [JsonProperty("carikisaaciklama")]
        public string AccountShortDescription { get; set; }

        [JsonProperty("carigrupkodu")]
        public string AccountGroupCode { get; set; }

        [JsonProperty("carigrupkoduaciklama")]
        public string AccountGroupDescription { get; set; }

        [JsonProperty("makbuzno")]
        public string ReceiptNo { get; set; }

        [JsonProperty("dalacak")]
        public string CreditD { get; set; }

        [JsonProperty("dborc")]
        public string DebtD { get; set; }

        [JsonProperty("kurfarkialacak")]
        public string ExchangeDiffCredit { get; set; }

        [JsonProperty("kurfarkiborc")]
        public string ExchangeDiffDebt { get; set; }

        [JsonProperty("harcananpuantutari")]
        public string UsedPointAmount { get; set; }

        [JsonProperty("kazanilanpuantutari")]
        public string EarnedPointAmount { get; set; }

        [JsonProperty("kalanhizmetmaliyettutari")]
        public string RemainingServiceCost { get; set; }

        [JsonProperty("maliyetlendirilenhizmettutari")]
        public string AllocatedServiceCost { get; set; }

        [JsonProperty("fisdovizkuru")]
        public string VoucherCurrencyRate { get; set; }

        [JsonProperty("saat")]
        public string Hour { get; set; }

        [JsonProperty("taksitsayisi")]
        public string InstallmentCount { get; set; }

        [JsonProperty("muhasebelesme")]
        public string AccountingStatus { get; set; }
        [JsonProperty("belgeno")]
        public string DocumentNumber { get; set; }       

        [JsonProperty("_level1")]
        public long FirmNumber { get; set; }
      
        [JsonProperty("_level2")]
        public long PeriodNumber { get; set; }
        public bool IntegrationStatus { get; set; }
    }
}
