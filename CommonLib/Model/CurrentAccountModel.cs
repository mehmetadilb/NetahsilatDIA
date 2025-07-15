using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace CommonLib.Model
{
    public class CurrentAccountModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("_key")]
        public long Id { get; set; }

        /// <summary>
        /// Son İşlem Tarihi
        /// </summary>
        [JsonProperty("_date")]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Oluşturma Tarihi
        /// </summary>
        [JsonProperty("_cdate")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Durum -> Aktif(A), Pasif(P)
        /// </summary>
        [JsonProperty("durum")]
        public string Status { get; set; }

        /// <summary>
        /// Mail Adresi
        /// </summary>
        [JsonProperty("eposta")]
        public string MailAddress { get; set; }

        /// <summary>
        /// Şehir
        /// </summary>
        [JsonProperty("sehir")]
        public string City { get; set; }

        /// <summary>
        /// Cari İlgili Kişi
        /// </summary>
        [JsonProperty("ilgili")]
        public string ContactName { get; set; }

        /// <summary>
        /// TC Kimlik No
        /// </summary>
        [JsonProperty("tckimlikno")]
        public string TCKN { get; set; }

        /// <summary>
        /// Cari Ünvan
        /// </summary>
        [JsonProperty("unvan")]
        public string Title { get; set; }

        /// <summary>
        /// Vergi Numarası
        /// </summary>
        [JsonProperty("verginumarasi")]
        public string TaxNumber { get; set; }

        /// <summary>
        /// Adres: Şehirle birlikte
        /// </summary>
        [JsonProperty("adresbilgi")]
        public string Address { get; set; }

        /// <summary>
        /// Bağlı Firmanın Adı
        /// </summary>
        [JsonProperty("firmaadi")]
        public string FirmName { get; set; }

        /// <summary>
        /// İlçe
        /// </summary>
        [JsonProperty("ilce")]
        public string District { get; set; }

        /// <summary>
        /// Cep Telefon Numarası
        /// </summary>
        [JsonProperty("ceptel")]
        public string MobilePhoneNumber { get; set; }
        /// <summary>
        /// Telefon Numarası 1
        /// </summary>
        [JsonProperty("telefon1")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Telefon Numarası 2
        /// </summary>
        [JsonProperty("telefon2")]
        public string PhoneNumber2 { get; set; }

        /// <summary>
        /// Vergi Dairesi
        /// </summary>
        [JsonProperty("vergidairesi")]
        public string TaxOffice { get; set; }

        /// <summary>
        /// Cari Kodu
        /// </summary>
        [JsonProperty("carikartkodu")]
        public string Code { get; set; }

        /// <summary>
        /// Ek Alan 10 - 'NTE Aktar' değeri bekleniyor.
        /// </summary>
        [JsonProperty("ekalan10")]
        public string AdditionField10 { get; set; }

        /// <summary>
        /// Ülke
        /// </summary>
        [JsonProperty("ulke")]
        public string Country { get; set; }

        /// <summary>
        /// Ana Adres Id
        /// </summary>
        [JsonProperty("anaadreskey")]
        public long MainAddressKey { get; set; }

        /// <summary>
        /// Firma No
        /// </summary>
        [JsonProperty("_level1")]
        public long FirmNumber { get; set; }

        /// <summary>
        /// Periyot
        /// </summary>
        [JsonProperty("_level2")]
        public long PeriodNumber { get; set; }

        /// <summary>
        /// Aktarım Durumu
        /// </summary>
        public bool IntegrationStatus { get; set; }

        /// <summary>
        /// NTE Aktar Dinamik Alanı
        /// </summary>
        [JsonProperty("__dinamik__nteaktar")]
        public string NteAktar { get; set; }
    }
}
