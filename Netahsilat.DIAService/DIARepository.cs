using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netahsilat.DIAService.Model;

namespace Netahsilat.DIAService
{
    public static class DIARepository
    {
        // Basit memory cache için
        private static readonly Dictionary<string, CacheItem> _cache = new Dictionary<string, CacheItem>();
        private static readonly object _cacheLock = new object();
        private static readonly TimeSpan _defaultCacheExpiration = TimeSpan.FromMinutes(5); // 5 dakika

        private class CacheItem
        {
            public dynamic Data { get; set; }
            public DateTime ExpirationTime { get; set; }
            public bool IsExpired => DateTime.Now > ExpirationTime;
        }

        /// <summary>
        /// Cache'den veri alır, yoksa null döner
        /// </summary>
        private static dynamic GetFromCache(string cacheKey)
        {
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(cacheKey, out var item) && !item.IsExpired)
                {
                    Logging.AddLog($"Cache'den veri alındı: {cacheKey}");
                    return item.Data;
                }
                
                if (item?.IsExpired == true)
                {
                    _cache.Remove(cacheKey);
                }
                
                return null;
            }
        }

        /// <summary>
        /// Veriyi cache'e kaydeder
        /// </summary>
        private static void SetCache(string cacheKey, dynamic data, TimeSpan? expiration = null)
        {
            lock (_cacheLock)
            {
                _cache[cacheKey] = new CacheItem
                {
                    Data = data,
                    ExpirationTime = DateTime.Now.Add(expiration ?? _defaultCacheExpiration)
                };
                Logging.AddLog($"Veri cache'e kaydedildi: {cacheKey}");
            }
        }

        /// <summary>
        /// Cache'i temizler
        /// </summary>
        public static void ClearCache()
        {
            lock (_cacheLock)
            {
                _cache.Clear();
                Logging.AddLog("DIARepository cache temizlendi.");
            }
        }

        /// <summary>
        /// Cache'den belirli bir key'i temizler
        /// </summary>
        public static void RemoveFromCache(string cacheKey)
        {
            lock (_cacheLock)
            {
                if (_cache.Remove(cacheKey))
                {
                    Logging.AddLog($"Cache'den kaldırıldı: {cacheKey}");
                }
            }
        }

        /// <summary>
        /// Cache key oluşturur
        /// </summary>
        private static string CreateCacheKey(string diaPath, BaseApiRequestParams parameters)
        {
            var paramString = parameters != null ? 
                $"_firma{parameters.FirmaKodu}_donem{parameters.DonemKodu}" : "";
            return $"{diaPath}{paramString}";
        }

        /// <summary>
        /// Cache'lenebilir endpoint'ler
        /// </summary>
        private static readonly HashSet<string> _cacheableEndpoints = new HashSet<string>
        {
            DiaEndPoints.Keys.CURRENCY,           // Döviz bilgileri
            DiaEndPoints.Keys.SPECODE,            // Özel kodlar
            DiaEndPoints.Keys.COMPANY,            // Firma bilgileri
            DiaEndPoints.Keys.DIAPARAMETER,       // DIA parametreleri
            DiaEndPoints.Keys.TOPTRANSACTIONTYPE, // Üst işlem türleri
            DiaEndPoints.Keys.EXPENCECENTER,      // Masraf merkezleri
            DiaEndPoints.Keys.AUTHKEY,            // Seviye kodları
            DiaEndPoints.Keys.PROJECTCODE,        // Proje kodları
            DiaEndPoints.Keys.SALESMAN,           // Satış elemanları
            DiaEndPoints.Keys.BANKACCOUNT,        // Banka hesapları
            DiaEndPoints.Keys.VOUCHERNUMBER       // Voucher numaraları
        };

        /// <summary>
        /// Endpoint'in cache'lenebilir olup olmadığını kontrol eder
        /// </summary>
        private static bool IsCacheable(string diaPath)
        {
            return _cacheableEndpoints.Contains(diaPath);
        }



        /// <summary>
        /// Muhtemelen kullanılmayacak ama kod bütünlüğünü korumak için eklendi
        /// </summary>
        /// <param name="diaPath"></param>
        /// <returns></returns>
        public static  Task<dynamic> Post(string diaPath)
        {
            return Post(diaPath, new BaseApiRequestParams());
        }



        public static  dynamic List(string diaPath, BaseApiRequestParams _params = null, string suffix = DiaEndPoints.Suffixes.LIST)
        {
            diaPath += suffix;
            
            // Cache kontrolü
            if (IsCacheable(diaPath))
            {
                var cacheKey = CreateCacheKey(diaPath, _params);
                var cachedData = GetFromCache(cacheKey);
                if (cachedData != null)
                {
                    return cachedData;
                }
            }

            var result = CommonRequest(diaPath, _params ?? new BaseApiRequestParams()).Result;
            
            // Başarılı sonuçları cache'e kaydet
            if (IsCacheable(diaPath) && result != null)
            {
                var cacheKey = CreateCacheKey(diaPath, _params);
                SetCache(cacheKey, result);
            }
            
            return result;
        }

        public static dynamic Post(string diaPath, BaseApiRequestParams _params)
        {
            diaPath += DiaEndPoints.Suffixes.POST;

            return CommonRequest(diaPath,_params ?? new BaseApiRequestParams());
        }

        public static dynamic CommonRequest(string diaPath, BaseApiRequestParams _params = null )
        {
            _params = _params ?? new BaseApiRequestParams();

            using (var apiClient = new DiaApiClient())
            {
                var expandoDict = (IDictionary<string, object>)new ExpandoObject();

                expandoDict[diaPath] = _params;

                BaseApiGetResponse Response = apiClient.SendRequest(
                        diaPath,
                        (BaseApiRequestParams)expandoDict[diaPath],
                        diaPath.Substring(0, 3) + "/json"
                        );

                if (Response != null && Response.IsSuccess)
                {
                    Console.WriteLine($"Fatura Listesi Başarıyla Alındı. ({Response.Count} adet)");
                    Console.WriteLine($"Kalan Kontör: {Response.Credit?.ToString() ?? "Bilgi Yok"}");
                    return Response;
                }
                else
                {
                    Console.WriteLine($"Fatura Listesi Alınamadı. Mesaj: {Response?.Message ?? "Bilinmeyen Hata"}");
                    Console.WriteLine($"Kalan Kontör: {Response?.Credit?.ToString() ?? "Sorgulanamadı"}");
                    return Response;
                }
            }
        }

        public static  dynamic Get(string diaPath)
        {
            return Get(diaPath, new BaseApiRequestParams());
        }
        
        public static  dynamic Get(string diaPath, BaseApiRequestParams _params)
        {
            diaPath += DiaEndPoints.Suffixes.GET;

            // Cache kontrolü
            if (IsCacheable(diaPath))
            {
                var cacheKey = CreateCacheKey(diaPath, _params);
                var cachedData = GetFromCache(cacheKey);
                if (cachedData != null)
                {
                    return cachedData;
                }
            }

            var result = CommonRequest(diaPath, _params).Result;
            
            // Başarılı sonuçları cache'e kaydet
            if (IsCacheable(diaPath) && result != null)
            {
                var cacheKey = CreateCacheKey(diaPath, _params);
                SetCache(cacheKey, result);
            }
            
            return result;
        }
    }
}
