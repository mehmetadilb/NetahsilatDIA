using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class DiaCurrencyResponseModel
    {
        [JsonProperty("_key")]
        public long Id { get; set; }

        [JsonProperty("adi")]
        public string Code { get; set; }
    }

    public class DiaCurrencyModel
    {
        [JsonProperty("_key")]
        public long Id { get; set; }

        [JsonProperty("adi")]
        public string Code { get; set; }
    }
}
