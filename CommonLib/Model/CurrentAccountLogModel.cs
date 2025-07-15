using System;

namespace CommonLib.Model
{
    public class CurrentAccountLogModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public long Firm { get; set; }
        public long Period { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
        public DateTime RecordDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
} 