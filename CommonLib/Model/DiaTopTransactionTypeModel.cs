using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class DiaTopTransactionTypeModel
    {
        [JsonProperty("_key")]
        public long Id { get; set; }

        [JsonProperty("kodu")]
        public string Code { get; set; }
    }
}
