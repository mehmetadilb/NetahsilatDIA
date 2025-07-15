using CommonLib;
using CommonLib.Model;
using NetahsilatWebServiceLib.Accounts;
using NetahsilatWebServiceLib.Payments;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetahsilatWebServiceLib
{
    public class ServiceHandler // IDisposable eklemeyi düşünebilirsiniz (eğer yöneticiler IDisposable ise)
    {
        // Bağımlılıklar (Instance Members)
        private readonly PaymentManager _paymentManager;
        private readonly AccountManager _accountManager;

        // Görev Kuyruğu (Thread-Safe)
        private readonly ConcurrentQueue<ServiceTask> _taskQueue;

        // Yapılandırma Bayrakları (ServiceStart'ta ayarlanır)
        private bool _runPaymentService = false;
        private bool _runAccountService = false;
        private bool _runVendorService = false;
        private bool _setReversals = false; // Global.SetReversals karşılığı

        public ServiceHandler()
        {
            // Bağımlılıkları ve kuyruğu başlat
            _paymentManager = new PaymentManager(); // Her zaman oluştur
            _accountManager = new AccountManager(); // Her zaman oluştur
            _taskQueue = new ConcurrentQueue<ServiceTask>();
        }

        /// <summary>
        /// Servis başlatıldığında gerekli başlatma işlemlerini yapar ve yapılandırmayı okur.
        /// </summary>
        /// <returns>Servisin çalışma parametrelerini içeren yanıt nesnesi.</returns>
        public ServiceStartResponse ServiceStart()
        {
            var response = new ServiceStartResponse();

            // Yapılandırmayı oku ve bayrakları ayarla (Global erişimi hala burada)
            _runPaymentService = !string.IsNullOrWhiteSpace(Config.GlobalParameters.Parameters.ERP_SERVICE);
            _runAccountService = !string.IsNullOrWhiteSpace(Config.GlobalParameters.Parameters.ACCOUNT_SERVICE);
            _runVendorService = !string.IsNullOrWhiteSpace(Config.GlobalParameters.Parameters.VENDOR_SERVICE);
            _setReversals = Config.GlobalParameters.GlobalSettings.SET_REVERSAL; // Global değeri al

            response.RunPayment = _runPaymentService;
            response.RunAccount = _runAccountService;
            response.RunVendor = _runVendorService;

            // Çalışma saatlerini ayrıştır (Daha okunaklı LINQ kullanımı)
            if (!string.IsNullOrWhiteSpace(Config.GlobalParameters.GlobalSettings.WORK_HOURS))
            {
                response.WorkingHours = Config.GlobalParameters.GlobalSettings.WORK_HOURS
                    .Split(',')
                    .Select(hourStr => hourStr.Trim()) // Boşlukları temizle
                    .Select(hourStr => int.TryParse(hourStr, out int hour) ? hour : (int?)null) // int'e çevir, başarısızsa null
                    .Where(hour => hour.HasValue) // Sadece başarılı olanları al
                    .Select(hour => hour.Value)   // int'e dönüştür
                    .ToList();
            }
            else
            {
                response.WorkingHours = new List<int>(); // Boş liste ata
            }

            // Zamanlayıcı intervalini ayarla
            response.TimerInterval = ServiceHelper.SetTimerInterval();

            Logging.AddLog("WinServiceManager başlatıldı. Yapılandırma okundu.");
            Logging.AddLog($"RunPayment: {response.RunPayment}, RunAccount: {response.RunAccount}, RunVendor: {response.RunVendor}, SetReversals: {_setReversals}");
            if (response.WorkingHours.Any())
                Logging.AddLog($"WorkingHours: {string.Join(",", response.WorkingHours)}");

            return response;
        }

        /// <summary>
        /// İşlenmek üzere yeni bir görevi kuyruğa ekler.
        /// </summary>
        /// <param name="serviceTask">Eklenecek görevin türü.</param>
        public void AddTask(ServiceTask serviceTask)
        {
            try
            {
                _taskQueue.Enqueue(serviceTask);
                Logging.AddLog($"Görev '{serviceTask}' kuyruğa eklendi. Kuyruk boyutu: {_taskQueue.Count}");
            }
            catch (Exception ex)
            {
                // Normalde ConcurrentQueue.Enqueue pek hata vermez ama loglamak iyidir.
                Logging.AddLog($"Görev '{serviceTask}' kuyruğa eklenirken hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// Kuyruktaki bekleyen görevleri işler.
        /// </summary>
        public void TaskWorker()
        {
            if (_taskQueue.IsEmpty)
            {
                // Sık sık loglamamak için belki bu log kaldırılabilir veya seviyesi düşürülebilir.
                // Logging.AddLog("İşlenecek görev kuyrukta bulunamadı.");
                return;
            }

            Logging.AddLog($"Görev işleyici başlatıldı. Kuyrukta {_taskQueue.Count} görev var.");

            // Kuyruktan güvenli bir şekilde görevleri çek ve işle
            while (_taskQueue.TryDequeue(out ServiceTask currentTask))
            {
                try
                {
                    Logging.AddLog($"Görev '{currentTask}' işleniyor...");

                    // Görev türüne göre ilgili metodu çağır
                    switch (currentTask)
                    {
                        case ServiceTask.NonIntegratedPayments:
                            if (_runPaymentService)
                                _paymentManager.GetNonIntegratedPayment();
                            // İptal/iade kontrolü ServiceStart'ta alınan değere göre yapılır
                            if (_runPaymentService && _setReversals)
                                _paymentManager.GetNonIntegratedReversal();
                            break;

                        case ServiceTask.UnsuccessPayments:
                            //if (_runPaymentService)
                            //    _paymentManager.GetDailyUnsuccessPayment();
                            break;

                        case ServiceTask.CurrentAccount:
                            if (_runVendorService) // Cari hesap Vendor ile ilgili
                                _accountManager.SendAccount();
                            break;

                        case ServiceTask.UnsuccessCurrentAccount:
                            if (_runVendorService)
                                _accountManager.SendAccount(true);
                            break;

                        case ServiceTask.CurrentAccountTransaction:
                            if (_runAccountService) // Ekstre Account ile ilgili
                            {
                                //_accountManager.SendAccountTransaction();
                                //_accountManager.SendClosedAccountTransaction();
                            }
                            break;

                        case ServiceTask.UnsuccessCurrentAccountTransaction:
                            if (_runAccountService)
                                _accountManager.SendAccountTransactionDaily();
                            break;

                        case ServiceTask.NonIntegratedBankPaymentDetail:
                            // Bu görevin hangi bayrağa bağlı olduğu orijinal kodda belirsiz, Payment varsayılıyor.
                            //if (_runPaymentService)
                            //    _paymentManager.GetNonIntegratedBankPaymentDetail();
                            break;

                        case ServiceTask.UpdateMaxInstallments:
                            // Bu görevin hangi bayrağa bağlı olduğu orijinal kodda belirsiz, Account varsayılıyor.
                            //if (_runAccountService)
                            //    _accountManager.UpdateMaxInstallment();
                            break;

                        default:
                            Logging.AddLog($"Tanımsız veya işlenmeyen görev türü algılandı: {currentTask}");
                            break;
                    }

                    Logging.AddLog($"Görev '{currentTask}' başarıyla işlendi (veya ilgili servis kapalıydı).");
                }
                catch (Exception ex)
                {
                    // Görevi işlerken bir hata oluştu, logla ve sonraki göreve geç.
                    // Başarısız görevlerin tekrar denenmesi gerekiyorsa, buraya ek mantık eklenebilir
                    // (örn. başka bir kuyruğa atma veya belirli sayıda deneme).
                    Logging.AddLog($"Görev '{currentTask}' işlenirken KRİTİK HATA oluştu: {ex}"); // Hatanın detayını logla
                }
            }

            Logging.AddLog("Görev işleyici mevcut görevleri tamamladı.");
            // ApplicationStop() burada ÇAĞRILMAMALI.
        }

        /// <summary>
        /// Servis durdurulduğunda çağrılacak temizleme işlemlerini yapar.
        /// </summary>
        public void StopService()
        {
            Logging.AddLog("WinServiceManager durduruluyor...");

            // Kuyruktaki bekleyen görevleri temizle (opsiyonel, gereksinime göre)
            int remainingTasks = _taskQueue.Count;
            if (remainingTasks > 0)
            {
                Logging.AddLog($"Servis durdurulurken kuyrukta bekleyen {remainingTasks} görev iptal ediliyor.");
                // Kuyruğu boşaltmak için:
                while (_taskQueue.TryDequeue(out _)) { }
            }

            Logging.AddLog("WinServiceManager durduruldu.");
        }
    }
}
