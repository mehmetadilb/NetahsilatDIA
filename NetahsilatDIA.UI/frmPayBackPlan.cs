using CommonLib;
using CommonLib.Model;
using NetahsilatWebServiceLib.ErpWebService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetahsilatDIA.UI
{
    public partial class frmPayBackPlan : Form
    {
        private VPosObject vposList;
        private string originalVPosList;
        private BasicHttpBinding_IErpWebService ews;
        public frmPayBackPlan()
        {
            ews = new BasicHttpBinding_IErpWebService();
            InitializeComponent();
            vposList = JsonDbManager.LoadFromFile<VPosObject>("PayBackPlan.json");
            if (vposList?.vposlist == null)
            {
                vposList = vposList ?? new VPosObject();
                vposList.vposlist = new Vposlist();
                vposList.vposlist.Vpos = new List<Vpos>();
            }
            originalVPosList = JsonConvert.SerializeObject(vposList);
        }

        private async void frmPayBackPlan_Load(object sender, EventArgs e)
        {
            if (Config.GlobalParameters.Parameters != null && !string.IsNullOrWhiteSpace(Config.GlobalParameters.Parameters.ERP_SERVICE))
            {
                System.Net.ServicePointManager.Expect100Continue = false;
                ews.Url = Config.GlobalParameters.Parameters.ERP_SERVICE;
                AuthenticationInfo authInfo = new AuthenticationInfo
                {
                    UserName = Config.GlobalParameters.Parameters.WEB_SERVICE_UID,
                    Password = Config.GlobalParameters.Parameters.WEB_SERVICE_PWD
                };

                await Task.Run(() => LoadVPoses(authInfo));
                dgv.DataSource = vposList.vposlist.Vpos;
            }
            else
            {
                MessageBox.Show("Öncelikle genel parametre tanımı yapınız");
            }
        }

        private void LoadVPoses(AuthenticationInfo authInfo)
        {
            try
            {
                var virtualPosResult = ews.GetVirtualPosList(authInfo);

                if (!virtualPosResult.IsSuccess || virtualPosResult.VirtualPoses == null || !virtualPosResult.VirtualPoses.Any())
                {
                    throw new Exception("Sanal pos bilgileri Erp servisden alınamadı.");
                }

                var erpCodeEmptyNumber = virtualPosResult.VirtualPoses.Count(x => string.IsNullOrEmpty(x.ErpCode));

                var virtualPosList = virtualPosResult.VirtualPoses.Where(x => !string.IsNullOrEmpty(x.ErpCode)).ToArray();

                foreach (VirtualPos pos in virtualPosList)
                {
                    if (!vposList.vposlist.Vpos.Any(x => x.ERPCODE == pos.ErpCode))
                    {
                        vposList.vposlist.Vpos.Add(new Vpos() { ERPCODE = pos.ErpCode });
                    }
                }

                string message = string.Empty;

                if (vposList.vposlist.Vpos.Count != virtualPosList.Length)
                {
                    message += "#Sanal poslar içerisinde birden fazla aynı erp kod değerine sahip poslar tespit edildi. Aynı erp kodlu sanal poslar listelenmeyecek";
                }

                if (erpCodeEmptyNumber > 0)
                {
                    message += $"\n#{erpCodeEmptyNumber} adet erp kod alanı tanımlanmamış sanal pos bulundu. Erp kodu tanımlı olmayan poslar listelenmeyecek";
                }

                if (!vposList.vposlist.Vpos.Any())
                {
                    message += "\n#Sanal pos bilgileri bulunamadı";
                }

                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, "Geri Ödeme Planı Eşleştirme Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // Log ekleme için örnek (Logging sınıfınız varsa kullanabilirsiniz)
                Logging.AddLog($"LoadVPoses error: {ex.Message}");

                MessageBox.Show("Sanal poslar okunurken bir hata alındı. Logları kontrol ediniz", "Sanal Pos Okuma Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (JsonConvert.SerializeObject(vposList) != originalVPosList)
            {
                switch (CloseForm())
                {
                    case DialogResult.Cancel:
                        return;
                    case DialogResult.Yes:
                        btnSave_Click(null, null);
                        break;
                    case DialogResult.No:
                        vposList = JsonConvert.DeserializeObject<VPosObject>(originalVPosList);
                        break;
                    default:
                        break;
                }
            }
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                JsonDbManager.SaveToFile(vposList, "PayBackPlan.json");
                originalVPosList = JsonConvert.SerializeObject(vposList);
                MessageBox.Show("Geri ödeme planı kaydedildi");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmPayBackPlan_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && JsonConvert.SerializeObject(vposList) != originalVPosList)
            {
                var result = CloseForm();
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (result == DialogResult.No)
                {
                    vposList = JsonConvert.DeserializeObject<VPosObject>(originalVPosList);
                    btnCancel_Click(null, EventArgs.Empty); // parametre uyumlu şekilde çağırıldı
                }
                else
                {
                    btnSave_Click(null, EventArgs.Empty);
                }
            }
        }

        private DialogResult CloseForm()
        {
            return MessageBox.Show("Kaydedilmeyen değişiklikler var. Yapılan değişiklikler kaydedilsin mi?", "Veri Kaybı Uyarısı!", MessageBoxButtons.YesNoCancel);
        }
    }
}
