using CommonLib;
using CommonLib.Model;
using Netahsilat.DIAService;
using Netahsilat.DIAService.Model;
using NetahsilatWebServiceLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace NetahsilatDIA
{
    public partial class Service1 : ServiceBase
    {
        // --- Constants ---
        private const string DefaultServiceName = "FinrotaErpIntegrationService";
        private const string DefaultEventLogName = "Application";
        private const int DailyCheckHour = 2; // Günlük görevlerin kontrol edileceği saat (0-23)
        private static readonly TimeSpan HourlyInterval = TimeSpan.FromHours(1);
        private static readonly TimeSpan DefaultCheckInterval = TimeSpan.FromMinutes(1); // Zamanlayıcının ne sıklıkla kontrol yapacağı
        private static readonly TimeSpan WorkingHourCheckGracePeriod = TimeSpan.FromMinutes(5); // Çalışma saati dışı loglamayı azaltmak için

        // --- Dependencies & Configuration ---
        private WinServiceManager _winServiceManager;
        private ServiceStartResponse _serviceStartResponse;

        // --- Timer & State ---
        private Timer _mainTimer;
        private readonly object _runTaskLock = new object(); // TaskWorker çağrılarını senkronize etmek için
        private TimeSpan _configuredInterval = TimeSpan.Zero; // Yapılandırmadan gelen interval
        private DateTime _lastPaymentCheck = DateTime.MinValue;
        private DateTime _lastTransactionCheck = DateTime.MinValue;
        private DateTime _lastVendorCheck = DateTime.MinValue;
        private DateTime _lastDailyCheck = DateTime.MinValue;
        private DateTime _lastWorkingHourLog = DateTime.MinValue; // Çalışma saati dışı loglamayı sınırlamak için

        public Service1()
        {
            InitializeComponent();

            this.ServiceName = DefaultServiceName;

            // ServiceBase özellikleri
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Logging.AddLog($"Servis başlatılıyor... Windows Kullanıcı Adı: {WindowsIdentity.GetCurrent().Name}");

                _winServiceManager = new WinServiceManager();
                _serviceStartResponse = _winServiceManager.ServiceStart();

                if (_serviceStartResponse == null)
                {
                    throw new InvalidOperationException("ServiceStart yanıtı (konfigürasyon) alınamadı.");
                }

                // Zamanlayıcı intervalini belirle
                _configuredInterval = TimeSpan.FromMilliseconds(_serviceStartResponse.TimerInterval);
                Logging.AddLog($"Yapılandırılmış ana interval: {_configuredInterval.TotalMinutes} dakika.");

                if (_serviceStartResponse.WorkingHours != null && _serviceStartResponse.WorkingHours.Any())
                {
                    Logging.AddLog($"Çalışma saatleri aktif: {string.Join(",", _serviceStartResponse.WorkingHours.Select(h => h.ToString("00") + ":00"))}");
                }

                // Ana zamanlayıcıyı başlat
                _mainTimer = new Timer(MainTimerCallback, null, TimeSpan.Zero, DefaultCheckInterval);

                Logging.AddLog($"'{ServiceName}' servisi başarıyla başlatıldı.");
            }
            catch (Exception ex)
            {
                var errorMessage = $"Servis başlatılamadı! Hata Detayı: {ex}";
                Logging.AddLog(errorMessage);
                Stop();
            }
        }

        protected override void OnStop()
        {
            Logging.AddLog($"'{ServiceName}' servisi durduruluyor...");

            // Zamanlayıcıyı durdur ve kaynakları serbest bırak
            _mainTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _mainTimer?.Dispose();
            _mainTimer = null;

            try
            {
                _winServiceManager?.StopService();
            }
            catch (Exception ex)
            {
                Logging.AddLog($"WinServiceManager durdurulurken hata oluştu: {ex}");
            }

            Logging.AddLog($"'{ServiceName}' servisi durduruldu.");
        }

        // Debug modunda çalıştırmak için
        internal void OnDebug()
        {
            Logging.AddLog("Servis DEBUG modunda başlatılıyor...");
            
            // Servisi normal şekilde başlat
            OnStart(null);
            Logging.AddLog("DEBUG modunda servis başlatıldı.");

            // Test görevi ekle
            _winServiceManager.AddTask(ServiceTask.NonIntegratedPayments);
            MainTimerCallback(null);
        }

        private void MainTimerCallback(object state)
        {
            try
            {
                DateTime now = DateTime.Now;
                bool canRunTasks = true;

                // 1. Çalışma Saatlerini Kontrol Et
                if (_serviceStartResponse.WorkingHours != null && _serviceStartResponse.WorkingHours.Any())
                {
                    if (!_serviceStartResponse.WorkingHours.Contains(now.Hour))
                    {
                        canRunTasks = false;
                        // Çalışma saati dışında olduğumuzu sadece belirli aralıklarla loglayalım (spam olmasın)
                        if (now > _lastWorkingHourLog + WorkingHourCheckGracePeriod)
                        {
                            Logging.AddLog($"Çalışma saati ({now:HH:mm}) dışında. Görevler bekletiliyor. Aktif saatler: {string.Join(",", _serviceStartResponse.WorkingHours.Select(h => h.ToString("00") + ":00"))}");
                            _lastWorkingHourLog = now;
                        }
                    }
                    else
                    {
                        // Çalışma saatine geri dönüldüyse loglayalım
                        if (_lastWorkingHourLog != DateTime.MinValue)
                        {
                            Logging.AddLog($"Çalışma saatine ({now:HH:mm}) girildi. Görevler kontrol edilecek.");
                            _lastWorkingHourLog = DateTime.MinValue; // Tekrar loglamamak için sıfırla
                        }
                    }
                }

                // Çalışma saatleri uygunsa veya tanımlı değilse görevleri kontrol et
                if (canRunTasks)
                {
                    bool tasksAdded = false;

                    // Görevleri Zaman Aralıklarına Göre Ekle
                    TimeSpan effectiveInterval = (_serviceStartResponse.WorkingHours != null && _serviceStartResponse.WorkingHours.Any())
                                                 ? HourlyInterval
                                                 : _configuredInterval;

                    // Ödeme görevi
                    if (_serviceStartResponse.RunPayment && now >= _lastPaymentCheck + effectiveInterval)
                    {
                        _winServiceManager.AddTask(ServiceTask.NonIntegratedPayments);
                        _lastPaymentCheck = now;
                        tasksAdded = true;
                        Logging.AddLog($"Ödeme görevi (NonIntegratedPayments) eklendi. Sonraki kontrol yaklaşık: {now + effectiveInterval}");
                    }

                    // Hesap (Ekstre) görevi
                    if (_serviceStartResponse.RunAccount && now >= _lastTransactionCheck + effectiveInterval)
                    {
                        _winServiceManager.AddTask(ServiceTask.CurrentAccountTransaction);
                        _lastTransactionCheck = now;
                        tasksAdded = true;
                        Logging.AddLog($"Ekstre görevi (CurrentAccountTransaction) eklendi. Sonraki kontrol yaklaşık: {now + effectiveInterval}");
                    }

                    // Cari Hesap (Vendor) görevi
                    if (_serviceStartResponse.RunVendor && now >= _lastVendorCheck + effectiveInterval)
                    {
                        _winServiceManager.AddTask(ServiceTask.CurrentAccount);
                        _lastVendorCheck = now;
                        tasksAdded = true;
                        Logging.AddLog($"Cari Hesap görevi (CurrentAccount) eklendi. Sonraki kontrol yaklaşık: {now + effectiveInterval}");
                    }

                    // Günlük Görevleri Kontrol Et (Sadece belirli saatte ve günde bir kez)
                    if (now.Hour == DailyCheckHour && now.Date > _lastDailyCheck.Date)
                    {
                        Logging.AddLog($"Günlük görevler kontrol ediliyor (Saat: {now:HH:mm})...");
                        if (_serviceStartResponse.RunAccount)
                        {
                            _winServiceManager.AddTask(ServiceTask.UnsuccessCurrentAccountTransaction);
                            _winServiceManager.AddTask(ServiceTask.UpdateMaxInstallments);
                            tasksAdded = true;
                            Logging.AddLog("Günlük başarısız ekstre ve taksit güncelleme görevleri eklendi.");
                        }
                        if (_serviceStartResponse.RunVendor)
                        {
                            _winServiceManager.AddTask(ServiceTask.UnsuccessCurrentAccount);
                            tasksAdded = true;
                            Logging.AddLog("Günlük başarısız cari hesap görevi eklendi.");
                        }
                        if (_serviceStartResponse.RunPayment)
                        {
                            _winServiceManager.AddTask(ServiceTask.UnsuccessPayments);
                            tasksAdded = true;
                            Logging.AddLog("Günlük başarısız ödeme görevi eklendi.");
                        }
                        _lastDailyCheck = now;
                    }

                    // Eklenen görevleri çalıştırmayı dene
                    if (tasksAdded)
                    {
                        lock (_runTaskLock)
                        {
                            Logging.AddLog("Bekleyen görevler çalıştırılıyor...");
                            _winServiceManager.TaskWorker();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"Ana zamanlayıcı geri çağrısında (MainTimerCallback) hata oluştu: {ex}");
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose Timer
                _mainTimer?.Dispose();
                _mainTimer = null;

                // Dispose other managed resources
                (_winServiceManager as IDisposable)?.Dispose();

                // Dispose designer components
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        #endregion
    }
}