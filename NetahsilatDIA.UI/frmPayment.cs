using CommonLib;
using CommonLib.Enum;
using NetahsilatWebServiceLib.Payments;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetahsilatDIA.UI
{
    public partial class frmPayment : Form
    {
        private int PaymentType = 0;

        public frmPayment()
        {
            InitializeComponent();
            cmbOrderType.SelectedIndex = 0;
        }

        private void frmPayment_Load(object sender, EventArgs e)
        {
            lblTransferType.Text += Config.GlobalParameters.GlobalSettings.TRANSFER_MODE;
        }

        private void TransferPayment()
        {
            try
            {
                HideFormElements();

                if (string.IsNullOrEmpty(txtReferenceNumber.Text))
                    GetTransaction(TransactionType.Payment);
                else
                    GetPaymentByRefNo(txtReferenceNumber.Text);
            }
            catch (Exception ex)
            {
                Logging.AddLog(ex.Message);
                MessageBox.Show("İşlem Sırasında Bir Hata Oluştu. Log dosyanızı kontrol ediniz.", "Aktarım Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnClose.PerformClick();
            }
            finally
            {
                ShowFormElements();
            }
        }

        private void HideFormElements()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(HideFormElements));
                return;
            }
            btnGetPayment.Enabled = false;
            btnGetReversal.Enabled = false;
            btnClose.Enabled = false;
            PaymentType = cmbOrderType.SelectedIndex;
        }

        private void ShowFormElements()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ShowFormElements));
                return;
            }
            btnGetReversal.Enabled = true;
            btnGetPayment.Enabled = true;
            btnClose.Enabled = true;
        }

        private void TransferReversal()
        {
            try
            {
                HideFormElements();

                if (string.IsNullOrEmpty(txtReferenceNumber.Text))
                    GetTransaction(TransactionType.Reversal);
                else
                    MessageBox.Show("Referans numarasına göre iade işlemi aktaramazsınız. \n İade işlemleri sadece tarih aralığına göre aktarılır.", "Iade Aktarımı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logging.AddLog(ex.Message);
                MessageBox.Show("İşlem Sırasında Bir Hata Oluştu. Log dosyanızı kontrol ediniz.", "Aktarım Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnClose.PerformClick();
            }
            finally
            {
                ShowFormElements();
            }
        }
        private void GetPaymentByRefNo(string referenceNumber)
        {
            PaymentManager myPaymentManager = new PaymentManager();
            bool result = myPaymentManager.GetPaymentByReferenceNumber(referenceNumber);

            if (result)
            {
                MessageBox.Show($"{referenceNumber} referans numaralı ödeme başarıyla aktarıldı.", "Başarılı İşlem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"{referenceNumber} referans numaralı ödeme aktarılamadı! Log dosyasını kontrol ediniz.", "Başarısız İşlem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetTransaction(TransactionType tranType)
        {
            PaymentManager myPaymentManager = new PaymentManager();
            int transactionCount = 0;

            if (tranType == TransactionType.Payment)
            {
                transactionCount = myPaymentManager.GetPaymentFromDate(dtpAllFirstDate.Value.ToString("dd.MM.yyyy"), dtpAllLastDate.Value.ToString("dd.MM.yyyy"), PaymentType);
            }
            else
            {
                transactionCount = myPaymentManager.GetReversalFromDate(dtpAllFirstDate.Value.ToString("dd.MM.yyyy"), dtpAllLastDate.Value.ToString("dd.MM.yyyy"), PaymentType);
            }

               if (transactionCount > 0)
            {
                MessageBox.Show(transactionCount.ToString() + " Adet Ödeme / İade Logo'ya Aktarıldı", "Aktarım Bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
              {
                MessageBox.Show("Belirtilen tarihler arasında ödeme / iade kaydınız bulunmamaktadır.", "Aktarım Bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void btnGetPayment_Click(object sender, EventArgs e)
        {
            await Task.Run(() => TransferPayment());
        }

        private async void btnGetReversal_Click(object sender, EventArgs e)
        {
            await Task.Run(() => TransferReversal());
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
