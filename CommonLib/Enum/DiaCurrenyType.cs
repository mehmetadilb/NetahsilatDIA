using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enum
{
    public enum DiaCurrenyType
    {
        [Display(Name = "TL")]
        TL = 1,
        [Display(Name = "USD")]
        USD = 2,
        [Display(Name = "EUR")]
        EUR = 3,
    }
}
