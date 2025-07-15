using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Model
{
    public class VPosObject { public Vposlist vposlist { get; set; } }
    public class Vposlist
    {
        public List<Vpos> Vpos { get; set; }
    }

    public class Vpos
    {
        public string ERPCODE { get; set; }
        public string PB1PLAN { get; set; }
        public string PB2PLAN { get; set; }
        public string PB3PLAN { get; set; }
        public string PB4PLAN { get; set; }
        public string PB5PLAN { get; set; }
        public string PB6PLAN { get; set; }
        public string PB7PLAN { get; set; }
        public string PB8PLAN { get; set; }
        public string PB9PLAN { get; set; }
        public string PB10PLAN { get; set; }
        public string PB11PLAN { get; set; }
        public string PB12PLAN { get; set; }
        public string PBDEFPLAN { get; set; }
        public string Diger { get; set; }
    }
}
