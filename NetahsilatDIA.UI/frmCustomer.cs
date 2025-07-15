using CommonLib;
using Netahsilat.DIAService;
using NetahsilatDIA.UI.CostomControl;
using NetahsilatWebServiceLib.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NetahsilatDIA.UI
{
    public partial class frmCustomer : Form
    {
        private AccountManager accountManager;

        public frmCustomer()
        {
            InitializeComponent();
        }

        private void frmCustomer_Load(object sender, EventArgs e)
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

            if (JsonDbManager.GlobalParameters.Parameters.ACCOUNT_SERVICE != null)
            {
                accountManager = new AccountManager();
            }

            var list = JsonDbManager.GlobalParameters.GlobalSettings.CUSTOMERPAYMENTSETS;
            if (list != null)
            {
                foreach (var customer in list)
                {
                    var control = CreateCustomerPaymentSetControlFromModel(customer);
                    flwCustomerMapping.Controls.Add(control);
                }
            }
        }

        private CustomerPaymentSetControl CreateCustomerPaymentSetControlFromModel(CustomerPaymentSet customer)
        {
            var control = new CustomerPaymentSetControl
            {
                Id = customer.Id,
                PaymentSetId = customer.PaymentSetId.ToString(),
                CustomerPrefix = customer.CustomerPrefix,
                FirmNo = customer.FirmNo,
                CurrentAccountTypeId = customer.CurrentAccountTypeId.ToString()
            };

            control.SetDeleteButtonEnabled(true);
            control.DeleteClicked += CustomerControl_DeleteClicked;

            return control;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var errorMessages = new List<string>();

            if (!int.TryParse(txtPaymentPlanId.Text.Trim(), out int paymentSetId))
            {
                errorMessages.Add("Ödeme Planı ID'si geçersiz veya boş.");
            }

            if (!int.TryParse(txtCurrentAccountTypeId.Text.Trim(), out int currentAccountTypeId))
            {
                errorMessages.Add("Cari Hesap Türü ID'si geçersiz veya boş.");
            }

            if (errorMessages.Count > 0)
            {
                MessageBox.Show(string.Join(Environment.NewLine, errorMessages), "Hatalı Giriş", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newControl = new CustomerPaymentSetControl
            {
                Id = Guid.NewGuid().ToString(),
                CustomerPrefix = txtCustomerPrefix.Text.Trim(),
                PaymentSetId = paymentSetId.ToString(),
                FirmNo = txtFirmNo.Text.Trim(),
                CurrentAccountTypeId = currentAccountTypeId.ToString()
            };

            newControl.SetDeleteButtonEnabled(true);
            newControl.DeleteClicked += CustomerControl_DeleteClicked;

            flwCustomerMapping.Controls.Add(newControl);

            // Temizle
            txtCustomerPrefix.Clear();
            txtPaymentPlanId.Clear();
            txtFirmNo.Clear();
            txtCurrentAccountTypeId.Clear();
        }

        private void CustomerControl_DeleteClicked(object sender, EventArgs e)
        {
            var control = sender as CustomerPaymentSetControl;
            if (control == null) return;

            var result = MessageBox.Show("Bu kaydı silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Listeden sil
                var list = JsonDbManager.GlobalParameters.GlobalSettings.CUSTOMERPAYMENTSETS;
                var toRemove = list?.FirstOrDefault(c => c.Id == control.Id);
                if (toRemove != null)
                {
                    list.Remove(toRemove);
                }

                // UI'dan kaldır
                flwCustomerMapping.Controls.Remove(control);
                // Burada JSON'a kaydetme yok, sadece Kaydet butonunda olacak
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var list = new List<CustomerPaymentSet>();

                foreach (Control ctrl in flwCustomerMapping.Controls)
                {
                    if (ctrl is CustomerPaymentSetControl cpsc)
                    {
                        if (int.TryParse(cpsc.PaymentSetId, out int paymentSetId) &&
                            int.TryParse(cpsc.CurrentAccountTypeId, out int currentAccountTypeId))
                        {
                            list.Add(new CustomerPaymentSet
                            {
                                Id = cpsc.Id,
                                CustomerPrefix = cpsc.CustomerPrefix,
                                PaymentSetId = paymentSetId,
                                FirmNo = cpsc.FirmNo,
                                CurrentAccountTypeId = currentAccountTypeId
                            });
                        }
                        else
                        {
                            MessageBox.Show("Listede geçersiz veriler var. Kontrol ediniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                JsonDbManager.GlobalParameters.GlobalSettings.CUSTOMERPAYMENTSETS = list;

                JsonDbManager.SaveParametersToJson();

                MessageBox.Show("Değişiklikler başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kaydetme sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSendAccount_Click(object sender, EventArgs e)
        {
            if (accountManager == null)
            {
                MessageBox.Show("Cari Hesap servisi yapılandırılmamış. Lütfen ayarları kontrol edin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string accountCode = txtAccountCode.Text.Trim();

            if (!string.IsNullOrEmpty(accountCode))
            {
                var infoResult = MessageBox.Show(
                    $"Sadece '{accountCode}' kodlu cari hesap aktarılacak. Devam etmek istiyor musunuz?",
                    "Belirli Cari Hesap Aktarımı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (infoResult != DialogResult.Yes)
                    return;
            }

            if (!cboxTransferFailedAccounts.Checked)
            {
                accountManager.SendAccount(false, accountCode);
                return;
            }

            var dialogResult = MessageBox.Show(
                "Daha önce aktarım sırasında hata alınan veya eksik verisi olan hesaplar tekrar aktarılacak. Onaylıyor musunuz?",
                "Cari Hesap Aktarımı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
                accountManager.SendAccount(isDaily: true, accountCode);
        }
    }
}
