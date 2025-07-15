using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Netahsilat.DIAService.Model;
using System.Reflection;

namespace Netahsilat.DIAService
{
    public class DiaApiClient : IDisposable
    {
        private static readonly HttpClient _httpClient;
        private static string _sessionId = null;
        private static readonly object _sessionLock = new object(); // Thread-safe session yönetimi için
        
        private readonly string _baseApiUrl;
        private readonly string _apiEndpoint;

        static DiaApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(3);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public DiaApiClient()
        {
            _baseApiUrl = ConfigHelper.DiaBaseUrl.TrimEnd('/') + "/";
            _apiEndpoint = ConfigHelper.DiaApiEndpoint.TrimStart('/');
        }

        public static string CurrentSessionId 
        { 
            get 
            { 
                lock (_sessionLock) 
                { 
                    return _sessionId; 
                } 
            } 
        }

        /// <summary>
        /// Kontör bilgisini API'den çeker
        /// </summary>
        private decimal? GetCredit(string sessionIdToUse)
        {
            if (string.IsNullOrEmpty(sessionIdToUse)) return null;

            var request = new KontorSorgulaRequest
            {
                KontorSorgula = new KontorSorgulaData { SessionId = sessionIdToUse }
            };

            string payload = JsonConvert.SerializeObject(request);
            string url = _baseApiUrl + _apiEndpoint;

            try
            {
                using (var content = new StringContent(payload, Encoding.UTF8, "application/json"))
                using (var response = _httpClient.PostAsync(url, content).Result)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        Logging.AddLog($"Kontör sorgulama başarısız. Status Code: {response.StatusCode} Response: {responseString}");
                        return null;
                    }

                    var result = JsonConvert.DeserializeObject<KontorSorgulaResponse>(responseString);
                    return result?.Result?.Kontor;
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"Kontör sorgulama hatası. Hata: {ex.Message} StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public LoginResponse Login(bool forceNewLogin = false)
        {
            lock (_sessionLock) // Thread-safe login
            {
                try
                {
                    if (!forceNewLogin && !string.IsNullOrEmpty(_sessionId))
                    {
                        return new LoginResponse { Code = 1, Message = _sessionId };
                    }

                    _sessionId = null;

                    var loginRequest = new LoginRequest
                    {
                        Login = new LoginData
                        {
                            Username = ConfigHelper.DiaUsername,
                            Password = ConfigHelper.DiaPassword,
                            DisconnectSameUser = ConfigHelper.DiaDisconnectSameUser,
                            Lang = ConfigHelper.DiaLang,
                            Params = new LoginParams { ApiKey = ConfigHelper.DiaApiKey }
                        }
                    };

                    string payload = JsonConvert.SerializeObject(loginRequest);
                    string url = _baseApiUrl + _apiEndpoint;

                    using (var content = new StringContent(payload, Encoding.UTF8, "application/json"))
                    using (var response = _httpClient.PostAsync(url, content).Result)
                    {
                        string responseString = response.Content.ReadAsStringAsync().Result;

                        if (!response.IsSuccessStatusCode)
                        {
                            Logging.AddLog($"Login başarısız. Status Code: {response.StatusCode} Response: {responseString}");
                            return new LoginResponse { Code = 0, Message = "Login isteği başarısız." };
                        }

                        var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseString);
                        if (loginResponse != null && loginResponse.IsSuccess && !string.IsNullOrEmpty(loginResponse.Message))
                        {
                            _sessionId = loginResponse.Message;
                            Logging.AddLog($"Login başarılı Token: {loginResponse.Message}");
                            
                            // Login başarılıysa kontör sorgula ve logla
                            decimal? kredi = GetCredit(_sessionId);
                            if (kredi.HasValue)
                            {
                                Logging.AddLog($"Login sonrası kontör durumu: {kredi.Value}");
                                
                                // Kontör yetersizse uyarı ver
                                if (kredi.Value < -49)
                                {
                                    Logging.AddLog($"⚠️ UYARI: Kontör yetersiz! Mevcut kontör: {kredi.Value} (minimum -49 olması gerekiyor)");
                                }
                            }
                            else
                            {
                                Logging.AddLog("⚠️ UYARI: Kontör bilgisi alınamadı!");
                            }
                            
                            return loginResponse;
                        }

                        Logging.AddLog($"Login başarısız: {loginResponse?.Message}");
                        return loginResponse ?? new LoginResponse { Code = 0, Message = "Login cevabı deserialize edilemedi." };
                    }
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"Login sırasında hata oluştu. Hata: {ex.Message} StackTrace: {ex.StackTrace}");
                    _sessionId = null;
                    return new LoginResponse { Code = -1, Message = ex.Message };
                }
            }
        }

