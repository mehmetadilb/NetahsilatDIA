using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netahsilat.DIAService
{
    public static class ConfigHelper
    {
        public class DiaParameters
        {
            public string DiaBaseUrl { get; set; }
            public string DiaApiEndpoint { get; set; }
            public string DiaSessionErrorMessageHint { get; set; }
        }

        public class DiaFirm
        {
            public int Company { get; set; }
            public int Period { get; set; }
            public string DiaUserName { get; set; }
            public string DiaPassword { get; set; }
            public bool DisconnectSameUser { get; set; } = true;
            public string DiaLang { get; set; }
            public string DiaApiKey { get; set; }
        }

        private static DiaParameters _parameters;
        private static DiaFirm _firm;

        private static string[] _sessionErrorMessages = Array.Empty<string>();

        public static void Init(DiaParameters parameters, DiaFirm firm)
        {
            _parameters = parameters;
            _firm = firm;

            // Oturum hata mesajlarını JSON'dan gelen parametre ile ayarla
            _sessionErrorMessages = (_parameters?.DiaSessionErrorMessageHint ?? "")
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
        }

        public static string DiaBaseUrl => _parameters?.DiaBaseUrl;
        public static string DiaApiEndpoint => _parameters?.DiaApiEndpoint ?? "sis/json";
        public static string DiaUsername => _firm?.DiaUserName;
        public static string DiaPassword => _firm?.DiaPassword;
        public static string DiaApiKey => _firm?.DiaApiKey;
        public static bool DiaDisconnectSameUser => _firm?.DisconnectSameUser ?? true;
        public static string DiaLang => _firm?.DiaLang ?? "TR";
        public static int DiaFirmaKodu => _firm?.Company ?? 0;
        public static int DiaDonemKodu => _firm?.Period ?? 0;

        public static bool IsSessionError(string apiMessage)
        {
            if (string.IsNullOrEmpty(apiMessage) || _sessionErrorMessages.Length == 0)
                return false;

            return _sessionErrorMessages.Any(errMsg =>
                apiMessage.IndexOf(errMsg, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}