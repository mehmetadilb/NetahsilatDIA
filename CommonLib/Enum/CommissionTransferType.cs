using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enum
{
    public enum CommissionTransferType
    {
        [Display(Name = "Komisyon Dahil")]
        PureAmount = 0, 
        
        [Display(Name = "Net Tutar")]
        NetAmount = 1,
    }
}
