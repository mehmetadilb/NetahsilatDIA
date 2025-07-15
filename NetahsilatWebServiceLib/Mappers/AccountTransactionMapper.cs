using NetahsilatWebServiceLib.CurrentAccountTransactionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using CommonLib.Enum;
using CommonLib.Model;

namespace NetahsilatWebServiceLib.Mappers
{
    public static class AccountTransactionMapper
    {
        public static CATCreateOrUpdateParameters Map(CurrentAccountTransactionModel obj)
        {
            var param = new CATCreateOrUpdateParameters
            {
                AmountSpecified = true,
                AmountTLSpecified = true,
                CrDrSpecified = true,
                PaidAmountSpecified = true,
                PaidAmountTLSpecified = true,
                MaxInstallmentSpecified = true,

                AccountErpCode = obj.AccountCode,
                Description = obj.Description,
                ErpCode = obj.Key,
                DocumentId = !string.IsNullOrEmpty(obj.VoucherNo) ? obj.VoucherNo : $"NT-{obj.Key}"
            };

            try
            {

                decimal credit = Utils.StrToDecimal(obj.Credit, 0);
                decimal debit = Utils.StrToDecimal(obj.Debit, 0);

                if (credit <= 0 && debit <= 0)
                    return null;

                param.CrDr = credit > 0 ? 1 : 2;
                decimal amount = credit > 0 ? credit : debit;
                param.AmountTL = amount;

                string currencyCode = obj.CurrencyType;

                if (currencyCode == "CTL" || currencyCode == "TRY" || string.IsNullOrEmpty(currencyCode))
                    currencyCode = "TL";

                param.CurrencyCode = currencyCode;

                if (currencyCode != "TL")
                {
                    param.Amount = Utils.StrToDecimal(obj.CreditD, amount);
                    param.ExchangeRate = Utils.StrToDecimal(obj.ExchangeRate, 1);
                }
                else
                {
                    param.Amount = amount;
                    param.ExchangeRate = 1;
                }

                DateTime transTime = obj.CreatedDate ?? DateTime.Now;
                DateTime dueTime = obj.DueDate ?? transTime;

                param.TransactionDate = transTime.ToString("dd.MM.yyyy");
                param.DueDate = dueTime.ToString("dd.MM.yyyy");

                string actType = obj.Type;
                if (string.IsNullOrEmpty(actType))
                {
                    actType = ActType.Diger.ToString();
                }
                else
                {
                    ActType mappedActType = MapToActType(actType);
                    actType = mappedActType.ToString();
                }

                param.ActTypeCode = actType;


                return param;
            }
            catch (Exception ex)
            {
                Logging.AddLog($"CAT Mapping Error - Cari Kod: {obj.AccountCode} - Id: {obj.Key} - Hata: {ex.Message}");
                return null;
            }
        }

        public static List<CATCreateOrUpdateParameters> Map(List<dynamic> obj)
        {
            List<CATCreateOrUpdateParameters> retVal = new List<CATCreateOrUpdateParameters>();

            foreach (dynamic item in obj)
            {
                retVal.Add(Map(item));
            }
            return retVal;
        }
        public static ActType MapToActType(string typeCode)
        {
            switch (typeCode)
            {
                case "NT": // Nakit Tahsilat
                case "NÖ": // Nakit Ödeme
                case "KF": // Kur Farkı Fişi
                case "VF": // Virman Fişi
                case "ÖF": // Özel Fiş
                case "AF": // Açılış Fişi
                    return ActType.Devir;

                case "BD": // Borç Dekontu
                case "AD": // Alacak Dekontu
                    return ActType.Fatura;

                case "KK": // Kredi Kartı Fişi
                case "KI": // Kredi Kartı İade Fişi
                case "SK": // Şirket Kredi Kartı Fişi
                case "IK": // İş Yeri Kredi Kartı Fişi
                case "II": // İş Yeri Kredi Kartı İade Fişi
                    return ActType.KrediKart;

                default:
                    return ActType.Diger;
            }
        }

    }
}
