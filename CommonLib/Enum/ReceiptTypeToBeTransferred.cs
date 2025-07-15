using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Enum
{
    public enum ReceiptTypeToBeTransferred : short
    {
        /// <summary>
        /// Kredi Kartı
        /// </summary>
        [Display(Name = "Kredi Kartı")]
        CreditCard = 0,
        /// <summary>
        /// Virman
        /// </summary>
        [Display(Name = "Virman")]
        TransferFiche = 1,
        /// <summary>
        /// Hepsi
        /// </summary>
        [Display(Name = "Kredi Kartı/Virman")]
        All = 2,
    }
}
