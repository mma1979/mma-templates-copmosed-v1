using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using WebApplication1.Common.Helpers;

namespace WebApplication1.Common.Extensions
{
    public static class StringExtensions
    {
        static string[] Lang = new string[] { "ar", "en" };

        public static string ToBase64ForUrl(this string input)
        {
            return EncryptionHelper.ToBase64ForUrl(input);
        }

        public static string FromBase64ForUrl(this string input)
        {
            return EncryptionHelper.FromBase64ForUrl(input);
        }

        public static string Base64Encode(this string plainText)
        {
            return EncryptionHelper.Base64Encode(plainText);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            return EncryptionHelper.Base64Decode(base64EncodedData);
        }

        public static string TrimStart(this string value, string trimString)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(trimString) || !value.StartsWith(trimString))
                return value;
            else
                return value.Remove(0, trimString.Length);
        }
        public static string ReplaceWhiteSpaceWith(this string value, string newString)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            else
                return new Regex("\\s+").Replace(value.Trim(), newString);
        }
        public static string ReplaceWithNullIfEmpty(this string value)
        {
            return value.Trim().ReplaceIfNullOrEmpty(null);
        }
        public static string ReplaceIfNullOrEmpty(this string value, string newValue)
        {
            if (string.IsNullOrEmpty(value))
                return newValue;
            else
                return value;
        }
        public static string Encrypt(this string value)
        {
            return EncryptionHelper.Encrypt(value);
        }
        public static string TryEncrypt(this string value)
        {
            try
            {
                return EncryptionHelper.Encrypt(value);
            }
            catch (Exception)
            {

                return value;
            }

        }
        public static string Decrypt(this string value)
        {
            return EncryptionHelper.Decrypt(value);
        }
        public static string ToHashPassword(this string value)
        {
            return "******";
        }
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim().Length == 0;
        }
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
        public static bool IsValidLanguage(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            if (!Lang.Contains(value.ToLower()))
                return false;
            return true;


        }

        public static string ConvertArabicNumbersToEnglish(this string value)
        {
            if (value.IsNullOrEmpty() || value.Trim().Length == 0)
                return value;

            string englishNumber = string.Empty;
            foreach (var character in value)
                englishNumber += char.IsDigit(character) ? char.GetNumericValue(character).ToString() : character.ToString();
            return englishNumber;
        }
        public static string ConvertDateNoToString(this string date, string fformat, string tformat)
        {
            try
            {
                return DateTime.ParseExact(date, fformat, System.Globalization.CultureInfo.InvariantCulture).ToString(tformat);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string GetQueryString(this Dictionary<string, string> queryStrings, string key)
        {
            // IEnumerable<KeyValuePair<string,string>> - right!

            if (queryStrings == null)
                return null;
            if (queryStrings.ContainsKey(key))
            {
                return queryStrings[key];
            }
            else
            {
                return null;
            }

        }

        public static bool IsValidEmail(this string email)
        {

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool ContainsOneOf(this string value, HashSet<string> tokens)
        {

            IEnumerable<bool> res = tokens.Select(t => value.Contains(t));

            return res.Any(e => e == true);
        }

        public static string ExtractPin(this string str)
        {
            var hash = str.GetHashCode().ToString();
            return hash.Substring(hash.Length - 6, 6);
        }

        public static string ExtractPin(this string str, int pinLength)
        {
            var hash = str.GetHashCode().ToString();
            return hash.Substring(hash.Length - pinLength, pinLength);
        }

        public static bool VirifyPin(this string str, string pin)
        {
            var hash = str.GetHashCode().ToString();
            var computedPin = hash.Substring(hash.Length - 6, 6);
            return pin == computedPin;
        }

        public static ulong ToUniqueNumericValue(this string inputText)
        {
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(inputText));

            // Convert the first 8 bytes of the hash to a ulong value
            ulong numericValue = BitConverter.ToUInt64(hashBytes, 0);

            return numericValue;
        }
    }
}
