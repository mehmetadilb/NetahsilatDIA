using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class DiaFirmInfoModel
    {
        [JsonProperty("_key")]
        public long Id { get; set; }

        [JsonProperty("_key_sis_doviz_raporlama")]
        public DiaCurrencyModel ReportCurrency { get; set; }

        [JsonProperty("m_subeler")]
        public List<FirmBranchModel> Branches { get; set; } = new List<FirmBranchModel>();

        [JsonProperty("_key_sis_doviz_list")]
        public List<DiaCurrencyModel> Currencies { get; set; }
    }

    public class FirmBranchModel
    {
        [JsonProperty("_key")]
        public long Id { get; set; }

        [JsonProperty("subeadi")]
        public string Name { get; set; }

        [JsonProperty("subekodu")]
        public string Code { get; set; }

        [JsonProperty("merkezmi")]
        public string IsCenterRaw { get; set; }

        [JsonProperty("durum")]
        public string IsActiveRaw { get; set; }

        [JsonIgnore]
        public bool IsCenter => IsCenterRaw == "t";

        [JsonIgnore]
        public bool IsActive => IsActiveRaw == "A";
    }
}
