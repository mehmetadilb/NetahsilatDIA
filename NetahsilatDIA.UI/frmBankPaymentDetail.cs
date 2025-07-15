using CommonLib;
using NetahsilatWebServiceLib.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetahsilatDIA.UI
{
    public partial class frmBankPaymentDetail : Form
    {
        public frmBankPaymentDetail()
        {
            InitializeComponent();
            btnTransfer.Location = new Point((this.Width - btnTransfer.Width) / 2, btnTransfer.Location.Y);
        }

        private void frmBankPaymentDetail_Load(object sender, EventArgs e)
        {

        }

        private void cbGetNonIntegrated_CheckedChanged(object sender, EventArgs e)
        {
            if (cbGetNonIntegrated.Checked)
            {
                txtOrderReference.Enabled = false;
            }
            else
            {
                txtOrderReference.Enabled = true;
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            if (Config.GlobalParameters.GlobalSettings.GET_BANK_PAYMENT)
            {
                var paymentManager = new PaymentManager();

                if (cbGetNonIntegrated.Checked)
                {
                    paymentManager.GetNonIntegratedPayment();
                }
                else
                {
                    paymentManager.GetPaymentByReferenceNumber(txtOrderReference.Text.Trim());
                }

                MessageBox.Show("Aktarım Tamamlandı");
            }
            else
            {
                MessageBox.Show("Bu Ekranı Kullanmak için Pos Rapor sahibi olmalısınız.");
            }
        }
    }
}
