using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommonLib.Enum;
using CommonLib.Model;
using Newtonsoft.Json;

public static class Logging
{
    private static readonly object _logLock = new object();
    private static readonly object _jsonLogLock = new object();

    public static void Error(string message, MethodBase methodBase, object data = null)
    {
        AddLog(LogType.Error, message, methodBase, data);
    }

    public static void Warning(string message, MethodBase methodBase, object data = null)
    {
        AddLog(LogType.Warning, message, methodBase, data);
    }

    public static void Info(string message, MethodBase methodBase, object data = null)
    {
        AddLog(LogType.Info, message, methodBase, data);
    }

    public static void Debug(string message, MethodBase methodBase, object data = null)
    {
        AddLog(LogType.Debug, message, methodBase, data);
    }

    public static void AddLog(LogType logType, string message, MethodBase methodBase, object data = null)
    {
        try
        {
            lock (_jsonLogLock)
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string logPath = Path.Combine(basePath, "Log", "GunlukJson");
                Directory.CreateDirectory(logPath);

                string fileName = $"log_{DateTime.Now:ddMMyyyy}.txt";
                string fullPath = Path.Combine(logPath, fileName);
                string logTypeName = EnumExtensions.GetDisplayName(logType);

                var logModel = new LogModel
                {
                    ObjectValue = data,
                    ObjectValueType = data?.GetType(),
                    LogType = logTypeName,
                    Date = DateTime.Now,
                    Message = message,
                    Location = $"{methodBase?.DeclaringType?.Name}.{methodBase?.Name}"
                };

                string jsonData = JsonConvert.SerializeObject(logModel, Formatting.None);

                using (StreamWriter streamWriter = File.AppendText(fullPath))
                {
                    streamWriter.WriteLine("#:# " + jsonData);
                }
            }
        }
        catch (Exception ex)
        {
            try
            {
                lock (_logLock)
                {
                    File.AppendAllText("log_fallback.txt", $"{DateTime.Now} - JSON Log Error: {ex}{Environment.NewLine}");
                }
            }
            catch
            {
                // Son çare - hiçbir şey yapma
            }
        }
    }

    public static void AddLog(string message, LogType logType = LogType.Info)
    {
        try
        {
            lock (_logLock)
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string logDir = Path.Combine(basePath, "Log", "Gunluk");
                Directory.CreateDirectory(logDir);

                string fileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
                string fullPath = Path.Combine(logDir, fileName);

                using (StreamWriter writer = File.AppendText(fullPath))
                {
                    writer.WriteLine($"{DateTime.Now:HH:mm:ss} {message}");
                }

                if (logType == LogType.Error)
                {
                    string errorLogDir = Path.Combine(basePath, "Log", "Hata");
                    Directory.CreateDirectory(errorLogDir);

                    string errorFileName = $"log_hata_{DateTime.Now:yyyyMMdd}.txt";
                    string errorPath = Path.Combine(errorLogDir, errorFileName);

                    using (StreamWriter errorWriter = File.AppendText(errorPath))
                    {
                        errorWriter.WriteLine($"{DateTime.Now:HH:mm:ss} {message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            try
            {
                lock (_logLock)
                {
                    File.AppendAllText("log_fallback.txt", $"{DateTime.Now} - Log Error: {ex}{Environment.NewLine}");
                }
            }
            catch
            {
                // Son çare - hiçbir şey yapma
            }
        }
    }
}
