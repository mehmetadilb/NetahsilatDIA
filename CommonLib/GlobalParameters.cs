using CommonLib.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommonLib
{
    public static class Config
    { 
        public static GlobalParameters GlobalParameters { get; set; }
    }
    public class GlobalSettings
    {
        public bool USE_MULTIFIRM { get; set; }
        public List<Firm> FIRMS { get; set; }
        public int TMR_INTERVAL { get; set; }
        public string CREDIT_CARD_NUMBER_FIELD { get; set; }
        public string CURRENCY_TYPE { get; set; }
        public bool SET_REVERSAL { get; set; }
        public bool IS_TRANSFER_REPAYMENTPLAN { get; set; }
        public List<CustomerPaymentSet> CUSTOMERPAYMENTSETS { get; set; } = new List<CustomerPaymentSet>();
        public bool ADD_COMMISSION_LINE { get; set; }
        public PaymentCustomerFirstFieldType ACCOUNT_ERPCODE_FIELD { get; set; }
        public CustomerTransferType CUSTOMER_TRANSFER_TYPE { get; set; }
        public string TRANSFER_MODE { get; set; }
        public string PUBLIC_DESCRIPTION_FORMAT { get; set; }
        public string LINE_DESCRIPTION_FORMAT { get; set; }
        public bool SEND_EMAIL { get; set; }
        public CommissionTransferType OUT_OF_COMMISSION { get; set; }
        public bool PAYMENT_SET_CODE { get; set; }
        public NonCustomerPayment NONCUSTOMER_PAYMENT_TYPE { get; set; }
        public string NONCUSTOMER_BAGACCOUNT { get; set; }
        public bool SEND_TRANSFER_TRANS { get; set; }
        public ReceiptTypeToBeTransferred RECEIPT_TYPE_TO_BE_TRANSFERRED { get; set; }
        public string WORK_HOURS { get; set; }
        public bool GET_BANK_PAYMENT { get; set; }
    }
    public class CustomerPaymentSet
    {
        public string Id { get; set; }
        public string CustomerPrefix { get; set; }
        public int PaymentSetId { get; set; }
        public string FirmNo { get; set; }
        public int CurrentAccountTypeId { get; set; }
    }

    public class GlobalParameters
    {
        [JsonPropertyName("Parameters")]
        public Parameters Parameters { get; set; }

        [JsonPropertyName("GlobalSettings")]
        public GlobalSettings GlobalSettings { get; set; }
    }

    public class Firm
    {
        [JsonPropertyName("Company")]
        public int Company { get; set; }

        [JsonPropertyName("Period")]
        public int Period { get; set; }

        [JsonPropertyName("DiaUserName")]
        public string DiaUserName { get; set; }

        [JsonPropertyName("DiaPassword")]
        public string DiaPassword { get; set; }

        [JsonPropertyName("DisconnectSameUser")]
        public bool DisconnectSameUser { get; set; } = true;

        [JsonPropertyName("DiaLang")]
        public string DiaLang { get; set; } = "TR";

        [JsonPropertyName("DiaToken")]
        public string DiaToken { get; set; }

        [JsonPropertyName("IsActive")]
        public bool IsActive { get; set; }
    }

    public class Parameters
    {
        [JsonPropertyName("ERP_SERVICE")]
        public string ERP_SERVICE { get; set; }

        [JsonPropertyName("ACCOUNT_SERVICE")]
        public string ACCOUNT_SERVICE { get; set; }

        [JsonPropertyName("VENDOR_SERVICE")]
        public string VENDOR_SERVICE { get; set; }

        [JsonPropertyName("WEB_SERVICE_UID")]
        public string WEB_SERVICE_UID { get; set; }

        [JsonPropertyName("WEB_SERVICE_PWD")]
        public string WEB_SERVICE_PWD { get; set; }

        [JsonPropertyName("DiaBaseUrl")]
        public string DiaBaseUrl { get; set; }

        [JsonPropertyName("DiaApiEndpoint")]
        public string DiaApiEndpoint { get; set; } = "sis/json";

        [JsonPropertyName("DiaSessionErrorMessageHint")]
        public string DiaSessionErrorMessageHint { get; set; }

        [JsonPropertyName("Firms")]
        public List<Firm> Firms { get; set; }
    }
}
