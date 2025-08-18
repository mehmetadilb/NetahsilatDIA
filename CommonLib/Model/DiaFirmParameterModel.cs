using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class DiaFirmParameterModel
    {
        [JsonProperty("_key")]
        public long Id { get; set; }

        [JsonProperty("degeri")]
        public string Value { get; set; }

        [JsonProperty("kodu")]
        public string Code { get; set; }
        [JsonProperty("parametreno")]
        public string ParameterNumber { get; set; }
    }
}
