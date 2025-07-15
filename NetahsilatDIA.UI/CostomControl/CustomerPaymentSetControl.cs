using CommonLib;
using Netahsilat.DIAService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetahsilatDIA.UI.CostomControl
{
    public partial class CustomerPaymentSetControl : UserControl
    {
        public string Id { get; set; }
        public event EventHandler DeleteClicked;
        public CustomerPaymentSetControl()
        {
            InitializeComponent();
        }

        public string CustomerPrefix
        {
            get { return txtCustomerPrefix.Text; }
            set { txtCustomerPrefix.Text = value; }
        }
        public string PaymentSetId
        {
            get { return txtPaymentSetId.Text; }
            set { txtPaymentSetId.Text = value; }
        }
        public string FirmNo
        {
            get { return txtFirmNo.Text; }
            set { txtFirmNo.Text = value; }
        }
        public string CurrentAccountTypeId
        {
            get { return txtCurrentAccountTypeId.Text; }
            set { txtCurrentAccountTypeId.Text = value; }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteClicked?.Invoke(this, EventArgs.Empty);
        }
        public void SetDeleteButtonEnabled(bool enabled)
        {
            btnDelete.Enabled = enabled;
            btnDelete.Visible = enabled;
        }
    }
}
