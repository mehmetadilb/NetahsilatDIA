using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class LogModel
    {
        public string LogType { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string Location { get; set; }
        public object ObjectValue { get; set; }
        public Type ObjectValueType { get; set; }
    }
}
