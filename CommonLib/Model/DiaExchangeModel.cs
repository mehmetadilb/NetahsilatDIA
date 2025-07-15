using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class DiaExchangeModel
    {
        [JsonProperty("_key")]
        public long Id { get; set; }

        [JsonProperty("_key_sis_doviz")]
        public long CurrencyId { get; set; }

        [JsonProperty("kur1")]
        public decimal Rate { get; set; }

        [JsonProperty("_cdate")]
        public DateTime Date { get; set; }
    }
}
