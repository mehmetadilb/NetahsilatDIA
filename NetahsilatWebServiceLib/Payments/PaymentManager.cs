using CommonLib;
using CommonLib.Enum;
using CommonLib.Model;
using Netahsilat.DIAService;
using Netahsilat.DIAService.Model;
using NetahsilatWebServiceLib.Accounts;
using NetahsilatWebServiceLib.ErpWebService;
using NetahsilatWebServiceLib.Models;
using NetahsilatWebServiceLib.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetahsilatWebServiceLib.Payments
{
    public class PaymentManager
    {
        private readonly BasicHttpBinding_IErpWebService _erpWebService;

        private readonly AuthenticationInfo _authenticationInfo;

        //private readonly ITransferService _transferService;
        private readonly DescriptionHelper _descriptionHelper;
        private readonly GlobalParameters _globalParameters;
        private readonly BatchDataManager _batchDataManager;

        public PaymentManager()
        {
            _globalParameters = new GlobalParameters();
            _batchDataManager = new BatchDataManager();
            try
            {
                System.Net.ServicePointManager.Expect100Continue = false;

                if (_erpWebService == null)
                    _erpWebService = new BasicHttpBinding_IErpWebService();

                if (!string.IsNullOrWhiteSpace(Config.GlobalParameters.Parameters.ERP_SERVICE))
                {
                    _erpWebService.Url = Config.GlobalParameters.Parameters.ERP_SERVICE;
                    _authenticationInfo = new AuthenticationInfo { UserName = Config.GlobalParameters.Parameters.WEB_SERVICE_UID, Password = Config.GlobalParameters.Parameters.WEB_SERVICE_PWD };
                }

                _descriptionHelper = new DescriptionHelper();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} - (PaymentManager)");
            }
        }

        public void GetNonIntegratedPayment()
        {
            Logging.AddLog("Daha önce aktarılmamış ödemeler aktarılacak");

            VirtualPosListResult posList = _erpWebService.GetVirtualPosList(_authenticationInfo);

            if (posList.IsSuccess)
            {
                PaymentServiceModelListResult payments = _erpWebService.GetNonIntegratedPayments(_authenticationInfo);

                if (payments.IsSuccess)
                {
                    GetPayment(payments, posList);
                }
                else
                {
                    Logging.AddLog($"Hata: Ödemeler Netahsilat tarafından alınamadı! Detay: {posList.ErrorMessage} - (GetNonIntegratedPayment)");
                }
            }
            else
            {
                Logging.AddLog($"Hata: Pos listesi Netahsilat tarafından alınamadı! Detay: {posList.ErrorMessage} - (GetNonIntegratedPayment)");
            }
        }

        public int GetNonIntegratedReversal()
        {
            Logging.AddLog("Daha önce aktarılmamıl iadeler aktarılacak");

            VirtualPosListResult posList = _erpWebService.GetVirtualPosList(_authenticationInfo);

            if (posList.IsSuccess)
            {
                ReversalServiceModelListResult reversals = _erpWebService.GetNonIntegratedReversals(_authenticationInfo);

                if (reversals.IsSuccess)
                {
                    return GetReversals(reversals, posList);
                }
                else
                {
                    if (!String.IsNullOrEmpty(reversals.ErrorCode) && reversals.ErrorCode == "404")
                    {
                        Logging.AddLog($"Entegrasyonu yapılmamış iade tespit edilmedi.");
                    }
                    else
                    {
                        Logging.AddLog($"Hata: İadeler Netahsilat tarafından alınamadı! Detay: {reversals.ErrorMessage} - (NonIntegratedReversal)");
                    }
                }
            }
            else
            {
                Logging.AddLog($"Hata: Pos listesi Netahsilat tarafından alınamadı! Detay: {posList.ErrorMessage} - (NonIntegratedReversal)");
            }

            return 0;
        }

        public int GetPaymentFromDate(string firstDate, string LastDate, int IntegrationStatus)
        {
            VirtualPosListResult posList = _erpWebService.GetVirtualPosList(_authenticationInfo);


            if (!posList.IsSuccess)
            {
                throw new Exception($"Hata: Pos listesi Netahsilat tarafından alınamadı! Detay: {posList.ErrorMessage} - (GetPaymentFromDate)");
            }

            PaymentServiceModelListResult payments = _erpWebService.GetPayments(_authenticationInfo, firstDate, LastDate, IntegrationStatus, true);

            if (!payments.IsSuccess)
            {
                throw new Exception($"Hata: Ödemeler Netahsilat tarafından alınamadı! Detay: {payments.ErrorMessage} - (GetPaymentFromDate)");
            }

            return GetPayment(payments, posList);
        }

        public bool GetPaymentByReferenceNumber(string referenceNumber)
        {
            try
            {
                VirtualPosListResult posList = _erpWebService.GetVirtualPosList(_authenticationInfo);

                if (!posList.IsSuccess)
                {
                    throw new Exception("Hata: Pos listesi Netahsilat tarafından alınamadı! Detay: " + posList.ErrorMessage);
                }

                PaymentServiceModelResult paymentResult = _erpWebService.GetPaymentByReferenceCode(_authenticationInfo, referenceNumber);

                if (paymentResult.IsSuccess)
                {
                    if (paymentResult.Payment.TransactionStatusId == (int)PaymentSuccessStatus.Failed)
                        throw new Exception(referenceNumber + " referans no lu ödeme başarısız olarak gerçekleşmiştir. Başarısız ödemenin aktarımı yapılamaz!");

                    return InsertPayment(paymentResult.Payment, posList);
                }
                else
                {
                    throw new Exception("Hata: Ödemeler Netahsilat tarafından alınamadı! Detay: " + paymentResult.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Hata: Ödemeler Netahsilat tarafından alınamadı! Detay: {ex.Message} - (GetPaymentByReferenceNumber)");
            }
        }

        public int GetReversalFromDate(string firstDate, string LastDate, int IntegrationStatus)
        {
            VirtualPosListResult posList = _erpWebService.GetVirtualPosList(_authenticationInfo);

            if (!posList.IsSuccess)
            {
                throw new Exception("Hata: Pos listesi Netahsilat tarafından alınamadı! Detay: " + posList.ErrorMessage);
            }

            ReversalServiceModelListResult reversals = _erpWebService.GetReversals(_authenticationInfo, firstDate, LastDate, IntegrationStatus, true);

            if (!reversals.IsSuccess)
            {
                throw new Exception("Hata: İade ödemeleri Netahsilat tarafından alınamadı! Detay: " + reversals.ErrorMessage);
            }

            return GetReversals(reversals, posList);
        }

        public int GetPayment(PaymentServiceModelListResult pymListResult, VirtualPosListResult posList)
        {
            int successCount = 0;
            int totalCount = pymListResult.Payments.Length;

            Logging.AddLog($"Toplam {totalCount} adet ödeme işlenecek.");

            try
            {
                // Batch veri yükleme - tüm payment'lar için ortak verileri önceden yükle
                var payments = pymListResult.Payments.ToList();
                _batchDataManager.PreloadBatchDataAsync(payments, posList).Wait();

                // Her payment'ı işle
                foreach (var payment in pymListResult.Payments)
                {
                    try
                    {
                        if (InsertPayment(payment, posList))
                        {
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.AddLog($"Payment işleme hatası - PaymentId: {payment.PaymentId}, Hata: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"Batch payment işleme hatası: {ex.Message}");
            }
            finally
            {
                // Cache'i temizle
                _batchDataManager.ClearCache();
            }

            Logging.AddLog($"Payment işleme tamamlandı. Başarılı: {successCount}/{totalCount}");
            return successCount;
        }

        private bool InsertPayment(PaymentServiceModel payment, VirtualPosListResult posList)
        {
            string voucherNumber = payment.ErpCode;
            bool isSuccess = true;

            try
            {
                Logging.AddLog($"Ödeme Id: {payment.PaymentId} Referans Kodu: {payment.ReferenceCode}");

                if (payment.Reversals != null && payment.Reversals.Any())
                {
                    if (payment.Reversals.Any(x => x.ReversalType == 50))
                    {
                        throw new Exception($"Ödeme durumu iptal olduğu için aktarım işlemi durduruldu. Ödeme Id: {payment.PaymentId}");
                    }
                }

                if (!posList.VirtualPoses.Any(i => i.VPosId == payment.VPosId))
                {
                    throw new Exception($"Pos pasif olduğu için aktarım yapılamadı. Sanal Pos:  {payment.VPosId}-{payment.VPosERPCode}");
                }

                Logging.AddLog("İptal mi kontrolü yapıldı.");


                var _params = new BaseApiRequestParams();
                if (!string.IsNullOrEmpty(voucherNumber))
                {
                    _params = new BaseApiRequestParams().AddFilter(voucherNumber, "fisno", FilterTypes.EQUAL);

                    var isExistsPayment = DIARepository.List(DiaEndPoints.Keys.CURRENTACCOUNTFICHE, _params);
                    //var isExistsPayment = _paymentRepository.IsExistPayment(voucherNumber);

                    if (isExistsPayment?.Count > 0)
                    {
                        Logging.AddLog($"'{voucherNumber}' fiş numaralı kredi kartı fişi DİA'da olduğundan işlem yapılamadı.");
                        return false;
                    }
                }

                _params = new BaseApiRequestParams()
                {
                    ColumnName = "fisno",
                    TableName = "scf_carihesap_fisi",
                    TemplateType = "CRHSP_FIS_FISNO",
                };
                var paymentFicheNumbers = DIARepository.Get(DiaEndPoints.Keys.VOUCHERNUMBER, _params);

                voucherNumber = !string.IsNullOrEmpty(payment.ErpCode) ? payment.ErpCode : paymentFicheNumbers?.kod?.ToString();

                var dynamicFieldValues = DynamicValuesHelper.GetAllDynamicFieldValues(payment);

                // BatchDataManager'dan verileri al
                if (!String.IsNullOrEmpty(dynamicFieldValues.Salesman))
                {
                    var salesmanModel = _batchDataManager.GetSalesman(dynamicFieldValues.Salesman);
                    if (salesmanModel == null)
                    {
                        Logging.AddLog($"'{dynamicFieldValues.Salesman}' kodlu satış elemanı DİA'da bulunamadı.");
                        dynamicFieldValues.Salesman = String.Empty;
                    }
                }

                if (!String.IsNullOrEmpty(dynamicFieldValues.AuthCode))
                {
                    var authCode = _batchDataManager.GetAuthCode(dynamicFieldValues.AuthCode);
                    if (authCode == null)
                    {
                        Logging.AddLog($"'{dynamicFieldValues.AuthCode}' kodlu yetki DİA'da bulunamadı.");
                        dynamicFieldValues.AuthCode = String.Empty;
                    }
                }

                if (!String.IsNullOrEmpty(dynamicFieldValues.ProjectCode))
                {
                    var project = _batchDataManager.GetProjectCode(dynamicFieldValues.ProjectCode);
                    if (project == null)
                    {
                        Logging.AddLog($"'{dynamicFieldValues.ProjectCode}' kodlu proje kodu DİA'da bulunamadı.");
                        dynamicFieldValues.ProjectCode = String.Empty;
                    }
                }

                var descriptionModel = new DescriptionModel();

                if (!String.IsNullOrEmpty(Config.GlobalParameters.GlobalSettings.PUBLIC_DESCRIPTION_FORMAT))
                {
                    descriptionModel.PublicDescriptionModel = _descriptionHelper.GetDynamicPublicDescription(payment, Config.GlobalParameters.GlobalSettings.PUBLIC_DESCRIPTION_FORMAT, 51);
                }

                if (!String.IsNullOrEmpty(Config.GlobalParameters.GlobalSettings.LINE_DESCRIPTION_FORMAT))
                {
                    descriptionModel.LineDescription = _descriptionHelper.GetDynamicLineDescription(payment, Config.GlobalParameters.GlobalSettings.LINE_DESCRIPTION_FORMAT, 251);
                }

                bool isTransferVirmanFiche = false;

                switch (Config.GlobalParameters.GlobalSettings.RECEIPT_TYPE_TO_BE_TRANSFERRED)
                {
                    case ReceiptTypeToBeTransferred.CreditCard:
                        isTransferVirmanFiche = false;
                        break;
                    case ReceiptTypeToBeTransferred.TransferFiche:
                        isTransferVirmanFiche = true;
                        break;
                    case ReceiptTypeToBeTransferred.All:
                        isTransferVirmanFiche = payment.BankDetails != null && !String.IsNullOrEmpty(payment.BankDetails.BankVPosCode);
                        break;
                    default:
                        break;
                }

                if (isTransferVirmanFiche)
                {
                    var customerAccountCode = ServiceHelper.GetCustomerCodeFromPayment(payment);
                    var customerAccount = GetCustomer(payment.AccountErpCode, payment.Agent);

                    if (customerAccount == null)
                    {
                        throw new Exception($"{payment.PaymentId} id'li ödemeye ait DİAda kayıtlı cari bulunamadığı için aktarımı yapılmadı. Cari Kod: {customerAccountCode} Ödeme Id: {payment.PaymentId}");
                    }

                    var opponentCustomerCode = payment.BankDetails?.BankVPosCode ?? string.Empty;

                    var opponentCustomer = _batchDataManager.GetCustomerByCode(opponentCustomerCode);

                    //var opponentCustomer = _paymentRepository.GetCustomerByErpCode(opponentCustomerCode);

                    if (opponentCustomer == null)
                    {
                        throw new Exception($"{opponentCustomerCode} kodlu karşı cari hesap kaydı DİA'da bulunamadı.");
                    }

                    var customerVirmanFicheParameters = new CustomerVirmanFicheParameters()
                    {
                        Payment = payment,
                        VoucherNumber = voucherNumber,
                        Customer = customerAccount,
                        OpponentCustomer = opponentCustomer,
                        DynamicFields = dynamicFieldValues,
                        DescriptionModel = descriptionModel
                    };

                    CreateVirmanFiche(customerVirmanFicheParameters);
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(payment.VPosERPCode))
                        Logging.AddLog($"Ödeme içerisinde banka hesap kodu bulunamadı! Alan adı: VPosERPCode");

                    var bankAccount = _batchDataManager.GetBankAccount(payment.VPosERPCode);
                    if (bankAccount == null || bankAccount == 0)
                        throw new Exception($"DİA veritabanında banka hesabı bulunamadı! Banka Hesap Kodu: {payment.VPosERPCode}");

                    var rePayPlanCode = String.Empty;

                    if (payment.Agent.Code == "Üyeliksiz İşlem")
                    {
                        if (Config.GlobalParameters.GlobalSettings.NONCUSTOMER_PAYMENT_TYPE == NonCustomerPayment.Nothing)
                        {
                            throw new Exception($"İşlem tipi üyeliksiz işlem olduğundan işlem yapılamadı!");
                        }
                        else if (Config.GlobalParameters.GlobalSettings.NONCUSTOMER_PAYMENT_TYPE == NonCustomerPayment.UseBagAccount)
                        {
                            var customer = _batchDataManager.GetCustomerByCode(Config.GlobalParameters.GlobalSettings.NONCUSTOMER_BAGACCOUNT);

                            if (customer == null)
                                throw new Exception($"Torba hesaba ait cari bulunamadı için aktarım yapılamadı. Torba Hesap No: {Config.GlobalParameters.GlobalSettings.NONCUSTOMER_BAGACCOUNT}");

                            CreateCreditCardFiche(payment, payment.VPosERPCode, customer, bankAccount.Value, rePayPlanCode, dynamicFields: dynamicFieldValues, descriptionModel: descriptionModel, voucherNumber: voucherNumber);

                            Logging.AddLog($"Kredi kartı fişi '{Config.GlobalParameters.GlobalSettings.NONCUSTOMER_PAYMENT_TYPE}' torba hesabına aktarıldı.");
                        }
                    }
                    else
                    {
                        var currentAccountCode = String.Empty;
                        var currentAccount = GetCustomer(payment.AccountErpCode, payment.Agent);

                        if (currentAccount == null || String.IsNullOrEmpty(currentAccount.Code))
                        {
                            throw new Exception($"Cari bilgisi DİA veritabanında bulunamadı! Cari Kod: {payment.AccountErpCode}");
                        }

                        //Ödeme yapılana sanal posun tipini alırız. Netahsilat posu ise Virman fişi kesilecek.
                        int vposApiTypeId = ServiceHelper.GetVPosApiTypeIdByPosId(payment.VPosId, posList);

                        //Ödemeden gelen sanal PosId sine göre posList dizimizden Pos ' un Erp Kodu alınır.
                        string vPosErpCode = ServiceHelper.GetVposErpCodeByPosId(payment.VPosId, posList);

                        CreateCreditCardFiche(payment, vPosErpCode, currentAccount, bankAccount.Value, rePayPlanCode, voucherNumber, dynamicFieldValues, descriptionModel);
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logging.AddLog(ex.Message + " - (InsertPayment)");
            }
            finally
            {
                try
                {
                    if (!isSuccess)
                        voucherNumber = null;

                    var execResult = _erpWebService.SetPaymentERPCode(_authenticationInfo, payment.PaymentId, true, voucherNumber, "");

                    if (!execResult.IsSuccess)
                        Logging.AddLog($"Cari Hesap Fiş Numarası: {voucherNumber}, Ödeme Id: {payment.PaymentId}, Hata: {execResult?.ErrorCode} - {execResult?.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"Erp kod güncelleme sırasında hata alındı. Ödeme Id: {payment.PaymentId}, Fiş No: {voucherNumber}, Hata Detayı: {ex.Message}");
                }
            }

            return isSuccess;
        }

        private decimal GetRePayPlanInfo(BankPaymentModel[] bankPaymentModels, ref string valorDays, PaymentServiceModel payment)
        {
            decimal totalComission = 0;

            for (int i = 0; i < bankPaymentModels.Length; i++)
            {
                valorDays += bankPaymentModels[i].InstallmentDate.Subtract(payment.PaymentDate).Days.ToString();
                totalComission = totalComission + (bankPaymentModels[i].ProcessAmount - bankPaymentModels[i].ProcessNetAmount);
            }

            return totalComission;
        }

        private void CreateCreditCardFiche(PaymentServiceModel payment, string vPosErpCode, CurrentAccountModel customer, long bankAccountKey, string rePayPlanCode = "", string voucherNumber = null, DynamicFieldsModel dynamicFields = null, DescriptionModel descriptionModel = null)
        {
            var paymentBackPlan = new BankPaymentBackPlan();

            if (Config.GlobalParameters.GlobalSettings.IS_TRANSFER_REPAYMENTPLAN)
            {
                paymentBackPlan.PayBackPlans = ServiceHelper.SetBankPayBackPlans(vPosErpCode);

                var applyBankPaymentBackPlan = paymentBackPlan != null && paymentBackPlan.PayBackPlans != null && paymentBackPlan.PayBackPlans.Any();

                if (applyBankPaymentBackPlan)
                {
                    string payBackPlanCode = paymentBackPlan.PayBackPlans[payment.Period];

                    if (!string.IsNullOrEmpty(payBackPlanCode))
                    {
                        if (payment.DefaultPOSUsed)
                        {
                            payBackPlanCode = paymentBackPlan.PayBackPlans[0];
                        }
                        var _params = new BaseApiRequestParams().AddFilter(vPosErpCode, "bankahesapkodu", FilterTypes.EQUAL);
                        _params.AddFilter(payBackPlanCode, "kodu", FilterTypes.EQUAL);
                        var paybackPlanListObj = DIARepository.List(DiaEndPoints.Keys.BANKPAYBACKPLAN, _params);

                        
                        if(paybackPlanListObj.Count > 0)
                        {
                            paymentBackPlan.ApplyPayBackPlan = payBackPlanCode;
                        }
                        //List<dynamic> paybankplans = ((IEnumerable<dynamic>)paybackPlanListObj)?
                        //        .Select(x =>
                        //        {
                        //            dynamic obj = new ExpandoObject();
                        //            obj.Id = x._key;
                        //            obj.Code = x.kodu;
                        //            obj.BankAccountCode = x.bankahesapkodu;
                        //            return obj;
                        //        })
                        //        .ToList() ?? new List<dynamic>();

                        //if (paybankplans.Count > 0)
                        //{
                        //    Logging.AddLog("Geri ödeme planı alındı.");
                        //}
                        else
                        {
                           Logging.AddLog($"Geri ödeme planı oluşturulamadı. Ödeme plan kodu: {payBackPlanCode}");
                        }
                    }
                    else
                        Logging.AddLog("Banka geri ödeme planı oluşturulamadı! - Eşleştirme yapılmamış.");

                }
                else
                   Logging.AddLog("Geri Ödeme Planı Olusturulamadı - Hata : Tanım Bulunamadı.");

            }

            if (bankAccountKey != null)
            {
                try
                {
                    // Firma bilgisini cache'den al
                    var firmInfoModelObj = _batchDataManager.GetFirmInfo();
                    if (firmInfoModelObj == null)
                    {
                        throw new Exception("Firma bilgisi cache'den alınamadı.");
                    }

                    string jsonString = JsonConvert.SerializeObject(firmInfoModelObj);
                    var firmInfoModel = JsonConvert.DeserializeObject<DiaFirmInfoModel>(jsonString);

                    var diaBranch = GetBranchKeyValueModel(dynamicFields.DivisionCode, firmInfoModel.Branches);

                    if (diaBranch != null)
                    {
                        var selectedBranch = firmInfoModel.Branches.FirstOrDefault(b => b.Id == diaBranch.Key);

                        if (selectedBranch != null)
                        {
                            firmInfoModel.Branches = new List<FirmBranchModel> { selectedBranch };
                        }
                        else
                        {
                            firmInfoModel.Branches = new List<FirmBranchModel>();
                        }
                    }

                    // Döviz kurlarını cache'den al
                    var exchangeRates = _batchDataManager.GetExchangeRates();
                    if (exchangeRates == null)
                    {
                        Logging.AddLog("Döviz kurları cache'den alınamadı, varsayılan değerler kullanılacak.");
                        exchangeRates = new List<DiaExchangeModel>();
                    }

                    DiaCurrencyParameterModel reportExchangeModel = null;

                    var reportingCurrency = exchangeRates?.FirstOrDefault(x => x.CurrencyId == firmInfoModel.ReportCurrency.Id);

                    if (reportingCurrency != null && reportingCurrency.Rate > 0)
                    {
                        reportExchangeModel = new DiaCurrencyParameterModel
                        {
                            Amount = payment.ProccessAmount,
                            Rate = (decimal)reportingCurrency.Rate,
                            CurrencyType = firmInfoModel.ReportCurrency.Code
                        };
                    }

                    var currencyModel = new CurrencyModel()
                    {
                        PureAmount = payment.ProccessAmount,
                        NetAmount = payment.ProccessNetAmount,
                        PureExchangeAmount = payment.CurrencyProccessAmount,
                        NetExchangeAmount = payment.CurrencyProccessNetAmount,
                        PureReportAmount = 0,
                        NetReportAmount = 0,
                        ReportRate = 0,
                        ExchangeRate = payment.ExchangeRate,
                        CurrencyType = payment.CurrencyCode
                    };

                    if (reportingCurrency != null && reportingCurrency.Rate > 0 && payment.CurrencyCode != CurrencyCode.TRY.ToString())
                    {
                        currencyModel.ReportRate = reportingCurrency.Rate;
                        currencyModel.PureReportAmount = payment.ProccessAmount / reportingCurrency.Rate;
                        currencyModel.NetReportAmount = payment.ProccessNetAmount / reportingCurrency.Rate;
                    }
                    else
                    { //işlem dövizliyse ve döviz türü ile raporla döviz türü tutuyorsa raporlama dövizine ödemedeki değerleri işlenir.
                        currencyModel.ReportRate = payment.ExchangeRate;
                        currencyModel.PureReportAmount = payment.ProccessAmount;
                        currencyModel.NetReportAmount = payment.ProccessNetAmount;
                    }


                    var creditCardFicheParameters = new CreditCardFicheParameters()
                    {
                        BankAccount = bankAccountKey,
                        BankPaymentBackPlan = paymentBackPlan,
                        Customer = customer,
                        Payment = payment,
                        RePaymentPlanCode = rePayPlanCode,
                        VoucherNumber = voucherNumber,
                        DynamicFields = dynamicFields,
                        CurrencyModel = currencyModel,
                        PaymentCommissionType = payment.ComissionType,
                        DescriptionModel = descriptionModel,
                        FirmInfoModel = firmInfoModel
                    };

                    var postParams = new BaseApiRequestParams();
                    postParams.Kart = CreateCurrentAccountFiche(creditCardFicheParameters);
                    var result = DIARepository.Post(DiaEndPoints.Keys.CURRENTACCOUNTFICHE, postParams);



                    if (!result.IsSuccess)
                    {
                        throw new Exception($"Kredi kartı fişi eklenemedi({result.Message}). Ödeme Referans Kodu: {payment.ReferenceCode} Ödeme Id: {payment.PaymentId}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Kredi kartı fişi eklenemedi. Ödeme Referans Kodu: {payment.ReferenceCode} Ödeme Id: {payment.PaymentId} " +
                        $"Cari Hesap Fiş Numarası : {voucherNumber} Hata Detayı: {ex.Message}");
                }
            }
            else
            {
                Logging.AddLog($"Hata : {vPosErpCode} kodlu sanal pos'a ait bir banka hesabı DİA da bulunamadı.");
            }
        }

        private dynamic CreateCurrentAccountFiche(CreditCardFicheParameters creditCardFicheParameters)
        {
            dynamic model = new ExpandoObject();

            model._key_scf_malzeme_baglantisi = 0;
            model._key_scf_odeme_plani = 0;
            model._key_sis_ozelkod = 0;
            model._key_sis_seviyekodu = 0;
            if(creditCardFicheParameters.FirmInfoModel.Branches.Count > 0)
            {
                model._key_sis_sube = new ExpandoObject();
                model._key_sis_sube.subekodu = creditCardFicheParameters.FirmInfoModel.Branches[0].Code;
            }
            else
                model._key_sis_sube = 0;
            model.aciklama1 = creditCardFicheParameters.DescriptionModel.PublicDescriptionModel.Description1 ?? "";
            model.aciklama2 = "";
            model.aciklama3 = "";
            model.belgeno = "";
            model.fisno = creditCardFicheParameters.VoucherNumber;

            dynamic kalem = new ExpandoObject();
            kalem._key_bcs_bankahesabi = creditCardFicheParameters.BankAccount;
            kalem._key_muh_masrafmerkezi = 0;
            kalem._key_prj_proje = 0;
            if (!string.IsNullOrEmpty(creditCardFicheParameters.BankPaymentBackPlan.ApplyPayBackPlan))
            {
                kalem._key_scf_banka_odeme_plani = new ExpandoObject();
                kalem._key_scf_banka_odeme_plani.kodu = creditCardFicheParameters.BankPaymentBackPlan.ApplyPayBackPlan;
            }
            else
                kalem._key_scf_banka_odeme_plani = 0;
            kalem._key_scf_carikart = new ExpandoObject();
            kalem._key_scf_carikart.carikartkodu = creditCardFicheParameters.Customer.Code;
            kalem._key_scf_odeme_plani = 0;
            kalem._key_scf_satiselemani = 0;
            kalem._key_shy_servisformu = 0;
            kalem._key_sis_doviz = new ExpandoObject();
            kalem._key_sis_doviz.adi = creditCardFicheParameters.Payment.CurrencyCode ?? "TL";
            kalem._key_sis_doviz_raporlama = new ExpandoObject();
            kalem._key_sis_doviz_raporlama.adi = creditCardFicheParameters.FirmInfoModel.ReportCurrency.Code;
            kalem._key_sis_ozelkod = 0;
            kalem.aciklama = creditCardFicheParameters.DescriptionModel.LineDescription ?? "";
            if (creditCardFicheParameters.Payment.Reversals != null)
                kalem.borc = creditCardFicheParameters.Payment.ProccessAmount;
            else
                kalem.alacak = creditCardFicheParameters.Payment.ProccessAmount;
            kalem.dovizkuru = creditCardFicheParameters.CurrencyModel.ExchangeRate;
            kalem.kurfarkialacak = "0.00";
            kalem.kurfarkiborc = "0.00";
            kalem.raporlamadovizkuru = creditCardFicheParameters.CurrencyModel.ReportRate;
            kalem.vade = creditCardFicheParameters.Payment.PaymentDate.ToString("yyyy-MM-dd");

            model.m_kalemler = new List<dynamic> { kalem };

            model.saat = creditCardFicheParameters.Payment.PaymentDate.ToString("HH:mm:ss");
            model.tarih = creditCardFicheParameters.Payment.PaymentDate.ToString("yyyy-MM-dd");
            model.turu = creditCardFicheParameters.Payment.Reversals != null ? "KI" : "KK";

            return model;
        }

        private dynamic CreateCustomerVirmanFiche(CustomerVirmanFicheParameters creditCardFicheParameters)
        {
            dynamic model = new ExpandoObject();

            model._key_scf_malzeme_baglantisi = 0;
            model._key_scf_odeme_plani = 0;
            model._key_sis_ozelkod = 0;
            model._key_sis_seviyekodu = 0;
            model._key_sis_sube = new ExpandoObject();
            model._key_sis_sube.subekodu = "";
            model.aciklama1 = creditCardFicheParameters.DescriptionModel.PublicDescriptionModel;
            model.aciklama2 = "";
            model.aciklama3 = "";
            model.belgeno = "";
            model.fisno = creditCardFicheParameters.Payment.ErpCode;

            dynamic kalem = new ExpandoObject();
            kalem._key_bcs_bankahesabi = 0;
            kalem._key_muh_masrafmerkezi = 0;
            kalem._key_ote_rezervasyonkarti = 0;
            kalem._key_prj_proje = 0;
            kalem._key_scf_banka_odeme_plani = new ExpandoObject();
            kalem._key_scf_banka_odeme_plani.kodu = 0;
            kalem._key_scf_carikart = new ExpandoObject();
            kalem._key_scf_carikart.carikartkodu = creditCardFicheParameters.Customer.Id;
            kalem._key_scf_odeme_plani = 0;
            kalem._key_scf_satiselemani = 0;
            kalem._key_shy_servisformu = 0;
            kalem._key_sis_doviz = new ExpandoObject();
            kalem._key_sis_doviz.adi = "TL";
            kalem._key_sis_doviz_raporlama = new ExpandoObject();
            kalem._key_sis_doviz_raporlama.adi = "TL";
            kalem._key_sis_ozelkod = 0;
            kalem.aciklama = "";
            kalem.alacak = creditCardFicheParameters.Payment.ProccessAmount;
            kalem.dovizkuru = 1;
            kalem.kurfarkialacak = "0.00";
            kalem.kurfarkiborc = "0.00";
            kalem.raporlamadovizkuru = 1;
            kalem.vade = creditCardFicheParameters.Payment.PaymentDate;

            model.m_kalemler = new List<dynamic> { kalem };


            dynamic kalem2 = new ExpandoObject();
            kalem2._key_bcs_bankahesabi = 0;
            kalem2._key_muh_masrafmerkezi = 0;
            kalem2._key_ote_rezervasyonkarti = 0;
            kalem2._key_prj_proje = 0;
            kalem2._key_scf_banka_odeme_plani = new ExpandoObject();
            kalem2._key_scf_banka_odeme_plani.kodu = 0;
            kalem2._key_scf_carikart = new ExpandoObject();
            kalem2._key_scf_carikart.carikartkodu = creditCardFicheParameters.OpponentCustomer.Id;
            kalem2._key_scf_odeme_plani = 0;
            kalem2._key_scf_satiselemani = 0;
            kalem2._key_shy_servisformu = 0;
            kalem2._key_sis_doviz = new ExpandoObject();
            kalem2._key_sis_doviz.adi = "TL";
            kalem2._key_sis_doviz_raporlama = new ExpandoObject();
            kalem2._key_sis_doviz_raporlama.adi = "TL";
            kalem2._key_sis_ozelkod = 0;
            kalem2.aciklama = "";
            kalem2.borc = creditCardFicheParameters.Payment.ProccessAmount;
            kalem2.dovizkuru = 1;
            kalem2.kurfarkialacak = "0.00";
            kalem2.kurfarkiborc = "0.00";
            kalem2.raporlamadovizkuru = 1;
            kalem2.vade = creditCardFicheParameters.Payment.PaymentDate;
            model.m_kalemler.Add(kalem2);

            model.saat = creditCardFicheParameters.Payment.PaymentDate.ToString("HH:mm:ss");
            model.tarih = creditCardFicheParameters.Payment.PaymentDate.ToString("yyyy-MM-dd");
            model.turu = "VF";

            return model;
        }

        private void CreateVirmanFiche(CustomerVirmanFicheParameters parameters)
        {
            try
            {
                var _params = new BaseApiRequestParams();
                _params.Kart = CreateCustomerVirmanFiche(parameters);
                int logicalRef = DIARepository.Post(DiaEndPoints.Keys.CURRENTACCOUNTFICHE, _params).Result;

                if (logicalRef < 0)
                {
                    throw new Exception($"Cari virman fişi eklenemedi. Ödeme Referans Kodu: {parameters.Payment.ReferenceCode} Ödeme Id: {parameters.Payment.PaymentId}");
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"Cari virman fişi eklenemedi. Ödeme Referans Kodu: {parameters.Payment.ReferenceCode} Ödeme Id: {parameters.Payment.PaymentId} " +
                    $"Cari Hesap Fiş Numarası : {parameters.VoucherNumber} Hata Detayı: {ex.Message}");

                throw new Exception($"Cari virman fişi eklenemedi. Ödeme Referans Kodu: {parameters.Payment.ReferenceCode} Ödeme Id: {parameters.Payment.PaymentId} " +
                    $"Cari Hesap Fiş Numarası : {parameters.VoucherNumber} Hata Detayı: {ex.Message}");
            }
        }

        private decimal CalcBankPaymentCommission(BankPaymentModel[] bankPaymentModels)
        {
            decimal comm = 0;

            foreach (BankPaymentModel item in bankPaymentModels)
            {
                comm += (item.ProcessAmount - item.ProcessNetAmount);
            }

            return comm;
        }


        private int GetReversals(ReversalServiceModelListResult reversalsResult, VirtualPosListResult virtualPoses)
        {
            if (reversalsResult.Reversals != null && reversalsResult.Reversals.Any())
            {
                // Her reversal için yeni voucher number al
                var _params = new BaseApiRequestParams()
                {
                    ColumnName = "fisno",
                    TableName = "scf_carihesap_fisi",
                    TemplateType = "CRHSP_FIS_FISNO",
                };
                var reversalFicheNumbers = DIARepository.Get(DiaEndPoints.Keys.VOUCHERNUMBER, _params);

                var nonExistsReversals = reversalsResult.Reversals
                      .Where(x => string.IsNullOrEmpty(x.ErpCode)).ToList();

                if (nonExistsReversals != null && nonExistsReversals.Any())
                {
                    Logging.AddLog("İade işlemleri başladı. İade Sayısı :" + nonExistsReversals.Count());

                    // Batch preload reversals
                    _batchDataManager.PreloadBatchDataAsync(nonExistsReversals.Select(r => r.Payment).ToList(), virtualPoses).Wait();

                    foreach (ReversalServiceModel reversal in nonExistsReversals.OrderBy(x => x.ErpFirmCode != null ? x.ErpFirmCode : x.PaymentId))
                    {
                        reversal.ErpCode = reversalFicheNumbers?.kod?.ToString();
                        InsertReversal(reversal, virtualPoses);
                    }

                    _batchDataManager.ClearCache();

                    Logging.AddLog("İade işlemleri bitti.");

                    return nonExistsReversals.Count();
                }
                else
                {
                    Logging.AddLog("Aktarımı yapılmamış iade işlemi tespit edilemedi");
                }
            }
            else
            {
                Logging.AddLog("Entegrasyonu yapılmamış iade tespit edilmedi. - (Payment)");
            }

            return 0;
        }

        public bool InsertReversal(ReversalServiceModel reversal, VirtualPosListResult posList)
        {
            var voucherNumber = reversal.ErpCode;
            bool isSuccess = true;

            try
            {
                Logging.AddLog($"Ödeme Id: {reversal.PaymentId} Referans Kodu: {reversal.Payment.ReferenceCode}");

                if (reversal.Payment.Reversals != null && reversal.Payment.Reversals.Any())
                {
                    if (reversal.Payment.Reversals.Any(x => x.ReversalType == 50))
                    {
                        throw new Exception($"Ödeme durumu iptal olduğu için aktarım işlemi durduruldu. Ödeme Id: {reversal.PaymentId}");
                    }
                }

                if (!posList.VirtualPoses.Any(i => i.VPosId == reversal.Payment.VPosId))
                {
                    throw new Exception($"Pos pasif olduğu için aktarım yapılamadı. Sanal Pos:  {reversal.Payment.VPosId}-{reversal.VPosERPCode}");
                }

                Logging.AddLog("İptal mi kontrolü yapıldı.");


                var _params = new BaseApiRequestParams().AddFilter(voucherNumber, "fisno", FilterTypes.EQUAL);
                var isExistsPayment = (List<dynamic>)DIARepository.List(DiaEndPoints.Keys.CURRENTACCOUNTFICHE, _params).Result;
                //var isExistsPayment = _paymentRepository.IsExistPayment(voucherNumber);

                if (isExistsPayment.Count > 0)
                {
                    Logging.AddLog($"'{voucherNumber}' fiş numaralı kredi kartı fişi DİA'da olduğundan işlem yapılamadı.");
                    return false;
                }

                var dynamicFieldValues = DynamicValuesHelper.GetAllDynamicFieldValues(reversal);

                // BatchDataManager'dan verileri al
                if (!String.IsNullOrEmpty(dynamicFieldValues.Salesman))
                {
                    var salesmanModel = _batchDataManager.GetSalesman(dynamicFieldValues.Salesman);
                    if (salesmanModel == null)
                    {
                        Logging.AddLog($"'{dynamicFieldValues.Salesman}' kodlu satış elemanı DİA'da bulunamadı.");
                        dynamicFieldValues.Salesman = String.Empty;
                    }
                }

                if (!String.IsNullOrEmpty(dynamicFieldValues.AuthCode))
                {
                    var authCode = _batchDataManager.GetAuthCode(dynamicFieldValues.AuthCode);
                    if (authCode == null)
                    {
                        Logging.AddLog($"'{dynamicFieldValues.AuthCode}' kodlu yetki DİA'da bulunamadı.");
                        dynamicFieldValues.AuthCode = String.Empty;
                    }
                }

                if (!String.IsNullOrEmpty(dynamicFieldValues.ProjectCode))
                {
                    var project = _batchDataManager.GetProjectCode(dynamicFieldValues.ProjectCode);
                    if (project == null)
                    {
                        Logging.AddLog($"'{dynamicFieldValues.ProjectCode}' kodlu proje kodu DİA'da bulunamadı.");
                        dynamicFieldValues.ProjectCode = String.Empty;
                    }
                }

                var descriptionModel = new DescriptionModel();

                if (!String.IsNullOrEmpty(Config.GlobalParameters.GlobalSettings.PUBLIC_DESCRIPTION_FORMAT))
                {
                    descriptionModel.PublicDescriptionModel = _descriptionHelper.GetDynamicPublicDescription(reversal.Payment, Config.GlobalParameters.GlobalSettings.PUBLIC_DESCRIPTION_FORMAT, 51);
                }

                if (!String.IsNullOrEmpty(Config.GlobalParameters.GlobalSettings.LINE_DESCRIPTION_FORMAT))
                {
                    descriptionModel.LineDescription = _descriptionHelper.GetDynamicLineDescription(reversal.Payment, Config.GlobalParameters.GlobalSettings.LINE_DESCRIPTION_FORMAT, 251);
                }

                bool isTransferVirmanFiche = false;

                switch (Config.GlobalParameters.GlobalSettings.RECEIPT_TYPE_TO_BE_TRANSFERRED)
                {
                    case ReceiptTypeToBeTransferred.CreditCard:
                        isTransferVirmanFiche = false;
                        break;
                    case ReceiptTypeToBeTransferred.TransferFiche:
                        isTransferVirmanFiche = true;
                        break;
                    case ReceiptTypeToBeTransferred.All:
                        isTransferVirmanFiche = reversal.Payment.BankDetails != null && !String.IsNullOrEmpty(reversal.Payment.BankDetails.BankVPosCode);
                        break;
                    default:
                        break;
                }

                if (String.IsNullOrWhiteSpace(reversal.VPosERPCode))
                    Logging.AddLog($"Ödeme içerisinde banka hesap kodu bulunamadı! Alan adı: VPosERPCode");

                var bankAccount = _batchDataManager.GetBankAccount(reversal.VPosERPCode);
                var rePayPlanCode = String.Empty;

                if (reversal.Agent.Code == "Üyeliksiz İşlem")
                {
                    if (Config.GlobalParameters.GlobalSettings.NONCUSTOMER_PAYMENT_TYPE == NonCustomerPayment.Nothing)
                    {
                        throw new Exception($"İşlem tipi üyeliksiz işlem olduğundan işlem yapılamadı!");
                    }
                    else if (Config.GlobalParameters.GlobalSettings.NONCUSTOMER_PAYMENT_TYPE == NonCustomerPayment.UseBagAccount)
                    {
                        var customer = _batchDataManager.GetCustomerByCode(Config.GlobalParameters.GlobalSettings.NONCUSTOMER_BAGACCOUNT);

                        if (customer == null)
                            throw new Exception($"Torba hesaba ait cari bulunamadı için aktarım yapılamadı. Torba Hesap No: {Config.GlobalParameters.GlobalSettings.NONCUSTOMER_BAGACCOUNT}");

                        if (bankAccount == null)
                            throw new Exception($"DİA veritabanında banka hesabı bulunamadı! Banka Hesap Kodu: {reversal.VPosERPCode}");

                        CreateCreditCardFiche(reversal.Payment, reversal.VPosERPCode, customer, bankAccount.Value, rePayPlanCode, dynamicFields: dynamicFieldValues, descriptionModel: descriptionModel, voucherNumber: voucherNumber);

                        Logging.AddLog($"Kredi kartı fişi '{Config.GlobalParameters.GlobalSettings.NONCUSTOMER_PAYMENT_TYPE}' torba hesabına aktarıldı.");
                    }
                }
                else
                {
                    var currentAccount = _batchDataManager.GetCustomer(reversal.AccountErpCode, reversal.Agent);

                    if (currentAccount == null || String.IsNullOrEmpty(currentAccount.Code))
                    {
                        throw new Exception($"Cari bilgisi DİA veritabanında bulunamadı! Cari Kod: {reversal.AccountErpCode}");
                    }

                    //Ödeme yapılana sanal posun tipini alırız. Netahsilat posu ise Virman fişi kesilecek.
                    int vposApiTypeId = ServiceHelper.GetVPosApiTypeIdByPosId(reversal.Payment.VPosId, posList);

                    //Ödemeden gelen sanal PosId sine göre posList dizimizden Pos ' un Erp Kodu alınır.
                    string vPosErpCode = ServiceHelper.GetVposErpCodeByPosId(reversal.Payment.VPosId, posList);

                    if (bankAccount == null)
                        throw new Exception($"DİA veritabanında banka hesabı bulunamadı! Banka Hesap Kodu: {reversal.VPosERPCode}");

                    CreateCreditCardFiche(reversal.Payment, vPosErpCode, currentAccount, bankAccount.Value, rePayPlanCode, voucherNumber, dynamicFieldValues, descriptionModel);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logging.AddLog(ex.Message + " - (InsertPayment)");
            }
            finally
            {
                try
                {
                    if (!isSuccess)
                        voucherNumber = null;

                    var execResult = _erpWebService.SetReversalERPCode(_authenticationInfo, reversal.PaymentId, true, voucherNumber, "");

                    if (!execResult.IsSuccess)
                        Logging.AddLog($"Cari Hesap Fiş Numarası: {voucherNumber}, Ödeme Id: {reversal.PaymentId}, Hata: {execResult?.ErrorCode} - {execResult?.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"Erp kod güncelleme sırasında hata alındı. Ödeme Id: {reversal.PaymentId}, Fiş No: {voucherNumber}, Hata Detayı: {ex.Message}");
                }
            }

            return isSuccess;
        }

        private CurrentAccountModel GetCustomer(string accountErpCode, Agent agent, string agentCode = null)
        {
            return _batchDataManager.GetCustomer(accountErpCode, agent);
        }
        public KeyValueModel GetBranchKeyValueModel(string dynamicBranchCode, List<FirmBranchModel> firmBranches)
        {
            var defaultBranch = firmBranches.FirstOrDefault(x => x.IsActive && x.IsCenter);

            if (!string.IsNullOrEmpty(dynamicBranchCode))
            {
                var branch = firmBranches.FirstOrDefault(x => x.IsActive && x.Code == dynamicBranchCode.Split('-')[0]);

                if (branch != null)
                {
                    return new KeyValueModel()
                    {
                        Key = branch.Id,
                        Value = branch.Code
                    };
                }
            }

            if (defaultBranch != null)
            {
                return new KeyValueModel()
                {
                    Key = defaultBranch.Id,
                    Value = defaultBranch.Code
                };
            }

            return null;
        }

        // Başarısız ödemeleri getirir
        public void GetUnsuccessPayments()
        {
            // Implementation will be added when needed
        }

        // Entegre edilmemiş banka ödeme detaylarını getirir
        public void GetNonIntegratedBankPaymentDetail()
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
