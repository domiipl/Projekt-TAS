using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjektTAS.Classes;
using System;
using System.Linq;

namespace ProjektTAS.Controllers
{
    [Produces("application/json")]
    [Route("rest/v1/[controller]/[action]")]
    public class ApiController : ControllerBase
    {
        [HttpGet("{Categoryid}")]
        public object GetCategory(int CategoryId)
        {
            if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value) && value.ToString().Contains("Bearer "))
            {
                MySQLObject mySQL = new MySQLObject();
                var data = mySQL.Select($@"select t1.`id_uprawnienia` 
                                                                from `projekt_mysql`.`tokeny_logowania` t0 
                                                                inner join `projekt_mysql`.`uzytkownicy` t1 
                                                                on t0.`id_uzytkownika` = t1.`id_uzytkownika`
                                                                where t0.`token` = '{value.ToString().Replace("Bearer ", "")}' and t0.`aktywny` = 1 and NOW() < t0.`data_wygasniecia`");
                if (data.Rows.Count > 0 && new int[] { 3, 4 }.Contains(Convert.ToInt32(data.Rows[0]["id_uprawnienia"])))
                {
                    data = mySQL.Select($@"select `id`,`nazwa`,`id_parent` from `projekt_mysql`.`kategoria` where `id` = {CategoryId}");
                    if (data.Rows.Count > 0)
                    {
                        int? parentId = null;
                        if (!(data.Rows[0]["id_parent"] is DBNull))
                        {
                            parentId = Convert.ToInt32(data.Rows[0]["id_parent"]);
                        }
                        return new Category() { Id = CategoryId, Name = data.Rows[0]["nazwa"].ToString(), ParentId = parentId };
                    }
                    else
                    {
                        return StatusCode(500, "Something went terribly wrong");
                    }
                }
                else
                {
                    return StatusCode(403, "Method requires administrative privileges or your token is invalid");
                }
            }
            else
            {
                return StatusCode(400, "Wrong request");
            }
        }
        [HttpPost]
        public object CreateProduct([FromBody]Product product)
        {
            if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value) && value.ToString().Contains("Bearer ") && product.Name != null && product.CategoryId != null && product.Price != null)
            {
                MySQLObject mySQL = new MySQLObject();
                var data = mySQL.Select($@"select t1.`id_uprawnienia` 
                                                                from `projekt_mysql`.`tokeny_logowania` t0 
                                                                inner join `projekt_mysql`.`uzytkownicy` t1 
                                                                on t0.`id_uzytkownika` = t1.`id_uzytkownika`
                                                                where t0.`token` = '{value.ToString().Replace("Bearer ", "")}' and t0.`aktywny` = 1 and NOW() < t0.`data_wygasniecia`");
                if (data.Rows.Count > 0 && new int[] { (int)Privileges.User, (int)Privileges.Administrator, (int)Privileges.Moderator }.Contains(Convert.ToInt32(data.Rows[0]["id_uprawnienia"])))
                {
                    try
                    {
                        mySQL.Insert($@"insert into `projekt_mysql`.`przedmiot`(`id_kategorii`,`id_uzytkownika`, `nazwa`,`cena`) values ({product.CategoryId},{StaticMethods.GetUserId(value.ToString().Replace("Bearer ", ""))},'{product.Name}',{product.Price.ToString().Replace(",", ".")})");
                        data = mySQL.Select($@"select max(`id_przedmiotu`) as `value` from `projekt_mysql`.`przedmiot` where `Id_kategorii` = {product.CategoryId} and `nazwa` = '{product.Name}' and `cena` = {product.Price.ToString().Replace(",", ".")}");
                        if (data.Rows.Count > 0)
                        {
                            product.Id = Convert.ToInt32(data.Rows[0]["value"]);
                            product.UserId = StaticMethods.GetUserId(value.ToString().Replace("Bearer ", ""));
                            return product;
                        }
                        else
                        {
                            return StatusCode(500, "Something went terribly wrong");
                        }
                    }
                    catch (Exception)
                    {
                        return StatusCode(500, "Something went terribly wrong");
                    }
                }
                else
                {
                    return StatusCode(403, "Method requires administrative privileges or your token is invalid");
                }
            }
            else
            {
                return StatusCode(400, "Wrong request");
            }
        }

        [HttpPost]
        public object CreateCategory([FromBody]Category category)
        {
            if (Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value) && value.ToString().Contains("Bearer "))
            {
                MySQLObject mySQL = new MySQLObject();
                var data = mySQL.Select($@"select t1.`id_uprawnienia` 
                                                                from `projekt_mysql`.`tokeny_logowania` t0 
                                                                inner join `projekt_mysql`.`uzytkownicy` t1 
                                                                on t0.`id_uzytkownika` = t1.`id_uzytkownika`
                                                                where t0.`token` = '{value.ToString().Replace("Bearer ", "")}' and t0.`aktywny` = 1 and NOW() < t0.`data_wygasniecia`");
                if (data.Rows.Count > 0 && new int[] { 3, 4 }.Contains(Convert.ToInt32(data.Rows[0]["id_uprawnienia"])))
                {
                    if (category.Name != null)
                    {
                        string isNull = category.ParentId == null ? "null" : category.ParentId.ToString();
                        try
                        {
                            mySQL.Insert($@"insert into `projekt_mysql`.`kategoria`(`nazwa`,`id_parent`) values('{category.Name}',{isNull})");
                            data = mySQL.Select($@"select max(`id`) as `value` from `projekt_mysql`.`kategoria` where `nazwa` = '{category.Name}'");
                            if (data.Rows.Count > 0)
                            {
                                category.Id = Convert.ToInt32(data.Rows[0]["value"]);
                                return StatusCode(200, category);
                            }
                            else
                            {
                                return StatusCode(500, "Something went terribly wrong");
                            }
                        }
                        catch (Exception exc)
                        {
                            if (exc is MySqlException)
                            {
                                return StatusCode(400, "Category with that name already exists");
                            }
                            else
                            {
                                return StatusCode(500, "Something went terribly wrong");
                            }
                        }
                    }
                    else
                    {
                        return StatusCode(400, "Wrong request");
                    }
                }
                else
                {
                    return StatusCode(403, "Method requires administrative privileges or your token is invalid");
                }
            }
            else
            {
                return StatusCode(400, "Wrong request");
            }
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
                    return StatusCode(200, "Created user " + user.Login + " successfully");
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
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
    public class Product
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
    }
    public enum Privileges
    {
        User = 2,
        Moderator = 3,
        Administrator = 4
    }
}
