using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjektTAS.Classes;

namespace ProjektTAS.Controllers
{
    /// <summary>
    /// Kontroler kategorii
    /// GET rest/v1/category/get/{id}
    ///     Wymaga headera Authorization
    ///     Wymaga od użytkownika bycia moderatorem lub administratorem
    ///     
    /// POST rest/v1/category/create
    ///     Wymaga headera Authorization
    ///     Wymaga od użytkownika bycia moderatorem lub administratorem
    ///     {
    ///         "Name" : string required,
    ///         "ParentId" : int
    ///     }
    /// </summary>
    [Produces("application/json")]
    [Route("rest/v1/[controller]/[action]")]
    public class CategoryController : Controller
    {
        [HttpGet("{Categoryid}")]
        public object Get(int CategoryId)
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
                        return StatusCode(500, @"{""Result"" : ""Something went terribly wrong""}");
                    }
                }
                else
                {
                    return StatusCode(403, @"{""Result"" : ""Method requires administrative privileges or your token is invalid""}");
                }
            }
            else
            {
                return StatusCode(400, @"{""Result"" : ""Wrong request""}");
            }
        }

        [HttpPost]
        public object Create([FromBody]Category category)
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
    }
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
