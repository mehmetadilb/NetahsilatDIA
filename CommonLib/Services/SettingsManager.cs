using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLib
{
    public class SettingsManager
    {
        private GlobalSettings _config;

        public SettingsManager(GlobalSettings config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        // Kontrolleri gezerek GlobalConfig'den değerleri kontrollere yükler
        public void LoadConfigToForm(Control parentControl)
        {
            TraverseControls(parentControl, LoadControlValue);
        }

        // Kontrolleri gezerek kontrol değerlerini GlobalConfig'e kaydeder
        public void SaveConfigFromForm(Control parentControl)
        {
            TraverseControls(parentControl, SaveControlValue);
        }

        // Recursive olarak kontrolleri gezen ana metot
        private void TraverseControls(Control currentControl, Action<Control> processAction)
        {
            // Önce mevcut kontrolü işle
            processAction(currentControl);

            // Sonra alt kontrollerini gez
            foreach (Control childControl in currentControl.Controls)
            {
                TraverseControls(childControl, processAction);
            }
        }

        // Tek bir kontrolün değerini GlobalConfig'den yükler
        private void LoadControlValue(Control ctrl)
        {
            if (ctrl.Tag == null || string.IsNullOrWhiteSpace(ctrl.Tag.ToString()))
                return;

            string propertyName = ctrl.Tag.ToString();
            PropertyInfo propInfo = typeof(GlobalSettings).GetProperty(propertyName);

            if (propInfo == null)
            {
                Console.WriteLine($"Uyarı: GlobalConfig içerisinde '{propertyName}' adında bir property bulunamadı (Kontrol: {ctrl.Name}).");
                return;
            }

            try
            {
                object value = propInfo.GetValue(_config);
                if (value == null) return;

                if (ctrl is TextBox txt)
                {
                    txt.Text = value.ToString();
                }
                else if (ctrl is CheckBox chk)
                {
                    chk.Checked = Convert.ToBoolean(value);
                }
                else if (ctrl is NumericUpDown nud)
                {
                    nud.Value = Convert.ToDecimal(value);
                }
                else if (ctrl is DateTimePicker dtp)
                {
                    dtp.Value = Convert.ToDateTime(value);
                }
                else if (ctrl is ComboBox cmb)
                {
                    // ComboBox için değer atama biraz daha karmaşık olabilir.
                    // Eğer Tag'deki property adı ComboBox'ın metnini tutuyorsa:
                    cmb.Text = value.ToString();
                    // Veya SelectedItem/SelectedValue bazlı ise ona göre ayarlanmalı.
                    // Örneğin: cmb.SelectedItem = value; (eğer value ComboBox item'larından biri ise)
                    // Veya: cmb.SelectedValue = value; (eğer ValueMember ayarlı ise)
                }
                // Diğer kontrol türleri için de benzer şekilde else if blokları eklenebilir
                // else if (ctrl is RadioButton rdo) { ... }
                // else if (ctrl is ListBox lst) { ... }
                else
                {
                    Console.WriteLine($"Uyarı: '{ctrl.GetType().Name}' kontrol türü için yükleme desteği bulunmuyor (Kontrol: {ctrl.Name}).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: '{propertyName}' yüklenirken hata oluştu (Kontrol: {ctrl.Name}). Detay: {ex.Message}");
            }
        }

        // Tek bir kontrolün değerini GlobalConfig'e kaydeder
        private void SaveControlValue(Control ctrl)
        {
            if (ctrl.Tag == null || string.IsNullOrWhiteSpace(ctrl.Tag.ToString()))
                return;

            string propertyName = ctrl.Tag.ToString();
            PropertyInfo propInfo = typeof(GlobalSettings).GetProperty(propertyName);

            if (propInfo == null)
            {
                Console.WriteLine($"Uyarı: GlobalConfig içerisinde '{propertyName}' adında bir property bulunamadı (Kontrol: {ctrl.Name}).");
                return;
            }

            try
            {
                object valueToSet = null;

                if (ctrl is TextBox txt)
                {
                    valueToSet = txt.Text;
                }
                else if (ctrl is CheckBox chk)
                {
                    valueToSet = chk.Checked;
                }
                else if (ctrl is NumericUpDown nud)
                {
                    valueToSet = nud.Value;
                }
                else if (ctrl is DateTimePicker dtp)
                {
                    valueToSet = dtp.Value;
                }
                else if (ctrl is ComboBox cmb)
                {
                    // ComboBox için değer alma şekli kullanımınıza bağlıdır
                    valueToSet = cmb.Text; // Veya cmb.SelectedItem, cmb.SelectedValue
                }
                // Diğer kontrol türleri için de benzer şekilde else if blokları eklenebilir
                else
                {
                    Console.WriteLine($"Uyarı: '{ctrl.GetType().Name}' kontrol türü için kaydetme desteği bulunmuyor (Kontrol: {ctrl.Name}).");
                    return; // Değer atanamayacağı için işlemi sonlandır
                }

                // Property'nin tipine uygun dönüşüm yap
                if (valueToSet != null)
                {
                    object convertedValue = Convert.ChangeType(valueToSet, propInfo.PropertyType);
                    propInfo.SetValue(_config, convertedValue);
                }
                else if (propInfo.PropertyType.IsClass || (Nullable.GetUnderlyingType(propInfo.PropertyType) != null)) // Referans tipleri veya nullable değer tipleri null olabilir
                {
                    propInfo.SetValue(_config, null);
                }
                else // Değer tipleri null olamaz (int, bool, DateTime vs.), bu durumda bir varsayılan değer atanabilir veya hata verilebilir.
                {
                    Console.WriteLine($"Uyarı: '{propertyName}' için '{ctrl.Name}' kontrolünden null değer alındı ancak property null kabul etmiyor.");
                    // Gerekirse: propInfo.SetValue(_config, Activator.CreateInstance(propInfo.PropertyType)); // Varsayılan değer atar
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: '{propertyName}' kaydedilirken hata oluştu (Kontrol: {ctrl.Name}). Detay: {ex.Message}");
            }
        }
    }
}
