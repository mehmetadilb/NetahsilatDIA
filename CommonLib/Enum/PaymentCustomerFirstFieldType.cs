using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enum
{
    public enum PaymentCustomerFirstFieldType
    {
        [Display(Name = "Varsayılan")]
        Default,

        [Display(Name = "Cari Hesap Kodu")]
        AccountErpCode,

        [Display(Name = "Bayi/Müşteri Erp Kodu")]
        AgentErpCode,

        [Display(Name = "Bayi/Müşteri Kodu")]
        AgentCode,

        [Display(Name = "Bayi/Müşteri Tckn")]
        AgentTckn,

        [Display(Name = "Bayi/Müşteri Vkn")]
        AgentVkn
    }
}
