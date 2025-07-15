using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CommonLib
{

    public class Utils
    {

        public static string CreateRePayPlanCode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha256.ComputeHash(bytes);
                return GetStringFromHash(hash).Substring(0, 16);
            }
        }

        private static string GetStringFromHash(byte[] hash)
        {
            if (hash == null || hash.Length == 0)
                return string.Empty;

            StringBuilder result = new StringBuilder(hash.Length * 2);
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        public static string CheckFieldMaximumSize(string strValue, int maximum)
        {
            if (string.IsNullOrEmpty(strValue))
                return string.Empty;

            if (maximum <= 0)
                return string.Empty;

            return strValue.Length > maximum ? strValue.Substring(0, maximum) : strValue;
        }
        public static DateTime StrToDateTime(string strVal, DateTime defaultVal)
        {
            if (string.IsNullOrEmpty(strVal))
                return defaultVal;

            try
            {
                return Convert.ToDateTime(strVal);
            }
            catch
            {
                return defaultVal;
            }
        }

        public static object StrToDateTime(string strVal)
        {
            if (string.IsNullOrEmpty(strVal))
                return null;

            try
            {
                return Convert.ToDateTime(strVal);
            }
            catch
            {
                return null;
            }
        }

        public static string DecryptXML(string xmlPath)
        {
            if (string.IsNullOrEmpty(xmlPath) || !CheckConfigFile(xmlPath))
                return string.Empty;

            try
            {
                using (var sr = new StreamReader(xmlPath))
                {
                    if (sr.BaseStream.Length > 0)
                    {
                        string body = sr.ReadToEnd();
                        return CryptorEngine.Decrypt(body, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"XML decrypt error: {ex.Message}");
            }

            return string.Empty;
        }

        public static void EncryptXML(string innerXml, string xmlPath)
        {
            if (string.IsNullOrEmpty(innerXml) || string.IsNullOrEmpty(xmlPath))
                return;

            try
            {
                string cipherText = CryptorEngine.Encrypt(innerXml, true);
                using (var srDec = new StreamWriter(xmlPath))
                {
                    srDec.Write(cipherText);
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"XML encrypt error: {ex.Message}");
            }
        }

        public static string ReadXMLFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !CheckConfigFile(filePath))
                return string.Empty;

            try
            {
                using (var stringReader = new StreamReader(filePath))
                {
                    if (stringReader.BaseStream.Length > 0)
                    {
                        return stringReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"XML read error: {ex.Message}");
            }

            return string.Empty;
        }

        public static void SaveXMLFile(string innerXml, string xmlPath)
        {
            if (string.IsNullOrEmpty(innerXml) || string.IsNullOrEmpty(xmlPath))
                return;

            try
            {
                using (var srDec = new StreamWriter(xmlPath))
                {
                    srDec.Write(innerXml);
                }
            }
            catch (Exception ex)
            {
                Logging.AddLog($"XML save error: {ex.Message}");
            }
        }

        public static bool CheckConfigFile(string xmlPath)
        {
            if (string.IsNullOrEmpty(xmlPath))
                return false;

            try
            {
                FileInfo fInfo = new FileInfo(xmlPath);
                return fInfo.Exists;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return false;

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logging.AddLog($"Folder check/create error: {ex.Message}");
                return false;
            }
        }

        public static string GetElementValue(XmlElement xmlElement, string elementName)
        {
            if (xmlElement?.SelectSingleNode(elementName) != null)
                return xmlElement.SelectSingleNode(elementName).InnerText;
            return string.Empty;
        }
        public static bool CheckNodeIsEmpty(XmlNode xmlNode, string nodeName)
        {
            if (xmlNode?.SelectSingleNode(nodeName) != null)
                return !string.IsNullOrEmpty(xmlNode.SelectSingleNode(nodeName).InnerText);
            return false;
        }
        public static string GetNodeValue(XmlNode xmlNode, string nodeName)
        {
            if (xmlNode?.SelectSingleNode(nodeName) != null)
                return xmlNode.SelectSingleNode(nodeName).InnerText;
            return string.Empty;
        }
        public static string GetAttribute(XmlNode node, string attributeName)
        {
            if (node?.Attributes?[attributeName] != null)
                return node.Attributes[attributeName].Value;
            
            throw new ArgumentException($"Xml Attribute not found. Attribute Name={attributeName}");
        }

        public static string GetXAttributeValue(XElement product, string tagName)
        {
            if (product?.Attribute(tagName) != null)
                return product.Attribute(tagName).Value.Trim();
            return string.Empty;
        }

        public static string GetXElementValue(XElement product, string tagName)
        {
            if (product?.Element(tagName) != null)
                return product.Element(tagName).Value.Trim();
            return string.Empty;
        }

        public static void AddTextNode(string nodeName, string nodeValue, XmlElement myNode, XmlDocument xmlDoc)
        {
            XmlElement newNode = xmlDoc.CreateElement(nodeName);
            myNode.AppendChild(newNode);

            XmlText txt = xmlDoc.CreateTextNode(nodeValue);
            newNode.AppendChild(txt);
        }
        public static void AddTextNode(string nodeName, string nodeValue, XmlNode myNode, XmlDocument xmlDoc)
        {
            XmlElement newNode = xmlDoc.CreateElement(nodeName);
            myNode.AppendChild(newNode);

            XmlText txt = xmlDoc.CreateTextNode(nodeValue);
            newNode.AppendChild(txt);
        }
        public static void AddAttribute(string attName, string attValue, XmlNode myNode, XmlDocument xmlDoc)
        {
            XmlAttribute myAttr = xmlDoc.CreateAttribute(attName);
            myAttr.Value = attValue;
            myNode.Attributes.Append(myAttr);
        }

        public static string CreatePaymentVoucherNumber(int paymentId)
        {
            return "NT" + (paymentId + 100000).ToString();
        }
        public static string CreatePaymentCQPNNumber(int paymentId)
        {
            return "NT" + (paymentId + 100000).ToString();
        }

        public static string CreateReversalVoucherNumber(int reversalId)
        {
            return "NTIADE" + (reversalId + 100000).ToString();
        }

        public static string CreateLogFolder()
        {
            return FillPreZeros(DateTime.Now.Month, 2) + "-" + DateTime.Now.Year.ToString();
        }

        public static bool CheckLogDir(string logDir)
        {
            bool ret = true;

            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(logDir);

                if (!dInfo.Exists)
                {
                    dInfo.Create();
                }
            }
            catch
            {
                ret = false;
            }

            return ret;
        }

        static public void ConReWrite(string msg)
        {
            Console.CursorLeft = 0;

            Console.Write("                    ");
            Console.CursorLeft = 0;
            Console.Write(msg);
        }

        static public string GetExceptionMsg(Exception ex)
        {
            string strMsg = "";
            BuildExceptionMsg(ref strMsg, ex);
            return strMsg;
        }

        static private void BuildExceptionMsg(ref string msg, Exception ex)
        {
            msg += ex.Message;
            if (ex.InnerException != null)
            {
                msg += "\r\n";
                BuildExceptionMsg(ref msg, ex.InnerException);
            }
        }

        //public static string GetSiteDomainName()
        //{
        //    string host = new Uri(GlobalSettings.ERP_SERVICE).Host;

        //    var nodes = host.Split('.');
        //    int startNode = 0;
        //    if (nodes[0] == "www") startNode = 1;

        //    return nodes[startNode];
        //}

        public static string FillPreZeros(int intValue, int strLength)
        {
            return FillPreZeros(intValue.ToString(), strLength);
        }

        public static string FillPreZeros(string strValue, int strLength)
        {
            return FixStr(strValue, strLength, "0", 2);
        }

        public static string FixStr(string strValue, int strLength, string fixChar, int fixDirection)
        {
            string res = strValue;
            if (fixDirection == 1) for (int i = strValue.Length; i < strLength; i++) res = res + fixChar;
            else for (int i = strValue.Length; i < strLength; i++) res = fixChar + res;
            return res;
        }

        public static bool IsDateTime(string strDate)
        {
            bool result = true;
            try
            {
                System.Convert.ToDateTime(strDate);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static int StrToInt(string strVal, int defaultVal)
        {
            if (int.TryParse(strVal, out int value))
                return value;
            else
                return defaultVal;
        }

        public static decimal StrToDecimal(string strVal, decimal defaultVal = 0)
        {
            if (string.IsNullOrWhiteSpace(strVal))
                return defaultVal;

            strVal = strVal.Trim();

            strVal = strVal.Replace(",", ".");

            if (decimal.TryParse(strVal, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }

            return defaultVal;
        }

        public static float StrToFloat(string strVal, float defaultVal)
        {
            float result = defaultVal;
            try
            {
                result = System.Convert.ToSingle(strVal);
            }
            catch (Exception)
            {
                result = defaultVal;
            }
            return result;
        }

        public static bool StrToBoolean(string strVal, bool defaultVal)
        {
            bool result = defaultVal;
            try
            {
                result = System.Convert.ToBoolean(strVal);
            }
            catch (Exception)
            {
                result = defaultVal;
            }
            return result;
        }

        public static decimal StrToDecimal(string strVal, int precision, decimal defaultVal)
        {
            decimal result = defaultVal;
            try
            {
                result = System.Convert.ToDecimal(strVal);
                result = Math.Round(result, precision);
            }
            catch (Exception)
            {
                result = defaultVal;
            }
            return result;
        }

        public static Int16 StrToInt16(string strVal, Int16 defaultVal)
        {
            Int16 result = defaultVal;
            try
            {
                result = Convert.ToInt16(strVal);
            }
            catch (Exception)
            {
                result = defaultVal;
            }
            return result;
        }

        public static int Asc(char c)
        {
            return Asc(c.ToString());
        }

        public static int Asc(string s)
        {
            byte[] binData;
            //binData = Encoding.ASCII.GetBytes(str);
            binData = Encoding.GetEncoding(1254).GetBytes(s);
            return binData[0];
        }

        public static char Chr(int i)
        {
            return Chr(System.Convert.ToByte(i));
        }

        public static char Chr(byte b)
        {
            byte[] binData = new byte[1];
            binData[0] = b;
            //binData = System.Convert.ToByte(str,10);

            //binData = Encoding.ASCII.GetBytes(str);
            string res = Encoding.GetEncoding(1254).GetString(binData);

            return res[0];
        }

        //Ver 1.1 : Girilen string karakterden Türkçe karakterleri ve çeşitli karakterleri değiştirirek yeni bir string oluşturur.
        public static string FixInvalidChar(string myValue)
        {
            string fixedValue = myValue;
            fixedValue = fixedValue.Replace('ö', 'o');
            fixedValue = fixedValue.Replace('Ö', 'O');
            fixedValue = fixedValue.Replace('ü', 'u');
            fixedValue = fixedValue.Replace('Ü', 'U');
            fixedValue = fixedValue.Replace('ı', 'i');
            fixedValue = fixedValue.Replace('İ', 'I');
            fixedValue = fixedValue.Replace('ş', 's');
            fixedValue = fixedValue.Replace('Ş', 'S');
            fixedValue = fixedValue.Replace('ç', 'c');
            fixedValue = fixedValue.Replace('Ç', 'C');
            fixedValue = fixedValue.Replace('ğ', 'g');
            fixedValue = fixedValue.Replace('Ğ', 'G');
            fixedValue = fixedValue.Replace('#', '_');
            fixedValue = fixedValue.Replace('?', '_');
            fixedValue = fixedValue.Replace('!', '_');
            fixedValue = fixedValue.Replace('#', '_');
            fixedValue = fixedValue.Replace('+', '_');
            fixedValue = fixedValue.Replace(',', '_');
            fixedValue = fixedValue.Replace('%', '_');
            fixedValue = fixedValue.Replace('/', '_');
            fixedValue = fixedValue.Replace('=', '_');
            fixedValue = fixedValue.Replace('-', '_');
            fixedValue = fixedValue.Replace('>', '_');
            fixedValue = fixedValue.Replace('<', '_');
            fixedValue = fixedValue.Replace('|', '_');
            fixedValue = fixedValue.Replace(':', '_');
            fixedValue = fixedValue.Replace(';', '_');
            fixedValue = fixedValue.Replace('.', '_');
            fixedValue = fixedValue.Replace('[', '_');
            fixedValue = fixedValue.Replace(']', '_');
            fixedValue = fixedValue.Replace('{', '_');
            fixedValue = fixedValue.Replace('}', '_');
            fixedValue = fixedValue.Replace('\\', '_');
            fixedValue = fixedValue.Replace('"', '_');
            fixedValue = fixedValue.Replace(' ', '_');
            fixedValue = fixedValue.Replace('&', '_');
            fixedValue = fixedValue.Replace('~', '_');
            fixedValue = fixedValue.Replace('¨', '_');
            fixedValue = fixedValue.Replace('`', '_');
            fixedValue = fixedValue.Replace('@', '_');
            fixedValue = fixedValue.Replace('"', '_');
            fixedValue = fixedValue.Replace('é', '_');
            fixedValue = fixedValue.Replace('€', '_');
            return fixedValue;
        }

        public static string CalcPriodPrice(decimal ProccessAmount, int Period, ref string periodPrice)
        {
            string retVal = "";
            if (decimal.Round(ProccessAmount / Period, 2) * Period != ProccessAmount)
            {
                periodPrice = decimal.Round(ProccessAmount / Period, 2).ToString().Replace("M", "").Replace(",", ".");
                decimal price = Convert.ToDecimal(periodPrice.Replace(".", ","));
                retVal = (price + (ProccessAmount - (price * Period))).ToString().Replace("M", "").Replace(",", "."); ;
            }
            else
            {
                periodPrice = (decimal.Round(ProccessAmount / Period, 2)).ToString().Replace("M", "").Replace(",", ".");
                retVal = periodPrice;
            }
            return retVal;

        }

        public static string CreateHashedTCKN(string tckn)
        {
            byte[] bytes = Encoding.GetEncoding(1026).GetBytes(tckn);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}

