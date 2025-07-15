using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enum
{
    public enum NonCustomerPayment
    {
        [Display(Name = "Aktarım Yapma")]
        Nothing = 0, 
        [Display(Name = "Torba Hesabı Kullan")]
        UseBagAccount = 1, 
    }
}