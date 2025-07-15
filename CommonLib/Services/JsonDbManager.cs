using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks; // Asenkron işlemler için

namespace CommonLib
{
    public static class JsonDbManager
    {
        private static readonly string _filePath = "Parameters.json";
        private static readonly object _lockObject = new object();

        public static GlobalParameters GlobalParameters { get; private set; } = new GlobalParameters();

        static JsonDbManager()
        {
            LoadParameters();
        }

        public static void LoadParameters()
        {
            lock (_lockObject)
            {
                try
                {
                    if (!File.Exists(_filePath))
                    {
                        GlobalParameters = new GlobalParameters();
                        SaveParametersToJson();
                        return;
                    }

                    string json = File.ReadAllText(_filePath);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        GlobalParameters = new GlobalParameters();
                    }
                    else
                    {
                        var deserialized = JsonConvert.DeserializeObject<GlobalParameters>(json);
                        GlobalParameters = deserialized != null ? deserialized : new GlobalParameters();
                    }

                    SetDefaultsIfNull();
                    
                    // Config.GlobalParameters'ı da güncelle
                    Config.GlobalParameters = GlobalParameters;
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"Parametreler okunurken hata: {ex.Message}");
                    GlobalParameters = new GlobalParameters();
                    SetDefaultsIfNull();
                    
                    // Config.GlobalParameters'ı da güncelle
                    Config.GlobalParameters = GlobalParameters;
                }
            }
        }

        private static void SetDefaultsIfNull()
        {
            if (GlobalParameters.Parameters == null)
                GlobalParameters.Parameters = new Parameters();

            if (GlobalParameters.GlobalSettings == null)
                GlobalParameters.GlobalSettings = new GlobalSettings();

            if (GlobalParameters.GlobalSettings.CUSTOMERPAYMENTSETS == null)
                GlobalParameters.GlobalSettings.CUSTOMERPAYMENTSETS = new List<CustomerPaymentSet>();

            if (GlobalParameters.Parameters.Firms == null)
                GlobalParameters.Parameters.Firms = new List<Firm> { new Firm() };
        }

        public static void SaveParametersToJson()
        {
            lock (_lockObject)
            {
                try
                {
                    // Config.GlobalParameters'dan güncel veriyi al
                    var parametersToSave = Config.GlobalParameters ?? GlobalParameters;
                    
                    string jsonString = JsonConvert.SerializeObject(parametersToSave, Formatting.Indented);
                    File.WriteAllText(_filePath, jsonString);
                    
                    // JsonDbManager'ın kendi instance'ını da güncelle
                    GlobalParameters = parametersToSave;
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"Parametreler dosyaya kaydedilemedi: {ex.Message}");
                    throw new Exception($"Parametreler dosyaya kaydedilemedi: {ex.Message}", ex);
                }
            }
        }

        public static T LoadFromFile<T>(string filePath) where T : new()
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Dosya yolu boş olamaz.", nameof(filePath));

            lock (_lockObject)
            {
                try
                {
                    if (!File.Exists(filePath))
                    {
                        var defaultInstance = new T();
                        File.WriteAllText(filePath, JsonConvert.SerializeObject(defaultInstance, Formatting.Indented));
                        return defaultInstance;
                    }

                    string json = File.ReadAllText(filePath);
                    if (string.IsNullOrWhiteSpace(json))
                        return new T();

                    var settings = new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    var result = JsonConvert.DeserializeObject<T>(json, settings);
                    return result != null ? result : new T();
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"JSON dosyası okunamadı veya geçersiz formatta. Dosya: '{filePath}'. Hata: {ex.Message}");
                    throw new Exception($"JSON dosyası okunamadı veya geçersiz formatta. Dosya: '{filePath}'. Hata: {ex.Message}", ex);
                }
            }
        }

        public static void SaveToFile<T>(T data, string filePath)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Kaydedilecek veri boş olamaz.");

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Dosya yolu boş olamaz.", nameof(filePath));

            lock (_lockObject)
            {
                try
                {
                    var settings = new JsonSerializerSettings
                    {
                        DateFormatString = "yyyy-MM-dd HH:mm:ss",
                        Formatting = Formatting.Indented
                    };
                    
                    string json = JsonConvert.SerializeObject(data, settings);
                    File.WriteAllText(filePath, json);
                }
                catch (Exception ex)
                {
                    Logging.AddLog($"Veriler dosyaya kaydedilemedi: {ex.Message}");
                    throw new Exception($"Veriler dosyaya kaydedilemedi: {ex.Message}", ex);
                }
            }
        }
    }
}