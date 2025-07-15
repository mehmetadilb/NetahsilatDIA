using CommonLib;
using NetahsilatWebServiceLib.Accounts;
using System;
using System.Windows.Forms;

namespace NetahsilatDIA.UI
{
    public partial class frmCurrentAccountTransactions : Form
    {
        AccountManager myAccountManager;

        public frmCurrentAccountTransactions()
        {
            InitializeComponent(); 
            this.Load += frmCurrentAccountTransactions_Load;
        }


        private void frmCurrentAccountTransactions_Load(object sender, EventArgs e)
        {
            try
            {
                JsonDbManager.LoadParameters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Parametreler yüklenirken hata oluştu: {ex.Message}");
                return;
            }

            if (!string.IsNullOrWhiteSpace(Config.GlobalParameters.Parameters.ACCOUNT_SERVICE))
            {
                myAccountManager = new AccountManager();
            }
        }

        private void btnSendAccountTrans_Click(object sender, EventArgs e)
        {
            string cariKod = txtAccountCode.Text.Trim();
            bool allTransactions = chkAllTransactions.Checked;

            if (myAccountManager == null)
            {
                MessageBox.Show("Cari Hesap Ekstre servisi yapılandırılmamış. Lütfen ayarları kontrol edin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!allTransactions && string.IsNullOrEmpty(cariKod))
            {
                MessageBox.Show("Lütfen cari hesap kodunu giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (allTransactions)
                    myAccountManager.SendAccountTransactionDaily();
                else
                    myAccountManager.SendAccountTransaction(cariKod, true);

                MessageBox.Show("Hareketler Aktarıldı");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Aktarım sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
