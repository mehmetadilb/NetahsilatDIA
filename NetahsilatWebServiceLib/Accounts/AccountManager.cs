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
                Logging.AddLog("Daha önce aktarımı başarısız olanlar ve güncellenmiş olan cari hesap hareketleri aktarılacak");

                var myParamList = new List<CATCreateOrUpdateParameters>();
                var currentAccountTransactionAll = new List<CurrentAccountTransactionModel>();

                Logging.AddLog("Cari hesap hareketleri API'den yükleniyor.");
                var diaResult = DIARepository.List(DiaEndPoints.Keys.CURRENTACCOUNTFICHE, null, DiaEndPoints.Suffixes.DETAILED_LIST);
                var json = JsonConvert.SerializeObject(diaResult);
                currentAccountTransactionAll = JsonConvert.DeserializeObject<List<CurrentAccountTransactionModel>>(json);

                _cachedTransactionKeys = currentAccountTransactionAll?.Select(x => x.Key).ToHashSet() ?? new HashSet<string>();

                if (currentAccountTransactionAll == null || currentAccountTransactionAll.Count == 0)
                {
                    Logging.AddLog("API'den gelen cari hesap hareketi bulunamadı.");
                    return;
                }

                var activeFirm = Config.GlobalParameters.Parameters.Firms?.FirstOrDefault(f => f.IsActive);

                var localFailedTransactions = JsonDbManager.LoadFromFile<List<AccountTransaction>>("AccountTransactions.json")
                    ?.Where(x => x.Firm == activeFirm?.Company && x.Period == activeFirm?.Period && x.Deleted == false)
                    .ToList() ?? new List<AccountTransaction>();

                var localDict = localFailedTransactions.ToDictionary(x => x.TransId);

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

        public void SendAccountTransaction(string accountCode = "", bool isManuel = false)
        {
            Logging.AddLog("Cari hesap hareketleri aktarılacak");
            try
            {
                var myParamList = new List<CATCreateOrUpdateParameters>();
                var currentAccountTransactionAll = new List<CurrentAccountTransactionModel>();

                Logging.AddLog("Cari hesap hareketleri API'den yükleniyor.");
                var diaResult = DIARepository.List(DiaEndPoints.Keys.CURRENTACCOUNTFICHE, null, DiaEndPoints.Suffixes.DETAILED_LIST);
                var json = JsonConvert.SerializeObject(diaResult);
                currentAccountTransactionAll = JsonConvert.DeserializeObject<List<CurrentAccountTransactionModel>>(json);

                _cachedTransactionKeys = currentAccountTransactionAll?.Select(x => x.Key).ToHashSet() ?? new HashSet<string>();

                if (currentAccountTransactionAll == null || currentAccountTransactionAll.Count == 0)
                {
                    Logging.AddLog("Yeni cari hesap hareketi bulunamadı.");
                    return;
                }
                var activeFirm = Config.GlobalParameters.Parameters.Firms?.FirstOrDefault(f => f.IsActive);

                var localCurrentAccountTransactions = JsonDbManager.LoadFromFile<List<AccountTransaction>>("AccountTransactions.json")
                    ?.Where(x => x.Firm == activeFirm?.Company && x.Period == activeFirm?.Period && x.Deleted == false)
                    .ToList() ?? new List<AccountTransaction>();

                var localDict = localCurrentAccountTransactions.ToDictionary(x => x.TransId);

                var newOrUpdatedTransactions = currentAccountTransactionAll
                    .Where(x =>!localDict.ContainsKey(int.Parse(x.Key)) ||
                        (localDict[int.Parse(x.Key)].Status == true && x.Date > localDict[int.Parse(x.Key)].RecordDate)
                    ).ToList();

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
                Logging.AddLog(string.Format("SendAccountTransaction - Hata : {0}", ex.Message));
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

            foreach (var sendParam in sendParamList)
            {
                try
                {
                    tempSendParamList.Add(sendParam);

                    var result = _catService.CreateOrUpdate(_catServiceAuthenticationInfo, tempSendParamList.ToArray())?.CurrentAccountTransactionList?.FirstOrDefault();

                    var catParam = sendParamList.Where(x => x.ErpCode == result.ErpCode).FirstOrDefault();

                    if (result.Result.Status == ExecuteStatus.Success)
                    {
                        SetAccountTransaction(catParam, true, false);
                        Logging.AddLog($"{result.ErpCode} erp kodlu hareket aktarıldı.");
                    }
                    else
                    {
                        SetAccountTransaction(catParam, false, false);
                        Logging.AddLog($"{result.ErpCode} erp kodlu hareket aktarılamadı. Hata: {result?.Result?.Message}");
                    }
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
        }

        public  void DeletedTrans()
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

                var activeFirm = Config.GlobalParameters.Parameters.Firms?.FirstOrDefault(f => f.IsActive);

                var localTransactions = JsonDbManager.LoadFromFile<List<AccountTransaction>>("AccountTransactions.json")
                    ?.Where(x => x.Firm == activeFirm?.Company && x.Period == activeFirm?.Period && x.Deleted == false)
                    .ToList() ?? new List<AccountTransaction>();

                var deletedTransactions = localTransactions.Where(x => !apiKeys.Contains(x.TransId.ToString())).ToList();

                if (!deletedTransactions.Any())
                {
                    Logging.AddLog("Silinecek kayıt bulunamadı.");
                    return;
                }

                Logging.AddLog($"{deletedTransactions.Count} adet kayıt silinecek.");


                foreach (var trans in deletedTransactions)
                {
                    var deleteParams = new[] { new CATDeleteParameters { ErpCode = trans.TransId.ToString() } };

                    var result = _catService.Delete(_catServiceAuthenticationInfo, deleteParams);

                    if (result != null && result.CurrentAccountTransactionList?.Any() == true)
                    {
                        var catParam = new CATCreateOrUpdateParameters { ErpCode = trans.TransId.ToString() };

                        SetAccountTransaction(catParam, true, true);

                        Logging.AddLog($"{trans.TransId} Erp Kodlu hareket silindi.");
                    }
                    else
                    {
                        Logging.AddLog($"Silinemedi veya sonuç alınamadı: {trans.TransId}");
                    }
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

            // Eğer belirli bir hesap kodu aranıyorsa cache kullanmaz
            if (!string.IsNullOrEmpty(accountCode))
            {
                var specificParams = new BaseApiRequestParams()
                    .AddFilter("", "eposta", FilterTypes.NOT_EQUAL)
                    .AddFilter("1", "__dinamik__nteaktar", FilterTypes.EQUAL)
                    .AddFilter(accountCode, "carikartkodu", FilterTypes.EQUAL)
                    .AddSort("_key", SortTypes.DESC);

                var specificResponse = DIARepository.List(DiaEndPoints.Keys.CURRENTACCOUNT, specificParams);

                if (specificResponse == null || (specificResponse is JArray arr && !arr.Any()) || (specificResponse is JObject objj && !objj.Properties().Any()))
                    throw new Exception($"Cari hesap sorgulama sırasında beklenmedik hata alındı. Cari Hesap Kodu : {accountCode}");

                string specificAccountJson = JsonConvert.SerializeObject(specificResponse);
                var specificAccounts = JsonConvert.DeserializeObject<List<CurrentAccountModel>>(specificAccountJson);
                return specificAccounts;
            }

            // Önce BatchDataManager'dan almayı dene
            if (_batchDataManager != null)
            {
                var batchData = _batchDataManager.GetCurrentAccounts();
                if (batchData != null && batchData.Any())
                {
                    Logging.AddLog("Cari hesaplar BatchDataManager'dan alınıyor ve filtreleniyor.");
                    // BatchDataManager'dan gelen verileri filtrele ve döndür
                    return batchData
                        .Where(x => !string.IsNullOrEmpty(x.MailAddress)) // Eposta boş olmamalı
                        .Where(x => x.NteAktar == "1") // NTE Aktar = 1 olmalı
                        .ToList();
                }
            }

            // BatchDataManager'dan veri gelmezse API'den çek
            Logging.AddLog("Cari hesaplar API'den yükleniyor.");
            var allParams = new BaseApiRequestParams()
                .AddFilter("", "eposta", FilterTypes.NOT_EQUAL)
                .AddFilter("1", "__dinamik__nteaktar", FilterTypes.EQUAL)
                .AddSort("_key", SortTypes.DESC);

            var allResponse = DIARepository.List(DiaEndPoints.Keys.CURRENTACCOUNT, allParams);

            if (allResponse == null || (allResponse is JArray arr2 && !arr2.Any()) || (allResponse is JObject obj2 && !obj2.Properties().Any()))
                throw new Exception($"Cari hesap sorgulama sırasında beklenmedik hata alındı.");

            string jsonString = JsonConvert.SerializeObject(allResponse);
            var currentAccounts = JsonConvert.DeserializeObject<List<CurrentAccountModel>>(jsonString);
            return currentAccounts;
        }

        public void SendAccount(bool isDaily = false, string accountCode = "")
        {
            var currentAccounts = GetCurrentAccounts(accountCode);

            if (currentAccounts.Count < 0 )
            {
                Logging.AddLog("Aktarılacak cari hesap bulunamadı.");
                return;
            }

            var activeFirm = Config.GlobalParameters.Parameters.Firms?.FirstOrDefault(f => f.IsActive);
            if (activeFirm == null)
            {
                Logging.AddLog("Aktif firma bulunamadı.");
                return;
            }

            var localCurrentAccounts = JsonDbManager.LoadFromFile<List<CurrentAccountLogModel>>("CurrentAccount.json")
                                          ?.Where(x => x.Firm == activeFirm.Company
                                                   && x.Period == activeFirm.Period)
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

            var existingLogs = new List<CurrentAccountLogModel>();
            try
            {
                existingLogs = JsonDbManager.LoadFromFile<List<CurrentAccountLogModel>>("CurrentAccount.json");
            }
            catch
            {
                existingLogs = new List<CurrentAccountLogModel>();
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
                                    Name = currentAccount.FirmName,
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
            SaveCurrentAccountLog(existingLogs);
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
                Password = "net_" + customerData.Code,
                SendMail = Config.GlobalParameters.GlobalSettings.SEND_EMAIL,
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
                CurrencyTypeId = 1,
                ParentCode = "",
                ParentUserEmail = "",
                Phone = customer.PhoneNumber.Length > 0 ? customer.PhoneNumber : "2121111111",
                FirstName = customer.Title,
                LastName = "-",
                Mobile = customer.MobilePhoneNumber.Length > 0 ? customer.MobilePhoneNumber : "5321111111",
                Password = "1234",
                TCKN = tckn,
                SendMail = Config.GlobalParameters.GlobalSettings.SEND_EMAIL,
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

        public void SetAccountTransaction(CATCreateOrUpdateParameters catParam, bool status, bool deleted)
        {
            string jsonPath = "AccountTransactions.json";
            try
            {
                if (catParam != null)
                {
                    string transIdStr = catParam.ErpCode;
                    int transId = int.Parse(transIdStr);
                    int firm = ConfigHelper.DiaFirmaKodu;
                    int period = ConfigHelper.DiaDonemKodu;

                    List<AccountTransaction> transactions = new List<AccountTransaction>();

                    if (File.Exists(jsonPath))
                    {
                        string existingJson = File.ReadAllText(jsonPath);
                        transactions = JsonConvert.DeserializeObject<List<AccountTransaction>>(existingJson) ?? new List<AccountTransaction>();
                    }

                    var existingTransaction = transactions.FirstOrDefault(t =>
                        t.TransId == transId && t.Firm == firm && t.Period == period);

                    if (existingTransaction != null)
                    {
                        existingTransaction.RecordDate = DateTime.Now;
                        existingTransaction.Status = status;
                        existingTransaction.Paid = Convert.ToDouble(catParam.PaidAmount);
                        existingTransaction.Total = catParam.Amount;
                        existingTransaction.Deleted = deleted;
                    }
                    else
                    {
                        transactions.Add(new AccountTransaction
                        {
                            TransId = transId,
                            RecordDate = DateTime.Now,
                            Firm = firm,
                            Period = period,
                            Status = status,
                            Paid = Convert.ToDouble(catParam.PaidAmount),
                            Total = catParam.Amount,
                            Deleted = deleted
                        });
                    }

                    JsonDbManager.SaveToFile(transactions, "AccountTransactions.json");
                }
                else
                {
                    Logging.AddLog("SetAccountTransaction Hatası : Hareket bilgisi boş geldi.");
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog("SetAccountTransaction Hatası : " + ex.Message);
            }
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