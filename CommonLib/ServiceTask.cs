using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public enum ServiceTask
    {
        /// <summary>
        /// Entegre edilmemiş ödemelerin alınması.
        /// </summary>
        NonIntegratedPayments,

        /// <summary>
        /// Günlük başarısız ödemelerin kontrolü.
        /// </summary>
        UnsuccessPayments,

        /// <summary>
        /// Cari hesapların gönderilmesi.
        /// </summary>
        CurrentAccount,

        /// <summary>
        /// Günlük başarısız cari hesapların gönderilmesi.
        /// </summary>
        UnsuccessCurrentAccount,

        /// <summary>
        /// Cari hesap ekstrelerinin (hareketlerinin) gönderilmesi.
        /// </summary>
        CurrentAccountTransaction,

        /// <summary>
        /// Günlük başarısız cari hesap ekstrelerinin gönderilmesi.
        /// </summary>
        UnsuccessCurrentAccountTransaction,

        /// <summary>
        /// Entegre edilmemiş banka ödeme detaylarının alınması.
        /// </summary>
        NonIntegratedBankPaymentDetail,

        /// <summary>
        /// Maksimum taksit sayılarının güncellenmesi.
        /// </summary>
        UpdateMaxInstallments
    }
}
