using CommonLib;
using CommonLib.Enum;
using CommonLib.Model;
using Netahsilat.DIAService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetahsilatDIA.UI
{
    public partial class frmParametersConfig : Form
    {
        private string _originalConfigString;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true, // JSON'ı okunabilir formatta yazar
            PropertyNameCaseInsensitive = true // Yüklerken büyük/küçük harf duyarsız eşleştirme
        };
        public frmParametersConfig()
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
        }

        private void frmParametersConfig_Load(object sender, EventArgs e)
        {
            PopulateControlsFromSettings();
            SetupDataGridView();
            _originalConfigString = JsonConvert.SerializeObject(Config.GlobalParameters, Formatting.Indented);
        }

        private void SetupDataGridView()
        {
            dgvFirms.AutoGenerateColumns = false; // Kolonları manuel tanımlayacağız
            dgvFirms.AllowUserToAddRows = false; // Kullanıcı doğrudan grid'e satır eklemesin
            dgvFirms.AllowUserToDeleteRows = false; // Kullanıcı doğrudan grid'den satır silmesin
            dgvFirms.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Tam satır seçimi
            dgvFirms.MultiSelect = false; // Tek satır seçimi
            dgvFirms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Kolonları doldur

            dgvFirms.Columns.Clear(); // Önceki kolonları temizle

            // Kolonları Oluştur
            dgvFirms.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCompany",
                HeaderText = "Firma No",
                DataPropertyName = nameof(Firm.Company)
            });

            dgvFirms.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPeriod",
                HeaderText = "Dönem No",
                DataPropertyName = nameof(Firm.Period)
            });

            dgvFirms.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUserName",
                HeaderText = "Kullanıcı Adı",
                DataPropertyName = nameof(Firm.DiaUserName)
            }); 
            
            dgvFirms.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPassword",
                HeaderText = "Şifre",
                DataPropertyName = nameof(Firm.DiaPassword)
            });

            dgvFirms.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colIsActive",
                HeaderText = "Aktif",
                DataPropertyName = nameof(Firm.IsActive) // Firm sınıfındaki IsActive özelliğine bağla
            });

            dgvFirms.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colToken",
                HeaderText = "Token",
                DataPropertyName = nameof(Firm.DiaToken) // Firm sınıfındaki Token özelliğine bağla
            });


            dgvFirms.CellFormatting += (s, e) =>
            {
                if (dgvFirms.Columns[e.ColumnIndex].Name == "colPassword" && e.Value != null)
                {
                    e.Value = new string('●', e.Value.ToString().Length);
                    e.FormattingApplied = true;
                }
            };

            // Düzenleme kontrolü gösterilirken TextBox'un PasswordChar'ını ayarla
            dgvFirms.EditingControlShowing += (s, e) =>
            {
                if (dgvFirms.CurrentCell.OwningColumn.Name == "colPassword")
                {
                    if (e.Control is TextBox tb)
                    {
                        tb.UseSystemPasswordChar = true; // Bu TextBox'u şifre gibi gösterir
                    }
                }
                else
                {
                    if (e.Control is TextBox tb)
                    {
                        tb.UseSystemPasswordChar = false; // Diğer kolonlarda şifre karakteri olmasın
                    }
                }
            };


            dgvFirms.DataSource = new BindingList<Firm>(Config.GlobalParameters.Parameters.Firms);
            
            // DataGridView değişikliklerini dinle
            if (dgvFirms.DataSource is BindingList<Firm> bindingList)
            {
                bindingList.ListChanged += (s, e) =>
                {
                    // DataGridView değiştiğinde Config.GlobalParameters'ı güncelle
                    Config.GlobalParameters.Parameters.Firms = bindingList.ToList();
                };
            }
        }

        private void btnPayBackPlan_Click(object sender, EventArgs e)
        {
            new frmPayBackPlan().ShowDialog();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Mevcut durumu kontrol et
            var currentConfigString = JsonConvert.SerializeObject(Config.GlobalParameters, Formatting.Indented);
            
            if (currentConfigString != _originalConfigString)
            {
                var result = MessageBox.Show("Kaydedilmeyen değişiklikler var. Yapılan değişiklikler kaydedilsin mi?", "Veri Kaybı Uyarısı!", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Cancel:
                        return;
                    case DialogResult.Yes:
                        btnSave_Click(null, null);
                        break;
                    case DialogResult.No:
                    default:
                        // Değişiklikleri geri al
                        Config.GlobalParameters = JsonConvert.DeserializeObject<GlobalParameters>(_originalConfigString);
                        PopulateControlsFromSettings();
                        break;
                }
            }
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSettingsFromControls();
                SaveSettingsToFile();

                _originalConfigString = JsonConvert.SerializeObject(Config.GlobalParameters, Formatting.Indented);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void PopulateControlsFromSettings()
        {
            if (Config.GlobalParameters == null) return;

            // Form üzerindeki tüm kontrolleri dolaş (iç içe kontroller için recursive)
            IterateControls(this.Controls);
        }

        private void IterateControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control.Tag != null && !string.IsNullOrEmpty(control.Tag.ToString()))
                {
                    string propertyName = control.Tag.ToString();
                    PropertyInfo propInfo = typeof(GlobalSettings).GetProperty(propertyName);

                    if (propInfo != null)
                    {
                        object value = propInfo.GetValue(Config.GlobalParameters.GlobalSettings);
                        SetControlValue(control, propInfo.PropertyType, value);
                        AddControlChangeListener(control, propInfo, true);
                    }
                    else
                    {
                        propInfo = typeof(Parameters).GetProperty(propertyName);
                        if (propInfo != null)
                        {
                            object value = propInfo.GetValue(Config.GlobalParameters.Parameters);
                            SetControlValue(control, propInfo.PropertyType, value);
                            AddControlChangeListener(control, propInfo, false);
                        }
                    }
                }

                // Eğer kontrolün içinde başka kontroller varsa (örn: Panel, GroupBox)
                if (control.Controls.Count > 0)
                {
                    IterateControls(control.Controls);
                }
            }
        }


        private void SetControlValue(Control control, Type propertyType, object value)
        {
            if (value == null && propertyType != typeof(string) && !propertyType.IsValueType) // Referans tipleri için null kontrolü (string hariç)
            {
                // Eğer değer null ise ve kontrol DataGridView ise ve özellik bir liste ise, boş bir liste atayabiliriz.
                if (control is DataGridView dgv && propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    dgv.DataSource = null; // Veya Activator.CreateInstance(propertyType) ile boş liste
                }
                // Diğer null durumları için özel işlem gerekebilir veya varsayılan bırakılabilir.
                return;
            }

            if (control is CheckBox chk)
            {
                chk.Checked = Convert.ToBoolean(value);
            }
            else if (control is TextBox txt)
            {
                txt.Text = Convert.ToString(value);
            }
            else if (control is NumericUpDown nud)
            {
                // NumericUpDown'ın Value özelliği decimal'dir.
                nud.Value = Convert.ToDecimal(value);
            }
            else if (control is ComboBox cmb)
            {
                if (propertyType.IsEnum)
                {
                    if (cmb.Items.Count == 0) // Sadece bir kere doldur
                    {
                        var enumValues = Enum.GetValues(propertyType)
                            .Cast<Enum>()
                            .Select(e => new
                            {
                                Value = e,
                                Display = e.GetDisplayName()
                            })
                            .ToList();

                        cmb.DataSource = enumValues;
                        cmb.DisplayMember = "Display";
                        cmb.ValueMember = "Value";
                    }

                    // Seçili item'ı ayarla
                    cmb.SelectedValue = value;
                }
                else // String tabanlı ComboBox (bu örnekte yok ama genel amaçlı)
                {
                    cmb.Text = Convert.ToString(value);
                }
            }
            else if (control is DataGridView dgv)
            {
                // CUSTOMERPAYMENTSETS listesini DataGridView'e bağla
                if (propertyType == typeof(List<CustomerPaymentSet>))
                {
                    dgv.DataSource = null; // Önce temizle
                    dgv.DataSource = value as List<CustomerPaymentSet>;
                    // İsteğe bağlı: Kolonları otomatik oluşturmayı kapatıp manuel tanımlayabilirsiniz.
                    // dgv.AutoGenerateColumns = true; // Veya false yapıp kolonları tasarımcıda/kodla ekleyin
                }
            }
            // Diğer kontrol tipleri için buraya eklemeler yapabilirsiniz.
        }


        private void UpdateSettingsFromControls()
        {
            IterateControlsForUpdate(this.Controls);
            
            // DataGridView için özel kontrol - Firms listesini güncelle
            if (dgvFirms.DataSource is BindingList<Firm> bindingList)
            {
                Config.GlobalParameters.Parameters.Firms = bindingList.ToList();
            }
        }

        private void IterateControlsForUpdate(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control.Tag != null && !string.IsNullOrEmpty(control.Tag.ToString()))
                {
                    string propertyName = control.Tag.ToString();
                    PropertyInfo propInfo = typeof(GlobalSettings).GetProperty(propertyName);

                    if (propInfo != null)
                    {
                        object valueToSet = GetControlValue(control, propInfo.PropertyType);
                        if (valueToSet != null || propInfo.PropertyType.IsValueType || propInfo.PropertyType == typeof(string)) // String null olabilir
                        {
                            try
                            {
                                propInfo.SetValue(Config.GlobalParameters.GlobalSettings, valueToSet);
                            }
                            catch (ArgumentException argEx)
                            {
                                // Tip uyuşmazlığı gibi durumlar için
                                Console.WriteLine($"Hata: '{propertyName}' özelliğine değer atanırken tip uyuşmazlığı. Kontrol: {control.Name}, Değer: {valueToSet}. Detay: {argEx.Message}");
                            }
                        }
                    }
                    else
                    {
                        propInfo = typeof(Parameters).GetProperty(propertyName);

                        if (propInfo != null)
                        {
                            object valueToSet = GetControlValue(control, propInfo.PropertyType);
                            if (valueToSet != null || propInfo.PropertyType.IsValueType || propInfo.PropertyType == typeof(string)) // String null olabilir
                            {
                                try
                                {
                                    propInfo.SetValue(Config.GlobalParameters.Parameters, valueToSet);
                                }
                                catch (ArgumentException argEx)
                                {
                                    // Tip uyuşmazlığı gibi durumlar için
                                    Console.WriteLine($"Hata: '{propertyName}' özelliğine değer atanırken tip uyuşmazlığı. Kontrol: {control.Name}, Değer: {valueToSet}. Detay: {argEx.Message}");
                                }
                            }
                        }
                    }
                }
                if (control.Controls.Count > 0)
                {
                    IterateControlsForUpdate(control.Controls);
                }
            }
        }

        private object GetControlValue(Control control, Type expectedPropertyType)
        {
            if (control is CheckBox chk)
            {
                return chk.Checked;
            }
            if (control is TextBox txt)
            {
                return txt.Text;
            }
            if (control is NumericUpDown nud)
            {
                // Özellik tipine göre dönüştür
                if (expectedPropertyType == typeof(int)) return Convert.ToInt32(nud.Value);
                if (expectedPropertyType == typeof(long)) return Convert.ToInt64(nud.Value);
                if (expectedPropertyType == typeof(short)) return Convert.ToInt16(nud.Value);
                return nud.Value; // decimal olarak döner
            }
            if (control is ComboBox cmb)
            {
                if (expectedPropertyType.IsEnum)
                {
                    // Eğer ComboBox DisplayMember/ValueMember üzerinden bağlıysa
                    // doğrudan SelectedValue alınmalı
                    if (cmb.SelectedValue != null && expectedPropertyType.IsInstanceOfType(cmb.SelectedValue))
                        return cmb.SelectedValue;

                    // Değilse, SelectedItem deneyelim (enum doğrudan bind edilmiştir)
                    if (cmb.SelectedItem != null && expectedPropertyType.IsInstanceOfType(cmb.SelectedItem))
                        return cmb.SelectedItem;

                    // Enum tipine cast edilemiyorsa fallback olarak parse et
                    if (!string.IsNullOrEmpty(cmb.Text))
                    {
                        try
                        {
                            return Enum.Parse(expectedPropertyType, cmb.Text);
                        }
                        catch
                        {
                            return Enum.GetValues(expectedPropertyType).GetValue(0); // varsayılan değer
                        }
                    }

                    return Enum.GetValues(expectedPropertyType).GetValue(0); // fallback default enum
                }

                // Enum değilse normal string olarak döndür
                return cmb.Text;
            }
            if (control is DataGridView dgv)
            {
                return Config.GlobalParameters.GlobalSettings.CUSTOMERPAYMENTSETS ?? new List<CustomerPaymentSet>();
            }
            return null;
        }

        private void AddControlChangeListener(Control control, PropertyInfo propInfo, bool isGlobalSettings)
        {
            if (control is TextBox txt)
            {
                txt.TextChanged += (s, e) =>
                {
                    try
                    {
                        var value = Convert.ChangeType(txt.Text, propInfo.PropertyType);
                        if (isGlobalSettings)
                            propInfo.SetValue(Config.GlobalParameters.GlobalSettings, value);
                        else
                            propInfo.SetValue(Config.GlobalParameters.Parameters, value);
                    }
                    catch { /* Dönüştürme hatası olursa görmezden gel */ }
                };
            }
            else if (control is CheckBox chk)
            {
                chk.CheckedChanged += (s, e) =>
                {
                    if (isGlobalSettings)
                        propInfo.SetValue(Config.GlobalParameters.GlobalSettings, chk.Checked);
                    else
                        propInfo.SetValue(Config.GlobalParameters.Parameters, chk.Checked);
                };
            }
            else if (control is NumericUpDown nud)
            {
                nud.ValueChanged += (s, e) =>
                {
                    try
                    {
                        var value = Convert.ChangeType(nud.Value, propInfo.PropertyType);
                        if (isGlobalSettings)
                            propInfo.SetValue(Config.GlobalParameters.GlobalSettings, value);
                        else
                            propInfo.SetValue(Config.GlobalParameters.Parameters, value);
                    }
                    catch { /* Dönüştürme hatası olursa görmezden gel */ }
                };
            }
            else if (control is ComboBox cmb)
            {
                cmb.SelectedValueChanged += (s, e) =>
                {
                    if (cmb.SelectedValue != null)
                    {
                        if (isGlobalSettings)
                            propInfo.SetValue(Config.GlobalParameters.GlobalSettings, cmb.SelectedValue);
                        else
                            propInfo.SetValue(Config.GlobalParameters.Parameters, cmb.SelectedValue);
                    }
                };
            }
        }

        private void SaveSettingsToFile()
        {
            try
            {
                JsonDbManager.SaveParametersToJson();
                MessageBox.Show("Ayarlar başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ayarlar kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
