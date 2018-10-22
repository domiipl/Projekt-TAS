using Microsoft.AspNetCore.Mvc;
using ProjektTAS.Classes;
using System;

namespace ProjektTAS.Controllers
{
    [Route("rest/v1/[controller]/[action]")]
    [ApiController]
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
                user.Token = StaticMethods.GenerateToken();
                try
                {
                    mySQL.Insert($@"INSERT INTO `projekt_mysql`.`uzytkownicy`(`login`,`haslo`,`email`,`token`) VALUES('{user.Login}','{user.Password}','{user.Email}','{user.Token}')");
                    return StatusCode(200, user.Token);
                }
                catch(Exception exc)
                {
                    if(exc is MySql.Data.MySqlClient.MySqlException)
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
        public object GetToken([FromBody]LoginAttempt login)
        {
            if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value) && value.ToString() == "client" && login.Login != "" && login.Password != "")
            {
                string token = "";
                MySQLObject mySQL = new MySQLObject(Config.ConnectionString);
                mySQL.Select($@"SELECT `id` FROM `project_mysql`.`uzytkownicy` WHERE `login` = '{login.Login}' AND `haslo` = '{login.Password}'");
                if (mySQL.Data.Rows.Count > 0)
                {
                    token = StaticMethods.GenerateToken();
                    mySQL.Update($@"UPDATE `project_mysql`.`uzytkownicy` SET `token` = '{token}' WHERE `id` = '{mySQL.Data.Rows[0]["id"]}' ");
                    return StatusCode(200, token);
                }
                else
                {
                    return StatusCode(403, "Wrong login/password");
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
        public string Login { get; set; }
        private string _Password { get; set; }
        public string Password
        {
            get => _Password;
            set => _Password = StaticMethods.GeneratePasswordHash(value);
        }
    }
    public class User
    {
        public string Login { get; set; }
        private string _Password { get; set; }
        public string Password { get => _Password; set => _Password = StaticMethods.GeneratePasswordHash(value); }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
