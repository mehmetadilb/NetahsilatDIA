using CommonLib;
using CommonLib.Enum;
using CommonLib.Model;
using Netahsilat.DIAService;
using Netahsilat.DIAService.Model;
using NetahsilatWebServiceLib.CurrentAccountTransactionService;
using NetahsilatWebServiceLib.Mappers;
using NetahsilatWebServiceLib.VendorWebService;
using NetahsilatWebServiceLib.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace NetahsilatWebServiceLib.Accounts
{
    public class AccountManager
    {
        //private readonly DIARepository _DIARepository;
        private readonly BasicHttpBinding_ICurrentAccountTransaction _catService;
        private readonly IVendorWebService _vendorWebService;
        private readonly ErpWebService.AuthenticationInfo _authenticationInfo;
        private readonly CurrentAccountTransactionService.AuthenticationInfo _catServiceAuthenticationInfo;
        private readonly VendorWebService.AuthenticationInfo _vendorServiceAuthenticationInfo;

        // BatchDataManager referansı
        private readonly BatchDataManager _batchDataManager;
        private HashSet<string> _cachedTransactionKeys = null;



        public AccountManager(BatchDataManager batchDataManager = null)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;

                // BatchDataManager referansını al (null olabilir)
                _batchDataManager = batchDataManager;

                if (_catService == null)
                {
                    _catService = new BasicHttpBinding_ICurrentAccountTransaction();
                }
                if(_vendorWebService == null)
                {
                    var vendorService = new VendorService().ConnectVendorService(Config.GlobalParameters.Parameters.VENDOR_SERVICE, Config.GlobalParameters.Parameters.WEB_SERVICE_UID, Config.GlobalParameters.Parameters.WEB_SERVICE_PWD);

                    _vendorWebService = vendorService.Client;
                    _vendorServiceAuthenticationInfo = vendorService.AuthInfo;
                }

                if (!String.IsNullOrWhiteSpace(Config.GlobalParameters.Parameters.ACCOUNT_SERVICE))
                {
                    _catService.Url = Config.GlobalParameters.Parameters.ACCOUNT_SERVICE;

                    _catServiceAuthenticationInfo = new CurrentAccountTransactionService.AuthenticationInfo
                    {
                        UserName = Config.GlobalParameters.Parameters.WEB_SERVICE_UID,
                        Password = Config.GlobalParameters.Parameters.WEB_SERVICE_PWD
                    };
                }

                if (!String.IsNullOrWhiteSpace(Config.GlobalParameters.Parameters.VENDOR_SERVICE))
                {
                    _vendorServiceAuthenticationInfo = new VendorWebService.AuthenticationInfo
                    {
                        UserName = Config.GlobalParameters.Parameters.WEB_SERVICE_UID,
                        Password = Config.GlobalParameters.Parameters.WEB_SERVICE_PWD
                    };
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog(ex.Message);
            }
        }

        public void SendAccountTransactionDaily()
        {
            try
            {
                // Local AccountTransactions.json okuma (SyncData ve eski format destekli)
                List<AccountTransaction> existingTransactions = new List<AccountTransaction>();
                try
                {
                    var syncJson = File.Exists("AccountTransactions.json") ? File.ReadAllText("AccountTransactions.json") : null;
                    if (!string.IsNullOrEmpty(syncJson))
                    {
                        if (syncJson.TrimStart().StartsWith("{"))
                        {
                            var loadedSyncData = JsonConvert.DeserializeObject<SyncData<AccountTransaction>>(syncJson);
                            if (loadedSyncData != null)
                                existingTransactions = loadedSyncData.Data ?? new List<AccountTransaction>();
                        }
                        else
                        {
                            existingTransactions = JsonConvert.DeserializeObject<List<AccountTransaction>>(syncJson) ?? new List<AccountTransaction>();
                        }
                    }
                }
                catch
                {
                    existingTransactions = new List<AccountTransaction>();
                }

                Logging.AddLog("Daha önce aktarımı başarısız olanlar ve güncellenmiş olan cari hesap hareketleri aktarılacak");

                var myParamList = new List<CATCreateOrUpdateParameters>();
                var currentAccountTransactionAll = new List<CurrentAccountTransactionModel>();

                Logging.AddLog("Cari hesap hareketleri API'den yükleniyor.");
                int limit = 200;
                int offset = 0;
                bool hasMore = true;

                while (hasMore)
                {
                    var _params = new BaseApiRequestParams()
                        .AddFilter(ConfigHelper.DiaFirmaKodu.ToString(),"level1",FilterTypes.EQUAL)
                        .AddFilter(ConfigHelper.DiaDonemKodu.ToString(), "level2", FilterTypes.EQUAL)
                        .Limit(limit)
                        .Offset(offset);

                    var diaResult = DIARepository.List(DiaEndPoints.Keys.CURRENTACCOUNTFICHE, _params, DiaEndPoints.Suffixes.DETAILED_LIST);

                    if (diaResult == null)
                        break;

                    var json = JsonConvert.SerializeObject(diaResult);
                    var batchList = JsonConvert.DeserializeObject<List<CurrentAccountTransactionModel>>(json);

                    if (batchList == null || batchList.Count == 0)
                    {
                        hasMore = false;
                        break;
                    }

                    currentAccountTransactionAll.AddRange(batchList);

                    if (batchList.Count < limit)
                        hasMore = false;
                    else
                        offset += limit;
                }

                _cachedTransactionKeys = currentAccountTransactionAll?.Select(x => x.Key).ToHashSet() ?? new HashSet<string>();

                if (currentAccountTransactionAll == null || currentAccountTransactionAll.Count == 0)
                {
                    Logging.AddLog("API'den gelen cari hesap hareketi bulunamadı.");
                    return;
                }

                var localDict = existingTransactions
                    .Where(x => x.Firm == ConfigHelper.DiaFirmaKodu && x.Period == ConfigHelper.DiaDonemKodu && x.Deleted == false)
                    .ToDictionary(x => x.TransId);
                var filteredList = new List<CurrentAccountTransactionModel>();

                foreach (var apiRecord in currentAccountTransactionAll)
                {
                    if (localDict.TryGetValue((int.Parse(apiRecord.Key)), out AccountTransaction localRecord))
                    {
                        if (localRecord.Status == false)
                        {
                            filteredList.Add(apiRecord);
                        }
                        else if (localRecord.Status == true)
                        {
                            if (apiRecord.Date > localRecord.RecordDate)
                            {
                                filteredList.Add(apiRecord);
                            }
                        }
                    }
                    else
                    {
                        filteredList.Add(apiRecord);
                    }
                }

                if (filteredList.Count == 0)
                {
                    Logging.AddLog("Aktarılacak başarısız cari hareket bulunamadı.");
                    return;
                }

                foreach (var item in filteredList)
                {
                    var mapped = AccountTransactionMapper.Map(item);
                    if (mapped != null)
                    {
                        myParamList.Add(mapped);
                    }
                }

                Logging.AddLog($"Günlük aktarılacak extre hareket sayısı: {myParamList.Count}");

                if (myParamList.Count > 0)
                {
                    Logging.AddLog($"Gönderilecek hareket sayısı: {myParamList.Count}");
                    SendAccountTrans(myParamList);
                    Logging.AddLog("Hareket aktarımı tamamlandı.");
                }
                DeletedTrans();
            }
            catch (Exception ex)
            {
                Logging.AddLog($"SendAccountTransactionDaily - Hata : {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        private SyncData<T> ReadSyncData<T>(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var syncJson = File.ReadAllText(path);
                    if (!string.IsNullOrEmpty(syncJson))
                    {
                        if (syncJson.TrimStart().StartsWith("{"))
                        {
                            var loadedSyncData = JsonConvert.DeserializeObject<SyncData<T>>(syncJson);
                            if (loadedSyncData != null)
                                return loadedSyncData;
                        }
                        else
                        {
                            var data = JsonConvert.DeserializeObject<List<T>>(syncJson) ?? new List<T>();
                            return new SyncData<T> { LastSync = DateTime.MinValue, Data = data };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"ReadSyncData error: {ex.Message}");
            }
            return new SyncData<T> { LastSync = DateTime.MinValue, Data = new List<T>() };
        }

        private void WriteSyncData<T>(string path, SyncData<T> data)
        {
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented));
                Logging.AddLog($"Toplam kayıt sayısı: {data.Data.Count}");
            }
            catch (Exception ex)
            {
                Logging.AddLog($"WriteSyncData error: {ex.Message}");
            }
        }

        public void SendAccountTransaction(string accountCode = "", bool isManuel = false)
        {
            Logging.AddLog("Cari hesap hareketleri aktarılacak");
            try
            {
                var syncData = ReadSyncData<AccountTransaction>("AccountTransactions.json");
                DateTime lastSync = syncData.LastSync;
                List<AccountTransaction> existingTransactions = syncData.Data;

                var myParamList = new List<CATCreateOrUpdateParameters>();
                var currentAccountTransactionAll = new List<CurrentAccountTransactionModel>();

                Logging.AddLog("Cari hesap hareketleri API'den yükleniyor.");

                int limit = 200;
                int offset = 0;
                bool hasMore = true;

                while (hasMore)
                {
                    var _params = new BaseApiRequestParams()
                        .AddFilter(ConfigHelper.DiaFirmaKodu.ToString(),"level1",FilterTypes.EQUAL)
                        .AddFilter(ConfigHelper.DiaDonemKodu.ToString(), "level2", FilterTypes.EQUAL)
                        .Limit(limit)
                        .Offset(offset);
                    if (!string.IsNullOrEmpty(accountCode))
                    {
                        _params.AddFilter(accountCode, "carikartkodu", FilterTypes.EQUAL);
                    }
                    else if (lastSync > DateTime.MinValue)
                    {
                        _params.AddFilter(lastSync.ToString("yyyy-MM-dd"), "_date", FilterTypes.GREATER_THEN_OR_EQUAL);
                    }

                    var diaResult = DIARepository.List(DiaEndPoints.Keys.CURRENTACCOUNTFICHE, _params, DiaEndPoints.Suffixes.DETAILED_LIST);

                    if (diaResult == null)
                        break;

                    var json = JsonConvert.SerializeObject(diaResult);
                    var batchList = JsonConvert.DeserializeObject<List<CurrentAccountTransactionModel>>(json);

                    if (batchList == null || batchList.Count == 0)
                    {
                        hasMore = false;
                        break;
                    }

                    currentAccountTransactionAll.AddRange(batchList);

                    if (batchList.Count < limit)
                        hasMore = false;
                    else
                        offset += limit;
                }

                _cachedTransactionKeys = currentAccountTransactionAll?.Select(x => x.Key).ToHashSet() ?? new HashSet<string>();

                if (currentAccountTransactionAll == null || currentAccountTransactionAll.Count == 0)
                {
                    Logging.AddLog("Yeni cari hesap hareketi bulunamadı.");
                    return;
                }

                var localCurrentAccountTransactions = existingTransactions
                    ?.Where(x => x.Firm == ConfigHelper.DiaFirmaKodu && x.Period == ConfigHelper.DiaDonemKodu && x.Deleted == false)
                    .ToList() ?? new List<AccountTransaction>();

                var localDict = localCurrentAccountTransactions.ToDictionary(x => x.TransId);

                var newOrUpdatedTransactions = currentAccountTransactionAll
                    .Where(x => !localDict.ContainsKey(int.Parse(x.Key)) ||
                                (localDict[int.Parse(x.Key)].Status == true && x.Date > localDict[int.Parse(x.Key)].RecordDate))
                    .ToList();

                Logging.AddLog($"Yeni bulunan cari hareket sayısı: {newOrUpdatedTransactions.Count}");

                foreach (var item in newOrUpdatedTransactions)
                {
                    var mapped = AccountTransactionMapper.Map(item);
                    if (mapped != null)
                    {
                        myParamList.Add(mapped);
                    }
                }

                if (myParamList.Count > 0)
                {
                    Logging.AddLog($"Gönderilecek extre hareket sayısı: {myParamList.Count}");
                    SendAccountTrans(myParamList);
                    Logging.AddLog("Extre aktarımı tamamlandı.");
                }
                else
                {
                    Logging.AddLog("Gönderilecek yeni cari hareket bulunamadı.");
                }

                DeletedTrans();
            }
            catch (Exception ex)
            {
                Logging.AddLog($"SendAccountTransaction - Hata : {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        private void SendOrderDebtInfoMail(List<string> orderSendMailErpCodeList)
        {
            CATCreateOrUpdateResult[] result = _catService.SendCATDebtInfoMail(_catServiceAuthenticationInfo, orderSendMailErpCodeList.ToArray())?.CurrentAccountTransactionList;
            if (result != null && result.Length > 0)
            {
                Logging.AddLog("Sipariş Ödeme Mail Gönderimi Başladı");
                foreach (var item in result)
                {
                    if (item.Result.Status == ExecuteStatus.Success)
                        Logging.AddLog(string.Format("{0} Erp Kodlu Siparişe ait ödeme bildirim maili gönderildi.", item.ErpCode));
                    else
                        Logging.AddLog(string.Format("{0} Erp Kodlu Siparişe ait ödeme bildirim maili gönderilemedi. Hata : {1}", item.ErpCode, item.Result.Message));
                }
                Logging.AddLog("Sipariş Ödeme Mail Gönderimi Bitti");
            }
        }

        private void SendAccountTrans(List<CATCreateOrUpdateParameters> sendParamList)
        {
            Logging.AddLog($"Aktarılacak hareket sayısı: {sendParamList.Count}");
            Logging.AddLog($"Aktarım Servis URL: {_catService.Url}");

            var tempSendParamList = new List<CATCreateOrUpdateParameters>();
            var allTransactions = new List<AccountTransaction>(); // tüm kayıtları burada topla

            foreach (var sendParam in sendParamList)
            {
                try
                {
                    tempSendParamList.Add(sendParam);

                    var result = _catService.CreateOrUpdate(_catServiceAuthenticationInfo, tempSendParamList.ToArray())?.CurrentAccountTransactionList?.FirstOrDefault();

                    var catParam = sendParamList.FirstOrDefault(x => x.ErpCode == result.ErpCode);

                    bool status = result?.Result?.Status == ExecuteStatus.Success;
                    string logMessage = status ? "aktarıldı" : $"aktarılamadı. Hata: {result?.Result?.Message}";

                    var transaction = SetAccountTransaction(catParam, status, false);
                    if (transaction != null)
                        allTransactions.Add(transaction);

                    Logging.AddLog($"{result.ErpCode} erp kodlu hareket {logMessage}.");
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"Hesap ekstre aktarımında hata alındı. Erp Kod: {sendParam.ErpCode} Hata: {ex.Message} - (SendAccountTrans)");
                }
                finally
                {
                    tempSendParamList.Clear();
                }
            }

            string jsonPath = "AccountTransactions.json";
            List<AccountTransaction> existingTransactions = new List<AccountTransaction>();
            DateTime lastSync = DateTime.MinValue;

            if (File.Exists(jsonPath))
            {
                var existingJson = File.ReadAllText(jsonPath);
                if (!string.IsNullOrEmpty(existingJson) && existingJson.TrimStart().StartsWith("{"))
                {
                    var syncData = JsonConvert.DeserializeObject<SyncData<AccountTransaction>>(existingJson);
                    if (syncData != null)
                    {
                        lastSync = syncData.LastSync;
                        existingTransactions = syncData.Data ?? new List<AccountTransaction>();
                    }
                }
                else
                {
                    existingTransactions = JsonConvert.DeserializeObject<List<AccountTransaction>>(existingJson) ?? new List<AccountTransaction>();
                }
            }

            foreach (var trx in allTransactions)
            {
                var existing = existingTransactions.FirstOrDefault(x =>
                    x.TransId == trx.TransId && x.Firm == trx.Firm && x.Period == trx.Period);

                if (existing != null)
                {
                    existing.RecordDate = trx.RecordDate;
                    existing.Status = trx.Status;
                    existing.Paid = trx.Paid;
                    existing.Total = trx.Total;
                    existing.Deleted = trx.Deleted;
                }
                else
                {
                    existingTransactions.Add(trx);
                }
            }
            var syncDataToSave = new SyncData<AccountTransaction>
            {
                LastSync = DateTime.Now,
                Data = existingTransactions
            };
            try
            {
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(syncDataToSave, Formatting.Indented));
                Logging.AddLog($"Cari hesap hareketleri JSON dosyasına kaydedildi. Toplam kayıt sayısı: {existingTransactions.Count}");
            }
            catch (Exception ex)
            {
                Logging.AddLog($"JSON dosyasına kaydetme hatası: {ex.Message}");
            }
        }

        public void DeletedTrans()
        {
            try
            {
                if (_cachedTransactionKeys == null || _cachedTransactionKeys.Count == 0)
                {
                    Logging.AddLog("Silinecek hareket bulunamadı.");
                    return;
                }

                Logging.AddLog("Silinen transaction key'leri cache'den alınıyor.");
                var apiKeys = _cachedTransactionKeys;

                var jsonPath = "AccountTransactions.json";
                List<AccountTransaction> localTransactions = new List<AccountTransaction>();
                DateTime lastSync = DateTime.MinValue;
                try
                {
                    var syncJson = File.Exists(jsonPath) ? File.ReadAllText(jsonPath) : null;
                    if (!string.IsNullOrEmpty(syncJson))
                    {
                        if (syncJson.TrimStart().StartsWith("{"))
                        {
                            var syncData = JsonConvert.DeserializeObject<SyncData<AccountTransaction>>(syncJson);
                            if (syncData != null)
                            {
                                lastSync = syncData.LastSync;
                                localTransactions = syncData.Data ?? new List<AccountTransaction>();
                            }
                        }
                        else
                        {
                            localTransactions = JsonConvert.DeserializeObject<List<AccountTransaction>>(syncJson) ?? new List<AccountTransaction>();
                        }
                    }
                }
                catch
                {
                    localTransactions = new List<AccountTransaction>();
                }

                localTransactions = localTransactions
                    ?.Where(x => x.Firm == ConfigHelper.DiaFirmaKodu && x.Period == ConfigHelper.DiaDonemKodu)
                    .ToList() ?? new List<AccountTransaction>();

                var deletedTransactions = localTransactions
                    .Where(x => !apiKeys.Contains(x.TransId.ToString()) && !x.Deleted)
                    .ToList();

                if (!deletedTransactions.Any())
                {
                    Logging.AddLog("Silinecek kayıt bulunamadı.");
                    return;
                }

                Logging.AddLog($"{deletedTransactions.Count} adet kayıt silinecek.");

                var updatedTransactions = new List<AccountTransaction>();

                foreach (var trans in deletedTransactions)
                {
                    var deleteParams = new[] { new CATDeleteParameters { ErpCode = trans.TransId.ToString() } };

                    var result = _catService.Delete(_catServiceAuthenticationInfo, deleteParams);

                    if (result != null && result.CurrentAccountTransactionList?.Any() == true)
                    {
                        trans.Deleted = true;
                        trans.RecordDate = DateTime.Now;
                        trans.Status = true;
                        updatedTransactions.Add(trans);

                        Logging.AddLog($"{trans.TransId} Erp Kodlu hareket silindi.");
                    }
                    else
                    {
                        Logging.AddLog($"Silinemedi veya sonuç alınamadı: {trans.TransId}");
                    }
                }

                foreach (var updated in updatedTransactions)
                {
                    var existing = localTransactions.FirstOrDefault(x =>
                        x.TransId == updated.TransId &&
                        x.Firm == updated.Firm &&
                        x.Period == updated.Period);

                    if (existing != null)
                    {
                        existing.Deleted = true;
                        existing.Status = true;
                        existing.RecordDate = DateTime.Now;
                    }
                    else
                    {
                        localTransactions.Add(updated);
                    }
                }

                var syncDataToSave = new SyncData<AccountTransaction>
                {
                    LastSync = lastSync,
                    Data = localTransactions
                };
                try
                {
                    File.WriteAllText(jsonPath, JsonConvert.SerializeObject(syncDataToSave, Formatting.Indented));
                    Logging.AddLog($"Cari hesap hareketleri JSON dosyasına kaydedildi. Toplam kayıt sayısı: {localTransactions.Count}");
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"JSON dosyasına kaydetme hatası: {ex.Message}");
                }

                _cachedTransactionKeys = null;
            }
            catch (Exception ex)
            {
                Logging.AddLog("Delete Trans - " + ex.Message);
                _cachedTransactionKeys = null;
            }
        }

        public List<CurrentAccountModel> GetCurrentAccounts(string accountCode = "")
        {
            Logging.AddLog("Cari hesapların aktarım işlemleri başlıyor...");

            DateTime lastSync = DateTime.MinValue;
            try
            {
                var syncJson = File.Exists("CurrentAccount.json") ? File.ReadAllText("CurrentAccount.json") : null;
                if (!string.IsNullOrEmpty(syncJson))
                {
                    if (syncJson.TrimStart().StartsWith("{"))
                    {
                        var syncData = JsonConvert.DeserializeObject<SyncData<CurrentAccountLogModel>>(syncJson);
                        if (syncData != null)
                            lastSync = syncData.LastSync;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (!string.IsNullOrEmpty(accountCode))
            {
                var specificParams = new BaseApiRequestParams()
                    .AddFilter("", "eposta", FilterTypes.NOT_EQUAL)
                    .AddFilter("1", "__dinamik__nteaktar", FilterTypes.EQUAL)
                    .AddFilter(accountCode, "carikartkodu", FilterTypes.EQUAL)
                    .AddFilter(ConfigHelper.DiaFirmaKodu.ToString(),"level1",FilterTypes.EQUAL)
                    .AddFilter(ConfigHelper.DiaDonemKodu.ToString(), "level2", FilterTypes.EQUAL)
                    .AddSort("_key", SortTypes.DESC);
                if (lastSync > DateTime.MinValue)
                    specificParams.AddFilter(lastSync.ToString("yyyy-MM-dd HH:mm:ss"), "_date", FilterTypes.GREATER_THEN_OR_EQUAL);

                var specificResponse = DIARepository.List(DiaEndPoints.Keys.CURRENTACCOUNT, specificParams);

                if (specificResponse == null || (specificResponse is JArray arr && !arr.Any()) || (specificResponse is JObject objj && !objj.Properties().Any()))
                    throw new Exception($"Cari hesap sorgulama sırasında beklenmedik hata alındı. Cari Hesap Kodu : {accountCode}");

                string specificAccountJson = JsonConvert.SerializeObject(specificResponse);
                var specificAccounts = JsonConvert.DeserializeObject<List<CurrentAccountModel>>(specificAccountJson);
                return specificAccounts;
            }

            if (_batchDataManager != null)
            {
                var batchData = _batchDataManager.GetCurrentAccounts();
                if (batchData != null && batchData.Any())
                {
                    Logging.AddLog("Cari hesaplar BatchDataManager'dan alınıyor ve filtreleniyor.");
                    return batchData
                        .Where(x => !string.IsNullOrEmpty(x.MailAddress))
                        .Where(x => x.NteAktar == "1")
                        .ToList();
                }
            }

            int limit = 200;
            int offset = 0;
            bool hasMore = true;
            var allCurrentAccounts = new List<CurrentAccountModel>();

            Logging.AddLog("Cari hesaplar API'den yükleniyor.");
            while (hasMore)
            {
                var allParams = new BaseApiRequestParams()
                    .AddFilter("", "eposta", FilterTypes.NOT_EQUAL)
                    .AddFilter("1", "__dinamik__nteaktar", FilterTypes.EQUAL)
                    .AddSort("_key", SortTypes.DESC)
                    .AddFilter(ConfigHelper.DiaFirmaKodu.ToString(), "level1", FilterTypes.EQUAL)
                    .AddFilter(ConfigHelper.DiaDonemKodu.ToString(), "level2", FilterTypes.EQUAL)
                    .Limit(limit)
                    .Offset(offset);
                if (lastSync > DateTime.MinValue)
                    allParams.AddFilter(lastSync.ToString("yyyy-MM-dd"), "_date", FilterTypes.GREATER_THEN_OR_EQUAL);

                var allResponse = DIARepository.List(DiaEndPoints.Keys.CURRENTACCOUNT, allParams);

                if (allResponse == null)
                    throw new Exception("Cari hesap sorgulama sırasında beklenmedik hata alındı.");

                List<CurrentAccountModel> currentBatch = new List<CurrentAccountModel>();

                if (allResponse is JArray arr2 && arr2.Any())
                {
                    string jsonString = JsonConvert.SerializeObject(arr2);
                    currentBatch = JsonConvert.DeserializeObject<List<CurrentAccountModel>>(jsonString);
                }
                else if (allResponse is JObject obj2 && obj2.Properties().Any())
                {
                    string jsonString = JsonConvert.SerializeObject(obj2);
                    currentBatch = JsonConvert.DeserializeObject<List<CurrentAccountModel>>(jsonString);
                }
                else
                {
                    hasMore = false;
                    break;
                }

                allCurrentAccounts.AddRange(currentBatch);

                if (currentBatch.Count < limit)
                    hasMore = false;
                else
                    offset += limit;
            }

            return allCurrentAccounts;
        }

        public void SendAccount(bool isDaily = false, string accountCode = "")
        {

            DateTime lastSync = DateTime.MinValue;
            List<CurrentAccountLogModel> existingLogs = new List<CurrentAccountLogModel>();
            try
            {
                var syncJson = File.Exists("CurrentAccount.json") ? File.ReadAllText("CurrentAccount.json") : null;
                if (!string.IsNullOrEmpty(syncJson))
                {
                    if (syncJson.TrimStart().StartsWith("{"))
                    {
                        var loadSyncData = JsonConvert.DeserializeObject<SyncData<CurrentAccountLogModel>>(syncJson);
                        if (loadSyncData != null)
                            lastSync = loadSyncData.LastSync;
                        existingLogs = loadSyncData?.Data ?? new List<CurrentAccountLogModel>();
                    }
                    else
                    {
                        existingLogs = JsonConvert.DeserializeObject<List<CurrentAccountLogModel>>(syncJson) ?? new List<CurrentAccountLogModel>();
                    }
                }
            }
            catch
            {
                existingLogs = new List<CurrentAccountLogModel>();
            }

            var currentAccounts = GetCurrentAccounts(accountCode);

            if (currentAccounts == null || currentAccounts.Count < 0 )
            {
                Logging.AddLog("Aktarılacak cari hesap bulunamadı.");
                return;
            }

            var localCurrentAccounts = existingLogs
                                          ?.Where(x => x.Firm == ConfigHelper.DiaFirmaKodu
                                                   && x.Period == ConfigHelper.DiaDonemKodu)
                                          .ToList() ?? new List<CurrentAccountLogModel>();
            var successfulCodes = localCurrentAccounts.Where(x => x.Status).ToDictionary(x => x.Code, x => x.RecordDate);

            List<CurrentAccountModel> transferCurrentAccounts = new List<CurrentAccountModel>();

            if (isDaily)
            {
                var failedCodes = localCurrentAccounts
                    .Where(x => !x.Status)
                    .Select(x => x.Code)
                    .ToHashSet();

                transferCurrentAccounts = currentAccounts
                    .Where(ca => failedCodes.Contains(ca.Code) || 
                                (successfulCodes.ContainsKey(ca.Code) && ca.UpdateDate > successfulCodes[ca.Code]))
                    .Select(ca => 
                    {
                        ca.IntegrationStatus = successfulCodes.ContainsKey(ca.Code);
                        return ca;
                    })
                    .ToList();

                Logging.AddLog($"Günlük aktarım: {transferCurrentAccounts.Count} adet cari hesap aktarılacak");
            }
            else
            {
                transferCurrentAccounts = currentAccounts
                    .Where(ca => !successfulCodes.ContainsKey(ca.Code) || 
                                (successfulCodes.ContainsKey(ca.Code) && ca.UpdateDate > successfulCodes[ca.Code]))
                    .Select(ca => 
                    {
                        ca.IntegrationStatus = successfulCodes.ContainsKey(ca.Code);
                        return ca;
                    })
                    .ToList();

                Logging.AddLog($"{transferCurrentAccounts.Count} adet cari hesap aktarılacak");
            }

            foreach (CurrentAccountModel currentAccount in transferCurrentAccounts)
            {
                bool isSuccess = false;
                string message = string.Empty;
                
                try
                {
                    var paymentSetParam = Config.GlobalParameters.GlobalSettings.CUSTOMERPAYMENTSETS?.FirstOrDefault(x => currentAccount.Code.StartsWith(x.CustomerPrefix) &&
                    (string.IsNullOrEmpty(x.FirmNo) || (int.TryParse(x.FirmNo, out int firmNo) && firmNo == currentAccount.FirmNumber)));

                    int paymentSetId = paymentSetParam?.PaymentSetId ?? 0;
                    int currentAccountTypeId = paymentSetParam?.CurrentAccountTypeId ?? 0;

                    string errorMessage = string.Empty;

                    var customerData = MapCustomer(currentAccount, paymentSetId);

                    if (customerData is null)
                        throw new Exception("Veri eşleşmesinde sorun yaşandı.");

                    if (Config.GlobalParameters.GlobalSettings.CUSTOMER_TRANSFER_TYPE == CustomerTransferType.Vendor)
                    {
                        var vendorData = MapVendor(customerData);
                        var userData = MapUser(customerData);

                        if (currentAccount.IntegrationStatus)
                        {
                            var updateVendorResult = _vendorWebService.UpdateVendor(_vendorServiceAuthenticationInfo, vendorData.Code, vendorData);
                            isSuccess = updateVendorResult.IsSuccess;
                            errorMessage = updateVendorResult.ErrorMessage;
                        }
                        else
                        {
                            var createVendorResult = _vendorWebService.CreateVendor(_vendorServiceAuthenticationInfo, vendorData, userData);
                            isSuccess = createVendorResult.IsSuccess;
                            errorMessage = createVendorResult.ErrorMessage;
                        }

                        message = isSuccess ? "Bayi aktarımı başarılı" : $"Bayi aktarımı başarısız: {errorMessage}";
                        Logging.AddLog(isSuccess ? $"{currentAccount.Code} ERP kodlu bayi aktarımı başarılı." : $"{currentAccount.Code} ERP kodlu bayi aktarımı yapılamadı. Hata: {errorMessage}");
                    }
                    else
                    {
                        if (currentAccount.IntegrationStatus)
                        {
                            var updateResult = _vendorWebService.UpdateCustomer(_vendorServiceAuthenticationInfo, customerData.Code, customerData);
                            isSuccess = updateResult.IsSuccess;
                            errorMessage = updateResult.ErrorMessage;
                        }
                        else
                        {
                            var createResult = _vendorWebService.CreateCustomer(_vendorServiceAuthenticationInfo, customerData);
                            isSuccess = createResult.IsSuccess;
                            errorMessage = createResult.ErrorMessage;
                        }

                        if (!isSuccess)
                        {
                            Logging.AddLog($"{currentAccount.Code} ERP kodlu cari hesap aktarılamadı. Hata: {errorMessage}");
                            message = $"Cari hesap aktarımı başarısız: {errorMessage}";
                        }
                        else
                        {
                            if (!currentAccount.IntegrationStatus)
                            {
                                var currentAccountData = new CurrentAccountData
                                {
                                    ErpCode = currentAccount.Code,
                                    Description = "",
                                    Name = currentAccount.Title,
                                    VendorErpCode = customerData.ErpCode,
                                    PaymentSetDefinitionId = paymentSetId,
                                    CurrencyCodes = new int[] { customerData.CurrencyTypeId },
                                    IsVisibleOnSimplePaymentSection = true,
                                    IsVisibleOnPaymentWithPaymentItemSection = true,
                                    CurrentAccountTypeId = currentAccountTypeId,
                                    IsUsableBySubVendor = false,
                                    ErpFirmCode = Config.GlobalParameters.GlobalSettings.USE_MULTIFIRM ? (int?)currentAccount.FirmNumber : (int?)null,
                                };

                                var currentAccountDetailsResponse = _vendorWebService
                                    .CreateOrUpdateCurrentAccount(_vendorServiceAuthenticationInfo, currentAccountData);

                                if (currentAccountDetailsResponse.IsSuccess)
                                {
                                    message = "Cari hesap detayları aktarımı başarılı";
                                    Logging.AddLog($"{currentAccount.Code} erp kodlu carini hesap detayları aktarıldı.");
                                }
                                else
                                {
                                    currentAccount.IntegrationStatus = false;
                                    var failedMessage = $"{currentAccount.Code} erp kodlu carini hesap detayları aktarılamadı. " +
                                        $"Hata: {currentAccountDetailsResponse.ErrorMessage}";

                                    Logging.AddLog(failedMessage);
                                    message = $"Cari hesap detayları aktarımı başarısız: {currentAccountDetailsResponse.ErrorMessage}";
                                    isSuccess = false;
                                }
                            }
                            else
                            {
                                message = "Cari hesap güncelleme işlemi başarılı";
                                Logging.AddLog($"{currentAccount.Code} erp kodlu cari hesap güncellendi.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"{currentAccount.Code} erp kodlu hesabın aktarımında hata alındı. Hata: {ex.Message}");
                    message = $"Aktarım hatası: {ex.Message}";
                    isSuccess = false;
                }
                finally
                {
                    UpdateCurrentAccountLog(existingLogs, currentAccount, message, isSuccess);
                }
            }
            // SyncData ile kaydet
            var syncData = new SyncData<CurrentAccountLogModel>
            {
                LastSync = DateTime.Now,
                Data = existingLogs
            };
            try
            {
                File.WriteAllText("CurrentAccount.json", JsonConvert.SerializeObject(syncData, Formatting.Indented));
                Logging.AddLog($"Cari hesaplar JSON dosyasına kaydedildi. Toplam kayıt sayısı: {existingLogs.Count}");
            }
            catch (Exception ex)
            {
                Logging.AddLog($"JSON dosyasına kaydetme hatası: {ex.Message}");
            }
        }




        private UserData MapUser(CustomerData customerData)
        {
            var userData = new UserData()
            {
                Code = customerData.Code,
                CountryCodeISO = customerData.CountryCodeISO,
                Email = customerData.Email,
                FirstName = customerData.FirstName,
                LastName = customerData.LastName,
                IsActive = true,
                Mobile = customerData.Mobile,
                PassportNumber = customerData.PassportNumber,
                Password = customerData.Password ?? null,
                SendMail = customerData.SendMail,
                TCKN = customerData.TCKN
            };

            return userData;
        }

        private VendorData MapVendor(CustomerData customerData)
        {
            var vendorData = new VendorData()
            {
                Address = customerData.Address,
                CityCode = customerData.CityCode,
                Code = customerData.Code,
                CompanyName = customerData.CompanyName,
                CountryCodeISO = customerData.CountryCodeISO,
                CurrencyTypeId = customerData.CurrencyTypeId,
                CurrentAccountGroupCode = customerData.CurrentAccountGroupCode,
                Email = customerData.Email,
                ErpCode = customerData.ErpCode,
                IsActive = customerData.IsActive,
                IsCompany = customerData.IsCompany,
                ParentCode = customerData.ParentCode,
                ParentUserEmail = customerData.ParentUserEmail,
                PassportNumber = customerData.PassportNumber,
                PaymentSetId = customerData.PaymentSetId,
                Phone = customerData.Phone,
                TCKN = customerData.TCKN,
                TaxOffice = customerData.TaxOffice,
                TaxNumber = customerData.TaxNumber,
                AddDefaultCurrentAccount = true,
                CanCreateCustomer = true,
                CanCreateVendor = true,
                CanCancelPayment = true,
                ErpFirmCode = customerData.ErpFirmCode,
            };

            return vendorData;
        }

        private CustomerData MapCustomer(CurrentAccountModel customer, int paymentSetId)
        {
            var tckn = customer.TCKN;
            var taxnumber = customer.TaxNumber;
            var erpCode = customer.Code;
            var isCompany = !string.IsNullOrEmpty(taxnumber);

            var mailAddress = string.IsNullOrWhiteSpace(customer.MailAddress)
                ? $"{erpCode}@temp.com"
                : customer.MailAddress;

            var guid = Guid.NewGuid().ToString("N");
            var random = new Random();
            var password = guid.Select(c => random.Next(2) == 0 ? char.ToLower(c) : char.ToUpper(c)).OrderBy(_ => random.Next()).Take(12).ToArray();


            return new CustomerData
            {
                Code = erpCode,
                CompanyName = customer.Title,
                Email = mailAddress,
                ErpCode = erpCode,
                IsActive = true,
                TaxNumber = taxnumber.Length > 0 ? taxnumber : null,
                TaxOffice = customer.TaxOffice,
                Address = customer.Address,
                CurrencyTypeId = (int)EnumExtensions.GetValueFromDisplayName<DiaCurrenyType>(customer.CurrencyType),
                ParentCode = "",
                ParentUserEmail = "",
                Phone = customer.PhoneNumber.Length > 0 ? customer.PhoneNumber : "2121111111",
                FirstName = customer.Title,
                LastName = "-",
                Mobile = !string.IsNullOrWhiteSpace(customer.MobilePhoneNumber) && customer.MobilePhoneNumber.Length >= 10 ? customer.MobilePhoneNumber.Substring(customer.MobilePhoneNumber.Length - 10) : "5321111111",
                Password = customer.IntegrationStatus ? null : new string(password),
                TCKN = tckn,
                SendMail = customer.IntegrationStatus ? false : Config.GlobalParameters.GlobalSettings.SEND_EMAIL,
                CityCode = customer.City.ToString(),
                CountryCodeISO = "TR",
                IsCompany = isCompany,
                AddDefaultCurrentAccount = true,
                PassportNumber = "",
                PaymentSetId = paymentSetId,
            };
        }

        private void UpdateCurrentAccountLog(List<CurrentAccountLogModel> existingLogs, CurrentAccountModel currentAccount, string message, bool isSuccess)
        {
            var existingIndex = existingLogs.FindIndex(x => x.Code == currentAccount.Code);
            var currentTime = DateTime.Now;
            if (existingIndex >= 0)
            {
                var existingLog = existingLogs[existingIndex];
                existingLog.Name = currentAccount.Title;
                existingLog.Firm = currentAccount.FirmNumber;
                existingLog.Period = currentAccount.PeriodNumber;
                existingLog.Message = message;
                existingLog.Status = isSuccess;
                existingLog.LastUpdateDate = currentTime;
                existingLogs[existingIndex] = existingLog;
            }
            else
            {
                var logData = new CurrentAccountLogModel
                {
                    Code = currentAccount.Code,
                    Name = currentAccount.Title,
                    Firm = currentAccount.FirmNumber,
                    Period = currentAccount.PeriodNumber,
                    Message = message,
                    Status = isSuccess,
                    RecordDate = currentTime,
                    LastUpdateDate = currentTime
                };
                existingLogs.Add(logData);
            }
        }

        private void SaveCurrentAccountLog(List<CurrentAccountLogModel> existingLogs)
        {
            try
            {
                JsonDbManager.SaveToFile(existingLogs, "CurrentAccount.json");
                Logging.AddLog($"Cari hesaplar JSON dosyasına kaydedildi. Toplam kayıt sayısı: {existingLogs.Count}");
            }
            catch (Exception ex)
            {
                Logging.AddLog($"JSON dosyasına kaydetme hatası: {ex.Message}");
            }
        }

        public AccountTransaction SetAccountTransaction(CATCreateOrUpdateParameters catParam, bool status, bool deleted)
        {
            if (catParam == null || !int.TryParse(catParam.ErpCode, out int transId))
                return null;

            return new AccountTransaction
            {
                TransId = transId,
                RecordDate = DateTime.Now,
                Firm = ConfigHelper.DiaFirmaKodu,
                Period = ConfigHelper.DiaDonemKodu,
                Status = status,
                Paid = Convert.ToDouble(catParam.PaidAmount),
                Total = catParam.Amount,
                Deleted = deleted
            };
        }

        // Maksimum taksit güncelleme
        public void UpdateMaxInstallments()
        {
            // Implementation will be added when needed
        }

        // IDisposable desteği
        public void Dispose()
        {
            // Implementation will be added when needed
        }
    }
}