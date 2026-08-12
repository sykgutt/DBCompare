using System;
using System.Security.Cryptography;
using System.Text;

namespace DBCompare
{
    /// <summary>
    /// Cifra y descifra secretos con DPAPI (CurrentUser) para persistirlos en INI/XML.
    /// Los valores antiguos en texto plano se aceptan al leer y se cifran en el siguiente guardado.
    /// </summary>
    internal static class CredentialProtector
    {
        private const string Prefix = "DPAPI:";

        public static string Protect(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return plaintext;
            }

            if (plaintext.StartsWith(Prefix, StringComparison.Ordinal))
            {
                return plaintext;
            }

            byte[] data = Encoding.UTF8.GetBytes(plaintext);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            return Prefix + Convert.ToBase64String(encrypted);
        }

        public static string Unprotect(string stored)
        {
            if (string.IsNullOrEmpty(stored))
            {
                return stored;
            }

            if (!stored.StartsWith(Prefix, StringComparison.Ordinal))
            {
                return stored;
            }

            try
            {
                byte[] encrypted = Convert.FromBase64String(stored.Substring(Prefix.Length));
                byte[] data = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(data);
            }
            catch (CryptographicException)
            {
                return stored;
            }
            catch (FormatException)
            {
                return stored;
            }
        }
    }
}