        public LogoutResponse Logout()
        {
            lock (_sessionLock) // Thread-safe logout
            {
                try
                {
                    if (_sessionId == null)
                        return new LogoutResponse { Code = -1, Message = "Zaten çıkış yapılmış." };

                    var logoutRequest = new LogoutRequest
                    {
                        Logout = new LogoutData { SessionId = _sessionId }
                    };

                    string payload = JsonConvert.SerializeObject(logoutRequest);
                    string url = _baseApiUrl + _apiEndpoint;

                    using (var content = new StringContent(payload, Encoding.UTF8, "application/json"))
                    using (var response = _httpClient.PostAsync(url, content))
                    {
                        string responseString = response.Result.Content.ReadAsStringAsync().Result;
                        if (!response.Result.IsSuccessStatusCode)
                        {
                            Logging.AddLog($"Logout başarısız. Status Code: {response.Result.StatusCode}, Response: {responseString}");
                            return new LogoutResponse { Code = 0, Message = "Logout isteği başarısız." };
                        }

                        var logoutResponse = JsonConvert.DeserializeObject<LogoutResponse>(responseString);
                        _sessionId = null;
                        
                        Logging.AddLog("Logout başarılı");
                        return logoutResponse;
                    }
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"Logout sırasında hata oluştu. Hata: {ex.Message} StackTrace: {ex.StackTrace}");
                    return new LogoutResponse { Code = -1, Message = ex.Message };
                }
            }
        }

        public BaseApiGetResponse SendRequest(string requestKey, BaseApiRequestParams requestPayload, string endpoint = null, bool requiresSession = true)
        {
            string targetEndpoint = string.IsNullOrEmpty(endpoint) ? _apiEndpoint : endpoint.TrimStart('/');
            string url = _baseApiUrl + targetEndpoint;

            string sessionIdUsed = null;

            if (requiresSession)
            {
                try
                {
                    lock (_sessionLock) // Thread-safe session kontrolü
                    {
                        if (string.IsNullOrEmpty(_sessionId))
                        {
                            var loginResponse = Login(forceNewLogin: true);
                            if (!loginResponse.IsSuccess || string.IsNullOrEmpty(_sessionId))
                            {
                                return new BaseApiGetResponse
                                {
                                    Code = -1,
                                    Message = "İstek öncesi otomatik login başarısız: " + loginResponse.Message
                                };
                            }
                        }

                        sessionIdUsed = _sessionId;
                    }
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"Session/Kredi kontrolü hatası. Hata: {ex.Message} StackTrace: {ex.StackTrace}");
                    return new BaseApiGetResponse
                    {
                        Code = -1,
                        Message = "Session/Kredi kontrolü hatası: " + ex.Message
                    };
                }

                var sessionIdProp = typeof(BaseApiRequestParams).GetProperty("SessionId");
                if (sessionIdProp != null && sessionIdProp.PropertyType == typeof(string))
                {
                    sessionIdProp.SetValue(requestPayload, sessionIdUsed);
                }
                else
                {
                    Logging.AddLog($"{nameof(BaseApiRequestParams)} tipi 'SessionId' property'si içermiyor.");
                }
            }

            var wrappedPayload = new Dictionary<string, BaseApiRequestParams>
            {
                { requestKey, requestPayload }
            };

            string payload = JsonConvert.SerializeObject(wrappedPayload, Formatting.None,
                                 new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            HttpResponseMessage response = null;
            string responseString = null;
            BaseApiGetResponse apiResponse = default;

            try
            {
                using (var content = new StringContent(payload, Encoding.UTF8, "application/json"))
                {
                    response = _httpClient.PostAsync(url, content).Result;
                }

                responseString = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    Logging.AddLog($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}. URL: {url} Response: {responseString}");
                    return new BaseApiGetResponse
                    {
                        Code = (int)response.StatusCode,
                        Message = $"HTTP Error: {response.ReasonPhrase}. Response: {responseString}"
                    };
                }

                JObject jsonResponse = JObject.Parse(responseString);
                string apiMessage = jsonResponse["msg"]?.ToString() ?? string.Empty;
                int apiResult = jsonResponse["code"]?.ToObject<int>() ?? -99;

                if (requiresSession && apiResult != 1 && ConfigHelper.IsSessionError(apiMessage))
                {
                    Logging.AddLog($"Session hatası alındı: '{apiMessage}' - RequestKey: {requestKey}");
                    lock (_sessionLock)
                    {
                        if (_sessionId == sessionIdUsed)
                        {
                            _sessionId = null;
                        }
                    }
                }

                apiResponse = JsonConvert.DeserializeObject<BaseApiGetResponse>(responseString)
                              ?? new BaseApiGetResponse { Code = -1, Message = "API yanıtı deserialize edilemedi." };

                // Sadece kritik işlemler için işlem sonrası kontör kontrolü
                if (sessionIdUsed != null && IsCriticalOperation(requestKey))
                {
                    apiResponse.Credit = GetCredit(sessionIdUsed);
                    Logging.AddLog($"İşlem sonrası kontör durumu: {apiResponse.Credit}");

                    if (apiResponse.Credit.HasValue && apiResponse.Credit.Value < -49)
                    {
                        Logging.AddLog($"UYARI: İşlem sonrası kontör yetersiz! Mevcut kontör: {apiResponse.Credit.Value} (minimum -49 olması gerekiyor)");
                    }
                }

                return apiResponse;
            }
            catch (JsonException jsonEx)
            {
                Logging.AddLog($"API yanıtı JSON parse hatası: {jsonEx.Message} Response: {responseString?.Substring(0, Math.Min(responseString?.Length ?? 0, 500))}...");
                return new BaseApiGetResponse { Code = -1, Message = $"JSON Parse Error: {jsonEx.Message}" };
            }
            catch (HttpRequestException httpEx)
            {
                Logging.AddLog($"API isteği sırasında HTTP hatası: {httpEx.Message} URL: {url}");
                return new BaseApiGetResponse { Code = -1, Message = $"HTTP Request Error: {httpEx.Message}" };
            }
            catch (Exception ex)
            {
                Logging.AddLog($"API isteği sırasında genel hata. Hata: {ex.Message} StackTrace: {ex.StackTrace} URL: {url}");
                return new BaseApiGetResponse { Code = -1, Message = $"General Error: {ex.Message}" };
            }
            finally
            {
                response?.Dispose();
            }
        }

        /// <summary>
        /// Kritik işlemleri belirler (kontör kontrolü yapılacak işlemler)
        /// </summary>
        private bool IsCriticalOperation(string requestKey)
        {
            // Kontör kontrolü yapılacak kritik işlemler
            var criticalOperations = new HashSet<string>
            {
                // Cari hesap işlemleri
                "scf_carihesap_fisi_ekle",
                "scf_carikart_ekle",
                "scf_carikart_guncelle",
                
                // Ödeme işlemleri
                "scf_odeme_plani_ekle",
                "scf_banka_odeme_plani_ekle",
                "scf_odeme_plani_guncelle",
                
                // Sistem işlemleri
                "sis_firma_ekle",
                "sis_firma_guncelle",
                "sis_parametreler_ekle",
                "sis_parametreler_guncelle"
            };

            return criticalOperations.Contains(requestKey);
        }

        public void Dispose()
        {
            // static HttpClient dispose edilmemeli
        }
    }
}
