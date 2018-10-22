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
        internal static string ConnectionString { get; private set; } = @"Server=107.6.175.140;Port=3306;Database=projekt_mysql;User Id=tasbackend;Password=TASBACKEND;";
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
        internal static string GenerateToken() => StringToBase64(new string(Enumerable.Repeat(chars, 24).Select(s => s[new Random().Next(s.Length)]).ToArray()));

        /// <summary>
        /// Do zaimplementowania lepszego hashowania haseł
        /// </summary>
        /// <returns>na razie zwraca string w Base64</returns>
        internal static string GeneratePasswordHash(string password) => StringToBase64(password);
    }
}
