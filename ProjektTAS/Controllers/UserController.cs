using Microsoft.AspNetCore.Mvc;
using ProjektTAS.Classes;
using System;

namespace ProjektTAS.Controllers
{
    /// <summary>
    /// Kontroler użytkownika
    /// POST rest/v1/user/create
    /// {
    ///     "Login" : string required,
    ///     "Password" : string required,
    ///     "Email" : string required
    /// }
    /// 
    /// POST rest/v1/user/login
    /// {
    ///     "Login" : string required,
    ///     "Password" : string required
    /// }
    /// </summary>
    [Produces("application/json")]
    [Route("rest/v1/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public object Create([FromBody]User user)
        {
            if (user.Login != null && user.Password != null && user.Email != null && user.IsPasswordOk && user.IsLoginOk)
            {
                MySQLObject mySQL = new MySQLObject();
                try
                {
                    mySQL.Insert($@"INSERT INTO `projekt_mysql`.`uzytkownicy`(`login`,`haslo`,`email`,`salt`) 
                                                  VALUES('{user.Login}','{user.Password}','{user.Email}','{user._Salt}')");
                    mySQL.Insert($@"INSERT INTO `projekt_mysql`.`tokeny_logowania`(`id_uzytkownika`,`token`,`aktywny`,`data_wygasniecia`) 
                                                  VALUES((select min(`id_uzytkownika`) from `projekt_mysql`.`uzytkownicy` where `login` = '{user.Login}' and `haslo` = '{user.Password}'),
                                                                  '{StaticMethods.GenerateToken()}',
                                                                  0,
                                                                  NOW())");
                    return StatusCode(200, @"{""Result"" : ""Created user " + user.Login + @" successfully""}");
                }
                catch (Exception exc)
                {
                    if (exc is MySql.Data.MySqlClient.MySqlException)
                    {
                        return StatusCode(403, @"{""Result"" : ""Login or email is already taken, try another one""}");
                    }
                    else
                    {
                        return StatusCode(400, @"{""Result"" : ""Wrong request""}");
                    }
                }
            }
            else if (!user.IsLoginOk || !user.IsPasswordOk)
            {
                return StatusCode(403, @"{""Result"" : ""Login or password did not meet the requirements""}");
            }
            else
            {
                return StatusCode(400, @"{""Result"" : ""Wrong request""}");
            }
        }

        [HttpPost]
        public object Login([FromBody]LoginAttempt login)
        {
            if (login.Login != "" && login.Password != "")
            {
                string token = "";
                MySQLObject mySQL = new MySQLObject(Config.ConnectionString);
                mySQL.Select($@"SELECT `id_uzytkownika` FROM `projekt_mysql`.`uzytkownicy` WHERE `login` = '{login.Login}' AND `haslo` = '{login.Password}'");
                if (mySQL.Data.Rows.Count > 0)
                {
                    token = StaticMethods.GenerateToken();
                    mySQL.Update($@"UPDATE `projekt_mysql`.`tokeny_logowania` SET `token` = '{token}', `aktywny` = 1, `data_wygasniecia` = ADDTIME(NOW(),'02:00:00') WHERE `id_uzytkownika` = '{mySQL.Data.Rows[0]["id_uzytkownika"].ToString()}' ");
                    return StatusCode(200, token);
                }
                else
                {
                    return StatusCode(403, @"{""Result"" : ""Wrong login or password""}");
                }
            }
            else
            {
                return StatusCode(400, @"{""Result"" : ""Wrong request""}");
            }
        }

        [HttpPost]
        public object ChangePassword([FromBody]ChangePasswordAttempt changePasswordAttempt)
        {
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value) && value.ToString().Contains("Bearer "))
                {
                    value = value.ToString().Replace("Bearer ", "");
                    if (changePasswordAttempt.NewPassword == changePasswordAttempt.RepeatedNewPassword
                        && StaticMethods.IsTokenValid(value)
                        && StaticMethods.GeneratePasswordHash(changePasswordAttempt.CurrentPassword, StaticMethods.GetUserSalt(StaticMethods.GetUserName(value))) == StaticMethods.GetUserPassword(value))
                    {
                        MySQLObject mysql = new MySQLObject();
                        string newHash = StaticMethods.GeneratePasswordHash(changePasswordAttempt.NewPassword, StaticMethods.GetUserSalt(StaticMethods.GetUserName(value)));
                        mysql.Update($"update `uzytkownicy` set `haslo` = '{newHash}' where `id_uzytkownika` = '{StaticMethods.GetUserId(value)}' ");
                        return StatusCode(200);
                    }
                    else
                    {
                        return StatusCode(403, @"{""Result"" : ""Wrong data""}");
                    }
                }
                else
                {
                    return StatusCode(403, @"{""Result"" : ""Wrong token or password""}");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public object ChangeEmail([FromBody]ChangeEmailAttempt changeEmailAttempt)
        {
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value) && value.ToString().Contains("Bearer "))
                {
                    value = value.ToString().Replace("Bearer ", "");
                    if (changeEmailAttempt.NewEmail == changeEmailAttempt.RepeatedNewEmail
                        && StaticMethods.IsTokenValid(value)
                        && StaticMethods.GeneratePasswordHash(changeEmailAttempt.CurrentPassword, StaticMethods.GetUserSalt(StaticMethods.GetUserName(value))) == StaticMethods.GetUserPassword(value))
                    {
                        MySQLObject mysql = new MySQLObject();
                        mysql.Update($"update `uzytkownicy` set `email` = '{changeEmailAttempt.NewEmail}' where `id_uzytkownika` = '{StaticMethods.GetUserId(value)}' ");
                        return StatusCode(200);
                    }
                    else
                    {
                        return StatusCode(403, @"{""Result"" : ""Wrong data""}");
                    }
                }
                else
                {
                    return StatusCode(403, @"{""Result"" : ""Wrong token or password""}");
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }

    public class LoginAttempt
    {
        public string Login
        {
            get => _Login;
            set
            {
                _Login = value;
                _Salt = StaticMethods.GetUserSalt(_Login);
            }
        }
        private string _Login { get; set; }
        private string _Password { get; set; }
        private string _Salt { get; set; }
        public string Password
        {
            get => _Password;
            set => _Password = StaticMethods.GeneratePasswordHash(value, _Salt);
        }
    }
    public class User
    {
        public string Login { get; set; }
        private string _Password { get; set; }
        internal string _Salt { get; set; } = StaticMethods.GenerateSalt();
        public string Password { get => _Password; set => _Password = StaticMethods.GeneratePasswordHash(value, _Salt); }
        public string Email { get; set; }
        public bool IsPasswordOk { get => _Password.Length >= 6; }
        public bool IsLoginOk { get => Login.Length >= 6; }
    }
    public enum Privileges
    {
        User = 2,
        Moderator = 3,
        Administrator = 4
    }

    public class ChangePasswordAttempt
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string RepeatedNewPassword { get; set; }
    }
    public class ChangeEmailAttempt
    {
        public string CurrentPassword { get; set; }
        public string NewEmail { get; set; }
        public string RepeatedNewEmail { get; set; }
    }
}
