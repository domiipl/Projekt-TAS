﻿using Microsoft.AspNetCore.Mvc;
using ProjektTAS.Classes;
using System;

namespace ProjektTAS.Controllers
{
    [Produces("application/json")]
    [Route("rest/v1/[controller]/[action]")]
    public class ApiController : ControllerBase
    {
        [HttpGet]
        public object GetUsers()
        {
            if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value) && value.ToString() == "")
            {
                return StatusCode(200, "Ok");
            }
            return StatusCode(403, "Method requires administrative privileges");
        }

        [HttpPost]
        public object CreateUser([FromBody]User user)
        {
            if (user.Login != null && user.Password != null && user.Email != null)
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
                    return StatusCode(200, "Created user " + user.Login + " sucessfully");
                }
                catch (Exception exc)
                {
                    if (exc is MySql.Data.MySqlClient.MySqlException)
                    {
                        return StatusCode(400, "Login or email is already taken, try another one");
                    }
                    else
                    {
                        return StatusCode(400, "Wrong request");
                    }
                }
            }
            else
            {
                return StatusCode(400, "Wrong request");
            }
        }

        [HttpPost]
        public object Login([FromBody]LoginAttempt login)
        {
            if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value) && value.ToString() == "client" && login.Login != "" && login.Password != "")
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
                    return StatusCode(403, "Wrong login or password");
                }
            }
            else
            {
                return StatusCode(400, "Wrong request");
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
    }
}
