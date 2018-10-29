using System;
using System.Linq;
using System.Text;

namespace ProjektTAS.Classes
{
    /// <summary>
    /// Główna klasa konfiguracyjna programu
    /// </summary>
    internal static class Config
    {
        /// <summary>
        /// String wymagany do połączenia się z bazą danych
        /// </summary>
        internal const string ConnectionString = @"Server=107.6.175.140;Port=3306;Database=projekt_mysql;User Id=tasbackend;Password=TASBACKEND;";
    }

    /// <summary>
    /// Statyczne metody
    /// </summary>
    internal static class StaticMethods
    {
        /// <summary>
        /// Do generowania tokenów
        /// </summary>
        private const string chars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+{}|;:,<.>/?|";
        /// <summary>
        /// Konwersja stringa zakodowanego w UTF8 na Base64
        /// </summary>
        /// <param name="ValueToConvert">string zakodowany w UTF8</param>
        /// <returns></returns>
        internal static string StringToBase64(string ValueToConvert) => Convert.ToBase64String(Encoding.UTF8.GetBytes(ValueToConvert));
        /// <summary>
        /// Generowanie tokenu uwierzytelniającego
        /// </summary>
        /// <returns>token uwierzytelniający</returns>
        internal static string GenerateToken() => StringToBase64(new string(Enumerable.Repeat(chars, 8).Select(s => s[new Random().Next(s.Length)]).ToArray()));
        /// <summary>
        /// Generowanie saltu do hashowania hasła
        /// </summary>
        /// <returns>salt w formie stringa</returns>
        internal static string GenerateSalt() => StringToBase64(new string(Enumerable.Repeat(chars, 24).Select(s => s[new Random().Next(s.Length)]).ToArray()));
        /// <summary>
        /// Generowanie zahashowanego hasła
        /// </summary>
        /// <returns>Zahashowane hasło na podstawie podanego hasła i salt</returns>
        internal static string GeneratePasswordHash(string password, string salt)
        {
            string password64 = StringToBase64(password);
            return StringToBase64(salt.Substring(0, salt.Length / 4) + password64.Substring(0, password64.Length / 2) + salt.Substring(0, Convert.ToInt32(salt.Length / 2.33)) + password64.Substring(password.Length / 2, password64.Length / 4) + salt.Substring(0, salt.Length - Convert.ToInt32(Math.Floor(salt.Length / 4.5))));
        }
        /// <summary>
        /// Wyselectuj salt użytkownika
        /// </summary>
        /// <param name="login">login użytkownika</param>
        /// <returns>salt użytkownika</returns>
        internal static string GetUserSalt(string login)
        {
            string salt = "";
            MySQLObject mySQL = new MySQLObject();
            var data = mySQL.Select("select `salt` from `uzytkownicy` where `login` = '" + login + "'");
            if (data.Rows.Count > 0)
            {
                salt = data.Rows[0]["salt"].ToString();
            }
            return salt;
        }
    }
}
