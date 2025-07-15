using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLib;
using Netahsilat.DIAService;
using static Netahsilat.DIAService.ConfigHelper;

namespace NetahsilatDIA.UI
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            try
            {
                JsonDbManager.LoadParameters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Parametreler yüklenirken hata oluştu: {ex.Message}");
                return;
            }

            var parameters = Config.GlobalParameters?.Parameters;
            var firm = parameters?.Firms?.FirstOrDefault();

            if (firm == null || parameters == null)
            {
                MessageBox.Show("Konfigürasyon eksik! Lütfen ayarları kontrol edin.");
                return;
            }
            var firmDto = new DiaFirm
            {
                Company = firm.Company,
                Period = firm.Period,
                DiaUserName = firm.DiaUserName,
                DiaPassword = firm.DiaPassword,
                DiaLang = firm.DiaLang,
                DisconnectSameUser = firm.DisconnectSameUser,
                DiaApiKey = firm.DiaToken,
            };

            var paramDto = new DiaParameters
            {
                DiaBaseUrl = parameters.DiaBaseUrl,
                DiaApiEndpoint = parameters.DiaApiEndpoint,
                DiaSessionErrorMessageHint = parameters.DiaSessionErrorMessageHint
            };

            ConfigHelper.Init(paramDto, firmDto);
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            new frmPayment().ShowDialog();
            //var btnContext = (Button)sender;
            //AddContextMenuStrip(btnContext, contextMenuStrip1);
        }
        private void btnAccount_Click(object sender, EventArgs e)
        {
            AddContextMenuStrip(sender, contextMenuStrip2);
        }

        private void AddContextMenuStrip(object sender,ContextMenuStrip contextMenu)
        {
            var btnContext = (Button)sender;
            var controlLocation = btnContext.PointToScreen(Point.Empty);

            contextMenu.Show(controlLocation.X - btnContext.Location.X + 12, controlLocation.Y + btnContext.Height + 6);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            new frmParametersConfig().ShowDialog();
        }

        private void cariHesapYönetimiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmCustomer().ShowDialog();
        }
        private void cariHesapEkstreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmCurrentAccountTransactions().ShowDialog();
        }

        private void menü1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmBankPaymentDetail().ShowDialog();
        }

        private void menü2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmPayment().ShowDialog();
        }
    }
}
