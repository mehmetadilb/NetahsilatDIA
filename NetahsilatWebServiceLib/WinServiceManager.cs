using CommonLib;
using CommonLib.Model;
using Netahsilat.DIAService;
using NetahsilatWebServiceLib.Accounts;
using NetahsilatWebServiceLib.Payments;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Netahsilat.DIAService.ConfigHelper;

namespace NetahsilatWebServiceLib
{
    public class WinServiceManager : IDisposable
    {
        private PaymentManager _paymentManager;
        private AccountManager _accountManager;

        private readonly ConcurrentQueue<ServiceTask> _taskQueue;

        private bool _runPaymentService = false;
        private bool _runAccountService = false;
        private bool _runVendorService = false;
        private bool _setReversals = false;

        private bool _disposed = false;

        public WinServiceManager()
        {
            _taskQueue = new ConcurrentQueue<ServiceTask>();
        }

        public ServiceStartResponse ServiceStart()
        {
            Logging.AddLog("WinServiceManager ServiceStart called.");

            var response = new ServiceStartResponse();

            try
            {
                LoadParameters();

                _runPaymentService = !string.IsNullOrWhiteSpace(Config.GlobalParameters.Parameters.ERP_SERVICE);
                _runAccountService = !string.IsNullOrWhiteSpace(Config.GlobalParameters.Parameters.ACCOUNT_SERVICE);
                _runVendorService = !string.IsNullOrWhiteSpace(Config.GlobalParameters.Parameters.VENDOR_SERVICE);
                _setReversals = Config.GlobalParameters.GlobalSettings.SET_REVERSAL;

                if (_runPaymentService)
                {
                    _paymentManager = new PaymentManager();
                    response.RunPayment = _runPaymentService;
                }
                if (_runAccountService || _runVendorService)
                {
                    _accountManager = new AccountManager();
                    response.RunAccount = _runAccountService;
                    response.RunVendor = _runVendorService;
                }

                Logging.AddLog($"Configuration Read: RunPayment={_runPaymentService}, RunAccount={_runAccountService}, RunVendor={_runVendorService}, SetReversals={_setReversals}");

                response.TimerInterval = ServiceHelper.SetTimerInterval();
                Logging.AddLog($"Timer Interval Calculated: {response.TimerInterval} ms");

                ValidateFirmAndParameters();

                var firm = Config.GlobalParameters.Parameters.Firms.First();
                var parameters = Config.GlobalParameters.Parameters;

                var firmDto = new DiaFirm
                {
                    Company = firm.Company,
                    Period = firm.Period,
                    DiaUserName = firm.DiaUserName,
                    DiaPassword = firm.DiaPassword,
                    DiaLang = firm.DiaLang,
                    DisconnectSameUser = firm.DisconnectSameUser,
                    DiaApiKey = firm.DiaToken,
                };

                var paramDto = new DiaParameters
                {
                    DiaBaseUrl = parameters.DiaBaseUrl,
                    DiaApiEndpoint = parameters.DiaApiEndpoint,
                    DiaSessionErrorMessageHint = parameters.DiaSessionErrorMessageHint
                };

                ConfigHelper.Init(paramDto, firmDto);
            }
            catch (Exception ex)
            {
                Logging.AddLog($"ERROR during ServiceStart initialization: {ex}");
            }

            Logging.AddLog("WinServiceManager ServiceStart finished.");
            return response;
        }

        private void LoadParameters()
        {
            try
            {
                JsonDbManager.LoadParameters();
            }
            catch (Exception ex)
            {
                throw new Exception($"Parametreler yüklenirken hata oluştu: {ex.Message}");
            }
        }

        private void ValidateFirmAndParameters()
        {
            var firm = Config.GlobalParameters?.Parameters?.Firms?.FirstOrDefault();
            var parameters = Config.GlobalParameters?.Parameters;

            if (firm == null || parameters == null)
            {
                throw new Exception("Konfigürasyon eksik! Lütfen ayarları kontrol edin.");
            }
        }

        public void AddTask(ServiceTask serviceTask)
        {
            try
            {
                _taskQueue.Enqueue(serviceTask);
                Logging.AddLog($"Task '{serviceTask}' added to the queue. Current queue size: {_taskQueue.Count}");
            }
            catch (Exception ex)
            {
                Logging.AddLog($"ERROR adding task '{serviceTask}' to the queue: {ex.Message}");
            }
        }

