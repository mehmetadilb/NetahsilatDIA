using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enum
{
    public enum LogType
    {
        [Display(Name = "Bilgi")]
        Info,

        [Display(Name = "Uyarı")]
        Warning,

        [Display(Name = "Hata")]
        Error,

        [Display(Name = "Debug")]
        Debug
    }
}
