using System;
using System.Collections.Generic;
using System.Data;
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
    public static class StaticMethods
    {
        /// <summary>
        /// Do generowania tokenów
        /// </summary>
        private const string _Chars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+{}|;:,<.>/?|";
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
        internal static string GenerateToken() => StringToBase64(new string(Enumerable.Repeat(_Chars, 8).Select(s => s[new Random().Next(s.Length)]).ToArray()));
        /// <summary>
        /// Generowanie saltu do hashowania hasła
        /// </summary>
        /// <returns>salt w formie stringa</returns>
        internal static string GenerateSalt() => StringToBase64(new string(Enumerable.Repeat(_Chars, 24).Select(s => s[new Random().Next(s.Length)]).ToArray()));
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
        /// <summary>
        /// Wyselectuj id użytkownika bazując na tokenie
        /// </summary>
        /// <param name="login">token</param>
        /// <returns>id użytkownika</returns>
        internal static int? GetUserId(string token)
        {
            int? id = null;
            MySQLObject mySQL = new MySQLObject();
            var data = mySQL.Select("select `id_uzytkownika` from `tokeny_logowania` where `token` = '" + token.Replace("\"","") + "'");
            if (data.Rows.Count > 0)
            {
                id = Convert.ToInt32(data.Rows[0]["id_uzytkownika"]);
            }
            return id;
        }

        /// <summary>
        /// Przeparsowanie danych z selecta do łatwiejszej do obrobienia formy
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>Asynchroniczny enumerable na podstawie wierszy, w pierwszym wierszu są podane kolumny, w kolejnych wierszach dane</returns>
        public static IAsyncEnumerable<object[]> ParseSelect(DataTable dt)
        {
            List<object[]> rows = new List<object[]>();
            List<object> columns = new List<object>();
            dt.Columns.OfType<DataColumn>().ToList().ForEach(x => columns.Add(x.ColumnName));
            rows.Add(columns.ToArray());
            dt.Rows.OfType<DataRow>().ToList().ForEach(x =>
            {
                List<object> column = new List<object>();
                foreach (object item in x.ItemArray)
                {
                    if (item is DBNull)
                        column.Add(null);
                    else column.Add(item);
                }
                rows.Add(column.ToArray());
            });
            return rows.ToAsyncEnumerable();
        }

        public static bool IsTokenValid(string token)
        {
            MySQLObject mySQL = new MySQLObject();
            var data = mySQL.Select("select `id_uzytkownika` from `tokeny_logowania` where `token` = '" + token.Replace("\"","") + "' and `aktywny` = 1");
            if (data.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string GetUserName(string token)
        {
            MySQLObject mySQL = new MySQLObject();
            var data = mySQL.Select("select t1.`login` " +
                "                    from `tokeny_logowania` t0 " +
                "                    inner join `uzytkownicy` t1 on t0.`id_uzytkownika` = t1.`id_uzytkownika` " +
                "                    where t0.`token` = '" + token.Replace("\"","") + "'");
            if (data.Rows.Count > 0)
                return data.Rows[0]["login"].ToString();
            return "";
        }
    }
}
