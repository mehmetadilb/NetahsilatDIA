using CommonLib;
using CommonLib.Enum;
using CommonLib.Model;
using Netahsilat.DIAService;
using Netahsilat.DIAService.Model;
using NetahsilatWebServiceLib.Accounts;
using NetahsilatWebServiceLib.ErpWebService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetahsilatWebServiceLib.Common
{
    /// <summary>
    /// Payment'lar için ortak verileri toplu olarak sorgulayarak token tasarrufu sağlar
    /// </summary>
    public class BatchDataManager
    {
        private readonly Dictionary<string, CurrentAccountModel> _customerCache = new Dictionary<string, CurrentAccountModel>();
        private readonly Dictionary<string, long> _bankAccountCache = new Dictionary<string, long>();
        private readonly Dictionary<string, long> _dynamicFieldCache = new Dictionary<string, long>();
        private dynamic _cachedFirmInfo;
        private List<DiaExchangeModel> _cachedExchangeRates;
        

        
        private DateTime _longCacheLastUpdate = DateTime.MinValue;
        private DateTime _customerCacheLastUpdate = DateTime.MinValue;
        private DateTime _bankAccountCacheLastUpdate = DateTime.MinValue;
        private DateTime _dynamicFieldCacheLastUpdate = DateTime.MinValue;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(12);
        private readonly TimeSpan _shortCacheExpiration = TimeSpan.FromHours(1); // Kısa süreli cache'ler için (1 saat)

        public BatchDataManager()
        {
        }

        /// <summary>
        /// Tüm payment'lar için gerekli ortak verileri toplu olarak yükler
        /// </summary>
        public async Task PreloadBatchDataAsync(List<PaymentServiceModel> payments, VirtualPosListResult posList)
        {
            Logging.AddLog($"Batch veri yükleme başlıyor. {payments.Count} payment için ortak veriler sorgulanacak.");

            var tasks = new List<Task>();

            // Payment'lardan gelen customer kodlarını topla (eşleştirme kontrolü için)
            var customerCodes = payments
                .SelectMany(p => GetCustomerCodes(p))
                .Where(code => !string.IsNullOrEmpty(code))
                .Distinct()
                .ToList();

            // Opponent customer kodlarını topla (virman işlemleri için)
            var opponentCustomerCodes = payments
                .Where(p => p.BankDetails?.BankVPosCode != null && !string.IsNullOrEmpty(p.BankDetails.BankVPosCode))
                .Select(p => p.BankDetails.BankVPosCode)
                .Distinct()
                .ToList();

            // Torba hesap kodunu ekle (üyeliksiz işlemler için)
            if (!string.IsNullOrEmpty(Config.GlobalParameters.GlobalSettings.NONCUSTOMER_BAGACCOUNT) && 
                !customerCodes.Contains(Config.GlobalParameters.GlobalSettings.NONCUSTOMER_BAGACCOUNT) &&
                !opponentCustomerCodes.Contains(Config.GlobalParameters.GlobalSettings.NONCUSTOMER_BAGACCOUNT))
            {
                customerCodes.Add(Config.GlobalParameters.GlobalSettings.NONCUSTOMER_BAGACCOUNT);
            }

            // Tüm customer kodlarını birleştir (eşleştirme kontrolü için)
            customerCodes.AddRange(opponentCustomerCodes);
            customerCodes = customerCodes.Distinct().ToList();

            // BankAccount kodlarını topla
            var bankAccountCodes = payments
                .Where(p => !string.IsNullOrEmpty(p.VPosERPCode))
                .Select(p => p.VPosERPCode)
                .Distinct()
                .ToList();

            // Dinamik alan değerlerini endpoint'lerine göre grupla (tekrar alınmaması için kontrol)
            var salesmanCodes = new List<string>();
            var authCodes = new List<string>();
            var projectCodes = new List<string>();

            foreach (var payment in payments)
            {
                var dynamicFields = DynamicValuesHelper.GetAllDynamicFieldValues(payment);
                
                if (!string.IsNullOrEmpty(dynamicFields.Salesman) && 
                    !_dynamicFieldCache.ContainsKey(dynamicFields.Salesman) && 
                    !salesmanCodes.Contains(dynamicFields.Salesman))
                    salesmanCodes.Add(dynamicFields.Salesman);
                    
                if (!string.IsNullOrEmpty(dynamicFields.AuthCode) && 
                    !_dynamicFieldCache.ContainsKey(dynamicFields.AuthCode) && 
                    !authCodes.Contains(dynamicFields.AuthCode))
                    authCodes.Add(dynamicFields.AuthCode);
                    
                if (!string.IsNullOrEmpty(dynamicFields.ProjectCode) && 
                    !_dynamicFieldCache.ContainsKey(dynamicFields.ProjectCode) && 
                    !projectCodes.Contains(dynamicFields.ProjectCode))
                    projectCodes.Add(dynamicFields.ProjectCode);
            }

            // Tüm aktif cari hesapları yükle (1 saat cache ile)
            tasks.Add(LoadCustomersAsync(customerCodes));

            // Tüm aktif banka hesaplarını yükle (1 saat cache ile)
            tasks.Add(LoadBankAccountsAsync(bankAccountCodes));

            // Tüm aktif dinamik alanları yükle (1 saat cache ile)
            tasks.Add(LoadDynamicFieldsByEndpointAsync(salesmanCodes, DiaEndPoints.Keys.SALESMAN));
            tasks.Add(LoadDynamicFieldsByEndpointAsync(authCodes, DiaEndPoints.Keys.AUTHKEY));
            tasks.Add(LoadDynamicFieldsByEndpointAsync(projectCodes, DiaEndPoints.Keys.PROJECTCODE));

            tasks.Add(LoadCommonDataAsync());

            await Task.WhenAll(tasks);

            Logging.AddLog($"Batch veri yükleme tamamlandı. Cache durumu: Customer={_customerCache.Count}, BankAccount={_bankAccountCache.Count}, DynamicFields={_dynamicFieldCache.Count}");
        }

        /// <summary>
        /// Ortak verileri (firma bilgisi, döviz kuru) yükler
        /// </summary>
        private async Task LoadCommonDataAsync()
        {
            try
            {
                Logging.AddLog("Ortak veriler yükleniyor (firma bilgisi, döviz kuru)...");

                // Firma bilgisi yükle
                if (DateTime.Now - _longCacheLastUpdate > _cacheExpiration)
                {
                    var firmInfoResponse = DIARepository.Get(DiaEndPoints.Keys.COMPANY);
                    if (firmInfoResponse != null)
                    {
                        _cachedFirmInfo = firmInfoResponse;
                        _longCacheLastUpdate = DateTime.Now;
                        Logging.AddLog("Firma bilgisi cache'e eklendi.");
                    }
                }

                // Döviz kuru yükle (bugünün tarihi için)
                if (DateTime.Now - _longCacheLastUpdate > _cacheExpiration)
                {
                    var exchangeParams = new BaseApiRequestParams()
                        .AddFilter(DateTime.Now.ToString("yyyy-MM-dd"), "tarih", FilterTypes.EQUAL)
                        .AddSort("_cdate", SortTypes.DESC);

                    var exchangeResponse = DIARepository.List(DiaEndPoints.Keys.EXCHANGERATES, exchangeParams);
                    if (exchangeResponse != null)
                    {
                        var json = JsonConvert.SerializeObject(exchangeResponse);
                        List<DiaExchangeModel> exchangeRatesAll = JsonConvert.DeserializeObject<List<DiaExchangeModel>>(json);
                        
                        // CurrencyId'ye göre grupla ve her gruptan en son tarihli olanı al
                        _cachedExchangeRates = (exchangeRatesAll ?? new List<DiaExchangeModel>())
                            .GroupBy(x => x.CurrencyId)
                            .Select(g => g.OrderByDescending(x => x.Date).First())
                            .ToList();
                        
                        _longCacheLastUpdate = DateTime.Now;
                        Logging.AddLog($"Döviz kuru cache'e eklendi. {_cachedExchangeRates?.Count ?? 0} adet kur bulundu.");
                    }
                }

                Logging.AddLog("Ortak veriler yükleme tamamlandı.");
            }
            catch (Exception ex)
            {
                Logging.AddLog($"Ortak veriler yükleme hatası: {ex.Message}");
            }
        }

        private List<string> GetCustomerCodes(PaymentServiceModel payment)
        {
            var codes = new List<string>();

            if (Config.GlobalParameters.GlobalSettings.ACCOUNT_ERPCODE_FIELD != (int)PaymentCustomerFirstFieldType.Default)
            {
                switch ((PaymentCustomerFirstFieldType)Config.GlobalParameters.GlobalSettings.ACCOUNT_ERPCODE_FIELD)
                {
                    case PaymentCustomerFirstFieldType.AccountErpCode:
                        if (!string.IsNullOrWhiteSpace(payment.AccountErpCode))
                            codes.Add(payment.AccountErpCode.Length > 25 ? payment.AccountErpCode.Substring(0, 25) : payment.AccountErpCode);
                        break;
                    case PaymentCustomerFirstFieldType.AgentErpCode:
                        if (!string.IsNullOrWhiteSpace(payment.Agent.ErpCode))
                            codes.Add(payment.Agent.ErpCode.Length > 25 ? payment.Agent.ErpCode.Substring(0, 25) : payment.Agent.ErpCode);
                        break;
                    case PaymentCustomerFirstFieldType.AgentCode:
                        if (!string.IsNullOrWhiteSpace(payment.Agent.Code))
                            codes.Add(payment.Agent.Code.Length > 25 ? payment.Agent.Code.Substring(0, 25) : payment.Agent.Code);
                        break;
                }
            }
            else
            {
                // Varsayılan sıralama
                if (!string.IsNullOrWhiteSpace(payment.AccountErpCode))
                    codes.Add(payment.AccountErpCode.Length > 25 ? payment.AccountErpCode.Substring(0, 25) : payment.AccountErpCode);
                if (!string.IsNullOrWhiteSpace(payment.Agent.ErpCode))
                    codes.Add(payment.Agent.ErpCode.Length > 25 ? payment.Agent.ErpCode.Substring(0, 25) : payment.Agent.ErpCode);
                if (!string.IsNullOrWhiteSpace(payment.Agent.Code))
                    codes.Add(payment.Agent.Code.Length > 25 ? payment.Agent.Code.Substring(0, 25) : payment.Agent.Code);
            }

            return codes;
        }

        private async Task LoadCustomersAsync(List<string> customerCodes)
        {
            try
            {
                // Cache süresi kontrolü - 1 saat geçtiyse yeniden çek
                if (DateTime.Now - _customerCacheLastUpdate <= _shortCacheExpiration)
                {
                    Logging.AddLog("Customer cache hala geçerli, yeniden yükleme yapılmayacak.");
                    return;
                }

                // Sadece aktif (durumu A olan) cari hesapları çek
                var _params = new BaseApiRequestParams()
                    .AddFilter("A", "durumu", FilterTypes.EQUAL)
                    .AddSort("_key", SortTypes.DESC);

                var response = DIARepository.List(DiaEndPoints.Keys.CURRENTACCOUNT, _params);

                if (response != null)
                {
                    var json = JsonConvert.SerializeObject(response);
                    var allCustomers = JsonConvert.DeserializeObject<List<CurrentAccountModel>>(json);

                    if (allCustomers != null)
                    {
                        // Cache'i temizle ve tüm aktif cari hesapları ekle
                        _customerCache.Clear();
                        
                        foreach (var customer in allCustomers)
                        {
                            if (!string.IsNullOrEmpty(customer.Code))
                            {
                                _customerCache[customer.Code] = customer;
                            }
                        }

                        _customerCacheLastUpdate = DateTime.Now;
                        
                        Logging.AddLog($"Tüm aktif cari hesaplar yüklendi. {allCustomers.Count} adet aktif cari hesap cache'e eklendi.");
                        
                        // Payment'lardan gelen kodlarla eşleştirme kontrolü
                        if (customerCodes.Any())
                        {
                            var foundCount = 0;
                            var notFoundCodes = new List<string>();
                            
                            foreach (var code in customerCodes)
                            {
                                if (_customerCache.ContainsKey(code))
                                {
                                    foundCount++;
                                }
                                else
                                {
                                    notFoundCodes.Add(code);
                                }
                            }
                            
                            Logging.AddLog($"Cari hesap kodları eşleştirme sonucu: {foundCount}/{customerCodes.Count} kod bulundu.");
                            if (notFoundCodes.Any())
                            {
                                Logging.AddLog($"Bulunamayan kodlar: {string.Join(", ", notFoundCodes)}");
                            }
                        }
                    }
                    else
                    {
                        Logging.AddLog("Aktif cari hesap sorgulamasında sonuç alınamadı.");
                    }
                }
                else
                {
                    Logging.AddLog("Aktif cari hesap sorgulamasında null response alındı.");
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"Customer batch yükleme hatası: {ex.Message}");
            }
        }

        private async Task LoadBankAccountsAsync(List<string> bankAccountCodes)
        {
            try
            {
                // Cache süresi kontrolü - 1 saat geçtiyse yeniden çek
                if (DateTime.Now - _bankAccountCacheLastUpdate <= _shortCacheExpiration)
                {
                    Logging.AddLog("Bank account cache hala geçerli, yeniden yükleme yapılmayacak.");
                    return;
                }

                // Sadece aktif banka hesaplarını çek (durumu A olan)
                var _params = new BaseApiRequestParams()
                    .AddFilter("A", "durumu", FilterTypes.EQUAL)
                    .AddSort("_key", SortTypes.DESC);

                var response = DIARepository.List(DiaEndPoints.Keys.BANKACCOUNT, _params);

                if (response != null)
                {
                    var json = JsonConvert.SerializeObject(response);
                    var allBankAccounts = JsonConvert.DeserializeObject<List<dynamic>>(json);

                    if (allBankAccounts != null)
                    {
                        // Cache'i temizle ve tüm aktif banka hesaplarını ekle
                        _bankAccountCache.Clear();
                        
                        foreach (var bankAccount in allBankAccounts)
                        {
                            var code = bankAccount?.hesapkodu?.ToString();
                            var key = bankAccount?._key?.ToString();
                            
                            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(key))
                            {
                                long keyValue;
                                if (long.TryParse(key, out keyValue))
                                {
                                    _bankAccountCache[code] = keyValue;
                                }
                            }
                        }

                        _bankAccountCacheLastUpdate = DateTime.Now;
                        
                        Logging.AddLog($"Tüm aktif banka hesapları yüklendi. {allBankAccounts.Count} adet aktif banka hesabı cache'e eklendi.");
                        
                        // Payment'lardan gelen kodlarla eşleştirme kontrolü
                        if (bankAccountCodes.Any())
                        {
                            var foundCount = 0;
                            var notFoundCodes = new List<string>();
                            
                            foreach (var code in bankAccountCodes)
                            {
                                if (_bankAccountCache.ContainsKey(code))
                                {
                                    foundCount++;
                                }
                                else
                                {
                                    notFoundCodes.Add(code);
                                }
                            }
                            
                            Logging.AddLog($"Banka hesap kodları eşleştirme sonucu: {foundCount}/{bankAccountCodes.Count} kod bulundu.");
                            if (notFoundCodes.Any())
                            {
                                Logging.AddLog($"Bulunamayan banka hesap kodları: {string.Join(", ", notFoundCodes)}");
                            }
                        }
                    }
                    else
                    {
                        Logging.AddLog("Aktif banka hesap sorgulamasında sonuç alınamadı.");
                    }
                }
                else
                {
                    Logging.AddLog("Aktif banka hesap sorgulamasında null response alındı.");
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"BankAccount batch yükleme hatası: {ex.Message}");
            }
        }





        /// <summary>
        /// Belirtilen endpoint'te tüm aktif dinamik alanları yükler
        /// </summary>
        private async Task LoadDynamicFieldsByEndpointAsync(List<string> codes, string endpoint)
        {
            try
            {
                // Cache süresi kontrolü - 1 saat geçtiyse yeniden çek
                if (DateTime.Now - _dynamicFieldCacheLastUpdate <= _shortCacheExpiration)
                {
                    Logging.AddLog($"Dynamic field cache hala geçerli ({endpoint}), yeniden yükleme yapılmayacak.");
                    return;
                }

                Logging.AddLog($"Tüm aktif dinamik alanlar yükleniyor ({endpoint})...");

                // Sadece aktif dinamik alanları çek (durumu A olan)
                var _params = new BaseApiRequestParams()
                    .AddFilter("A", "durumu", FilterTypes.EQUAL)
                    .AddSort("_key", SortTypes.DESC);

                var response = DIARepository.List(endpoint, _params);

                if (response != null)
                {
                    var json = JsonConvert.SerializeObject(response);
                    var allDynamicFields = JsonConvert.DeserializeObject<List<dynamic>>(json);

                    if (allDynamicFields != null)
                    {
                        // Cache'i temizle ve tüm aktif dinamik alanları ekle
                        _dynamicFieldCache.Clear();
                        
                        foreach (var dynamicField in allDynamicFields)
                        {
                            var code = dynamicField?.kodu?.ToString();
                            var key = dynamicField?._key?.ToString();
                            
                            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(key))
                            {
                                long keyValue;
                                if (long.TryParse(key, out keyValue))
                                {
                                    _dynamicFieldCache[code] = keyValue;
                                }
                            }
                        }

                        _dynamicFieldCacheLastUpdate = DateTime.Now;
                        
                        Logging.AddLog($"Tüm aktif dinamik alanlar yüklendi ({endpoint}). {allDynamicFields.Count} adet aktif dinamik alan cache'e eklendi.");
                        
                        // Payment'lardan gelen kodlarla eşleştirme kontrolü
                        if (codes.Any())
                        {
                            var foundCount = 0;
                            var notFoundCodes = new List<string>();
                            
                            foreach (var code in codes)
                            {
                                if (_dynamicFieldCache.ContainsKey(code))
                                {
                                    foundCount++;
                                }
                                else
                                {
                                    notFoundCodes.Add(code);
                                }
                            }
                            
                            Logging.AddLog($"Dinamik alan kodları eşleştirme sonucu ({endpoint}): {foundCount}/{codes.Count} kod bulundu.");
                            if (notFoundCodes.Any())
                            {
                                Logging.AddLog($"Bulunamayan dinamik alan kodları ({endpoint}): {string.Join(", ", notFoundCodes)}");
                            }
                        }
                    }
                    else
                    {
                        Logging.AddLog($"Aktif dinamik alan sorgulamasında sonuç alınamadı ({endpoint}).");
                    }
                }
                else
                {
                    Logging.AddLog($"Aktif dinamik alan sorgulamasında null response alındı ({endpoint}).");
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"Dynamic field endpoint yükleme hatası ({endpoint}): {ex.Message}");
            }
        }



        // Cache'den veri alma metodları
        public CurrentAccountModel GetCustomer(string accountErpCode, Agent agent)
        {
            var codes = GetCustomerCodes(new PaymentServiceModel { AccountErpCode = accountErpCode, Agent = agent });
            
            foreach (var code in codes)
            {
                if (_customerCache.TryGetValue(code, out var customer))
                    return customer;
            }
            
            return null;
        }

        /// <summary>
        /// Sadece kod ile customer arar (opponent customer için)
        /// </summary>
        public CurrentAccountModel GetCustomerByCode(string customerCode)
        {
            if (string.IsNullOrEmpty(customerCode))
                return null;
                
            return _customerCache.TryGetValue(customerCode, out var customer) ? customer : null;
        }

        public long? GetBankAccount(string bankAccountCode)
        {
            return _bankAccountCache.TryGetValue(bankAccountCode, out var bankAccount) ? (long?)bankAccount : null;
        }

        /// <summary>
        /// Dinamik alan değerini cache'den alır
        /// </summary>
        public long? GetDynamicField(string fieldCode)
        {
            return _dynamicFieldCache.TryGetValue(fieldCode, out var value) ? (long?)value : null;
        }

        /// <summary>
        /// Geriye uyumluluk için eski metodlar
        /// </summary>
        public long? GetSalesman(string salesmanCode)
        {
            return GetDynamicField(salesmanCode);
        }

        public long? GetAuthCode(string authCode)
        {
            return GetDynamicField(authCode);
        }

        public long? GetProjectCode(string projectCode)
        {
            return GetDynamicField(projectCode);
        }



        /// <summary>
        /// Cache'den firma bilgisi alır
        /// </summary>
        public dynamic GetFirmInfo()
        {
            return _cachedFirmInfo;
        }

        /// <summary>
        /// Cache'den döviz kurlarını alır
        /// </summary>
        public List<DiaExchangeModel> GetExchangeRates()
        {
            return _cachedExchangeRates;
        }

        /// <summary>
        /// Cache'den tüm cari hesapları alır
        /// </summary>
        public List<CurrentAccountModel> GetCurrentAccounts()
        {
            return _customerCache.Values.ToList();
        }



        public void ClearCache()
        {
            _customerCache.Clear();
            _bankAccountCache.Clear();
            _dynamicFieldCache.Clear();
            
            // Cache'leri temizle
            _cachedFirmInfo = null;
            _cachedExchangeRates = null;
            _longCacheLastUpdate = DateTime.MinValue;
            _customerCacheLastUpdate = DateTime.MinValue;
            _bankAccountCacheLastUpdate = DateTime.MinValue;
            _dynamicFieldCacheLastUpdate = DateTime.MinValue;
        }
    }
} 