        public void TaskWorker()
        {
            if (_taskQueue.IsEmpty)
            {
                return;
            }

            Logging.AddLog($"TaskWorker starting. Processing {_taskQueue.Count} tasks.");

            while (_taskQueue.TryDequeue(out ServiceTask currentTask))
            {
                try
                {
                    Logging.AddLog($"Processing task: '{currentTask}'...");

                    switch (currentTask)
                    {
                        case ServiceTask.NonIntegratedPayments:
                            if (_runPaymentService && _paymentManager != null)
                            {
                                _paymentManager.GetNonIntegratedPayment();
                                if (_setReversals)
                                {
                                    _paymentManager.GetNonIntegratedReversal();
                                }
                            }
                            else 
                            {
                                Logging.AddLog($"Skipping '{currentTask}': Payment Service is disabled or PaymentManager is null.");
                            }
                            break;

                        case ServiceTask.UnsuccessPayments:
                            if (_runPaymentService && _paymentManager != null)
                            {
                                _paymentManager.GetUnsuccessPayments();
                            }
                            else 
                            {
                                Logging.AddLog($"Skipping '{currentTask}': Payment Service is disabled or PaymentManager is null.");
                            }
                            break;

                        case ServiceTask.CurrentAccount:
                            if (_runVendorService && _accountManager != null)
                            {
                                _accountManager.SendAccount();
                            }
                            else 
                            {
                                Logging.AddLog($"Skipping '{currentTask}': Vendor Service is disabled or AccountManager is null.");
                            }
                            break;

                        case ServiceTask.UnsuccessCurrentAccount:
                            if (_runVendorService && _accountManager != null)
                            {
                                _accountManager.SendAccount(isDaily: true);
                            }
                            else 
                            {
                                Logging.AddLog($"Skipping '{currentTask}': Vendor Service is disabled or AccountManager is null.");
                            }
                            break;

                        case ServiceTask.CurrentAccountTransaction:
                            if (_runAccountService && _accountManager != null)
                            {
                                _accountManager.SendAccountTransaction();
                            }
                            else 
                            {
                                Logging.AddLog($"Skipping '{currentTask}': Account Service is disabled or AccountManager is null.");
                            }
                            break;

                        case ServiceTask.UnsuccessCurrentAccountTransaction:
                            if (_runAccountService && _accountManager != null)
                            {
                                _accountManager.SendAccountTransactionDaily();
                            }
                            else 
                            {
                                Logging.AddLog($"Skipping '{currentTask}': Account Service is disabled or AccountManager is null.");
                            }
                            break;

                        case ServiceTask.NonIntegratedBankPaymentDetail:
                            if (_runPaymentService && _paymentManager != null)
                            {
                                _paymentManager.GetNonIntegratedBankPaymentDetail();
                            }
                            else 
                            {
                                Logging.AddLog($"Skipping '{currentTask}': Payment Service is disabled or PaymentManager is null.");
                            }
                            break;

                        case ServiceTask.UpdateMaxInstallments:
                            if (_runAccountService && _accountManager != null)
                            {
                                _accountManager.UpdateMaxInstallments();
                            }
                            else 
                            {
                                Logging.AddLog($"Skipping '{currentTask}': Account Service is disabled or AccountManager is null.");
                            }
                            break;

                        default:
                            Logging.AddLog($"WARNING: Unknown or unhandled task type encountered: '{currentTask}'");
                            break;
                    }

                    Logging.AddLog($"Finished processing task: '{currentTask}'.");
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"ERROR processing task '{currentTask}': {ex}");
                }
            }

            Logging.AddLog("TaskWorker finished processing available tasks.");
        }

        public void StopService()
        {
            Logging.AddLog("WinServiceManager StopService called.");

            int remainingTasks = _taskQueue.Count;
            if (remainingTasks > 0)
            {
                Logging.AddLog($"WARNING: Service stopping with {remainingTasks} tasks remaining in the queue.");
            }

            Dispose();
            Logging.AddLog("WinServiceManager StopService finished.");
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _paymentManager?.Dispose();
            _accountManager?.Dispose();

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
