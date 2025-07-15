using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Reflection;
using NetahsilatWebServiceLib.CurrentAccountTransactionService;
using NetahsilatWebServiceLib.ErpWebService;
using CommonLib;
using Newtonsoft.Json.Linq;

namespace NetahsilatWebServiceLib
{
    public class ServiceHelper
    {

        public static string GetVposErpCodeByPosId(int posId, VirtualPosListResult vplResult, ref int vposTypeId)
        {
            string retVal = "";
            if (vplResult.VirtualPoses != null && vplResult.VirtualPoses.Length > 0)
            {
                foreach (VirtualPos myPos in vplResult.VirtualPoses)
                {
                    if (myPos.VPosId == posId)
                    {
                        retVal = myPos.ErpCode;
                        vposTypeId = myPos.VposApiId;
                    }
                }
            }
            return retVal;
        }

        public static string GetVposErpCodeByPosId(int posId, VirtualPosListResult vplResult)
        {
            string retVal = "";
            if (vplResult.VirtualPoses != null && vplResult.VirtualPoses.Length > 0)
            {
                VirtualPos vpos = vplResult.VirtualPoses.Where(x => x.VPosId == posId).FirstOrDefault();
                if (vpos != null)
                    retVal = vpos.ErpCode;
            }
            return retVal;
        }


        public static int GetVPosApiTypeIdByPosId(int posId, VirtualPosListResult vplResult)
        {
            int retVal = 0;
            if (vplResult.VirtualPoses != null && vplResult.VirtualPoses.Length > 0)
            {
                var vpos = vplResult.VirtualPoses.Where(x => x.VPosId == posId).FirstOrDefault();

                if (vpos != null)
                    retVal = vpos.VPosApiTypeId;
            }

            return retVal;
        }

        public static List<string> SetBankPayBackPlans(string vPosErpCode)
        {
            if (string.IsNullOrEmpty(vPosErpCode))
                return null;

            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var jsonFilePath = Path.Combine(folderPath, "PayBackPlan.json");

            if (!File.Exists(jsonFilePath))
                throw new Exception("PayBackPlan.json dosyası bulunamadı. Geri ödeme planı eşleştirme sayfasından veri giriniz");

            var jsonText = File.ReadAllText(jsonFilePath);

            if (string.IsNullOrWhiteSpace(jsonText))
                throw new Exception("PayBackPlan.json dosyası boş.");

            var virtualPosConfigs = new List<string>();

            var jsonObj = JObject.Parse(jsonText);

            var vposList = jsonObj["vposlist"]?["Vpos"] as JArray;

            if (vposList == null)
                throw new Exception("Vpos listesi JSON dosyasında bulunamadı.");

            foreach (var vpos in vposList)
            {
                var erpCode = (string)vpos["ERPCODE"];
                if (string.Equals(erpCode, vPosErpCode, StringComparison.OrdinalIgnoreCase))
                {
                    virtualPosConfigs.Add((string)vpos["PBDEFPLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB1PLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB2PLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB3PLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB4PLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB5PLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB6PLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB7PLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB8PLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB9PLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB10PLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB11PLAN"]);
                    virtualPosConfigs.Add((string)vpos["PB12PLAN"]);
                    break; 
                }
            }

            return virtualPosConfigs;
        }


        public enum VposType
        {
            Vpos,
            Cek = 2060,
            Senet = 2070
        }

        public static int SetTimerInterval()
        {
                return Config.GlobalParameters.GlobalSettings.TMR_INTERVAL * 60000;
        }

        public static StringBuilder GetNewOrUpdatedAccountSqlQuery(string firmNumber, bool isDaily, string accountCode)
        {
            StringBuilder sb = new StringBuilder();
            // Implementation will be added when needed
            return sb;
        }

        public static class VPosApiType
        {
            // Kredi Kartı Girilenler    (+)
            // Kredi Kartı Girilmeyenler (-)

            public const int Bank = 100;//+
            public const int Est = 200;//+
            public const int Arena = 210; //arena posu +
            public const int Kvk = 220;//+
            public const int Genpa = 230;//+
            public const int ArenaVendor = 240; //arena bayi posu +
            public const int PayPal = 300;//-
            public const int iPara = 400;//-
            public const int Netahsilat = 500;//+
            public const int DummyApi = 550;//-
            public const int PayU = 600;//iptal
            public const int BKMExpress = 700; //-
            public const int ArenaPayPos = 800; //arena paypos -
            public const int PayUAlu = 900;//+
            public const int CreditApplication = 1000;//-
            public const int GarantiPay = 1100;
            public const int Hesap = 1200;
            public const int Paynet = 1300;
        }

        public static string GetCustomerCodeFromPayment(PaymentServiceModel payment)
        {
            var agentCode = string.Empty;

            if (payment.AccountErpCode != null && payment.AccountErpCode.Length > 0)
            {
                agentCode = payment.AccountErpCode.Length > 15 ? payment.AccountErpCode.Substring(0, 15) : payment.AccountErpCode;
            }
            else if (payment.Agent.ErpCode != null && payment.Agent.ErpCode.Length > 0)
            {
                agentCode = payment.Agent.ErpCode.Length > 15 ? payment.Agent.ErpCode.Substring(0, 15) : payment.Agent.ErpCode;
            }
            else if (payment.Agent.Code != null && payment.Agent.Code.Length > 0)
            {
                agentCode = payment.Agent.Code.Length > 15 ? payment.Agent.Code.Substring(0, 15) : payment.Agent.Code;
            }
            else if (payment.Agent.TCKN != null && payment.Agent.TCKN.Length > 0)
            {
                agentCode = payment.Agent.TCKN.Length > 15 ? payment.Agent.TCKN.Substring(0, 15) : payment.Agent.ErpCode;
            }
            else if (payment.Agent.TaxNumber != null && payment.Agent.TaxNumber.Length > 0)
            {
                agentCode = payment.Agent.TaxNumber.Length > 15 ? payment.Agent.TaxNumber.Substring(0, 15) : payment.Agent.Code;
            }

            return agentCode;
        }
    }
}
