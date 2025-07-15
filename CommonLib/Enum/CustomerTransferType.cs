using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enum
{
    public enum CustomerTransferType
    {
        [Display(Name = "Müşteri")]
        Customer = 0,
        [Display(Name = "Bayi")]
        Vendor = 1
    }
}
