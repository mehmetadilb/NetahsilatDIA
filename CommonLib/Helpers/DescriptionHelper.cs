using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class DescriptionHelper
    {
        ////KS: Kart Sahibi ---CardHolderName
        ////FN: Fiş Numarası --Payment Model ErpCode
        ////B: Banka Hesap Kodu--BankVPosCode
        ////BA: Banka Adı --BankVPosName
        ////CU: Cari Unvan ---Name
        ////RN: Referans No --ReferenceCode
        ////CK: Cari Kodu --Agent / ErpCode
        ////TS: Taksit Sayısı --InstallmentCount
        ////ERT: Erteleme --PaymentDeferral

        //public string GetDynamicLineDescription(PaymentServiceModel payment, string descriptionFormat, int maxLength)
        //{
        //    try
        //    {
        //        var formattedDesc = GetFormattedDescriptions(payment, descriptionFormat);

        //        if (formattedDesc.Length > maxLength)
        //            return formattedDesc.Substring(0, maxLength);

        //        return formattedDesc;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.AddLog($"Satır açıklama bölme işlemleri yapılamadı. Detay: {ex.Message}");
        //        return string.Empty;
        //    }
        //}

        //public PublicDescriptionModel GetDynamicPublicDescription(PaymentServiceModel payment, string descriptionFormat, int maxLength)
        //{
        //    try
        //    {
        //        var formattedDesc = GetFormattedDescriptions(payment, descriptionFormat);
        //        var splittedDescModel = SplitDescriptions(formattedDesc, maxLength);
        //        return splittedDescModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.AddLog($"Genel açıklama bölme işlemleri yapılamadı. Detay: {ex.Message}");
        //        return new PublicDescriptionModel();
        //    }
        //}

        //private string GetFormattedDescriptions(PaymentServiceModel payment, string descriptionFormat)
        //{
        //    if (string.IsNullOrEmpty(descriptionFormat))
        //        return null;

        //    var descriptionKeys = descriptionFormat.Split(',');
        //    var descriptions = new List<string>();

        //    foreach (var descKey in descriptionKeys)
        //    {
        //        switch (descKey)
        //        {
        //            case "KS":
        //                if (!string.IsNullOrEmpty(payment.CardHolderName))
        //                    descriptions.Add($"KS:{payment.CardHolderName}");
        //                break;
        //            case "FN":
        //                if (!string.IsNullOrEmpty(payment.ErpCode))
        //                    descriptions.Add($"FN:{payment.ErpCode}");
        //                break;
        //            case "B":
        //                if (payment.BankDetails != null && !string.IsNullOrEmpty(payment.BankDetails.BankVPosCode))
        //                    descriptions.Add($"B:{payment.BankDetails.BankVPosCode}");
        //                break;
        //            case "BA":
        //                if (payment.BankDetails != null && !string.IsNullOrEmpty(payment.BankDetails.BankVPosName))
        //                    descriptions.Add($"BA:{payment.BankDetails.BankVPosName}");
        //                break;
        //            case "CU":
        //                if (payment.Agent != null && !string.IsNullOrEmpty(payment.Agent.Name))
        //                    descriptions.Add($"CU:{payment.Agent.Name}");
        //                break;
        //            case "RN":
        //                if (!string.IsNullOrEmpty(payment.ReferenceCode))
        //                    descriptions.Add($"RN:{payment.ReferenceCode}");
        //                break;
        //            case "CK":
        //                if (payment.Agent != null && !string.IsNullOrEmpty(payment.Agent.ErpCode))
        //                    descriptions.Add($"CK:{payment.Agent.ErpCode}");
        //                break;
        //            case "TS":
        //                if (payment.PlusPeriod != 0)
        //                    descriptions.Add($"TS:{payment.Period} + {payment.PlusPeriod}");
        //                else
        //                    descriptions.Add($"TS:{payment.Period}");
        //                break;
        //            case "ERT":
        //                if (payment.PaymentDeferral > 0)
        //                    descriptions.Add($"ERT:{payment.PaymentDeferral}");
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    return string.Join(" ", descriptions);
        //}

        //private PublicDescriptionModel SplitDescriptions(string descriptionText, int maxLength)
        //{
        //    if (string.IsNullOrEmpty(descriptionText))
        //        return null;

        //    if (descriptionText.Length > maxLength * 4)
        //        descriptionText = descriptionText.Substring(0, maxLength * 4);

        //    var descriptionLines = new List<string>();
        //    var splittedDesc = descriptionText.Split(' ');

        //    var description = string.Empty;
        //    for (int descIndex = 0; descIndex < splittedDesc.Length; descIndex++)
        //    {
        //        var concatDescLength = description.Length + splittedDesc[descIndex].Length;
        //        if (description.Length < maxLength && concatDescLength < maxLength)
        //        {
        //            if (description.Length != 0 && concatDescLength + 1 < maxLength)
        //                description += " ";

        //            description += splittedDesc[descIndex];

        //            if (descIndex != splittedDesc.Length - 1)
        //                continue;
        //        }
        //        else
        //            descIndex--;

        //        descriptionLines.Add(description);
        //        description = string.Empty;
        //    }

        //    var descReturnModel = new PublicDescriptionModel();

        //    if (descriptionLines.Any())
        //    {
        //        if (descriptionLines.Count >= 1)
        //            descReturnModel.Description1 = descriptionLines[0];

        //        if (descriptionLines.Count >= 2)
        //            descReturnModel.Description2 = descriptionLines[1];

        //        if (descriptionLines.Count >= 3)
        //            descReturnModel.Description3 = descriptionLines[2];

        //        if (descriptionLines.Count >= 4)
        //            descReturnModel.Description4 = descriptionLines[3];
        //    }

        //    return descReturnModel;
        //}
    }
}
