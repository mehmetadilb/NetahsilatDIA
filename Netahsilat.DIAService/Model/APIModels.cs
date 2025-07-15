using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netahsilat.DIAService.Model
{
    public class BaseApiGetResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("result")]
        public dynamic Result { get; set; }

        public decimal? Credit { get; set; }

        public bool IsSuccess => Code == 200;
        public int Count => Result is JArray array ? array.Count : Result is JObject ? 1 : 0;
    }

    public class LoginParams
    {
        [JsonProperty("apikey")]
        public string ApiKey { get; set; }
    }

    public class LoginData
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("disconnect_same_user")]
        public bool DisconnectSameUser { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("params")]
        public LoginParams Params { get; set; }
    }

    public class LoginRequest
    {
        [JsonProperty("login")]
        public LoginData Login { get; set; }
    }

    public class LoginResponse : BaseApiGetResponse
    {
    }

    public class LogoutData
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
    }

    public class LogoutRequest
    {
        [JsonProperty("logout")]
        public LogoutData Logout { get; set; }
    }

    public class LogoutResponse : BaseApiGetResponse
    {

    }

    public class KontorSorgulaData
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
    }

    public class KontorSorgulaRequest
    {
        [JsonProperty("sis_kontor_sorgula")]
        public KontorSorgulaData KontorSorgula { get; set; }
    }

    public class KontorSorgulaResponse
    {
        [JsonProperty("result")]
        public KontorSayisi Result { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        public bool IsSuccess => Code == 200;
    }

    public class KontorSayisi
    {
        [JsonProperty("kontorsayisi")]
        public decimal? Kontor { get; set; }

    }

    //public class FaturaListeParams : BaseApiRequestParams
    //{
    //}

    //public class FaturaListeleRequest
    //{
    //    [JsonProperty("scf_fatura_listele")]
    //    public FaturaListeParams FaturaListe { get; set; }
    //}

    //public class FaturaListeResponse : BaseApiGetResponse
    //{
    //}

    /// <summary>
    /// DIA API'sine gönderilecek çoğu istek için ortak parametreleri içeren temel sınıf.
    /// Login, Logout, KontorSorgula gibi özel yapısı olan istekler bunu kullanmaz.
    /// </summary>
    public class BaseApiRequestParams
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("firma_kodu")]
        public int FirmaKodu { get; set; } = ConfigHelper.DiaFirmaKodu;

        [JsonProperty("donem_kodu")]
        public int DonemKodu { get; set; } = ConfigHelper.DiaDonemKodu;

        [JsonProperty("filters", NullValueHandling = NullValueHandling.Ignore)]
        private List<Filter> filters { get; set; } = new List<Filter>();

        [JsonProperty("sorts", NullValueHandling = NullValueHandling.Ignore)]
        private List<Sort> sorts { get; set; } = new List<Sort>();

        [JsonProperty("params", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Params { get; set; } = new Dictionary<string, object>();

        [JsonProperty("limit")]
        private int limit { get; set; } = 0;

        [JsonProperty("offset")]
        private int offset { get; set; } = 0;

        [JsonProperty("kart")]
        public object Kart { get; set; }



        [JsonProperty("table_name")]
        public string TableName { get; set; }


        [JsonProperty("template_type")]
        public string TemplateType { get; set; }


        [JsonProperty("column_name")]
        public string ColumnName { get; set; }

        public BaseApiRequestParams Limit(int value)
        {
            limit = value;
            return this;
        }
        public BaseApiRequestParams Offset(int value)
        {
            offset = value;
            return this;
        }

        public BaseApiRequestParams AddFilter(string value, string field, string _operator)
        {
            filters.Add(
                new Filter()
                {
                    value = value,
                    field = field,
                    _operator = _operator
                });
            return this;
        }
        public BaseApiRequestParams AddParam(string field, object value)
        {
            Params[field] = value;
            return this;
        }
        public BaseApiRequestParams AddSort(string field, string sorttype)
        {
            sorts.Add(
                new Sort()
                {
                    field = field,
                    sorttype = sorttype
                });
            return this;
        }
        public BaseApiRequestParams PostObject(object postObject)
        {
            Kart = postObject;
            return this;
        }
    }

    public class Filter
    {
        public string field { get; set; }
        public string value { get; set; }
        public string _operator { get; set; }
    }

    public class FilterTypes
    {
        public const string LESS_THAN = "<";
        public const string GREATER_THEN = ">";
        public const string LESS_THAN_OR_EQUAL = "<=";
        public const string GREATER_THEN_OR_EQUA = ">=";
        public const string EQUAL = "=";
        public const string NOT_EQUAL = "!";
        public const string IN = "IN";
        public const string NOT_IN = "NOT IN";
    }

    public class Sort
    {
        public string field { get; set; }
        public string sorttype { get; set; }
    }

    public class SortTypes
    {
        public const string ASC = "ASC";
        public const string DESC = "DESC";
    }
}
